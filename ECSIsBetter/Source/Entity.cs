using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECSIsBetter.Exceptions;

namespace ECSIsBetter
{
    public sealed class Entity
    {
        /// <summary>
        /// Delegate for ComponentAdded and ComponentRemoved.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public delegate void EntityComponentChanged(Entity entity, IComponent component);

        /// <summary>
        /// Events
        /// </summary>
        public event EntityComponentChanged ComponentAdded;
        public event EntityComponentChanged ComponentRemoved;

        public string Tag { get; set; }

        /// <summary>
        /// The pool which this Entity resides in.
        /// </summary>
        public EntityPool OwnerPool { get; set; }
        
        /// <summary>
        /// An internal list of this Entity's components.
        /// </summary>
        internal List<IComponent> Components { get; set; }

        public Entity(string tag, EntityPool pool)
        {   
            Tag = tag;

            if (pool == null) throw new IndependentEntityException(this);

            OwnerPool = pool;

            Components = new List<IComponent>();
        }

        /// <summary>
        /// Checks if this already has "component"
        /// If it does, ComponentAlreadyExistsException.
        /// Otherwise, add the component to "Components".
        /// And fire the ComponentAdded event (if it's also not null)
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
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

        /// <summary>
        /// DOESN'T check if this already has "component"
        /// Adds the component to "Components".
        /// And fire the ComponentAdded event (if it's also not null)
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public IComponent UnsafeAddComponent(IComponent component)
        {
            component.Owner = this;
            Components.Add(component);
            if (ComponentAdded != null) ComponentAdded(this, component);

            return component;
        }

        /// <summary>
        /// Removes "component" if it isn't null and this has it.
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(IComponent component)
        {
            if (!Components.Contains(component))
                throw new ComponentNotFoundException(this);

            Components.Remove(component);

            if (ComponentRemoved != null) ComponentRemoved(this, component);
        }

        /// <summary>
        /// Checks through Components for a component of type "T"
        /// If it doesn't have one, throw ComponentNotFoundException.
        /// Otherwise returns the component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : IComponent
        {
            var match = (T)Components.FirstOrDefault(ent => ent.GetType() == typeof(T));

            if (match == null) throw new ComponentNotFoundException(this);

            return match;
        }

        /// <summary>
        /// Moves a component between this and "destination".
        /// If destination or the component are null, throw a ComponentNotFoundException.
        /// Otherwise add the component to the destination and remove it from this.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="destination"></param>
        public void MoveComponent(IComponent component, Entity destination)
        {
            // If the component itself isn't null and its actually on "this".
            if (component != null && Components.FirstOrDefault(com => com.GetType() == component.GetType()) != null)
            {
                destination.AddComponent(component);
                Components.Remove(component);
            } else
            {
                throw new ComponentNotFoundException(this);
            }
        }

        /// <summary>
        /// MoveComponent except calls "destination.UnsafeAddComponent(component)" instead of
        /// destination.AddComponent(component).
        /// </summary>
        /// <param name="component"></param>
        /// <param name="destination"></param>
        public void UnsafeMoveComponent(IComponent component, Entity destination)
        {
            // If the component itself isn't null and its actually on "this".
            if (component != null && Components.FirstOrDefault(com => com.GetType() == component.GetType()) != null)
            {
                destination.UnsafeAddComponent(component);
                Components.Remove(component);
            }
        }

        /// <summary>
        /// Returns the last component that was added.
        /// </summary>
        /// <returns></returns>
        public IComponent LastComponent()
        {
            return this.Components.Last();
        }

        /// <summary>
        /// Remove all components.
        /// </summary>
        public void RemoveAllComponents()
        {
            Components.Clear();
        }

        /// <summary>
        /// RemoveAllComponents(), reset Tag and OwnerPool.
        /// </summary>
        public void Reset()
        {
            RemoveAllComponents();

            this.Tag = string.Empty;
            this.OwnerPool = null;
        }

        /// <summary>
        /// Returns Components.
        /// </summary>
        /// <returns></returns>
        public List<IComponent> GetAllComponents()
        {
            return this.Components;
        }

        /// <summary>
        /// Moves this Entity to another EntityPool (if it isn't null)
        /// </summary>
        /// <param name="pool"></param>
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

        /// <summary>
        /// Lets you add "component" to "entity" with a +=
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <returns></returns>
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