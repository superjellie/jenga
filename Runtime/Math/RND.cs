using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    public delegate uint  RNG();
    public delegate int   RNGi();
    public delegate float RNGf();
    // public delegate uint64_t  RNGu64();
    // public delegate uint128_t RNGu128();

    public static class RND {
    
        public static uint Xorshift32(ref uint state) {
            uint x = state;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            return state = x;
        }

    }
}
