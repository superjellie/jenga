using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // TODO: Editor
    [System.Serializable]
    public class SpriteAnimation {
        public Texture2D texture;

        public float fps = 20f;
        
        public Sprite[] sprites = { };

        public GameObject Play(Vector3 pos, Quaternion rot, Vector3 scale) {
            
            var go = new GameObject(
                "AnimEvaluator", 
                typeof(SpriteRenderer),
                typeof(CoroutineHolderBehaviour)
            );

            go.transform.localScale = scale;
            go.transform.rotation = rot;
            go.transform.position = pos;

            var renderer = go.GetComponent<SpriteRenderer>();
            var holder = go.GetComponent<CoroutineHolderBehaviour>();

            IEnumerator Play() {
                foreach (var sprite in sprites) { 
                    renderer.sprite = sprite; 
                    yield return new WaitForSeconds(1f / fps);
                }

                GameObject.Destroy(go);
            }

            holder.StartCoroutine(Play());
            return go;
        }

        public GameObject PlayAttached(Transform transform) {
            
            var go = new GameObject(
                "AnimEvaluator", 
                typeof(SpriteRenderer),
                typeof(CoroutineHolderBehaviour)
            );

            go.transform.SetParent(transform, false);

            var renderer = go.GetComponent<SpriteRenderer>();
            var holder = go.GetComponent<CoroutineHolderBehaviour>();

            IEnumerator Play() {
                foreach (var sprite in sprites) { 
                    renderer.sprite = sprite; 
                    yield return new WaitForSeconds(1f / fps);
                }

                GameObject.Destroy(go);
            }

            holder.StartCoroutine(Play());
            return go;
        }
    }

}