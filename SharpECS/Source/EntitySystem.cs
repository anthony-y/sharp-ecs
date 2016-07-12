using System;
using System.Collections.Generic;
using System.Linq;

using static SharpECS.Util;

namespace SharpECS
{
    public abstract class EntitySystem
    {
        public EntityPool Pool { get; set; }
        public List<Entity> Compatible { get; set; }

        protected List<Type> CompatibleTypes { get; private set; }

        public EntitySystem(EntityPool pool, params Type[] compatibleTypes)
        {
            if (compatibleTypes.Any(t => !ImplementsInterface(t, typeof(IComponent))))
                throw new Exception("Type passed into EntitySystem is not an IComponent!");

            CompatibleTypes = new List<Type>();
            CompatibleTypes.AddRange(compatibleTypes);

            Pool = pool;

            Compatible = GetCompatibleInPool();

            Pool.EntityComponentAdded += OnPoolEntityChanged;
            Pool.EntityComponentRemoved += OnPoolEntityChanged;

            Pool.EntityAdded += OnPoolEntityChanged;
            Pool.EntityRemoved += OnPoolEntityChanged;
        }

        public void AddCompatibleType(Type type)
        {
            if (ImplementsInterface(type, typeof(IComponent)))
            {
                CompatibleTypes.Add(type);

                Compatible = GetCompatibleInPool();
            } else
            {
                throw new Exception("Type passed into AddCompatibleType is not an IComponent!");
            }
        }

        private void OnPoolEntityChanged(EntityPool pool, Entity entity)
        {
            Pool = pool;
            Compatible = GetCompatibleInPool();
        }

        protected virtual List<Entity> GetCompatibleInPool()
        {
            return Pool.Entities.Where(ent => ent.HasComponents(CompatibleTypes)).ToList();
        }
    }
}
