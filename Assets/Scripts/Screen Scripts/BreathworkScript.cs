using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BreathworkScript : MonoBehaviour
{
    [SerializeField] private Button _backBtn;
    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _stopBtn;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private GameObject _introObjects;
    [SerializeField] private GameObject _breathworkObjects;
    [SerializeField] private GameObject _endObjects;
    [SerializeField] private TMP_Text _instructionText;
    [SerializeField] private Image _circle1;
    [SerializeField] private Image _circle2;
    [SerializeField] private Image _circle3;
    [SerializeField] private Image _circle4;
    [SerializeField] private Sprite _blueCircle;
    [SerializeField] private Sprite _yellowCircle;
    [SerializeField] private TMP_Text _carrotCount;
    [SerializeField] private Button _returnBtn;
    [SerializeField] private Image _centerCircle;
    private const int Duration = 300; // 5 minutes
    private bool _finished = false;
    private bool _meditating = false;
    private Image[] circles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        circles = new Image[] { _circle1, _circle2, _circle3, _circle4 };
        _backBtn.onClick.AddListener(ReturnToDaily);
        _startBtn.onClick.AddListener(StartBreathwork);
        _returnBtn.onClick.AddListener(ReturnToDaily);
        _stopBtn.onClick.AddListener(Initialize);
        _progressBar.maxValue = Duration;
        Initialize();
    }

    private void ReturnToDaily() => SceneManager.LoadSceneAsync("Daily");

    private void Initialize()
    {
        _introObjects.SetActive(true);
        _breathworkObjects.SetActive(false);
        _endObjects.SetActive(false);
        _meditating = false;
        _centerCircle.fillAmount = 0;

    }

    private void StartBreathwork()
    {
        _introObjects.SetActive(false);
        _progressBar.value = 0;
        _finished = false;
        _breathworkObjects.SetActive(true);
        _centerCircle.fillAmount = 0;
        _meditating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_finished || !_meditating) return;
        _progressBar.value += Time.deltaTime;
        _centerCircle.fillAmount = (_progressBar.value % 12) / 12;
        if (_progressBar.value >= Duration)
        {
            _finished = true;
            int carrotsEarned = GameManager.DidBreathworkToday() ? 0 : 10;
            _carrotCount.text = carrotsEarned.ToString();
            GameManager.IncreaseCarrots(carrotsEarned);
            GameManager.DoBreathwork();
            _endObjects.SetActive(true);
        }
        else
        {
            int mod4 = (int)(MathF.Floor(_progressBar.value)) % 4;
            for(int i=0;i<4;i++)
            {
                bool blue = i <= mod4;
                circles[i].sprite = blue ? _blueCircle : _yellowCircle;
            }
            int cycle = (int)(MathF.Floor(_progressBar.value / 4)) % 3;
            _instructionText.text = cycle switch
            {
                0 => "Breathe In",
                1 => "Hold",
                2 => "Breathe Out",
                _ => "Error"
            };
        }
    }
}
