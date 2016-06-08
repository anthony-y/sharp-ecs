using System;
using System.Collections.Generic;
using System.Linq;

using SharpECS.Exceptions;

namespace SharpECS
{
    /// <summary>
    /// The object that managed all your game Entities.
    /// </summary>
    public class EntityPool
    {
        public event Action<EntityPool, Entity> EntityAdded;
        public event Action<EntityPool, Entity> EntityRemoved;

        public event Action<EntityPool, Entity> EntityComponentAdded;
        public event Action<EntityPool, Entity> EntityComponentRemoved;

        public List<Entity> Entities { get; private set; }
        public Stack<Entity> CachedEntities { get; private set; }

        public string Id { get; set; }

        // How many Entites the cache can store at a time.
        private readonly int MAX_CACHED_ENTITIES = 5;

        /// <summary>
        /// Creates and returns a new instance of EntityPool
        /// (it looks prettier than "var pool = new EntityPool("Name");"
        /// also less code) :3
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EntityPool New(string id)
        {
            return new EntityPool(id);
        }

        private EntityPool(string id)
        {
            Entities = new List<Entity>();
            CachedEntities = new Stack<Entity>();

            if (id != null)
            {
                Id = id;
            }
        }

        /// <summary>
        /// Adds an already existing instance of Entity to the pool.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Entity AddEntity(Entity entity)
        {
            Entities.Add(entity);

            EntityAdded?.Invoke(this, entity);

            return entity;
        }

        /// <summary>
        /// Creates a new Entity with "entityTag", adds it to active Entities and returns it.
        /// </summary>
        /// <param name="entityTag"></param>
        /// <returns>Final Entity</returns>
        public Entity CreateEntity(string entityTag)
        {
            Entity newEntity = null;

            var tagMatch = Entities.FirstOrDefault(entity => entity.Id == entityTag);

            if (tagMatch != null)
            {
                throw new DuplicateEntityException(this);
            }

            if (entityTag == string.Empty || entityTag == null || entityTag.Trim() == string.Empty || entityTag.Trim() == null)
            {
                throw new Exception("The string you entered was blank or null.");
            }
                
            if (CachedEntities.Count > 0)
            {
                newEntity = CachedEntities.Pop();
                Entities.Add(newEntity);

                if (newEntity != null && Entities.Contains(newEntity))
                {
                    newEntity.Id = entityTag;
                    newEntity.OwnerPool = this;
#if DEBUG
                    Console.WriteLine($"Retrieved {newEntity.Id} from cache.");
#endif
                }
                else
                {
                    throw new EntityNotFoundException(this);
                }
            } else
            {
                newEntity = new Entity(entityTag, this);
#if DEBUG
                Console.WriteLine($"Created new instance for {newEntity.Id} because the cache was empty.");
#endif
            }

            Entities.Add(newEntity);

            EntityAdded?.Invoke(this, newEntity);

            return newEntity;
        }

        public bool DoesEntityExist(string tag)
        {
            return Entities.FirstOrDefault(ent => ent.Id == tag) != null;
        }

        public bool DoesEntityExist(Entity entity)
        {
            return Entities.FirstOrDefault(ent => ent.Id == entity.Id) != null;
        }

        public Entity GetEntity(string entityTag)
        {
            var match = Entities.FirstOrDefault(ent => ent.Id == entityTag);

            if (match != null)
            {
                return match;
            }
            throw new EntityNotFoundException(this);
        }

        /// <summary>
        /// Adds an Entity to the cache to be re-used if cachedEntities isn't full.
        /// If the cache is full, just remove completely.
        /// </summary>
        /// <param name="entity"></param>
        public void DestroyEntity(Entity entity)
        {
            // Keep a copy of the entity so that when EntityRemoved is called,
            // it still has the tag and stuff.
            var held = entity;

            // Reset the Entity.
            // See Entity.cs
            entity.Reset();

            if (Entities.Contains(entity))
            {
                if (CachedEntities.Count < MAX_CACHED_ENTITIES)
                {
                    CachedEntities.Push(entity);
                    Entities.Remove(entity);
                }
                else
                {
                    Entities.Remove(entity);
                }
            }
            else
            {
                throw new EntityNotFoundException(this);
            }

            EntityRemoved?.Invoke(this, held);
        }

        public void UnsafeDestroyEntity(Entity entity)
        {
            if (entity != null && Entities.Contains(entity))
            {
                Entities.Remove(entity);
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
            CachedEntities.Clear();
        }

        /// <summary>
        /// Clears the active Entities list.
        /// </summary>
        public void WipeEntities()
        {
            Entities.Clear();
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
        /// <param name="pool"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static EntityPool operator+ (EntityPool pool, Entity entity)
        {
            pool.AddEntity(entity);

            return pool;
        }

        /// <summary>
        /// Operator overload to let you do "pool -= entity" to remove an Entity from the pool.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static EntityPool operator- (EntityPool pool, Entity entity)
        {
            if (entity != null)
            {
                pool.DestroyEntity(entity);
                return pool;
            }
            else
            {
                throw new EntityNotFoundException(pool);
            }
        }
    }
}