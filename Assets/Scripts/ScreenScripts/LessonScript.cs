using Firebase.Extensions;
using Firebase.Storage;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LessonScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Button _backwardButton;
    [SerializeField] private Button _forwardButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private TMP_Text _timeElapsedText;
    [SerializeField] private TMP_Text _durationText;
    [SerializeField] private Slider _slider;
    [SerializeField] private Sprite _paused;
    [SerializeField] private Sprite _unpaused;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private EventTrigger _sliderTrigger;
    [SerializeField] private Image _banner;
    [SerializeField] private GameObject _darkener;
    [SerializeField] private Button _backArrow;
    [SerializeField] private TMP_Text _levelNumText;
    [SerializeField] private Button _redoButton;
    [SerializeField] private Button _bookmarkButton;
    [SerializeField] private Sprite _unfilledBookmark;
    [SerializeField] private Sprite _filledBookmark;
    [SerializeField] private Button _homeButton;
    [SerializeField] private GameObject _levelCompletePopup;
    private Image _pauseButtonImage;
    private Image _bookmarkButtonImage;
    private Image _forwardButtonImage;
    private float _maxTimeReached;
    private float _durationInSeconds;

    private bool _playAudio = false;
    private bool _hasCompleted = false;
    private bool _userIsDraggingSlider = false;
    private readonly bool DebugMode = true; // todo - set to false for production

    void Start()
    {
        _pauseButtonImage = _pauseButton.GetComponent<Image>();
        _bookmarkButtonImage = _bookmarkButton.GetComponent<Image>();
        _forwardButtonImage = _forwardButton.GetComponent<Image>();
        Level currLevel = GameManager.GetCurrLevel();
        _title.text = currLevel.Name;
        _hasCompleted = GameManager.HasCompletedLevel(currLevel);

        LoadImage();
        LoadAudio();
        _slider.maxValue = _durationInSeconds;
        TimeSpan durationTS = TimeSpan.FromSeconds(_durationInSeconds);
        _durationText.text = FormatSeconds(_durationInSeconds);
        _backwardButton.onClick.AddListener(OnBackward);
        _forwardButton.onClick.AddListener(OnForward);
        _pauseButton.onClick.AddListener(OnPause);
        _redoButton.onClick.AddListener(Redo);
        _audioSource.volume = 1.0f;
        _backArrow.onClick.AddListener(ReturnToJourney);
        _homeButton.onClick.AddListener(ReturnToJourney);
        _bookmarkButton.onClick.AddListener(OnBookmarkButtonClicked);
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        // only update audio, etc. once the user unselects the slider
        EventTrigger.Entry pointerUp = new()
        {
            eventID = EventTriggerType.PointerUp
        };
        pointerUp.callback.AddListener((eventData) => OnUserReleasedSlider());
        _sliderTrigger.triggers.Add(pointerUp);
        Brighten();
        _levelCompletePopup.SetActive(false);
        UpdateForwardButtonState();
    }
    private void ReturnToJourney() => SceneManager.LoadSceneAsync("Journey");

    private void Darken() => _darkener.SetActive(true);

    private void Brighten() => _darkener.SetActive(false);

    private void LoadImage()
    {
        string bannerName = GameManager.GetCurrLevel().BannerFile;
        Sprite sprite = Resources.Load<Sprite>(bannerName);
        if (sprite == null)
        {
            Debug.LogError("Banner image not found: " + bannerName);
        }
        _banner.sprite = sprite;
    }

    // private void LoadImage()
    //{
    //    string bannerName = GameManager.GetCurrLevel().BannerFile;
    //    FirebaseStorage storage = FirebaseStorage.DefaultInstance;
    //    StorageReference gsReference;
    //    try
    //    {
    //        gsReference = storage.GetReferenceFromUrl("gs://capy-s-journey-bc2f8.firebasestorage.app/Banners/IntroWorld/" + bannerName + ".png");
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Error getting storage reference: " + e.Message);
    //        return;
    //    }
    //    gsReference.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
    //    {
    //        if (!task.IsFaulted && !task.IsCanceled)
    //        {
    //            string url = task.Result.ToString();
    //            StartCoroutine(DownloadImage(url));
    //        }
    //        else
    //        {
    //            Debug.LogError("Failed to get download URL for banner image: " + bannerName);
    //        }
    //    });
    //}

    //private System.Collections.IEnumerator DownloadImage(string url)
    //{
    //    UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url);
    //    yield return request.SendWebRequest();

    //    if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
    //    {
    //        Texture2D texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(request);
    //        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    //        _banner.sprite = sprite;
    //    }
    //    else
    //    {
    //        Debug.LogError("Failed to download banner image: " + request.error);
    //    }
    //}

    private void LoadAudio()
    {
        _audioSource.Stop();
        int age = PlayerPrefs.GetInt("age", 10);
        string audioName = GameManager.GetAudioName(GetAgeGroup(age));
        AudioClip audioClip = Resources.Load<AudioClip>(audioName);
        if (audioClip == null)
        {
            Debug.LogError("Audio clip not found: " + audioName);
        }
        _audioSource.clip = audioClip;
        _durationInSeconds = audioClip.length;
        _maxTimeReached = _hasCompleted ? _durationInSeconds : 0;
        _playAudio = true;
        UnpauseAudio();
    }

    private AgeGroup GetAgeGroup(int age)
    {
        if (age <= 10)
        {
            return AgeGroup.Child;
        }
        else if (age <= 14)
        {
            return AgeGroup.YoungTeen;
        }
        else if (age <= 19)
        {
            return AgeGroup.OldTeen;
        }
        else
        {
            return AgeGroup.Adult;
        }
    }

    private void OnBackward()
    {
        _audioSource.time = Mathf.Max(0, _audioSource.time - 15);
        _slider.SetValueWithoutNotify(_audioSource.time);
        _timeElapsedText.text = FormatSeconds(_audioSource.time);
        UpdateForwardButtonState();
    }

    private void OnForward()
    {
        float targetTime = Mathf.Min(_durationInSeconds, _audioSource.time + 15);

        // Only allow forward if user has already reached that point or lesson is completed
        if (DebugMode || targetTime <= _maxTimeReached)
        {
            _slider.SetValueWithoutNotify(targetTime);
            _timeElapsedText.text = FormatSeconds(targetTime);
            if (!Mathf.Approximately(targetTime, _durationInSeconds))
            {
                _audioSource.time = targetTime;
            }
            else
            {
                OnDone();
            }
        }

        UpdateForwardButtonState();
    }

    private void OnPause()
    {
        _playAudio = !_playAudio;
        if (_playAudio)
        {
            _audioSource.Play();
            _pauseButtonImage.sprite = _unpaused;
        }
        else
        {
            _audioSource.Pause();
            _pauseButtonImage.sprite = _paused;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        if (value > _maxTimeReached)
        {
            _slider.SetValueWithoutNotify(_maxTimeReached);
        }
        _userIsDraggingSlider = value <= _maxTimeReached;
    }

    private void OnUserReleasedSlider()
    {
        if (!_userIsDraggingSlider) return;
        float sliderValue = _slider.value;
        if (sliderValue > _maxTimeReached)
        {
            _slider.SetValueWithoutNotify(_maxTimeReached);
        }
        else
        {
            _slider.SetValueWithoutNotify(sliderValue);
            if (!Mathf.Approximately(_audioSource.time, sliderValue))
            {
                if (Mathf.Approximately(sliderValue, _durationInSeconds))
                {
                    _timeElapsedText.text = FormatSeconds(_audioSource.time);
                    _userIsDraggingSlider = false;
                    OnDone();
                    return;
                }
                else
                {
                    _audioSource.time = sliderValue;
                }
            }
            _timeElapsedText.text = FormatSeconds(_audioSource.time);
        }
        _userIsDraggingSlider = false;
    }
    private void UpdateForwardButtonState()
    {
        if (DebugMode) return;
        float targetTime = Mathf.Min(_durationInSeconds, _audioSource.time + 15);
        bool canFastForward = targetTime <= _maxTimeReached;

        _forwardButton.interactable = canFastForward;

        // Darken the button when disabled
        Color buttonColor = _forwardButtonImage.color;
        buttonColor.a = canFastForward ? 1f : 0.5f;
        _forwardButtonImage.color = buttonColor;
    }
    void Update()
    {
        if (!_playAudio) return;
        if (_audioSource.time >= _durationInSeconds)
        {
            _timeElapsedText.text = FormatSeconds(_durationInSeconds);
            OnDone();
            return;
        }
        if (!_userIsDraggingSlider)
        {
            _slider.SetValueWithoutNotify(_audioSource.time);
        }
        _timeElapsedText.text = FormatSeconds(_audioSource.time);
        _maxTimeReached = Math.Max(_maxTimeReached, _audioSource.time);
        UpdateForwardButtonState();
    }

    private void PauseAudio()
    {
        _playAudio = false;
        _audioSource.Stop();
        _pauseButtonImage.sprite = _paused;
    }

    private void UnpauseAudio()
    {
        _playAudio = true;
        _audioSource.Play();
        _pauseButtonImage.sprite = _unpaused;
    }

    private void OnDone()
    {
        PauseAudio();
        Darken();
        _levelNumText.text = GameManager.GetCurrLevel().ShortName;
        GameManager.CompleteLevel(_durationInSeconds);
        _hasCompleted = true;
        if (GameManager.IsLevelBookmarked())
        {
            _bookmarkButtonImage.sprite = _filledBookmark;
        }
        else
        {
            _bookmarkButtonImage.sprite = _unfilledBookmark;
        }
        _levelCompletePopup.SetActive(true);
        UpdateForwardButtonState();
    }

    private void Redo()
    {
        _levelCompletePopup.SetActive(false);
        _maxTimeReached = 0;
        _playAudio = false;
        _audioSource.Stop();
        _audioSource.time = 0;
        _slider.SetValueWithoutNotify(0);
        Brighten();
        UnpauseAudio();
        UpdateForwardButtonState();
    }

    private void OnBookmarkButtonClicked()
    {
        GameManager.ToggleBookmark();
        bool bookmarked = GameManager.IsLevelBookmarked();
        _bookmarkButtonImage.sprite = bookmarked ? _filledBookmark : _unfilledBookmark;
        GameManager.Bookmark(bookmarked);
    }

    private string FormatSeconds(float time)
    {
        TimeSpan ts = TimeSpan.FromSeconds(time);
        return string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
    }
}
