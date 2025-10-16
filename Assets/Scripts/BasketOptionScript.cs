using System;
using UnityEngine;
using UnityEngine.UI;

public class BasketOptionScript : MonoBehaviour
{
    [SerializeField] private Button _basketImage;
    [SerializeField] private Button _sign;

    public void SetOnClickListener(Action OnClick)
    {
        _basketImage.onClick.AddListener(() => OnClick());
        _sign.onClick.AddListener(() => OnClick());
    }
}
