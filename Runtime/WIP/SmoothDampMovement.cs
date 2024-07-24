using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class SmoothDampMovement : MonoBehaviour {

        // public UpdateMethod updateMethod = UpdateMethod.Update;
        // public TransformMask transformMask = TransformMask.Global;
        // [Range(0f, 5f)] public float snapTime = .2f;


        // // [Gizmoj.DrawTarget(Gizmoj.DefaultSelectedStyleID)]
        // Vector3 positionTarget;
        // Vector3 lookTarget;
        // Vector3 lookUp = Vector3.up;
        // Vector3 scaleTarget;
        // Vector3 velocity = Vector3.zero; 

        // void Awake() {
        //     this.positionTarget = transform.position;
        //     this.lookTarget     = transform.forward + this.positionTarget;
        //     this.scaleTarget    = transform.localScale;
        // }

        // [Signal] void SnapTo(Vector3 pos) => this.positionTarget = pos;
        // [Signal] void ScaleTo(Vector3 scale) => this.scaleTarget = scale;
        // [Signal] void LookAt(Vector3 target, Vector3 up) {
        //     this.lookTarget = target;
        //     this.lookUp = up;
        // }

        // [Signal] void AnyUpdate(UpdateMethod method, float deltaTime) {
        //     if (method != this.updateMethod) return;
        //     this.transform.position = Vector3.SmoothDamp(
        //         this.transform.position, this.target, 
        //         ref this.velocity, this.snapTime
        //     );
        // }

    }
}
