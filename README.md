## Гайдлайны для себя
1. Функция это самая сильная абстракция
2. Меньше объектов значит удобнее
3. Меньше иерархии значит удобнее
4. Меньше компонентов значит удобнее
5. Нет наследованию
6. Нет синглтонам
7. Больше функций значит функциональнее
8. Всего есть три абстракции: `{ }`, `f(x)` и `int x;`
9. Самая лучшая функция - это чистая функция
10. Самый лучшый тип данных - тот который уменьшает число аргументов в функциях
11. Самые лучшие фигурные скобочки - те которые стоят после объявления функции
12. Инкапсуляция это маленькое зло

## План
- [x] Упростить название, переехать 100% на Unity
- [x] Система документации
- [ ] Математика & Физика
	+ [ ] LinAlg
	+ [ ] GeoAlg
	+ [ ] Физика твёрдого тела
	+ [ ] Прямоугольный тайлинг
	+ [ ] Шестиугольный и треугольный тайлинг
	+ [ ] Поиск путей
	+ [ ] Разбиения пространства
- [ ] Процедурные анимации
	+ [ ] FirstOrderSystem
	+ [ ] SecondOrderSystem
	+ [ ] Квадратные пульсы ??? что то про ряды Фурье
	+ [ ] Не только transform ??? Произвольные параметры
	+ [ ] Sprite/Animation Swap
	+ [ ] Левитация и Idle анимации ??
	+ [ ] Функции для кастомных систем
	+ [ ] Inverse Kinematics
- [ ] Базы данных и локализация
	+ [ ] Реляционная база данных с подгрузкой из csv и google doc'ов
- [ ] Парсинг текста
- [ ] Таски и твининг
	+ [ ] Перейти от объектов к функциям
	+ [ ] Нужен ли твининг с процедурными анимациями?
- [ ] UI
	+ [ ] Как привязать процедурные анимации
	+ [ ] Кнопка
	+ [ ] Текст
	+ [ ] Drag & Drop
	+ [ ] Drop Down
	+ [ ] Возможность делать кастомные элементы в одном (!) скрипте
- [ ] Styles
	+ [ ] Можно ли сделать одну и ту же систему для эдитора и рантайма?
	+ [ ] Можно ли и код и префабы на выбор?
	+ [ ] Применяем только для рантайма или эдитор тоже? На выбор?
- [ ] Функционал для Editor'а
	+ [ ] Сериализуемые синглтоны
	+ [ ] Гизмо, только удобнее
	+ [ ] Более удобная система эдиторов на сцене
	+ [ ] Стили но лучше ??
	+ [ ] Драйвинг параметров если возможно ??
	+ [ ] Слои, Автопарентинг и Сетки координат на сцене
	+ [ ] Генераторы для расстановки объектов ??
- [ ] Аттрибуты для Editor'a
	+ [ ] Label("name")
	+ [ ] ReadOnly, ReadOnly(condition) ??
	+ [ ] SubOption("param", condition) ??
	+ [ ] FromArray("array")
	+ [ ] ShowWhen("name", condition)
	+ [ ] DebugGizmo(color) ??
	+ [ ] EditGizmo(style) ??
- [ ] Простые пути и сплайны
- [ ] Кастомный Linq
- [ ] Тайлмапы
	+ Как слои + 2d-деревья и 3d-деревья однородных объектов 
- [ ] User Input
	+ [ ] Набор функций
	+ [ ] Интерфейс для настройки
	+ [ ] Триггеры
	+ [ ] Оси
	+ [ ] Навигация элементов
	+ [ ] Поинтер
- [ ] AI
	+ [ ] Разные полезные функции для обнаружения
	+ [ ] Разные полезные функции для движения
	+ [ ] Навмеш 
	+ [ ] Дефолтный AI: Шутерный противник
	+ [ ] Дефолтный AI: Мариолайк противник 
	+ [ ] Дефолтный AI: Простой 2D Босс 
- [ ] Контроллеры
	+ [ ] Third-person
	+ [ ] Мариолайк
	+ [ ] 8-way
	+ [ ] Автомобиль 
	+ [ ] Самолёт 
	+ [ ] Космический корабль 
	+ [ ] Корабль 
	+ [ ] Тактический
- [ ] Инвентарь
	+ Как Map из слотов в произвольные объекты
	+ [ ] Дефолтный инвентарь
	+ [ ] UI Слотов
- [ ] Платформеры
	+ [ ] Патрулирующая платформа
	+ [ ] Хрупкая платформа
	+ [ ] Прыжковая платформа
- [ ] Шутеры
	+ [ ] Aim Assist 2D и 3D, скорее всего как набор функций для удобства
- [ ] Айтемы
	+ Или как сохранить один объект, меняя его GameObject
	+ [ ] Набор функций
	+ [ ] Дефолтная система
- [ ] Поля
	+ [ ] Набор функций
	+ [ ] Компоненты
- [ ] HLSL функции

Игры в C/OpenGL:
- [ ] Собрать процесс компиляции
- [ ] Мультитрединг
- [ ] Корутины
- [ ] ECS? FLECS?
- [ ] ImGUI 
- [ ] OpenGL
	+ [ ] PBR
	+ [ ] Simple and Fast lights?
	+ [ ] Unlit
	+ [ ] Terrain
	+ [ ] Sunlight
	+ [ ] LODs
	+ [ ] Voxels
	+ [ ] Foliage
- [ ] Повторить всё что выше в Си
