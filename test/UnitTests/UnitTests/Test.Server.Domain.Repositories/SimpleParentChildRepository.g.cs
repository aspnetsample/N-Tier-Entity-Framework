﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator SimpleParentChildModel.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </auto-generated>
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
using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories;
using Test.Common.Domain.Model.SimpleParentChild;

namespace Test.Server.Domain.Repositories
{
    public partial class SimpleParentChildRepository : NTier.Server.Domain.Repositories.EntityFramework.Repository, ISimpleParentChildRepository
    {
        #region Constructors

        public SimpleParentChildRepository()
            : base("name=SimpleParentChildEntities", "SimpleParentChildEntities")
        {
        }

        public SimpleParentChildRepository(string connectionString, string containerName = "SimpleParentChildEntities")
            : base(connectionString, containerName)
        {
        }

        public SimpleParentChildRepository(EntityConnection connection, string containerName = "SimpleParentChildEntities")
            : base(connection, containerName)
        {
        }

        #endregion Constructors

        #region EntitySets

        public IEntitySet<Parent> ParentSet
        {
            get { return _parentSet  ?? (_parentSet = CreateEntitySet<Parent>("ParentSet")); }
        }
        private IEntitySet<Parent> _parentSet;

        public IEntitySet<Child> ChildSet
        {
            get { return _childSet  ?? (_childSet = CreateEntitySet<Child>("ChildSet")); }
        }
        private IEntitySet<Child> _childSet;

        #endregion EntitySets
    }
}
