using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

abstract public class MapScript : MonoBehaviour
{

    [SerializeField] private RectTransform _capy;
    [SerializeField] private RectTransform _mapContainer;

    [SerializeField] private Button _background;

    [SerializeField] private Sprite _lockedBtn;
    [SerializeField] private Sprite _completedBtn;
    [SerializeField] private NavBarScript _navBarScript;

    [Header("Level Popups")]
    [SerializeField] private GameObject _levelPopupBelow;
    [SerializeField] private GameObject _levelPopupAbove;

    [Header("Quincy")]
    [SerializeField] private Button _quincy;
    [SerializeField] private Button _mask;
    [SerializeField] private QuincyScript _quincyScript;

    private Image _quincyImage;

    private Level _level; // level that capy is standing on
    private Dictionary<Level, LevelStatus> _levelStatuses;
    private Dictionary<Level, Button> _buttonOfLevel;
    private Dictionary<Level, Sprite> _iconOfLevel;
    private Dictionary<Image, Level> _LevelBeforeRoad;
    private bool _isQuincyUnlocked;

    /*
     * Threshold for capy's position to determine whether popup should appear above or below capy
     */
    private const float Threshold = -94;

    private Vector3 PopUpBelowPositionRelativeToCapy = new(0, -177.7f, 0);
    private Vector3 PopUpAbovePositionRelativeToCapy = new(0, 180, 0);

    private LevelPopupScript _scriptBelow;
    private LevelPopupScript _scriptAbove;

    private Coroutine _repositionCoroutine;
    private const float AnimationDuration = 0.5f;

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

    public void Initialize(Level level,
        Dictionary<Level, LevelStatus> levelStatuses,
        bool isQuincyUnlocked)
    {
        _quincyImage = _quincy.GetComponent<Image>();
        _levelStatuses = levelStatuses;
        _isQuincyUnlocked = isQuincyUnlocked;
        _level = level;
        _scriptBelow = _levelPopupBelow.GetComponent<LevelPopupScript>();
        _scriptAbove = _levelPopupAbove.GetComponent<LevelPopupScript>();
        HideQuincysQuestions();
        HidePopups();
        RepositionMap(level, true);
        StyleQuincy();
        StyleRoads();
        StyleLevelButtons(level.World);
        SetClickListeners(level.World);
    }

    private void InitializeDictionaries()
    {
        _buttonOfLevel = CreateButtonDictionary();
        _iconOfLevel = CreateIconDictionary();
        _LevelBeforeRoad = CreateLevelBeforeRoadDictionary();
    }

    private void RepositionMap(Level level, bool instant = false)
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

    private void StyleQuincy()
    {
        _quincy.gameObject.SetActive(true);
        _quincy.interactable = _isQuincyUnlocked;
    }

    private void StyleRoads()
    {
        if (_LevelBeforeRoad == null)
        {
            InitializeDictionaries();
        }
        foreach (Image road in _LevelBeforeRoad.Keys)
        {
            LevelStatus prevLevelStatus = _levelStatuses[_LevelBeforeRoad[road]];
            Color32 grayRoadColor = new(182, 202, 203, 255);
            Color32 greenRoadColor = new(29, 121, 64, 255);
            road.color = prevLevelStatus == LevelStatus.Completed ? greenRoadColor : grayRoadColor;
        }
    }

    private void SetClickListeners(World world)
    {
        if (_buttonOfLevel == null)
        {
            InitializeDictionaries();
        }
        foreach (Level level in world.Levels)
        {
            SetLevelClickListener(level);
        }
        _background.onClick.AddListener(HidePopups);
        SetCapyClickListener();
        SetQuincyClickListener();
        SetMaskClickListener();
    }
    private void SetLevelClickListener(Level level)
    {
        Button button = _buttonOfLevel[level];
        button.onClick.AddListener(() =>
        {
            if (button.image.sprite != _lockedBtn)
            {
                OnLevelClicked(level);
            }
            AudioManager.Instance.PlayUIEffect(Sound.InitialLevelClick);
            OnLevelClicked(level);
        });
    }
    private void SetCapyClickListener()
    {
        _capy.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnLevelClicked(_level);
        });
    }

    private void HideQuincysQuestions()
    {
        _mask.gameObject.SetActive(false);
        _quincyScript.gameObject.SetActive(false);
    }

    private void ShowQuincysQuestions()
    {
        _mask.gameObject.SetActive(true);
        _quincyScript.gameObject.SetActive(true);
        _quincyScript.OnReset();
    }

    private void SetQuincyClickListener()
    {
        _quincy.onClick.AddListener(ShowQuincysQuestions);
        _quincyScript.SetOnFinish(() =>
        {
            HideQuincysQuestions();
            // todo - world transition
        });
    }

    private void SetMaskClickListener()
    {
        _mask.onClick.AddListener(HideQuincysQuestions);
    }

    private void OnLevelClicked(Level btnLevel)
    {
        if (_level != btnLevel)
        {
            _level = btnLevel;
            GameManager.SetCurrLevel(_level);
            _navBarScript.ChangeLevel(_level);
            RepositionMap(_level);
        }
        GameObject levelPopup;
        LevelPopupScript popupScript;
        if (_mapContainer.anchoredPosition.y + _capy.anchoredPosition.y >= Threshold)
        {
            levelPopup = _levelPopupBelow;
            popupScript = _scriptBelow;
            levelPopup.GetComponent<RectTransform>().position = _capy.position + PopUpBelowPositionRelativeToCapy;
        }
        else
        {
            levelPopup = _levelPopupAbove;
            popupScript = _scriptAbove;
            levelPopup.GetComponent<RectTransform>().position = _capy.position + PopUpAbovePositionRelativeToCapy;
        }
        popupScript.UpdatePopup(btnLevel.ShortName, btnLevel.Name, btnLevel.Description, 10);
        popupScript.ConfigureStartButton(() =>
        {
            GameManager.SetCurrLevel(btnLevel);
            AudioManager.Instance.PlayUIEffect(Sound.LevelBegin);
            SceneManager.LoadSceneAsync("Lesson");
        });
        _levelPopupAbove.SetActive(levelPopup == _levelPopupAbove);
        _levelPopupBelow.SetActive(levelPopup == _levelPopupBelow);
    }
    private void StyleLevelButtons(World world)
    {
        if (_iconOfLevel == null)
        {
            InitializeDictionaries();
        }
        foreach (Level level in world.Levels)
        {
            LevelStatus status = _levelStatuses[level];
            Sprite sprite = status switch
            {
                LevelStatus.Locked => _lockedBtn,
                LevelStatus.Completed => _completedBtn,
                LevelStatus.Available => _iconOfLevel[level],
                _ => throw new NotImplementedException()
            };
            _buttonOfLevel[level].image.sprite = sprite;
        }
    }

    private void HidePopups()
    {
        _levelPopupAbove.SetActive(false);
        _levelPopupBelow.SetActive(false);
    }
}
