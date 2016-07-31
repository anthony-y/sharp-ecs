using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SharpECS;
using SharpECS.Samples.Components;
using SharpECS.Samples.Systems;
using System.Collections.Generic;

namespace SharpECS.Samples
{
    internal class Game 
        : Microsoft.Xna.Framework.Game
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
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            entityPool = EntityPool.New("EntityPool");

            playerEntity = entityPool.CreateEntity("Player");
            hostileEntity = entityPool.CreateEntity("HostileEntity");

            // Systems will refresh when new Entities have compatible components added to them.
            graphicsSystem = new GraphicsSystem(entityPool);
            controllerSystem = new ControllerSystem(entityPool);

            // One way of adding components.
            playerEntity += new TransformComponent() { Position = new Vector2(200, 364) };
            playerEntity += new GraphicsComponent() { Texture = Content.Load<Texture2D>("Sprite") };
            playerEntity += new ControllerComponent() { MoveSpeed = 100 };

            // Alternate way.
            hostileEntity.AddComponents
            (
                new GraphicsComponent() { Texture = Content.Load<Texture2D>("Sprite") },
                new TransformComponent() { Position = new Vector2(350, 200) }
            );

            var newEntity = entityPool.CreateEntity("NewEntity");
            newEntity.AddComponents(new TransformComponent(), new GraphicsComponent());

            newEntity.GetComponent<TransformComponent>().Position = new Vector2(450, 320);
            newEntity.GetComponent<GraphicsComponent>().Texture = Content.Load<Texture2D>("Sprite2");
            newEntity.Activate();

            Console.WriteLine("HostileEntity texture name: " + hostileEntity?.GetComponent<GraphicsComponent>()?.Texture?.Name);

            newEntity.CreateChild("ChildOfNew", false);

            newEntity.GetChild("ChildOfNew").CreateChild("GrandChildOfNew").CreateChild("GreatGrandChildOfNew");

            var grandChild = entityPool.GetEntity("GreatGrandChildOfNew");

            Console.WriteLine("Root entity of GrandChildOfNew (should be NewEntity): " + grandChild?.RootEntity?.Id);
            Console.WriteLine("Root entity of Player (should be Player): " + playerEntity.RootEntity.Id);

            var familyTree = newEntity.FamilyTree();

            var position = new Vector2(10, 10);
            var random = new Random();

            for (int i = 0; i < familyTree.Count(); i++)
            {
                var ent = familyTree.ElementAt(i);

                ent += new GraphicsComponent() { Texture = Content.Load<Texture2D>("Sprite"), };
                ent += new TransformComponent() { Position = new Vector2(position.X, position.Y) };

                position.X += 128;
                position.Y += 272;
            }

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
                && entityPool.DoesEntityExist(hostileEntity))
            {
                hostileEntity.GetComponent<TransformComponent>().Position = new Vector2(mouse.Position.X - 16, mouse.Position.Y - 16);
            }

            if (mouse.RightButton == ButtonState.Pressed && previousMouse.RightButton == ButtonState.Released
                && entityPool.DoesEntityExist(hostileEntity))
            {
                entityPool.DestroyEntity(ref hostileEntity);

                var fromTheCache = entityPool.CreateEntity("FromTheCache");

                IComponent[] components = new IComponent[]
                {
                    new TransformComponent() { Position = new Vector2(300, 256) },
                    new GraphicsComponent() { Texture = Content.Load<Texture2D>("Sprite") }
                };

                fromTheCache.AddComponents(components);
            }

            if (keyboard.IsKeyDown(Keys.R) && previousKeyboard.IsKeyUp(Keys.R) 
                    && entityPool.DoesEntityExist("Player"))
            {
                playerEntity.Switch();
            }

            controllerSystem?.Update(gameTime);

            previousMouse = mouse;
            previousKeyboard = keyboard;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            graphicsSystem?.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
