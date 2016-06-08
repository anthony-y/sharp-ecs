using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpECS
{
    public abstract class EntitySystem
    {
        public EntityPool Pool { get; set; }

        public IEnumerable<Entity> Compatible { get; set; }

        private List<IComponent> _compatibleComponents;

        //public EntitySystem(EntityPool pool)
        //{
        //    Pool = pool;

        //    Compatible = GetCompatibleInPool();

        //    Pool.EntityAdded += OnPoolEntityChanged;
        //    Pool.EntityRemoved += OnPoolEntityChanged;

        //    Pool.EntityComponentAdded += OnPoolEntityChanged;
        //    Pool.EntityComponentRemoved += OnPoolEntityChanged;
        //}

        public EntitySystem(EntityPool pool, params Type[] compatible)
        {
            _compatibleComponents = new List<IComponent>();

            Pool = pool;
            
            foreach (var i in compatible)
            {
                if (IsImplementedFromComponent(i))
                {
                    _compatibleComponents.Add(i as IComponent);
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

        private bool IsImplementedFromComponent(Type objectType)
        {
            foreach (var interfac in objectType.GetInterfaces())
            {
                if (objectType.IsSubclassOf(typeof(IComponent)))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<Entity> GetCompatibleInPool()
        {
            var list = new List<Entity>();

            foreach (var i in Pool.Entities)
            {
                foreach (var j in _compatibleComponents)
                {
                    if (i.Components.FirstOrDefault(com => com.GetType() == j.GetType()) != null)
                    {
                        list.Add(i);
                    }
                }
            }

            return list;
        }
    }
}