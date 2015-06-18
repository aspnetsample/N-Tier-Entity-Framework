﻿//------------------------------------------------------------------------------
// <autogenerated>
//   This file was generated by T4 code generator DemoModel.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ConcurrencyDemo.Common.Domain.Model.ConcurrencyTest;
using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories;

namespace ConcurrencyDemo.Server.Domain.Repositories
{
    public partial class ConcurrencyTestRepository : NTier.Server.Domain.Repositories.EntityFramework.Repository, IConcurrencyTestRepository
    {
        #region Constructors

        public ConcurrencyTestRepository()
            : base("name=ConcurrencyTestEntities", "ConcurrencyTestEntities")
        {
        }

        public ConcurrencyTestRepository(string connectionString, string containerName = "ConcurrencyTestEntities")
            : base(connectionString, containerName)
        {
        }

        public ConcurrencyTestRepository(EntityConnection connection, string containerName = "ConcurrencyTestEntities")
            : base(connection, containerName)
        {
        }

        #endregion Constructors

        #region EntitySets

        public IEntitySet<ARecord> ARecords
        {
            get { return _aRecords  ?? (_aRecords = CreateEntitySet<ARecord>("ARecords")); }
        }
        private IEntitySet<ARecord> _aRecords;

        public IEntitySet<BRecord> BRecords
        {
            get { return _bRecords  ?? (_bRecords = CreateEntitySet<BRecord>("BRecords")); }
        }
        private IEntitySet<BRecord> _bRecords;

        public IEntitySet<CRecord> CRecords
        {
            get { return _cRecords  ?? (_cRecords = CreateEntitySet<CRecord>("CRecords")); }
        }
        private IEntitySet<CRecord> _cRecords;

        #endregion EntitySets
    }
}
