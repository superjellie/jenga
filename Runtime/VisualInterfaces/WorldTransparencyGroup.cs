using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // Works like CanvasGroup but for scene renderers
    // (SpriteRenderer, LineRenderer)
    public class WorldTransparencyGroup : MonoBehaviour {

        public float alpha = 1f;
        Renderer[] renderers;
        float[][] initialAlphas;

        void OnEnable() {
            renderers = GetComponentsInChildren<Renderer>(true);
            initialAlphas = new float[renderers.Length][];
            for (int i = 0; i < renderers.Length; ++i) 
                initialAlphas[i] = GetAlphas(renderers[i]);
        }

        void OnDisable() {
            if (renderers == null) return;
            for (int i = 0; i < renderers.Length; ++i) 
                SetAlphas(renderers[i], 1f, initialAlphas[i]);
        }

        void Update() {
            for (int i = 0; i < renderers.Length; ++i) 
                SetAlphas(renderers[i], alpha, initialAlphas[i]);
        }

        float[] GetAlphas(Renderer rd) {
            if (rd is SpriteRenderer sr)
                return new[] { sr.color.a };
            else if (rd is LineRenderer lr) 
                return new[] { lr.startColor.a, lr.endColor.a };

            return System.Array.Empty<float>();
        }

        void SetAlphas(Renderer rd, float mult, float[] alphas) {
            if (rd is SpriteRenderer sr && alphas.Length >= 1) {
                var c = sr.color;
                sr.color = new(c.r, c.g, c.b, alphas[0] * mult);
            } else if (rd is LineRenderer lr && alphas.Length >= 2) {
                var c0 = lr.startColor;
                var c1 = lr.endColor;
                lr.startColor = new(c0.r, c0.g, c0.b, alphas[0] * mult);
                lr.endColor   = new(c1.r, c1.g, c1.b, alphas[1] * mult);
            }
        }
    }
}
