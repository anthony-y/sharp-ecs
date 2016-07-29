using System;
using System.Collections.Generic;
using System.Linq;

using SharpECS.Exceptions;

namespace SharpECS
{
    public enum EntityState
    {
        Active,
        Inactive,
        Cached,
    }

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

        public string Id { get; set; }

        /// <summary>
        /// The pool which this Entity resides in.
        /// </summary>
        public EntityPool OwnerPool { get; set; }
        
        /// <summary>
        /// A list of this Entity's components.
        /// </summary>
        public List<IComponent> Components { get; set; }

        /// <summary>
        /// A list of this Entitys children Entities.
        /// </summary>
        public List<Entity> Children { get; set; }

        public Entity Parent { get; set; }
        public Entity RootEntity 
        {
            get 
            {
                if (this.Parent == null)
                {
                    return this;
                }

                var parent = Parent;

                while (parent != null) 
                {
                    if (parent.Parent == null)
                    {
                        return parent;
                    } else
                    {
                        parent = parent.Parent;
                    }
                }

                throw new Exception($"Entity \"{Id}\" has no Root!");
            }
        }

        public EntityState State { get; internal set; }
        
        internal Entity(string id, EntityPool pool)
        {   
            Id = id;

            if (pool == null) throw new IndependentEntityException(this);

            OwnerPool = pool;

            Components = new List<IComponent>();
            Children = new List<Entity>();

            State = EntityState.Active;
        }
        
        public void Activate()
        {
            if (!this.IsAvailable())
                return;

            State = EntityState.Active;
        }

        public void Deactivate()
        {
            if (!this.IsAvailable())
                return;

            State = EntityState.Inactive;
        }

        /// <summary>
        /// Switches this Entity on or off
        /// </summary>
        public void Switch()
        {
            if (!this.IsAvailable())
                return;

            State = (State == EntityState.Active ? EntityState.Inactive : EntityState.Active);
        }
        
        /// <summary>
        /// Checks if this already has "component"
        /// If it does, ComponentAlreadyExistsException.
        /// Otherwise, add the component to "Components".
        /// And fire the ComponentAdded event (if it's also not null)
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private IComponent AddComponent(IComponent component)
        {
            if (!this.IsAvailable())
                return null;

            // If it has a component of the same type as "component".
            if (HasComponent(component.GetType()))
                throw new ComponentAlreadyExistsException(this);

            Components.Add(component);
            ComponentAdded?.Invoke(this, component);
            OwnerPool.ComponentAdded(this);

            return component;
        }

        public void RemoveComponent<T>() 
            where T : IComponent
        {
            if (!this.IsAvailable())
                return;

            if (this.HasComponent<T>())
            {
                IComponent componentToRemove = GetComponent<T>();

                Components.Remove(componentToRemove);
                ComponentRemoved?.Invoke(this, componentToRemove);
                OwnerPool.ComponentRemoved(this);
            } else
            {
                throw new ComponentNotFoundException(this);
            }
        }

        public void RemoveComponent(Type componentType)
        {
            if (!this.IsAvailable())
                return;

            if (!Util.ImplementsInterface(componentType, typeof(IComponent)))
                throw new Exception("One or more of the types you passed were not IComponent children.");

            if (!HasComponent(componentType)) throw new ComponentNotFoundException(this);

            IComponent componentToRemove = GetComponent(componentType);

            Components.Remove(componentToRemove);
            ComponentRemoved?.Invoke(this, componentToRemove);
            OwnerPool.ComponentRemoved(this);
        }

        /// <summary>
        /// Checks through Components for a component of type "T"
        /// If it doesn't have one, throw ComponentNotFoundException.
        /// Otherwise returns the component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>()
            where T : IComponent
        {
            if (!this.IsAvailable())
                return default(T);

            var match = Components.OfType<T>().FirstOrDefault();

            if (match == null) throw new ComponentNotFoundException(this);

            return match;
        }

