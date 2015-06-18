﻿// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories.EntityFramework
{
    internal static class SelfTrackingEntitiesContextExtensions
    {
        /// <summary>
        /// ApplyChanges takes the changes in a connected set of entities and applies them to an ObjectContext.
        /// </summary>
        /// <typeparam name="TEntity">Expected type of the ObjectSet</typeparam>
        /// <param name="objectSet">The ObjectSet referencing the ObjectContext to which changes will be applied.</param>
        /// <param name="entity">The entity serving as the entry point of the object graph that contains changes.</param>
        public static void ApplyChanges<TEntity>(this ObjectSet<TEntity> objectSet, TEntity entity) where TEntity : class, IObjectWithChangeTracker
        {
            if (objectSet == null)
            {
                throw new ArgumentNullException("objectSet");
            }

            objectSet.Context.ApplyChanges<TEntity>(objectSet.EntitySet.EntityContainer.Name + "." + objectSet.EntitySet.Name, entity);
        }

        /// <summary>
        /// ApplyChanges takes the changes in a connected set of entities and applies them to an ObjectContext.
        /// </summary>
        /// <typeparam name="TEntity">Expected type of the EntitySet</typeparam>
        /// <param name="context">The ObjectContext to which changes will be applied.</param>
        /// <param name="entitySetName">The EntitySet name of the entity.</param>
        /// <param name="entity">The entity serving as the entry point of the object graph that contains changes.</param>
        public static void ApplyChanges<TEntity>(this ObjectContext context, string entitySetName, TEntity entity) where TEntity : IObjectWithChangeTracker
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (String.IsNullOrEmpty(entitySetName))
            {
                throw new ArgumentException("String parameter cannot be null or empty.", "entitySetName");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            bool lazyLoadingSetting = context.ContextOptions.LazyLoadingEnabled;
            try
            {
                context.ContextOptions.LazyLoadingEnabled = false;

                EntityIndex entityIndex = AddHelper.AddAllEntities(context, entitySetName, entity);
                RelationshipSet allRelationships = new RelationshipSet(context, entityIndex.AllEntities);

                #region Handle Initial Entity State

                foreach (IObjectWithChangeTracker changedEntity in entityIndex.AllEntities.Where(x => x.ChangeTracker.State == ObjectState.Deleted))
                {
                    HandleDeletedEntity(context, entityIndex, allRelationships, changedEntity);
                }

                foreach (IObjectWithChangeTracker changedEntity in entityIndex.AllEntities.Where(x => x.ChangeTracker.State != ObjectState.Deleted))
                {
                    HandleEntity(context, entityIndex, allRelationships, changedEntity);
                }

                #endregion

                #region Loop through each object state entries

                foreach (IObjectWithChangeTracker changedEntity in entityIndex.AllEntities)
                {
                    ObjectStateEntry entry = context.ObjectStateManager.GetObjectStateEntry(changedEntity);

                    EntityType entityType = context.MetadataWorkspace.GetCSpaceEntityType(changedEntity.GetType());

                    foreach (NavigationProperty navProp in entityType.NavigationProperties)
                    {
                        RelatedEnd relatedEnd = entry.GetRelatedEnd(navProp.Name);
                        if (!((AssociationType)relatedEnd.RelationshipSet.ElementType).IsForeignKey)
                        {
                            ApplyChangesToIndependentAssociation(context, (IObjectWithChangeTracker)changedEntity, entry, navProp, relatedEnd, allRelationships);
                        }
                    }
                }
                #endregion

                // Change all the remaining relationships to the appropriate state
                foreach (var relationship in allRelationships)
                {
                    context.ObjectStateManager.ChangeRelationshipState(
                        relationship.End0,
                        relationship.End1,
                        relationship.AssociationSet.ElementType.FullName,
                        relationship.AssociationEndMembers[1].Name,
                        relationship.State);
                }
            }
            finally
            {
                context.ContextOptions.LazyLoadingEnabled = lazyLoadingSetting;
            }
        }

        private static void ApplyChangesToIndependentAssociation(ObjectContext context, IObjectWithChangeTracker changedEntity, ObjectStateEntry entry, NavigationProperty navProp,
            IRelatedEnd relatedEnd, RelationshipSet allRelationships)
        {
            ObjectChangeTracker changeTracker = changedEntity.ChangeTracker;

            if (changeTracker.State == ObjectState.Added)
            {
                // Relationships should remain added so remove them from the list of allRelationships
                foreach (object relatedEntity in relatedEnd)
                {
                    ObjectStateEntry addedRelationshipEntry =
                                context.ObjectStateManager.ChangeRelationshipState(
                                    changedEntity,
                                    relatedEntity,
                                    navProp.Name,
                                    EntityState.Added);

                    allRelationships.Remove(addedRelationshipEntry);
                }
            }
            else
            {
                if (navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                {
                    //Handle removal to FixupCollections
                    EntityList collectionPropertyChanges = null;
                    if (changeTracker.ObjectsRemovedFromCollectionProperties.TryGetValue(navProp.Name, out collectionPropertyChanges))
                    {
                        foreach (var removedEntityFromAssociation in collectionPropertyChanges)
                        {
                            ObjectStateEntry deletedRelationshipEntry =
                                context.ObjectStateManager.ChangeRelationshipState(
                                    changedEntity,
                                    removedEntityFromAssociation,
                                    navProp.Name,
                                    EntityState.Deleted);

                            allRelationships.Remove(deletedRelationshipEntry);
                        }
                    }

                    //Handle addition to FixupCollection
                    if (changeTracker.ObjectsAddedToCollectionProperties.TryGetValue(navProp.Name, out collectionPropertyChanges))
                    {
                        foreach (var addedEntityFromAssociation in collectionPropertyChanges)
                        {
                            ObjectStateEntry addedRelationshipEntry =
                                context.ObjectStateManager.ChangeRelationshipState(
                                    changedEntity,
                                    addedEntityFromAssociation,
                                    navProp.Name,
                                    EntityState.Added);

                            allRelationships.Remove(addedRelationshipEntry);
                        }
                    }
                }
                else
                {
                    // Handle original relationship values
                    object originalReferenceValue;
                    if (changeTracker.OriginalValues.TryGetValue(navProp.Name, out originalReferenceValue))
                    {
                        if (originalReferenceValue != null)
                        {
                            //Capture the deletion of association
                            ObjectStateEntry deletedRelationshipEntry =
                                context.ObjectStateManager.ChangeRelationshipState(
                                    entry.Entity,
                                    originalReferenceValue,
                                    navProp.Name,
                                    EntityState.Deleted);

                            allRelationships.Remove(deletedRelationshipEntry);
                        }

                        //Capture the Addition of association
                        object currentReferenceValue = null;
                        foreach (object o in relatedEnd)
                        {
                            currentReferenceValue = o;
                            break;
                        }
                        if (currentReferenceValue != null)
                        {
                            ObjectStateEntry addedRelationshipEntry =
                                context.ObjectStateManager.ChangeRelationshipState(
                                    changedEntity,
                                    currentReferenceValue,
                                    navProp.Name,
                                    EntityState.Added);

                            allRelationships.Remove(addedRelationshipEntry);
                        }
                        // if the current value of the reference is null, then the user must set the entity reference to null
                        // which is already being handled by the deletion of the relationship
                    }
                }
            }
        }

        // Extracts the relationship key information from the ExtendedProperties and OriginalValues records of each ObjectChangeTracker
        // This is done by:
        //  1. Creating any existing relationship specified in the ExtendedProperties
        //  2. Determine if there was a previous relationship, and if there was create a deleted relationship between the entity and the previous entity or key value
        private static void HandleRelationshipKeys(ObjectContext context, EntityIndex entityIndex, RelationshipSet allRelationships, IObjectWithChangeTracker entity)
        {
            ObjectChangeTracker changeTracker = entity.ChangeTracker;
            if (changeTracker.State == ObjectState.Unchanged ||
                changeTracker.State == ObjectState.Modified ||
                changeTracker.State == ObjectState.Deleted)
            {
                ObjectStateEntry entry = context.ObjectStateManager.GetObjectStateEntry(entity);
                EntityType entityType = context.MetadataWorkspace.GetCSpaceEntityType(entity.GetType());
                RelationshipManager relationshipManager = context.ObjectStateManager.GetRelationshipManager(entity);

                foreach (var entityReference in EnumerateSaveReferences(relationshipManager))
                {
                    AssociationSet associationSet = ((AssociationSet)entityReference.RelationshipSet);
                    AssociationEndMember fromEnd = associationSet.AssociationSetEnds[entityReference.SourceRoleName].CorrespondingAssociationEndMember;
                    AssociationEndMember toEnd = associationSet.AssociationSetEnds[entityReference.TargetRoleName].CorrespondingAssociationEndMember;

                    // Find if there is a NavigationProperty for this candidate
                    NavigationProperty navigationProperty = entityType.NavigationProperties.
                                               SingleOrDefault(x => x.RelationshipType == associationSet.ElementType &&
                                                               x.FromEndMember == fromEnd &&
                                                               x.ToEndMember == toEnd);

                    // Only handle relationship keys in one of these cases
                    // 1. There is no navigation property
                    // 2. The navigation property has a null current reference value and there are no removes or adds
                    // 3. The navigation property has a current reference value, but there is no remove

                    EntityKey currentKey = GetSavedReferenceKey(entityIndex, entityReference, entity, navigationProperty, changeTracker.ExtendedProperties);

                    // Get any original value from the change tracking information
                    object originalValue = null;
                    EntityKey originalKey = null;
                    bool hasOriginalValue = false;
                    if (changeTracker.OriginalValues != null)
                    {
                        // Try to get the original value from the NavigationProperty first
                        if (navigationProperty != null)
                        {
                            hasOriginalValue = changeTracker.OriginalValues.TryGetValue(navigationProperty.Name, out originalValue);
                        }
                        // Try to get the original value from the reference key second
                        if (!hasOriginalValue || originalValue == null)
                        {
                            originalKey = GetSavedReferenceKey(entityIndex, entityReference, entity, navigationProperty, changeTracker.OriginalValues);
                        }
                    }

                    // Create the current relationship
                    if (currentKey != null)
                    {
                        // If the key is for a deleted entity, move that key to an originalValue and fixup the entities key values
                        // Otherwise create a new relationship
                        ObjectStateEntry currentEntry;
                        if (context.ObjectStateManager.TryGetObjectStateEntry(currentKey, out currentEntry) &&
                           currentEntry.Entity != null &&
                           currentEntry.State == EntityState.Deleted)
                        {
                            entityReference.EntityKey = null;
                            MoveSavedReferenceKey(entityReference, entity, navigationProperty, changeTracker.ExtendedProperties, changeTracker.OriginalValues);
                            originalKey = currentKey;
                        }
                        else
                        {
                            CreateRelationship(context, entityReference, entry.EntityKey, currentKey, originalKey == null ? EntityState.Unchanged : EntityState.Added);
                        }
                    }
                    else
                    {
                        // Find the current key
                        // Cannot get the EntityKey directly because this is null when it points to an Added entity
                        currentKey = entityReference.GetCurrentEntityKey(context);
                    }

                    // Create the original relationship
                    if (originalKey != null)
                    {
                        // If the key is for a deleted entity, remember to create a deleted relationship,
                        // otherwise use the entityReference to setup the deleted relationship
                        ObjectStateEntry originalEntry = null;
                        ObjectStateEntry deletedRelationshipEntry = null;
                        if (context.ObjectStateManager.TryGetObjectStateEntry(originalKey, out originalEntry) &&
                           originalEntry.Entity != null &&
                           originalEntry.State == EntityState.Deleted)
                        {
                            allRelationships.Add(entityReference, entry.Entity, originalEntry.Entity, EntityState.Deleted);
                        }
                        else
                        {
                            // To create a deleted relationship to a key, first detach the existing relationship between entry and currentKey
                            EntityState currentRelationshipState = DetachRelationship(context, entityReference, entry, currentKey);

                            // If the relationship is 1 to 0..1, detach the relationship from currentKey to its target (targetKey)
                            EntityState targetRelationshipState = EntityState.Detached;
                            EntityReference targetReference = null;
                            EntityKey targetKey = null;
                            if (originalEntry != null &&
                                originalEntry.Entity != null &&
                                originalEntry.RelationshipManager != null &&
                                associationSet.AssociationSetEnds[fromEnd.Name].CorrespondingAssociationEndMember.RelationshipMultiplicity != RelationshipMultiplicity.Many)
                            {
                                targetReference = originalEntry.RelationshipManager.GetRelatedEnd(entityReference.RelationshipName, entityReference.SourceRoleName) as EntityReference;
                                targetKey = targetReference.GetCurrentEntityKey(context);
                                if (targetKey != null)
                                {
                                    targetRelationshipState = DetachRelationship(context, targetReference, originalEntry, targetKey);
                                }
                            }


                            // Create the deleted relationship between entry and originalKey
                            deletedRelationshipEntry = CreateRelationship(context, entityReference, entry.EntityKey, originalKey, EntityState.Deleted);

                            // Set the previous relationship between entry and currentKey back
                            CreateRelationship(context, entityReference, entry.EntityKey, currentKey, currentRelationshipState);

                            // Set the previous relationship between originalEntry and targetKey back
                            if (targetKey != null)
                            {
                                CreateRelationship(context, targetReference, originalEntry.EntityKey, targetKey, targetRelationshipState);
                            }
                        }
                        if (deletedRelationshipEntry != null)
                        {
                            // Remove the deleted relationship from those that need to be processed later in ApplyChanges
                            allRelationships.Remove(deletedRelationshipEntry);
                        }
                    }
                    else if (currentKey == null && originalValue != null && entityReference.IsDependentEndOfReferentialConstraint())
                    {
                        // the graph won't have this hooked up because there is no current value, but there is an original value,
                        // so the relationship processing code will want to delete a relationship.
                        // we can add this one so it has a relationship to change to deleted.
                        context.ObjectStateManager.ChangeRelationshipState(
                                                            entry.Entity,
                                                            originalValue,
                                                            entityReference.RelationshipName,
                                                            entityReference.TargetRoleName,
                                                            EntityState.Added);
                    }
                }
            }
        }

        private static ObjectStateEntry CreateRelationship(ObjectContext context, EntityReference entityReference, EntityKey fromKey, EntityKey toKey, EntityState state)
        {
            if (state != EntityState.Detached)
            {
                AssociationSet associationSet = ((AssociationSet)entityReference.RelationshipSet);
                AssociationEndMember fromEnd = associationSet.AssociationSetEnds[entityReference.SourceRoleName].CorrespondingAssociationEndMember;
                AssociationEndMember toEnd = associationSet.AssociationSetEnds[entityReference.TargetRoleName].CorrespondingAssociationEndMember;

                // set the relationship to the original relationship in the unchanged state
                Debug.Assert(toKey != null, "why/how would we do a delete with a null originalKey?");

                if (toKey.IsTemporary)
                {
                    // Clear any existing relationship
                    entityReference.EntityKey = null;

                    // If the target entity is Added, use Add on RelatedEnd
                    ObjectStateEntry targetEntry;
                    context.ObjectStateManager.TryGetObjectStateEntry(toKey, out targetEntry);
                    Debug.Assert(targetEntry != null, "Should have found the state entry");
                    ((IRelatedEnd)entityReference).Add(targetEntry.Entity);
                }
                else
                {
                    entityReference.EntityKey = toKey;
                }

                ObjectStateEntry relationshipEntry;
                bool found = context.TryGetObjectStateEntry(fromKey, toKey, associationSet, fromEnd, toEnd, out relationshipEntry);
                Debug.Assert(found, "Did not find the created relationship.");

                switch (state)
                {
                    case EntityState.Added:
                        break;
                    case EntityState.Unchanged:
                        relationshipEntry.AcceptChanges();
                        break;
                    case EntityState.Deleted:
                        relationshipEntry.AcceptChanges();
                        entityReference.EntityKey = null;
                        break;
                }
                return relationshipEntry;
            }
            return null;
        }

        private static EntityState DetachRelationship(ObjectContext context, EntityReference entityReference, ObjectStateEntry fromEntry, EntityKey toKey)
        {
            EntityState currentRelationshipState = EntityState.Detached;

            if (toKey != null)
            {
                AssociationSet associationSet = ((AssociationSet)entityReference.RelationshipSet);
                AssociationEndMember fromEnd = associationSet.AssociationSetEnds[entityReference.SourceRoleName].CorrespondingAssociationEndMember;
                AssociationEndMember toEnd = associationSet.AssociationSetEnds[entityReference.TargetRoleName].CorrespondingAssociationEndMember;

                ObjectStateEntry currentRelationshipEntry = null;

                if (context.TryGetObjectStateEntry(fromEntry.EntityKey, toKey, associationSet, fromEnd, toEnd, out currentRelationshipEntry))
                {
                    currentRelationshipState = currentRelationshipEntry.State;

                    entityReference.EntityKey = null;
                    if (currentRelationshipEntry.State == EntityState.Deleted)
                    {
                        currentRelationshipEntry.AcceptChanges();
                    }
                    Debug.Assert(currentRelationshipEntry.State == EntityState.Detached, "relationship was not detached");
                }
            }
            return currentRelationshipState;
        }

        private static string CreateReferenceKeyLookup(string keyMemberName, EntityReference reference, NavigationProperty navigationProperty)
        {
            // use the more usable navigation property name to qualify the member
            // if available
            if (navigationProperty != null)
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}.{1}", navigationProperty.Name, keyMemberName);
            }
            else
            {
                return String.Format(CultureInfo.InvariantCulture, "Navigate({0}.{1}).{2}", reference.RelationshipSet.ElementType.FullName, reference.TargetRoleName, keyMemberName);
            }
        }

        // retrieves the key corresponding to the passed in EntityReference
        // these keys can be set during the ObjectMaterialized event or through relationship fixup
        private static EntityKey GetSavedReferenceKey(EntityIndex entityIndex, EntityReference reference, object entity, NavigationProperty navigationProperty, IDictionary<string, object> values)
        {
            Debug.Assert(navigationProperty == null || reference.RelationshipSet.ElementType == navigationProperty.RelationshipType, "the reference and navigationProperty should coorospond");

            EntitySet entitySet = ((AssociationSet)reference.RelationshipSet).AssociationSetEnds[reference.TargetRoleName].EntitySet;

            List<EntityKeyMember> foundKeyMembers = new List<EntityKeyMember>(1);
            bool foundNone = true;
            bool missingSome = false;
            foreach (var keyMember in entitySet.ElementType.KeyMembers)
            {
                string lookupKey = CreateReferenceKeyLookup(keyMember.Name, reference, navigationProperty);
                object value;
                if (values.TryGetValue(lookupKey, out value))
                {
                    foundKeyMembers.Add(new EntityKeyMember(keyMember.Name, value));
                    foundNone = false;
                }
                else
                {
                    missingSome = true;
                }
            }

            if (foundNone)
            {
                // we didn't find a key
                return null;
            }
            else if (missingSome)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "The OriginalValues or ExtendedProperties collections on the type '{0}' contained only a partial key to satisfy the relationship '{1}' targeting the role '{2}'",
                        entity.GetType().FullName,
                        reference.RelationshipName,
                        reference.TargetRoleName));
            }

            EntityKey key = entityIndex.ConvertEntityKey(new EntityKey(reference.GetEntitySetName(), foundKeyMembers));
            return key;
        }

        // Moves the key corresponding to the passed in EntityReference from a source collection to a target collection
        private static void MoveSavedReferenceKey(EntityReference reference, object entity, NavigationProperty navigationProperty, IDictionary<string, object> sourceValues, IDictionary<string, object> targetValues)
        {
            Debug.Assert(navigationProperty == null || reference.RelationshipSet.ElementType == navigationProperty.RelationshipType, " the reference and navigationProperty should correspond");

            EntitySet entitySet = ((AssociationSet)reference.RelationshipSet).AssociationSetEnds[reference.TargetRoleName].EntitySet;

            bool missingSome = false;
            foreach (var keyMember in entitySet.ElementType.KeyMembers)
            {
                string lookupKey = CreateReferenceKeyLookup(keyMember.Name, reference, navigationProperty);
                object value;
                if (sourceValues.TryGetValue(lookupKey, out value))
                {
                    if (targetValues.ContainsKey(lookupKey))
                    {
                        targetValues[lookupKey] = value;
                    }
                    else
                    {
                        targetValues.Add(lookupKey, value);
                    }
                    sourceValues.Remove(lookupKey);
                }
                else
                {
                    missingSome = true;
                }
            }

            if (missingSome)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        " The OriginalValues or ExtendedProperties collections on the type '{0}' contained only a partial key to satisfy the relationship '{1}' targeting the role '{2}'",
                        entity.GetType().FullName,
                        reference.RelationshipName,
                        reference.TargetRoleName));
            }
        }

        private static IEnumerable<EntityReference> EnumerateSaveReferences(RelationshipManager manager)
        {
            return manager.GetAllRelatedEnds().OfType<EntityReference>()
                    .Where(er => er.RelationshipSet.ElementType.RelationshipEndMembers[er.SourceRoleName].RelationshipMultiplicity != RelationshipMultiplicity.One &&
                        !((AssociationSet)er.RelationshipSet).ElementType.IsForeignKey);
        }
        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static void StoreReferenceKeyValues(this ObjectContext context, IObjectWithChangeTracker entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            ObjectStateEntry entry;
            if (!context.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
            {
                // must be a no tracking query, the reference key info won't be available
                return;
            }

            var relationshipManager = entry.RelationshipManager;
            EntityType entityType = context.MetadataWorkspace.GetCSpaceEntityType(entity.GetType());
            foreach (EntityReference entityReference in EnumerateSaveReferences(relationshipManager))
            {
                NavigationProperty navigationProperty = entityType.NavigationProperties.FirstOrDefault(n => n.RelationshipType == entityReference.RelationshipSet.ElementType &&
                        n.FromEndMember.Name == entityReference.SourceRoleName &&
                        n.ToEndMember.Name == entityReference.TargetRoleName);

                object value = entityReference.GetValue();
                if ((navigationProperty == null || value == null) && entityReference.EntityKey != null)
                {
                    foreach (var item in entityReference.EntityKey.EntityKeyValues)
                    {
                        string key = CreateReferenceKeyLookup(item.Key, entityReference, navigationProperty);
                        entity.ChangeTracker.ExtendedProperties.Add(key, item.Value);
                    }
                }
            }
        }

        private static void HandleEntity(ObjectContext context, EntityIndex entityIndex, RelationshipSet allRelationships, IObjectWithChangeTracker entity)
        {
            ChangeEntityStateBasedOnObjectState(context, entity);
            HandleRelationshipKeys(context, entityIndex, allRelationships, entity);
            UpdateOriginalValues(context, entity);
        }

        private static void HandleDeletedEntity(ObjectContext context, EntityIndex entityIndex, RelationshipSet allRelationships, IObjectWithChangeTracker entity)
        {
            HandleRelationshipKeys(context, entityIndex, allRelationships, entity);
            ChangeEntityStateBasedOnObjectState(context, entity);
            UpdateOriginalValues(context, entity);
        }

        private static void UpdateOriginalValues(ObjectContext context, IObjectWithChangeTracker entity)
        {
            if (entity.ChangeTracker.State == ObjectState.Unchanged ||
                entity.ChangeTracker.State == ObjectState.Added ||
                entity.ChangeTracker.OriginalValues == null)
            {
                // nothing to do here
                return;
            }

            // we only need/want to deal with scalar and complex properties

            ObjectStateEntry entry = context.ObjectStateManager.GetObjectStateEntry(entity);
            OriginalValueRecord originalValueRecord = entry.GetUpdatableOriginalValues();
            EntityType entityType = context.MetadataWorkspace.GetCSpaceEntityType(entity.GetType());

            // walk through each property and see if we have an original value for it
            // set it if we do.  Walk down through ComplexType properties to set original values
            // for each of them also
            //
            // it is expected that the original values will be sparse because we are trying
            // to only capture originals for the ones we are required to have (concurency, sproc, condition, more?)
            foreach (EdmProperty property in entityType.Properties)
            {
                object value;
                if (property.TypeUsage.EdmType is PrimitiveType && entity.ChangeTracker.OriginalValues.TryGetValue(property.Name, out value))
                {
                    originalValueRecord.SetValue(property, value);
                }
                else if (property.TypeUsage.EdmType is ComplexType)
                {
                    OriginalValueRecord complexOriginalValues = originalValueRecord.GetOriginalValueRecord(property.Name);
                    UpdateOriginalValues((ComplexType)property.TypeUsage.EdmType, entity.GetType().FullName, property.Name, entity.ChangeTracker.OriginalValues, complexOriginalValues);
                }
            }
        }

        private static void UpdateOriginalValues(ComplexType complexType, string entityTypeName, string propertyPathToType, IDictionary<string, object> originalValueSource, OriginalValueRecord complexOriginalValueRecord)
        {
            // Note that complexOriginalValueRecord may be null
            // a null complexOriginalValueRecord will only occur if a null reference is assigned
            // to a ComplexType property and then given to ApplyChanges.
            //
            // walk through each property and see if we have an original value for it
            // set it if we do.  Walk down through ComplexType properties to set original values
            // for each of them also
            foreach (EdmProperty property in complexType.Properties)
            {
                object value;
                string propertyPath = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", propertyPathToType, property.Name);
                if (property.TypeUsage.EdmType is PrimitiveType && originalValueSource.TryGetValue(propertyPath, out value))
                {
                    if (complexOriginalValueRecord != null)
                    {
                        complexOriginalValueRecord.SetValue(property, value);
                    }
                    else if (value != null)
                    {
                        Debug.Assert(complexOriginalValueRecord == null, "we only throw when the value is not null and the recored is null");
                        throw new InvalidOperationException(
                            String.Format(
                            CultureInfo.CurrentCulture,
                            "Can not set the original value on the object stored in the property '{0}' on the type '{1}' because the property is null.",
                            propertyPathToType,
                            entityTypeName));
                    }
                }
                else if (property.TypeUsage.EdmType is ComplexType)
                {
                    OriginalValueRecord nestedOriginalValueRecord = null;
                    if (complexOriginalValueRecord != null)
                    {
                        nestedOriginalValueRecord = complexOriginalValueRecord.GetOriginalValueRecord(property.Name);
                    }
                    // recurse down the chain of complex types...
                    UpdateOriginalValues((ComplexType)property.TypeUsage.EdmType, entityTypeName, propertyPath, originalValueSource, nestedOriginalValueRecord);
                }
            }
        }

        private static OriginalValueRecord GetOriginalValueRecord(this OriginalValueRecord record, string name)
        {
            int ordinal = record.GetOrdinal(name);
            if (!record.IsDBNull(ordinal))
            {
                return record.GetDataRecord(ordinal) as OriginalValueRecord;
            }
            else
            {
                return null;
            }
        }

        private static void SetValue(this OriginalValueRecord record, EdmProperty edmProperty, object value)
        {
            if (value == null)
            {
                Type entityClrType = ((PrimitiveType)edmProperty.TypeUsage.EdmType).ClrEquivalentType;
                if (entityClrType.IsValueType &&
                    !(entityClrType.IsGenericType && typeof(Nullable<>) == entityClrType.GetGenericTypeDefinition()))
                {
                    // Skip setting null original values on non-nullable CLR types because the ObjectStateEntry won't allow this
                    return;
                }
            }

            int ordinal = record.GetOrdinal(edmProperty.Name);
            record.SetValue(ordinal, value);
        }


        private static void ChangeEntityStateBasedOnObjectState(ObjectContext context, IObjectWithChangeTracker entity)
        {
            switch (entity.ChangeTracker.State)
            {
                case (ObjectState.Added):
                    // No-op: the state entry is already marked as added
                    Debug.Assert(context.ObjectStateManager.GetObjectStateEntry(entity).State == EntityState.Added, "State should have been Added");
                    break;
                case (ObjectState.Unchanged):
                    context.ObjectStateManager.ChangeObjectState(entity, EntityState.Unchanged);
                    break;
                case (ObjectState.Modified):
                    //context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
                    context.ObjectStateManager.ChangeObjectState(entity, EntityState.Unchanged);
                    var ose = context.ObjectStateManager.GetObjectStateEntry(entity);
                    ose.SetModified();
                    foreach (var propertyName in entity.ChangeTracker.ModifiedProperties)
                    {
                        try
                        {
                            ose.SetModifiedProperty(propertyName);
                        }
                        catch (ArgumentException)
                        {
                            // note: exceptions of type ArgumentException are ignored to allow for submission of properties which are not part of entity model
                            // TODO: improve code to avoid this exception
                        }
                    }
                    break;
                case (ObjectState.Deleted):
                    context.ObjectStateManager.ChangeObjectState(entity, EntityState.Deleted);
                    break;

            }
        }

        private static EntityType GetCSpaceEntityType(this MetadataWorkspace workspace, Type type)
        {
            EntityType ospaceEntityType = null;
            StructuralType cspaceEntityType = null;
            EntityType entityType = null;
            if (workspace.TryGetItem<EntityType>(
                type.FullName,
                DataSpace.OSpace,
                out ospaceEntityType))
            {
                if (workspace.TryGetEdmSpaceType(
                    ospaceEntityType,
                    out cspaceEntityType))
                {
                    entityType = cspaceEntityType as EntityType;
                }
            }
            if (entityType == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Unable to find a CSpace type for type {0}", type.FullName));
            }
            return entityType;
        }

        private static object GetValue(this System.Data.Objects.DataClasses.EntityReference entityReference)
        {
            foreach (object value in entityReference)
            {
                return value;
            }
            return null;
        }

        private static EntityKey GetCurrentEntityKey(this System.Data.Objects.DataClasses.EntityReference entityReference, ObjectContext context)
        {
            EntityKey currentKey = null;
            object currentValue = entityReference.GetValue();
            if (currentValue != null)
            {
                ObjectStateEntry relatedEntry = context.ObjectStateManager.GetObjectStateEntry(currentValue);
                currentKey = relatedEntry.EntityKey;
            }
            else
            {
                currentKey = entityReference.EntityKey;
            }
            return currentKey;
        }

        private static RelatedEnd GetRelatedEnd(this ObjectStateEntry entry, string navigationPropertyIdentity)
        {
            NavigationProperty navigationProperty =
                            GetNavigationProperty(entry.ObjectStateManager.MetadataWorkspace.GetCSpaceEntityType(entry.Entity.GetType()), navigationPropertyIdentity);
            return entry.RelationshipManager.GetRelatedEnd(
                navigationProperty.RelationshipType.FullName, navigationProperty.ToEndMember.Name) as RelatedEnd;
        }

        private static NavigationProperty GetNavigationProperty(this EntityType entityType, string navigationPropertyIdentity)
        {
            NavigationProperty navigationProperty;
            if (!entityType.NavigationProperties.TryGetValue(navigationPropertyIdentity, false, out navigationProperty))
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "Could not find navigation property '{0}' in EntityType '{1}'.",
                        navigationPropertyIdentity,
                        entityType.FullName));
            }
            return navigationProperty;
        }

        private static string GetEntitySetName(this RelatedEnd relatedEnd)
        {
            EntitySet entitySet = ((AssociationSet)relatedEnd.RelationshipSet).AssociationSetEnds[relatedEnd.TargetRoleName].EntitySet;
            return entitySet.EntityContainer.Name + "." + entitySet.Name;
        }

        private static bool IsDependentEndOfReferentialConstraint(this RelatedEnd relatedEnd)
        {
            if (null != relatedEnd.RelationshipSet)
            {
                // NOTE Referential constraints collection will usually contains 0 or 1 element,
                // so performance shouldn't be an issue here
                foreach (ReferentialConstraint constraint in ((AssociationType)relatedEnd.RelationshipSet.ElementType).ReferentialConstraints)
                {
                    if (constraint.ToRole.Name == relatedEnd.SourceRoleName)
                    {
                        // Example:
                        //    Client<C_ID> --- Order<O_ID, Client_ID>
                        //    RI Constraint: Principal/From <Client.C_ID>,  Dependent/To <Order.Client_ID>
                        // When current RelatedEnd is a CollectionOrReference in Order's relationships,
                        // constarint.ToRole == this._fromEndProperty == Order
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool TryGetObjectStateEntry(this ObjectContext context, EntityKey from, EntityKey to, AssociationSet associationSet, AssociationEndMember fromEnd, AssociationEndMember toEnd, out ObjectStateEntry entry)
        {
            entry = null;
            foreach (var relationshipEntry in (from e in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Unchanged)
                                               where e.IsRelationship && e.EntitySet == associationSet
                                               select e))
            {
                CurrentValueRecord currentValues = relationshipEntry.CurrentValues;
                int fromOrdinal = currentValues.GetOrdinal(fromEnd.Name);
                int toOrdinal = currentValues.GetOrdinal(toEnd.Name);
                if (((EntityKey)currentValues.GetValue(fromOrdinal)) == from &&
                    ((EntityKey)currentValues.GetValue(toOrdinal)) == to)
                {
                    entry = relationshipEntry;
                    return true;
                }
            }
            return false;
        }

        private sealed class AddHelper
        {
            private readonly ObjectContext _context;
            private readonly EntityIndex _entityIndex;

            // Used during add processing
            private readonly Queue<Tuple<string, IObjectWithChangeTracker>> _entitiesToAdd;
            private readonly Queue<Tuple<ObjectStateEntry, string, IEnumerable<object>>> _entitiesDuringAdd;

            public static EntityIndex AddAllEntities(ObjectContext context, string entitySetName, IObjectWithChangeTracker entity)
            {
                AddHelper addHelper = new AddHelper(context);

                try
                {
                    // Include the root element to start the Apply
                    addHelper.QueueAdd(entitySetName, entity);

                    // Add everything
                    while (addHelper.HasMore)
                    {
                        Tuple<string, IObjectWithChangeTracker> entityInSet = addHelper.NextAdd();
                        // Only add the object if it's not already in the context
                        ObjectStateEntry entry = null;
                        if (!context.ObjectStateManager.TryGetObjectStateEntry(entityInSet.Item2, out entry))
                        {
                            context.AddObject(entityInSet.Item1, entityInSet.Item2);
                        }
                    }
                }
                finally
                {
                    addHelper.Detach();
                }
                return addHelper.EntityIndex;
            }

            private AddHelper(ObjectContext context)
            {
                _context = context;
                _context.ObjectStateManager.ObjectStateManagerChanged += this.HandleStateManagerChange;

                _entityIndex = new EntityIndex(context);
                _entitiesToAdd = new Queue<Tuple<string, IObjectWithChangeTracker>>();
                _entitiesDuringAdd = new Queue<Tuple<ObjectStateEntry, string, IEnumerable<object>>>();
            }

            private void Detach()
            {
                _context.ObjectStateManager.ObjectStateManagerChanged -= this.HandleStateManagerChange;
            }

            private void HandleStateManagerChange(object sender, CollectionChangeEventArgs args)
            {
                if (args.Action == CollectionChangeAction.Add)
                {
                    IObjectWithChangeTracker entity = args.Element as IObjectWithChangeTracker;
                    ObjectStateEntry entry = _context.ObjectStateManager.GetObjectStateEntry(entity);
                    ObjectChangeTracker changeTracker = entity.ChangeTracker;

                    changeTracker.IsChangeTrackingEnabled = false;

                    _entityIndex.Add(entry, changeTracker);

                    // Queue removed reference values
                    var navPropNames = _context.MetadataWorkspace.GetCSpaceEntityType(entity.GetType()).NavigationProperties.Select(n => n.Name);
                    var entityRefOriginalValues = changeTracker.OriginalValues.Where(kvp => navPropNames.Contains(kvp.Key));
                    foreach (KeyValuePair<string, object> originalValueWithName in entityRefOriginalValues)
                    {
                        if (originalValueWithName.Value != null)
                        {
                            _entitiesDuringAdd.Enqueue(new Tuple<ObjectStateEntry, string, IEnumerable<object>>(
                                entry,
                                originalValueWithName.Key,
                                new object[] { originalValueWithName.Value }));
                        }
                    }

                    // Queue removed collection values
                    foreach (KeyValuePair<string, EntityList> collectionPropertyChangesWithName in changeTracker.ObjectsRemovedFromCollectionProperties)
                    {
                        _entitiesDuringAdd.Enqueue(new Tuple<ObjectStateEntry, string, IEnumerable<object>>(
                            entry,
                            collectionPropertyChangesWithName.Key,
                            collectionPropertyChangesWithName.Value));
                    }
                }
            }

            private EntityIndex EntityIndex
            {
                get { return _entityIndex; }
            }

            private bool HasMore
            {
                get { ProcessNewAdds(); return _entitiesToAdd.Count > 0; }
            }

            private void QueueAdd(string entitySetName, IObjectWithChangeTracker entity)
            {
                if (!_entityIndex.Contains(entity))
                {
                    // Queue the entity so that we can add the 'removed collection' items
                    _entitiesToAdd.Enqueue(new Tuple<string, IObjectWithChangeTracker>(entitySetName, entity));
                }
            }

            private Tuple<string, IObjectWithChangeTracker> NextAdd()
            {
                ProcessNewAdds();
                return _entitiesToAdd.Dequeue();
            }

            private void ProcessNewAdds()
            {
                while (_entitiesDuringAdd.Count > 0)
                {
                    Tuple<ObjectStateEntry, string, IEnumerable<object>> relatedEntities = _entitiesDuringAdd.Dequeue();
                    RelatedEnd relatedEnd = relatedEntities.Item1.GetRelatedEnd(relatedEntities.Item2);
                    string entitySetName = relatedEnd.GetEntitySetName();

                    foreach (var targetEntity in relatedEntities.Item3)
                    {
                        QueueAdd(entitySetName, targetEntity as IObjectWithChangeTracker);
                    }
                }
            }
        }

        private sealed class EntityIndex
        {
            private readonly ObjectContext _context;

            // Set of all entities
            private readonly HashSet<IObjectWithChangeTracker> _allEntities;

            // Index of the final key that will be used in the context (could be real for non-added, could be temporary for added)
            // to the initial temporary key
            private readonly Dictionary<EntityKey, EntityKey> _temporaryKeyMap;

            public EntityIndex(ObjectContext context)
            {
                _context = context;

                _allEntities = new HashSet<IObjectWithChangeTracker>();
                _temporaryKeyMap = new Dictionary<EntityKey, EntityKey>();
            }

            public void Add(ObjectStateEntry entry, ObjectChangeTracker changeTracker)
            {
                EntityKey temporaryKey = entry.EntityKey;
                EntityKey finalKey;

                if (!_allEntities.Contains(entry.Entity))
                {
                    // Track that this Apply will be handling this entity
                    _allEntities.Add(entry.Entity as IObjectWithChangeTracker);
                }

                if (changeTracker.State == ObjectState.Added)
                {
                    finalKey = temporaryKey;
                }
                else
                {
                    finalKey = _context.CreateEntityKey(temporaryKey.EntityContainerName + "." + temporaryKey.EntitySetName, entry.Entity);
                }
                if (!_temporaryKeyMap.ContainsKey(finalKey))
                {
                    _temporaryKeyMap.Add(finalKey, temporaryKey);
                }
            }

            public bool Contains(object entity)
            {
                return _allEntities.Contains(entity);
            }

            public IEnumerable<IObjectWithChangeTracker> AllEntities
            {
                get { return _allEntities; }
            }

            // Converts the passed in EntityKey to the EntityKey that is usable by the current state of ApplyChanges
            public EntityKey ConvertEntityKey(EntityKey targetKey)
            {
                ObjectStateEntry targetEntry;
                if (!_context.ObjectStateManager.TryGetObjectStateEntry(targetKey, out targetEntry))
                {
                    // If no entry exists, then either:
                    // 1. This is an EntityKey that is not represented in the set of entities being dealt with during the Apply
                    // 2. This is an EntityKey that will represent one of the yet-to-be-processed Added entries, so look it up
                    EntityKey temporaryKey;
                    if (_temporaryKeyMap.TryGetValue(targetKey, out temporaryKey))
                    {
                        targetKey = temporaryKey;
                    }
                }
                return targetKey;
            }
        }

        // The RelationshipSet builds a list of all relationships from an
        // initial set of entities
        private sealed class RelationshipSet : IEnumerable<RelationshipWrapper>
        {
            private readonly HashSet<RelationshipWrapper> _relationships;
            private readonly ObjectContext _context;

            public RelationshipSet(ObjectContext context, IEnumerable<object> allEntities)
            {
                _context = context;
                _relationships = new HashSet<RelationshipWrapper>();
                foreach (object entity in allEntities)
                {
                    ObjectStateEntry entry = context.ObjectStateManager.GetObjectStateEntry(entity);
                    foreach (IRelatedEnd relatedEnd in entry.RelationshipManager.GetAllRelatedEnds())
                    {
                        if (!((AssociationType)relatedEnd.RelationshipSet.ElementType).IsForeignKey)
                        {
                            foreach (object targetEntity in relatedEnd)
                            {
                                Add(relatedEnd, entity, targetEntity, EntityState.Unchanged);
                            }
                        }
                    }
                }
            }

            // Adds an entry to the index based on a IRelatedEnd
            public void Add(IRelatedEnd relatedEnd, object sourceEntity, object targetEntity, EntityState state)
            {
                RelationshipWrapper wrapper = new RelationshipWrapper(
                                    (AssociationSet)relatedEnd.RelationshipSet,
                                    relatedEnd.SourceRoleName,
                                    sourceEntity,
                                    relatedEnd.TargetRoleName,
                                    targetEntity,
                                    state);
                if (!_relationships.Contains(wrapper))
                {
                    _relationships.Add(wrapper);
                }
            }

            // Removes an entry from the index based on a relationship ObjectStateEntry
            public void Remove(ObjectStateEntry relationshipEntry)
            {
                Debug.Assert(relationshipEntry.IsRelationship);
                AssociationSet associationSet = (AssociationSet)relationshipEntry.EntitySet;
                DbDataRecord values = relationshipEntry.State == EntityState.Deleted ? relationshipEntry.OriginalValues : relationshipEntry.CurrentValues;
                int fromOridinal = values.GetOrdinal(associationSet.ElementType.AssociationEndMembers[0].Name);
                object fromEntity = _context.ObjectStateManager.GetObjectStateEntry((EntityKey)values.GetValue(fromOridinal)).Entity;
                int toOridinal = values.GetOrdinal(associationSet.ElementType.AssociationEndMembers[1].Name);
                object toEntity = _context.ObjectStateManager.GetObjectStateEntry((EntityKey)values.GetValue(toOridinal)).Entity;

                if (fromEntity != null && toEntity != null)
                {
                    RelationshipWrapper wrapper = new RelationshipWrapper(
                        associationSet,
                        associationSet.ElementType.AssociationEndMembers[0].Name,
                        fromEntity,
                        associationSet.ElementType.AssociationEndMembers[1].Name,
                        toEntity,
                        EntityState.Unchanged);

                    _relationships.Remove(wrapper);
                }
            }

            #region IEnumerable<RelationshipWrapper>

            public IEnumerator<RelationshipWrapper> GetEnumerator()
            {
                return _relationships.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _relationships.GetEnumerator();
            }

            #endregion
        }

        // A RelationshipWrapper is used to identify a relationship between two entities
        // The relationship is identified by the AssociationSet, and the order of the entities based
        // on the roles they play (via AssociationEndMember)
        private sealed class RelationshipWrapper : IEquatable<RelationshipWrapper>
        {
            internal readonly AssociationSet AssociationSet;
            internal readonly object End0;
            internal readonly object End1;
            internal readonly EntityState State;

            internal RelationshipWrapper(AssociationSet extent,
                                         string role0, object end0,
                                         string role1, object end1,
                                         EntityState state)
            {
                Debug.Assert(null != extent, "null AssociationSet");
                Debug.Assert(null != (object)end0, "null end0");
                Debug.Assert(null != (object)end1, "null end1");

                AssociationSet = extent;
                Debug.Assert(extent.ElementType.AssociationEndMembers.Count == 2, "only 2 ends are supported");

                State = state;

                if (extent.ElementType.AssociationEndMembers[0].Name == role0)
                {
                    Debug.Assert(extent.ElementType.AssociationEndMembers[1].Name == role1, "a)roleAndKey1 Name differs");
                    End0 = end0;
                    End1 = end1;
                }
                else
                {
                    Debug.Assert(extent.ElementType.AssociationEndMembers[0].Name == role1, "b)roleAndKey1 Name differs");
                    Debug.Assert(extent.ElementType.AssociationEndMembers[1].Name == role0, "b)roleAndKey0 Name differs");
                    End0 = end1;
                    End1 = end0;
                }
            }

            internal ReadOnlyMetadataCollection<AssociationEndMember> AssociationEndMembers
            {
                get { return this.AssociationSet.ElementType.AssociationEndMembers; }
            }

            public override int GetHashCode()
            {
                return this.AssociationSet.Name.GetHashCode() ^ (this.End0.GetHashCode() + this.End1.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as RelationshipWrapper);
            }

            public bool Equals(RelationshipWrapper wrapper)
            {
                return (Object.ReferenceEquals(this, wrapper) ||
                        ((null != wrapper) &&
                         Object.ReferenceEquals(this.AssociationSet, wrapper.AssociationSet) &&
                         Object.ReferenceEquals(this.End0, wrapper.End0) &&
                         Object.ReferenceEquals(this.End1, wrapper.End1)));
            }
        }
    }
}