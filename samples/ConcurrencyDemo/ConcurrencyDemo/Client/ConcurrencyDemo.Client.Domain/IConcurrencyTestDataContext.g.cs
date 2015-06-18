﻿//------------------------------------------------------------------------------
// <autogenerated>
//   This file was generated by T4 code generator DemoModel.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using ConcurrencyDemo.Common.Domain.Model.ConcurrencyTest;
using NTier.Client.Domain;

namespace ConcurrencyDemo.Client.Domain
{
    public partial interface IConcurrencyTestDataContext : IDataContext
    {

        #region ARecords

        IEntitySet<ARecord> ARecords { get; }

        void Add(ARecord entity);
        void Delete(ARecord entity);
        void Attach(ARecord entity);
        void AttachAsModified(ARecord entity, ARecord original);
        void Detach(ARecord entity);

        #endregion ARecords

        #region BRecords

        IEntitySet<BRecord> BRecords { get; }

        void Add(BRecord entity);
        void Delete(BRecord entity);
        void Attach(BRecord entity);
        void AttachAsModified(BRecord entity, BRecord original);
        void Detach(BRecord entity);

        #endregion BRecords

        #region CRecords

        IEntitySet<CRecord> CRecords { get; }

        void Add(CRecord entity);
        void Delete(CRecord entity);
        void Attach(CRecord entity);
        void AttachAsModified(CRecord entity, CRecord original);
        void Detach(CRecord entity);

        #endregion CRecords

    }
}
