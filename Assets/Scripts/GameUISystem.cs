using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GameUISystem : MonoBehaviour
{
    [Header("Основные окна")]
    public GameObject _menuScreen;
    public GameObject _selectScreen;
    public GameObject _aboutScreen;
    public GameObject _bestScoreScreen;
    public GameObject _settingsScreen;
    public GameObject _winnerScreen;
    public GameObject _loserScreen;

    // ------------------------------- элементы окна с очками и уровнем -------------------
    [Header("Окно очков")]
    public GameObject _scoreScreen;
    public TextMeshProUGUI _textLevel;
    public GameUISystem SetTextLevel(string level) { _textLevel.text = level; return this; }
    public TextMeshProUGUI _textScore;
    public GameUISystem SetTextScore(string score) { _textScore.text = score; return this; }

    // ------------------------------- элементы окна с экстра-жизнями ---------------------
    [Header("Окно экстра-жизней")]
    public GameObject _extraScreen;                  // объект окна с эекстра-жизнями
    public GameObject[] _extraLifes;                 // заранее сформированный лист с отображениями
    public Color _activeExtraLifeColor;              // цвет активной экстра-жизни
    public Color _inactiveExtraLifeColor;            // цвет неактивной экстра-жизни
    private int _currentActiveLives = 0;             // при создании скрипта нет активных экстра-жизней

    // отображает требуемое количество экстра-жизней
    public GameUISystem SetExtraLifeCount(int count)
    {
        if (_currentActiveLives != count)
        {
            for (int i = 0; i < count; i++)
            {
                _extraLifeColorSystem[i].SetMainColor(_activeExtraLifeColor);
            }

            for (int i = count; i < _extraLifeColorSystem.Count; i++)
            {
                _extraLifeColorSystem[i].SetMainColor(_inactiveExtraLifeColor);
            }
        }
        return this;
    }
    // назначает все экстра-жизни неактивными
    public GameUISystem SetAllExtraLifeInactive()
    {
        foreach(var item in _extraLifeColorSystem)
        {
            item.SetMainColor(_inactiveExtraLifeColor);
        }
        return this;
    }

    // лист с рендерами, который формируется при старте скрипта
    public List<DissolvableObject> _extraLifeColorSystem = new List<DissolvableObject>();

    [Header("Прочие элементы")]
    public GameObject _audioEngine;

    public enum Mode
    {
        menu, select, about, score, settings, game, winner, loser
    }

    public Mode _activeMode = Mode.game;

    public GameUISystem SetActiveMode(Mode mode) { _activeMode = mode; return this; }
    public GameUISystem.Mode GetActiveMode() { return _activeMode; }

    private Mode _archiveMode = Mode.menu;

    private void Awake()
    {
        for (int i = 0; i < _extraLifes.Length; i++)
        {
            _extraLifeColorSystem.Add(_extraLifes[i].GetComponent<DissolvableObject>().SetMainColor(_inactiveExtraLifeColor));
        }
    }
    private void Start()
    {
        _activeMode = Mode.menu;
    }
    private void Update()
    {
        EnterMainMenu();

        switch (_activeMode)
        {
            case Mode.menu:
                ActivateMenuScreen();
                break;

            case Mode.select:
                ActivateSelectScreen();
                break;

            case Mode.about:
                ActivateAboutScreen();
                break;

            case Mode.score:
                ActivateBestScoreScreen();
                break;

            case Mode.settings:
                ActivateSettingsScreen();
                break;

            case Mode.winner:
                ActivateWinnerScreen();
                break;

            case Mode.loser:
                ActivateLoserScreen();
                break;

            case Mode.game:
                CloseAllScreen();
                break;
        }
    }

    // активация главного меню по нажатию кнопки 'Esc'
    public void EnterMainMenu()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            _audioEngine.GetComponent<GameSoundSystem>().PlayClick();

            if (_activeMode != Mode.menu)
            {
                _archiveMode = _activeMode;
                _activeMode = Mode.menu;
            }
            else
            {
                _activeMode = _archiveMode;
            }
        }
    }

    // активация главного меню
    public void ActivateMenuScreen()
    {
        if (_activeMode != Mode.menu)
        {
            _activeMode = Mode.menu;

            _menuScreen.SetActive(true);
            _selectScreen.SetActive(false);
            _aboutScreen.SetActive(false);
            _bestScoreScreen.SetActive(false);
            _settingsScreen.SetActive(false);
            _winnerScreen.SetActive(false);
            _loserScreen.SetActive(false);
        }
    }
    // активация подменю выбора уровня
    public void ActivateSelectScreen()
    {
        if (_activeMode != Mode.select)
        {
            _activeMode = Mode.select;

            _menuScreen.SetActive(false);
            _selectScreen.SetActive(true);
            _aboutScreen.SetActive(false);
            _bestScoreScreen.SetActive(false);
            _settingsScreen.SetActive(false);
            _winnerScreen.SetActive(false);
            _loserScreen.SetActive(false);
        }
    }
    // активация окна About
    public void ActivateAboutScreen()
    {
        if (_activeMode != Mode.about)
        {
            _activeMode = Mode.about;

            _menuScreen.SetActive(false);
            _selectScreen.SetActive(false);
            _aboutScreen.SetActive(true);
            _bestScoreScreen.SetActive(false);
            _settingsScreen.SetActive(false);
            _winnerScreen.SetActive(false);
            _loserScreen.SetActive(false);
        }
    }
    // активация окна со списком достижений
    public void ActivateBestScoreScreen()
    {
        if (_activeMode != Mode.score)
        {
            _activeMode = Mode.score;

            _menuScreen.SetActive(false);
            _selectScreen.SetActive(false);
            _aboutScreen.SetActive(false);
            _bestScoreScreen.SetActive(true);
            _settingsScreen.SetActive(false);
            _winnerScreen.SetActive(false);
            _loserScreen.SetActive(false);
        }
    }
    // активация меню настроек
    public void ActivateSettingsScreen()
    {
        if (_activeMode != Mode.settings)
        {
            _activeMode = Mode.settings;

            _menuScreen.SetActive(false);
            _selectScreen.SetActive(false);
            _aboutScreen.SetActive(false);
            _bestScoreScreen.SetActive(false);
            _settingsScreen.SetActive(true);
            _winnerScreen.SetActive(false);
            _loserScreen.SetActive(false);
        }
    }
    // активация меню победы
    public void ActivateWinnerScreen()
    {
        if (_activeMode != Mode.winner)
        {
            _activeMode = Mode.winner;

            _menuScreen.SetActive(false);
            _selectScreen.SetActive(false);
            _aboutScreen.SetActive(false);
            _bestScoreScreen.SetActive(false);
            _settingsScreen.SetActive(false);
            _winnerScreen.SetActive(true);
            _loserScreen.SetActive(false);
        }
    }
    // активация меню поражения
    public void ActivateLoserScreen()
    {
        if (_activeMode != Mode.loser)
        {
            _activeMode = Mode.loser;

            _menuScreen.SetActive(false);
            _selectScreen.SetActive(false);
            _aboutScreen.SetActive(false);
            _bestScoreScreen.SetActive(false);
            _settingsScreen.SetActive(false);
            _winnerScreen.SetActive(false);
            _loserScreen.SetActive(true);
        }
    }

    // закрыть все окна и дать управление игроку
    public void CloseAllScreen()
    {
        if (_activeMode != Mode.game)
        {
            _activeMode = Mode.game;

            _menuScreen.SetActive(false);
            _selectScreen.SetActive(false);
            _aboutScreen.SetActive(false);
            _bestScoreScreen.SetActive(false);
            _settingsScreen.SetActive(false);
            _winnerScreen.SetActive(false);
            _loserScreen.SetActive(false);
        }
    }

    // основная функция выключения приложения
    public void ApplicationTurnOff()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }

        Application.Quit();
    }

}
