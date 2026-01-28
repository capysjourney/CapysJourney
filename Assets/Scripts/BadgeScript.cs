using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BadgeScript : MonoBehaviour
{
    [SerializeField] private Image _badgeImage;
    [SerializeField] private TMP_Text _badgeNameText;
    [SerializeField] private TMP_Text _badgeDescriptionText;
    [SerializeField] private Button _badgeButton;

    private bool _isSelected = false;
    private readonly Color HiddenColor = new(0f, 0f, 0f, 0f);

    public void Start()
    {
        SetIsSelected(false);
    }

    public void SetBadge(Badge badge)
    {
        _badgeNameText.SetText(badge.Name);
        _badgeDescriptionText.SetText(badge.Description);
        _badgeImage.sprite = Resources.Load<Sprite>(badge.SpritePath);
    }

    public void SetOnBadgeClicked(Action action)
    {
        _badgeButton.onClick.AddListener(() => action());
    }

    public void SetIsSelected(bool isSelected)
    {
        _isSelected = isSelected;
        gameObject.GetComponent<Image>().color = isSelected ? Color.white : HiddenColor;
    }

    public bool GetIsSelected()
    {
        return _isSelected;
    }

}