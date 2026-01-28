using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BadgeNotifScript : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _badgeName;
    [SerializeField] private Button _closeButton;

    public void SetBadge(Badge badge)
    {
        _badgeName.text = badge.Name;
        _image.sprite = Resources.Load<Sprite>(badge.SpritePath);
    }

    public void SetOnClose(Action action)
    {
        _closeButton.onClick.AddListener(() => action());
    }
}
