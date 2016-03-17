using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECSIsBetter.Exceptions;

namespace ECSIsBetter
{
    /// <summary>
    /// This is now actually an object pool, not just a collection of instances.
    /// </summary>
    public class EntityPool
    {
        public delegate void EntityChanged(EntityPool pool, Entity entity);

        public event EntityChanged EntityAdded;
        public event EntityChanged EntityRemoved;

        private List<Entity> _activeEntities;
        private Stack<Entity> _cachedEntities;

        public List<Entity> Entities
        {
            get { return _activeEntities; }
            private set { _activeEntities = value; }
        }

        public string Name { get; set; }

        private readonly int MAX_CACHED_ENTITIES = 5;

        public EntityPool()
        {
            _activeEntities = new List<Entity>();
            _cachedEntities = new Stack<Entity>();
        }

        public EntityPool(string name)
        {
            _activeEntities = new List<Entity>();
            _cachedEntities = new Stack<Entity>();

            Name = name;
        }

        public Entity AddEntity(Entity entity)
        {
            _activeEntities.Add(entity);

            if (EntityAdded != null) EntityAdded(this, entity);

            return entity;
        }

        public Entity CreateEntity(string entityTag)
        {
            Entity newEntity = null;

            var tagMatch = this.Entities.FirstOrDefault(ent => ent.Tag == entityTag);

            if (tagMatch != null)
            {
                throw new DuplicateEntityException(this);
            }

            if (_cachedEntities.Count > 0)
            { 
                int lastEnt = _cachedEntities.Count - 1;

                newEntity = _cachedEntities.Pop();
                _activeEntities.Add(newEntity);

                if (newEntity != null && _activeEntities.Contains(newEntity))
                {
                    newEntity.Tag = entityTag;
                    newEntity.OwnerPool = this;
                    Console.WriteLine("Retrieved " + newEntity.Tag + " from cache.");
                } else
                {
                    throw new EntityNotFoundException(this);
                }
            } else
            {
                newEntity = new Entity(entityTag, this);
            }

            _activeEntities.Add(newEntity);

            if (EntityAdded != null) EntityAdded(this, newEntity);

            return newEntity;
        }

        public void DestroyEntity(Entity entity)
        {
            var held = entity;

            entity.Reset();

            if (_activeEntities.Contains(entity))
            {
                if (_cachedEntities.Count < MAX_CACHED_ENTITIES)
                {
                    _cachedEntities.Push(entity);
                    _activeEntities.Remove(entity);
                }
                else
                {
                    _activeEntities.Remove(entity);
                }
            } else
            {
                throw new EntityNotFoundException(this);
            }

            if (EntityRemoved != null) EntityRemoved(this, held);
        }

        public void WipeCache()
        {
            _cachedEntities.Clear();
        }

        public void WipeEntities()
        {
            _activeEntities.Clear();
        }

    }
}
