using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpECS.Samples.Components;
using System;

namespace SharpECS.Samples.Systems
{
    public class GraphicsSystem 
        : EntitySystem
    {
        public GraphicsSystem(EntityPool pool, params Type[] compatible)
            : base(pool, compatible)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in Compatible)
            {
                string comId = (entity.Id == "Player" ? "transformOne" : "");

                var texture = entity.GetComponent<GraphicsComponent>().Texture;
                var position = entity.GetComponent<TransformComponent>(comId).Position;

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