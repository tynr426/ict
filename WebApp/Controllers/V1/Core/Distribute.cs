
using ECF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.V1.Core
{
    
    public class Distribute : API
    {
        /// <summary>
        /// 获得接口请求结果
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public override IResultResponse GetResultResponse(SysParameter para)
        {
            IResultResponse result = null;
            string method = para.Method.Replace("core.", "");
            switch (method)
            {
                case "account.list":
                    result = Account.Get(para);
                    break;
                case "account.add":
                    result = Account.Add(para);
                    break;
                case "account.delete":
                    result = Account.Delete(para);
                    break;
                case "account.update":
                    result = Account.Update(para);
                    break;
                #region 合作商
                case "connector.list":
                    result = Connector.GetList(para);
                    break;
                case "connector.add":
                    result = Connector.Insert(para);
                    break;
                case "connector.delete":
                    result = Connector.Delete(para);
                    break;
                case "connector.update":
                    result = Connector.Update(para);
                    break;
                case "partner.mylist":
                    result = Connector.GetMyList(para);
                    break;
                case "partner.newlist":
                    result = Connector.GetNewList(para);
                    break;
                case "partner.downconnectorstock":
                    result = Connector.DownConnectorStock(para);
                    break;
                case "partner.adduprelation":
                    result = Connector.AddUpRelation(para);
                    break;
                case "partner.adddownrelation":
                    result = Connector.AddDownRelation(para);
                    break;
                case "partner.changerelation":
                    result = Connector.CheckConnector(para);
                    break;
                case "partner.deleterelation":
                    result = Connector.DeleteRelation(para);
                    break;
                //网站克隆
                case "partner.clone.site":
                    result = Connector.PushCloneCommand(para);
                    break;
                //清除克隆数据
                case "partner.clear.clone.data":
                    result = Connector.ClearCloneData(para);
                    break;
                //是否有克隆权限
                case "partner.clone.authority":
                    result = Connector.CloneAuthority(para);
                    break;
                //授权登录
                case "auth.sso.login":
                    result = Connector.AuthLogin(para);
                    break;
                //根据qr获取连接者内容
                case "partner.get.connector.byqr":
                    result = Connector.GetConnectorByQr(para);
                    break;
                case "partner.add.checkdownconnector":
                    result = Connector.CheckDownConnector(para);
                    break;
                case "partner.add.getscmcode":
                    result = Connector.GetScmCode(para);
                    break;
                case "partner.add.validatecode":
                    result = Connector.ValidateCode(para);
                    break;
                case "partner.add.getvalidatecode":
                    result = Connector.GetValidateCode(para);
                    break;
                //全站克隆权限
                case "partner.isopen":
                    result = Connector.ChangeIsOpen(para);
                    break;
                case "get.up.default.url"://获取上游供应商的首页
                    result = Connector.GetUpDefalutUrl(para);
                    break;
                case "get.down.list"://获得下游列表
                    result = Connector.GetDownList(para);
                    break;
                #endregion
                #region 商品
                case "product.addunauthorized":
                    result = Product.AddUnauthorized(para);
                    break;
                case "product.deleteunauthorized":
                    result = Product.DeleteUnauthorized(para);
                    break;
                case "product.import":
                    result = Product.ImportProduct(para);
                    break;
                case "product.batchimport":
                    result = Product.BatchImportProduct(para);
                    break;
                case "product.list":
                    result = Product.GetProductList(para);
                    break;
                case "upproduct.list":
                    result = Product.GetUpProductList(para);
                    break;
                case "brand.addauth":
                    result = Brand.AddAuth(para);
                    break;
                case "brand.list":
                    result = Brand.GetBrandList(para);
                    break;
                case "category.addauth":
                    result = Category.AddAuth(para);
                    break;
                case "category.list":
                    result = Category.GetCategoryList(para);
                    break;
                #endregion
                #region 网站
                case "site.addauth":
                    result = Site.AddAuth(para);
                    break;
                case "site.list":
                    result = Site.GetSitePageList(para);
                    break;
                case "site.authlist":
                    result = Site.GetAuthSiteById(para);
                    break;
                case "site.import":
                    result = Site.ImportAuthPage(para);
                    break;
                #endregion

                #region 代下单
                case "order.check"://下游订单商品检测
                    result = Order.OrderCheck(para);
                    break;
                case "order.submit"://下游代下单提交中转
                    result = Order.OrderSubmit(para);
                    break;
                case "order.dispatch.create"://上游订单发货中转
                    result = Order.DispatchSubmit(para);
                    break;
                case "order.receive.create"://下游确认收货
                    result = Order.OrderReceive(para);
                    break;
                case "order.list"://代发订单列表
                    result = Order.OrderList(para);
                    break;
                case "order.pay.event"://下游订单支付
                    result = Order.OrderPay(para);
                    break;
                case "order.update.event"://上游订单修改
                    result = Order.OrderUpdate(para);
                    break;
                case "order.orderinfo"://上游订单修改
                    result = Order.GetOrderInfo(para);
                    break;
                    #endregion

            }

            return result;
        }

    }
}
