using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpECS.Samples.Components;
using System;

namespace SharpECS.Samples.Systems
{
    public class ControllerSystem : EntitySystem
    {
        public ControllerSystem(EntityPool pool, params Type[] compatible)
            : base(pool, compatible)
        {

        }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var entity in Compatible)
            {
                string comId = (entity.Id == "Player" ? "transformOne" : "");
                
                var transform = entity.GetComponent<TransformComponent>(comId);

                var moveSpeed = entity.GetComponent<ControllerComponent>().MoveSpeed;

                if (Keyboard.GetState().IsKeyDown(Keys.D)) { transform.SetX(transform.Position.X + moveSpeed * delta); }
                if (Keyboard.GetState().IsKeyDown(Keys.A)) { transform.SetX(transform.Position.X - moveSpeed * delta); }
                if (Keyboard.GetState().IsKeyDown(Keys.W)) { transform.SetY(transform.Position.Y - moveSpeed * delta); }
                if (Keyboard.GetState().IsKeyDown(Keys.S)) { transform.SetY(transform.Position.Y + moveSpeed * delta); }
            }
        }
    }
}