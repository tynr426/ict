using Core.BLL;
using ECF;
using ECF.Data.Query;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.V1.Core
{
    class Account
    {
        public static IResultResponse Get(SysParameter para)
        {
            try
            {
                PagingQuery query = para.ToPagingQuery();
                PagingResult result = new AccountBusiness(para.dapperFactory).GetList(query);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }

        public static IResultResponse Add(SysParameter para)
        {
            try
            {
                int result = new AccountBusiness(para.dapperFactory).Insert(para.Properties);
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
                int result = new AccountBusiness(para.dapperFactory).Delete(para.ToInt("Id"));
                return ResultResponse.GetSuccessResult(result);
            }
            catch(Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse Update(SysParameter para)
        {
            try
            {
                int result = new AccountBusiness(para.dapperFactory).Update(para.Properties);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
    }
}
