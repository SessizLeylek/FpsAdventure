using FpsAdventure.Scripts.Engine;
using FpsAdventure.Scripts.Player;
using FpsAdventure.Scripts.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FpsAdventure
{
    public class Game1 : Game
    {
        public static Game1 instance;

        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            instance = this;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            (new Camera(new Vector3(20, 10, 0), Vector3.Zero, 90)).PushToStack();
            GameObjectManager.Init();
            GameObjectManager.Create(new PlayerController());

            foreach(var adp in GraphicsAdapter.Adapters)
            {
                Debug.WriteLine($"GPU: {adp.Description}\n");
            }

            base.Initialize();
        }

        Model model;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("mdl_houseplant");
            ModelRendering.EnableModelLighting(model);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            CheckFullscreenAction();

            // TODO: Add your update logic here
            GameObjectManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            Debug.Assert(Camera.MainCamera != null, "There is no Main Camera");
            Matrix matView = Camera.MainCamera.ViewMatrix;
            Matrix matProj = Camera.MainCamera.ProjectionMatrix;

            float size = 0.01f;
            Matrix matWorld = new Matrix(size, 0, 0, 0,
                                            0, size, 0, 0,
                                            0, 0, size, 0,
                                            0, 0, 0, 1);

            model.Draw(matWorld, matView, matProj);

            Debugging.DrawGrid(this, matView, matProj);
            Debugging.DrawTriangle(this, matView, matProj, Terrain.groundTriangles[0]);
            Debugging.DrawTriangle(this, matView, matProj, Terrain.groundTriangles[1]);

            base.Draw(gameTime);
        }

        bool isFullscreen = false;
        public void ChangeFullscreen()
        {
            isFullscreen = !isFullscreen;

            if(isFullscreen)
            {
                var displayMode = GraphicsDevice.Adapter.CurrentDisplayMode;

                _graphics.PreferredBackBufferWidth = displayMode.Width;
                _graphics.PreferredBackBufferHeight = displayMode.Height;

                Window.IsBorderless = true;
                _graphics.HardwareModeSwitch = false;

                _graphics.ApplyChanges();
            }
            else
            {
                var displayMode = GraphicsDevice.Adapter.CurrentDisplayMode;

                _graphics.PreferredBackBufferWidth = displayMode.Width / 2;
                _graphics.PreferredBackBufferHeight = displayMode.Height / 2;

                // Enable borderless fullscreen
                Window.IsBorderless = false;
                _graphics.HardwareModeSwitch = true;

                // Apply changes
                _graphics.ApplyChanges();
            }
        }

        bool fullscreenButtonPressed = false;
        void CheckFullscreenAction()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F11)) fullscreenButtonPressed = true;
            else if(fullscreenButtonPressed)
            {
                fullscreenButtonPressed = false;
                ChangeFullscreen();
            }
        }

    }

    
}
