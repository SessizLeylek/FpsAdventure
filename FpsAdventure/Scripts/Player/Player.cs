using FpsAdventure.Scripts.Engine;
using FpsAdventure.Scripts.World;
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
        const float PLAYER_SPEED = 5f;

        Camera camera = new Camera(Vector3.UnitY, Vector3.Zero, 90);
        Triangle terrainTriangleUnder = null;
        Vector3 position;
        Vector2 directionRadians;
        bool mouseLookActive = true;
        bool headBobActive = true;
        float walkingForSeconds = 0f;



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
            if(mouseLookActive)
            {
                Point screenCenter = new Point(Game1.instance._graphics.PreferredBackBufferWidth / 2, Game1.instance._graphics.PreferredBackBufferHeight / 2);

                Point deltaMousePoint = currentState.mouseState.Position - screenCenter;
                Vector2 deltaMouse = deltaMousePoint.ToVector2() / Game1.instance.Window.ClientBounds.Height;
                directionRadians -= deltaMouse;

                // Clamp Vertical Look
                if (directionRadians.Y > MathF.PI * 0.45f) directionRadians.Y = MathF.PI * 0.45f;
                else if (directionRadians.Y < MathF.PI * -0.45f) directionRadians.Y = MathF.PI * -0.45f;

                // Lock Cursor
                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
                Game1.instance.IsMouseVisible = false;
            }

            //////////////////////////////////////////////////////////
            // PLAYER MOVEMENT
            float playerSpeed = PLAYER_SPEED;
            if (currentState.keyboardState.IsKeyDown(Keys.LeftShift)) playerSpeed *= 2;

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
            if(moveAmount.Length() == 0)
            {
                // update walk cycle parameter
                walkingForSeconds = walkingForSeconds % 1;
                walkingForSeconds -= (float)currentState.time.ElapsedGameTime.TotalSeconds;
                if(walkingForSeconds < 0) walkingForSeconds = 0;
            }
            else
            {
                walkingForSeconds += (float)currentState.time.ElapsedGameTime.TotalSeconds * playerSpeed * 0.4f;
                moveAmount.Normalize();
            }
            Move(moveAmount * (float)currentState.time.ElapsedGameTime.TotalSeconds * playerSpeed);

            //////////////////////////////////////////////////////////
            // CAMERA UPDATE
            Vector3 lookDirection = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(directionRadians.Y));
            lookDirection = Vector3.Transform(lookDirection, Matrix.CreateRotationY(directionRadians.X));
            float headBob = 0;
            if (headBobActive) headBob = MathF.Abs(MathF.Sin(MathF.PI * walkingForSeconds) * 0.2f);
            camera.UpdateCamera(position: position + new Vector3(0, 1.8f + headBob, 0), direction: lookDirection);
        }
        public void OnDestroy(GameInputState prevState, GameInputState currentState)
        {
            throw new NotImplementedException();
        }

        private bool CheckGround(Vector3 relativePoint, out Vector3 groundPoint)
        {
            Ray ray = new Ray(relativePoint + new Vector3(0, 0.05f, 0), Vector3.Down);
            terrainTriangleUnder = Raycasting.ClosestTriangle(ray, Terrain.groundTriangles, out float dist, 0.3f);

            groundPoint = ray.Position + ray.Direction * dist;
            return terrainTriangleUnder != null;
        }

        private void Move(Vector3 deltaPosition)
        {
            // Below part is for to be able to walk while on edges, i know it is bad but i have no other solution
            if(!MoveTowardsPoint(deltaPosition))
            {
                // if player cannot move forward
                // it checks possible move points
                for(float f = 0; f < 0.5f; f += 0.05f)
                {
                    Vector3 sparePosition0 = Vector3.Transform(deltaPosition, Matrix.CreateRotationY(f * MathF.PI)) * (0.5f - f) * 2;
                    Vector3 sparePosition1 = Vector3.Transform(deltaPosition, Matrix.CreateRotationY(-f * MathF.PI)) * (0.5f - f) * 2;
                    if (MoveTowardsPoint(sparePosition0)) break;
                    else if (MoveTowardsPoint(sparePosition1)) break;
                    
                }
            }
        }

        private bool MoveTowardsPoint(Vector3 deltaPosition)
        {
            // if there is a ground under the delta position, player moves there
            if (CheckGround(position + deltaPosition + Vector3.Up * deltaPosition.Length(), out Vector3 point))
            {
                position = point;
                return true;
            }

            return false;
        }

        // There is no need to draw the player
        public void Draw() { }
    }
}
