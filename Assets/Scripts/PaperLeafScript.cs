using System;
using TMPro;
using UnityEngine;

public class PaperLeafScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private GameObject _paper;
    public void SetText(string text)
    {
        _text.text = text;
    }
    public void Hide()
    {
        _paper.SetActive(false);
    }
    public void Show()
    {
        _paper.SetActive(true);
    }
    public void SetOnClickListener(Action action)
    {
        _paper.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => action());
    }
}