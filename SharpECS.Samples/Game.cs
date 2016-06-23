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
            playerEntity += new TransformComponent() { Position = new Vector2(10, 20) };
            playerEntity += new GraphicsComponent() { Texture = Content.Load<Texture2D>("Sprite") };
            playerEntity += new ControllerComponent();

            // Alternate way.
            hostileEntity.AddComponents
            (
                new GraphicsComponent() { Texture = Content.Load<Texture2D>("Sprite") },
                new TransformComponent() { Position = new Vector2(350, 200) }
            );

            var showOffCarbonCopy = playerEntity.CarbonCopy("MadeWithCarbonCopy");
            Console.WriteLine($"Id of showOffCarbonCopy: \"{showOffCarbonCopy.Id}\"");

            // Should be identical to Player.
            //showOffCarbonCopy.Components.ForEach(compo => { Console.WriteLine(compo.ToString()); });

            showOffCarbonCopy.CreateChild("ChildOfCC", true);

            showOffCarbonCopy.GetChild("ChildOfCC")
                .CreateChild("GrandChildOfCC")
                    .CreateChild("GreatGrandChildOfCC");

            var grandChild = showOffCarbonCopy.GetChild("ChildOfCC")
                .GetChild("GrandChildOfCC")
                    .GetChild("GreatGrandChildOfCC");

            Console.WriteLine("Root entity of GrandChildOfCC (should be MadeWithCarbonCopy): " + grandChild.RootEntity.Id);
            Console.WriteLine("Root entity of Player (should be Player): " + playerEntity.RootEntity.Id);

            //entityPool.Entities.ForEach(ent => { Console.WriteLine(ent.Id); });
            //showOffCarbonCopy.Children.ForEach(child => { Console.WriteLine(child.Id); });

            var familyTree = showOffCarbonCopy.FamilyTree();

            float posX = 10;
            float posY = 10;

            for (int i = 0; i < familyTree.Count(); i++)
            {
                var ent = familyTree.ToList()[i];

                ent += new TransformComponent()
                {
                    Position = new Vector2(posX, posY)
                };

                posX += 32;
                posY += 64;

                ent += new GraphicsComponent()
                {
                    Texture = Content.Load<Texture2D>("Sprite"),
                };
            }

            //entityPool.DestroyEntity(showOffCarbonCopy);

            //var fromTheCache = entityPool.CreateEntity("FromTheCache");

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
                entityPool.DestroyEntity(hostileEntity);
            }

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
