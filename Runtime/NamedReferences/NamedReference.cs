using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public interface INamedReferenceUsageStrategy<T> {
        T Aquire(T original);
        void Release(T aquired);
    }

    [System.Serializable, ALay.Inline]
    public class BaseNamedReference<T, UsageStrategy> : ALay.ILayoutMe
        where UsageStrategy : INamedReferenceUsageStrategy<T> { 

        [ALay.Style(width = 100f)]
        public NamedReferenceRegistry<T, UsageStrategy> registry;
        
        [ALay.Style(flexGrow = 1f)]
        [ALay.Options("MakeIDOptions")]
        public int id = 0; 

        public T Aquire() => registry.Aquire(id);
        public void Release(T item) => registry.Release(id, item);
    
    #if UNITY_EDITOR
        static void MakeIDOptions(
            UnityEditor.SerializedProperty self, ALay.OptionsAttribute.Map map
        ) {
            var reg = self.FindPropertyRelative("registry");

            if (reg.objectReferenceValue == null) 
                { map.SetError("No Registry"); return; }

            var refs = reg.FindPropertyRelative("references")
                .FindPropertyRelative("pairs");

            for (int i = 0; i < refs.arraySize; ++i) {
                var pair = refs.GetArrayElementAtIndex(i);
                var id   = pair.FindPropertyRelative("key"); 
                var name = pair.FindPropertyRelative("value")
                    .FindPropertyRelative("name"); 

                map.Add(name.stringValue, id.intValue);
            }

            map.UpdateWith(reg.propertyPath);
        }
    #endif 
    }

    [System.Serializable]
    public class NamedReference<T> 
        : BaseNamedReference<T, NoUsageStartegy<T>> { }


    public class NamedReferenceRegistry<T, UsageStrategy> 
        : ScriptableObject
        where UsageStrategy : INamedReferenceUsageStrategy<T> {

        [System.Serializable, ALay.Inline]
        public struct RefData : ALay.ILayoutMe {
            [ALay.Style(width = 100f)] 
            public T reference;
            [ALay.Style(flexGrow = 1f)]  
            public string name;
            [ALay.Style(width = 100f)] 
            public UsageStrategy strategy;
        }

        // [ALay.LayoutMe]
        public ADT.Map<int, RefData> references = new();

        public bool HasID(int id) => references.ContainsKey(id);
        public int CountItems() => references.Count;

        public T Aquire(int id) 
            => id > 0 && HasID(id) 
                ? references[id].strategy.Aquire(references[id].reference) 
                : default(T);

        public void Release(int id, T item) {
            if (id > 0 && HasID(id)) 
                references[id].strategy.Release(item); 
        }

        public T Get(int id) 
            => id > 0 && HasID(id) ? references[id].reference : default(T);
        
        public string GetName(int id) 
            => id > 0 
                ? HasID(id) ? references[id].name : $"[Lost: {id}]"
                : id == 0   ? "[None]" : $"[Invalid: {id}]";
    }

    public class NoUsageStartegy<T> : INamedReferenceUsageStrategy<T> {
        public T Aquire(T original) => original;
        public void Release(T aquiered) { }
    }
}