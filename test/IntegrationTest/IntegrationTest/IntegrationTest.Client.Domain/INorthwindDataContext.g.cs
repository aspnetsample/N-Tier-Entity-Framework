﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator Northwind.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using IntegrationTest.Common.Domain.Model.Northwind;
using NTier.Client.Domain;

namespace IntegrationTest.Client.Domain
{
    public partial interface INorthwindDataContext : IDataContext
    {

        #region Categories

        IEntitySet<Category> Categories { get; }

        void Add(Category entity);
        void Delete(Category entity);
        void Attach(Category entity);
        void AttachAsModified(Category entity, Category original);
        void Detach(Category entity);

        #endregion Categories

        #region DemographicGroups

        IEntitySet<DemographicGroup> DemographicGroups { get; }

        void Add(DemographicGroup entity);
        void Delete(DemographicGroup entity);
        void Attach(DemographicGroup entity);
        void AttachAsModified(DemographicGroup entity, DemographicGroup original);
        void Detach(DemographicGroup entity);

        #endregion DemographicGroups

        #region Customers

        IEntitySet<Customer> Customers { get; }

        void Add(Customer entity);
        void Delete(Customer entity);
        void Attach(Customer entity);
        void AttachAsModified(Customer entity, Customer original);
        void Detach(Customer entity);

        #endregion Customers

        #region DynamicContentEntities

        IEntitySet<DynamicContentEntity> DynamicContentEntities { get; }

        void Add(DynamicContentEntity entity);
        void Delete(DynamicContentEntity entity);
        void Attach(DynamicContentEntity entity);
        void AttachAsModified(DynamicContentEntity entity, DynamicContentEntity original);
        void Detach(DynamicContentEntity entity);

        #endregion DynamicContentEntities

        #region Employees

        IEntitySet<Employee> Employees { get; }

        void Add(Employee entity);
        void Delete(Employee entity);
        void Attach(Employee entity);
        void AttachAsModified(Employee entity, Employee original);
        void Detach(Employee entity);

        #endregion Employees

        #region Order_Details

        IEntitySet<Order_Detail> Order_Details { get; }

        void Add(Order_Detail entity);
        void Delete(Order_Detail entity);
        void Attach(Order_Detail entity);
        void AttachAsModified(Order_Detail entity, Order_Detail original);
        void Detach(Order_Detail entity);

        #endregion Order_Details

        #region Orders

        IEntitySet<Order> Orders { get; }

        void Add(Order entity);
        void Delete(Order entity);
        void Attach(Order entity);
        void AttachAsModified(Order entity, Order original);
        void Detach(Order entity);

        #endregion Orders

        #region Products

        IEntitySet<Product> Products { get; }

        void Add(Product entity);
        void Delete(Product entity);
        void Attach(Product entity);
        void AttachAsModified(Product entity, Product original);
        void Detach(Product entity);

        #endregion Products

        #region Regions

        IEntitySet<Region> Regions { get; }

        void Add(Region entity);
        void Delete(Region entity);
        void Attach(Region entity);
        void AttachAsModified(Region entity, Region original);
        void Detach(Region entity);

        #endregion Regions

        #region Shippers

        IEntitySet<Shipper> Shippers { get; }

        void Add(Shipper entity);
        void Delete(Shipper entity);
        void Attach(Shipper entity);
        void AttachAsModified(Shipper entity, Shipper original);
        void Detach(Shipper entity);

        #endregion Shippers

        #region Suppliers

        IEntitySet<Supplier> Suppliers { get; }

        void Add(Supplier entity);
        void Delete(Supplier entity);
        void Attach(Supplier entity);
        void AttachAsModified(Supplier entity, Supplier original);
        void Detach(Supplier entity);

        #endregion Suppliers

        #region Territories

        IEntitySet<Territory> Territories { get; }

        void Add(Territory entity);
        void Delete(Territory entity);
        void Attach(Territory entity);
        void AttachAsModified(Territory entity, Territory original);
        void Detach(Territory entity);

        #endregion Territories

    }
}
