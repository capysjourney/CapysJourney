using System;
using System.Diagnostics;
using System.Collections;

using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using static System.Net.Mime.MediaTypeNames;

public class CustomMeditationController : MonoBehaviour
{
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
    [SerializeField] private UnityEngine.UI.Image _banner;
    [SerializeField] private GameObject _darkener;
    [SerializeField] private Button _backArrow;
    [SerializeField] private TMP_Text _levelNumText;
    [SerializeField] private GameObject _levelCompletePopup;
    [SerializeField] private TMP_Text carrotCount;

    [SerializeField] private AudioClip rainAudio;
    [SerializeField] private AudioClip oceanAudio;
    [SerializeField] private AudioClip riverAudio;
    [SerializeField] private AudioClip fireAudio;

    [SerializeField] private AudioClip bell;
    [SerializeField] private AudioClip bell1;
    [SerializeField] private AudioClip bell2;

    private UnityEngine.UI.Image _pauseButtonImage;
    private float _timeElapsed = 0;
    private float _duration;
    private bool _playAudio = false;
    private AudioClip _chimeClip;
    private bool _isDraggingSlider = false;

    void Start()
    {
        _pauseButtonImage = _pauseButton.GetComponent<UnityEngine.UI.Image>();

        LoadAudio();
        _slider.maxValue = _duration;
        TimeSpan durationTS = TimeSpan.FromSeconds(_duration);
        _durationText.text = FormatSeconds(_duration);
        _backwardButton.onClick.AddListener(OnBackward);
        _forwardButton.onClick.AddListener(OnForward);
        _pauseButton.onClick.AddListener(OnPause);
        _audioSource.volume = 1.0f;
        _levelCompletePopup.SetActive(false);
        _backArrow.onClick.AddListener(ReturnToJourney);

        EventTrigger.Entry pointerDownEntry = new()
        {
            eventID = EventTriggerType.PointerDown
        };
        pointerDownEntry.callback.AddListener((eventData) =>
        {
            _isDraggingSlider = true;
        });
        _sliderTrigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new()
        {
            eventID = EventTriggerType.PointerUp
        };
        pointerUpEntry.callback.AddListener((eventData) =>
        {
            _isDraggingSlider = false;
            UpdateAudio();
        });
        _sliderTrigger.triggers.Add(pointerUpEntry);

        Brighten();
    }

    private void ReturnToJourney() => SceneManager.LoadSceneAsync("Dojo");

    private void Darken() => _darkener.SetActive(true);

    private void Brighten() => _darkener.SetActive(false);

    private void LoadAudio()
    {
        _audioSource.Stop();
        _duration = SharedData.duration * 60f;
        float interval = SharedData.interval * 60f;
        string chimeName = SharedData.chime;

        if (SharedData.effectData.Contains("Rain"))
        {
            _audioSource.clip = rainAudio;
        }
        else if (SharedData.effectData.Contains("Ocean"))
        {
            _audioSource.clip = oceanAudio;
        }
        else if (SharedData.effectData.Contains("River"))
        {
            _audioSource.clip = riverAudio;
        }
        else if (SharedData.effectData.Contains("Fire"))
        {
            _audioSource.clip = fireAudio;
        }

        if (chimeName.Contains("Classic Bell"))
        {
            _chimeClip = bell;
        }
        else if (chimeName.Contains("Echo Bell"))
        {
            _chimeClip = bell1;
        }
        else if (chimeName.Contains("Mellow Bell"))
        {
            _chimeClip = bell2;
        }


        if (_audioSource.clip != null)
        {
            _audioSource.loop = true;
            _audioSource.Play();
        }

        _slider.maxValue = _duration;
        _durationText.text = FormatSeconds(_duration);
        _playAudio = true;

        if (_chimeClip != null && !string.IsNullOrEmpty(chimeName) && chimeName != "None" && interval > 0)
        {
            StartCoroutine(PlayChimeAtIntervals(_duration, interval));
        }
    }

    private IEnumerator PlayChimeAtIntervals(float duration, float interval)
    {
        float nextChimeTime = interval;

        while (_timeElapsed < duration)
        {
            if (_playAudio && _timeElapsed >= nextChimeTime)
            {
                _audioSource.PlayOneShot(_chimeClip);
                nextChimeTime += interval;
            }

            yield return null;
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
            if (_audioSource.isPlaying)
            {
                _audioSource.UnPause();
            }
            else
            {
                _audioSource.Play();
            }
            _pauseButtonImage.sprite = _unpaused;
        }
        else
        {
            _audioSource.Pause();
            _pauseButtonImage.sprite = _paused;
        }
    }

    private void PauseAudio()
    {
        _playAudio = false;
        _audioSource.Pause();
        _pauseButtonImage.sprite = _paused;
    }

    private void UnpauseAudio()
    {
        _playAudio = true;
        if (_audioSource.clip != null)
        {
            _audioSource.Play();
        }
        _pauseButtonImage.sprite = _unpaused;
    }

    private void UpdateAudio()
    {
        _timeElapsed = _slider.value;

        if (_slider.value >= _duration)
        {
            PauseAudio();
        }
        else
        {
            if (_audioSource != null && _audioSource.clip != null)
            {
                _audioSource.time = _slider.value % _audioSource.clip.length;
            }
        }
    }


    void Update()
    {
        if (_playAudio && !_isDraggingSlider)
        {
            UpdateTimeElapsed();
        }

        if (_isDraggingSlider)
        {
            _timeElapsed = _slider.value;
        }

        _timeElapsedText.text = FormatSeconds(_timeElapsed);
    }

    private void UpdateTimeElapsed()
    {
        if (_timeElapsed < _duration)
        {
            _timeElapsed = Math.Min(_duration, _timeElapsed + Time.deltaTime);
            _slider.value = _timeElapsed;
            _timeElapsedText.text = FormatSeconds(_timeElapsed);
        }
        else
        {
            _timeElapsed = _duration;
            _timeElapsedText.text = FormatSeconds(_timeElapsed);
            OnDone();
        }
    }

    private void OnDone()
    {

        int carrotsEarned = Mathf.RoundToInt(10.1f * Mathf.Log(1 + 0.36f * _duration));
        _levelCompletePopup.SetActive(true);
        carrotCount.text = carrotsEarned + "";
        DataManager.WithStats(stats =>
        {
            stats.IncreaseCarrots(carrotsEarned, GameManager.HandleBadgesEarned);
        }, true); 

        PauseAudio();
        Darken();
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
        LoadAudio();
    }

    private string FormatSeconds(float time)
    {
        TimeSpan ts = TimeSpan.FromSeconds(time);
        return string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
    }
}