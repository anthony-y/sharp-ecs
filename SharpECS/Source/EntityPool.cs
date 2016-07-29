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
        private readonly int MAX_CACHED_ENTITIES = 25;

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
        /// Creates a new Entity with "entityId", adds it to active Entities and returns it.
        /// </summary>
        /// <param Id="entityId"></param>
        /// <returns>Final Entity</returns>
        public Entity CreateEntity(string entityId)
        {
            if (Entities.Any(ent => ent.Id == entityId))
                throw new DuplicateEntityException(this);

            if (string.IsNullOrEmpty(entityId))
                throw new Exception("The string you entered was blank or null.");
                
            Entity newEntity;

            if (_cachedEntities.Any())
            {
                newEntity = _cachedEntities.Pop();

                if (newEntity == null)
                    throw new EntityNotFoundException(this);

                newEntity.Id = entityId;
                newEntity.OwnerPool = this;
                newEntity.State = EntityState.Active;

                _activeEntities.Add(newEntity);

                #if DEBUG
                    Console.WriteLine($"Retrieved {newEntity.Id} from cache.");
                #endif
            } else
            {
                newEntity = new Entity(entityId, this);
                _activeEntities.Add(newEntity);

                #if DEBUG
                    Console.WriteLine($"Created new instance for {newEntity.Id} because the cache was empty.");
                #endif
            }

            EntityAdded?.Invoke(this, newEntity);

            return newEntity;
        }

        internal void AddEntity(Entity entity)
        {
            if (entity.IsAvailable())
                _activeEntities.Add(entity);

            else if (!entity.IsAvailable())
                _cachedEntities.Push(entity);

            EntityAdded?.Invoke(this, entity);
        }

        public bool DoesEntityExist(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return false;

            return Entities.Any(ent => ent.Id == Id && GetEntity(Id).IsAvailable());
        }

        public bool DoesEntityExist(Entity entity)
        {
            if (entity == null || entity == default(Entity))
                return false;

            if (string.IsNullOrEmpty(entity.Id))
                return false;

            return Entities.Any(ent => ent == entity && entity.IsAvailable());
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
        public void DestroyEntity(ref Entity entity)
        {
            if (!entity.IsAvailable())
                return;

            if (!_activeEntities.Contains(entity))
                throw new EntityNotFoundException(this);

            if (_cachedEntities.Count < MAX_CACHED_ENTITIES)
            {
                // Reset the Entity.
                entity.Reset();
                _cachedEntities.Push(entity);
            }

            _activeEntities.Remove(entity);
            EntityRemoved?.Invoke(this, entity);

            // Set the entity that was passed in to null
            entity = null;
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
        public static EntityPool operator + (EntityPool pool, string id)
        {
            pool.CreateEntity(id);

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
            if (entity == null || !pool.DoesEntityExist(entity.Id))
            {
                throw new EntityNotFoundException(pool);
            }

            pool.DestroyEntity(ref entity);
            return pool;
        }

        public static EntityPool operator - (EntityPool pool, string id)
        {
            if (string.IsNullOrEmpty(id) || !pool.DoesEntityExist(id))
            {
                throw new EntityNotFoundException(pool);
            }

            var toDestroy = pool.GetEntity(id);
            pool.DestroyEntity(ref toDestroy);
            return pool;
        }
    }
}
