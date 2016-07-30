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

        /// <summary>
        /// The Entity which this Entity is a child of
        /// Is null if this Entity is root
        /// </summary>
        public Entity Parent { get; set; }

        /// <summary>
        /// Walks up all the parents of this Entity and returns the top one
        /// This Entity is called "root"
        /// </summary>
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

        /// <summary>
        /// Holds the current state of this Entity
        /// </summary>
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
        
        /// <summary>
        /// Set this Entity's state to active, providing it isn't in the cache
        /// </summary>
        public void Activate()
        {
            if (!this.IsAvailable())
                return;

            State = EntityState.Active;
        }

        /// <summary>
        /// Set this Entity's state to inactive, providing it isn't in the cache
        /// </summary>
        public void Deactivate()
        {
            if (!this.IsAvailable())
                return;

            State = EntityState.Inactive;
        }

        /// <summary>
        /// Toggles this Entity on or off
        /// </summary>
        public void Switch()
        {
            if (!this.IsAvailable())
                return;

            State = (State == EntityState.Active ? EntityState.Inactive : EntityState.Active);
        }

        public bool IsActive()
        {
            if (!this.IsAvailable())
                return false;

            return (State == EntityState.Active);
        }

        public bool IsInactive()
        {
            if (!this.IsAvailable())
                return false;

            return (State == EntityState.Inactive);
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

        /// <summary>
        /// Remove a component by generic type parameter and notify the appropriate Entity pool and Systems
        /// </summary>
        /// <typeparam name="T"></typeparam>
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

        /// <summary>
        /// Remove a Component by a Type parameter and notify the appropriate Entity pool and Systems
        /// Uses runtime type checking to make sure the type you passed implements IComponent
        /// </summary>
        /// <param name="componentType"></param>
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

        /// <summary>
        /// Checks through Components for a component of type "componentType"
        /// If it doesn't have one, throw ComponentNotFoundException.
        /// Otherwise return the component.
        /// Uses runtime type checking to make sure "componentType" implements IComponent
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if "this" contains a component of type "TComponent".
        /// If it does, return true. Otherwise, return false.
        /// Uses runtime type checking to make sure "componentType" implements IComponent
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check to see if this Entity has all the components in a collection
        /// If it does, return true, otherwise return false.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Add all the components in a collection to this Entity
        /// </summary>
        /// <param name="components"></param>
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

        /// <summary>
        /// Creates and returns a new Entity as a child under this Entity
        /// </summary>
        /// <param name="childId"></param>
        /// <param name="inheritComponents"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds an existing Entity as a child to this Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Entity AddChild(Entity entity)
        {
            if (!this.IsAvailable())
                return null;

            entity.Parent = this;
            Children.Add(entity);

            return entity;
        }

        /// <summary>
        /// Get a child by ID
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        public Entity GetChild(string childId)
        {
            if (!this.IsAvailable())
                return null;

            return Children.FirstOrDefault(c => c.Id == childId);
        }

        /// <summary>
        /// Returns the whole "family tree" of this Entity (all children, "grandchildren", etc.)
        /// </summary>
        /// <returns></returns>
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