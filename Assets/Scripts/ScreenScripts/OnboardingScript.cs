using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class OnboardingScript : MonoBehaviour
{
    private enum Stage
    {
        PARENT_CONFIRMATION,
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
    [SerializeField] private Button _continueButton;
    [SerializeField] private GameObject _backButton;
    [SerializeField] private GameObject _languageSelection;
    [SerializeField] private GameObject _parentConfirmationArea;
    [SerializeField] private GameObject _bigCapy;
    [SerializeField] private Image _progressBar;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private GameObject _questionHeader;
    [SerializeField] private GameObject _textInputArea;
    [SerializeField] private TMP_Text _inputLabel;
    [SerializeField] private TMP_Text _placeholder;
    [SerializeField] private GameObject _appearanceInputArea;
    [SerializeField] private GameObject _bigSpeechBubble1;
    [SerializeField] private GameObject _bigSpeechBubble2;
    private const string _askUsername = "Pick a username!";
    private const string _askAppearance = "Pick an appearance!";
    private const string _usernameLabel = "Username:";
    private const string _appearanceLabel = "Appearance:";
    private const string _usernamePlaceholder = "Enter username";
    [SerializeField] private Sprite _noProgress;
    [SerializeField] private Sprite _halfProgress;
    [SerializeField] private Sprite _fullProgress;
    private GameObject[] _stageUIs;
    private TMP_Text _confirmationSpeechBubbleText;
    private bool _didParentVerify = true;

    void Start()
    {
        _stageUIs = new GameObject[] { _languageSelection, _bigCapy, _parentConfirmationArea, _continueButton.gameObject, _backButton, _questionHeader, _textInputArea, _appearanceInputArea };
        if (LocalizationSettings.SelectedLocale == null)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }
        AddContinueButtonListener();
        AddBackButtonListener();
        _didParentVerify = !GameManager.NeedParentConfirmation;
        _confirmationSpeechBubbleText = _parentConfirmationArea.GetComponentInChildren<TMP_Text>();
        if (GameManager.NeedParentConfirmation)
        {
            LoadParentConfirmation();
        }
        else
        {
            _parentConfirmationArea.SetActive(false);
            LoadCapyIntro();
        }
    }

    private void LoadParentConfirmation()
    {
        _currStage = Stage.PARENT_CONFIRMATION;
        HideAllStages();
        _confirmationSpeechBubbleText.SetText("Waiting for parent confirmation....");
        _continueButton.interactable = false;
        _backButton.SetActive(false);
        Show(_parentConfirmationArea);
        StartCoroutine(WaitForParentConfirmation());
    }

    private IEnumerator WaitForParentConfirmation()
    {
        FirebaseAuth.DefaultInstance.CurrentUser.SendEmailVerificationAsync();
        while (!_didParentVerify)
        {
            FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    _didParentVerify = FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified;
                }
            });
            yield return new WaitForSeconds(1f);
        }
        GameManager.NeedParentConfirmation = false;
        _confirmationSpeechBubbleText.SetText("Parent confirmation received!");
        _continueButton.gameObject.SetActive(true);
        _continueButton.interactable = true;
    }

    private void AddContinueButtonListener()
    {
        _continueButton.onClick.AddListener(() =>
        {
            switch (_currStage)
            {
                case Stage.PARENT_CONFIRMATION:
                    LoadCapyIntro();
                    break;
                case Stage.CAPY_INTRO:
                    LoadInputName();
                    break;
                case Stage.INPUT_NAME:
                    PlayerPrefs.SetString("username", _textInputArea.GetComponentInChildren<TMP_InputField>().text);
                    LoadTransition();
                    break;
                case Stage.TRANSITION:
                    CreatePlayerStats();
                    SceneManager.LoadSceneAsync("Tutorial");
                    break;
                default:
                    break;
            }
        });
    }
    private void AddBackButtonListener()
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
                    LoadInputName();
                    break;
                case Stage.TRANSITION:
                    LoadInputName();
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
        Show(_continueButton.gameObject);
        GameObject[] languageBoxes = GameObject.FindGameObjectsWithTag("Language");
        foreach (GameObject languageBox in languageBoxes)
        {
            GetScript(languageBox).SetOnClickListener(() => SelectLanguage(languageBox.name, languageBoxes));
        }
        if (LocalizationSettings.SelectedLocale == null)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
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
    private ToggleLanguageScript GetScript(GameObject gameObject)
    {
        return gameObject.transform.GetComponentInChildren<ToggleLanguageScript>();
    }

    private void LoadCapyIntro()
    {
        _currStage = Stage.CAPY_INTRO;
        HideAllStages();
        _bigSpeechBubble2.SetActive(false);
        Show(_bigCapy);
        Show(_continueButton.gameObject);
        Show(_bigSpeechBubble1);
    }

    private void LoadInputName()
    {
        _currStage = Stage.INPUT_NAME;
        HideAllStages();
        _progressBar.sprite = _noProgress;
        _questionText.text = _askUsername;
        _inputLabel.text = _usernameLabel;
        _placeholder.text = _usernamePlaceholder;
        _textInputArea.GetComponentInChildren<TMP_InputField>().text = PlayerPrefs.GetString("username", "");
        Show(_continueButton.gameObject);
        Show(_questionHeader);
        Show(_textInputArea);
        Show(_backButton);
    }

    private void LoadInputAppearance()
    {
        HideAllStages();
        _currStage = Stage.INPUT_APPEARANCE;
        _progressBar.sprite = _fullProgress;
        SetText(_questionText, _askAppearance);
        SetText(_inputLabel, _appearanceLabel);
        Show(_questionHeader);
        Show(_continueButton.gameObject);
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
        Show(_continueButton.gameObject);
        Show(_backButton);
        Show(_bigSpeechBubble2);
    }

    private void CreatePlayerStats()
    {
        PlayerStats stats = new(PlayerPrefs.GetInt("isGuest", 1) == 1);

        DataManager.SetStats(stats);

        string username = PlayerPrefs.GetString("username", "");
        int age = PlayerPrefs.GetInt("age", 0);

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
        foreach (GameObject gameObject in _stageUIs)
        {
            gameObject.SetActive(false);
        }
    }

    private void Show(GameObject gameObject)
    {
        gameObject.SetActive(true);
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
