using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
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
                var texture = Compatible[i].GetComponent<GraphicsComponent>().Texture;
                var position = Compatible[i].GetComponent<TransformComponent>().Position;

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
