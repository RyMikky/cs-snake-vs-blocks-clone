using System.Collections.Generic;
using UnityEngine;

public class GameConstantsKeeper : MonoBehaviour
{
    [Header("Серая палитра")]
    public Color _grey_main;
    public Color _grey_pulse;
    public Color _grey_frans;

    [Header("Красная палитра")]
    public Color _red_main;
    public Color _red_pulse;
    public Color _red_frans;

    [Header("Оранжевая палитра")]
    public Color _orange_main;
    public Color _orange_pulse;
    public Color _orange_frans;

    [Header("Желтая палитра")]
    public Color _yellow_main;
    public Color _yellow_pulse;
    public Color _yellow_frans;

    [Header("Зеленая палитра")]
    public Color _green_main;
    public Color _green_pulse;
    public Color _green_frans;

    [Header("Голубая палитра")]
    public Color _cyan_main;
    public Color _cyan_pulse;
    public Color _cyan_frans;

    [Header("Синяя палитра")]
    public Color _blue_main;
    public Color _blue_pulse;
    public Color _blue_frans;

    [Header("Фиолетровая палитра")]
    public Color _purple_main;
    public Color _purple_pulse;
    public Color _purple_frans;

    public enum BoxColorPalette
    {
        none, grey, red, orange, yellow, green, cyan, blue, purple
    }

    public struct ColorPallete
    {
        public ColorPallete(Color main, Color pulse, Color frensnel)
        {
            _mainColor = main;
            _pulseColor = pulse;
            _fresnelColor = frensnel;
        }

        private Color _mainColor;
        private Color _pulseColor;
        private Color _fresnelColor;

        public Color MainColor { get { return _mainColor; } }
        public Color PulseColor { get { return _pulseColor; } }
        public Color FresnelColor { get { return _fresnelColor; } }

    }

    private static readonly Dictionary<BoxColorPalette, ColorPallete> __BASIC_COLOR_PALETTE__
        = new Dictionary<BoxColorPalette, ColorPallete>()
        {
            { BoxColorPalette.none, new ColorPallete(Color.black, Color.black, Color.black) },
            { BoxColorPalette.red, 
                new ColorPallete(
                    new Color(1, 0, 0, 0), new Color(0.75f, 0, 0, 0), new Color(0.2f, 0, 0, 0)) 
            },

            { BoxColorPalette.grey,
                new ColorPallete(
                    new Color(0.4f, 0.4f, 0.4f, 0), new Color(0.2f, 0.2f, 0.2f, 0), new Color(0.1f, 0.1f, 0.1f, 0))
            },

            { BoxColorPalette.orange,
                new ColorPallete(
                    new Color(1, 0.4f, 0, 0), new Color(0.75f, 0.2f, 0, 0), new Color(0.2f, 0.1f, 0, 0))
            },

            { BoxColorPalette.yellow,
                new ColorPallete(
                    new Color(1, 1, 0, 0), new Color(0.75f, 0.75f, 0, 0), new Color(0.2f, 0.2f, 0, 0))
            },

            { BoxColorPalette.green,
                new ColorPallete(
                    new Color(0, 1, 0, 0), new Color(0, 0.75f, 0, 0), new Color(0, 0.2f, 0, 0))
            },

            { BoxColorPalette.cyan,
                new ColorPallete(
                    new Color(0, 1, 1, 0), new Color(0, 0.75f, 0.75f, 0), new Color(0, 0.2f, 0.2f, 0))
            },

            { BoxColorPalette.blue,
                new ColorPallete(
                    new Color(0, 0, 1, 0), new Color(0, 0, 0.75f, 0), new Color(0, 0, 0.2f, 0))
            },

            { BoxColorPalette.purple,
                new ColorPallete(
                    new Color(1, 0, 1, 0), new Color(0.75f, 0, 0.75f, 0), new Color(0.2f, 0, 0.2f, 0))
            }
        };

    private void Awake()
    {
        _grey_main = __BASIC_COLOR_PALETTE__[BoxColorPalette.grey].MainColor;
        _grey_pulse = __BASIC_COLOR_PALETTE__[BoxColorPalette.grey].PulseColor;
        _grey_frans = __BASIC_COLOR_PALETTE__[BoxColorPalette.grey].FresnelColor;

        _red_main = __BASIC_COLOR_PALETTE__[BoxColorPalette.red].MainColor;
        _red_pulse = __BASIC_COLOR_PALETTE__[BoxColorPalette.red].PulseColor;
        _red_frans = __BASIC_COLOR_PALETTE__[BoxColorPalette.red].FresnelColor;

        _orange_main = __BASIC_COLOR_PALETTE__[BoxColorPalette.orange].MainColor;
        _orange_pulse = __BASIC_COLOR_PALETTE__[BoxColorPalette.orange].PulseColor;
        _orange_frans = __BASIC_COLOR_PALETTE__[BoxColorPalette.orange].FresnelColor;

        _yellow_main = __BASIC_COLOR_PALETTE__[BoxColorPalette.yellow].MainColor;
        _yellow_pulse = __BASIC_COLOR_PALETTE__[BoxColorPalette.yellow].PulseColor;
        _yellow_frans = __BASIC_COLOR_PALETTE__[BoxColorPalette.yellow].FresnelColor;

        _green_main = __BASIC_COLOR_PALETTE__[BoxColorPalette.green].MainColor;
        _green_pulse = __BASIC_COLOR_PALETTE__[BoxColorPalette.green].PulseColor;
        _green_frans = __BASIC_COLOR_PALETTE__[BoxColorPalette.green].FresnelColor;

        _cyan_main = __BASIC_COLOR_PALETTE__[BoxColorPalette.cyan].MainColor;
        _cyan_pulse = __BASIC_COLOR_PALETTE__[BoxColorPalette.cyan].PulseColor;
        _cyan_frans = __BASIC_COLOR_PALETTE__[BoxColorPalette.cyan].FresnelColor;

        _blue_main = __BASIC_COLOR_PALETTE__[BoxColorPalette.blue].MainColor;
        _blue_pulse = __BASIC_COLOR_PALETTE__[BoxColorPalette.blue].PulseColor;
        _blue_frans = __BASIC_COLOR_PALETTE__[BoxColorPalette.blue].FresnelColor;

        _purple_main = __BASIC_COLOR_PALETTE__[BoxColorPalette.purple].MainColor;
        _purple_pulse = __BASIC_COLOR_PALETTE__[BoxColorPalette.purple].PulseColor;
        _purple_frans = __BASIC_COLOR_PALETTE__[BoxColorPalette.purple].FresnelColor;
    }

