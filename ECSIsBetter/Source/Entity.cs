using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECSIsBetter.Exceptions;

namespace ECSIsBetter
{
    public sealed class Entity
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
            // If it has a component of the same type as "component".
            if (this.Components.FirstOrDefault(com => com.GetType() == component.GetType()) != null)
            {
                throw new ComponentAlreadyExistsException(this);
            }

            component.Owner = this;
            Components.Add(component);
            if (ComponentAdded != null) ComponentAdded(this, component);

            return component;
        }

        public IComponent UnsafeAddComponent(IComponent component)
        {
            component.Owner = this;
            Components.Add(component);
            if (ComponentAdded != null) ComponentAdded(this, component);

            return component;
        }

        public void RemoveComponent(IComponent component)
        {
            if (!Components.Contains(component))
                throw new ComponentNotFoundException(this);

            Components.Remove(component);

            if (ComponentRemoved != null) ComponentRemoved(this, component);
        }

        public T GetComponent<T>() where T : IComponent
        {
            return (T)Components.FirstOrDefault(ent => ent.GetType() == typeof(T));
        }

        public void MoveComponent(IComponent component, Entity destination)
        {
            // If the component itself isn't null and its actually on "this".
            if (component != null && Components.FirstOrDefault(com => com.GetType() == component.GetType()) != null)
            {
                destination.AddComponent(component);
                Components.Remove(component);
            }
        }

        public void UnsafeMoveComponent(IComponent component, Entity destination)
        {
            // If the component itself isn't null and its actually on "this".
            if (component != null && Components.FirstOrDefault(com => com.GetType() == component.GetType()) != null)
            {
                destination.UnsafeAddComponent(component);
                Components.Remove(component);
            }
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

        public static Entity operator + (Entity entity, IComponent component)
        {
            if (entity != null && component != null)
            {
                entity.AddComponent(component);
                return entity;
            } else
            {
                throw new Exception();
            }
        }

    }
}