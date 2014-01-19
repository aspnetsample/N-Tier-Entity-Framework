﻿//------------------------------------------------------------------------------
// <autogenerated>
//   This file was generated by T4 code generator NTierDemoModel_NTierEntityGenerator.tt.
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

namespace NTierDemo.Common.Domain.Model.NTierDemo
{
    [DataContract(IsReference = true)]
    public sealed partial class NTierDemoResultSet : NTierDemoChangeSet, IResultSet
    {
        #region ctor
        public NTierDemoResultSet(NTierDemoChangeSet changeSet)
            : base(changeSet)
        {
        }
        #endregion

        #region DataMember

        [DataMember]
        public IList<Author> AuthorConcurrencyConflicts { get; private set; }

        [DataMember]
        public IList<Blog> BlogConcurrencyConflicts { get; private set; }

        [DataMember]
        public IList<Post> PostConcurrencyConflicts { get; private set; }

        [DataMember]
        public IList<PostInfo> PostInfoConcurrencyConflicts { get; private set; }

        #endregion

        #region AddConcurrencyConflicts

        public void AddConcurrencyConflicts(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is Author)
                {
                    AddConcurrencyConflict((Author)entity);
                }
                else if (entity is Blog)
                {
                    AddConcurrencyConflict((Blog)entity);
                }
                else if (entity is Post)
                {
                    AddConcurrencyConflict((Post)entity);
                }
                else if (entity is PostInfo)
                {
                    AddConcurrencyConflict((PostInfo)entity);
                }
                else
                {
                    throw new Exception(string.Format("Unknown type {0}", entity.GetType().Name));
                }
            }
        }

        private void AddConcurrencyConflict(Author entity)
        {
            if (AuthorConcurrencyConflicts == null)
            {
                AuthorConcurrencyConflicts = new List<Author>();
            }
            AuthorConcurrencyConflicts.Add(entity);
        }

        private void AddConcurrencyConflict(Blog entity)
        {
            if (BlogConcurrencyConflicts == null)
            {
                BlogConcurrencyConflicts = new List<Blog>();
            }
            BlogConcurrencyConflicts.Add(entity);
        }

        private void AddConcurrencyConflict(Post entity)
        {
            if (PostConcurrencyConflicts == null)
            {
                PostConcurrencyConflicts = new List<Post>();
            }
            PostConcurrencyConflicts.Add(entity);
        }

        private void AddConcurrencyConflict(PostInfo entity)
        {
            if (PostInfoConcurrencyConflicts == null)
            {
                PostInfoConcurrencyConflicts = new List<PostInfo>();
            }
            PostInfoConcurrencyConflicts.Add(entity);
        }

        #endregion

        #region IsConcurrencyConflict

        public bool IsConcurrencyConflict(Author e)
        {
            return AuthorConcurrencyConflicts != null && AuthorConcurrencyConflicts.Count > 0 && AuthorConcurrencyConflicts.Contains(e);
        }

        public bool IsConcurrencyConflict(Blog e)
        {
            return BlogConcurrencyConflicts != null && BlogConcurrencyConflicts.Count > 0 && BlogConcurrencyConflicts.Contains(e);
        }

        public bool IsConcurrencyConflict(Post e)
        {
            return PostConcurrencyConflicts != null && PostConcurrencyConflicts.Count > 0 && PostConcurrencyConflicts.Contains(e);
        }

        public bool IsConcurrencyConflict(PostInfo e)
        {
            return PostInfoConcurrencyConflicts != null && PostInfoConcurrencyConflicts.Count > 0 && PostInfoConcurrencyConflicts.Contains(e);
        }

        #endregion

        #region HasConcurrencyConflicts
        public bool HasConcurrencyConflicts
        {
            get
            {
                return AuthorConcurrencyConflicts != null ||
                    BlogConcurrencyConflicts != null ||
                    PostConcurrencyConflicts != null ||
                    PostInfoConcurrencyConflicts != null;
            }
        }
        #endregion
    }
}