    public ColorPallete GetColorPaletteByType(BoxColorPalette type)
    {
        if (__BASIC_COLOR_PALETTE__.TryGetValue(type, out ColorPallete result))
        {
            return result;
        }

        throw new System.Exception("Такого элемента в списке нет");
    }

    public ColorPallete GetColorPaletteByScore(int score)
    {
        // возвращаем по касту множитель ящика деленный на три
        // полагаю, что выше номера 30-40 не будет
        if (score < 0)
        {
            return __BASIC_COLOR_PALETTE__[BoxColorPalette.none];
        }
        else if (score / 3 > __BASIC_COLOR_PALETTE__.Count - 1)
        {
            return __BASIC_COLOR_PALETTE__[(BoxColorPalette)(__BASIC_COLOR_PALETTE__.Count - 1)];
        }
        else
        {
            return __BASIC_COLOR_PALETTE__[(BoxColorPalette)(score / 3)];
        }
    }

    public struct GameLevelConfig
    {
        // ------------------------------ блок настроек уроовня ----------------------------

        public int _gameLevelWidth { set; get; }                  // ширина уровня в плитках
        public int _boxMaxCount { set; get; }                     // максимум ящиков на полоске элемента
        public int _boxMinValue { set; get; }                     // минимальное значения ящика
        public int _boxMaxValue { set; get; }                     // максимальное значение ящика

        public int _lineVisible { set; get; }                     // количество отображаемых линий
        public int _lineMaxCount { set; get; }                    // максимальное количество сгенерированных линий
        public float _levelMinSpeed { set; get; }                 // минимальная допустимая скорость прокрутки уровня
        public float _levelMaxSpeed { set; get; }                 // максимальная допустимая скорость прокрутки уровня
        public float _LevelStartSpeed { set; get; }               // скорость с которой стартует уровень

        // ------------------------------ блок настроек змеи -------------------------------

        public int _snakeLinkVisible { set; get; }                // количество отображаемых звеньев змеи
        public int _snakeExtraLife { set; get; }                  // количество "экстра" жизней
        public int _snakeScoreScaler { set; get; }                // множитель получаемых очков на уровне

        // ------------------------------ блок настроек камеры -----------------------------

        public float _minCameraDistance { set; get; }             // максимальное приближение камеры
        public float _maxCameraDistance { set; get; }             // максимальное отдаление камеры

        // ээээбанцуа инициализация... ох уж этот шарп...
        public GameLevelConfig(int gamelevelwidth, int boxmaxcount, int boxminvalue, int boxmaxvalue, int linevisible, 
            int linemaxcount, float levelminspeed, float levelmaxspeed, float levelstartspeed, int snakelinkvisible, 
            int snakeextralife, int snakescorescaler, float mincameradistance, float maxcameradistance)
        {
            _gameLevelWidth = gamelevelwidth;
            _boxMaxCount = boxmaxcount;
            _boxMinValue = boxminvalue;
            _boxMaxValue = boxmaxvalue;
            _lineVisible = linevisible;
            _lineMaxCount = linemaxcount;
            _levelMinSpeed = levelminspeed;
            _levelMaxSpeed = levelmaxspeed;
            _LevelStartSpeed = levelstartspeed;
            _snakeLinkVisible = snakelinkvisible;
            _snakeExtraLife = snakeextralife;
            _snakeScoreScaler = snakescorescaler;
            _minCameraDistance = mincameradistance;
            _maxCameraDistance = maxcameradistance;
        }
    }

    public enum GameDifficulty
    {
        demo, easy, normal, hard, insane
    }

    private static readonly Dictionary<GameDifficulty, GameLevelConfig> __GAME_DIFFICULTY__
        = new Dictionary<GameDifficulty, GameLevelConfig>()
        {
            {GameDifficulty.demo, new GameLevelConfig(6, 3, 1, 30, 35, 0, 0.1f, 0.8f, 0.35f, 8, 5, 50, 60, 60) },
            {GameDifficulty.easy, new GameLevelConfig(6, 4, 2, 8, 35, 150, 0.1f, 0.7f, 0.3f, 8, 4, 100, 50, 65) },
            {GameDifficulty.normal, new GameLevelConfig(6, 4, 3, 12, 35, 200, 0.2f, 0.8f, 0.4f, 10, 3, 250, 50, 70) },
            {GameDifficulty.hard, new GameLevelConfig(6, 5, 4, 14, 35, 250, 0.3f, 0.9f, 0.5f, 10, 2, 750, 50, 70) },
            {GameDifficulty.insane, new GameLevelConfig(6, 5, 6, 18, 35, 350, 0.4f, 1.0f, 0.6f, 12, 1, 1500, 50, 75) }
        };

    public GameLevelConfig GetLevelConfiguration(GameDifficulty difficulty)
    {
        return __GAME_DIFFICULTY__[difficulty];
    }
}
