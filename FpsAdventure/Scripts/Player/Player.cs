using FpsAdventure.Scripts.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FpsAdventure.Scripts.Player
{
    public class PlayerController : IGameObject
    {
        float PLAYER_SPEED = 5f;

        Camera camera = new Camera(Vector3.UnitY, Vector3.Zero, 90);
        Vector3 position;
        Vector2 directionRadians;


        public void OnStart(GameInputState prevState, GameInputState currentState)
        {
            position = Vector3.Zero;
            directionRadians = Vector2.Zero;

            camera.PushToStack();
        }

        public void OnUpdate(GameInputState prevState, GameInputState currentState)
        {
            //////////////////////////////////////////////////////////
            // CAMERA LOOK
            Point screenCenter = new Point(Game1.instance._graphics.PreferredBackBufferWidth / 2, Game1.instance._graphics.PreferredBackBufferHeight / 2);

            Point deltaMousePoint = currentState.mouseState.Position - screenCenter;
            Vector2 deltaMouse = deltaMousePoint.ToVector2() / Game1.instance.Window.ClientBounds.Height;
            directionRadians -= deltaMouse;

            // Clamp Vertical Look
            if (directionRadians.Y > MathF.PI * 0.45f) directionRadians.Y = MathF.PI * 0.45f;
            else if (directionRadians.Y < MathF.PI * -0.45f) directionRadians.Y = MathF.PI * -0.45f;

            // Lock Cursor
            if (Camera.MainCamera == camera)
            {
                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
                Game1.instance.IsMouseVisible = false;
            }

            //////////////////////////////////////////////////////////
            // PLAYER MOVEMENT
            Vector3 moveAmount = Vector3.Zero;
            if (currentState.keyboardState.IsKeyDown(Keys.W))
            {
                moveAmount.Z = -1;
            }
            else if (currentState.keyboardState.IsKeyDown(Keys.S))
            {
                moveAmount.Z = 1;
            }
            if (currentState.keyboardState.IsKeyDown(Keys.A))
            {
                moveAmount.X = -1;
            }
            else if (currentState.keyboardState.IsKeyDown(Keys.D))
            {
                moveAmount.X = 1;
            }
            moveAmount = Vector3.Transform(moveAmount, Matrix.CreateRotationY(directionRadians.X));
            if(moveAmount.Length() != 0) moveAmount.Normalize();
            position += moveAmount * (float)currentState.time.ElapsedGameTime.TotalSeconds * PLAYER_SPEED;

            //////////////////////////////////////////////////////////
            // CAMERA UPDATE
            Vector3 lookDirection = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(directionRadians.Y));
            lookDirection = Vector3.Transform(lookDirection, Matrix.CreateRotationY(directionRadians.X));
            camera.UpdateCamera(position: position + new Vector3(0, 1.8f, 0), direction: lookDirection);
        }
        public void OnDestroy(GameInputState prevState, GameInputState currentState)
        {
            throw new NotImplementedException();
        }

        // There is no need to draw the player
        public void Draw() { }
    }
}
