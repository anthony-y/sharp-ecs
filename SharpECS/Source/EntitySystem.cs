using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS
{
    public abstract class EntitySystem
    {
        public EntityPool Pool { get; set; }
        public List<Entity> Compatible { get; set; }

        private List<Type> _compatibleTypes;

        public EntitySystem(EntityPool pool, params Type[] compatibleTypes)
        {
            if (compatibleTypes.Any(t => !typeof(IComponent).IsAssignableFrom(t)))
                throw new Exception("Type passed into EntitySystem is not an IComponent!");

            _compatibleTypes = new List<Type>();
            _compatibleTypes.AddRange(compatibleTypes);

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

        protected virtual List<Entity> GetCompatibleInPool()
        {
            return Pool.Entities.Where(ent => ent.HasComponents(_compatibleTypes)).ToList();
        }
    }
}
