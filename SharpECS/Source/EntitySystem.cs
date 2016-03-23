using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS
{
    public abstract class EntitySystem<TComponent> where TComponent : IComponent
    {
        public EntityPool Pool { get; set; }

        public List<Entity> Compatible { get; set; }

        public EntitySystem(EntityPool pool)
        {
            Pool = pool;

            Compatible = GetCompatibleInPool();

            Pool.EntityAdded += OnPoolEntityChanged;
            Pool.EntityRemoved += OnPoolEntityChanged;

            Pool.EntityComponentAdded += OnPoolEntityChanged;
            Pool.EntityComponentRemoved += OnPoolEntityChanged;
        }

        private void OnPoolEntityChanged(EntityPool pool, Entity entity)
        {
            Pool = pool;
            Compatible = GetCompatibleInPool();

#if DEBUG
            Console.WriteLine($"{entity.Tag} had a component changed so {pool.Name} refreshed it's Compatible Entity list.");
#endif
        }

        private List<Entity> GetCompatibleInPool()
        {
            return Pool.Entities.Where(ent => ent.HasComponentOfType<TComponent>()).ToList();
        }
    }
}
