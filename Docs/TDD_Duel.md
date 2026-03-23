# TDD — «Дуэль» (Technical Design Document)

## 1. Общие настройки проекта

- Unity: 6000 (URP шаблон).
- Платформа: PC (Windows).
- Цель FPS: 60+ на средней машине.
- Графика:
  - URP, Forward Rendering.
  - Качество: среднее (ограничение теней, простые материалы).

## 2. Структура проекта

### 2.1. Папки

- `Assets/Scripts/Player`
- `Assets/Scripts/Weapons`
- `Assets/Scripts/AI`
- `Assets/Scripts/Systems`
- `Assets/Scripts/UI`
- `Assets/Art/Characters`
- `Assets/Art/Weapons`
- `Assets/Art/Environment`
- `Assets/Art/VFX`
- `Assets/Audio/SFX`
- `Assets/Audio/Music`
- `Assets/Scenes/Menu`
- `Assets/Scenes/Duel_West`
- `Assets/Scenes/Duel_RussiaWinter`
- `Assets/Data` (ScriptableObjects)

### 2.2. Сцены

- `Menu/MainMenu.unity`
- `Duel_West/Duel_West.unity`
- `Duel_RussiaWinter/Duel_RussiaWinter.unity`
- `Sandbox/TestDuel.unity` (внутренняя тестовая сцена)

## 3. Ключевые системы и классы

### 3.1. Игрок и камера

**Класс:** `PlayerControllerFP` (`Scripts/Player`)

- Отвечает за:
  - Обработку ввода (движение, повороты).
  - Перемещение персонажа (CharacterController или Rigidbody).
  - Синхронизацию с камерой.
- Основные поля:
  - `float moveSpeed`
  - `float mouseSensitivity`
  - `Transform cameraTransform`
  - `CharacterController controller`
- Основные методы:
  - `void Update()` — опрос ввода, вызов `HandleMovement()` и `HandleLook()`.
  - `void HandleMovement()`
  - `void HandleLook()`

**Класс:** `CameraSway` (`Scripts/Player`)

- Добавляет лёгкий sway/дрожание камере или оружию.

### 3.2. Оружие и стрельба

**Интерфейс/базовый класс:** `Weapon` (`Scripts/Weapons`)

- Поля:
  - `string weaponName`
  - `float fireDelay`
  - `float damage`
  - `float range`
  - `float spread`
  - `Transform firePoint`
  - `LayerMask hitMask`
- Методы:
  - `void TryFire()` — проверка кулдауна, вызов `Fire()`.
  - `protected virtual void Fire()` — raycast, обработка урона.

**Класс:** `Revolver` : `Weapon`

- Специфика револьвера (анимации, звук).

**Класс:** `BulletImpactEffect`

- Спавнит партиклы/дефекты на точке попадания.

### 3.3. Урон и здоровье

**Интерфейс:** `IDamageable` (`Scripts/Systems`)

- Методы:
  - `void TakeDamage(float amount, HitInfo hitInfo);`

**Структура:** `HitInfo`

- Данные о попадании:
  - `Vector3 hitPoint`
  - `Vector3 hitNormal`
  - `HitBodyPart bodyPart`

**Перечисление:** `HitBodyPart` (`Head`, `Torso`, `Limb`, `Unknown`)

**Класс:** `Health` (`Scripts/Systems`)

- Поля:
  - `float maxHealth`
  - `float currentHealth`
  - `bool isDead`
- Методы:
  - `void TakeDamage(float amount, HitInfo hitInfo)`
  - `void Die()`
- Реализует `IDamageable`.

### 3.4. Дуэльный менеджер

**Класс:** `DuelManager` (`Scripts/Systems`)

- Отвечает за:
  - Логику раунда: подготовка → отсчёт → бой → результат.
  - Взаимодействие с UI и игроком/ботом.
- Состояния (enum или отдельные классы):
  - `DuelState`: `Waiting`, `Countdown`, `Fight`, `Result`
- Поля:
  - `DuelState currentState`
  - `float countdownTime`
  - `float fightStartTime`
  - Ссылки на `PlayerControllerFP`, `AI_Duelist`, UI.
- Методы:
  - `void StartDuel()`
  - `void StartCountdown()`
  - `void StartFight()`
  - `void EndDuel(DuelResult result)`
  - `void OnCharacterDied(Health whoDied)`

**Структура:** `DuelResult`

- Поля:
  - `bool playerWon`
  - `float playerReactionTime`
  - `HitBodyPart playerHitPart`
  - `HitBodyPart enemyHitPart`

### 3.5. ИИ дуэлянта

**Класс:** `AI_Duelist` (`Scripts/AI`)

- Поля:
  - Ссылка на `Weapon` (револьвер).
  - `float reactionDelayMin`, `float reactionDelayMax`
  - `float aimError`
  - `Transform aimTarget` (где целиться в игрока).
- Методы:
  - `void OnDuelStarted()`
  - `void OnFightStart()` — запускает корутину выстрела с задержкой.
  - `void FireAtPlayer()` — raycast в сторону игрока с учётом `aimError`.

### 3.6. UI

**Классы:**

- `UI_DuelHUD` (`Scripts/UI`)
  - Показывает перекрестие, подсказки, состояние.
- `UI_DuelResult` (`Scripts/UI`)
  - Показывает победителя, время реакции, кнопки.

### 3.7. Настройки и данные

**ScriptableObjects:**

- `WeaponConfig` (`Scripts/Weapons` / `Data`)
  - Параметры оружия (урон, разброс, задержка спуска, звуки, анимации).
- `DuelLocationConfig` (`Data`)
  - Название, сцена, визуальные настройки.

### 3.8. Ввод

- Использовать новый Input System или старый — по выбору.
- Абстракция:
  - Класс `PlayerInputHandler` для маппинга действий (Movement, Look, Fire).

## 4. Локации и окружение

### 4.1. Дикий Запад (Duel_West)

- Освещение:
  - Один Directional Light (солнце).
  - Мягкие тени.
- Геометрия:
  - Улица, 2–3 здания, заборы, барьеры.
  - Простые коллайдеры (BoxCollider).
- VFX:
  - Пыль, лёгкий дым от выстрелов.

### 4.2. Россия зима (Duel_RussiaWinter)

- Освещение:
  - Пасмурное небо, холодный цвет.
- Геометрия:
  - Сугробы, небольшие домики/сарай, деревья.
- VFX:
  - Снегопад (particle system), лёгкий туман.

## 5. Аудио

- Система:
  - `AudioManager` (Singleton или Service Locator).
  - Отдельные каналы для SFX, музыки, окружения.
- Эвенты:
  - `OnShotFired`, `OnHit`, `OnDuelStart`, `OnDuelEnd`.

## 6. Сохранения и настройки

- Минимум:
  - Настройки чувствительности мыши, громкости, выбранной сложности.
- Хранение:
  - `PlayerPrefs` или простой JSON (на будущее).

## 7. Тестирование

- Юнит‑тесты (по мере роста проекта):
  - Логика `DuelManager`.
  - Логика урона/смерти.
- Вручную:
  - Проверка реакций ИИ на разных сложностях.
  - Проверка стабильного FPS в локациях.

## 8. Риски и технические ограничения

- Рендер: не перегружать сцену сложными материалами и тенями.
- Анимации: использовать простые риг‑анимации для рук и оружия.
- Потенциальное расширение: сетевой код — отдельный модуль, не мешающий соло‑логике.
