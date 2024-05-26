using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    public static class Math {

        public const float SMALL = .01f;
        public const float TINY  = .0001f;
        public const float INF   = float.PositiveInfinity;
        // public const float EPS   = float.Epsilon;
        public const float PI    = Mathf.PI;
        public const float TAU   = 2f * Mathf.PI;
        public const float E     = 2.7182818284590452353602874713526624977572f;

        // Vector constructors
        public static Vector2 Vec2(float x, float y)
            => new Vector2(x, y);
        public static Vector3 Vec3(float x, float y, float z)
            => new Vector3(x, y, z);
        public static Vector4 Vec4(float x, float y, float z, float w)
            => new Vector4(x, y, z, w);
        public static Vector2Int Int2(int x, int y)
            => new Vector2Int(x, y);
        public static Vector3Int Int3(int x, int y, int z)
            => new Vector3Int(x, y, z);
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

        // Clamping
        public static float Clamp(float x, float min, float max)
            => x < min ? min : x > max ? max : x;
        public static Vector2 Clamp(Vector2 x, Vector2 min, Vector2 max)
            => Vec2(Clamp(x.x, min.x, max.x), Clamp(x.y, min.y, max.y));
        public static Vector3 Clamp(Vector3 x, Vector3 min, Vector3 max)
            => Vec3(
                Clamp(x.x, min.x, max.x), 
                Clamp(x.y, min.y, max.y),
                Clamp(x.z, min.z, max.z)
            ); 
        public static Vector4 Clamp(Vector4 x, Vector4 min, Vector4 max)
            => Vec4(
                Clamp(x.x, min.x, max.x), 
                Clamp(x.y, min.y, max.y),
                Clamp(x.z, min.z, max.z),
                Clamp(x.w, min.w, max.w)
            );
        public static int Clamp(int x, int min, int max)
            => x < min ? min : x > max ? max : x;
        public static 
        Vector2Int Clamp(Vector2Int x, Vector2Int min, Vector2Int max)
            => Int2(Clamp(x.x, min.x, max.x), Clamp(x.y, min.y, max.y));
        public static 
        Vector3Int Clamp(Vector3Int x, Vector3Int min, Vector3Int max)
            => Int3(
                Clamp(x.x, min.x, max.x), 
                Clamp(x.y, min.y, max.y),
                Clamp(x.z, min.z, max.z)
            );
        // public static 
        // Vector4Int Clamp(Vector4Int x, Vector4Int min, Vector4Int max)
        //     => Int4(
        //         Clamp(x.x, min.x, max.x), 
        //         Clamp(x.y, min.y, max.y),
        //         Clamp(x.z, min.z, max.z),
        //         Clamp(x.w, min.w, max.w)
        //     );

        // Dot & Distance
        public static float Dot(Vector2 a, Vector2 b)
            => a.x * b.x + a.y * b.y;
        public static float Dot(Vector3 a, Vector3 b)
            => a.x * b.x + a.y * b.y + a.z * b.z;
        public static float Distance(Vector2 a, Vector2 b)
            => Math.Sqrt(Math.Sqr(a.x - b.x) + Math.Sqr(a.y - b.y));
        public static float Distance(Vector3 a, Vector3 b)
            => Math.Sqrt(
                Math.Sqr(a.x - b.x) + Math.Sqr(a.y - b.y) + Math.Sqr(a.z - b.z)
            );

        // Abs
        public static float Abs(float x) => x < 0f ? -x : x;
        public static int Abs(int x) => x < 0 ? -x : x;

        public static Vector2 Abs(Vector2 x) 
            => Vec2(Abs(x.x), Abs(x.y));
        public static Vector3 Abs(Vector3 x) 
            => Vec3(Abs(x.x), Abs(x.y), Abs(x.z));
        public static Vector4 Abs(Vector4 x) 
            => Vec4(Abs(x.x), Abs(x.y), Abs(x.z), Abs(x.w));
        public static Vector2Int Abs(Vector2Int x) 
            => Int2(Abs(x.x), Abs(x.y));
        public static Vector3Int Abs(Vector3Int x) 
            => Int3(Abs(x.x), Abs(x.y), Abs(x.z));

        // Floor & Ceil
        public static float Floor(float x) => Mathf.Floor(x);
        public static float Ceil(float x) => Mathf.Ceil(x);
        public static int FloorToInt(float x) => Mathf.FloorToInt(x);
        public static int CeilToInt(float x) => Mathf.CeilToInt(x);

        public static Vector2 Floor(Vector2 x) 
            => Vec2(Floor(x.x), Floor(x.y));
        public static Vector2 Ceil(Vector2 x) 
            => Vec2(Ceil(x.x), Ceil(x.y));
        public static Vector2Int FloorToInt(Vector2 x) 
            => Int2(FloorToInt(x.x), FloorToInt(x.y));
        public static Vector2Int CeilToInt(Vector2 x) 
            => Int2(CeilToInt(x.x), CeilToInt(x.y));

        public static Vector3 Floor(Vector3 x) 
            => Vec3(Floor(x.x), Floor(x.y), Floor(x.z));
        public static Vector3 Ceil(Vector3 x) 
            => Vec3(Ceil(x.x), Ceil(x.y), Ceil(x.z));
        public static Vector3Int FloorToInt(Vector3 x) 
            => Int3(FloorToInt(x.x), FloorToInt(x.y), FloorToInt(x.z));
        public static Vector3Int CeilToInt(Vector3 x) 
            => Int3(CeilToInt(x.x), CeilToInt(x.y), CeilToInt(x.z));

        // Projection
        public static Vector2 Project(Vector2 x, Vector2 y) 
            => Dot(x, y) / Dot(y, y) * y;
        public static Vector3 Project(Vector3 x, Vector3 y) 
            => Dot(x, y) / Dot(y, y) * y;

        public static float ProjectFloat(Vector2 x, Vector2 y) 
            => Dot(x, y) / Dot(y, y);
        public static float ProjectFloat(Vector3 x, Vector3 y) 
            => Dot(x, y) / Dot(y, y);

        // Swizzles Vector3 -> Vector3
        public static Vector3 Vec000(Vector3 v) => Vec3( 0f,  0f,  0f);
        public static Vector3 Vec00X(Vector3 v) => Vec3( 0f,  0f, v.x);
        public static Vector3 Vec00Y(Vector3 v) => Vec3( 0f,  0f, v.y);
        public static Vector3 Vec00Z(Vector3 v) => Vec3( 0f,  0f, v.z);
        public static Vector3 Vec0X0(Vector3 v) => Vec3( 0f, v.x,  0f);
        public static Vector3 Vec0XX(Vector3 v) => Vec3( 0f, v.x, v.x);
        public static Vector3 Vec0XY(Vector3 v) => Vec3( 0f, v.x, v.y);
        public static Vector3 Vec0XZ(Vector3 v) => Vec3( 0f, v.x, v.z);
        public static Vector3 Vec0Y0(Vector3 v) => Vec3( 0f, v.y,  0f);
        public static Vector3 Vec0YX(Vector3 v) => Vec3( 0f, v.y, v.x);
        public static Vector3 Vec0YY(Vector3 v) => Vec3( 0f, v.y, v.y);
        public static Vector3 Vec0YZ(Vector3 v) => Vec3( 0f, v.y, v.z);
        public static Vector3 Vec0Z0(Vector3 v) => Vec3( 0f, v.z,  0f);
        public static Vector3 Vec0ZX(Vector3 v) => Vec3( 0f, v.z, v.x);
        public static Vector3 Vec0ZY(Vector3 v) => Vec3( 0f, v.z, v.y);
        public static Vector3 Vec0ZZ(Vector3 v) => Vec3( 0f, v.z, v.z);
        public static Vector3 VecX00(Vector3 v) => Vec3( 0f,  0f,  0f);
        public static Vector3 VecX0X(Vector3 v) => Vec3( 0f,  0f, v.x);
        public static Vector3 VecX0Y(Vector3 v) => Vec3( 0f,  0f, v.y);
        public static Vector3 VecX0Z(Vector3 v) => Vec3( 0f,  0f, v.z);
        public static Vector3 VecXX0(Vector3 v) => Vec3( 0f, v.x,  0f);
        public static Vector3 VecXXX(Vector3 v) => Vec3( 0f, v.x, v.x);
        public static Vector3 VecXXY(Vector3 v) => Vec3( 0f, v.x, v.y);
        public static Vector3 VecXXZ(Vector3 v) => Vec3( 0f, v.x, v.z);
        public static Vector3 VecXY0(Vector3 v) => Vec3( 0f, v.y,  0f);
        public static Vector3 VecXYX(Vector3 v) => Vec3( 0f, v.y, v.x);
        public static Vector3 VecXYY(Vector3 v) => Vec3( 0f, v.y, v.y);
        public static Vector3 VecXYZ(Vector3 v) => Vec3( 0f, v.y, v.z);
        public static Vector3 VecXZ0(Vector3 v) => Vec3( 0f, v.z,  0f);
        public static Vector3 VecXZX(Vector3 v) => Vec3( 0f, v.z, v.x);
        public static Vector3 VecXZY(Vector3 v) => Vec3( 0f, v.z, v.y);
        public static Vector3 VecXZZ(Vector3 v) => Vec3( 0f, v.z, v.z);
        public static Vector3 VecY00(Vector3 v) => Vec3( 0f,  0f,  0f);
        public static Vector3 VecY0X(Vector3 v) => Vec3( 0f,  0f, v.x);
        public static Vector3 VecY0Y(Vector3 v) => Vec3( 0f,  0f, v.y);
        public static Vector3 VecY0Z(Vector3 v) => Vec3( 0f,  0f, v.z);
        public static Vector3 VecYX0(Vector3 v) => Vec3( 0f, v.x,  0f);
        public static Vector3 VecYXX(Vector3 v) => Vec3( 0f, v.x, v.x);
        public static Vector3 VecYXY(Vector3 v) => Vec3( 0f, v.x, v.y);
        public static Vector3 VecYXZ(Vector3 v) => Vec3( 0f, v.x, v.z);
        public static Vector3 VecYY0(Vector3 v) => Vec3( 0f, v.y,  0f);
        public static Vector3 VecYYX(Vector3 v) => Vec3( 0f, v.y, v.x);
        public static Vector3 VecYYY(Vector3 v) => Vec3( 0f, v.y, v.y);
        public static Vector3 VecYYZ(Vector3 v) => Vec3( 0f, v.y, v.z);
        public static Vector3 VecYZ0(Vector3 v) => Vec3( 0f, v.z,  0f);
        public static Vector3 VecYZX(Vector3 v) => Vec3( 0f, v.z, v.x);
        public static Vector3 VecYZY(Vector3 v) => Vec3( 0f, v.z, v.y);
        public static Vector3 VecYZZ(Vector3 v) => Vec3( 0f, v.z, v.z);
        public static Vector3 VecZ00(Vector3 v) => Vec3( 0f,  0f,  0f);
        public static Vector3 VecZ0X(Vector3 v) => Vec3( 0f,  0f, v.x);
        public static Vector3 VecZ0Y(Vector3 v) => Vec3( 0f,  0f, v.y);
        public static Vector3 VecZ0Z(Vector3 v) => Vec3( 0f,  0f, v.z);
        public static Vector3 VecZX0(Vector3 v) => Vec3( 0f, v.x,  0f);
        public static Vector3 VecZXX(Vector3 v) => Vec3( 0f, v.x, v.x);
        public static Vector3 VecZXY(Vector3 v) => Vec3( 0f, v.x, v.y);
        public static Vector3 VecZXZ(Vector3 v) => Vec3( 0f, v.x, v.z);
        public static Vector3 VecZY0(Vector3 v) => Vec3( 0f, v.y,  0f);
        public static Vector3 VecZYX(Vector3 v) => Vec3( 0f, v.y, v.x);
        public static Vector3 VecZYY(Vector3 v) => Vec3( 0f, v.y, v.y);
        public static Vector3 VecZYZ(Vector3 v) => Vec3( 0f, v.y, v.z);
        public static Vector3 VecZZ0(Vector3 v) => Vec3( 0f, v.z,  0f);
        public static Vector3 VecZZX(Vector3 v) => Vec3( 0f, v.z, v.x);
        public static Vector3 VecZZY(Vector3 v) => Vec3( 0f, v.z, v.y);
        public static Vector3 VecZZZ(Vector3 v) => Vec3( 0f, v.z, v.z);

        // Swizzles Vector3 -> Vector2
        public static Vector2 Vec00(Vector3 v) => Vec2( 0f,  0f);
        public static Vector2 Vec0X(Vector3 v) => Vec2( 0f, v.x);
        public static Vector2 Vec0Y(Vector3 v) => Vec2( 0f, v.y);
        public static Vector2 Vec0Z(Vector3 v) => Vec2( 0f, v.z);
        public static Vector2 VecX0(Vector3 v) => Vec2(v.x,  0f);
        public static Vector2 VecXX(Vector3 v) => Vec2(v.x, v.x);
        public static Vector2 VecXY(Vector3 v) => Vec2(v.x, v.y);
        public static Vector2 VecXZ(Vector3 v) => Vec2(v.x, v.z);
        public static Vector2 VecY0(Vector3 v) => Vec2(v.y,  0f);
        public static Vector2 VecYX(Vector3 v) => Vec2(v.y, v.x);
        public static Vector2 VecYY(Vector3 v) => Vec2(v.y, v.y);
        public static Vector2 VecYZ(Vector3 v) => Vec2(v.y, v.z);
        public static Vector2 VecZ0(Vector3 v) => Vec2(v.z,  0f);
        public static Vector2 VecZX(Vector3 v) => Vec2(v.z, v.x);
        public static Vector2 VecZY(Vector3 v) => Vec2(v.z, v.y);
        public static Vector2 VecZZ(Vector3 v) => Vec2(v.z, v.z);

        // Swizzles Vector2 -> Vector2
        public static Vector2 Vec00(Vector2 v) => Vec2( 0f,  0f);
        public static Vector2 Vec0X(Vector2 v) => Vec2( 0f, v.x);
        public static Vector2 Vec0Y(Vector2 v) => Vec2( 0f, v.y);
        public static Vector2 VecX0(Vector2 v) => Vec2(v.x,  0f);
        public static Vector2 VecXX(Vector2 v) => Vec2(v.x, v.x);
        public static Vector2 VecXY(Vector2 v) => Vec2(v.x, v.y);
        public static Vector2 VecY0(Vector2 v) => Vec2(v.y,  0f);
        public static Vector2 VecYX(Vector2 v) => Vec2(v.y, v.x);
        public static Vector2 VecYY(Vector2 v) => Vec2(v.y, v.y);

        // Swizzles Vector3Int -> Vector3Int
        public static Vector3Int Int000(Vector3Int v) => Int3(  0,   0,   0);
        public static Vector3Int Int00X(Vector3Int v) => Int3(  0,   0, v.x);
        public static Vector3Int Int00Y(Vector3Int v) => Int3(  0,   0, v.y);
        public static Vector3Int Int00Z(Vector3Int v) => Int3(  0,   0, v.z);
        public static Vector3Int Int0X0(Vector3Int v) => Int3(  0, v.x,   0);
        public static Vector3Int Int0XX(Vector3Int v) => Int3(  0, v.x, v.x);
        public static Vector3Int Int0XY(Vector3Int v) => Int3(  0, v.x, v.y);
        public static Vector3Int Int0XZ(Vector3Int v) => Int3(  0, v.x, v.z);
        public static Vector3Int Int0Y0(Vector3Int v) => Int3(  0, v.y,   0);
        public static Vector3Int Int0YX(Vector3Int v) => Int3(  0, v.y, v.x);
        public static Vector3Int Int0YY(Vector3Int v) => Int3(  0, v.y, v.y);
        public static Vector3Int Int0YZ(Vector3Int v) => Int3(  0, v.y, v.z);
        public static Vector3Int Int0Z0(Vector3Int v) => Int3(  0, v.z,   0);
        public static Vector3Int Int0ZX(Vector3Int v) => Int3(  0, v.z, v.x);
        public static Vector3Int Int0ZY(Vector3Int v) => Int3(  0, v.z, v.y);
        public static Vector3Int Int0ZZ(Vector3Int v) => Int3(  0, v.z, v.z);
        public static Vector3Int IntX00(Vector3Int v) => Int3(  0,   0,   0);
        public static Vector3Int IntX0X(Vector3Int v) => Int3(  0,   0, v.x);
        public static Vector3Int IntX0Y(Vector3Int v) => Int3(  0,   0, v.y);
        public static Vector3Int IntX0Z(Vector3Int v) => Int3(  0,   0, v.z);
        public static Vector3Int IntXX0(Vector3Int v) => Int3(  0, v.x,   0);
        public static Vector3Int IntXXX(Vector3Int v) => Int3(  0, v.x, v.x);
        public static Vector3Int IntXXY(Vector3Int v) => Int3(  0, v.x, v.y);
        public static Vector3Int IntXXZ(Vector3Int v) => Int3(  0, v.x, v.z);
        public static Vector3Int IntXY0(Vector3Int v) => Int3(  0, v.y,   0);
        public static Vector3Int IntXYX(Vector3Int v) => Int3(  0, v.y, v.x);
        public static Vector3Int IntXYY(Vector3Int v) => Int3(  0, v.y, v.y);
        public static Vector3Int IntXYZ(Vector3Int v) => Int3(  0, v.y, v.z);
        public static Vector3Int IntXZ0(Vector3Int v) => Int3(  0, v.z,   0);
        public static Vector3Int IntXZX(Vector3Int v) => Int3(  0, v.z, v.x);
        public static Vector3Int IntXZY(Vector3Int v) => Int3(  0, v.z, v.y);
        public static Vector3Int IntXZZ(Vector3Int v) => Int3(  0, v.z, v.z);
        public static Vector3Int IntY00(Vector3Int v) => Int3(  0,   0,   0);
        public static Vector3Int IntY0X(Vector3Int v) => Int3(  0,   0, v.x);
        public static Vector3Int IntY0Y(Vector3Int v) => Int3(  0,   0, v.y);
        public static Vector3Int IntY0Z(Vector3Int v) => Int3(  0,   0, v.z);
        public static Vector3Int IntYX0(Vector3Int v) => Int3(  0, v.x,   0);
        public static Vector3Int IntYXX(Vector3Int v) => Int3(  0, v.x, v.x);
        public static Vector3Int IntYXY(Vector3Int v) => Int3(  0, v.x, v.y);
        public static Vector3Int IntYXZ(Vector3Int v) => Int3(  0, v.x, v.z);
        public static Vector3Int IntYY0(Vector3Int v) => Int3(  0, v.y,   0);
        public static Vector3Int IntYYX(Vector3Int v) => Int3(  0, v.y, v.x);
        public static Vector3Int IntYYY(Vector3Int v) => Int3(  0, v.y, v.y);
        public static Vector3Int IntYYZ(Vector3Int v) => Int3(  0, v.y, v.z);
        public static Vector3Int IntYZ0(Vector3Int v) => Int3(  0, v.z,   0);
        public static Vector3Int IntYZX(Vector3Int v) => Int3(  0, v.z, v.x);
        public static Vector3Int IntYZY(Vector3Int v) => Int3(  0, v.z, v.y);
        public static Vector3Int IntYZZ(Vector3Int v) => Int3(  0, v.z, v.z);
        public static Vector3Int IntZ00(Vector3Int v) => Int3(  0,   0,   0);
        public static Vector3Int IntZ0X(Vector3Int v) => Int3(  0,   0, v.x);
        public static Vector3Int IntZ0Y(Vector3Int v) => Int3(  0,   0, v.y);
        public static Vector3Int IntZ0Z(Vector3Int v) => Int3(  0,   0, v.z);
        public static Vector3Int IntZX0(Vector3Int v) => Int3(  0, v.x,   0);
        public static Vector3Int IntZXX(Vector3Int v) => Int3(  0, v.x, v.x);
        public static Vector3Int IntZXY(Vector3Int v) => Int3(  0, v.x, v.y);
        public static Vector3Int IntZXZ(Vector3Int v) => Int3(  0, v.x, v.z);
        public static Vector3Int IntZY0(Vector3Int v) => Int3(  0, v.y,   0);
        public static Vector3Int IntZYX(Vector3Int v) => Int3(  0, v.y, v.x);
        public static Vector3Int IntZYY(Vector3Int v) => Int3(  0, v.y, v.y);
        public static Vector3Int IntZYZ(Vector3Int v) => Int3(  0, v.y, v.z);
        public static Vector3Int IntZZ0(Vector3Int v) => Int3(  0, v.z,   0);
        public static Vector3Int IntZZX(Vector3Int v) => Int3(  0, v.z, v.x);
        public static Vector3Int IntZZY(Vector3Int v) => Int3(  0, v.z, v.y);
        public static Vector3Int IntZZZ(Vector3Int v) => Int3(  0, v.z, v.z);

        // Swizzles Vector3 -> Vector2
        public static Vector2Int Int00(Vector3Int v) => Int2(  0,   0);
        public static Vector2Int Int0X(Vector3Int v) => Int2(  0, v.x);
        public static Vector2Int Int0Y(Vector3Int v) => Int2(  0, v.y);
        public static Vector2Int Int0Z(Vector3Int v) => Int2(  0, v.z);
        public static Vector2Int IntX0(Vector3Int v) => Int2(v.x,   0);
        public static Vector2Int IntXX(Vector3Int v) => Int2(v.x, v.x);
        public static Vector2Int IntXY(Vector3Int v) => Int2(v.x, v.y);
        public static Vector2Int IntXZ(Vector3Int v) => Int2(v.x, v.z);
        public static Vector2Int IntY0(Vector3Int v) => Int2(v.y,   0);
        public static Vector2Int IntYX(Vector3Int v) => Int2(v.y, v.x);
        public static Vector2Int IntYY(Vector3Int v) => Int2(v.y, v.y);
        public static Vector2Int IntYZ(Vector3Int v) => Int2(v.y, v.z);
        public static Vector2Int IntZ0(Vector3Int v) => Int2(v.z,   0);
        public static Vector2Int IntZX(Vector3Int v) => Int2(v.z, v.x);
        public static Vector2Int IntZY(Vector3Int v) => Int2(v.z, v.y);
        public static Vector2Int IntZZ(Vector3Int v) => Int2(v.z, v.z);

        // Swizzles Vector2 -> Vector2
        public static Vector2Int Int00(Vector2Int v) => Int2(  0,   0);
        public static Vector2Int Int0X(Vector2Int v) => Int2(  0, v.x);
        public static Vector2Int Int0Y(Vector2Int v) => Int2(  0, v.y);
        public static Vector2Int IntX0(Vector2Int v) => Int2(v.x,   0);
        public static Vector2Int IntXX(Vector2Int v) => Int2(v.x, v.x);
        public static Vector2Int IntXY(Vector2Int v) => Int2(v.x, v.y);
        public static Vector2Int IntY0(Vector2Int v) => Int2(v.y,   0);
        public static Vector2Int IntYX(Vector2Int v) => Int2(v.y, v.x);
        public static Vector2Int IntYY(Vector2Int v) => Int2(v.y, v.y);


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
    }

}
