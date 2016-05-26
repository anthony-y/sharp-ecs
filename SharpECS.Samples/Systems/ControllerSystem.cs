using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using SharpECS.Samples.Components;

namespace SharpECS.Samples.Systems
{
    internal class ControllerSystem 
        : EntitySystem<ControllerComponent>
    {
        public ControllerSystem(EntityPool pool)
            : base(pool)
        {

        }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var entity in Compatible)
            {
                var transform = entity.GetComponent<TransformComponent>();
                var moveSpeed = entity.GetComponent<ControllerComponent>().MoveSpeed;

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    transform.SetX(transform.Position.X + moveSpeed * delta);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    transform.SetX(transform.Position.X - moveSpeed * delta);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    transform.SetY(transform.Position.Y - moveSpeed * delta);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    transform.SetY(transform.Position.Y + moveSpeed * delta);
                }
            }
        }
    }
}
