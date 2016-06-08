using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpECS
{
    public abstract class EntitySystem
    {
        public EntityPool Pool { get; set; }

        public IEnumerable<Entity> Compatible { get; set; }

        private List<Type> _compatibleTypes;

        //public EntitySystem(EntityPool pool)
        //{
        //    Pool = pool;

        //    Compatible = GetCompatibleInPool();

        //    Pool.EntityAdded += OnPoolEntityChanged;
        //    Pool.EntityRemoved += OnPoolEntityChanged;

        //    Pool.EntityComponentAdded += OnPoolEntityChanged;
        //    Pool.EntityComponentRemoved += OnPoolEntityChanged;
        //}

        public EntitySystem(EntityPool pool, params Type[] compatibleTypes)
        {
            _compatibleTypes = new List<Type>();

            Pool = pool;
            
            foreach (var compatibleType in compatibleTypes)
            {
                if (typeof(IComponent).IsAssignableFrom(compatibleType))
                {
                    _compatibleTypes.Add(compatibleType);
                }
            }

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
            Console.WriteLine($"System refreshed it's Compatible Entities because Entity \"{entity.Id} was changed or added.");
#endif
        }

        private IEnumerable<Entity> GetCompatibleInPool()
        {
            var list = new List<Entity>();

            foreach (var entity in Pool.Entities)
            {
                foreach (var compatibleType in _compatibleTypes)
                {
                    if (entity.Components.FirstOrDefault(com => com.GetType() == compatibleType) != null)
                    {
                        list.Add(entity);
                    }
                }
            }

            return list;
        }
    }
}