using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static partial class JengaEditorGUI {

        static int forceCollapsedCount = 0;

        public static void BeginForceCollapsedGroup() 
            => forceCollapsedCount++;

        public static void EndForceCollapsedGroup()
            => forceCollapsedCount--;

        public static bool ShouldShowChildren() => forceCollapsedCount == 0;
    }
}
