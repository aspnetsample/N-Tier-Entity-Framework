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
    [Serializable]
    [DataContract(IsReference = true)]
    public partial class BRecord : Entity<BRecord>, INotifyPropertyChanged, INotifyPropertyChanging, IDataErrorInfo
    {
        #region Constructor and Initialization

        // partial method for initialization
        partial void Initialize();

        public BRecord()
        {
            Initialize();
        }

        #endregion Constructor and Initialization

        #region Simple Properties

        [DataMember]
        [Key]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [ServerGeneration(ServerGenerationTypes.Insert)]
        [SimpleProperty]
        public global::System.Int64 Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    //if (!IsDeserializing && ChangeTracker.IsChangeTrackingEnabled)
                    if (!IsDeserializing && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'Id' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    IdChanging(value);
                    OnPropertyChanging("Id", value);
                    var previousValue = _id;
                    _id = value;
                    OnPropertyChanged("Id", previousValue, value);
                    IdChanged(previousValue);
                }
            }
        }
        private global::System.Int64 _id;

        partial void IdChanging(global::System.Int64 newValue);
        partial void IdChanged(global::System.Int64 previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    ValueChanging(value);
                    OnPropertyChanging("Value", value);
                    var previousValue = _value;
                    _value = value;
                    OnPropertyChanged("Value", previousValue, value);
                    ValueChanged(previousValue);
                }
            }
        }
        private global::System.String _value;

        partial void ValueChanging(global::System.String newValue);
        partial void ValueChanged(global::System.String previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [ServerGeneration(ServerGenerationTypes.Insert | ServerGenerationTypes.Update)]
        [ConcurrencyProperty]
        [SimpleProperty]
        public global::System.Guid Key
        {
            get { return _key; }
            private set
            {
                if (_key != value)
                {
                    //RecordOriginalValue("Key", _key);
                    KeyChanging(value);
                    OnPropertyChanging("Key", value);
                    var previousValue = _key;
                    _key = value;
                    OnPropertyChanged("Key", previousValue, value);
                    KeyChanged(previousValue);
                }
            }
        }
        private global::System.Guid _key;

        partial void KeyChanging(global::System.Guid newValue);
        partial void KeyChanged(global::System.Guid previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.DateTime ChangedDate
        {
            get { return _changedDate; }
            set
            {
                if (_changedDate != value)
                {
                    ChangedDateChanging(value);
                    OnPropertyChanging("ChangedDate", value);
                    var previousValue = _changedDate;
                    _changedDate = value;
                    OnPropertyChanged("ChangedDate", previousValue, value);
                    ChangedDateChanged(previousValue);
                }
            }
        }
        private global::System.DateTime _changedDate;

        partial void ChangedDateChanging(global::System.DateTime newValue);
        partial void ChangedDateChanged(global::System.DateTime previousValue);

        #endregion Simple Properties

        #region Complex Properties

        #endregion Complex Properties

        #region Navigation Properties

        #endregion Navigation Properties

        #region ChangeTracking

        protected override void ClearNavigationProperties()
        {
        }

        #endregion ChangeTracking

        #region Association Fixup

        #endregion Association Fixup

        protected override bool IsKeyEqual(BRecord entity)
        {
            return this.Id == entity.Id;
        }

        protected override int GetKeyHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
