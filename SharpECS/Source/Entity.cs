using System;
using System.Collections.Generic;
using System.Linq;

using SharpECS.Exceptions;

namespace SharpECS
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

        internal Entity(string id, EntityPool pool)
        {   
            Id = id;

            if (pool == null) throw new IndependentEntityException(this);

            OwnerPool = pool;

            Components = new List<IComponent>();
            Children = new List<Entity>();
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
            // If it has a component of the same type as "component".
            if (this.Components.FirstOrDefault(com => com.GetType() == component.GetType()) != null)
            {
                throw new ComponentAlreadyExistsException(this);
            }

            Components.Add(component);
            ComponentAdded?.Invoke(this, component);
            OwnerPool.ComponentAdded(this);

            return component;
        }

        public void RemoveComponent<T>() where T : IComponent
        {
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
        public T GetComponent<T>() where T : IComponent
        {
            var match = Components.OfType<T>().FirstOrDefault();

            if (match == null) throw new ComponentNotFoundException(this);

            return match;
        }

        public IComponent GetComponent(Type componentType)
        {
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
        /// Checks if "this" contains a component of type "TComponent".
        /// If it does, return true. Otherwise, return false.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool HasComponent<TComponent>() where TComponent : IComponent
        {
            var match = Components.OfType<TComponent>().FirstOrDefault();

            if (match != null) return true;
            else return false;
        }

        public bool HasComponent(Type componentType)
        {
            if (!Util.ImplementsInterface(componentType, typeof(IComponent)))
                throw new Exception("One or more of the types you passed were not IComponent children.");

            var cMatch = Components.FirstOrDefault(com => com.GetType() == componentType);
            if (cMatch != null) return true;

            return false;
        }

        public bool HasComponents(IEnumerable<Type> types)
        {
            foreach (var t in types)
                if (!HasComponent(t)) return false;

            return true;
        }

        /// <summary>
        /// Remove all components.
        /// </summary>
        public void RemoveAllComponents()
        {
            foreach (var c in Components) RemoveComponent(c.GetType());

            Components.Clear();
        }

        /// <summary>
        /// RemoveAllComponents(), reset Id, OwnerPool.
        /// </summary>
        public void Reset()
        {
            RemoveAllComponents();
            foreach (var ent in Children) { OwnerPool.DestroyEntity(ent); }
            Children.Clear();

            this.Id = string.Empty;
            this.OwnerPool = null;
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
        /// Creates a "carbon copy" of this Entity with the Id of "newId".
        /// </summary>
        /// <param name="newId"></param>
        /// <returns></returns>
        public Entity CarbonCopy(string newId)
        {
            var newEntity = OwnerPool.CreateEntity(newId);

            newEntity.AddComponents(Components);

            return newEntity;
        }

        public Entity CreateChild(string childId, bool inheritComponents=false)
        {
            var child = OwnerPool.CreateEntity(childId);

            child.Parent = this;
            if (inheritComponents) { child.AddComponents(Components); }

            Children.Add(child);

            return child;
        }

        public Entity AddChild(Entity entity)
        {
            entity.Parent = this;
            Children.Add(entity);

            return entity;
        }

        public Entity GetChild(string childId)
        {
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