using Core.BLL;
using Core.ENT;
using ECF;
using ECF.Data.Query;
using ECF.Security;
using ECF.Web.Http;
using Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Controller.V1.Core
{
    class Connector
    {
        public static IResultResponse GetList(SysParameter para)
        {
            try
            {
                string mobile = para.ToValue("Mobile");
                int connectorId = para.ToInt("ConnectorId", 0);
                IResultResponse result = new ConnectorBusiness(para.dapperFactory).search(connectorId, mobile);
                return result;
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse Insert(SysParameter para)
        {
            try
            {
                int result = new ConnectorBusiness(para.dapperFactory).Insert(para.Properties);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse Delete(SysParameter para)
        {
            try
            {
                int id = para.ToInt("Id");
                int result = new ConnectorBusiness(para.dapperFactory).Delete(id);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse Update(SysParameter para)
        {
            try
            {
                int result = new ConnectorBusiness(para.dapperFactory).Update(para.Properties);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse GetMyList(SysParameter para)
        {
            try
            {
                PagingQuery query = para.ToPagingQuery();
                int connectorId = para.ToInt("ConnectorId");
                int status = para.ToInt("Status", 0);
                PagingResult result = new ConnectorBusiness(para.dapperFactory).GetMyConnectorList(query, connectorId, status);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse GetNewList(SysParameter para)
        {
            try
            {
                PagingQuery query = para.ToPagingQuery();
                int connectorId = para.ToInt("ConnectorId");

                PagingResult result = new ConnectorBusiness(para.dapperFactory).GetNewConnectorList(query, connectorId);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse AddUpRelation(SysParameter para)
        {
            try
            {
                string mobile = para.ToValue("Mobile");
                int connectorId = para.ToInt("ConnectorId");
                int result = new RelationBusiness(para.dapperFactory).InsertUpConnectorRelation(mobile, connectorId);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse AddDownRelation(SysParameter para)
        {
            try
            {
                //string mobile = para.ToValue("Mobile");
                int connectorId = para.ToInt("ConnectorId");
                int downConnectorId = para.ToInt("DownConnectorId");
                return new RelationBusiness(para.dapperFactory).InsertDownConnectorRelation(downConnectorId, connectorId);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse CheckConnector(SysParameter para)
        {
            try
            {
                int id = para.ToInt("Id", 0);
                int status = para.ToInt("Status");
                int connectorId = para.ToInt("ConnectorId", 0);
                string type = para.ToValue("Type");
                if (type == "up")
                {
                    return new RelationBusiness(para.dapperFactory).CheckDownConnector(connectorId, id, status);
                }
                else
                {
                    return new RelationBusiness(para.dapperFactory).CheckUpConnector(connectorId, id, status);

                }
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }

        public static IResultResponse DeleteRelation(SysParameter para)
        {
            try
            {
                int id = para.ToInt("Id");
                int result = new RelationBusiness(para.dapperFactory).Delete(id);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        /// <summary>
        /// 授权登录
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse AuthLogin(SysParameter para)
        {
            try
            {
                string token = para.ToValue("exchange_token");
                if (token.IndexOf("%") > -1)
                {
                    token = HttpUtility.UrlDecode(token);
                }
                string sign = para.ToValue("exhange_sign");
                string sign_new = Encrypt.MD532("rwxkj:" + token);
                if (sign_new != sign)
                {
                    return ResultResponse.ExceptionResult("参数被篡改");
                }
                string tokenStr = AES.Decode(token);

                var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(tokenStr);

                if (dic == null || dic.Count() == 0)
                {
                    return ResultResponse.ExceptionResult("交换token失败");
                }
                int fk_id = dic.ToInt("fk_id", 0);
                int db_id = dic.ToInt("db_id", 0);
                IResultResponse resultResponse = new ConnectorBusiness(para.dapperFactory).ExchangeToken(fk_id, db_id);
                if (resultResponse.Success)
                {
                    return resultResponse;
                }
                else
                {
                    string domain = dic.ToValue("domain");
                    string msg = string.Empty;
                    resultResponse = ApiRequest.GetResponse(domain, "account.base.info", new Dictionary<string, string>() {
                        {"exchange_token",HttpUtility.UrlEncode(token) },
                         {"exhange_sign",sign },
                    });
                    if (resultResponse.Success)
                    {
                        Dictionary<string, object> dic_content = resultResponse.Content as Dictionary<string, object>;
                        resultResponse = new ConnectorBusiness(para.dapperFactory).Join(dic_content);
                        if (!resultResponse.Success) return resultResponse;

                        resultResponse = new ConnectorBusiness(para.dapperFactory).ExchangeToken(fk_id, db_id);
                    }
                }
                return resultResponse;
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        /// <summary>
        /// 获得二维码内容
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse GetConnectorByQr(SysParameter para)
        {
            string qr = para.ToValue("qr");
            string s = ECF.Security.AES.Decode(qr);
            if (string.IsNullOrEmpty(s))
            {
                return ResultResponse.ExceptionResult("该二维码无效");
            }
            int connectorId = Utils.ToInt(s);

            Dictionary<string, object> dicInfo = new ConnectorBusiness(para.dapperFactory).GetConnectorById(connectorId);
            if (dicInfo == null || dicInfo.Count == 0)
            {
                return ResultResponse.ExceptionResult("不存在该连接器对象");
            }
            return ResultResponse.GetSuccessResult(dicInfo);
        }
        /// <summary>
        /// 推送克隆命令
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static IResultResponse PushCloneCommand(SysParameter para)
        {
            IResultResponse resultResponse = OpsUtil.VerifyAccessToken(para);

            if (!resultResponse.Success) return resultResponse;

            Sdk.AccessToken accessToken = resultResponse.Content as Sdk.AccessToken;
            if (accessToken.UpConnectorId == 0)
            {
                return ResultResponse.ExceptionResult("上下游关系没生效，请重新登录！");
            }
            try
            {
                string key = accessToken.Id + "_" + accessToken.UpConnectorId;

                if (!IsSendCmd(key)) return ResultResponse.ExceptionResult("三分钟之内发送次过多");
                //克隆权限
                var cloneAuthority = new ProductRelationBusiness(para.dapperFactory).CloneAuthority(accessToken.Id, accessToken.UpConnectorId);
                if (!cloneAuthority)
                {
                    return ResultResponse.ExceptionResult("你已经克隆过了！");
                }
                long re = DistributePubSub.RedisPub("product_clone", new Dictionary<string, int>()
            {
                {"upId",accessToken.UpConnectorId },
                 {"downId",accessToken.Id },
            });
                //bool sendCmd = WebSocketClient.SendCommand("Clone", "data.clone.push.command", new object[] { accessToken.Id, accessToken.UpConnectorId }, true);
                if (re == -1)
                {
                    return ResultResponse.ExceptionResult("命令已发送失败！");
                }

                //插入发送记录
                new CloneResultBusiness(para.dapperFactory).Insert(accessToken);

                //设置发送时间
                SetSendCmd(key);

                return ResultResponse.GetSuccessResult("命令已发送成功，请稍后！");

            }
            catch (Exception ex)
            {

                new Exceptions(ex, "clone");
                return ResultResponse.ExceptionResult(ex.Message);
            }
        }
        /// <summary>
        /// 发送命令集合
        /// </summary>
        static Dictionary<string, long> SendCmdDic = new Dictionary<string, long>();
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object SendCmdObj = new object();
        public static void SetSendCmd(string keys)
        {
            lock (SendCmdObj)
            {
                if (!SendCmdDic.ContainsKey(keys))
                {
                    SendCmdDic.Add(keys, Utils.ToUnixTime(DateTime.Now.AddMinutes(2)));
                }
                else
                {
                    SendCmdDic[keys] = Utils.ToUnixTime(DateTime.Now.AddMinutes(2));
                }
            }
        }
        /// <summary>
        /// 是否有发送命令
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool IsSendCmd(string keys)
        {
            var bo = true;
            lock (SendCmdObj)
            {
                if (SendCmdDic.ContainsKey(keys))
                {
                    var t = SendCmdDic[keys];
                    if (Utils.ToUnixTime(DateTime.Now) < t)
                    {
                        bo = false;
                    }
                }

            }
            return bo;
        }
        public static bool ClearCmd(string keys)
        {
            var bo = true;
            lock (SendCmdObj)
            {
                if (SendCmdDic.ContainsKey(keys))
                {
                    SendCmdDic.Remove(keys);
                }

            }
            return bo;
        }
        /// <summary>
        /// 清除克隆数据
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse ClearCloneData(SysParameter para)
        {
            IResultResponse resultResponse = OpsUtil.VerifyAccessToken(para);

            if (!resultResponse.Success) return resultResponse;

            Sdk.AccessToken accessToken = resultResponse.Content as Sdk.AccessToken;
            string key = accessToken.Id + "_" + accessToken.UpConnectorId;
            try
            {
                DataTable dt = new CloneResultBusiness(para.dapperFactory).GetCloneData(accessToken);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return ResultResponse.ExceptionResult("没有数据可清理");
                }
                //清缓存
                ClearCmd(key);
                string domain = accessToken.Domain;
                string content = Utils.ToString(dt.Rows[0]["CloneResult"]);
                resultResponse = ApiRequest.GetResponse(domain, "mall.data.clone.clear", new Dictionary<string, string>() {
                        {"FKId",accessToken.SellerId.ToString() },
                        {"FKFlag","2" },
                        {"Content",HttpUtility.UrlEncode(content) }
                    });

                if (resultResponse.Success)
                {
                    //删除连接器中
                    new ProductRelationBusiness(para.dapperFactory).DeleteData(accessToken.Id, accessToken.UpConnectorId);

                    return ResultResponse.GetSuccessResult("删除成功");
                }
                return ResultResponse.ExceptionResult("删除失败");
            }
            catch (Exception ex)
            {

                new Exceptions(ex, "clone");
                return ResultResponse.ExceptionResult(ex.Message);
            }
        }
        /// <summary>
        /// 克隆权限
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse CloneAuthority(SysParameter para)
        {
            IResultResponse resultResponse = OpsUtil.VerifyAccessToken(para);

            if (!resultResponse.Success) return resultResponse;

            Sdk.AccessToken accessToken = resultResponse.Content as Sdk.AccessToken;

            try
            {
                var cloneAuthority = new ProductRelationBusiness(para.dapperFactory).CloneAuthority(accessToken.Id, accessToken.UpConnectorId);
                DataTable dt = new CloneResultBusiness(para.dapperFactory).GetCloneData(accessToken);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return ResultResponse.GetSuccessResult(1);
                }
                int status = Utils.ToInt(dt.Rows[0]["Status"]);
                if (cloneAuthority)
                {
                    return ResultResponse.GetSuccessResult(1);
                }
                else
                {
                    if (status == -1)
                    {
                        return ResultResponse.ExceptionResult("清除克隆失败数据");
                    }
                    else
                    {
                        return ResultResponse.ExceptionResult("清除克隆成功数据");
                    }
                }
            }
            catch (Exception ex)
            {

                new Exceptions(ex, "clone");
                return ResultResponse.ExceptionResult(ex.Message);
            }
        }
        public static IResultResponse CheckDownConnector(SysParameter para)
        {
            string mobile = para.ToValue("Mobile");
            int upConnectorId = para.ToInt("UpConnectorId", 0);
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return ResultResponse.ExceptionResult("请输入正确手机号");
            }
            IResultResponse result = new RelationBusiness(para.dapperFactory).CheckDownConnector(mobile, upConnectorId);
            return result;
        }

        public static IResultResponse GetScmCode(SysParameter para)
        {
            string verifyCode = para.ToValue("verifyCode");
            string codeToken = para.ToValue("codeToken");
            int connectorId = para.ToInt("connectorId", 0);

            IResultResponse result = new RelationBusiness(para.dapperFactory).GetValidateCode(connectorId, codeToken, verifyCode);
            return result;
        }


        public static IResultResponse ValidateCode(SysParameter para)
        {
            string verifyCode = para.ToValue("verifyCode");
            int connectorId = para.ToInt("connectorId", 0);
            int upConnectorId = para.ToInt("upConnectorId", 0);
            IResultResponse result = new RelationBusiness(para.dapperFactory).ValidateCode(connectorId, upConnectorId, verifyCode);
            return result;
        }


        public static IResultResponse GetValidateCode(SysParameter para)
        {
            NetResponse response = null;
            string domain = para.ToValue("domain");
            if (domain == null)
            {
                return ResultResponse.ExceptionResult("域名为空");
            }
            NetRequest request = new NetRequest();
            request.SetTimeOut(8000);
            request.SetTryTimes(1);
            string url = domain + "/api/VerifyCode.axd?base64=1";
            string msg = string.Empty;
            response = request.Get(url, string.Empty, out msg);
            string content = response.Content;
            string codeToken = response.Header["CodeToken"];
            Dictionary<string, string> result = new Dictionary<string, string>() {
                {"content",content },
                {"codeToken",codeToken },
            };
            return ResultResponse.GetSuccessResult(result.ToJson());
        }


        public static IResultResponse ChangeIsOpen(SysParameter para)
        {
            int id = para.ToInt("Id", 0);
            int status = para.ToInt("Status");
            int connectorId = para.ToInt("ConnectorId", 0);
            return new RelationBusiness(para.dapperFactory).ChangeIsOpen(connectorId, id, status);

        }
        /// <summary>
        /// 获取上游供应商的首页
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse GetUpDefalutUrl(SysParameter para)
        {
            IResultResponse resultResponse = OpsUtil.VerifyAccessToken(para);

            if (!resultResponse.Success) return resultResponse;

            Sdk.AccessToken accessToken = resultResponse.Content as Sdk.AccessToken;
            try
            {

                ConnectorRelation connectorRelation = new ConnectorBusiness(para.dapperFactory).GetUpConnector(Utils.ToInt(accessToken.Id));
                if (connectorRelation.VirtualDir != null)
                {

                    Dictionary<string, string> dic = new Dictionary<string, string>()
                    {
                        {"method","linker.login" },
                        {"userId","0" },
                        {"storeId", connectorRelation.UpBuyerId.ToString()},
                        {"virtualdir", connectorRelation.VirtualDir},
                    };
                    var param = ApiRequest.GetPaySign(dic);
                    return ResultResponse.GetSuccessResult($"{connectorRelation.Domain}/linker.axd?{param}");
                }
                return ResultResponse.ExceptionResult("你没有上游供应商");
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        /// <summary>
        /// 获得下游列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IResultResponse GetDownList(SysParameter para)
        {
            IResultResponse resultResponse = OpsUtil.VerifyAccessToken(para);

            if (!resultResponse.Success) return resultResponse;

            Sdk.AccessToken accessToken = resultResponse.Content as Sdk.AccessToken;
            try
            {

                DataTable dt = new ConnectorBusiness(para.dapperFactory).GetDownList(accessToken.Id);
                return ResultResponse.GetSuccessResult(dt);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }


        public static IResultResponse DownConnectorStock(SysParameter para)
        {
            try
            {
                PagingQuery query = para.ToPagingQuery();
                int connectorId = para.ToInt("ConnectorId", 0);
                PagingResult result = new ConnectorBusiness(para.dapperFactory).DownConnectorStock(connectorId,query);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }

        }
    }
}
