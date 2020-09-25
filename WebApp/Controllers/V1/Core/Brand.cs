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
    class Brand
    {
        public static IResultResponse AddAuth(SysParameter para)
        {
            try
            {
                int connectorId = para.ToInt("ConnectorId", 0);
                string ids = para.ToValue("BrandIds");
                int result = new AuthBrandBusiness(para.dapperFactory).Insert(connectorId, ids);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }

        public static IResultResponse GetBrandList(SysParameter para)
        {
            try
            {
                int connectorId = para.ToInt("ConnectorId",0);
                string firstSpell = para.ToValue("FirstSpell");
                bool showAll = para.ToBool("ShowAll");
                string keyword= para.ToValue("KeyWord");
                ArrayList result = new AuthBrandBusiness(para.dapperFactory).GetAuthBrandList(connectorId, firstSpell, keyword, showAll);
                if (result==null) {
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
