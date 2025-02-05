using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FpsAdventure.Scripts.Engine
{
    // Manages gameobjects
    public interface IGameObject
    {
        public void OnStart(GameInputState prevState, GameInputState currentState);
        public void OnDestroy(GameInputState prevState, GameInputState currentState);
        public void OnUpdate(GameInputState prevState, GameInputState currentState);
        public void Draw();
    }

    public struct GameInputState
    {
        public GameTime time;
        public KeyboardState keyboardState;
        public GamePadState padState;
        public MouseState mouseState;

        public GameInputState(GameTime _time, KeyboardState _keyboardState, GamePadState _padState, MouseState _mouseState)
        {
            time = _time;
            keyboardState = _keyboardState;
            padState = _padState;
            mouseState = _mouseState;
        }
    }

    public static class GameObjectManager
    {
        static List<IGameObject> gameObjects;
        static List<IGameObject> gameObjectsToCreate;
        static List<IGameObject> gameObjectsToDestroy;
        static GameInputState previousState;

        public static void Init()
        {
            gameObjects = new List<IGameObject>();
            gameObjectsToCreate = new List<IGameObject>();
            gameObjectsToDestroy = new List<IGameObject>();
        }

        public static void Create(IGameObject gameObject)
        {
            gameObjectsToCreate.Add(gameObject);
        }

        public static void Destroy(IGameObject gameObject)
        {
            if (gameObjects.Contains(gameObject))
            {
                if (!gameObjectsToDestroy.Contains(gameObject))
                    gameObjectsToDestroy.Add(gameObject);
            }
            else
            {
                Debug.Fail("Object does not exists");
            }
        }

        public static void Update(GameTime gameTime)
        {
            GameInputState state = new GameInputState(gameTime, Keyboard.GetState(), GamePad.GetState(0), Mouse.GetState());

            foreach (IGameObject gameObject in gameObjects)
            {
                gameObject.OnUpdate(previousState, state);
            }

            while (gameObjectsToCreate.Count > 0)
            {
                IGameObject gameObject = gameObjectsToCreate[0];

                gameObject.OnStart(previousState, state);
                gameObjects.Add(gameObject);
                gameObjectsToCreate.RemoveAt(0);
            }

            while (gameObjectsToDestroy.Count > 0)
            {
                IGameObject gameObject = gameObjectsToDestroy[0];

                gameObject.OnDestroy(previousState, state);
                gameObjects.Remove(gameObject);
                gameObjectsToDestroy.RemoveAt(0);
            }

            previousState = state;
        }
    }
}
