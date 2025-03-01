using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [System.Serializable, ALay.HideHeader]
    public class SpriteAnimation /*: ALay.ILayoutMe */{
        [ALay.BeginRowGroup, ALay.UseRootLabel, ALay.FlexGrow(1f)]
        public Texture2D texture;

        [ALay.EndGroup, ALay.HideLabel, ALay.MaxWidth(50f), ALay.MinWidth(50f)]
        public float fps = 20f;
        
        [HideInInspector]
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
    }

}