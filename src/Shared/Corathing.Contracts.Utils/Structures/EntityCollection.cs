using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corathing.Contracts.Bases.Interfaces;
using Corathing.Contracts.Utils.Messages;

namespace Corathing.Contracts.Utils.Structures;

public class EntityCollection<T> : Dictionary<Guid, T> where T : IEntity
{
    public EntityCollection()
    {
    }

    public EntityCollection(IEnumerable<KeyValuePair<Guid, T>> entities)
    {
        foreach (var entity in entities)
        {
            this.Add(entity.Key, entity.Value);
        }
    }
}

public static class EntityCollectionExtensions
{
    public static EntityCollection<T> AddMany<T>(this EntityCollection<T> collection, IEnumerable<T> entities) where T : IEntity
    {
        return EntityCollectionUtils.AddManyToEntityCollection(collection, entities);
    }
}


public static class EntityCollectionUtils
{
    public static IReadOnlyList<T> ToEntitiesArray<T>(IEnumerable<T> entities) where T : IEntity
    {
        return entities.ToList();
    }

    public static EntityCollection<T> Merge<T>(EntityCollection<T> collection, IEnumerable<T> entities) where T : IEntity
    {
        var newCollection = new EntityCollection<T>(collection);
        foreach (var entity in entities)
        {
            newCollection[entity.Id] = entity;
        }
        return newCollection;
    }

    public static EntityCollection<T> AddManyToEntityCollection<T>(EntityCollection<T> collection, IEnumerable<T> entities) where T : IEntity
    {
        var newEntities = ToEntitiesArray(entities).Where(entity => !collection.ContainsKey(entity.Id)).ToList();

        if (newEntities.Count != 0)
        {
            return Merge(collection, newEntities);
        }
        else
        {
            return collection;
        }
    }

    public static EntityCollection<T> AddOneToEntityCollection<T>(EntityCollection<T> collection, T entity) where T : IEntity
    {
        return AddManyToEntityCollection(collection, new List<T> { entity });
    }

    public static EntityCollection<T> SetManyInEntityCollection<T>(EntityCollection<T> collection, IEnumerable<T> entities) where T : IEntity
    {
        var updEntities = ToEntitiesArray(entities);

        if (updEntities.Count != 0)
        {
            return Merge(collection, updEntities);
        }
        else
        {
            return collection;
        }
    }

    public static EntityCollection<T> SetOneInEntityCollection<T>(EntityCollection<T> collection, T entity) where T : IEntity
    {
        return SetManyInEntityCollection(collection, new List<T> { entity });
    }

    public static EntityCollection<T> CreateEntityCollection<T>(IEnumerable<T> entities = null) where T : IEntity
    {
        if (entities != null)
        {
            return Merge(new EntityCollection<T>(), entities);
        }
        else
        {
            return new EntityCollection<T>();
        }
    }

    public static EntityCollection<T> RemoveManyFromEntityCollection<T>(EntityCollection<T> collection, IEnumerable<Guid> ids) where T : IEntity
    {
        var newCollection = new EntityCollection<T>(collection);
        var changed = false;
        foreach (var id in ids)
        {
            if (newCollection.ContainsKey(id))
            {
                changed = true;
                newCollection.Remove(id);
            }
        }

        if (changed)
        {
            return newCollection;
        }
        else
        {
            return collection;
        }
    }

    public static EntityCollection<T> RemoveOneFromEntityCollection<T>(EntityCollection<T> collection, Guid id) where T : IEntity
    {
        return RemoveManyFromEntityCollection(collection, new List<Guid> { id });
    }

    public static EntityCollection<T> UpdateManyInEntityCollection<T>(EntityCollection<T> collection, IEnumerable<EntityUpdateMessage<T>> updates) where T : IEntity
    {
        var changed = false;
        var newCollection = new EntityCollection<T>(collection);

        foreach (var update in updates)
        {
            if (!collection.TryGetValue(update.Id, out var entity))
            {
                continue;
            }

            changed = true;
            var newEntity = (T)entity.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(entity, null);
            foreach (var change in update.Changes)
            {
                var property = newEntity.GetType().GetProperty(change.Key);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(newEntity, change.Value);
                }
            }

            if (update.Id != newEntity.Id)
            {
                newCollection.Remove(update.Id);
            }
            newCollection[newEntity.Id] = newEntity;
        }

        if (changed)
        {
            return newCollection;
        }
        else
        {
            return collection;
        }
    }

    public static EntityCollection<T> UpdateOneInEntityCollection<T>(EntityCollection<T> collection, EntityUpdateMessage<T> update) where T : IEntity
    {
        return UpdateManyInEntityCollection(collection, new List<EntityUpdateMessage<T>> { update });
    }

    public static IReadOnlyList<T> GetManyFromEntityCollection<T>(EntityCollection<T> collection, IEnumerable<Guid> ids) where T : IEntity
    {
        return ids.Select(id => collection.ContainsKey(id) ? collection[id] : default).ToList();
    }

    public static T GetOneFromEntityCollection<T>(EntityCollection<T> collection, Guid id) where T : IEntity
    {
        collection.TryGetValue(id, out var entity);
        return entity;
    }

    public static IReadOnlyList<T> GetEntitiesArrayFromEntityCollection<T>(EntityCollection<T> collection) where T : IEntity
    {
        return collection.Values.ToList();
    }

    public static Dictionary<Guid, Y> MapEntityCollection<T, Y>(EntityCollection<T> collection, Func<T, Y> callbackFn) where T : IEntity where Y : IEntity
    {
        return collection.Values.Select(entity => callbackFn(entity)).ToDictionary(entity => entity.Id);
    }
}
