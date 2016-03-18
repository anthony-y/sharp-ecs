using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ECSIsBetter;

using ECSIsBetter.Samples.Components;
using ECSIsBetter.Samples.Systems;

namespace ECSIsBetter.Samples
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        EntityPool entityPool;

        EntityGroup renderGroup;
        EntityGroup controllableGroup;

        GraphicsSystem graphicsSystem;
        ControllerSystem controllerSystem;

        Entity playerEntity;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            entityPool = EntityPool.New("EntityPool");

            renderGroup = EntityGroup.New("GraphicsGroup");
            controllableGroup = EntityGroup.New("ControllableGroup");

            playerEntity = entityPool.CreateEntity("Player");

            playerEntity += new ControllerComponent();
            playerEntity += new TransformComponent();
            playerEntity += new GraphicsComponent();

            playerEntity.GetComponent<TransformComponent>().Position = new Vector2(10, 20);
            playerEntity.GetComponent<GraphicsComponent>().Texture = Content.Load<Texture2D>("Sprite");

            renderGroup.AddEntity(playerEntity);
            controllableGroup.AddEntity(playerEntity);

            graphicsSystem = new GraphicsSystem(renderGroup);
            controllerSystem = new ControllerSystem(controllableGroup);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            controllerSystem.Update(gameTime);

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
