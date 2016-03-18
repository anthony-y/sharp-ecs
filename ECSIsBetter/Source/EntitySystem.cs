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

        private EntityPool _ownerPool;

        public EntitySystem(EntityPool entitySource)
        {
            _ownerPool = entitySource;
            
            _ownerPool.EntityAdded += OnCompatibleAdded;
            _ownerPool.EntityRemoved += OnCompatibleRemoved;

            CompatibleEntities = _ownerPool.Entities;
        }

        private void RefreshCompatible()
        {
            CompatibleEntities = _ownerPool.Entities;
        }

        protected void OnCompatibleAdded(EntityPool pool, Entity entity)
        {
            RefreshCompatible();
#if DEBUG
            Console.WriteLine("EntitySystem refreshed because " + entity.Tag + " was added to " + _ownerPool.Name);
#endif
        }

        protected void OnCompatibleRemoved(EntityPool pool, Entity entity)
        {
            RefreshCompatible();
#if DEBUG
            Console.WriteLine("EntitySystem refreshed because an entity was removed from " + _ownerPool.Name);
#endif
        }

        public abstract void Initialize();

        public abstract void Update();

        public abstract void Draw();

    }
}
