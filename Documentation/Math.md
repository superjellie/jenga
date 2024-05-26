## Jenga.Math
Математический модуль.
Цель &mdash; собирать тут все математические функции работающие с малым набором данных.
Таким образом сюда потихоньку переезжает контент из Mathf, Vector2, Vector3, Vector2Int, Vector3Int, Vector4 и т.п.

**TODO**: Внести последние изменения.

### Индекс
- [Интерполяция](#интерполяция)
<!-- - [Тригонометрия](#тригонометрия)  -->
- [Операции над векторами](#операции-над-векторами)

### Референс функций
- [Lerp](#lerp) Линейная интерполяция
- [Slerp](#slerp) Сферическая интерполяция
- [Vec](#vec) Конструктор вектора
- [Vec\*\*\*, Int\*\*\*, Color\*\*\*](#vec-int-color) Перестановка (свизлинг) координат вектора
### Интерполяция
#### Lerp
```cs
T Lerp(T a, T b, float t)
``` 
`T` это `float`, `Vector2` или `Vector3`.

Линейная интерполяция между `a` и `b` с параметром `t`.
Если `t` плавно менять от `0` до `1` то результат будет плавно меняться
от `a` к `b`.
Формально равно `a * (1 - t) + b * t`.

*Эквивалентно Mathf.Lerp, Vector2.Lerp, Vector3.Lerp.
Существует для удобства.*

#### Slerp
```cs
Quaternion Slerp(Quaternion a, Quaternion b, float t)
``` 
Сферическая интерполяция между кватернионами `a` и `b` с параметром `t`.
Если `t` плавно менять от `0` до `1` то результат будет плавно меняться
от `a` к `b`.

*Эквивалентно Quaternion.Slerp.
Существует для удобства.*

### Операции над векторами
#### Vec
```cs
VectorN VecN(float x, float y, ...);
VectorNInt IntN(int x, int y, ...);
```
где `N` &mdash; 2, 3 или 4 и количество аргументов равно N.

Создаёт новый вектор. 

*Эквивалентно `new VectorN(), new VectorNInt()`
Существует для удобства.*
#### Vec\*\*\*, Int\*\*\*, Color\*\*\*
```cs
float Float*(T v);
VectorN Vec***(T v);
VectorNInt Int***(T v);
Color Color***(T v);
```
где:
  - `N`, &mdash; это 2, 3 или 4
  - `T` &mdash; это любой векторный тип (`VectorM, VectorMInt, Color, float`)
  - вместо звёздочек надо подставлять `X`, `Y`, `Z`, (`R`, `G`, `B`) `0` или `1` (насколько это имеет смысл для типа `T`).

Переставляет координаты вектора в соотвествии с порядком `X`, `Y`, `Z` (`R`, `G`, `B`), обнуляет координаты с `0`, приравнивает координаты с `1` к единице.
Там где это надо округляет дробные числа вниз.

Например:
```cs
var a = Math.Vec3(1f, 2f, 3f);
var b = Math.VecZ0X(a); // (3f, 0f, 1f)
var c = Math.VecY0(a); // (2f, 0f)
var d = Math.IntX1XY(a); // (1, 1, 1, 2)
var e = Math.Vec0101(a); // (0f, 1f, 0f, 1f)
```

**TODO**: Убрать аргементы там где лишнее.