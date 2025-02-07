using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    [CreateAssetMenu(
        fileName = "New RNG",
        menuName = "Jenga/RNG Asset",
        order = 1200
    )]
    public class RNGAsset : ScriptableObject {

        public enum RNGType { Xorshift32 }
        public RNGType type;

        [ALay.LayoutField, ALay.UsageToggle("useSeed")] 
        public uint seed = 0xAABB;
        
        [HideInInspector] 
        public bool useSeed = true;

        public uint GetUint() {
            if (!useSeed) {
                seed = (uint)Random.Range(int.MinValue, int.MaxValue);
                useSeed = true;
            }
            
            return RND.Xorshift32(ref seed);
        }

        public int GetInt() => (int)GetUint();
    }


}
