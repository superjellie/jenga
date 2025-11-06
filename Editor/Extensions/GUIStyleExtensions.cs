using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class GUIStyleExtensions {
        public static void 
        ChangeBackgroundColor(this GUIStyle style, Color newColor) {
            style.normal.background = new Texture2D(2, 2);
            style.normal.background.SetColor(newColor);
        } 
    }

    public static class Texture2DExtensions {

        public static void SetColor(this Texture2D tex, Color col) {
            
            Color[] pix = new Color[tex.width * tex.height];
            for (int i = 0; i < pix.Length; ++i) 
                pix[i] = col;

            tex.SetPixels(pix);
            tex.Apply();
        }

    }

    
}

