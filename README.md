# Color Merge

Мобільна merge-гра для Android (портретна орієнтація), виконана як тестове завдання **Merge Grid**.

Гравець перетягує предмети на полі 5×7: два однакових предмети з'єднуються в предмет наступного рівня. Кнопка-генератор створює предмет першого рівня у випадковій вільній клітинці. Мета рівня — виконати його умови (наприклад, зібрати останній предмет ланцюжка) до кінця таймера.

## Зміст

- [Можливості](#можливості)
- [Інструменти та ассети](#інструменти-та-ассети)
- [Запуск](#запуск)
- [Архітектура](#архітектура)
- [Як створити рівень](#як-створити-рівень)
- [Рівень з туторіалом](#рівень-з-туторіалом)
- [Збереження](#збереження)

---

## Можливості

- **Поле 5×7**, Drag & Drop пальцем: переміщення, обмін місцями, мердж. При промаху предмет повертається на місце.
- **Ланцюжок із 7 предметів** (налаштовується, 6–8).
- **Генератор** створює предмет першого рівня у випадкову вільну клітинку; генератор можна **покращувати** за енергію, отриману з мерджів.
- **Умови рівня**: таймер, зібрати предмет рівня X, зробити X мерджів, прокачати генератор до рівня X — у будь-якій комбінації.
- **Таймер** стартує з першого мерджу й зупиняється на перемозі. **Рекорд** зберігається між сесіями й показується поруч із таймером.
- **Цукерка** (Candy Connector): предмет X + цукерка → предмет X+1.
- **Стіни**: заблоковані клітинки поля.
- **Цукеркова бомба**: перетворює предмети й стіни навколо на цукерки (затемнення, вибух, трусіння екрана).
- **12 рівнів**, з них 6 навчальних з туторіалами (0, 1, 2, 5, 7, 9).
- **Меню**: вибір рівня, замки на непройдених рівнях, рекорди, налаштування (музика, звуки, вібрація, скидання прогресу).
- Анімації мерджу, спавну й перемоги, конфеті, зірки, звуки, музика, вібрація.
- Коректне відображення на **9:16, 9:19.5 і 3:4** (Canvas Scaler + SafeArea).

---

## Інструменти та ассети

**Рушій:** Unity 6 (6000.3.22f1), URP 2D, Android (IL2CPP, ARM64).

**Бібліотеки та плагіни:**

| Що | Для чого |
|---|---|
| **VContainer** | DI-контейнер: LifetimeScope, Entry Point |
| **UniTask** | Асинхронність (корутини й `Task` не використовуються) |
| **Addressables** | Завантаження конфігів рівнів, префабів, звуків і музики |
| **DOTween (Pro)** | Анімації: мердж, спавн, перемога, UI |
| **Odin Inspector** | Серіалізація `HashSet` умов рівня та інспектор `LevelConfig` |
| **TextMeshPro** | Текст |
| **Text Animator (Febucci)** | Друк тексту туторіалів, пульсація «Tap to continue» |
| **uGUI** | Інтерфейс: Canvas Scaler (Scale With Screen Size), SafeArea |

**Ассети:**

| Що | Де використано |
|---|---|
| **GUI Pack Cartoon** | Кнопки, вікна, іконки, конфеті, стрілки й палець туторіалу |
| **Puzzle Pack** | Предмети ланцюжка й клітинки поля |
| Шрифти **Lilita One**, **BPreplay Extended Bold** | Увесь текст гри |
| Музика й звуки | Меню, геймплей, кліки, мердж, вибух |

---

## Запуск

1. Відкрити проєкт у **Unity 6000.3.22f1**.
2. Відкрити сцену `Assets/Game/Scenes/Boot.unity` і натиснути **Play**.
   > Запускати гру потрібно саме з **Boot**: там реєструються глобальні сервіси. Сцена рівня, запущена напряму, видасть помилку.
3. Збірка: **File → Build Profiles → Android → Build**.

---

## Архітектура

### Порядок запуску сцен

```
Boot ──► Mid Loader ──► Main Menu ──► Mid Loader ──► Level_N
                            ▲                          │
                            └──────── Mid Loader ◄─────┘
```

| Сцена | Скоуп | Що робить |
|---|---|---|
| **Boot** | `ApplicationScope` | Живе всю гру (`DontDestroyOnLoad`). Реєструє глобальні сервіси: збереження, звук, вібрацію, завантаження сцен, Addressables, конфіги. `BootController` прогріває звуки й відкриває меню |
| **Mid Loader** | `MidLoaderScope` | Екран завантаження: смуга прогресу й підказки. Завантажує наступну сцену у фоні й вмикає її за шторкою |
| **Main Menu** | `MainMenuScope` | Головне меню, вибір рівня, налаштування |
| **Level_N** | `Level_N_Scope` | Рівень. Кожен рівень має свій скоуп і контролер |

Дочірні скоупи підключаються до `ApplicationScope`, тому глобальні сервіси доступні в меню й на рівнях.

### Збірки (asmdef)

```
Core.*  ◄──  Content  ◄──  Infrastructure
```

- **Core** — переносний код без ігрових термінів (сервіси й утиліти). Кожен модуль — окрема збірка (`Core.Save`, `Core.Sound`…).
- **Content** — геймплей і UI. Бачить `Core`, але `Core` не бачить `Content`.
- **Infrastructure** — скоупи й контролери сцен, «склеює» `Content` і `Core`.

### Структура папок

```
Assets/Game/
├── Fonts/                          шрифти TextMeshPro
├── Scenes/
│   ├── Boot.unity
│   ├── Mid Loader.unity
│   ├── Main Menu.unity
│   └── Levels/Level_0 … Level_11.unity
└── Modules/
    ├── Core/                       переносні сервіси
    │   ├── AssetManagement/        IAssetProvider (Addressables)
    │   ├── Save/                   ISaveService (PlayerPrefs)
    │   ├── SceneManagement/        ISceneLoader
    │   ├── Sound/                  ISoundService
    │   ├── Vibration/              IVibrationService
    │   ├── Timer/                  Stopwatch, TimeFormatter
    │   ├── UI/                     ScreenFader, SafeArea, ScreenShakeView
    │   ├── Utils/                  розширення (пульс, сортинг, вікна, очікування)
    │   └── Infrastructure/         скоупи й контролери сцен
    │       ├── Bootstrap/          ApplicationScope, BootController
    │       ├── MidLoader/          MidLoaderScope, MidLoaderController
    │       ├── MainMenu/           MainMenuScope, MainMenuController
    │       └── Levels/
    │           ├── Scripts/        BaseLevelScope, LevelGameplay, LevelLoader
    │           └── Level 0 … Level 11/   Level_N_Scope, Level_N_Controller
    └── Content/                    геймплей і UI
        ├── Board/                  поле, перетягування, мердж, вибух бомби
        ├── Slot/                   клітинка поля
        ├── Item/                   предмети, ланцюжок, конфіги предметів
        ├── Generator/              генератор і його покращення
        ├── CandyBomb/              цукеркова бомба
        ├── Level/                  LevelConfig, умови, таймер, збереження прогресу, звуки й вібрація рівня
        ├── Tutorial/               сценарій туторіалу, кроки, стрілки, палець
        ├── SFXOneShot/             короткі звуки
        └── GUI/
            ├── HUD/                таймер, рекорд, цілі, ланцюжок предметів
            ├── Popups/             перемога, поразка, повідомлення
            ├── Settings/           налаштування
            ├── Main Menu/          меню, вибір рівня, кнопка рівня
            ├── Loading/            смуга завантаження, підказки
            ├── Level Screen/       UI Canvas рівня, фон, камера, EventSystem
            ├── Atlas/              Sprite Atlas
            └── Icon/               іконка гри
```

Кожна фіча лежить в одній папці: `Scripts/` + `Prefabs/` (+ `Configs/`, `Addressables/`).

### Ключові системи

| Клас | Відповідальність |
|---|---|
| `LevelGameplay` | Загальний хід будь-якого рівня: завантаження конфігу, побудова поля, таймер, перевірка умов, перемога й поразка, збереження |
| `Level_N_Controller` | Точка входу рівня: що зробити перед грою (туторіал), потім `LevelGameplay.RunAsync` |
| `BaseLevelScope` | Спільні реєстрації рівня (поле, генератор, бомба, UI). `Level_N_Scope` додає лише свої |
| `BoardController` | Модель поля й рішення по дропу: move / swap / merge, вибух бомби |
| `GeneratorController` | Спавн предмета у випадкову вільну клітинку |
| `UpgradeGeneratorSystem` | Енергія з мерджів і покращення генератора |
| `CandyBombSystem` | Енергія, створення й вибух бомби |
| `LevelConditionsChecker` | Перевірка умов перемоги та часу |
| `LevelLoader` | Перехід між сценами через Mid Loader |
| `LevelProgressRepository` / `BestTimeRepository` | Пройдені рівні та рекорди |

---

## Як створити рівень

Рівень складається з п'яти частин: **конфіг**, **сцена**, **скоуп + контролер**, **рядок у `Levels`** і **кнопка в меню**. Приклад — новий **Level 12**.

### 1. Конфіг рівня — `LevelConfig`

Усе, що відбувається на рівні, налаштовується тут, без коду.

1. У `Assets/Game/Modules/Content/Level/Configs/` продублювати схожий рівень (`Ctrl+D`) і назвати `Level_12`, або **ПКМ → Create → Game → Level → Level Config**.
2. Позначити ассет як **Addressable** (адреса `Level_12`, група `Default Local Group`).
3. Налаштувати поля:

| Розділ | Поле | Що задає |
|---|---|---|
| — | **Item Chain** | Ланцюжок предметів (`Item/Configs/ItemChain.asset`) |
| **Conditions** | список умов | Умови перемоги, кожен тип — не більше одного разу: |
| | `Timer` | час на рівень у секундах; без нього таймера й рекорду немає |
| | `Get_Item_With_Level_X` | зібрати предмет рівня X |
| | `Make_Merge_X_Times` | зробити X мерджів |
| | `Upgraded_Generator_With_Level_X` | прокачати генератор до рівня X |
| **Board** | Columns / Rows | Розмір поля (за ТЗ 5×7) |
| | Blocked Slots | Клітинки-стіни. Номер = `рядок × Columns + стовпець`, відлік з лівого верхнього кута |
| | Initial Items | Стартові предмети: клітинка + рівень предмета (1 — перший) |
| **Candy Connector** | Candy Connector, Initial Candies | Цукерка та клітинки, де вона лежить на старті |
| **Candy Bomb** | Candy Bomb, Cost | Кнопка бомби та її ціна в енергії (1 мердж = 1) |
| **Generator** | Generator Charges | Кількість натискань генератора (−1 — без обмежень) |
| | Generator Upgrade Enabled / Cost | Кнопка покращення генератора та її ціна |
| **Stars** | Three / Two Stars Time | Пороги зірок за часом (лише з `Timer`) |

Під полями інспектор сам показує підказки, якщо щось налаштовано неправильно (немає цілей, предмет на стіні, бомба без цукерки тощо).

### 2. Сцена

1. У `Assets/Game/Scenes/Levels/` продублювати `Level_11.unity` → `Level_12.unity`.
2. Додати сцену в **Build Profiles → Scene List**.

Увесь інтерфейс сцени (`UI Canvas`, `Drag Canvas`, камера, EventSystem) — екземпляри спільних префабів, збирати його вручну не потрібно.

### 3. Скоуп і контролер

Створити папку `Assets/Game/Modules/Core/Infrastructure/Levels/Level 12/` і два файли.

**`Level_12_Scope.cs`:**
```csharp
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_12
{
    public class Level_12_Scope : BaseLevelScope
    {
        protected override void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Level_12_Controller>();
        }
    }
}
```

**`Level_12_Controller.cs`:**
```csharp
using System.Threading;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_12
{
    public class Level_12_Controller : IAsyncStartable
    {
        private readonly LevelGameplay _levelGameplay;

        public Level_12_Controller(LevelGameplay levelGameplay)
        {
            _levelGameplay = levelGameplay;
        }

        public UniTask StartAsync(CancellationToken cancellation)
        {
            return _levelGameplay.RunAsync(cancellation);
        }
    }
}
```

**Підключити до сцени:** у `Level_12.unity` на об'єкті **LevelScope** видалити компонент `Level_11_Scope`, додати **`Level_12_Scope`** і в поле **Level Config** перетягнути `Level_12.asset`.

### 4. Порядок рівнів — ассет `Levels`

У `Content/Level/Configs/Levels.asset` додати рядок у кінець списку:

| Scene Name | Id | Is Tutorial |
|---|---|---|
| `Level_12` | `level_12` | ☐ |

- **Порядок рядків** — порядок рівнів у меню й для кнопки CONTINUE. Рівень відкривається, коли пройдено попередній.
- **Id** — ключ збережень (пройдено, рекорд). Після релізу його краще не змінювати.
- **Is Tutorial** — навчальний рівень: вікно NEW CONTENT у меню, рекорд не зберігається.

### 5. Кнопка в меню

1. У `Main Menu.unity` знайти `LevelSelectPopup/Panel/Levels`.
2. Перетягнути туди префаб `Content/GUI/Main Menu/Prefabs/Level Button.prefab` і назвати `Level 12` — сітка сама поставить кнопку на місце.
3. На `LevelSelectPopup` у компоненті `LevelSelectView` додати кнопку в кінець списку **Level Buttons** (порядок збігається з `Levels`).

Вигляд усіх кнопок рівнів змінюється в одному префабі `Level Button`.

### 6. Перевірка

Відкрити **Boot** → **Play** → **PLAY** → вибрати рівень. Якщо в консолі помилка «Сцени Level_12 немає в LevelsConfig» — не додано рядок у `Levels` або помилка в імені сцени.

---

## Рівень з туторіалом

До кроків звичайного рівня додаються три речі.

### 1. Сценарій туторіалу в сцені

У `Content/Tutorial/Tutor Scenario/` лежать сценарії наявних рівнів (`TUTOR SCENARIO Level_N.prefab`) і їхні кроки (`Level_N/LN_Tutor K.prefab`).

1. Продублювати схожий сценарій і його кроки (`Tutor BASE.prefab` — базовий крок).
2. У кожному кроці задати текст (`_text`) і стрілки (`_arrows`).
3. У сценарії (`TutorScenario`) заповнити список **Steps**. Для кожного кроку:
   - **Tutor** — префаб кроку;
   - **Skip Condition** — коли крок закривається: `Tap` (тап по екрану), `ItemMerged` (після мерджу), `ButtonClick` (натискання кнопки з поля **Required Button**), `External` (з коду через `RequestSkip()`);
   - **Use Background Fade** — затемнювати фон.
4. Покласти сценарій у сцену рівня.

### 2. Скоуп — реєстрація туторіалу

```csharp
using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Content.Tutorial.Tutor_Scenario.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_12
{
    public class Level_12_Scope : BaseLevelScope
    {
        [SerializeField] private TutorScenario _tutorScenario;
        [SerializeField] private MoveableTutorArrow _tutorFinger;

        protected override void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterComponent(_tutorScenario).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(_tutorFinger);

            builder.RegisterEntryPoint<Level_12_Controller>();
        }
    }
}
```

У сцені в поля скоупа **Tutor Scenario** і **Tutor Finger** перетягнути сценарій і палець.

### 3. Контролер — кроки туторіалу

Контролер спершу програє туторіал, потім запускає гру:

```csharp
public UniTask StartAsync(CancellationToken cancellation)
{
    return _levelGameplay.RunAsync(PlayTutorialAsync, cancellation);
}

private async UniTask PlayTutorialAsync(ItemChainConfig chain, CancellationToken ct)
{
    _boardController.SetInteractable(false);

    _preTutorCallbacks.With(() => _tutorScenario.GetTutor(0).Arrows[0].PointAt(_generatorButton.RT));
    await _tutorScenario.RunStepWithCallbacks(0, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);

    _boardController.SetInteractable(true);
}
```

- `RunStepWithCallbacks(index, pre, post, фон, стрілки після тексту, ct)` показує крок і чекає його закриття.
- `pre`-колбеки виконуються до тексту (підсвітити об'єкт, навести стрілку), `post` — після.
- Підсвічування об'єкта поверх затемнення — `ChangeSortGroupInObj` / `ResetSortGroupInObj` (об'єкт реалізує `IChangeSortGroupInCanvas`).

Готові приклади: `Level_0_Controller` (перший мердж і палець), `Level_9_Controller` (бомба й закриття кроку з коду).

У `Levels` для такого рівня ставиться галочка **Is Tutorial**.

---

## Збереження

Усі дані зберігаються через `ISaveService` (реалізація — `PlayerPrefsSaveService`):

| Ключ | Значення |
|---|---|
| `level_completed_<id>` | `1`, якщо рівень пройдено |
| `best_time_<id>` | найкращий час проходження (секунди) |
| гучність музики й звуків, вібрація | налаштування |

- **Перемога**: рівень позначається пройденим, рекорд оновлюється, якщо новий час кращий. Дані одразу записуються на диск.
- **Меню**: рівень відкритий, якщо пройдено попередній (рівень 0 відкритий завжди).
- **Clear Progress** у налаштуваннях меню стирає всі збереження.
