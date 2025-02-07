using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    // VI is condition-based state machine for different user interfaces
    // It should be coupled with transition components to make nice moving interface
    // VI is always in one and only state and always have DISABLED (id = 0) state
    // State ID should be less then 32
    public class VisualInterface : MonoBehaviour {

        [System.Serializable]
        public struct StateDescription {
    #if UNITY_EDITOR
            public string name;
    #endif
            public int id;
            public MonoConditionReference condition;
        }

        // State is updated automaticaly, based on conditions
        public int state;

        // You should subscribe to event in transition handler components
        public delegate void StateChangeDelegate(
            int oldState, int newState, bool immediate
        );
        public event StateChangeDelegate onStateChange;

        // 0 is ALWAYS Disabled state
        // Other states are specified in editor
        public StateDescription[] stateDescriptions = { };



        // Private
        void Update() => UpdateState();

        void SetState(int newState, bool immediate) {
            if (state != newState)
                onStateChange(state, newState, immediate);
            state = newState;
        }

        void UpdateState() {
            foreach (var desc in stateDescriptions) {
                if (desc.condition.Check(gameObject)) { 
                    SetState(desc.id, false);
                    return;
                }
            }

            SetState(0, false);
        }
    }


    // Use this class in transition components to setup per-state data 
    // Sadly data will be null-initialized
    [System.Serializable]
    public class VisualStateData<T> {

        [System.Serializable]
        public struct StateData {
            public int mask;
            public T data;
        }

        public StateData[] matchers = { };
        public T fallback;

        public T Get(int state) {
            foreach (var match in matchers)
                if ((match.mask & (1 << state)) != 0) 
                    return match.data;
            return fallback;
        }
    }

    // Use this class in transition components to setup per-transition data 
    // Sadly data will be null-initialized
    [System.Serializable]
    public class VisualTransitionData<T> {

        [System.Serializable]
        public struct TransitionData {
            public int fromMask;
            public int toMask;
            public T data;
        }

        public TransitionData[] matchers = { };
        public T fallback;

        public T Get(int fromState, int toState) {
            foreach (var match in matchers)
                if ((match.fromMask & (1 << fromState)) != 0
                    && (match.toMask & (1 << toState)) != 0) 
                    return match.data;
            return fallback;
        }
    }
}