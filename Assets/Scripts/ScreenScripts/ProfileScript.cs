using NUnit.Framework;
using System;
using System.Collections.Generic;
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
    [SerializeField] private TMP_Text _streakText;
    [SerializeField] private TMP_Text _bestStreakText;
    [SerializeField] private TMP_Text _themesText;
    [SerializeField] private TMP_Text _exercisesText;
    [SerializeField] private TMP_Text _minMeditationText;
    [SerializeField] private GameObject _achievementsBox;
    [SerializeField] private GameObject _badgePrefab;

    void Start()
    {
        _nameText.SetText(PlayerPrefs.GetString("username"));
        try
        {
            _themesText.SetText($"{GameManager.GetNumWorldsCompleted()}/{GameManager.NumWorlds}");
            _exercisesText.SetText($"{GameManager.GetNumLessonsCompleted()}");
            _minMeditationText.SetText(GameManager.GetTotalMinutesMeditated().ToString());
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
        PopulateAchievementsBox();
    }

    private void AddButtonListeners()
    {
        Button[] buttons = new Button[] { _settingsButton, _editProfileButton };
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
    }

    private void PopulateAchievementsBox()
    {
        foreach (Transform child in _achievementsBox.transform)
        {
            Destroy(child.gameObject);
        }
        HashSet<Badge> badges = GameManager.GetBadgesOwned();
        foreach(Badge badge in badges)
        {
            Debug.Log("Owned badge: " + badge.Name);
        }
        foreach (Badge badge in Badge.BadgesInOrder)
        {
            if (badges.Contains(badge))
            {
                Debug.Log("Adding badge: " + badge.Name);
                GameObject badgeObj = Instantiate(_badgePrefab, _achievementsBox.transform);
                BadgeScript badgeScript = badgeObj.GetComponent<BadgeScript>();
                badgeScript.SetBadge(badge);
                RectTransform rectTransform = badgeObj.GetComponent<RectTransform>();
            }
        }
    }
}
