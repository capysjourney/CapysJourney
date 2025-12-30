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

    [SerializeField] private AudioClip rainAudio;
    [SerializeField] private AudioClip oceanAudio;
    [SerializeField] private AudioClip riverAudio;
    [SerializeField] private AudioClip fireAudio;

    [SerializeField] private AudioClip bell;
    [SerializeField] private AudioClip bell1;
    [SerializeField] private AudioClip bell2;
    [SerializeField] private AudioClip bell3;

    private UnityEngine.UI.Image _pauseButtonImage;
    private float _timeElapsed = 0;
    private float _duration;
    private bool _playAudio = false;
    private AudioClip _chimeClip;
    private bool _isDraggingSlider = false;

    void Start()
    {
        UnityEngine.Debug.Log("Start method called");

        _pauseButtonImage = _pauseButton.GetComponent<UnityEngine.UI.Image>();

        UnityEngine.Debug.Log("AudioSource null? " + (_audioSource == null));

        LoadAudio();
        _slider.maxValue = _duration;
        TimeSpan durationTS = TimeSpan.FromSeconds(_duration);
        _durationText.text = FormatSeconds(_duration);
        _backwardButton.onClick.AddListener(OnBackward);
        _forwardButton.onClick.AddListener(OnForward);
        _pauseButton.onClick.AddListener(OnPause);
        _audioSource.volume = 1.0f;
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
        UnityEngine.Debug.Log("LoadAudio called");

        _audioSource.Stop();
        _duration = SharedData.duration * 60f;
        float interval = SharedData.interval * 60f;
        string chimeName = SharedData.chime;

        UnityEngine.Debug.Log("Effect: " + SharedData.effectData);
        UnityEngine.Debug.Log("Duration: " + _duration);
        UnityEngine.Debug.Log("Interval: " + interval);
        UnityEngine.Debug.Log("Chime: " + chimeName);
        UnityEngine.Debug.Log("Rain clip null? " + (rainAudio == null));
        UnityEngine.Debug.Log("Ocean clip null? " + (oceanAudio == null));
        UnityEngine.Debug.Log("River clip null? " + (riverAudio == null));
        UnityEngine.Debug.Log("Fire clip null? " + (fireAudio == null));

        if (SharedData.effectData.Contains("Rain"))
        {
            _audioSource.clip = rainAudio;
            UnityEngine.Debug.Log("Set Rain audio");
        }
        else if (SharedData.effectData.Contains("Ocean"))
        {
            _audioSource.clip = oceanAudio;
            UnityEngine.Debug.Log("Set Ocean audio");
        }
        else if (SharedData.effectData.Contains("River"))
        {
            _audioSource.clip = riverAudio;
            UnityEngine.Debug.Log("Set River audio");
        }
        else if (SharedData.effectData.Contains("Fire"))
        {
            _audioSource.clip = fireAudio;
            UnityEngine.Debug.Log("Set Fire audio");
        }

        UnityEngine.Debug.Log("Audio Clip assigned? " + (_audioSource.clip != null));
        if (_audioSource.clip != null)
        {
            UnityEngine.Debug.Log("Audio Clip name: " + _audioSource.clip.name);
        }

        if (chimeName.Contains("Bell 1"))
        {
            _chimeClip = bell;
        }
        else if (chimeName.Contains("Bell 2"))
        {
            _chimeClip = bell1;
        }
        else if (chimeName.Contains("Bell 3"))
        {
            _chimeClip = bell2;
        }
        else if (chimeName.Contains("Bell"))
        {
            _chimeClip = bell3;
        }

        UnityEngine.Debug.Log("Chime clip assigned? " + (_chimeClip != null));

        if (_audioSource.clip != null)
        {
            _audioSource.loop = true;
            _audioSource.Play();
            UnityEngine.Debug.Log("Audio playing? " + _audioSource.isPlaying);
            UnityEngine.Debug.Log("Audio volume: " + _audioSource.volume);
        }
        else
        {
            UnityEngine.Debug.LogError("AudioSource.clip is null! Cannot play audio.");
        }

        _slider.maxValue = _duration;
        _durationText.text = FormatSeconds(_duration);
        _playAudio = true;

        if (_chimeClip != null && !string.IsNullOrEmpty(chimeName) && chimeName != "None" && interval > 0)
        {
            UnityEngine.Debug.Log("Starting chime coroutine");
            StartCoroutine(PlayChimeAtIntervals(_duration, interval));
        }
        else
        {
            UnityEngine.Debug.Log("NOT starting chime coroutine - chimeClip null? " + (_chimeClip == null) + ", chimeName: " + chimeName + ", interval: " + interval);
        }
    }

    private IEnumerator PlayChimeAtIntervals(float duration, float interval)
    {
        UnityEngine.Debug.Log("Chime coroutine started");
        float nextChimeTime = interval;

        while (_timeElapsed < duration)
        {
            if (_playAudio && _timeElapsed >= nextChimeTime)
            {
                UnityEngine.Debug.Log("Playing chime at time: " + _timeElapsed);
                _audioSource.PlayOneShot(_chimeClip);
                nextChimeTime += interval;
            }

            yield return null;
        }

        UnityEngine.Debug.Log("Chime coroutine ended");
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
        UnityEngine.Debug.Log("Pause toggled. _playAudio: " + _playAudio);
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
        UnityEngine.Debug.Log("PauseAudio called");
        _playAudio = false;
        _audioSource.Pause();
        _pauseButtonImage.sprite = _paused;
    }

    private void UnpauseAudio()
    {
        UnityEngine.Debug.Log("UnpauseAudio called");
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
        UnityEngine.Debug.Log("OnDone called");
        PauseAudio();
        Darken();
    }

    private void Redo()
    {
        UnityEngine.Debug.Log("Redo called");
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