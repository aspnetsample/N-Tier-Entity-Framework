﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator Northwind.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Transactions;
using IntegrationTest.Common.Domain.Model.Northwind;
using IntegrationTest.Common.Domain.Service.Contracts;
using IntegrationTest.Server.Domain.Repositories;
using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories;
using NTier.Server.Domain.Service;


namespace IntegrationTest.Server.Domain.Service
{
    public partial class NorthwindDataService : DataService<INorthwindRepository>, INorthwindDataService
    {
        #region fields
 
        private static Func<ClientInfo, INorthwindRepository> _defaultRepositoryFactory = clientInfo => new NorthwindRepository();
        private readonly Func<ClientInfo, INorthwindRepository> _repositoryFactory;
 
        #endregion fields
 
        #region constructor
 
        public NorthwindDataService()
            : this(_defaultRepositoryFactory)
        {
        }
 
        public NorthwindDataService(Func<ClientInfo, INorthwindRepository> repositoryFactory)
        {
            if (ReferenceEquals(null, repositoryFactory)) throw new ArgumentNullException("repositoryFactory");
            _repositoryFactory = repositoryFactory;
        }
 
        #endregion constructor

        #region query service methods

        partial void PreProcessing(ClientInfo clientInfo, ref Query query, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Category> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<DemographicGroup> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Customer> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<DynamicContentEntity> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Employee> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Order_Detail> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Order> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Product> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Region> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Shipper> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Supplier> result, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, Query query, ref QueryResult<Territory> result, INorthwindRepository repository);

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Category> GetCategories(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Categories.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<DemographicGroup> GetDemographicGroups(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.DemographicGroups.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Customer> GetCustomers(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Customers.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<DynamicContentEntity> GetDynamicContentEntities(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.DynamicContentEntities.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Employee> GetEmployees(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Employees.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Order_Detail> GetOrder_Details(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Order_Details.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Order> GetOrders(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Orders.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Product> GetProducts(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Products.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Region> GetRegions(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Regions.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Shipper> GetShippers(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Shippers.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Supplier> GetSuppliers(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Suppliers.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public QueryResult<Territory> GetTerritories(ClientInfo clientInfo, Query query)
        {
            using (var dataRepository = _repositoryFactory(clientInfo))
            {
                PreProcessing(clientInfo, ref query, dataRepository);
                var result = Get(dataRepository.Territories.AsNoTrackingQueryable(), query, clientInfo);
                PostProcessing(clientInfo, query, ref result, dataRepository);
                return result;
            }
        }

        #endregion query service methods

        #region update service method

        partial void PreProcessing(ClientInfo clientInfo, ref NorthwindChangeSet changeSet, INorthwindRepository repository);
        partial void BeforeSaving(ClientInfo clientInfo, ref NorthwindChangeSet changeSet, INorthwindRepository repository);
        partial void PostProcessing(ClientInfo clientInfo, ref NorthwindResultSet result, INorthwindRepository repository);

        [OperationBehavior(TransactionScopeRequired = true, Impersonation = ImpersonationOption.Allowed)]
        public NorthwindResultSet SubmitChanges(ClientInfo clientInfo, NorthwindChangeSet changeSet)
        {
            var resultSet = new NorthwindResultSet(changeSet);
            using (var transactionScope = CreateSavingTransactionScope())
            {
                using (var dataRepository = _repositoryFactory(clientInfo))
                {
                    // optional custom processing
                    PreProcessing(clientInfo, ref changeSet, dataRepository);

                    // apply chnages to repository
                    ApplyChanges(dataRepository, dataRepository.Categories, changeSet, changeSet.Categories, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.DemographicGroups, changeSet, changeSet.DemographicGroups, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.Customers, changeSet, changeSet.Customers, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.DynamicContentEntities, changeSet, changeSet.DynamicContentEntities, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.Employees, changeSet, changeSet.Employees, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.Order_Details, changeSet, changeSet.Order_Details, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.Orders, changeSet, changeSet.Orders, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.Products, changeSet, changeSet.Products, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.Regions, changeSet, changeSet.Regions, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.Shippers, changeSet, changeSet.Shippers, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.Suppliers, changeSet, changeSet.Suppliers, clientInfo);
                    ApplyChanges(dataRepository, dataRepository.Territories, changeSet, changeSet.Territories, clientInfo);

                    // optional custom processing
                    BeforeSaving(clientInfo, ref changeSet, dataRepository);

                    // save changes
                    SaveChanges(dataRepository, changeSet, resultSet);

                    // optional custom processing
                    PostProcessing(clientInfo, ref resultSet, dataRepository);
                }
                transactionScope.Complete();
            }
            return resultSet;
        }

        protected override FaultException CreateUpdateFaultException(string message, IEnumerable<Entity> entities)
        {
            return new FaultException<NorthwindUpdateFault>(new NorthwindUpdateFault(message, entities), "Update error");
        }

        protected override FaultException CreateOptimisticConcurrencyFaultException(string message, IEnumerable<Entity> entities)
        {
            return new FaultException<NorthwindOptimisticConcurrencyFault>(new NorthwindOptimisticConcurrencyFault(message, entities), "Optimistic concurrency error");
        }

        #endregion update service method
    }
}

