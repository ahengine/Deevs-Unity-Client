﻿using System.Collections.Generic;

namespace Patterns.FSMMonoBase
{
    public class FiniteStateMachine<T>
    {
        protected Dictionary<T, State<T>> states;

        protected State<T> currentState;

        public FiniteStateMachine() => states = new Dictionary<T, State<T>>();

        public void Add(State<T> state) => states.Add(state.ID, state);

        public void Add(T stateID, State<T> state) => states.Add(stateID, state);

        public State<T> GetState(T stateID)
        {
            if(states.ContainsKey(stateID))
                return states[stateID];
            return null;
        }

        public void SetCurrentState(T stateID)
        {
            State<T> state = states[stateID];
            SetCurrentState(state);
        }

        public State<T> GetCurrentState => currentState;

        public void SetCurrentState(State<T> state)
        {
            if (currentState == state)
                return;

            if (currentState != null)
                currentState.Exit();

            currentState = state;

            if (currentState != null)
                currentState.Enter();
        }

        public void Update()
        {
            if (currentState != null)
                currentState.Updates();
        }

        public void LateUpdate()
        {
            if (currentState != null)
                currentState.LateUpdates();
        }

        public void FixedUpdate()
        {
            if (currentState != null)
                currentState.FixedUpdates();
        }
    }
}
