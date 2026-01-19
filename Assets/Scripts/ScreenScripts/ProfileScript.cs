using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileScript : MonoBehaviour
{
    [SerializeField] private GameObject _profileView;

    [Header("Header")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _editProfileButton;
    [SerializeField] private Sprite _plusButtonSprite;

    [Header("Statistics")]
    [SerializeField] private TMP_Text _streakText;
    [SerializeField] private TMP_Text _bestStreakText;
    [SerializeField] private TMP_Text _themesText;
    [SerializeField] private TMP_Text _exercisesText;
    [SerializeField] private TMP_Text _minMeditationText;

    [Header("Achievements")]
    [SerializeField] private GameObject _achievementsBox;
    [SerializeField] private GameObject _badgePrefab;
    [SerializeField] private GameObject _displayedBadgesBox;


    [Header("Badge Selection")]
    [SerializeField] private GameObject _badgeSelection;
    [SerializeField] private GameObject _selectableBadges;
    [SerializeField] private Button _selectBadgeButton;
    [SerializeField] private GameObject _emptyState;
    [SerializeField] private Button _backButton;

    private TMP_Text _editProfileButtonText;
    private bool _isInEditMode = false;
    private BadgesDisplayed _badgesDisplayed;
    private Badge _newBadgeSelected;
    private BadgeScript _newBadgeSelectedScript;
    private TMP_Text _selectBadgeButtonText;

    private enum BadgePosition
    {
        First, Second, Third, None
    }
    private BadgePosition _badgePosition = BadgePosition.None;

    void Start()
    {
        _editProfileButtonText = _editProfileButton.GetComponentInChildren<TMP_Text>();
        _selectBadgeButtonText = _selectBadgeButton.GetComponentInChildren<TMP_Text>();
        _nameText.SetText(PlayerPrefs.GetString("username"));
        _badgesDisplayed = GameManager.GetBadgesDisplayed();
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
        PopulateDisplayedBadgesArea();
        PopulateAchievementsBox();
        ShowDefaultView();
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
        _editProfileButton.onClick.AddListener(OnEditButtonClicked);
        _selectBadgeButton.onClick.AddListener(OnSelectBadgeButtonClicked);
        _backButton.onClick.AddListener(ShowDefaultView);
    }


    private void PopulateDisplayedBadgesArea()
    {
        _badgesDisplayed = GameManager.GetBadgesDisplayed();
        foreach (Transform child in _displayedBadgesBox.transform)
        {
            Destroy(child.gameObject);
        }
        List<Badge> displayedBadges = _badgesDisplayed.GetBadges();
        foreach (Badge badge in displayedBadges)
        {
            GameObject badgeObj = new("Badge", typeof(RectTransform), typeof(Image));
            badgeObj.transform.SetParent(_displayedBadgesBox.transform);
            RectTransform rectTransform = badgeObj.GetComponent<RectTransform>();
            Image badgeImage = badgeObj.GetComponent<Image>();
            badgeImage.preserveAspect = true;
            Sprite badgeSprite = Resources.Load<Sprite>(badge.SpritePath);
            badgeImage.sprite = badgeSprite;
            rectTransform.sizeDelta = new Vector2(73, 73);
        }
    }


    private void PopulateAchievementsBox()
    {
        foreach (Transform child in _achievementsBox.transform)
        {
            Destroy(child.gameObject);
        }
        HashSet<Badge> badges = GameManager.GetBadgesOwned();
        foreach (Badge badge in Badge.BadgesInOrder)
        {
            if (badges.Contains(badge))
            {
                GameObject badgeObj = Instantiate(_badgePrefab, _achievementsBox.transform);
                BadgeScript badgeScript = badgeObj.GetComponent<BadgeScript>();
                badgeScript.SetBadge(badge);
                RectTransform rectTransform = badgeObj.GetComponent<RectTransform>();
            }
        }
    }

    private void PopulateSelectableBadges()
    {
        foreach (Transform child in _selectableBadges.transform)
        {
            Destroy(child.gameObject);
        }
        HashSet<Badge> badgesOwned = GameManager.GetBadgesOwned();
        bool noSelectableBadges = true;
        foreach (Badge badge in Badge.BadgesInOrder)
        {
            if (badgesOwned.Contains(badge) && !_badgesDisplayed.GetBadges().Contains(badge))
            {
                noSelectableBadges = false;
                GameObject badgeObj = Instantiate(_badgePrefab, _selectableBadges.transform);
                BadgeScript badgeScript = badgeObj.GetComponent<BadgeScript>();
                badgeScript.SetBadge(badge);
                badgeScript.SetOnBadgeClicked(() =>
                {
                    bool isSelected = badgeScript.GetIsSelected();
                    if (!isSelected)
                    {
                        if (_newBadgeSelectedScript != null)
                        {
                            _newBadgeSelectedScript.SetIsSelected(false);
                        }
                        _newBadgeSelectedScript = badgeScript;
                        _newBadgeSelected = badge;
                        badgeScript.SetIsSelected(true);
                        _selectBadgeButton.interactable = true;
                    }
                    else
                    {
                        _newBadgeSelectedScript = null;
                        _newBadgeSelected = null;
                        badgeScript.SetIsSelected(false);
                        _selectBadgeButton.interactable = false;
                    }
                });
            }
        }
        _emptyState.SetActive(noSelectableBadges);
        if (noSelectableBadges)
        {
            _selectBadgeButtonText.text = "Return";
        }
        else
        {
            _selectBadgeButtonText.text = "Select Badge";
        }
        // start with button disabled unless there are no selectable badges
        _selectBadgeButton.interactable = noSelectableBadges;
    }

    private void ShowDefaultView()
    {
        SetDisplayedBadgesToDisplayMode();
        _profileView.SetActive(true);
        _badgeSelection.SetActive(false);
    }

    private void ShowBadgeSelectionView()
    {
        _profileView.SetActive(false);
        _badgeSelection.SetActive(true);
        PopulateSelectableBadges();
    }

    private void OnEditButtonClicked()
    {
        if (_isInEditMode)
        {
            _isInEditMode = false;
            SetDisplayedBadgesToDisplayMode();
        }
        else
        {
            _isInEditMode = true;
            _editProfileButtonText.SetText("Save");
            int numBadges = _badgesDisplayed.NumBadgesDisplayed();
            int numPlusButtonsToAdd = 3 - numBadges;
            if (numPlusButtonsToAdd > 0)
            {
                for (int i = 0; i < numPlusButtonsToAdd; i++)
                {
                    GameObject plusButtonObj = new("PlusButton", typeof(RectTransform), typeof(Button), typeof(Image));
                    plusButtonObj.transform.SetParent(_displayedBadgesBox.transform);
                    Image plusImage = plusButtonObj.GetComponent<Image>();
                    plusImage.sprite = _plusButtonSprite;
                    plusImage.preserveAspect = true;
                    BadgePosition badgePosition = (numBadges + i) switch
                    {
                        0 => BadgePosition.First,
                        1 => BadgePosition.Second,
                        2 => BadgePosition.Third,
                        _ => BadgePosition.None
                    };
                    plusButtonObj.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            _badgePosition = badgePosition;
                            ShowBadgeSelectionView();
                        });
                    plusButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(24, 24);
                }
            }
        }
    }

    private void SetDisplayedBadgesToDisplayMode()
    {
        _isInEditMode = false;
        PopulateDisplayedBadgesArea();
        _editProfileButtonText.SetText("Edit Profile");
    }

    private void OnSelectBadgeButtonClicked()
    {
        if (_newBadgeSelected != null)
        {
            _badgesDisplayed.AddBadge(_newBadgeSelected);
            GameManager.SetBadgesDisplayed(_badgesDisplayed);
            _newBadgeSelected = null;
            _newBadgeSelectedScript = null;
            _selectBadgeButton.interactable = false;
        }
        ShowDefaultView();
    }
}
