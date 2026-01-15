using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BadgeScript : MonoBehaviour
{
    [SerializeField] private Image _badgeImage;
    [SerializeField] private TMP_Text _badgeNameText;
    [SerializeField] private TMP_Text _badgeDescriptionText;

    public void SetBadge(Badge badge)
    {
        _badgeNameText.SetText(badge.Name);
        _badgeDescriptionText.SetText(badge.Description);
        _badgeImage.sprite = Resources.Load<Sprite>(badge.SpritePath);
    }
}