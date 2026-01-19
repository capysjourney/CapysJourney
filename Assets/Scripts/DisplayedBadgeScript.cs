using System;
using UnityEngine;
using UnityEngine.UI;

public class DisplayedBadgeScript : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Image image;

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void ShowCloseButton()
    {
        _closeButton.gameObject.SetActive(true);
    }

    public void HideCloseButton()
    {
        _closeButton.gameObject.SetActive(false);
    }

    public void SetOnClose(Action action)
    {
        _closeButton.onClick.AddListener(() => action());
    }

}
