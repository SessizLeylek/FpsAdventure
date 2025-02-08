using FpsAdventure.Scripts.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FpsAdventure.Scripts.Engine
{
    public static class ModelRendering
    {
        public static void EnableModelLighting(Model model)
        {
            // Enables DirectionalLighting for meshes
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = new Vector3(-1, -1, -1); // Light direction
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f); // White light
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1); // Adds highlights
                }
            }
        }
    }

    public class Camera
    {
        public Vector3 CameraPosition => camPos;
        public Vector3 CameraTarget => camTarget;
        public Vector3 CameraDirection => Vector3.Normalize(camTarget - camPos);
        public Matrix ViewMatrix => matView;
        public Matrix ProjectionMatrix => matProj;
        public static Camera MainCamera => cameraStack[cameraStack.Count - 1];

        static List<Camera> cameraStack = new List<Camera>();
        Vector3 camPos;
        Vector3 camTarget;
        Matrix matView;
        Matrix matProj;
        float fov;

        public Camera(Vector3 position, Vector3 target, float _fov)
        {
            UpdateCamera(position, target);
            UpdateFOV(_fov);
        }

        public void UpdateCamera(Vector3? position = null, Vector3? target = null, Vector3? direction = null)
        {
            if (position.HasValue)
                camPos = position.Value;
            if (target.HasValue)
                camTarget = target.Value;
            if (direction.HasValue)
                camTarget = camPos + direction.Value;

            matView = Matrix.CreateLookAt(camPos, camTarget, Vector3.Up);
        }

        public void UpdateFOV(float fovInDegrees)
        {
            const float NEAR_PLANE = 0.001f;
            const float FAR_PLANE = 1000f;

            var bounds = Game1.instance.Window.ClientBounds;
            float aspectRatio =bounds.Width / (float)bounds.Height;

            fov = MathHelper.ToRadians(fovInDegrees);

            matProj = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, NEAR_PLANE, FAR_PLANE);
        }

        public void PushToStack()
        {
            cameraStack.Add(this);
        }

        public void RemoveFromStack()
        {
            cameraStack.Remove(this);
        }
    }

    public static class Debugging
    {
        public static void DrawGrid(Game g, Matrix cameraView, Matrix cameraProjection)
        {
            int gridSize = 50;
            float spacing = 1.0f;
            BasicEffect effect = new BasicEffect(g.GraphicsDevice);

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
                g.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
            }
        }

        public static void DrawTriangle(Game g, Matrix cameraView, Matrix cameraProjection, Triangle triangle)
        {

            BasicEffect effect = new BasicEffect(g.GraphicsDevice);

            effect.World = Matrix.Identity;
            effect.View = cameraView;  // Replace with your camera's view matrix
            effect.Projection = cameraProjection;  // Replace with your camera's projection matrix
            effect.VertexColorEnabled = true;

            // Create vertices with positions and colors
            VertexPositionColor[] vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(triangle.v0, Color.Lime),
                new VertexPositionColor(triangle.v1, Color.Lime),
                new VertexPositionColor(triangle.v2, Color.Lime),
            };

            // Define the triangle indices (0, 1, 2)
            short[] indices = new short[] { 0, 1, 2 };

            // Set up buffers
            VertexBuffer vertexBuffer = new VertexBuffer(g.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            IndexBuffer indexBuffer = new IndexBuffer(g.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            // Apply effect and draw
            g.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            g.GraphicsDevice.Indices = indexBuffer;

            effect.CurrentTechnique.Passes[0].Apply();
            g.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 1);
        }

    }

}
