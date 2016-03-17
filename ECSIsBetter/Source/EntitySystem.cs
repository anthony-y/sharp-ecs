using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSIsBetter
{
    public abstract class EntitySystem<TComponent> where TComponent : IComponent
    {
        protected abstract List<Entity> CompatibleEntities { get; set; }

        public delegate void CompatibleEntityChanged(Entity entity, EntityPool pool);

        public event CompatibleEntityChanged EntityAdded;
        public event CompatibleEntityChanged EntityRemoved;

        // Actually not used :3
        private EntityPool _entityPool;

        public EntitySystem(EntityPool entityPool)
        {
            CompatibleEntities = new List<Entity>();
            _entityPool = entityPool;

            // Get all the entities that carry a component of type 'TComponent'
            // and add them all to a list.
            foreach (var ent in _entityPool.Entities)
            {
                if (ent.GetComponent<TComponent>() != null)
                {
                    CompatibleEntities.Add(ent);
                }
            }
        }

        public abstract void Update();
        
        public abstract void Draw();
    }
}
