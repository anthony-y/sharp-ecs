using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SharpECS;
using SharpECS.Samples.Components;
using SharpECS.Samples.Systems;

namespace SharpECS.Samples
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState keyboard;
        KeyboardState previousKeyboard;

        MouseState mouse;
        MouseState previousMouse;

        EntityPool entityPool;

        GraphicsSystem graphicsSystem;
        ControllerSystem controllerSystem;

        Entity playerEntity;
        Entity hostileEntity;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            entityPool = EntityPool.New("EntityPool");

            playerEntity = entityPool.CreateEntity("Player");
            hostileEntity = entityPool.CreateEntity("HostileEntity");

            // Generic
            graphicsSystem = new GraphicsSystem(entityPool);

            // Group
            controllerSystem = new ControllerSystem(entityPool);

            playerEntity += new TransformComponent();
            playerEntity += new GraphicsComponent();
            playerEntity += new ControllerComponent();

            hostileEntity += new GraphicsComponent();
            hostileEntity += new TransformComponent();

            playerEntity.GetComponent<TransformComponent>().Position = new Vector2(10, 20);
            playerEntity.GetComponent<GraphicsComponent>().Texture = Content.Load<Texture2D>("Sprite");

            hostileEntity.GetComponent<TransformComponent>().Position = new Vector2(350, 200);
            hostileEntity.GetComponent<GraphicsComponent>().Texture = Content.Load<Texture2D>("Sprite");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.Escape)) Exit();

            if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released
                && hostileEntity.Tag != string.Empty)
            {
                entityPool.GetEntity("HostileEntity").LastComponent().Position = new Vector2(mouse.Position.X - 16, mouse.Position.Y - 16);

                //entityPool.DestroyEntity(hostileEntity);
            }

            //Console.WriteLine("Count: " + entityPool.Entities.Count);

            controllerSystem.Update(gameTime);

            previousMouse = mouse;
            previousKeyboard = keyboard;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            graphicsSystem.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
