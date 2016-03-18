using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSIsBetter
{
    public abstract class EntitySystem
    {
        public List<Entity> CompatibleEntities { get; set; }

        /// <summary>
        /// The EntityPool of which CompatibleEntities comes from.
        /// </summary>
        private EntityPool _compatiblePool;

        public EntitySystem(EntityPool entitySource)
        {
            _compatiblePool = entitySource;
            
            _compatiblePool.EntityAdded += OnCompatibleAdded;
            _compatiblePool.EntityRemoved += OnCompatibleRemoved;

            CompatibleEntities = _compatiblePool.Entities;
        }

        /// <summary>
        /// Refreshes the compatible entities list to include new entities and remove old ones.
        /// </summary>
        private void RefreshCompatible()
        {
            CompatibleEntities = _compatiblePool.Entities;
        }

        /// <summary>
        /// Event Handler for EntityPool's EntityAdded event.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="entity"></param>
        protected void OnCompatibleAdded(EntityPool pool, Entity entity)
        {
            RefreshCompatible();
#if DEBUG
            Console.WriteLine("EntitySystem refreshed because " + entity.Tag + " was added to " + _compatiblePool.Name);
#endif
        }

        /// <summary>
        /// Event Handler for EntityPool's EntityRemoved event.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="entity"></param>
        protected void OnCompatibleRemoved(EntityPool pool, Entity entity)
        {
            RefreshCompatible();
#if DEBUG
            Console.WriteLine("EntitySystem refreshed because an entity was removed from " + _compatiblePool.Name);
#endif
        }
    }
}
