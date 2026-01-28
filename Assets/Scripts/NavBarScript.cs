using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavBarScript : MonoBehaviour
{
    [SerializeField] private Button _journeyTab;
    [SerializeField] private Button _dailyTab;
    [SerializeField] private Button _denTab;
    [SerializeField] private Button _profileTab;
    [SerializeField] private Button _dojoButton;
    [SerializeField] private Image _carrot;
    [SerializeField] private TMP_Text _carrotCount;
    [SerializeField] private TMP_Text _worldLabel;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _centerText;

    private Scene _currScene = Scene.Journey;

    // for tutorial use
    private bool _isForTutorial = false;
    public enum Scene
    {
        Journey,
        Daily,
        Den,
        Profile,
        Dojo
    }

    void Start()
    {
        if (_isForTutorial)
        {
            _currScene = Scene.Journey;
        }
        else
        {
            _currScene = SceneOf(SceneManager.GetActiveScene().name);
        }
        ConfigureButtons();
        InitializeContent();
    }

    public void ChangeLevel(Level level)
    {
        _level.SetText(level.ShortName);
        _worldLabel.SetText(level.World.Name);
    }

    public void SetIsForTutorial(bool isForTutorial)
    {
        _isForTutorial = isForTutorial;
    }

    public void ChangeScene(Scene newScene)
    {
        if (_isForTutorial)
        {
            _currScene = newScene;
            ConfigureButtons();
        }
    }

    /// <summary>
    /// Configures the order, appearance, and click behavior of navigation buttons based on the current scene.
    /// </summary>
    private void ConfigureButtons()
    {
        Button[] buttons = new Button[] { _journeyTab, _dailyTab, _denTab, _profileTab, _dojoButton };
        ResetOrder();
        Button currButton = ButtonOf(_currScene);
        if (_currScene != Scene.Profile)
        {
            currButton.transform.SetAsLastSibling(); // put on top
        }
        currButton.GetComponent<Image>().color = Color.white;
        foreach (Scene scene in Enum.GetValues(typeof(Scene)))
        {
            Button button = ButtonOf(scene);
            if (button != currButton && button != _dojoButton)
            {
                button.GetComponent<Image>().color = Color.gray8;
            }
            button.onClick.RemoveAllListeners();
            if (button != currButton)
            {
                if (!_isForTutorial)
                {
                    button.onClick.AddListener(() => SceneManager.LoadSceneAsync(scene.ToString()));
                }
            }
        }
    }

    private void InitializeContent()
    {
        if (_currScene == Scene.Journey)
        {
            JourneyContent();
        }
        else if (_currScene == Scene.Dojo)
        {
            // Dojo should not have a nav bar
            throw new InvalidOperationException();
        }
        else
        {
            string label = _currScene switch
            {
                Scene.Daily => "Daily",
                Scene.Den => "Capy's Den",
                Scene.Profile => "Profile",
                _ => ""
            };
            if (LocalizationSettings.SelectedLocale == null)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
            }
            _centerText.SetText(new LocalizedString("String Table", label).GetLocalizedString());
            ToggleLabels(false);
        }
        _carrotCount.SetText($"{CarrotManager.GetNumCarrots()}");
    }

    private void JourneyContent()
    {
        ToggleLabels(true);
        Level currLevel = GameManager.GetCurrLevel();
        _level.SetText(currLevel.ShortName);
        _worldLabel.SetText(currLevel.World.Name);
    }

    private void ToggleLabels(bool isJourney)
    {
        _worldLabel.gameObject.SetActive(isJourney);
        _level.gameObject.SetActive(isJourney);
        _centerText.gameObject.SetActive(!isJourney);
    }

    private Scene SceneOf(string sceneName)
    {
        return sceneName switch
        {
            "Journey" => Scene.Journey,
            "Daily" => Scene.Daily,
            "Den" => Scene.Den,
            "Profile" => Scene.Profile,
            "Dojo" => Scene.Dojo,
            _ => Scene.Journey,
        };
    }

    private Button ButtonOf(Scene scene)
    {
        return scene switch
        {
            Scene.Profile => _profileTab,
            Scene.Journey => _journeyTab,
            Scene.Den => _denTab,
            Scene.Daily => _dailyTab,
            Scene.Dojo => _dojoButton,
            _ => _journeyTab
        };
    }

    private void ResetOrder()
    {
        _profileTab.transform.SetSiblingIndex(0);
        _denTab.transform.SetSiblingIndex(1);
        _dailyTab.transform.SetSiblingIndex(2);
        _journeyTab.transform.SetSiblingIndex(4);
    }
}
