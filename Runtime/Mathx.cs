using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    public class Vector4Int {
        public int x; public int y; public int z; public int w;
        public Vector4Int(int x, int y, int z, int w) {
            this.x = x; this.y = y; this.z = z; this.w = w;
        }
    }

    public static partial class Mathx {

        public const float SMALL = .01f;
        public const float TINY  = .0001f;
        public const float INF   = float.PositiveInfinity;
        // public const float EPS   = float.Epsilon;
        public const float PI    = Mathf.PI;
        public const float TAU   = 2f * Mathf.PI;
        public const float E     = 2.7182818284590452353602874713526624977572f;

        // public static Vector4Int Int4(int x, int y, int z, int w)
        //     => new Vector4Int(x, y, z, w);

        // Interpolation
        public static float Lerp(float a, float b, float t) 
            => (1f - t) * a + t * b;
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) 
            => (1f - t) * a + t * b;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) 
            => (1f - t) * a + t * b;
        public static Vector4 Lerp(Vector4 a, Vector4 b, float t) 
            => (1f - t) * a + t * b;
        public static Quaternion Lerp(Quaternion a, Quaternion b, float t) 
            => Quaternion.LerpUnclamped(a, b, t);
        public static Quaternion Slerp(Quaternion a, Quaternion b, float t) 
            => Quaternion.SlerpUnclamped(a, b, t);
        public static Vector3 Slerp(Vector3 a, Vector3 b, float t) 
            => Vector3.SlerpUnclamped(a, b, t);

        public static float LerpClamped(float a, float b, float t) 
            => Lerp(a, b, Clamp(t, 0f, 1f));
        public static Vector2 LerpClamped(Vector2 a, Vector2 b, float t) 
            => Lerp(a, b, Clamp(t, 0f, 1f));
        public static Vector3 LerpClamped(Vector3 a, Vector3 b, float t) 
            => Lerp(a, b, Clamp(t, 0f, 1f));
        public static Vector4 LerpClamped(Vector4 a, Vector4 b, float t) 
            => Lerp(a, b, Clamp(t, 0f, 1f));
        public static 
        Quaternion LerpClamped(Quaternion a, Quaternion b, float t) 
            => Lerp(a, b, Clamp(t, 0f, 1f));
        public static 
        Quaternion SlerpClamped(Quaternion a, Quaternion b, float t) 
            => Lerp(a, b, Clamp(t, 0f, 1f));
        public static Vector3 SlerpClamped(Vector3 a, Vector3 b, float t) 
            => Lerp(a, b, Clamp(t, 0f, 1f));

        // Min, Max, Sum 
        public static float Min(params float[] fs) => AQRY.Min(fs);
        public static float Max(params float[] fs) => AQRY.Max(fs);
        public static float Sum(params float[] fs) => AQRY.Sum(fs);
        public static int Min(params int[] fs) 
            => AQRY.MinBy<int>(fs, (x, i) => x);
        public static int Max(params int[] fs) 
            => AQRY.MaxBy<int>(fs, (x, i) => x);

        // Clamping
        public static float Clamp(float x, float min, float max)
            => x < min ? min : x > max ? max : x;
        public static Vector2 Clamp(Vector2 x, Vector2 min, Vector2 max)
            => new(Clamp(x.x, min.x, max.x), Clamp(x.y, min.y, max.y));
        public static Vector3 Clamp(Vector3 x, Vector3 min, Vector3 max)
            => new(
                Clamp(x.x, min.x, max.x), 
                Clamp(x.y, min.y, max.y),
                Clamp(x.z, min.z, max.z)
            ); 
        public static Vector4 Clamp(Vector4 x, Vector4 min, Vector4 max)
            => new(
                Clamp(x.x, min.x, max.x), 
                Clamp(x.y, min.y, max.y),
                Clamp(x.z, min.z, max.z),
                Clamp(x.w, min.w, max.w)
            );
        public static int Clamp(int x, int min, int max)
            => x < min ? min : x > max ? max : x;
        public static 
        Vector2Int Clamp(Vector2Int x, Vector2Int min, Vector2Int max)
            => new(
                Clamp(x.x, min.x, max.x), Clamp(x.y, min.y, max.y)
            );
        public static 
        Vector3Int Clamp(Vector3Int x, Vector3Int min, Vector3Int max)
            => new(
                Clamp(x.x, min.x, max.x), 
                Clamp(x.y, min.y, max.y),
                Clamp(x.z, min.z, max.z)
            );

        // Dot & Distance
        public static float Dot(Vector2 a, Vector2 b)
            => a.x * b.x + a.y * b.y;
        public static float Dot(Vector3 a, Vector3 b)
            => a.x * b.x + a.y * b.y + a.z * b.z;
        public static float Distance(Vector2 a, Vector2 b)
            => Mathx.Sqrt(Mathx.Sqr(a.x - b.x) + Mathx.Sqr(a.y - b.y));
        public static float Distance(Vector3 a, Vector3 b)
            => Mathx.Sqrt(
                Mathx.Sqr(a.x - b.x) + Mathx.Sqr(a.y - b.y) + Mathx.Sqr(a.z - b.z)
            );

        // Abs
        public static float Abs(float x) => x < 0f ? -x : x;
        public static int Abs(int x) => x < 0 ? -x : x;

        public static Vector2 Abs(Vector2 x) 
            => new(Abs(x.x), Abs(x.y));
        public static Vector3 Abs(Vector3 x) 
            => new(Abs(x.x), Abs(x.y), Abs(x.z));
        public static Vector4 Abs(Vector4 x) 
            => new(Abs(x.x), Abs(x.y), Abs(x.z), Abs(x.w));
        public static Vector2Int Abs(Vector2Int x) 
            => new(Abs(x.x), Abs(x.y));
        public static Vector3Int Abs(Vector3Int x) 
            => new(Abs(x.x), Abs(x.y), Abs(x.z));

        // Floor & Ceil
        public static float Floor(float x) => Mathf.Floor(x);
        public static float Ceil(float x) => Mathf.Ceil(x);
        public static int FloorToInt(float x) => Mathf.FloorToInt(x);
        public static int CeilToInt(float x) => Mathf.CeilToInt(x);

        public static Vector2 Floor(Vector2 x) 
            => new(Floor(x.x), Floor(x.y));
        public static Vector2 Ceil(Vector2 x) 
            => new(Ceil(x.x), Ceil(x.y));
        public static Vector2Int FloorToInt(Vector2 x) 
            => new(FloorToInt(x.x), FloorToInt(x.y));
        public static Vector2Int CeilToInt(Vector2 x) 
            => new(CeilToInt(x.x), CeilToInt(x.y));

        public static Vector3 Floor(Vector3 x) 
            => new(Floor(x.x), Floor(x.y), Floor(x.z));
        public static Vector3 Ceil(Vector3 x) 
            => new(Ceil(x.x), Ceil(x.y), Ceil(x.z));
        public static Vector3Int FloorToInt(Vector3 x) 
            => new(FloorToInt(x.x), FloorToInt(x.y), FloorToInt(x.z));
        public static Vector3Int CeilToInt(Vector3 x) 
            => new(CeilToInt(x.x), CeilToInt(x.y), CeilToInt(x.z));

        public static Vector4Int FloorToInt(Vector4 x) 
            => new(
                FloorToInt(x.x), FloorToInt(x.y), 
                FloorToInt(x.z), FloorToInt(x.w)
            );
        public static Vector4Int CeilToInt(Vector4 x) 
            => new(
                CeilToInt(x.x), CeilToInt(x.y), 
                CeilToInt(x.z), CeilToInt(x.w)
            );

        // Projection
        public static Vector2 Project(Vector2 x, Vector2 y) 
            => Dot(x, y) / Dot(y, y) * y;
        public static Vector3 Project(Vector3 x, Vector3 y) 
            => Dot(x, y) / Dot(y, y) * y;

        public static float ProjectFloat(Vector2 x, Vector2 y) 
            => Dot(x, y) / Dot(y, y);
        public static float ProjectFloat(Vector3 x, Vector3 y) 
            => Dot(x, y) / Dot(y, y);

        // Functions
        public static float Sqr(float x) => x * x;
        public static float Exp(float x) => Mathf.Exp(x);
        public static float Sqrt(float x) => Mathf.Sqrt(x);
        public static float Cos(float x) => Mathf.Cos(x);
        public static float Sin(float x) => Mathf.Sin(x);
        public static float Tan(float x) => Mathf.Tan(x);
        public static float Atan(float x) => Mathf.Atan(x);
        public static float Acos(float x) => Mathf.Acos(x);
        public static float Asin(float x) => Mathf.Asin(x);


        public static int Mod(int x, int y) {
            if (y < 0) return Mod(-x, -y);
            var r = x % y;
            return r >= 0 ? r : r + y;
        }
        
        public static int Div(int x, int y) => x / y - (x % y < 0 ? 1 : 0);
    }

}
