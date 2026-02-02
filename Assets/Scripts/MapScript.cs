using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Abstract base class for world map visualization and interaction.
/// Handles level buttons, roads, positioning, popups, and Quincy.
/// </summary>
abstract public class MapScript : MonoBehaviour
{
    [SerializeField] private Sprite _backgroundGradient;

    // Shared references (injected by JourneyScript)
    private RectTransform _mapContainer;
    private Action _onQuincyDone;
    private Image _gradientBackground;
    private NavBarScript _navBarScript;
    private Button _mask;
    private QuincyScript _quincyScript;

    // UI Elements
    private Button _backgroundButton; // to hide popups
    private RectTransform _capy;
    private Button _quincy;
    private GameObject _levelPopupBelow;
    private GameObject _levelPopupAbove;
    private Sprite _lockedBtn;
    private Sprite _completedBtn;

    // State
    private Level _currentLevel;
    private Dictionary<Level, LevelStatus> _levelStatuses;
    private bool _isQuincyUnlocked;

    // Cached dictionaries
    private Dictionary<Level, Button> _buttonOfLevel;
    private Dictionary<Level, Sprite> _iconOfLevel;
    private Dictionary<Image, Level> _levelBeforeRoad;

    // Popup management
    private LevelPopupScript _scriptBelow;
    private LevelPopupScript _scriptAbove;
    private const float PopupThreshold = -94f;
    private Vector3 _popUpBelowPositionRelativeToCapy = new(0, -177.7f, 0);
    private Vector3 _popUpAbovePositionRelativeToCapy = new(0, 180, 0);

    // Animation
    private Coroutine _repositionCoroutine;
    private const float AnimationDuration = 0.5f;

    #region Abstract Methods and Properties
    abstract protected Vector2 QuincyPosition { get; }

    /// <summary>
    /// Initialize and return a dictionary that maps levels to their corresponding buttons
    /// </summary>
    abstract protected Dictionary<Level, Button> CreateButtonDictionary();

    /// <summary>
    /// Initialize and return a dictionary that maps levels to their corresponding icon sprites
    /// </summary>
    abstract protected Dictionary<Level, Sprite> CreateIconDictionary();

    /// <summary>
    /// Initialize and return a dictionary that maps road images to the levels before them
    /// </summary>
    abstract protected Dictionary<Image, Level> CreateLevelBeforeRoadDictionary();


    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the map with game state and shared UI references
    /// </summary>
    public void Initialize(
        RectTransform mapContainer,
        Level currentLevel,
        Dictionary<Level, LevelStatus> levelStatuses,
        bool isQuincyUnlocked,
        Action onQuincyDone,
        Image gradientBackground,
        NavBarScript navBarScript,
        Button quincyMask,
        QuincyScript quincyScript)
    {
        // Inject dependencies
        _mapContainer = mapContainer;
        _onQuincyDone = onQuincyDone;
        _navBarScript = navBarScript;
        _mask = quincyMask;
        _quincyScript = quincyScript;

        // Create UI elements
        _backgroundButton = MakeBackgroundButton();
        _capy = MakePrefab("Capy").GetComponent<RectTransform>();
        _quincy = MakeQuincy();
        _levelPopupBelow = MakePrefab("LevelPopupBelow");
        _levelPopupAbove = MakePrefab("LevelPopupAbove");
        _lockedBtn = Resources.Load<Sprite>("LevelButtons/levelLocked");
        _completedBtn = Resources.Load<Sprite>("LevelButtons/levelCompleted");

        // Set state
        _currentLevel = currentLevel;
        _levelStatuses = levelStatuses;
        _isQuincyUnlocked = isQuincyUnlocked;
        _gradientBackground = gradientBackground;

        // Cache popup scripts
        _scriptBelow = _levelPopupBelow.GetComponent<LevelPopupScript>();
        _scriptAbove = _levelPopupAbove.GetComponent<LevelPopupScript>();

        // Initialize map dictionaries
        InitializeDictionaries();

        // Setup map visuals and interactions
        HideQuincysQuestions();
        HidePopups();
        RepositionMap(_currentLevel.GetInfo(), instant: true);
        StyleBackgroundGradient();
        StyleQuincy();
        StyleRoads();
        World currentWorld = _currentLevel.GetWorld();
        WorldInfo currentWorldInfo = currentWorld.GetInfo();
        StyleLevelButtons(currentWorldInfo);
        SetClickListeners(currentWorldInfo);
        _navBarScript.ChangeLevel(_currentLevel.GetInfo());
    }

    private Button MakeBackgroundButton()
    {
        GameObject backgroundObj = new("MapBackgroundButton");
        backgroundObj.transform.SetParent(transform);
        RectTransform rectTransform = backgroundObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        Image image = backgroundObj.AddComponent<Image>();
        image.color = new(0, 0, 0, 0);
        Button button = backgroundObj.AddComponent<Button>();
        backgroundObj.transform.SetAsFirstSibling(); // put behind other UI elements
        return button;
    }

    private Button MakeQuincy()
    {
        GameObject quincyObj = MakePrefab("Quincy");
        RectTransform rectTransform = quincyObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = QuincyPosition;
        return quincyObj.GetComponent<Button>();
    }

