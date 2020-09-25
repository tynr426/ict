using Core.ENT;
using ECF;
using ECF.Data;
using ECF.Data.Query;
using Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Core.BLL
{
    public class OrderRelationBusiness : CoreBusiness
    {

        public OrderRelationBusiness(IDapperFactory dapperFactory) : base(dapperFactory)
        {

        }

        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "Order_Relation";

        /// <summary>
        /// 获取数据库表
        /// </summary>
        public override string TableName
        {
            get
            {
                return _TableName;
            }
        }

        /// <summary>
        /// 后期通过继承类接口而实现的数据实体接口
        /// </summary>
        public override ECF.IEntity Entity
        {
            get
            {
                return new AccountEntity();
            }
        }
        #endregion

        #region Insert 插入订单关联信息
        /// <summary>
        /// 插入订单关联信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="oddNumber"></param>
        /// <param name="ConnectorEntity"></param>
        /// <param name="orderDic"></param>
        /// <returns></returns>
        public int Insert(int orderId, string oddNumber, decimal downPayables, ConnectorRelation ConnectorEntity, Dictionary<string, object> orderDic)
        {
            int upOrderId = orderDic.ToInt("OrderId", 0);
            string upOddNumber = orderDic.ToValue("OddNumber");
            decimal upPayables = Utils.ToDecimal(orderDic.ToValue("Payables"));
            if (upOrderId == 0) throw new Exception("代下单失败");
            OrderRelationEntity orderRelationEntity = new OrderRelationEntity()
            {
                UpConnectorId = ConnectorEntity.Id,
                UpOrderId = upOrderId,
                UpOddNumber = upOddNumber,
                UpPayables = upPayables,
                DownConnectorId = ConnectorEntity.ThirdConnectorId,
                DownOrderId = orderId,
                DownOddNumber = oddNumber,
                DownPayables = downPayables,

            };
            return DbAccess.ExecuteUpsert(TableName, orderRelationEntity, new string[] { "UpConnectorId", "UpOrderId", "DownConnectorId", "DownOrderId" });
        }
        #endregion

        #region UpdateUpOrder 修改上游订单
        /// <summary>
        /// 修改上游订单
        /// </summary>
        /// <param name="ConnectorEntity"></param>
        /// <param name="orderDic"></param>
        /// <returns></returns>
        public int UpdateUpOrder(ConnectorRelation ConnectorEntity, Dictionary<string, object> orderDic)
        {
            int upOrderId = orderDic.ToInt("OrderId", 0);
            decimal upPayables = Utils.ToDecimal(orderDic.ToValue("Payables"));
            if (upOrderId == 0) throw new Exception("代下单失败");
            OrderRelationEntity orderRelationEntity = new OrderRelationEntity()
            {
                UpConnectorId = ConnectorEntity.ThirdConnectorId,
                UpOrderId = upOrderId,
                UpPayables = upPayables,
                DownConnectorId = ConnectorEntity.Id,

            };
            return DbAccess.ExecuteUpdate(TableName, orderRelationEntity, new string[] { "UpConnectorId", "UpOrderId", "DownConnectorId" });
        } 
        #endregion

        #region GetRelationMappingEntity
        /// <summary>
        /// 获得下游
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public RelationMappingEntity GetDownOrder(int sellerId, int dbId,int orderId, int upBuyerId)
        {
            RelationMappingEntity relationMappingEntity = new RelationMappingEntity();
            ConnectorRelation connectorEntity = new ConnectorBusiness(_dapperFactory).GetDownConnector(sellerId,upBuyerId, dbId);
            if (connectorEntity.Id == null)
            {
                return relationMappingEntity;
            }

            relationMappingEntity.ConnectorMapping.Add(sellerId, Utils.ToInt(connectorEntity.SellerId));

            relationMappingEntity.ConnectorEntity = connectorEntity;

            DataTable data = DBTable($"UpConnectorId={connectorEntity.ThirdConnectorId} and DownConnectorId={connectorEntity.Id} and UpOrderId={orderId}");

            if (data == null || data.Rows.Count == 0)
            {
                return relationMappingEntity;
            }
           

            foreach (DataRow dr in data.Rows)
            {
                OrderRelationEntity orderRelationEntity = new OrderRelationEntity();
                orderRelationEntity.SetValues(dr);

                relationMappingEntity.OrderRelationEntity = orderRelationEntity;

                int upOrderId = Utils.ToInt(orderRelationEntity.UpOrderId);
                int downOrderId = Utils.ToInt(orderRelationEntity.DownOrderId);
                string upOddNumber = orderRelationEntity.UpOddNumber;
                string downOddNumber = orderRelationEntity.DownOddNumber;

                if (!relationMappingEntity.OrderMapping.ContainsKey(upOrderId))
                {
                    relationMappingEntity.OrderMapping.Add(upOrderId, downOrderId);
                }
                if (!relationMappingEntity.OrderOddNumberMapping.ContainsKey(upOddNumber))
                {
                    relationMappingEntity.OrderOddNumberMapping.Add(upOddNumber, downOddNumber);
                }

                new DispatchRelationBusiness(_dapperFactory).GetOrderDispatch(relationMappingEntity);
            }


            return relationMappingEntity;
        }
        /// <summary>
        /// 获得上游
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public RelationMappingEntity GetUpOrder(int sellerId, int dbId, int orderId)
        {
            RelationMappingEntity relationMappingEntity = new RelationMappingEntity();
            ConnectorRelation connectorEntity = new ConnectorBusiness(_dapperFactory).GetUpConnector(sellerId, dbId);
            if (connectorEntity.Id == null)
            {
                return relationMappingEntity;
            }

            relationMappingEntity.ConnectorMapping.Add(sellerId, Utils.ToInt(connectorEntity.SellerId));

            relationMappingEntity.ConnectorEntity = connectorEntity;

            DataTable data = DBTable($"UpConnectorId={connectorEntity.Id} and DownConnectorId={connectorEntity.ThirdConnectorId} and DownOrderId={orderId}");

            if (data == null || data.Rows.Count == 0)
            {
                return relationMappingEntity;
            }

            foreach (DataRow dr in data.Rows)
            {
                OrderRelationEntity orderRelationEntity = new OrderRelationEntity();
                orderRelationEntity.SetValues(dr);

                relationMappingEntity.OrderRelationEntity = orderRelationEntity;

                int upOrderId = Utils.ToInt(orderRelationEntity.UpOrderId);
                int downOrderId = Utils.ToInt(orderRelationEntity.DownOrderId);
                string upOddNumber = orderRelationEntity.UpOddNumber;
                string downOddNumber = orderRelationEntity.DownOddNumber;

                if (!relationMappingEntity.OrderMapping.ContainsKey(downOrderId))
                {
                    relationMappingEntity.OrderMapping.Add(downOrderId, upOrderId);
                }
                if (!relationMappingEntity.OrderOddNumberMapping.ContainsKey(downOddNumber))
                {
                    relationMappingEntity.OrderOddNumberMapping.Add(downOddNumber, upOddNumber);
                }
                new DispatchRelationBusiness(_dapperFactory).GetOrderDispatch(relationMappingEntity);
            }
            return relationMappingEntity;
        }
        #endregion

        #region CheckOrder 检查订单
        /// <summary>
        /// 检查订单
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CheckOrder(int sellerId, int dbId, int orderId)
        {
            RelationMappingEntity relationMappingEntity = GetUpOrder(sellerId, dbId, orderId);
            return relationMappingEntity.OrderMapping.ContainsKey(orderId);
        } 
        #endregion


        #region 代发订单列表
        public PagingResult GetOrderList(PagingQuery pagingQuery, int connectorId, int status = 0)
        {
            PagingResult result = new PagingResult();
            result.PageIndex = pagingQuery.PageIndex;
            result.PageSize = pagingQuery.PageSize;
            string condition = "1=1";
            string field = string.Empty;
            if (status == 0)
            {
                //我的下游合作商订单
                condition += string.Format(" and a.UpConnectorId={0}", connectorId);
                field = "a.DownConnectorId=b.Id";
            }
            else
            {
                //我的上游合作商订单
                condition += string.Format(" and a.DownConnectorId={0}", connectorId);
                field = "a.UpConnectorId=b.Id";
            }

            //遍历所有子节点
            foreach (Condition c in pagingQuery.Condition)
            {
                if (!string.IsNullOrEmpty(c.Value))
                {
                    switch (c.Name.ToLower())
                    {

                        case "keyword":
                            condition += string.Format(" and (a.UpOddNumber like '%{0}%' or a.DownOddNumber like '%{0}%')", c.Value);
                            break;
                        //case "upoddnumber":
                        //    condition += c.ToWhereString("a.UpOddNumber");
                        //    break;
                        //case "downoddnumber":
                        //    condition += c.ToWhereString("a.DownOddNumber");
                        //    break;
                    }
                }
            }

            string sql = string.Format("select count(0) from {0} a inner join {1} b on {2} where {3} ;select b.Name,a.Id,a.UpConnectorId,a.DownConnectorId,a.UpOddNumber,a.UpPayables,a.DownOddNumber,a.DownPayables,a.AddTime from {0} a inner join {1} b on {2}  where {3} order by b.AddTime desc limit {4},{5};",
                                        TableName, ConnectorBusiness._TableName, field, condition, pagingQuery.PageSize * (pagingQuery.PageIndex - 1), pagingQuery.PageSize);
            DataSet ds = DbAccess.ExecuteDataset(sql);
            if (ds == null || ds.Tables.Count != 2) return result;
            result.TotalCount = Utils.ToInt(ds.Tables[0].Rows[0][0]);
            result.Data = ds.Tables[1];
            return result;
        }
        #endregion

        #region
        public  IResultResponse GetOrderInfo(int connectorId,string oddNumber) {
            ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness(_dapperFactory).GetEntity("Id=" + connectorId);
            if (connector == null || connector.Id == null)
            {
                return ResultResponse.ExceptionResult("下游不存在");
            }
            GroupEntity group = (GroupEntity)new GroupBusiness(_dapperFactory).GetEntity("Id=" + connector.GroupId);
            if (group == null || group.Id == null)
            {
                return ResultResponse.ExceptionResult("上游不存在");
            }
            Dictionary<string, string> dic = new Dictionary<string, string>() {
                        {"OddNumber",oddNumber },
                        {"FromFKId",connector.SellerId.ToString() },
                        {"FromFKFlag","2" },
                        {"IsBuyer","False" },
                    };
            string msg = "";
            string datajson = ApiRequest.GetRemoteContent(group.Domain + "/Route.axd", "order.trade.getorderinfo", dic, out msg);

            var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(datajson);
            if (!Utils.ToBool(dict["Success"]))
            {
                return ResultResponse.ExceptionResult(msg);
            }
            ArrayList content = dict["Content"] as ArrayList;
            return ResultResponse.GetSuccessResult(content);
        }
        #endregion
    }
}
