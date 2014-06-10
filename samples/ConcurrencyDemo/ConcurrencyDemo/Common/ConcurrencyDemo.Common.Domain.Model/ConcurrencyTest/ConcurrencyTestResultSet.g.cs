﻿//------------------------------------------------------------------------------
// <autogenerated>
//   This file was generated by T4 code generator Model1.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using NTier.Common.Domain.Model;

namespace ConcurrencyDemo.Common.Domain.Model.ConcurrencyTest
{
    [DataContract(IsReference = true)]
    public sealed partial class ConcurrencyTestResultSet : ConcurrencyTestChangeSet, IResultSet
    {
        #region ctor
        public ConcurrencyTestResultSet(ConcurrencyTestChangeSet changeSet)
            : base(changeSet)
        {
        }
        #endregion

        #region DataMember

        [DataMember]
        public IList<ARecord> ARecordConcurrencyConflicts { get; private set; }

        [DataMember]
        public IList<BRecord> BRecordConcurrencyConflicts { get; private set; }

        #endregion

        #region AddConcurrencyConflicts

        public void AddConcurrencyConflicts(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is ARecord)
                {
                    AddConcurrencyConflict((ARecord)entity);
                }
                else if (entity is BRecord)
                {
                    AddConcurrencyConflict((BRecord)entity);
                }
                else
                {
                    throw new Exception(string.Format("Unknown type {0}", entity.GetType().Name));
                }
            }
        }

        private void AddConcurrencyConflict(ARecord entity)
        {
            if (ARecordConcurrencyConflicts == null)
            {
                ARecordConcurrencyConflicts = new List<ARecord>();
            }
            ARecordConcurrencyConflicts.Add(entity);
        }

        private void AddConcurrencyConflict(BRecord entity)
        {
            if (BRecordConcurrencyConflicts == null)
            {
                BRecordConcurrencyConflicts = new List<BRecord>();
            }
            BRecordConcurrencyConflicts.Add(entity);
        }

        #endregion

        #region IsConcurrencyConflict

        public bool IsConcurrencyConflict(ARecord e)
        {
            return ARecordConcurrencyConflicts != null && ARecordConcurrencyConflicts.Count > 0 && ARecordConcurrencyConflicts.Contains(e);
        }

        public bool IsConcurrencyConflict(BRecord e)
        {
            return BRecordConcurrencyConflicts != null && BRecordConcurrencyConflicts.Count > 0 && BRecordConcurrencyConflicts.Contains(e);
        }

        #endregion

        #region HasConcurrencyConflicts
        public bool HasConcurrencyConflicts
        {
            get
            {
                return ARecordConcurrencyConflicts != null ||
                    BRecordConcurrencyConflicts != null;
            }
        }
        #endregion
    }
}
