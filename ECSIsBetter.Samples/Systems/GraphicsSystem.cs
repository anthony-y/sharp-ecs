using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ECSIsBetter;
using ECSIsBetter.Samples.Components;

namespace ECSIsBetter.Samples.Systems
{
    public class GraphicsSystem : EntitySystem
    {
        public GraphicsSystem(EntityGroup group)
            : base(group)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in this.Group.Collection)
            {
                if (entity.Tag != string.Empty)
                {
                    var texture = entity.GetComponent<GraphicsComponent>().Texture;
                    var position = entity.GetComponent<TransformComponent>().Position;

                    spriteBatch.Draw
                    (
                        texture,
                        position,
                        Color.White
                    );
                }
            }
        }

        public override void OnGroupEntityAdded(Entity entity, EntityGroup group, List<Entity> newCollection)
        {
            Console.WriteLine("EntitySystem refreshed because Entity \"" + entity.Tag + "\" was added.");

            base.OnGroupEntityAdded(entity, group, newCollection);
        }

        public override void OnGroupEntityRemoved(Entity entity, EntityGroup group, List<Entity> newCollection)
        {
            Console.WriteLine("EntitySystem refreshed because Entity \"" + entity.Tag + "\" was removed.");
            Console.WriteLine("EntitySystem now has " + newCollection.Count + " in group.");

            base.OnGroupEntityRemoved(entity, group, newCollection);
        }

    }
}
