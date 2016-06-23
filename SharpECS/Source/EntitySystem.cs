using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS
{
    public abstract class EntitySystem<TComponent> 
        where TComponent 
            : IComponent
    {
        public EntityPool Pool { get; set; }

        public List<Entity> Compatible { get; set; }

        public EntitySystem(EntityPool pool)
        {
            Pool = pool;

            Compatible = GetCompatibleInPool();

            Pool.EntityComponentAdded += OnPoolEntityChanged;
            Pool.EntityComponentRemoved += OnPoolEntityChanged;

            Pool.EntityAdded += OnPoolEntityChanged;
            Pool.EntityRemoved += OnPoolEntityChanged;
        }

        private void OnPoolEntityChanged(EntityPool pool, Entity entity)
        {
            Pool = pool;
            Compatible = GetCompatibleInPool();
        }

        private List<Entity> GetCompatibleInPool()
        {
            return Pool.Entities.Where(ent => ent.HasComponent<TComponent>()).ToList();
        } 

        protected TComponent GetCompatibleOn(Entity entity)
        {
            return entity.GetComponent<TComponent>();
        }
    }
}
