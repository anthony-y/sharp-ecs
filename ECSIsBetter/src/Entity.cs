using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            ComponentAdded(this, component);

            return component;
        }

        public void RemoveComponent(IComponent component)
        {
            if (!Components.Contains(component)) return;

            Components.Remove(component);

            ComponentRemoved(this, component);
        }

        public T GetComponent<T>() where T : IComponent
        {
           return (T)Components.FirstOrDefault(ent => ent.GetType() == typeof(T));
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

    }
}
