using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectLanguagesScript : MonoBehaviour
{
    private enum Stage
    {
        SELECT_LANGUAGE,
        CAPY_INTRO,
        INPUT_NAME,
        INPUT_AGE,
        INPUT_APPEARANCE,
        TRANSITION
    }
    private Stage _currStage = Stage.SELECT_LANGUAGE;

    private readonly string[] _languages = { "English", "Spanish" };
    private readonly string[] _locales = { "English (en)", "Spanish (es)" };
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _backButton;
    [SerializeField] private GameObject _languageSelection;
    [SerializeField] private GameObject _bigCapy;
    [SerializeField] private Image _progressBar;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private GameObject _questionHeader;
    [SerializeField] private GameObject _textInputArea;
    [SerializeField] private TMP_Text _inputLabel;
    [SerializeField] private TMP_Text _placeholder;
    [SerializeField] private GameObject _ageInputArea;
    [SerializeField] private GameObject _appearanceInputArea;
    [SerializeField] private GameObject _bigSpeechBubble1;
    [SerializeField] private GameObject _bigSpeechBubble2;
    private const string _askName = "What is your name?";
    private const string _askAge = "How old are you?";
    private const string _askAppearance = "Pick an appearance!";
    private const string _nameLabel = "Name:";
    private const string _ageLabel = "Age:";
    private const string _appearanceLabel = "Appearance:";
    private const string _namePlaceholder = "Enter name...";
    [SerializeField] private Sprite _noProgress;
    [SerializeField] private Sprite _halfProgress;
    [SerializeField] private Sprite _fullProgress;
    private GameObject[] _uiElements;

    void Start()
    {
        _uiElements = new GameObject[] { _languageSelection, _bigCapy, _continueButton, _backButton, _questionHeader, _textInputArea, _ageInputArea, _appearanceInputArea };
        //LoadSelectLanguage(); 
        // todo - uncomment above and remove below
        LoadCapyIntro();
        AddContinueBtnListener();
        AddBackBtnListener();
    }

    private void AddContinueBtnListener()
    {
        _continueButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            switch (_currStage)
            {
                //case Stage.SELECT_LANGUAGE: todo - uncomment
                //    LoadCapyIntro();
                //    break;
                case Stage.CAPY_INTRO:
                    LoadInputName();
                    break;
                case Stage.INPUT_NAME:
                    PlayerPrefs.SetString("username", _textInputArea.GetComponentInChildren<TMP_InputField>().text);
                    LoadInputAge();
                    break;
                case Stage.INPUT_AGE:
                    //LoadInputAppearance();
                    // todo - uncomment
                    LoadTransition();
                    break;
                // todo - uncomment
                //case Stage.INPUT_APPEARANCE:
                //    LoadTransition();
                //    break;
                case Stage.TRANSITION:
                    CreatePlayerStats();
                    SceneManager.LoadSceneAsync("Journey");
                    // todo - add transition to tutorial instead of journey
                    //SceneManager.LoadSceneAsync("Tutorial");
                    break;
                default:
                    break;
            }
        });
    }
    private void AddBackBtnListener()
    {
        _backButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            switch (_currStage)
            {
                case Stage.CAPY_INTRO:
                    LoadSelectLanguage();
                    break;
                case Stage.INPUT_NAME:
                    PlayerPrefs.SetString("username", _textInputArea.GetComponentInChildren<TMP_InputField>().text);
                    LoadCapyIntro();
                    break;
                case Stage.INPUT_AGE:
                    LoadInputName();
                    break;
                case Stage.INPUT_APPEARANCE:
                    LoadInputAge();
                    break;
                case Stage.TRANSITION:
                    //LoadInputAppearance(); //todo - uncomment
                    LoadInputAge(); // todo - remove
                    break;
                default: break;
            }
        });
    }

    private void LoadSelectLanguage()
    {
        _currStage = Stage.SELECT_LANGUAGE;
        HideAllStages();
        Show(_languageSelection);
        Show(_continueButton);
        GameObject[] languageBoxes = GameObject.FindGameObjectsWithTag("Language");
        foreach (GameObject languageBox in languageBoxes)
        {
            GetScript(languageBox).SetOnClickListener(() => SelectLanguage(languageBox.name, languageBoxes));
        }
        int selectedIdx = Array.IndexOf(_locales, LocalizationSettings.SelectedLocale.ToString());
        SelectLanguage(_languages[selectedIdx], languageBoxes);
    }

    private void SelectLanguage(string languageName, GameObject[] languageBoxes)
    {
        foreach (GameObject languageBox in languageBoxes)
        {
            GetScript(languageBox).SetSelected(languageBox.name == languageName);
        }
        int idx = Array.IndexOf(_languages, languageName);
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[idx];
    }
    private ToggleLanguageScript GetScript(GameObject go)
    {
        return go.transform.GetComponentInChildren<ToggleLanguageScript>();
    }

    private void LoadCapyIntro()
    {
        _currStage = Stage.CAPY_INTRO;
        HideAllStages();
        _bigSpeechBubble2.SetActive(false);
        Show(_bigCapy);
        Show(_continueButton);
        //Show(_backButton);
        Show(_bigSpeechBubble1);
    }

    private void LoadInputName()
    {
        _currStage = Stage.INPUT_NAME;
        HideAllStages();
        _progressBar.sprite = _noProgress;
        SetText(_questionText, _askName);
        SetText(_inputLabel, _nameLabel);
        SetText(_placeholder, _namePlaceholder);
        _textInputArea.GetComponentInChildren<TMP_InputField>().text = PlayerPrefs.GetString("username", "");
        Show(_continueButton);
        Show(_questionHeader);
        Show(_textInputArea);
        Show(_backButton);
    }

    private void LoadInputAge()
    {
        _currStage = Stage.INPUT_AGE;
        HideAllStages();
        _progressBar.sprite = _halfProgress;
        SetText(_questionText, _askAge);
        SetText(_inputLabel, _ageLabel);
        Show(_continueButton);
        Show(_questionHeader);
        Show(_backButton);
        Show(_ageInputArea);
    }

    private void LoadInputAppearance()
    {
        HideAllStages();
        _currStage = Stage.INPUT_APPEARANCE;
        _progressBar.sprite = _fullProgress;
        SetText(_questionText, _askAppearance);
        SetText(_inputLabel, _appearanceLabel);
        Show(_questionHeader);
        Show(_continueButton);
        Show(_backButton);
        Show(_appearanceInputArea);
    }

    private void LoadTransition()
    {
        _currStage = Stage.TRANSITION;
        HideAllStages();
        LocalizedString transitionText = new("String Table", "transition text") {
            { "username", new StringVariable { Value = PlayerPrefs.GetString("username") } }
        };
        _bigSpeechBubble2.GetComponentInChildren<TMP_Text>().SetText(transitionText.GetLocalizedString());
        _bigSpeechBubble1.SetActive(false);
        Show(_bigCapy);
        Show(_continueButton);
        Show(_backButton);
        Show(_bigSpeechBubble2);
    }

    private void CreatePlayerStats()
    {
        IDataService DataService = new JsonDataService();
        bool worked = DataService.SaveData("player-stats.json", new PlayerStats());
        if (!worked)
        {
            Debug.LogError("Could not save file!");
            return;
        }

        // Track user registration with PostHog
        string username = PlayerPrefs.GetString("username", "");
        int age = PlayerPrefs.GetInt("age", 0);
        
        // Ensure PostHogManager is initialized
        PostHogManager.Instance.Initialize();
        string distinctId = PostHogManager.Instance.DistinctId;
        
        PostHogManager.Instance.Identify(distinctId, new Dictionary<string, object>
        {
            { "username", username },
            { "age", age }
        });

        PostHogManager.Instance.Capture("user_registered", new Dictionary<string, object>
        {
            { "username", username },
            { "age", age }
        });
    }

    private void HideAllStages()
    {
        foreach (GameObject go in _uiElements)
        {
            go.SetActive(false);
        }
    }

    private void Show(GameObject go)
    {
        go.SetActive(true);
    }
    private void SetText(TMP_Text tmp_Text, string text, bool translate = true)
    {
        string displayText = text;
        if (translate)
        {
            displayText = new LocalizedString("String Table", text).GetLocalizedString();
        }
        tmp_Text.SetText(displayText);
    }
}
