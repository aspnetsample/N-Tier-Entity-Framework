﻿//------------------------------------------------------------------------------
// <autogenerated>
//   This file was generated by T4 code generator NTierDemoModel_NTierEntityGenerator.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using NTier.Client.Domain;
using NTierDemo.Common.Domain.Model.NTierDemo;

namespace NTierDemo.Client.Domain
{
    public partial interface INTierDemoDataContext : IDataContext
    {

        #region Users

        IEntitySet<User> Users { get; }

        void Add(User entity);
        void Delete(User entity);
        void Attach(User entity);
        void AttachAsModified(User entity, User original);
        void Detach(User entity);

        #endregion Users

        #region Blogs

        IEntitySet<Blog> Blogs { get; }

        void Add(Blog entity);
        void Delete(Blog entity);
        void Attach(Blog entity);
        void AttachAsModified(Blog entity, Blog original);
        void Detach(Blog entity);

        #endregion Blogs

        #region Posts

        IEntitySet<Post> Posts { get; }

        void Add(Post entity);
        void Delete(Post entity);
        void Attach(Post entity);
        void AttachAsModified(Post entity, Post original);
        void Detach(Post entity);

        #endregion Posts

    }
}
