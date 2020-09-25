using Core.BLL;
using Core.ENT;
using ECF;
using ECF.Data.Query;
using Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Controller.V1.Core
{
    class Order
    {

        public static IResultResponse OrderCheck(SysParameter para)
        {
            try
            {
                int sellerId = para.ToInt("FKId");
                int dbId = para.ToInt("DBId");
                string orderInfo = para.ToValue("ProductInfo");
                int orderId = para.ToInt("OrderId");
                bool result = false;
                if (!String.IsNullOrEmpty(orderInfo) && sellerId != 0)
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(orderInfo);
                    //检测商品
                    var res = new ProductRelationBusiness(para.dapperFactory).CheckProduct(sellerId, dbId, xml);
                    if (!res.Success)
                    {
                        return res;
                    }
                    //检测订单
                    bool check = new OrderRelationBusiness(para.dapperFactory).CheckOrder(sellerId, dbId, orderId);
                    if (check)
                    {
                        return ResultResponse.ExceptionResult("你已经代下过单了");
                    }
                    return ResultResponse.GetSuccessResult(result);
                }
                return ResultResponse.ExceptionResult("订单信息不全");
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }


        #region OrderSubmit
        /// <summary>
        /// 代下单中转
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse OrderSubmit(SysParameter para)
        {
            try
            {
                int sellerId = para.ToInt("FKId");
                int dbId = para.ToInt("DBId");
                string orderInfo = para.ToValue("Order");
                int orderId = para.ToInt("OrderId");
                string oddNumber = para.ToValue("OddNumber");
                decimal downPayables = para.ToDecimal("Payables");
                IResultResponse result = null;
                if (String.IsNullOrEmpty(orderInfo) && sellerId != 0)
                {
                    return ResultResponse.ExceptionResult("订单信息为空");
                }
                RelationMappingEntity relationMappingEntity = new ProductRelationBusiness(para.dapperFactory).GetUpMappingEntity(sellerId, dbId);
                if (relationMappingEntity.GoodsMapping.Count == 0) return ResultResponse.ExceptionResult("没有找到商品映射关系");
                //替换供应商ids
                orderInfo = ReplaceIds(relationMappingEntity.ConnectorMapping, orderInfo, "SupplierIds");
                orderInfo = ReplaceIds(relationMappingEntity.ConnectorMapping, orderInfo, "SupplierId");
                //替换商品ids
                orderInfo = ReplaceIds(relationMappingEntity.ProductMapping, orderInfo, "ProductIds");
                //替换productinfo里面的
                orderInfo = ReplaceIds(relationMappingEntity.ProductMapping, orderInfo, "ProductId");
                orderInfo = ReplaceIds(relationMappingEntity.GoodsMapping, orderInfo, "GoodsId");

                //发送
                result = ApiRequest.GetResponse(relationMappingEntity.ConnectorEntity.Domain, "order.dropshipping.submit", new Dictionary<string, string>() {
                        {"StoreId",relationMappingEntity.ConnectorEntity.UpBuyerId.ToString() },
                        {"Order",orderInfo}
                    });
                if (result.Success)
                {
                    //入库
                    var orderDic = result.Content as Dictionary<string, object>;
                    if (orderDic != null)
                    {
                        new OrderRelationBusiness(para.dapperFactory).Insert(orderId, oddNumber, downPayables, relationMappingEntity.ConnectorEntity, orderDic);
                    }
                    return ResultResponse.GetSuccessResult(orderId);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        #endregion

        #region DispatchSubmit 发货中转
        /// <summary>
        /// 发货中转
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <param name="dispatchId"></param>
        /// <param name="dispatchInfo"></param>
        /// <returns></returns>
        public static IResultResponse DispatchSubmit(SysParameter para)
        {
            IResultResponse result = null;
            int sellerId = para.ToInt("FKId");
            int dbId = para.ToInt("DBId");
            string dispatchInfo = para.ToValue("DispatchInfo");
            int dispatchId = para.ToInt("DispatchId");
            int orderId = para.ToInt("OrderId");
            int upBuyerId = para.ToInt("BuyerId");
            string oddNumber = para.ToValue("OddNumber");
            try
            {
                RelationMappingEntity productMappingEntity = new ProductRelationBusiness(para.dapperFactory).GetDownMappingEntity(sellerId, upBuyerId,dbId);
                if (productMappingEntity.GoodsMapping.Count == 0) return ResultResponse.ExceptionResult("没有找到商品映射关系");

                RelationMappingEntity orderMappingEntity = new OrderRelationBusiness(para.dapperFactory).GetDownOrder(sellerId, dbId, orderId, upBuyerId);

                if (!orderMappingEntity.OrderMapping.ContainsKey(orderId)||!orderMappingEntity.OrderOddNumberMapping.ContainsKey(oddNumber))
                {
                    return ResultResponse.ExceptionResult("没有找到订单映射关系");
                }
                //替换orderid
                dispatchInfo = ReplaceIds(orderMappingEntity.OrderMapping, dispatchInfo, "OrderId");
                //替换商品oddnumbers
                dispatchInfo = ReplaceStr(orderMappingEntity.OrderOddNumberMapping, dispatchInfo, "OddNumber");
                //替换product
                dispatchInfo = ReplaceIds(productMappingEntity.ProductMapping, dispatchInfo, "ProductId");
                dispatchInfo = ReplaceIds(productMappingEntity.GoodsMapping, dispatchInfo, "GoodsId");

                //发送
                result = ApiRequest.GetResponse(productMappingEntity.ConnectorEntity.Domain, "order.dropshippingdispatch.submit", new Dictionary<string, string>() {
                        {"OrderId",orderMappingEntity.OrderMapping[orderId].ToString() },
                        {"DispatchInfo",dispatchInfo}
                    });
                if (result.Success)
                {
                    //入库
                    var orderDic = result.Content as Dictionary<string, object>;
                    if (orderDic != null)
                    {
                        new DispatchRelationBusiness(para.dapperFactory).Insert(dispatchId, orderMappingEntity.OrderRelationEntity, orderDic);
                    }
                    return ResultResponse.GetSuccessResult(dispatchId);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {

                return ResultResponse.ExceptionResult(ex);
            }
        }
        #endregion

        #region OrderReceive 订单确认中转
        /// <summary>
        /// 订单确认中转
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse OrderReceive(SysParameter para)
        {
            IResultResponse result = null;
            int sellerId = para.ToInt("FKId");
            int dbId = para.ToInt("DBId");
            int orderId = para.ToInt("OrderId");
            string oddNumber = para.ToValue("OddNumber");
            int dispatchId = para.ToInt("DispatchId");

            try
            {
                RelationMappingEntity relationMappingEntity = new ProductRelationBusiness(para.dapperFactory).GetUpMappingEntity(sellerId, dbId);
                if (relationMappingEntity.GoodsMapping.Count == 0) return ResultResponse.ExceptionResult("没有找到商品映射关系");

                RelationMappingEntity orderMappingEntity = new OrderRelationBusiness(para.dapperFactory).GetUpOrder(sellerId, dbId, orderId);
                int replaceDispatchId = 0;
                if (dispatchId != 0)
                {
                    if (!orderMappingEntity.DispatchMapping.ContainsKey(dispatchId))
                    {
                        return ResultResponse.GetSuccessResult("没有找到发货单id");
                    }
                    replaceDispatchId = orderMappingEntity.DispatchMapping[dispatchId];
                }
                //发送
                result = ApiRequest.GetResponse(relationMappingEntity.ConnectorEntity.Domain, "order.dropshippingreceive.submit", new Dictionary<string, string>() {
                        {"DispatchId",replaceDispatchId.ToString() },
                        {"OddNumber",orderMappingEntity.OrderOddNumberMapping[oddNumber]},
                     {"OrderId",orderMappingEntity.OrderMapping[orderId].ToString()}
                    });
                if (result.Success)
                {
                    
                    return ResultResponse.GetSuccessResult(dispatchId);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {

                return ResultResponse.ExceptionResult(ex);
            }
        }

        #endregion

        #region OrderPay 订单支付
        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse OrderPay(SysParameter para)
        {
            int sellerId = para.ToInt("FKId");
            int dbId = para.ToInt("DBId");
            int orderId = para.ToInt("OrderId");
            try
            {
                ConnectorRelation connectorEntity = new ConnectorBusiness(para.dapperFactory).GetUpConnector(sellerId, dbId);
                if (connectorEntity.Id == null)
                {
                    return ResultResponse.ExceptionResult("没有找到对应的上游");
                }
                RelationMappingEntity orderMappingEntity = new OrderRelationBusiness(para.dapperFactory).GetUpOrder(sellerId, dbId, orderId);
                if (!orderMappingEntity.OrderMapping.ContainsKey(orderId))
                {
                    return ResultResponse.ExceptionResult("没有找到对应的订单");
                }
                Dictionary<string, string> p = new Dictionary<string, string>()
                {
                    {"domain",connectorEntity.Domain },
                    {"userId",connectorEntity.UpBuyerId.ToString() },
                    {"orderId",orderMappingEntity.OrderMapping[orderId].ToString() },
                };
              
                return ResultResponse.GetSuccessResult(p);
            }
            catch (Exception ex)
            {

                return ResultResponse.ExceptionResult(ex);
            }
        }
        #endregion

        #region OrderPay 订单支付
        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse OrderUpdate(SysParameter para)
        {
            int sellerId = para.ToInt("FKId");
            int dbId = para.ToInt("DBId");
            int orderId = para.ToInt("OrderId");
            int upBuyerId = para.ToInt("BuyerId");
            try
            {
                ConnectorRelation connectorEntity = new ConnectorBusiness(para.dapperFactory).GetDownConnector(sellerId, upBuyerId, dbId);
                if (connectorEntity.Id == null)
                {
                    return ResultResponse.ExceptionResult("没有找到对应的下游");
                }
                RelationMappingEntity orderMappingEntity = new OrderRelationBusiness(para.dapperFactory).GetDownOrder(sellerId, dbId, orderId,upBuyerId);
                if (!orderMappingEntity.OrderMapping.ContainsKey(orderId))
                {
                    return ResultResponse.ExceptionResult("没有找到对应的订单");
                }

                new OrderRelationBusiness(para.dapperFactory).UpdateUpOrder(connectorEntity, para.ToDictionary());
               
                return ResultResponse.GetSuccessResult(200);
            }
            catch (Exception ex)
            {

                return ResultResponse.ExceptionResult(ex);
            }
        }
        #endregion

        #region Replace 替换字段
        /// <summary>
        /// 替换id
        /// </summary>
        /// <param name="idsDic"></param>
        /// <param name="paramValue"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ReplaceIds(Dictionary<int, int> idsDic, string paramValue, string name)
        {
            Regex reg = new Regex("<" + name + ".*?>(?<id>[\\d,]+)</" + name + ">");

            return reg.Replace(paramValue, delegate (Match match)
            {
                string[] ids = Utils.ToString(match.Groups["id"].Value).Split(',');
                List<int> newIds = new List<int>();
                foreach (string s in ids)
                {
                    int id = Utils.ToInt(s);
                    if (idsDic.ContainsKey(id))
                    {
                        newIds.Add(idsDic[id]);
                    }
                }
                string v = match.Value;
                v = v.Replace(match.Groups["id"].Value, newIds.Join(","));
                return v;
            });
        }
        /// <summary>
        /// 替换字符串
        /// </summary>
        /// <param name="idsDic"></param>
        /// <param name="paramValue"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ReplaceStr(Dictionary<string, string> idsDic, string paramValue, string name)
        {
            Regex reg = new Regex("<" + name + ".*?>(?<id>[\\d,]+)</" + name + ">");

            return reg.Replace(paramValue, delegate (Match match)
            {
                string ids = Utils.ToString(match.Groups["id"].Value);
                string newIds = string.Empty;
                if (idsDic.ContainsKey(ids))
                {
                    newIds = idsDic[ids];
                }
                string v = match.Value;
                v = v.Replace(match.Groups["id"].Value, newIds);
                return v;
            });
        }
        #endregion

        public static IResultResponse OrderList(SysParameter para)
        {
            try
            {
                PagingQuery query = para.ToPagingQuery();
                int connectorId = para.ToInt("ConnectorId");
                int status = para.ToInt("Status", 0);
                PagingResult result = new OrderRelationBusiness(para.dapperFactory).GetOrderList(query, connectorId, status);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }

        public static IResultResponse GetOrderInfo(SysParameter para)
        {
            try
            {
                int connectorId = para.ToInt("ConnectorId");
                string oddNumber = para.ToValue("OddNumber");
                return new OrderRelationBusiness(para.dapperFactory).GetOrderInfo(connectorId, oddNumber);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
    }
}
