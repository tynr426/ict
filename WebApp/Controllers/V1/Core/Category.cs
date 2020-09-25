using Core.BLL;
using ECF;
using ECF.Data.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.V1.Core
{
    class Category
    {
        public static IResultResponse AddAuth(SysParameter para)
        {
            try
            {
                int connectorId= para.ToInt("ConnectorId",0);
                string ids = para.ToValue("CategoryIds");
                int result = new AuthCategoryBusiness(para.dapperFactory).Insert(connectorId, ids);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }

        public static IResultResponse GetCategoryList(SysParameter para)
        {
            try
            {

                int connectorId = para.ToInt("ConnectorId",0);
                bool showAll = para.ToBool("ShowAll");
                ArrayList result = new AuthCategoryBusiness(para.dapperFactory).GetAuthCategoryList(connectorId, showAll);
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
    }
}
