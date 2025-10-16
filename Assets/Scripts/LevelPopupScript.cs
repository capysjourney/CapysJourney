using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPopupScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _levelNameText;
    [SerializeField] private TMP_Text _levelDescriptionText;
    [SerializeField] private TMP_Text _numCarrotsText;
    [SerializeField] private Button startButton;
    public void UpdatePopup(string level, string levelName, string levelDescription, int numCarrots)
    {
        _levelText.text = level;
        _levelNameText.text = levelName;
        _levelDescriptionText.text = levelDescription;  
        _numCarrotsText.text = numCarrots.ToString();
    }

    public void ConfigureStartButton(Action listener)
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => listener());
    }
}
