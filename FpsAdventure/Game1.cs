using FpsAdventure.Scripts.Engine;
using FpsAdventure.Scripts.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

            // TODO: Add your update logic here
            Debug.WriteLine(Camera.MainCamera.CameraPosition);

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

            DrawGrid(matView, matProj);

            base.Draw(gameTime);
        }

        //temp
        public void DrawGrid(Matrix cameraView, Matrix cameraProjection)
        {
            int gridSize = 50;
            float spacing = 1.0f;
            BasicEffect effect = new BasicEffect(GraphicsDevice);

            List<VertexPositionColor> vertices = new List<VertexPositionColor>();
            Color gridColor = Color.Gray;

            // Draw lines along the X-axis
            for (int i = -gridSize; i <= gridSize; i++)
            {
                vertices.Add(new VertexPositionColor(new Vector3(i * spacing, 0, -gridSize * spacing), gridColor));
                vertices.Add(new VertexPositionColor(new Vector3(i * spacing, 0, gridSize * spacing), gridColor));
            }

            // Draw lines along the Z-axis
            for (int i = -gridSize; i <= gridSize; i++)
            {
                vertices.Add(new VertexPositionColor(new Vector3(-gridSize * spacing, 0, i * spacing), gridColor));
                vertices.Add(new VertexPositionColor(new Vector3(gridSize * spacing, 0, i * spacing), gridColor));
            }

            // Set effect properties
            effect.World = Matrix.Identity;
            effect.View = cameraView;  // Replace with your camera's view matrix
            effect.Projection = cameraProjection;  // Replace with your camera's projection matrix
            effect.VertexColorEnabled = true;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
            }
        }

    }

    
}
