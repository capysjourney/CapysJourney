using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgeInputScript : MonoBehaviour
{
    private int _age = 10;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _ageText;
    [SerializeField] private Image _sliderHandle;
    [SerializeField] private Button _plusButton;
    [SerializeField] private Button _minusButton;
    [SerializeField] private Sprite _capyChild;
    [SerializeField] private Sprite _capyTeen;
    [SerializeField] private Sprite _capyAdult;

    void Start()
    {
        _age = PlayerPrefs.GetInt("age", 10);
        UpdateAge(_age);
        _slider.onValueChanged.RemoveAllListeners();
        _plusButton.onClick.RemoveAllListeners();
        _minusButton.onClick.RemoveAllListeners();
        _slider.onValueChanged.AddListener((v) => UpdateAge(v));
        _plusButton.onClick.AddListener(() => UpdateAge(_age + 1));
        _minusButton.onClick.AddListener(() => UpdateAge(_age - 1));
    }

    private void UpdateAge(float newAge)
    {
        _age = (int)Math.Clamp(newAge, 0, 100);
        _slider.value = _age;
        _ageText.SetText(_age.ToString());
        if (_age <= 10)
        {
            _sliderHandle.sprite = _capyChild;
        }
        else if (_age <= 19)
        {
            _sliderHandle.sprite = _capyTeen;
        }
        else
        {
            _sliderHandle.sprite = _capyAdult;
        }
        PlayerPrefs.SetInt("age", _age);
    }

}
