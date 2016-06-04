using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpECS.Samples.Components;
using SharpECS.Samples.Systems;

namespace SharpECS.Samples
{
    internal class SampleGame 
        : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
         
        private KeyboardState _keyboard;
        private KeyboardState _previousKeyboard;
         
        private MouseState _mouse;
        private MouseState _previousMouse;
         
        private EntityPool _entityPool;
         
        private GraphicsSystem _graphicsSystem;
        private ControllerSystem _controllerSystem;
         
        private Entity _playerEntity;
        private Entity _hostileEntity;

        public SampleGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _entityPool = EntityPool.New("EntityPool");

            _playerEntity = _entityPool.CreateEntity("Player");
            _hostileEntity = _entityPool.CreateEntity("HostileEntity");

            // Systems will refresh when new Entities have compatible components added to them.
            _graphicsSystem = new GraphicsSystem(_entityPool);
            _controllerSystem = new ControllerSystem(_entityPool);

            // One way of adding components.
            _playerEntity += new TransformComponent { Position = new Vector2(10, 20) };
            _playerEntity += new GraphicsComponent { Texture = Content.Load<Texture2D>("Sprite") };
            _playerEntity += new ControllerComponent();

            // Alternate way.
            _hostileEntity.AddComponents
            (
                new GraphicsComponent { Texture = Content.Load<Texture2D>("Sprite") },
                new TransformComponent { Position = new Vector2(350, 200) }
            );

            var showOffCarbonCopy = _playerEntity.CarbonCopy("MadeWithCarbonCopy(TM)");

            // Should be identical to Player.
            showOffCarbonCopy.Components.ForEach(compo => { Console.WriteLine(compo.ToString()); });

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            _keyboard = Keyboard.GetState();
            _mouse = Mouse.GetState();

            if (_keyboard.IsKeyDown(Keys.Escape)) Exit();

            if (_mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released
                && _entityPool.DoesEntityExist(_hostileEntity))
            {
                _hostileEntity.GetComponent<TransformComponent>().Position = new Vector2(_mouse.Position.X - 16, _mouse.Position.Y - 16);
            }

            if (_mouse.RightButton == ButtonState.Pressed && _previousMouse.RightButton == ButtonState.Released
                && _entityPool.DoesEntityExist(_hostileEntity))
            {
                _entityPool.DestroyEntity(_hostileEntity);
            }

#if DEBUG
            foreach (var i in _entityPool.Entities)
            {
                Console.WriteLine($"Entity: {i.Tag}");
            }
#endif
            _controllerSystem.Update(gameTime);

            _previousMouse = _mouse;
            _previousKeyboard = _keyboard;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _graphicsSystem.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
