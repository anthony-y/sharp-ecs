using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECSIsBetter.Exceptions;

namespace ECSIsBetter
{
    public class EntityGroup
    {
        public List<Entity> Collection { get; set; }

        public string Name { get; set; }

        public delegate void GroupChanged(Entity entity, EntityGroup group);

        public event GroupChanged EntityAdded;
        public event GroupChanged EntityRemoved;

        public static EntityGroup New(string name, params Entity[] entities)
        {
            return new EntityGroup(name, entities);
        }

        public static EntityGroup New(string name)
        {
            return new EntityGroup(name);
        }

        private EntityGroup(string name)
        {
            Name = name;

            Collection = new List<Entity>();
        }

        private EntityGroup(string name, params Entity[] entities)
        {
            Name = name;

            Collection = new List<Entity>();

            AddEntities(entities);
        }

        public void AddEntity(Entity entity)
        {
            if (entity != null)
            {
                Collection.Add(entity);
                if (EntityAdded != null) EntityAdded(entity, this);
            }
            else throw new EntityNotFoundException(entity.OwnerPool);
        }

        public void AddEntities(params Entity[] entities)
        {
            foreach (var i in entities)
            {
                if (i != null)
                {
                    Collection.Add(i);
                    if (EntityAdded != null) EntityAdded(i, this);
                } else
                {
                    throw new EntityNotFoundException(i.OwnerPool);
                }
            }
        }

        public void RemoveEntity(Entity entity)
        {
            if (entity == null || !Collection.Contains(entity))
            {
                throw new EntityNotFoundException(entity.OwnerPool);
            }

            Collection.Remove(entity);

            if (EntityRemoved != null) EntityRemoved(entity, this);
        }

    }
}
