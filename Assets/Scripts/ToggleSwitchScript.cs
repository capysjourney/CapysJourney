using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitchScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Color _enabledColor;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private Image _background;
    private bool _isEnabled = false;
    private Slider _slider;
    private const float AnimationDuration = 0.25f;
    private Coroutine _animateSliderCoroutine;
    private Action _onToggleOn = () => { };
    private Action _onToggleOff = () => { };

    protected virtual void OnValidate()
    {
        if (_slider == null)
            SetupSliderComponent();
    }

    private void SetupSliderComponent()
    {
        _slider = GetComponent<Slider>();
        _slider.interactable = false;
        var sliderColors = _slider.colors;
        sliderColors.disabledColor = Color.white;
        _slider.colors = sliderColors;
        _slider.transition = Selectable.Transition.None;
    }
    protected virtual void Awake()
    {
        SetupSliderComponent();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isEnabled = !_isEnabled;
        if (_isEnabled)
            _onToggleOn();
        else
            _onToggleOff();
        if (_animateSliderCoroutine != null)
            StopCoroutine(_animateSliderCoroutine);

        _animateSliderCoroutine = StartCoroutine(AnimateSlider());
    }
    public void SetSwitch(bool enabled)
    {
        _isEnabled = enabled;
        _slider.value = _isEnabled ? 1 : 0;
        _background.color = _isEnabled ? _enabledColor : _disabledColor;
    }

    public void SetOnToggleOn(Action action)
    {
        _onToggleOn = action;
    }

    public void SetOnToggleOff(Action action)
    {
        _onToggleOff = action;
    }

    /// <summary>
    /// Coroutine that animates the slider transition. Assumes _isEnabled has been updated.
    /// </summary>
    private IEnumerator AnimateSlider()
    {
        float startValue = _slider.value;
        float endValue = _isEnabled ? 1 : 0;
        Color startColor = _background.color;
        Color endColor = _isEnabled ? _enabledColor : _disabledColor;
        float time = 0;
        while (time < AnimationDuration)
        {
            time += Time.deltaTime;
            AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
            float lerpFactor = slideEase.Evaluate(time / AnimationDuration);
            _slider.value = Mathf.Lerp(startValue, endValue, lerpFactor);
            _background.color = Color.Lerp(startColor, endColor, lerpFactor);
            yield return null;
        }
        _slider.value = endValue;
    }

}