using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private List<Entity> _cachedEntities;

        public List<Entity> Entities
        {
            get { return _activeEntities; }
            private set { _activeEntities = value; }
        }

        private const int MAX_CACHED_ENTITIES = 10;

        public EntityPool()
        {
            _activeEntities = new List<Entity>();
            _cachedEntities = new List<Entity>();

            EntityAdded = (EntityPool pool, Entity e) =>
            {
                Console.WriteLine("Entity \"" + e.Tag + "\" added to pool.");
            };

            EntityRemoved = (EntityPool pool, Entity e) =>
            {
                Console.WriteLine("Entity \"" + e.Tag + "\" was removed from pool.");
            };
        }

        public Entity CreateEntity(string entityTag)
        {
            Entity newEntity;

            if (_cachedEntities.Count > 0)
            {
                int lastEnt = _cachedEntities.Count - 1;

                newEntity = _cachedEntities[lastEnt];
                _cachedEntities.Remove(newEntity);

                //newEntity.Tag = entityTag;
                newEntity.OwnerPool = this;
            } else
            {
                newEntity = new Entity(entityTag, this);
            }

            _activeEntities.Add(newEntity);
            EntityAdded(this, newEntity);

            return newEntity;
        }

        public void RemoveEntity(Entity entity)
        {
            //entity.Reset();
            entity.RemoveAllComponents();

            if (_cachedEntities.Count < MAX_CACHED_ENTITIES)
            {
                _activeEntities.Remove(entity);
                _cachedEntities.Add(entity);
            } else
            {
                _activeEntities.Remove(entity);
            }

            EntityRemoved(this, entity);

        }
    }
}
