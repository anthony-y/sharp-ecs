using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECSIsBetter.Exceptions;

namespace ECSIsBetter
{
    public class EntityGroup
    {
        public List<Entity> Collection { get; set; }

        public string Name { get; set; }

        public IComponent Dependency { get; set; }

        public delegate void GroupChanged(Entity entity, EntityGroup group, List<Entity> newCollection);
        public event GroupChanged EntityAdded;
        public event GroupChanged EntityRemoved;

        public static EntityGroup New(string name, params Entity[] entities)
        {
            return new EntityGroup(name, entities);
        }

        public static EntityGroup New(string name, IComponent dependency, params Entity[] entities)
        {
            return new EntityGroup(name, dependency, entities);
        }

        private EntityGroup(string name, params Entity[] entities)
        {
            Collection = new List<Entity>();

            Name = name;

            Add(entities);

            foreach (var entity in Collection)
            {
                entity.OwnerPool.EntityAdded += OnEntityAdded;
                entity.OwnerPool.EntityRemoved += OnEntityRemoved;
            }
        }

        private EntityGroup(string name, IComponent dependency, params Entity[] entities)
        {
            Collection = new List<Entity>();

            Name = name;

            Dependency = dependency;

            Add(entities);

            foreach (var entity in Collection)
            {
                entity.OwnerPool.EntityAdded += OnEntityAdded;
                entity.OwnerPool.EntityRemoved += OnEntityRemoved;
            }
        }

        private void OnEntityRemoved(EntityPool pool, Entity entity)
        {
            RemoveEntity(entity);
        }
        
        private void OnEntityAdded(EntityPool pool, Entity entity)
        {
            Add(entity);
        }

        public void Add(params Entity[] entities)
        {
            foreach (var i in entities)
            {
                if (i != null)
                {
                    if (i.Tag != string.Empty)
                    {
                        Collection.Add(i);
                        if (EntityAdded != null) EntityAdded(i, this, Collection);
                    } else
                    {
                        throw new ECSCacheException();
                    }
                    
                } else
                {
                    throw new EntityNotFoundException(i.OwnerPool);
                }
            }
        }

        public void AddWithDependency(params Entity[] entities)
        {
            foreach (var i in entities)
            {
                if (i != null)
                {
                    i.AddComponent(Dependency);
                    Collection.Add(i);
                    if (EntityAdded != null) EntityAdded(i, this, Collection);
                }
                else
                {
                    throw new EntityNotFoundException(i.OwnerPool);
                }
            }
        }

        public void RemoveEntity(Entity entity)
        {
            if (entity == null || !Collection.Contains(entity))
            {
                throw new EntityNotFoundException(entity.OwnerPool);
            }

            Collection.Remove(entity);

            if (EntityRemoved != null) EntityRemoved(entity, this, Collection);
        }

    }
}
