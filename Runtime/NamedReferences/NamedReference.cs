using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public interface INamedReferenceUsageStrategy<T> {
        T Aquire(T original);
        void Release(T aquired);
    }

    [System.Serializable]
    public class BaseNamedReference<T, UsageStrategy>
        where UsageStrategy : INamedReferenceUsageStrategy<T> { 

        public NamedReferenceRegistry<T, UsageStrategy> registry;
        public int id = 0; 

        public T Aquire() => registry.Aquire(id);
        public void Release(T item) => registry.Release(id, item);
    }

    [System.Serializable]
    public class NamedReference<T> 
        : BaseNamedReference<T, NoUsageStartegy<T>> { }


    public class NamedReferenceRegistry<T, UsageStrategy> 
        : ScriptableObject
        where UsageStrategy : INamedReferenceUsageStrategy<T> {

        [System.Serializable]
        public struct RefData : ALay.ILayoutMe {
            [ALay.StartLine, ALay.HideLabel, ALay.MaxWidth(100f)] 
            public T reference;
            [ALay.EndLine, ALay.HideLabel, ALay.FlexGrow(1f)]  
            public string name;
            
            [ALay.HideLabel]
            public UsageStrategy strategy;
        }

        // public int nextID = 1; 
        public string categoryName = "Default";

        [ALay.LayoutField, ALay.HideLabel]
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