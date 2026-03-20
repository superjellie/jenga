using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    [System.Serializable]
    public class PersistentProperty {
        public GlobalObjectId objectID;
        public string propertyPath;

        SerializedObject cachedObject;
        SerializedProperty cachedProperty;

        public PersistentProperty(SerializedProperty prop) {
            objectID = GlobalObjectId
                .GetGlobalObjectIdSlow(prop.serializedObject.targetObject);
            propertyPath = prop.propertyPath;
        }

        public SerializedProperty GetProperty() {
            if (cachedObject != null && cachedObject.targetObject != null
                && cachedProperty != null)
                return cachedProperty;

            var o = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(objectID);
            if (o == null) return null;

            cachedObject = new SerializedObject(o);
            cachedProperty = cachedObject.FindProperty(propertyPath);

            return cachedProperty;
        }
    }
}
