using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpECS.Exceptions;

namespace SharpECS
{
    /// <summary>
    /// The object that managed all your game Entities.
    /// </summary>
    public class EntityPool
    {
        public delegate void EntityChanged(EntityPool pool, Entity entity);

        public event EntityChanged EntityAdded;
        public event EntityChanged EntityRemoved;

        public event EntityChanged EntityComponentAdded;
        public event EntityChanged EntityComponentRemoved;

        private List<Entity> _activeEntities;
        private Stack<Entity> _cachedEntities;

        public List<Entity> Entities
        {
            get { return _activeEntities; }
            private set { _activeEntities = value; }
        }

        public Stack<Entity> CachedEntities
        {
            get { return _cachedEntities; }
            private set { _cachedEntities = value; }
        }

        public string Id { get; set; }

        // How many Entites the cache can store at a time.
        private readonly int MAX_CACHED_ENTITIES = 5;

        /// <summary>
        /// Creates and returns a new instance of EntityPool
        /// (it looks prettier than "var pool = new EntityPool("Id");"
        /// also less code) :3
        /// </summary>
        /// <param Id="Id"></param>
        /// <returns></returns>
        public static EntityPool New(string Id)
        {
            return new EntityPool(Id);
        }

        private EntityPool(string Id)
        {
            _activeEntities = new List<Entity>();
            _cachedEntities = new Stack<Entity>();

            if (Id != null) this.Id = Id;
        }

        /// <summary>
        /// Adds an already existing instance of Entity to the pool.
        /// </summary>
        /// <param Id="entity"></param>
        /// <returns></returns>
        public Entity AddEntity(Entity entity)
        {
            _activeEntities.Add(entity);

            EntityAdded?.Invoke(this, entity);

            return entity;
        }

        /// <summary>
        /// Creates a new Entity with "entityId", adds it to active Entities and returns it.
        /// </summary>
        /// <param Id="entityId"></param>
        /// <returns>Final Entity</returns>
        public Entity CreateEntity(string entityId)
        {
            Entity newEntity = null;

            var IdMatch = this.Entities.FirstOrDefault(ent => ent.Id == entityId);

            if (IdMatch != null)
            {
                throw new DuplicateEntityException(this);
            }

            if (entityId == string.Empty || entityId == null || entityId.Trim() == string.Empty || entityId.Trim() == null)
            {
                throw new Exception("The string you entered was blank or null.");
            }
                
            if (_cachedEntities.Count > 0)
            {
                newEntity = _cachedEntities.Pop();
                _activeEntities.Add(newEntity);

                if (newEntity != null && _activeEntities.Contains(newEntity))
                {
                    newEntity.Id = entityId;
                    newEntity.OwnerPool = this;
#if DEBUG
                    Console.WriteLine($"Retrieved {newEntity.Id} from cache.");
#endif
                } else
                {
                    throw new EntityNotFoundException(this);
                }
            } else
            {
                newEntity = new Entity(entityId, this);
#if DEBUG
                Console.WriteLine($"Created new instance for {newEntity.Id} because the cache was empty.");
#endif
            }

            _activeEntities.Add(newEntity);

            EntityAdded?.Invoke(this, newEntity);

            return newEntity;
        }

        public bool DoesEntityExist(string Id)
        {
            return Entities.FirstOrDefault(ent => ent.Id == Id) != null;
        }

        public bool DoesEntityExist(Entity entity)
        {
            return Entities.FirstOrDefault(ent => ent.Id == entity.Id) != null;
        }

        public Entity GetEntity(string entityId)
        {
            var match = Entities.FirstOrDefault(ent => ent.Id == entityId);

            if (match != null) return match;

            throw new EntityNotFoundException(this);
        }

        /// <summary>
        /// Adds an Entity to the cache to be re-used if cachedEntities isn't full.
        /// If the cache is full, just remove completely.
        /// </summary>
        /// <param Id="entity"></param>
        public void DestroyEntity(Entity entity)
        {
            // Keep a copy of the entity so that when EntityRemoved is called,
            // it still has the Id and stuff.
            var held = entity;

            // Reset the Entity.
            // See Entity.cs
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

            EntityRemoved?.Invoke(this, held);
        }

        public void UnsafeDestroyEntity(Entity entity)
        {
            if (entity != null && _activeEntities.Contains(entity))
            {
                _activeEntities.Remove(entity);
                EntityRemoved?.Invoke(this, entity);
            }
            else
            {
                throw new EntityNotFoundException(this);
            }
        }

        /// <summary>
        /// Clears the cached Entities stack.
        /// </summary>
        public void WipeCache()
        {
            _cachedEntities.Clear();
        }

        /// <summary>
        /// Clears the active Entities list.
        /// </summary>
        public void WipeEntities()
        {
            _activeEntities.Clear();
        }

        internal void ComponentAdded(Entity entity)
        {
            EntityComponentAdded?.Invoke(this, entity);
        }

        internal void ComponentRemoved(Entity entity)
        {
            EntityComponentRemoved?.Invoke(this, entity);
        }

        /// <summary>
        /// Operator overload to let you do "pool += entity" to add an Entity to the pool.
        /// </summary>
        /// <param Id="pool"></param>
        /// <param Id="entity"></param>
        /// <returns></returns>
        public static EntityPool operator + (EntityPool pool, Entity entity)
        {
            pool.AddEntity(entity);

            return pool;
        }

        /// <summary>
        /// Operator overload to let you do "pool -= entity" to remove an Entity from the pool.
        /// </summary>
        /// <param Id="pool"></param>
        /// <param Id="entity"></param>
        /// <returns></returns>
        public static EntityPool operator - (EntityPool pool, Entity entity)
        {
            if (entity != null)
            {
                pool.DestroyEntity(entity);
                return pool;
            } else
            {
                throw new EntityNotFoundException(pool);
            }
        }
    }
}
