using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    // VI is condition-based state machine for different user interfaces
    // It should be coupled with transition components to make nice moving interface
    // VI is always in one and only state and always have DISABLED (id = 0) state
    // State ID should be less then 32
    public class VisualInterface : MonoBehaviour {

        [System.Serializable]
        public class StateDescription {
            public int id = 1;
    #if UNITY_EDITOR
            public string name;
    #endif
            [SerializeReference, TypeMenu]
            public MonoCondition condition = new ConstCondition();
        }

        // State is updated automaticaly, based on conditions
        [HideInInspector]
        public int state;

        public float delayBeforeStart = 0f;
        public bool immediatelyDisableOnEnable = true;

        // You should subscribe to event in transition handler components
        public delegate void StateChangeDelegate(
            int oldState, int newState, bool immediate
        );
        public event StateChangeDelegate onStateChange;

        // 0 is ALWAYS Disabled state
        // Other states are specified in editor
        public StateDescription[] stateDescriptions = { };

        // Fade, Move, etc can write to this variable to indicate that
        // interface can be safely disabled
        [System.NonSerialized]
        public bool canBeDisabled = false;
        public bool neverDisable = true;


        // Private
        void SetState(int newState, bool immediate) {
            canBeDisabled = newState == 0;

            if (state != newState && onStateChange != null)
                onStateChange(state, newState, immediate);
            
            state = newState;
        }

        void OnEnable() {
            if (immediatelyDisableOnEnable)
                SetState(0, true);
        }

        Coroutine crtn;
        void Awake() { 
            crtn = CoroutineMaster.main.StartCoroutine(UpdateState());
        }
        
        void OnDestroy() {
            if (crtn != null && CoroutineMaster.hasMain)
                CoroutineMaster.main.StopCoroutine(crtn);
        }

        IEnumerator UpdateState() {
            yield return new WaitForSeconds(delayBeforeStart);
        REPEAT:
            yield return null;
            
            // Skip checks if disabled by parent
            if (!gameObject.activeInHierarchy && gameObject.activeSelf)
                goto REPEAT;

            if (!neverDisable) {
                if (state == 0 && canBeDisabled)
                    gameObject.SetActive(false);

                // Enable if should not be disabled
                if (state != 0 && !gameObject.activeSelf)
                    gameObject.SetActive(true);
            }
            
            foreach (var desc in stateDescriptions) {
                if (desc.condition.Check()) { 
                    SetState(desc.id, false);
                    goto REPEAT;
                }
            }

            SetState(0, false);
            goto REPEAT;
        }
    }


    // Use this class in transition components to setup per-state data 
    // Sadly data will be null-initialized
    [System.Serializable]
    public class VisualStateData<T>{

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