using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginInputBoxScript : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_InputField _inputTextField;
    [SerializeField] private Image _box;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _errorColor;

    private bool _isError = false;
    private Action _onResetErrorState;
    void Start()
    {
        SetErrorState(false);
        _inputTextField.onValueChanged.AddListener(delegate { _onResetErrorState?.Invoke(); });
    }

    public void SetErrorState(bool isError)
    {
        _isError = isError;
        Color color = _isError ? _errorColor : _defaultColor;
        _box.color = color;
        _label.color = color;
        _inputTextField.textComponent.color = color;
        _icon.color = color;
    }

    public void SetOnResetErrorState(Action onResetErrorState)
    {
        _onResetErrorState = onResetErrorState;
    }

}
