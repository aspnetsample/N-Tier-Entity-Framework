//------------------------------------------------------------------------------
// <autogenerated>
//   This file was generated by T4 code generator ProductModel.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using NTier.Common.Domain.Model;
using ProductManager.Common.Domain.Model.ProductManager;

namespace ProductManager.Common.Domain.Service.Contracts
{
    [ServiceContract]
    public partial interface IProductManagerDataService
    {
        [OperationContractAttribute(AsyncPattern = true)]
        IAsyncResult BeginGetProducts(ClientInfo clientInfo, Query query, AsyncCallback callback, object asyncState);
        QueryResult<Product> EndGetProducts(IAsyncResult result);

        [OperationContractAttribute(AsyncPattern = true)]
        IAsyncResult BeginGetProductCategories(ClientInfo clientInfo, Query query, AsyncCallback callback, object asyncState);
        QueryResult<ProductCategory> EndGetProductCategories(IAsyncResult result);


        [OperationContractAttribute(AsyncPattern = true)]
        IAsyncResult BeginSubmitChanges(ClientInfo clientInfo, ProductManagerChangeSet changeSet, AsyncCallback callback, object asyncState);
        ProductManagerResultSet EndSubmitChanges(IAsyncResult result);
    }
}
