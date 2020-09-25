using Core.BLL;
using ECF;
using ECF.Data.Query;
using ECF.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Controller.V1.Core
{
    class Product
    {
        public static IResultResponse AddUnauthorized(SysParameter para)
        {
            try
            {
                string productIds= para.ToValue("ProductIds");
                int connectorId = para.ToInt("ConnectorId",0);
                int result = new AuthProductBusiness(para.dapperFactory).Insert(productIds, connectorId);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse DeleteUnauthorized(SysParameter para)
        {
            try
            {
                string authProductIds = para.ToValue("AuthProductIds");
                int connectorId = para.ToInt("ConnectorId",0);
                int result = new AuthProductBusiness(para.dapperFactory).Delete(authProductIds, connectorId);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }

        public static IResultResponse GetProductList(SysParameter para)
        {
            try
            {
                PagingQuery query = para.ToPagingQuery();
                int connectorId = para.ToInt("ConnectorId",0);
                bool auth = para.ToBool("Auth");
                PagingResult result = new AuthProductBusiness(para.dapperFactory).GetAuthProductList(query, connectorId, auth);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse GetUpProductList(SysParameter para)
        {
            try
            {
                PagingQuery query = para.ToPagingQuery();
                int connectorId = para.ToInt("ConnectorId",0);
                PagingResult result = new AuthProductBusiness(para.dapperFactory).UpConnectorAuthList(query, connectorId);
                return ResultResponse.GetSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
        public static IResultResponse ImportProduct(SysParameter para)
        {
            try
            {
                int connectorId = para.ToInt("ConnectorId");
                int productId = para.ToInt("ProductId");
                int upConnectorId = para.ToInt("UpConnectorId");
                return new ProductRelationBusiness(para.dapperFactory).ImportProduct(connectorId, upConnectorId, productId);
  
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }


        public static IResultResponse BatchImportProduct(SysParameter para)
        {
            try
            {
                int connectorId = para.ToInt("ConnectorId");
                string productIds = para.ToValue("ProductIds");
                int upConnectorId = para.ToInt("UpConnectorId");
                IResultResponse result = new ProductRelationBusiness(para.dapperFactory).BatchImportProduct(connectorId, upConnectorId, productIds);
                return result;
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex, 500);
            }
        }
    }
}
