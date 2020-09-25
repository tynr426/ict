using Core.BLL;
using ECF;
using ECF.Data.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.V1.Core
{
    class Group
    {
        public static IResultResponse GetGroupList(SysParameter para)
        {
            try
            {
                PagingQuery query = para.ToPagingQuery();
                PagingResult result = new GroupBusiness(para.dapperFactory).GetGroupList(query);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse GroupInsert(SysParameter para)
        {
            try
            {
                int result = new GroupBusiness(para.dapperFactory).Insert(para.Properties);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse GroupUpdete(SysParameter para)
        {
            try
            {
                int result = new GroupBusiness(para.dapperFactory).Update(para.Properties);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }

        public static IResultResponse GroupDelete(SysParameter para)
        {
            try
            {
                int id = para.ToInt("Id");
                int result = new GroupBusiness(para.dapperFactory).Delete(id);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
    }
}
