using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    public class NothingGenerator<T> : MonoGenerator<T> { }

    public class ItemGenerator<T> : MonoGenerator<T>, ALay.ILayoutMe {

        public T item;

        [HideInInspector] public bool isDone = false;
         
        public override bool MoveNext(GameObject go) 
            => isDone ? false : isDone = true;

        public override T Current => item;
        public override void Reset() => isDone = false;
    }

    public class AssetGenerator<T> : MonoGenerator<T>, ALay.ILayoutMe {

        public MonoGeneratorAsset<T> asset;
        
        MonoGeneratorAsset<T> instance;

        public override bool MoveNext(GameObject go) {
            if (instance == null)
                instance = ScriptableObject.Instantiate(asset);

            return instance.generator.MoveNext(go);
        }

        public override T Current => 
            instance != null 
                ? instance.generator.Current
                : default(T);

        public override void Reset() {
            if (instance != null) 
                instance.generator.Reset();
        }
    }

    public class RepeatGenerator<T> : MonoGenerator<T>, ALay.ILayoutMe {
        public int times = 5;

        [FormerlySerializedAs("generator")]
        public MonoGeneratorReference<T> item;

        [HideInInspector] public int index = 0;

        public override bool MoveNext(GameObject go) {
            for (; index < times && !item.MoveNext(go); ++index) 
                item.Reset();

            return index < times;
        }

        public override T Current => item.Current;
        public override void Reset() { index = 0; item.Reset(); }
    }


    public class SequenceGenerator<T> : MonoGenerator<T> {

        [ALay.ListView(showFoldoutHeader = false)] 
        public MonoGeneratorReference<T>[] items = { };

        [HideInInspector] public int index = 0;

        public override bool MoveNext(GameObject go) {
            for (; index < items.Length && !items[index].MoveNext(go); ++index) 
                items[index].Reset();

            return index < items.Length;
        }

        public override T Current => items[index].Current;
        public override void Reset() { 
            if (index < items.Length)
                items[index].Reset();

            index = 0;
        }
    } 

    public class TakeRandomGenerator<T> : MonoGenerator<T>, ALay.ILayoutMe {

        public RNGAsset rng; 

        [Tooltip("Uses one random generator")]
        [ALay.ListView(showFoldoutHeader = false)]
        public MonoGeneratorReference<T>[] items = { };

        [HideInInspector] public int randomIndex = -1; 

        public override bool MoveNext(GameObject go) {
            if (items.Length == 0) return false;

            if (randomIndex < 0) {
                randomIndex = Mathx.Mod(rng?.GetInt() ?? 0, items.Length);
            }
        
            return items[randomIndex].MoveNext(go);
        }

        public override T Current => items[randomIndex].Current;

        public override void Reset() {
            if (randomIndex >= 0)
                items[randomIndex].Reset();

            randomIndex = -1;
        }
    } 

    public class RepeatForeverGenerator<T> : MonoGenerator<T>, ALay.ILayoutMe {

        public MonoGeneratorReference<T> item;

        public override bool MoveNext(GameObject go) {
            if (item.MoveNext(go)) return true;
            
            item.Reset(); 
            return item.MoveNext(go);
        }

        public override T Current => item.Current;
        public override void Reset() => item.Reset();
    }

    public class GenerateWhileGenerator<T> : MonoGenerator<T>, ALay.ILayoutMe {

        [SerializeReference, TypeMenu]
        public MonoCondition condition = new ConstCondition();

        [FormerlySerializedAs("generator")]
        public MonoGeneratorReference<T> item;

        public override bool MoveNext(GameObject go) {
            if (!condition.Check()) return false; 
            return item.MoveNext(go);
        }

        public override T Current => item.Current;
        public override void Reset() => item.Reset();
    }

    public class GenerateUntilGenerator<T> : MonoGenerator<T>, ALay.ILayoutMe {
        
        [SerializeReference, TypeMenu]
        public MonoCondition condition = new ConstCondition();

        [FormerlySerializedAs("generator")]
        public MonoGeneratorReference<T> item;

        public override bool MoveNext(GameObject go) {
            if (condition.Check()) return false; 
            return item.MoveNext(go);
        }

        public override T Current => item.Current;
        public override void Reset() => item.Reset();
    }

    public class OptionalGenerator<T> : MonoGenerator<T>, ALay.ILayoutMe {

        [SerializeReference, TypeMenu]
        public MonoCondition condition;

        [FormerlySerializedAs("ifTrue")]
        public MonoGeneratorReference<T> item;

        [HideInInspector] public int wasTrue = -1;

        public override bool MoveNext(GameObject go) {
            if (wasTrue == -1)
                wasTrue = condition.Check() ? 1 : 0;

            return wasTrue == 1 ? item.MoveNext(go) : false;
        }

        public override T Current => 
            wasTrue == 1 ? item.Current : default(T);

        public override void Reset() { 
            if (wasTrue == 1)
                item.Reset();
            wasTrue = -1;
        }
    }
}