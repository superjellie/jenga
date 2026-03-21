using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    [RequireTool(typeof(ShortcutsTool))]
    public class SnappingTool : SceneTool {

        public ShortcutsTool shortcuts => toolbox.GetActiveTool<ShortcutsTool>();

        [System.Serializable]
        [System.Flags]
        public enum Axis {
            None = 0b000,
            X    = 0b001,
            Y    = 0b010,
            Z    = 0b100,
            All  = ~0
        }

        public override string category => "Config";
        public override string title => "Snapping"; 
        public override int order => 900; 

        public Vector3 step = Vector3.one;
        public Vector3 offset = Vector3.zero;
        public Vector3 eulerStep = new Vector3(90f, 90f, 90f);
        public Vector3 eulerOffset = Vector3.zero;
        public Vector3 scaleStep   = Vector3.one * .1f;

        public Quaternion rotationStep => Quaternion.Euler(eulerStep);
        public Quaternion rotationOffset => Quaternion.Euler(eulerOffset);

        public Axis snapOnAxes = Axis.None;
        public Axis snapToAxes = Axis.All;
        public Axis snapRotationOnAxes = Axis.None;
        public Axis snapRotationToAxes = Axis.All;

        public Vector3 SnapPosition(Matrix4x4 matrix, Vector3 pos) {
            var wpos = matrix.inverse.MultiplyPoint(pos) - offset;
            var spos = SnapLocalOffset(wpos);
            return matrix.MultiplyPoint(spos + offset);
        }  

        public Vector3 SnapOffset(Matrix4x4 matrix, Vector3 offset) {
            var woff = matrix.inverse.MultiplyVector(offset);
            var soff = SnapLocalOffset(woff);
            return matrix.MultiplyVector(soff);
        }

        public Vector3 SnapDirection(Matrix4x4 matrix, Vector3 direction) {
            var wdir = matrix.inverse.MultiplyVector(direction).normalized;
            var rot = Quaternion.FromToRotation(Vector3.right, wdir);
            var srot = SnapLocalRotation(rot);
            return matrix.MultiplyVector(srot * Vector3.right).normalized;
        } 

        public Quaternion SnapRotation(Matrix4x4 matrix, Quaternion rot) {
            var lmat = matrix.inverse * Matrix4x4.Rotate(rot) * matrix;
            var lrot = lmat.rotation;
            var srot = SnapLocalRotation(lrot);
            var wmat = matrix * Matrix4x4.Rotate(srot) * matrix.inverse;
            return wmat.rotation;
        }

        public Matrix4x4 SnapMatrix(Matrix4x4 matrix, Matrix4x4 m) {
            var lm = matrix.inverse * m * matrix;
            var lrot = lm.rotation;
            var lpos = lm.GetPosition();
            var lscale = lm.lossyScale;

            var sm = Matrix4x4.TRS(
                SnapLocalOffset(lpos - offset) + offset,
                SnapLocalRotation(lrot),
                lscale
            );

            return matrix * sm * matrix.inverse;
        }

        public Matrix4x4 SnapInOwnRotation(Matrix4x4 matrix, Matrix4x4 m) {
            var lm = matrix.inverse * m * matrix;

            var lrot = lm.rotation;                
            var lpos = lm.GetPosition();                
            var lscale = lm.lossyScale;

            var ownPos = Quaternion.Inverse(lrot) * lpos;     
            var snapOwnPos = SnapLocalOffset(ownPos - offset) + offset;
            var srot = SnapLocalRotation(lrot);

            var sm = Matrix4x4.TRS(lrot * snapOwnPos, srot, lscale);
            return matrix * sm * matrix.inverse;
        }

        // public Vector3 MovePosition(Matrix4x4 matrix, Vector3 pos, Vector3 move) 
        //     => SnapPosition(matrix, pos + Vector3.Scale(offset, move));

        // public Vector3 MoveOffset(Matrix4x4 matrix, Vector3 off, Vector3 move) 
        //     => SnapOffset(matrix, off + Vector3.Scale(offset, move));

        // public Quaternion MoveRotation(
        //     Matrix4x4 matrix, Quaternion rot, Vector3 move
        // ) => SnapRotation(matrix, 
        //     Quaternion.Euler(Vector3.Scale(eulerStep, move))
        //     * rot
        // );

        // public Vector3 MoveEuler(
        //     Matrix4x4 matrix, Vector3 euler, Vector3 move
        // ) => SnapRotation(matrix, 
        //     Quaternion.Euler(Vector3.Scale(eulerStep, move))
        //     * Quaternion.Euler(euler)
        // ).eulerAngles;

        Vector3 SnapLocalOffset(Vector3 offset) {
            float x = (~snapToAxes).HasFlag(Axis.X) ? 0f 
                : snapOnAxes.HasFlag(Axis.X) ? Snapping.Snap(offset.x, step.x)
                : offset.x;
            float y = (~snapToAxes).HasFlag(Axis.Y) ? 0f 
                : snapOnAxes.HasFlag(Axis.Y) ? Snapping.Snap(offset.y, step.y)
                : offset.y;
            float z = (~snapToAxes).HasFlag(Axis.Z) ? 0f 
                : snapOnAxes.HasFlag(Axis.Z) ? Snapping.Snap(offset.z, step.z)
                : offset.z;
            return new Vector3(x, y, z);
        }

        Quaternion SnapLocalRotation(Quaternion rotation) 
            => Quaternion.Euler(SnapLocalEuler(rotation.eulerAngles));

        Vector3 SnapLocalEuler(Vector3 euler) {
            float x = (~snapRotationToAxes).HasFlag(Axis.X) ? 0f 
                : snapRotationOnAxes.HasFlag(Axis.X) 
                    ? Snapping.Snap(euler.x, eulerStep.x)
                : euler.x;
            float y = (~snapRotationToAxes).HasFlag(Axis.Y) ? 0f 
                : snapRotationOnAxes.HasFlag(Axis.Y) 
                    ? Snapping.Snap(euler.y, eulerStep.y)
                : euler.y;
            float z = (~snapRotationToAxes).HasFlag(Axis.Z) ? 0f 
                : snapRotationOnAxes.HasFlag(Axis.Z) 
                    ? Snapping.Snap(euler.z, eulerStep.z)
                : euler.z;
            return new Vector3(x, y, z);
        }

        public override void OnUpdate(EditorWindow win) {
            
            shortcuts.SetShortcut(
                "SnappingTool/IncreaseSnapStep", this.GetType(),
                EventModifiers.None, KeyCode.I, KeyCode.Equals,
                "Increase snapping step", () => MoveStep(1f)
            ); 

            shortcuts.SetShortcut(
                "SnappingTool/DecreaseSnapStep", this.GetType(),
                EventModifiers.None, KeyCode.I, KeyCode.Minus,
                "Decrease snapping step", () => MoveStep(-1f)
            );

            shortcuts.SetShortcut(
                "SnappingTool/IncreaseEulerSnapStep", this.GetType(),
                EventModifiers.Shift, KeyCode.I, KeyCode.Equals,
                "Increase rotation snapping step", () => MoveEulerStep(1f)
            );

            shortcuts.SetShortcut(
                "SnappingTool/DecreaseEulerSnapStep", this.GetType(),
                EventModifiers.Shift, KeyCode.I, KeyCode.Minus,
                "Decrease rotation snapping step", () => MoveEulerStep(-1f)
            );

            shortcuts.SetShortcut(
                "SnappingTool/ToggleSnapX", this.GetType(),
                EventModifiers.None, KeyCode.I, KeyCode.X,
                "Toggle snapping on X", () => ToggleSnap(Axis.X)
            );

            shortcuts.SetShortcut(
                "SnappingTool/ToggleSnapY", this.GetType(),
                EventModifiers.None, KeyCode.I, KeyCode.Y,
                "Toggle snapping on Y", () => ToggleSnap(Axis.Y)
            );

            shortcuts.SetShortcut(
                "SnappingTool/ToggleSnapZ", this.GetType(),
                EventModifiers.None, KeyCode.I, KeyCode.Z,
                "Toggle snapping on Z", () => ToggleSnap(Axis.Z)
            );

            shortcuts.SetShortcut(
                "SnappingTool/ToggleRotationSnapX", this.GetType(),
                EventModifiers.Shift, KeyCode.I, KeyCode.X,
                "Toggle rotation snapping on X", () => ToggleRotationSnap(Axis.X)
            );

            shortcuts.SetShortcut(
                "SnappingTool/ToggleRotationSnapY", this.GetType(),
                EventModifiers.Shift, KeyCode.I, KeyCode.Y,
                "Toggle rotation snapping on Y", () => ToggleRotationSnap(Axis.Y)
            );

            shortcuts.SetShortcut(
                "SnappingTool/ToggleRotationSnapZ", this.GetType(),
                EventModifiers.Shift, KeyCode.I, KeyCode.Z,
                "Toggle rotation snapping on Z", () => ToggleRotationSnap(Axis.Z)
            );
        }

        public void MoveStep(float amount) {
            var x = step.x;

            var pos 
                = x <= .1f  ? 0f
                : x <= .2f  ? 1f
                : x <= .25f ? 2f
                : x <= .34f ? 3f
                : x <= .5f  ? 4f
                : 5f;

            pos += amount;

            var nx 
                = pos <= 0f ? .1f
                : pos <= 1f ? .2f
                : pos <= 2f ? .25f
                : pos <= 3f ? 1f/3f
                : pos <= 4f ? .5f
                : pos - 4f;

            Undo.RecordObject(this, "Change snapping step");
            step = new Vector3(nx, nx, nx);
        }

        public void MoveEulerStep(float amount) {
            var x = eulerStep.x;

            var pos 
                = x <= 10f    ? 0f
                : x <= 27.5f  ? 1f
                : x <= 45f    ? 2f
                : 3f;

            pos += amount;

            var nx 
                = pos <= 0f ? 10f
                : pos <= 1f ? 27.5f
                : pos <= 2f ? 45f
                : 90f;

            Undo.RecordObject(this, "Change rotation snapping step");
            eulerStep = new Vector3(nx, nx, nx);
        }

        public void ToggleSnap(Axis axis) {
            Undo.RecordObject(this, "Toggle snapping");
            snapOnAxes ^= axis;
        }

        public void ToggleRotationSnap(Axis axis) {
            Undo.RecordObject(this, "Toggle rotation snapping");
            snapRotationOnAxes ^= axis;
        }

    }
}