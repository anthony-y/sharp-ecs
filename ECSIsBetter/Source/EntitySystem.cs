using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSIsBetter
{
    public abstract class EntitySystem
    {
        public EntityGroup Group { get; set; }

        public EntitySystem(EntityGroup group)
        {
            Group = group;

            Group.EntityAdded += OnGroupEntityAdded;
            Group.EntityRemoved += OnGroupEntityRemoved;
        }

        public virtual void OnGroupEntityAdded(Entity entity, EntityGroup group, List<Entity> newCollection)
        {
            Group = group;
            Group.Collection = newCollection;
        }

        public virtual void OnGroupEntityRemoved(Entity entity, EntityGroup group, List<Entity> newCollection)
        {
            Group = group;
            Group.Collection = newCollection;
            if (Group.Collection.Contains(entity)) group.Collection.Remove(entity);
        }
    }
}
