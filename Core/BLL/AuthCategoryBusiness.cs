﻿using Core.ENT;
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
using System.Threading.Tasks;

namespace Core.BLL
{
    public class AuthCategoryBusiness : CoreBusiness
    {
        public AuthCategoryBusiness(IDapperFactory dapperFactory) : base(dapperFactory)
        {

        }

        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "auth_category";

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
                return new AuthCategoryEntity();
            }
        }
        #endregion

        #region 插入授权目录信息
        /// <summary>
        /// 插入授权目录信息
        /// </summary>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        public int Insert(int connectorId,string ids)
        {
            ClearCache();
            int val = 0;

            //检查信息是否存在
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("ConnectorId", connectorId);
            dic.Add("Status", 1);
            AuthCategoryEntity entConnector = DbAccess.GetEntity<AuthCategoryEntity>(TableName, dic);
            if (entConnector != null && entConnector.Id > 0)
            {
                entConnector.UpdateTime = DateTime.Now;
                entConnector.CategoryIds = ids;
                val = DbAccess.ExecuteUpdate(TableName, entConnector, new string[] { "Id" });

            }
            else
            {
                AuthCategoryEntity ent = new AuthCategoryEntity();
                ent.Status = 1;
                ent.ConnectorId = connectorId;
                ent.CategoryIds = ids;
                ent.AddTime = DateTime.Now;
                ent.UpdateTime = DateTime.Now;
                val = DbAccess.ExecuteInsert(TableName, ent);
            }

            return val;
        }

        #endregion

        #region 通过条件获取授权品牌信息
        /// <summary>
        /// 通过条件获取授权品牌信息
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public ArrayList GetAuthCategoryList(int connectorId,bool showAll=true)
        {
            DataTable dt = DBTable("ConnectorId=" + connectorId);
            string selectIds = "";
            //快马接口获取
            ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness(_dapperFactory).GetEntity("Id=" + connectorId);
            if (connector == null)
            {
                return null;
            }
            GroupEntity group = (GroupEntity)new GroupBusiness(_dapperFactory).GetEntity("Id=" + connector.GroupId);
            if (group == null)
            {
                return null;
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                selectIds = Utils.ToString(dt.Rows[0]["CategoryIds"]);
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (showAll)
            {
                dic.Add("layer", "1");
            }
            else {
                if (!string.IsNullOrWhiteSpace(selectIds))
                {
                    dic.Add("droitcategoryids", selectIds);
                }
                else {
                    return null;
                }
            }
            dic.Add("FKId", connector.SellerId.ToString());
            dic.Add("ProprietorId", connector.SellerId.ToString());
            dic.Add("FKFlag", "2");
            
            string msg = "";
            string datajson = ApiRequest.GetRemoteContent(group.Domain + "/Route.axd", "vast.mall.category.tree", dic, out msg);



            selectIds = "," + selectIds + ",";

            var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(datajson);
            if (!Utils.ToBool(dict["Success"]))
            {
                return null;
            }
            ArrayList content = dict["Content"] as ArrayList;
            if (content == null || content.Count <= 0)
            {
                return null;
            }
            foreach (Dictionary<string, object> item in content)
            {
                item.Add("ConnectorId", connectorId);
                if (!string.IsNullOrEmpty(selectIds))
                {
                    if (selectIds.Contains("," + Utils.ToString(item["Id"]) + ","))
                    {
                        item.Add("Checked", true);
                    }
                    else
                    {
                        item.Add("Checked", false);
                    }
                    //if (item["Children"] !=null) {
                    //    ArrayList secondChildren = item["Children"] as ArrayList;
                    //    foreach (Dictionary<string, object> secondChild in secondChildren) {
                    //        if (selectIds.Contains("," + Utils.ToString(secondChild["Id"]) + ","))
                    //        {
                    //            secondChild.Add("Checked", true);
                    //        }
                    //        else
                    //        {
                    //            secondChild.Add("Checked", false);
                    //        }
                    //        if (secondChild["Children"] != null)
                    //        {
                    //            ArrayList lastChildren = secondChild["Children"] as ArrayList;
                    //            foreach (Dictionary<string, object> lastChild in lastChildren)
                    //            {
                    //                if (selectIds.Contains("," + Utils.ToString(lastChild["Id"]) + ","))
                    //                {
                    //                    lastChild.Add("Checked", true);
                    //                }
                    //                else
                    //                {
                    //                    lastChild.Add("Checked", false);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }

            }
            return content;

        }
        #endregion

        #region 通过授权id获取授权目录信息
        /// <summary>
        /// 通过卖id获取授权品牌信息
        /// </summary>
        /// <param name="id">授权ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public DataTable GetAuthCategoryById(int id)
        {
            string sql = string.Format("Id={0}", id);
            DataTable dt = DBTable(sql);
            return dt;
        }
        #endregion
    }
}
