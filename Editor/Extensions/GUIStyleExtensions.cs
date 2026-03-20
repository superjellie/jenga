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

        public static Texture2D MultiplyColor(this Texture2D tex, Color col) {
            
            Color[] pix = new Color[tex.width * tex.height];

            for (int y = 0; y < tex.height; ++y)
            for (int x = 0; x < tex.width; ++x) 
                pix[x + y * tex.width] = tex.GetPixel(x, y) * col;

            var result = new Texture2D(tex.width, tex.height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        } 

    }

    
}

