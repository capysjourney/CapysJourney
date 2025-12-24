using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoodScript : MonoBehaviour
{
    [SerializeField] private Image _moodImage;
    [SerializeField] private Sprite _superCapy;
    [SerializeField] private Sprite _goodCapy;
    [SerializeField] private Sprite _mehCapy;
    [SerializeField] private Sprite _badCapy;
    [SerializeField] private Sprite _awfulCapy;
    [SerializeField] private TMP_Text _moodText;
    [SerializeField] private Button _logMoodBtn;
    [SerializeField] private Button _backBtn;
    [SerializeField] private GameObject _pointer;
    [SerializeField] private Button _awfulBtn;
    [SerializeField] private Button _badBtn;
    [SerializeField] private Button _mehBtn;
    [SerializeField] private Button _goodBtn;
    [SerializeField] private Button _superBtn;
    [SerializeField] private GameObject _darkener;
    [SerializeField] private GameObject _popup;
    [SerializeField] private Button _returnBtn;
    [SerializeField] private TMP_Text _rewardText;
    private bool _darkened = false;

    private Mood _currentMood = Mood.Good;

    // Pointer drag state
    private bool _isDragging = false;

    private static readonly float[] _snapAngles = { -70f, -36f, 0f, 36f, 70f };
    private static readonly Dictionary<Mood, float> _moodToAngle = new()
    {
        { Mood.Super, -70f },
        { Mood.Good, -36f },
        { Mood.Meh, 0f },
        { Mood.Bad, 36f },
        { Mood.Awful, 70f }
    };
    private static readonly Dictionary<float, Mood> _angleToMood = new()
    {
        { -70f, Mood.Super },
        { -36f, Mood.Good },
        { 0f, Mood.Meh },
        { 36f, Mood.Bad },
        { 70f, Mood.Awful}
    };

    void Start()
    {
        _logMoodBtn.onClick.AddListener(() => LogMood(_currentMood, System.DateTime.Now));
        _backBtn.onClick.AddListener(() => SceneManager.LoadSceneAsync("Daily"));
        SnapToMood(Mood.Good);
        _awfulBtn.onClick.AddListener(() => SnapToMood(Mood.Awful));
        _badBtn.onClick.AddListener(() => SnapToMood(Mood.Bad));
        _mehBtn.onClick.AddListener(() => SnapToMood(Mood.Meh));
        _goodBtn.onClick.AddListener(() => SnapToMood(Mood.Good));
        _superBtn.onClick.AddListener(() => SnapToMood(Mood.Super));
        _darkener.SetActive(false);
        _darkened = false;
        _popup.SetActive(false);
        _returnBtn.onClick.AddListener(() => SceneManager.LoadSceneAsync("Daily"));
    }

    void Update()
    {
        if (_darkened) return;
        // Handle pointer drag for both mouse and touch
        if (Input.GetMouseButtonDown(0) && IsPointerHit(Input.mousePosition))
        {
            _isDragging = true;
        }
        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            SnapPointerAndSetMood();
        }
        if (_isDragging)
        {
            UpdatePointerAngle(Input.mousePosition);
        }

        // Touch support (for mobile)
        if (Input.touchCount <= 0)
        {
            return;
        }
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (IsPointerHit(touch.position))
                    _isDragging = true;
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (_isDragging)
                {
                    _isDragging = false;
                    SnapPointerAndSetMood();
                }
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (_isDragging)
                {
                    UpdatePointerAngle((Vector3)touch.position);
                }
                break;
        }
    }
    private void SnapToMood(Mood mood)
    {
        RotatePointerTo(_moodToAngle[mood]);
        _currentMood = mood;
        UpdateDisplayedMood(mood);
    }
    private void RotatePointerTo(float angle)
    {
        _pointer.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateDisplayedMood(Mood mood)
    {
        switch (mood)
        {
            case Mood.Awful:
                _moodImage.sprite = _awfulCapy;
                _moodText.text = "Awful";
                break;
            case Mood.Bad:
                _moodImage.sprite = _badCapy;
                _moodText.text = "Bad";
                break;
            case Mood.Meh:
                _moodImage.sprite = _mehCapy;
                _moodText.text = "Meh";
                break;
            case Mood.Good:
                _moodImage.sprite = _goodCapy;
                _moodText.text = "Good";
                break;
            case Mood.Super:
                _moodImage.sprite = _superCapy;
                _moodText.text = "Super";
                break;
            default:
                Debug.LogError("Invalid mood");
                break;
        }
    }

    private void UpdatePointerAngle(Vector3 touchPosition)
    {
        Vector3 pointerScreenPos = _pointer.transform.position;
        Vector3 dir = touchPosition - pointerScreenPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        if (angle < -180f) angle += 360f;
        angle = Mathf.Clamp(angle, -85f, 85f);
        RotatePointerTo(angle);
        UpdateDisplayedMood(PointedMood());
    }


    // Returns true if the screen position is over the pointer object
    private bool IsPointerHit(Vector3 screenPosition)
    {
        RectTransform pointerRect = _pointer.GetComponent<RectTransform>();
        Canvas canvas = _pointer.GetComponentInParent<Canvas>();
        if (canvas == null) return false;
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            pointerRect, screenPosition, canvas.worldCamera, out Vector2 localPoint) &&
            pointerRect.rect.Contains(localPoint);
    }

    // Snaps the pointer to the closest angle and sets the mood
    private void SnapPointerAndSetMood() => SnapToMood(PointedMood());

    private Mood PointedMood()
    {
        float currentZ = _pointer.transform.eulerAngles.z;
        // Convert to range [-180, 180]
        if (currentZ > 180f) currentZ -= 360f;

        // Find closest snap angle
        float minDist = float.MaxValue;
        int closestIdx = 0;
        for (int i = 0; i < _snapAngles.Length; i++)
        {
            float dist = Mathf.Abs(Mathf.DeltaAngle(currentZ, _snapAngles[i]));
            if (dist < minDist)
            {
                minDist = dist;
                closestIdx = i;
            }
        }
        return _angleToMood[_snapAngles[closestIdx]];
    }

    private void LogMood(Mood mood, DateTime dateTime)
    {
        _darkener.SetActive(true);
        _darkened = true;
        _popup.SetActive(true);
        bool alreadyLoggedToday = GameManager.LoggedMoodToday();
        int carrotsEarned = alreadyLoggedToday ? 0 : 10;
        GameManager.LogMood(mood, dateTime);
        _rewardText.text = carrotsEarned.ToString();
        GameManager.IncreaseCarrots(carrotsEarned);

        // Track mood check-in completion with PostHog
        PostHogManager.Instance.Capture("daily_activity_completed", new Dictionary<string, object>
        {
            { "activity_type", "mood_check_in" },
            { "mood", mood.ToString() },
            { "carrots_earned", carrotsEarned },
            { "is_first_today", !alreadyLoggedToday }
        });
    }
}