    private GameObject MakePrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        GameObject instance = Instantiate(prefab, transform);
        return instance;
    }

    private void InitializeDictionaries()
    {
        _buttonOfLevel = CreateButtonDictionary();
        _iconOfLevel = CreateIconDictionary();
        _levelBeforeRoad = CreateLevelBeforeRoadDictionary();
    }

    #endregion

    #region Map Positioning

    private void RepositionMap(LevelInfo level, bool instant = false)
    {
        Vector2 mapPos = level.MapPosition;
        Vector2 capyPos = level.CapyPosition;
        _capy.anchoredPosition = capyPos;
        if (instant)
        {
            _mapContainer.anchoredPosition = mapPos;
        }
        else
        {
            if (_repositionCoroutine != null)
            {
                StopCoroutine(_repositionCoroutine);
            }
            _repositionCoroutine = StartCoroutine(AnimateRepositionMap(mapPos));
        }
    }

    private IEnumerator AnimateRepositionMap(Vector2 targetMapPos)
    {
        Vector2 startMapPos = _mapContainer.anchoredPosition;
        float time = 0;

        while (time < AnimationDuration)
        {
            time += Time.deltaTime;
            AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
            float lerpFactor = ease.Evaluate(time / AnimationDuration);
            _mapContainer.anchoredPosition = Vector2.Lerp(startMapPos, targetMapPos, lerpFactor);
            yield return null;
        }

        _mapContainer.anchoredPosition = targetMapPos;
        _repositionCoroutine = null;
    }

    #endregion

    #region Visual Styling
    private void StyleBackgroundGradient()
    {
        _gradientBackground.sprite = _backgroundGradient;
    }

    private void StyleQuincy()
    {
        _quincy.gameObject.SetActive(true);
        _quincy.interactable = _isQuincyUnlocked;
    }

    private void StyleRoads()
    {
        Color32 grayRoadColor = new(182, 202, 203, 255);
        Color32 greenRoadColor = new(29, 121, 64, 255);

        foreach (Image road in _levelBeforeRoad.Keys)
        {
            LevelStatus prevLevelStatus = _levelStatuses[_levelBeforeRoad[road]];
            road.color = prevLevelStatus == LevelStatus.Completed ? greenRoadColor : grayRoadColor;
        }
    }

    private void StyleLevelButtons(WorldInfo world)
    {
        foreach (Level level in world.Levels)
        {
            LevelStatus status = _levelStatuses[level];
            Sprite sprite = status switch
            {
                LevelStatus.Locked => _lockedBtn,
                LevelStatus.Completed => _completedBtn,
                LevelStatus.Available => _iconOfLevel[level],
                _ => throw new NotImplementedException($"Unhandled LevelStatus: {status}")
            };
            _buttonOfLevel[level].image.sprite = sprite;
        }
    }

    #endregion

    #region Click Listeners

    private void SetClickListeners(WorldInfo world)
    {
        foreach (Level level in world.Levels)
        {
            SetLevelClickListener(level);
        }
        _backgroundButton.onClick.AddListener(HidePopups);
        SetCapyClickListener();
        SetQuincyClickListener();
        SetMaskClickListener();
    }

    private void SetLevelClickListener(Level level)
    {
        _buttonOfLevel[level].onClick.AddListener(() =>
        {
            if (_levelStatuses[level] == LevelStatus.Locked)
            {
                return;
            }
            OnLevelClicked(level);
            AudioManager.Instance.PlayUIEffect(Sound.InitialLevelClick);
        });
    }

    private void SetCapyClickListener()
    {
        _capy.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnLevelClicked(_currentLevel);
        });
    }

    private void SetQuincyClickListener()
    {
        _quincy.onClick.AddListener(ShowQuincysQuestions);
        _quincyScript.SetOnFinish(() =>
        {
            HideQuincysQuestions();
            _onQuincyDone?.Invoke();
        });
    }

    private void SetMaskClickListener()
    {
        _mask.onClick.AddListener(HideQuincysQuestions);
    }

    #endregion

    #region Level Selection & Popup

    private void OnLevelClicked(Level selectedLevel)
    {
        LevelInfo levelInfo = selectedLevel.GetInfo();
        // Update current level if changed
        if (_currentLevel != selectedLevel)
        {
            _currentLevel = selectedLevel;
            GameManager.SetCurrLevel(_currentLevel);
            _navBarScript.ChangeLevel(levelInfo);
        }
        RepositionMap(levelInfo);

        // Determine popup position
        GameObject levelPopup;
        LevelPopupScript popupScript;
        float capyScreenY = _mapContainer.anchoredPosition.y + _capy.anchoredPosition.y;

        if (capyScreenY >= PopupThreshold)
        {
            levelPopup = _levelPopupBelow;
            popupScript = _scriptBelow;
            levelPopup.GetComponent<RectTransform>().position = _capy.position + _popUpBelowPositionRelativeToCapy;
        }
        else
        {
            levelPopup = _levelPopupAbove;
            popupScript = _scriptAbove;
            levelPopup.GetComponent<RectTransform>().position = _capy.position + _popUpAbovePositionRelativeToCapy;
        }

        // Configure popup
        popupScript.UpdatePopup(
            levelInfo.ShortName,
            levelInfo.Name,
            levelInfo.Description,
            10
        );
        popupScript.ConfigureStartButton(() =>
        {
            GameManager.SetCurrLevel(selectedLevel);
            AudioManager.Instance.PlayUIEffect(Sound.LevelBegin);
            SceneManager.LoadSceneAsync("Lesson");
        });

        // Show appropriate popup
        _levelPopupAbove.SetActive(levelPopup == _levelPopupAbove);
        _levelPopupBelow.SetActive(levelPopup == _levelPopupBelow);
    }

    private void HidePopups()
    {
        _levelPopupAbove.SetActive(false);
        _levelPopupBelow.SetActive(false);
    }

    #endregion

    #region Quincy Dialog

    private void HideQuincysQuestions()
    {
        _mask.gameObject.SetActive(false);
        _quincyScript.gameObject.SetActive(false);
    }

    private void ShowQuincysQuestions()
    {
        _mask.gameObject.SetActive(true);
        _quincyScript.gameObject.SetActive(true);
    }

    #endregion
}
