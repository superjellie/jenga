using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // Serializable generic generator (enumerator, iterator, collection, ...)
    // TODO: Write usage tutorial
    // Inherit from this class to write custom generators
    // You also need to instantiate generic (see GenericGeneratorsInstantiations.cs)
    // (inflated generic are not serialized by Unity)
    [System.Serializable]
    public class MonoGenerator<T> : ALay.ILayoutMe {

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
    public class MonoGeneratorReference<T> 
        : ISerializationCallbackReceiver, ALay.ILayoutMe {

        [System.NonSerialized] public MonoGenerator<T> generator;

        [SerializeReference] 
        [ALay.HideHeader]
        public object serializedValue;

        static ALay.FieldAttribute GetSelectorAttribute()
            => new ALay.TypeSelectorAttribute(typeof(MonoGenerator<T>))
                { path = "serializedValue" };

        public bool MoveNext(GameObject go) => generator.MoveNext(go);
        public T Current => generator.Current;
        public void Reset() => generator.Reset();

        public IEnumerable<T> GenerateWith(GameObject go) 
            => generator.GenerateWith(go);

        public void OnBeforeSerialize() {
            if (generator?.GetType().IsGenericType ?? true)
                serializedValue = null;
            else
                serializedValue = generator;
        }
        
        public void OnAfterDeserialize() 
            => generator = serializedValue as MonoGenerator<T>;

        public static implicit operator 
        MonoGeneratorReference<T>(MonoGenerator<T> generator) 
            => new MonoGeneratorReference<T>() { generator = generator };
    }
    
}