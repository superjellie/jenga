import itertools

tabs = "    "
print(f"using UnityEngine;");
print(f"namespace Jenga {{");
print(f"{tabs}public static partial class Mathx {{");

print(f"{tabs * 2}public static Vector2 Vec2(float x, float y)")
print(f"{tabs * 3}=> new Vector2(x, y);")
print(f"{tabs * 2}public static Vector3 Vec3(float x, float y, float z)")
print(f"{tabs * 3}=> new Vector3(x, y, z);")
print(f"{tabs * 2}public static Vector4 Vec4(float x, float y, float z, float w)")
print(f"{tabs * 3}=> new Vector4(x, y, z, w);")
print(f"{tabs * 2}public static Color Color(float r, float g, float b, float a)")
print(f"{tabs * 3}=> new Color(r, g, b, a);")

print(f"{tabs * 2}public static Vector2 Vec2(int x, int y)")
print(f"{tabs * 3}=> new Vector2((float)x, (float)y);")
print(f"{tabs * 2}public static Vector3 Vec3(int x, int y, int z)")
print(f"{tabs * 3}=> new Vector3((float)x, (float)y, (float)z);")
print(f"{tabs * 2}public static Vector4 Vec4(int x, int y, int z, int w)")
print(f"{tabs * 3}=> new Vector4((float)x, (float)y, (float)z, (float)w);")

print(f"{tabs * 2}public static Vector2Int Int2(int x, int y)")
print(f"{tabs * 3}=> new Vector2Int(x, y);")
print(f"{tabs * 2}public static Vector3Int Int3(int x, int y, int z)")
print(f"{tabs * 3}=> new Vector3Int(x, y, z);")
print(f"{tabs * 2}public static Vector4Int Int4(int x, int y, int z, int w)")
print(f"{tabs * 3}=> new Vector4Int(x, y, z, w);")

print(f"{tabs * 2}public static Vector2Int Int2(float x, float y)")
print(f"{tabs * 3}=> CeilToInt(Vec2(x, y));")
print(f"{tabs * 2}public static Vector3Int Int3(float x, float y, float z)")
print(f"{tabs * 3}=> CeilToInt(Vec3(x, y, z));")
print(f"{tabs * 2}public static Vector4Int Int4(float x, float y, float z, float w)")
print(f"{tabs * 3}=> CeilToInt(Vec4(x, y, z, w));")

vmap = lambda x: {
	"X": "v.x", "Y": "v.y", "Z": "v.z", "W": "v.w",
	"R": "v.r", "G": "v.g", "B": "v.b", "A": "v.a",
	"0": "0", "1": "1"
}[x]

