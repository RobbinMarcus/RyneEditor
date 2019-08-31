using System.Collections.Generic;
using Ryne.Entities;
using Ryne.Utility;
using Ryne.Utility.Collections;

namespace Ryne.GameStates
{
    public class StateManager : IDrawable
    {
        private readonly List<GameState> GameStates;

        private bool ExitState;
        public bool IsRunning { get; private set; }

        public StateManager()
        {
            ExitState = false;
            IsRunning = true;

            GameStates = new List<GameState>();
        }

        public void Initialize()
        {
        }

        public void Update(float dt)
        {
            GetCurrentInitializedState()?.Update(dt);
        }

        public void Draw(float dt)
        {
            GetCurrentInitializedState()?.Draw(dt);
        }

        public void PostFrame()
        {
            if (GetCurrentInitializedState() != null)
            {
                Global.EntityManager.PostFrame();
            }

            if (ExitState)
            {
                if (!GameStates.Empty())
                {
                    GetCurrentState().Destroy();
                    GameStates.RemoveLast();
                }

                if (GameStates.Empty())
                {
                    IsRunning = false;
                }

                ExitState = false;
            }

            var currentState = GetCurrentState();
            while (currentState != null && !currentState.IsInitialized)
            {
                currentState.Initialize();
                currentState = GetCurrentState();
            }

            GetCurrentInitializedState()?.PostFrame();
        }

        public GameState GetCurrentState()
        {
            return GameStates.Count > 0 ? GameStates[GameStates.Count - 1] : null;
        }

        private GameState GetCurrentInitializedState()
        {
            if (!GameStates.Empty())
            {
                int stateId = GameStates.Count - 1;
                while (stateId >= 0)
                {
                    var state = GameStates[stateId];
                    if (state.IsInitialized)
                    {
                        return state;
                    }
                    stateId--;
                }
            }

            return null;
        }

        public void Add(GameState state)
        {
            GameStates.Add(state);
        }

        public void ExitCurrentState()
        {
            ExitState = true;
        }

        // Should only be called from EntityManager
        public void RegisterEntity(Entity entity)
        {
            if (!entity.ContainsFlag(EntityFlag.Registered) || entity.EntityId < 0)
            {
                Logger.Warning($"Entity {entity.Name} was added but not registered: add entities through the EntityManager");
                return;
            }

            var state = GetCurrentInitializedState();
            if (state == null)
            {
                Logger.Error($"Register entity call when there was no initialized state: entity {entity.Name} will be lost");
                return;
            }

            state.RegisterEntity(entity);
        }

        // Should only be called from EntityManager
        public void UnregisterEntity(Entity entity)
        {
            if (!entity.ContainsFlag(EntityFlag.Registered) || entity.EntityId < 0)
            {
                Logger.Warning($"Entity {entity.Name} was not registered: remove entities through the EntityManager");
                return;
            }

            var state = GetCurrentInitializedState();
            if (state == null)
            {
                Logger.Error($"Unregister entity call when there was no initialized state: entity {entity.Name} will be lost");
                return;
            }

            state.UnregisterEntity(entity);
        }
    }
}
