using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECSIsBetter;
using ECSIsBetter.Samples.Components;

namespace ECSIsBetter.Samples.Systems
{
    public class ControllerSystem : EntitySystem
    {
        public ControllerSystem(EntityGroup group)
            : base(group)
        {

        }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var entity in Group.Collection)
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
