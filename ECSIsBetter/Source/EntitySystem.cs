using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSIsBetter
{
    public abstract class EntitySystem
    {
        public EntityGroup Group { get; set; }

        public EntitySystem(EntityGroup group)
        {
            Group = group;

            Group.EntityAdded += OnGroupEntityAdded;
            Group.EntityRemoved += OnGroupEntityRemoved;
        }

        public virtual void OnGroupEntityAdded(Entity entity, EntityGroup group, List<Entity> newCollection)
        {
            Group = group;
            Group.Collection = newCollection;
        }

        public virtual void OnGroupEntityRemoved(Entity entity, EntityGroup group, List<Entity> newCollection)
        {
            Group = group;
            Group.Collection = newCollection;
            if (Group.Collection.Contains(entity)) group.RemoveEntity(entity);
        }
    }

    public abstract class GenericSystem<TComponent> where TComponent : IComponent
    {
        public EntityPool Pool { get; set; }

        public List<Entity> Compatible { get; set; }

        public GenericSystem(EntityPool pool)
        {
            Pool = pool;

            Compatible = GetCompatibleInPool();

            Pool.EntityAdded += OnPoolEntityChanged;
            Pool.EntityRemoved += OnPoolEntityChanged;
        }

        private void OnPoolEntityChanged(EntityPool pool, Entity entity)
        {
            Pool = pool;
        }

        private List<Entity> GetCompatibleInPool()
        {
            var list = new List<Entity>();

            foreach (var ent in Pool.Entities)
            {
                if (ent.HasComponentOfType<TComponent>())
                {
                    list.Add(ent);
                }
            }

            return list;
        }
    }
}
