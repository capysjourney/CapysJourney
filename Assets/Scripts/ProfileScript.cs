using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _editProfileButton;
    [SerializeField] private Button _viewAllButton;
    [SerializeField] private TMP_Text _streakText;
    [SerializeField] private TMP_Text _bestStreakText;
    [SerializeField] private TMP_Text _themesText;
    [SerializeField] private TMP_Text _exercisesText;
    [SerializeField] private TMP_Text _minMeditationText;

    void Start()
    {
        _nameText.SetText(PlayerPrefs.GetString("username"));
        try
        {
            _themesText.SetText($"{GameManager.GetNumWorldsCompleted()}/{GameManager.NumWorlds}");
            _exercisesText.SetText($"{GameManager.GetNumLessonsCompleted()}");
            _minMeditationText.SetText($"{MathF.Floor(GameManager.GetTotalMinutesMeditated() / 60)}");
            LocalizedString localizedStreakText = new("String Table", "streak") {
            { "streak", new StringVariable{ Value = GameManager.GetCurrStreak().ToString()} } };
            _streakText.SetText(localizedStreakText.GetLocalizedString());
            _bestStreakText.SetText($"Best: {GameManager.GetBestStreak()}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
        AddButtonListeners();
    }

    private void AddButtonListeners()
    {
        Button[] buttons = new Button[] { _settingsButton, _editProfileButton, _viewAllButton };
        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
        _settingsButton.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync("Settings");
        });
        _editProfileButton.onClick.AddListener(() =>
        {
            // todo - edit profile button onclick
        });
        _viewAllButton.onClick.AddListener(() =>
        {
            // todo - view all button onclick
        });
    }
}
