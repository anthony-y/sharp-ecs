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

            Group.EntityAdded += GroupEntityAdded;
            Group.EntityRemoved += GroupEntityRemoved;
        }

        public void GroupEntityAdded(Entity entity, EntityGroup group)
        {
            Group = group;
        }

        public void GroupEntityRemoved(Entity entity, EntityGroup group)
        {
            Group = group;
        }
    }
}