# Vector4
v4coords = ["X", "Y", "Z", "W", "0", "1"]
print(f"\n{tabs * 2}// Vector4 -> Vector4")
for (A, B, C, D) in itertools.product(v4coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4 Vec{A}{B}{C}{D}(Vector4 v)", end = " ")
	print(f"=> Vec4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Vector4 -> Vector3")
for (A, B, C) in itertools.product(v4coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3 Vec{A}{B}{C}(Vector4 v)", end = " ")
	print(f"=> Vec3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Vector4 -> Vector2")
for (A, B) in itertools.product(v4coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2 Vec{A}{B}(Vector4 v)", end = " ")
	print(f"=> Vec2({a}, {b});")

print(f"\n{tabs * 2}// Vector4 -> Int4")
for (A, B, C, D) in itertools.product(v4coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4Int Int{A}{B}{C}{D}(Vector4 v)", end = " ")
	print(f"=> Int4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Vector4 -> Int3")
for (A, B, C) in itertools.product(v4coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3Int Int{A}{B}{C}(Vector4 v)", end = " ")
	print(f"=> Int3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Vector4 -> Int2")
for (A, B) in itertools.product(v4coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2Int Int{A}{B}(Vector4 v)", end = " ")
	print(f"=> Int2({a}, {b});")

# Vector3
v3coords = ["X", "Y", "Z", "0", "1"]
print(f"\n{tabs * 2}// Vector3 -> Vector4")
for (A, B, C, D) in itertools.product(v3coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4 Vec{A}{B}{C}{D}(Vector3 v)", end = " ")
	print(f"=> Vec4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Vector3 -> Vector3")
for (A, B, C) in itertools.product(v3coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3 Vec{A}{B}{C}(Vector3 v)", end = " ")
	print(f"=> Vec3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Vector3 -> Vector2")
for (A, B) in itertools.product(v3coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2 Vec{A}{B}(Vector3 v)", end = " ")
	print(f"=> Vec2({a}, {b});")

print(f"\n{tabs * 2}// Vector3 -> Int4")
for (A, B, C, D) in itertools.product(v3coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4Int Int{A}{B}{C}{D}(Vector3 v)", end = " ")
	print(f"=> Int4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Vector3 -> Int3")
for (A, B, C) in itertools.product(v3coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3Int Int{A}{B}{C}(Vector3 v)", end = " ")
	print(f"=> Int3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Vector3 -> Int2")
for (A, B) in itertools.product(v3coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2Int Int{A}{B}(Vector3 v)", end = " ")
	print(f"=> Int2({a}, {b});")

# Vector2
v2coords = ["X", "Y", "0", "1"]
print(f"\n{tabs * 2}// Vector2 -> Vector4")
for (A, B, C, D) in itertools.product(v2coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4 Vec{A}{B}{C}{D}(Vector2 v)", end = " ")
	print(f"=> Vec4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Vector2 -> Vector3")
for (A, B, C) in itertools.product(v2coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3 Vec{A}{B}{C}(Vector2 v)", end = " ")
	print(f"=> Vec3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Vector2 -> Vector2")
for (A, B) in itertools.product(v2coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2 Vec{A}{B}(Vector2 v)", end = " ")
	print(f"=> Vec2({a}, {b});")

print(f"\n{tabs * 2}// Vector2 -> Int4")
for (A, B, C, D) in itertools.product(v2coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4Int Int{A}{B}{C}{D}(Vector2 v)", end = " ")
	print(f"=> Int4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Vector2 -> Int3")
for (A, B, C) in itertools.product(v2coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3Int Int{A}{B}{C}(Vector2 v)", end = " ")
	print(f"=> Int3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Vector2 -> Int2")
for (A, B) in itertools.product(v2coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2Int Int{A}{B}(Vector2 v)", end = " ")
	print(f"=> Int2({a}, {b});")


# Int4
i4coords = ["X", "Y", "Z", "W", "0", "1"]
print(f"\n{tabs * 2}// Int4 -> Vector4")
for (A, B, C, D) in itertools.product(i4coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4 Vec{A}{B}{C}{D}(Vector4Int v)", end = " ")
	print(f"=> Vec4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Int4 -> Vector3")
for (A, B, C) in itertools.product(i4coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3 Vec{A}{B}{C}(Vector4Int v)", end = " ")
	print(f"=> Vec3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Int4 -> Vector2")
for (A, B) in itertools.product(i4coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2 Vec{A}{B}(Vector4Int v)", end = " ")
	print(f"=> Vec2({a}, {b});")

print(f"\n{tabs * 2}// Int4 -> Int4")
for (A, B, C, D) in itertools.product(i4coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4Int Int{A}{B}{C}{D}(Vector4Int v)", end = " ")
	print(f"=> Int4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Int4 -> Int3")
for (A, B, C) in itertools.product(i4coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3Int Int{A}{B}{C}(Vector4Int v)", end = " ")
	print(f"=> Int3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Int4 -> Int2")
for (A, B) in itertools.product(i4coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2Int Int{A}{B}(Vector4Int v)", end = " ")
	print(f"=> Int2({a}, {b});")

# Int3
i3coords = ["X", "Y", "Z", "0", "1"]
print(f"\n{tabs * 2}// Int3 -> Vector4")
for (A, B, C, D) in itertools.product(i3coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4 Vec{A}{B}{C}{D}(Vector3Int v)", end = " ")
	print(f"=> Vec4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Int3 -> Vector3")
for (A, B, C) in itertools.product(i3coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3 Vec{A}{B}{C}(Vector3Int v)", end = " ")
	print(f"=> Vec3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Int3 -> Vector2")
for (A, B) in itertools.product(i3coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2 Vec{A}{B}(Vector3Int v)", end = " ")
	print(f"=> Vec2({a}, {b});")

print(f"\n{tabs * 2}// Int3 -> Int4")
for (A, B, C, D) in itertools.product(i3coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4Int Int{A}{B}{C}{D}(Vector3Int v)", end = " ")
	print(f"=> Int4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Int3 -> Int3")
for (A, B, C) in itertools.product(i3coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3Int Int{A}{B}{C}(Vector3Int v)", end = " ")
	print(f"=> Int3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Int3 -> Int2")
for (A, B) in itertools.product(i3coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2Int Int{A}{B}(Vector3Int v)", end = " ")
	print(f"=> Int2({a}, {b});")

# Vector2
i2coords = ["X", "Y", "0", "1"]
print(f"\n{tabs * 2}// Int2 -> Vector4")
for (A, B, C, D) in itertools.product(i2coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4 Vec{A}{B}{C}{D}(Vector2Int v)", end = " ")
	print(f"=> Vec4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Int2 -> Vector3")
for (A, B, C) in itertools.product(i2coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3 Vec{A}{B}{C}(Vector2Int v)", end = " ")
	print(f"=> Vec3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Int2 -> Vector2")
for (A, B) in itertools.product(i2coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2 Vec{A}{B}(Vector2Int v)", end = " ")
	print(f"=> Vec2({a}, {b});")

print(f"\n{tabs * 2}// Int2 -> Int4")
for (A, B, C, D) in itertools.product(i2coords, repeat = 4):
	a, b, c, d = tuple(map(vmap, (A, B, C, D)))
	print(f"{tabs * 2}public static Vector4Int Int{A}{B}{C}{D}(Vector2Int v)", end = " ")
	print(f"=> Int4({a}, {b}, {c}, {d});")

print(f"\n{tabs * 2}// Int2 -> Int3")
for (A, B, C) in itertools.product(i2coords, repeat = 3):
	a, b, c = tuple(map(vmap, (A, B, C)))
	print(f"{tabs * 2}public static Vector3Int Int{A}{B}{C}(Vector2Int v)", end = " ")
	print(f"=> Int3({a}, {b}, {c});")

print(f"\n{tabs * 2}// Int2 -> Int2")
for (A, B) in itertools.product(i2coords, repeat = 2):
	a, b = tuple(map(vmap, (A, B)))
	print(f"{tabs * 2}public static Vector2Int Int{A}{B}(Vector2Int v)", end = " ")
	print(f"=> Int2({a}, {b});")


print(f"{tabs}}}")
print(f"}}")