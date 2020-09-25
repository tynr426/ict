using Core.BLL;
using ECF;
using ECF.Data.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.V1.Core
{
    class Site
    {
        public static IResultResponse AddAuth(SysParameter para)
        {
            try
            {
                string pageIds = para.ToValue("PageIds");
                int connectorId = para.ToInt("ConnectorId");
                int pageType = para.ToInt("PageType", 1);
                int result = new AuthSiteBusiness(para.dapperFactory).Insert(pageIds, connectorId, pageType);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }

        public static IResultResponse GetSitePageList(SysParameter para)
        {
            try
            {
                int siteType = para.ToInt("PageType",1);
                int connectorId = para.ToInt("ConnectorId");

                ArrayList result = new AuthSiteBusiness(para.dapperFactory).GetAuthSitePageList(siteType, connectorId);
                if (result == null)
                {
                    return ResultResponse.GetSuccessResult(new ArrayList());
                }
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse GetAuthSiteById(SysParameter para)
        {
            try
            {
                int siteType = para.ToInt("PageType", 1);
                int connectorId = para.ToInt("ConnectorId");

                ArrayList result = new AuthSiteBusiness(para.dapperFactory).GetAuthSiteById(connectorId,siteType);
                if (result == null)
                {
                    return ResultResponse.GetSuccessResult(new ArrayList());
                }
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse ImportAuthPage(SysParameter para)
        {
            try
            {
                int siteType = para.ToInt("PageType", 1);
                int pageId = para.ToInt("PageId", 1);
                int connectorId = para.ToInt("ConnectorId");
                int upConnectorId = para.ToInt("UpConnectorId");
                int result = new AuthSiteBusiness(para.dapperFactory).ImportPage(connectorId, upConnectorId, siteType, pageId);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
    }
}
