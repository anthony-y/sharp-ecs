using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using SharpECS.Samples.Components;

namespace SharpECS.Samples.Systems
{
    internal class GraphicsSystem 
        : EntitySystem
    {
        public GraphicsSystem(EntityPool pool)
            : base(pool, typeof(GraphicsComponent), typeof(TransformComponent))
        { }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Compatible.Count; i++)
            {
                var transform = Compatible[i].GetComponent<TransformComponent>();
                var graphics = Compatible[i].GetComponent<GraphicsComponent>();

                transform.Rect = new Rectangle((int)transform.Position.X, (int)transform.Position.Y, graphics.Texture.Width, graphics.Texture.Height);

                if (Compatible[i].State == EntityState.Active)
                {
                    var texture = graphics.Texture;
                    var position = transform.Position;

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
}
