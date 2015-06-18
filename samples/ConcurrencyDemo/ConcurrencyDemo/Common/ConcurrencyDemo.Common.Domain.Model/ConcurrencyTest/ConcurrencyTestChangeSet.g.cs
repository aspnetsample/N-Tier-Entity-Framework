﻿//------------------------------------------------------------------------------
// <autogenerated>
//   This file was generated by T4 code generator DemoModel.tt.
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
    public partial class ConcurrencyTestChangeSet : IChangeSet
    {
        #region Constructor

        public ConcurrencyTestChangeSet(IEnumerable<ARecord> aRecords, IEnumerable<BRecord> bRecords, IEnumerable<CRecord> cRecords)
        {
            // retrieve changes sets (modified entities)
            var aRecordChangeSet = aRecords.GetChangeSet();
            var bRecordChangeSet = bRecords.GetChangeSet();
            var cRecordChangeSet = cRecords.GetChangeSet();

            // reduce entities (copy changed values)
            var aRecordsMap = aRecordChangeSet.ReduceToModifications();
            var bRecordsMap = bRecordChangeSet.ReduceToModifications();
            var cRecordsMap = cRecordChangeSet.ReduceToModifications();

            // fixup relations (replaces related entities with reduced entites)
            this.FixupRelations(
                this.Union(aRecordsMap.CastToEntityTuple(), bRecordsMap.CastToEntityTuple(), cRecordsMap.CastToEntityTuple()),
                this.Union(aRecordChangeSet, bRecordChangeSet, cRecordChangeSet)
            );
            if (aRecordsMap.Count > 0) this.ARecords = aRecordsMap.Select(e => e.Item2).ToList();
            if (bRecordsMap.Count > 0) this.BRecords = bRecordsMap.Select(e => e.Item2).ToList();
            if (cRecordsMap.Count > 0) this.CRecords = cRecordsMap.Select(e => e.Item2).ToList();
        }

        protected ConcurrencyTestChangeSet(ConcurrencyTestChangeSet changeSet)
        {
            this.ARecords = changeSet.ARecords;
            this.BRecords = changeSet.BRecords;
            this.CRecords = changeSet.CRecords;
        }

        #endregion Constructor

        #region DataMember

        [DataMember]
        public List<ARecord> ARecords { get; private set; }

        [DataMember]
        public List<BRecord> BRecords { get; private set; }

        [DataMember]
        public List<CRecord> CRecords { get; private set; }

        #endregion DataMember

        #region IsEmpty

        public bool IsEmpty
        {
            get
            {
                return ARecords == null &&
                    BRecords == null &&
                    CRecords == null;
            }
        }

        #endregion IsEmpty

        #region IEnumerable

        public IEnumerator<Entity> GetEnumerator()
        {
            if (ARecords != null && ARecords.Count > 0)
            {
                foreach (var item in ARecords)
                {
                    yield return item;
                }
            }

            if (BRecords != null && BRecords.Count > 0)
            {
                foreach (var item in BRecords)
                {
                    yield return item;
                }
            }

            if (CRecords != null && CRecords.Count > 0)
            {
                foreach (var item in CRecords)
                {
                    yield return item;
                }
            }

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable
    }
}
