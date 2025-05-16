using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {

    // Serializable generic generator (enumerator, iterator, collection, ...)
    // TODO: Write usage tutorial
    // Inherit from this class to write custom generators
    // You also need to instantiate generic (see GenericGeneratorsInstantiations.cs)
    // (inflated generic are not serialized by Unity)
    [System.Serializable, ALay.HideHeader]
    public class MonoGenerator<T> : ALay.ILayoutMe {

        // Used to match together references
        [HideInInspector] public string refName;
        
        public virtual bool MoveNext(GameObject go) => false;
        public virtual T Current => default(T);
        public virtual void Reset() { }

        public IEnumerable<T> GenerateWith(GameObject go) {
            Reset();
            while (MoveNext(go))
                yield return Current;
        }
    }


    // Use this struct in your fields instead
    // Does some serialization & ui magic
    [System.Serializable] 
    [ALay.DelayAttribute("GetSelectorAttribute", inClass = true)]
    [ALay.MatchReferences]
    public class MonoGeneratorReference<T> : ALay.ILayoutMe {


        [SerializeReference] 
        [FormerlySerializedAs("serializedValue")]
        public object value;
        
        public MonoGenerator<T> generator => value as MonoGenerator<T>;

        static ALay.FieldAttribute GetSelectorAttribute()
            => new ALay.TypeSelectorAttribute(typeof(MonoGenerator<T>))
                { path = "value" };

        public bool MoveNext(GameObject go) => generator.MoveNext(go);
        public T Current => generator.Current;
        public void Reset() => generator.Reset();

        public IEnumerable<T> GenerateWith(GameObject go) 
            => generator.GenerateWith(go);

        public static implicit operator 
        MonoGeneratorReference<T>(MonoGenerator<T> generator) 
            => new MonoGeneratorReference<T>() { value = generator };
    }
    
}