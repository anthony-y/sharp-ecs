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
        private List<TransformComponent> _transforms;
        private List<GraphicsComponent> _graphics;

        public GraphicsSystem(EntityPool pool)
            : base(pool, typeof(GraphicsComponent), typeof(TransformComponent))
        {
            _transforms = new List<TransformComponent>();
            _graphics = new List<GraphicsComponent>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_transforms.Count <= 0 || _graphics.Count <= 0)
            {
                foreach (var e in Compatible)
                {
                    _transforms.Add(e.GetComponent<TransformComponent>());
                    _graphics.Add(e.GetComponent<GraphicsComponent>());
                }
            }

            for (int i = 0; i < Compatible.Count; i++)
            {
                var texture = _graphics[i].Texture;
                var position = _transforms[i].Position;

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
