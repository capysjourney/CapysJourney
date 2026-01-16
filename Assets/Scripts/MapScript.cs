using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

abstract public class MapScript : MonoBehaviour
{

    [SerializeField] protected RectTransform _capy;
    [SerializeField] protected RectTransform _mapContainer;

    [SerializeField] protected Button _background;

    [SerializeField] protected Sprite _lockedBtn;
    [SerializeField] protected Sprite _completedBtn;
    [SerializeField] protected NavBarScript _navBarScript;

    [Header("Level Popups")]
    [SerializeField] protected GameObject _levelPopupBelow;
    [SerializeField] protected GameObject _levelPopupAbove;

    [Header("Quincy")]
    [SerializeField] protected Button _quincy;
    [SerializeField] protected GameObject _mask;
    [SerializeField] protected QuincyScript _quincyScript;

    protected Level _level; // level that capy is standing on
    protected Dictionary<Level, LevelStatus> _levelStatuses;
    protected Dictionary<Level, Button> _levelButtonMap = null;
    protected Dictionary<Level, Sprite> _levelIconMap = null;
    protected Dictionary<Image, Level> _previousLevel = null;

    /*
     * Threshold for capy's position to determine whether popup should appear above or below capy
     */
    protected const float Threshold = -94;

    protected Vector3 PopUpBelowPositionRelativeToCapy = new(0, -177.7f, 0);
    protected Vector3 PopUpAbovePositionRelativeToCapy = new(0, 180, 0);

    protected LevelPopupScript _scriptBelow;
    protected LevelPopupScript _scriptAbove;

    private Coroutine _repositionCoroutine;
    private const float AnimationDuration = 0.5f;

    abstract protected void InitializeMaps();
    public virtual void Initialize(Level level, Dictionary<Level, LevelStatus> levelStatuses)
    {
        _levelStatuses = levelStatuses;
        _level = level;
        _scriptBelow = _levelPopupBelow.GetComponent<LevelPopupScript>();
        _scriptAbove = _levelPopupAbove.GetComponent<LevelPopupScript>();
        HideQuincysQuestions();
        HidePopups();
        RepositionMap(level, true);
        StyleRoads();
        StyleLevelButtons(level.World);
        SetClickListeners(level.World);
    }

    protected void RepositionMap(Level level, bool instant = false)
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

    private void StyleRoads()
    {
        if (_previousLevel == null)
        {
            InitializeMaps();
        }
        foreach (Image road in _previousLevel.Keys)
        {
            LevelStatus prevLevelStatus = _levelStatuses[_previousLevel[road]];
            Color32 grayRoadColor = new(182, 202, 203, 255);
            Color32 greenRoadColor = new(29, 121, 64, 255);
            road.color = prevLevelStatus == LevelStatus.Completed ? greenRoadColor : grayRoadColor;
        }
    }

    private void SetClickListeners(World world)
    {
        if (_levelButtonMap == null)
        {
            InitializeMaps();
        }
        foreach (Level level in world.Levels)
        {
            SetLevelClickListener(_levelButtonMap[level], level);
        }
        _background.onClick.AddListener(HidePopups);
        SetCapyClickListener();
        SetQuincyClickListener();
        SetMaskClickListener();
    }
    protected void SetLevelClickListener(Button btn, Level btnLevel)
    {
        btn.onClick.AddListener(() =>
        {
            if (btn.image.sprite == _lockedBtn)
            {
                return;
            }
            OnLevelClicked(btnLevel);
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
        _mask.SetActive(false);
        _quincyScript.gameObject.SetActive(false);
        _quincyScript.OnReset();
    }

    private void ShowQuincysQuestions()
    {
        _mask.SetActive(true);
        _quincyScript.gameObject.SetActive(true);
    }

    private void SetQuincyClickListener()
    {
        _quincy.onClick.AddListener(ShowQuincysQuestions);
        _quincyScript.SetOnFinish(HideQuincysQuestions);
    }

    private void SetMaskClickListener()
    {
        _mask.GetComponent<Button>().onClick.AddListener(HideQuincysQuestions);
    }

    private void OnLevelClicked(Level btnLevel)
    {
        if (_level != btnLevel)
        {
            _level = btnLevel;
            _navBarScript.ChangeLevel(_level);
            GameManager.SetCurrLevel(_level);
            RepositionMap(_level);
        }
        GameObject levelPopup;
        LevelPopupScript script;
        if (_mapContainer.anchoredPosition.y + _capy.anchoredPosition.y >= Threshold)
        {
            levelPopup = _levelPopupBelow;
            script = _scriptBelow;
            levelPopup.GetComponent<RectTransform>().position = _capy.position + PopUpBelowPositionRelativeToCapy;
        }
        else
        {
            levelPopup = _levelPopupAbove;
            script = _scriptAbove;
            levelPopup.GetComponent<RectTransform>().position = _capy.position + PopUpAbovePositionRelativeToCapy;
        }
        script.UpdatePopup(btnLevel.ShortName, btnLevel.Name, btnLevel.Description, 10);
        script.ConfigureStartButton(() =>
        {
            GameManager.SetCurrLevel(btnLevel);
            SceneManager.LoadSceneAsync("Lesson");
        });
        _levelPopupAbove.SetActive(levelPopup == _levelPopupAbove);
        _levelPopupBelow.SetActive(levelPopup == _levelPopupBelow);
    }
    private void StyleLevelButtons(World world)
    {
        if (_levelIconMap == null)
        {
            InitializeMaps();
        }
        foreach (Level level in world.Levels)
        {
            LevelStatus status = _levelStatuses[level];
            Sprite sprite = status switch
            {
                LevelStatus.Locked => _lockedBtn,
                LevelStatus.Completed => _completedBtn,
                LevelStatus.Available => _levelIconMap[level],
                _ => throw new NotImplementedException()
            };
            _levelButtonMap[level].image.sprite = sprite;
        }
    }

    private void HidePopups()
    {
        _levelPopupAbove.SetActive(false);
        _levelPopupBelow.SetActive(false);
    }
}
