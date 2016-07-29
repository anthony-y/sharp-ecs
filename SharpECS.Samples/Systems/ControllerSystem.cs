using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using SharpECS.Samples.Components;

namespace SharpECS.Samples.Systems
{
    internal class ControllerSystem 
        : EntitySystem
    {
        public ControllerSystem(EntityPool pool)
            : base(pool, typeof(ControllerComponent), typeof(TransformComponent))
        { }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < Compatible.Count; i++)
            {
                if (Compatible[i].State == EntityState.Active)
                {
                    var transform = Compatible[i].GetComponent<TransformComponent>();
                    var moveSpeed = Compatible[i].GetComponent<ControllerComponent>().MoveSpeed;

                    if (Keyboard.GetState().IsKeyDown(Keys.D)) { transform.SetX(transform.Position.X + moveSpeed * delta); }
                    if (Keyboard.GetState().IsKeyDown(Keys.A)) { transform.SetX(transform.Position.X - moveSpeed * delta); }
                    if (Keyboard.GetState().IsKeyDown(Keys.W)) { transform.SetY(transform.Position.Y - moveSpeed * delta); }
                    if (Keyboard.GetState().IsKeyDown(Keys.S)) { transform.SetY(transform.Position.Y + moveSpeed * delta); }
                }
            }
        }
    }
}
