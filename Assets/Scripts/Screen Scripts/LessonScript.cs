using System;
using TMPro;
using UnityEditor;
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
    private float _timeElapsed = 0;

    /// <summary>
    /// Duration of the audio clip in seconds.
    /// </summary>
    private float _duration;

    private bool _playAudio = false;

    void Start()
    {
        _pauseButtonImage = _pauseButton.GetComponent<Image>();
        _bookmarkButtonImage = _bookmarkButton.GetComponent<Image>();
        _title.text = GameManager.GetCurrLevel().Name;
        LoadImage();
        LoadAudio();
        _slider.maxValue = _duration;
        TimeSpan durationTS = TimeSpan.FromSeconds(_duration);
        _durationText.text = FormatSeconds(_duration);
        _backwardButton.onClick.AddListener(OnBackward);
        _forwardButton.onClick.AddListener(OnForward);
        _pauseButton.onClick.AddListener(OnPause);
        _redoButton.onClick.AddListener(Redo);
        _audioSource.volume = 1.0f;
        _backArrow.onClick.AddListener(ReturnToJourney);
        _homeButton.onClick.AddListener(ReturnToJourney);
        _bookmarkButton.onClick.AddListener(OnBookmarkButtonClicked);
        EventTrigger.Entry entry = new()
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener((eventData) =>
        {
            UpdateAudio();
        });
        _sliderTrigger.triggers.Add(entry);
        Brighten();
        _levelCompletePopup.SetActive(false);
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
        _duration = audioClip.length;
        _playAudio = true;
        UnpauseAudio();
    }

    private AgeGroup GetAgeGroup(int age)
    {
        if (age <= 10)
        {
            return AgeGroup.Child;
        } else if (age <= 14)
        {
            return AgeGroup.YoungTeen;
        } else if (age <= 19)
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
        _timeElapsed = Mathf.Max(0, _timeElapsed - 15);
        _slider.value = _timeElapsed;
        _timeElapsedText.text = FormatSeconds(_timeElapsed);
        UpdateAudio();
    }

    private void OnForward()
    {
        _timeElapsed = Mathf.Min(_duration, _timeElapsed + 15);
        _slider.value = _timeElapsed;
        _timeElapsedText.text = FormatSeconds(_timeElapsed);
        UpdateAudio();
        if (_timeElapsed == _duration)
        {
            OnDone();
        }
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

    private void UpdateAudio()
    {
        if (_slider.value >= _duration)
        {
            PauseAudio();
        }
        else
        {
            _audioSource.time = _slider.value;
        }
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

    void Update()
    {
        if (_playAudio)
        {
            UpdateTimeElapsed();
        }
        _timeElapsedText.text = FormatSeconds(_timeElapsed);
    }

    private void UpdateTimeElapsed()
    {
        float newTimeElapsed = _slider.value;
        if (newTimeElapsed < _duration)
        {
            _timeElapsed = Math.Min(_duration, newTimeElapsed + Time.deltaTime);
            _slider.value = _timeElapsed;
            _timeElapsedText.text = FormatSeconds(_timeElapsed);
        }
        else
        {
            _timeElapsed = newTimeElapsed;
            _timeElapsedText.text = FormatSeconds(_timeElapsed);
            OnDone();
        }
    }

    private void OnDone()
    {
        PauseAudio();
        Darken();
        _levelNumText.text = GameManager.GetCurrLevel().ShortName;
        GameManager.CompleteLevel(_duration);
        if (GameManager.IsLevelBookmarked())
        {
            _bookmarkButtonImage.sprite = _filledBookmark;
        }
        else
        {
            _bookmarkButtonImage.sprite = _unfilledBookmark;
        }
        _levelCompletePopup.SetActive(true);
    }

    private void Redo()
    {
        _levelCompletePopup.SetActive(false);
        _timeElapsed = 0;
        _playAudio = false;
        _audioSource.Stop();
        _audioSource.time = 0;
        _slider.value = 0f;
        Brighten();
        UnpauseAudio();
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
