using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECSIsBetter.Exceptions;

namespace ECSIsBetter
{
    public class Entity
    {
        public delegate void EntityComponentChanged(Entity entity, IComponent component);

        public event EntityComponentChanged ComponentAdded;
        public event EntityComponentChanged ComponentRemoved;

        public string Tag { get; set; }
        public EntityPool OwnerPool { get; set; }

        internal List<IComponent> Components { get; set; }

        public Entity(string tag)
        {
            Tag = tag;

            Components = new List<IComponent>();
        }

        public Entity(string tag, EntityPool pool)
        {
            Tag = tag;
            OwnerPool = pool;

            Components = new List<IComponent>();
        }

        public IComponent AddComponent(IComponent component)
        {
            Components.Add(component);
            if (ComponentAdded != null) ComponentAdded(this, component);

            return component;
        }

        public void RemoveComponent(IComponent component)
        {
            if (!Components.Contains(component)) return;

            Components.Remove(component);

            if (ComponentRemoved != null) ComponentRemoved(this, component);
        }

        public T GetComponent<T>() where T : IComponent
        {
            return (T)Components.FirstOrDefault(ent => ent.GetType() == typeof(T));
        }

        public IComponent LastComponent()
        {
            return this.Components.Last();
        }

        public void RemoveAllComponents()
        {
            Components.Clear();
        }

        public void Reset()
        {
            RemoveAllComponents();

            this.Tag = string.Empty;
            this.OwnerPool = null;
        }

        public List<IComponent> GetAllComponents()
        {
            return this.Components;
        }

        public void MoveTo(EntityPool pool)
        {
            if (pool != null)
            {
                pool.AddEntity(this);
                this.OwnerPool.DestroyEntity(this);
                this.OwnerPool = pool;
            } else
            {
                throw new NullEntityPoolException(pool);
            }
        }

    }
}