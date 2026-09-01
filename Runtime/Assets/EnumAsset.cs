using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    [CreateAssetMenu(
        menuName = "Jenga/Enum Asset",
        fileName = "EnumAsset"
    )]
    public class EnumAsset : ScriptableObject {
        public List<string> values = new();
    }

    [System.Serializable] 
    public class EnumValue {
        public EnumAsset asset;
        public string value;
    } 

}