        public IComponent GetComponent(Type componentType)
        {
            if (!this.IsAvailable())
                return null;

            if (!Util.ImplementsInterface(componentType, typeof(IComponent)))
                throw new Exception("One or more of the types you passed were not IComponent children.");

            var match = Components.FirstOrDefault(c => c.GetType() == componentType);
            if (match != null) return match;

            throw new ComponentNotFoundException(this);
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
            if (!this.IsAvailable())
                return;

            // If the component itself isn't null and its actually on "this".
            if (component != null && HasComponent(component.GetType()))
            {
                destination.AddComponent(component);
                Components.Remove(component);
            } else
            {
                throw new ComponentNotFoundException(this);
            }
        }

        /// <summary>
        /// Checks if "this" contains a component of type "TComponent".
        /// If it does, return true. Otherwise, return false.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool HasComponent<TComponent>() 
            where TComponent : IComponent
        {
            if (!this.IsAvailable())
                return false;

            var match = Components.Any(c => c.GetType() == typeof(TComponent));

            if (match) return true;
            else return false;
        }

        public bool HasComponent(Type componentType)
        {
            if (!this.IsAvailable())
                return false;

            if (!Util.ImplementsInterface(componentType, typeof(IComponent)))
                throw new Exception("One or more of the types you passed were not IComponent children.");

            var cMatch = Components.Any(c => c.GetType() == componentType);
            if (cMatch) return true;

            return false;
        }

        public bool HasComponents(IEnumerable<Type> types)
        {
            if (!this.IsAvailable())
                return false;

            foreach (var t in types)
                if (!HasComponent(t)) return false;

            return true;
        }

        /// <summary>
        /// Remove all components.
        /// </summary>
        public void RemoveAllComponents()
        {
            if (!this.IsAvailable())
                return;

            for (int i = Components.Count - 1; i >= 0; i--)
            {
                RemoveComponent(Components[i].GetType());
            }

            Components.Clear();
        }

        /// <summary>
        /// RemoveAllComponents(), reset Id, OwnerPool.
        /// </summary>
        public void Reset()
        {
            if (!this.IsAvailable())
                return;

            RemoveAllComponents();

            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                OwnerPool.DestroyEntity(ref child);
            }
            Children.Clear();

            this.Id = string.Empty;
            this.OwnerPool = null;
            this.State = EntityState.Cached;
        }

        public void AddComponents(IEnumerable<IComponent> components)
        {
            foreach (var c in components) 
            {
                AddComponent(c);
            }
        }

        /// <summary>
        /// Allows an infinite(?) number of components as parameters and adds them all at once to "this".
        /// </summary>
        /// <param name="components"></param>
        public void AddComponents(params IComponent[] components)
        {
            if (!this.IsAvailable())
                return;

            foreach (var c in components) 
            {
                AddComponent(c);
            }
        }

        /// <summary>
        /// Moves this Entity to another EntityPool (if it isn't null)
        /// </summary>
        /// <param name="pool"></param>
        public void MoveTo(EntityPool pool)
        {
            if (this == null)
                return;

            if (pool == null)
                throw new NullEntityPoolException(pool);

            pool.AddEntity(this);
            var me = this;
            OwnerPool.DestroyEntity(ref me);
            OwnerPool = pool;
        }

        public Entity CreateChild(string childId, bool inheritComponents=false)
        {
            if (!this.IsAvailable())
                return null;

            var child = OwnerPool.CreateEntity(childId);

            child.Parent = this;
            if (inheritComponents) { child.AddComponents(Components); }

            Children.Add(child);

            return child;
        }

        public Entity AddChild(Entity entity)
        {
            if (!this.IsAvailable())
                return null;

            entity.Parent = this;
            Children.Add(entity);

            return entity;
        }

        public Entity GetChild(string childId)
        {
            if (!this.IsAvailable())
                return null;

            return Children.FirstOrDefault(c => c.Id == childId);
        }

        public IEnumerable<Entity> FamilyTree()
        {
            var childSelector = new Func<Entity, IEnumerable<Entity>>(ent => ent.Children);

            var stack = new Stack<Entity>(Children);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next))
                    stack.Push(child);
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
            if (!entity.IsAvailable())
                return null;

            if (entity != null && component != null)
            {
                entity.AddComponent(component);
                return entity;
            } else
            {
                throw new NullReferenceException();
            }
        }
    }
}