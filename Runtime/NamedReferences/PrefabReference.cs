using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    [System.Serializable]
    public class PrefabReference<T> 
        : BaseNamedReference<T, PrefabUsageStrategy<T>>
        where T : Component { }

    [System.Serializable, ALay.HideHeader]
    public class PrefabUsageStrategy<T> 
        : INamedReferenceUsageStrategy<T>, ALay.ILayoutMe
        where T : Component {

        public enum Strategy { Activate, Instantiate, InstantiateAndActivate };

        // [ALay.Label("Usage strategy")]
        public Strategy usageStrategy;

        public T Aquire(T item) {
            switch (usageStrategy) {
            case Strategy.Activate:
                item.gameObject.SetActive(true);
                return item;

            case Strategy.Instantiate:
                var comp1 = Object.Instantiate(item);
                return comp1;

            case Strategy.InstantiateAndActivate:
                var comp2 = Object.Instantiate(item);
                comp2.gameObject.SetActive(true);
                return comp2;
            }

            return null;
        }

        public void Release(T item) {
            switch (usageStrategy) {
            case Strategy.Activate:
                item.gameObject.SetActive(false);
                break;

            case Strategy.Instantiate:
            case Strategy.InstantiateAndActivate:
                GameObject.Destroy(item.gameObject);
                break;
            }
        }
    }

    public class PrefabRegistry<T> 
        : NamedReferenceRegistry<T, PrefabUsageStrategy<T>>
        where T : Component { }
}
