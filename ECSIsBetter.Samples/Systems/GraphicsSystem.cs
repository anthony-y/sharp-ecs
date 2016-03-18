using System;

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
}
