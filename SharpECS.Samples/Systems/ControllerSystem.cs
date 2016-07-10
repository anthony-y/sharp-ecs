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
        private List<TransformComponent> _transforms;
        private List<ControllerComponent> _controls;

        public ControllerSystem(EntityPool pool)
            : base(pool, typeof(ControllerComponent), typeof(TransformComponent))
        {
            _transforms = new List<TransformComponent>();
            _controls = new List<ControllerComponent>();
        }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_transforms.Count <= 0 || _controls.Count <= 0)
            {
                foreach (var e in Compatible)
                {
                    _transforms.Add(e.GetComponent<TransformComponent>());
                    _controls.Add(e.GetComponent<ControllerComponent>());
                }
            }

            for (int i = 0; i < Compatible.Count; i++)
            {
                var transform = _transforms[i];
                var moveSpeed = _controls[i].MoveSpeed;

                if (Keyboard.GetState().IsKeyDown(Keys.D)) { transform.SetX(transform.Position.X + moveSpeed * delta); }
                if (Keyboard.GetState().IsKeyDown(Keys.A)) { transform.SetX(transform.Position.X - moveSpeed * delta); }
                if (Keyboard.GetState().IsKeyDown(Keys.W)) { transform.SetY(transform.Position.Y - moveSpeed * delta); }
                if (Keyboard.GetState().IsKeyDown(Keys.S)) { transform.SetY(transform.Position.Y + moveSpeed * delta); }
            }
        }
    }
}
