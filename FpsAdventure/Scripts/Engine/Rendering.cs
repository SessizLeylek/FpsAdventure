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

}
