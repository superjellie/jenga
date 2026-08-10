using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class CoroutineMaster : MonoBehaviour {

        static CoroutineMaster main_;

        public static CoroutineMaster main =>
            main_ != null ? main_ : main_ = SpawnMaster();
        public static bool hasMain => main_ != null;

        [System.Obsolete("Use hasMain")]
        public static CoroutineMaster mainDoNotInstantiate;

        public static CoroutineMaster SpawnMaster() {
            var go = new GameObject(
                "CoroutineMaster",
                typeof(CoroutineMaster)
            );
            DontDestroyOnLoad(go);
            return go.GetComponent<CoroutineMaster>();
        }


        public static CoroutineMaster GetOnObject(GameObject go) {
            var master = go.GetComponent<CoroutineMaster>();

            if (master != null)
                return master;

            return go.AddComponent<CoroutineMaster>();
        }

        public static IEnumerator
        StartOnObject(GameObject go, IEnumerator crtn) {
            var master = GetOnObject(go);
            master.StartCoroutine(crtn);
            return crtn;
        }

        public void PlayCoroutineAndDestroy(IEnumerator crtn) {

            IEnumerator Play() {
                yield return crtn;

                if (this != null)
                    Destroy(gameObject);
            }

            StartCoroutine(Play());
        }


        // General Coroutine Utils
        public static IEnumerator
        RunTogether(params IEnumerator[] enums) {
            try {
                var crtns = new Coroutine[enums.Length];

                for (int i = 0; i < enums.Length; ++i)
                    crtns[i] = main.StartCoroutine(enums[i]);

                foreach (var crtn in crtns)
                    yield return crtn;

            } finally {
                for (int i = 0; i < enums.Length; ++i)
                    StopAndFinalize(enums[i]);
            }
        }

        public static IEnumerator
        RunInSequence(params IEnumerator[] enums) {
            foreach (var e in enums)
                yield return e;
        }

        public static IEnumerator
        RunWithMain(IEnumerator main, IEnumerator side) {
            try {
                Start(side);
                yield return main;
            } finally {
                StopAndFinalize(side);
            }
        }

        public static IEnumerator
        RunWhile(IEnumerator main, System.Func<bool> condition) {
            bool mainDone = false;
            IEnumerator crtn = null;

            IEnumerator PlayMain() {
                yield return main;
                mainDone = true;
            }

            try {
                crtn = Start(PlayMain());
                while (condition() && !mainDone) yield return null;

            } finally {
                if (!mainDone)
                    StopAndFinalize(crtn);
            }
        }

        public static IEnumerator Start(IEnumerator crtn) {
            main.StartCoroutine(crtn);
            return crtn;
        }

        // Only use with coroutines started on main/using Start(crtn)
        public static void StopAndFinalize(IEnumerator crtn) {
            if (hasMain)
                main.StopCoroutine(crtn);
            Finalize(crtn);
        }

        // You should always finalize your coroutines
        // You can finilize any stopped coroutine using this
        // It will run all code that is inside finally blocks in IEnumerators
        public static void Finalize(IEnumerator crtn) {
            while (crtn != null) {
                if (crtn is System.IDisposable disposable)
                    disposable.Dispose();
                crtn = crtn.Current as IEnumerator;
            }
        }

        // 
        public static IEnumerator CatchException<E>(
            IEnumerator ie,
            System.Action<E> handle,
            System.Action noException = null
        ) where E : System.Exception {
        REPEAT:
            try { if (!ie.MoveNext()) goto DONE; }
            catch(E e) { handle(e); goto FAIL; }

            if (ie.Current is IEnumerator iecur) {
                E e = null;
                yield return CatchException<E>(iecur, x => e = x);
                if (e != null) { handle(e); goto FAIL; }
            }
            else
                yield return ie.Current;


            goto REPEAT;
        
        DONE:
            if (noException != null)
                noException();
            Finalize(ie);
            yield break;

        FAIL:
            Finalize(ie);
            yield break;
        }
    }
}
