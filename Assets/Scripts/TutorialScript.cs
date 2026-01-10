using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject _journeyScreen;
    [SerializeField] private GameObject _dailyScreen;
    [SerializeField] private GameObject _denScreen;
    [SerializeField] private GameObject _profileScreen;
    [SerializeField] private GameObject _endScreen;

    [Header("Tabs")]
    [SerializeField] private Button _dailyTab;
    [SerializeField] private Button _wardrobeTab;
    [SerializeField] private Button _profileTab;

    [Header("Mask Sprites")]
    [SerializeField] private Sprite _bubbleMask;
    [SerializeField] private Sprite _dojoMask;
    [SerializeField] private Sprite _circleMask;

    [Header("UI Elements")]
    [SerializeField] private Image _mask;
    [SerializeField] private Image _explanationBubble;
    [SerializeField] private Button _screenButton;
    [SerializeField] private Button _circleButton;
    [SerializeField] private Button _beginJourneyButton;
    [SerializeField] private RectTransform _tapToContinueText;
    [SerializeField] private NavBarScript _navBarScript;

    private TMP_Text _explanation;
    private int _currentStep = 1;
    private static readonly HashSet<int> TappableSteps = new() { 1, 2, 4, 6, 8 };
    private HashSet<GameObject> _screens;
    private HashSet<GameObject> _tutorialObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _explanation = _explanationBubble.GetComponentInChildren<TMP_Text>();
        _screenButton.onClick.AddListener(OnScreenButtonClicked);
        _beginJourneyButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Journey");
        });
        _navBarScript.SetIsForTutorial(true);
        _circleButton.onClick.AddListener(() =>
        {
            if (_currentStep == 3 || _currentStep == 5 || _currentStep == 7)
            {
                _currentStep++;
                GoToStep();
            }
            NavBarScript.Scene scene = _currentStep == 4 ? NavBarScript.Scene.Daily :
                                        _currentStep == 6 ? NavBarScript.Scene.Den :
                                        _currentStep == 8 ? NavBarScript.Scene.Profile :
                                        NavBarScript.Scene.Journey;
            _navBarScript.ChangeScene(scene);
        });
        _screens = new HashSet<GameObject>() { _journeyScreen, _dailyScreen, _denScreen, _profileScreen, _endScreen };
        _tutorialObjects = new HashSet<GameObject>() {
            _mask.gameObject,
            _explanationBubble.gameObject,
            _screenButton.gameObject,
            _tapToContinueText.gameObject,
            _navBarScript.gameObject ,
            _circleButton.gameObject
        };
        GoToStep();
    }

    private void OnScreenButtonClicked()
    {
        if (TappableSteps.Contains(_currentStep))
        {
            _currentStep++;
            GoToStep();
        }
    }

    private void HideAllScreens()
    {
        foreach (GameObject screen in _screens)
        {
            screen.SetActive(false);
        }
    }

    private void ShowTutorialObjects()
    {
        foreach (GameObject obj in _tutorialObjects)
        {
            obj.SetActive(true);
        }
    }

    private void HideTutorialObjects()
    {
        foreach (GameObject obj in _tutorialObjects)
        {
            obj.SetActive(false);
        }
    }

    private void GoToStep()
    {
        switch (_currentStep)
        {
            case 1:
                SwitchToScreen(_journeyScreen);
                ShowTutorialObjects();
                _mask.sprite = _bubbleMask;
                AnchorToBottomCenter(_mask.rectTransform);
                _mask.rectTransform.anchoredPosition = new Vector2(-111, 117);
                _explanation.text = "This is the Journey tab, where you’ll explore various levels and themes with me!";
                AnchorToBottomCenter(_explanationBubble.rectTransform);
                _explanationBubble.rectTransform.anchoredPosition = new Vector2(-78, 229);
                _explanationBubble.rectTransform.sizeDelta = new Vector2(230, 136);
                AnchorToBottomCenter(_tapToContinueText);
                _tapToContinueText.anchoredPosition = new Vector2(29.8f, 125.08f);
                EnableTapToContinue();
                break;
            case 2:
                _mask.sprite = _dojoMask;
                AnchorToBottomLeft(_mask.rectTransform);
                _mask.rectTransform.anchoredPosition = new Vector2(1194, 1252);
                _explanation.text = "You can also create your own meditations or review finished ones in the dojo!";
                AnchorToBottomLeft(_explanationBubble.rectTransform);
                _explanationBubble.rectTransform.anchoredPosition = new Vector2(129, 181);
                _explanationBubble.rectTransform.sizeDelta = new Vector2(237, 121);
                AnchorToBottomLeft(_tapToContinueText);
                _tapToContinueText.anchoredPosition = new Vector2(212, 87);
                break;
            case 3:
                _mask.sprite = _circleMask;
                AnchorToBottomCenter(_mask.rectTransform);
                _mask.rectTransform.anchoredPosition = new Vector2(-37.3f, 108.6f);
                _explanation.text = "Now, click on the daily activities tab.";
                AnchorToBottomCenter(_explanationBubble.rectTransform);
                _explanationBubble.rectTransform.anchoredPosition = new Vector2(-30.6f, 191);
                _explanationBubble.rectTransform.sizeDelta = new Vector2(218, 85);
                _tapToContinueText.gameObject.SetActive(false);
                DisableTapToContinue();
                break;
            case 4:
                _mask.sprite = _bubbleMask;
                SwitchToScreen(_dailyScreen);
                _mask.rectTransform.anchoredPosition = new Vector2(-34.7f, 120.6f);
                _explanation.text = "This is the Daily Activities tab, where you’ll be given daily exercises to complete!";
                _explanationBubble.rectTransform.anchoredPosition = new Vector2(-30.6f, 269);
                _explanationBubble.rectTransform.sizeDelta = new Vector2(231, 136);
                AnchorToBottomCenter(_tapToContinueText);
                _tapToContinueText.anchoredPosition = new Vector2(-21, 176);
                EnableTapToContinue();
                break;
            case 5:
                _mask.sprite = _circleMask;
                _mask.rectTransform.anchoredPosition = new Vector2(42, 110);
                _explanation.text = "Now, click on the wardrobe tab.";
                _explanationBubble.rectTransform.anchoredPosition = new Vector2(45, 193);
                _explanationBubble.rectTransform.sizeDelta = new Vector2(218, 85);
                DisableTapToContinue();
                break;
            case 6:
                SwitchToScreen(_denScreen);
                _mask.sprite = _bubbleMask;
                _mask.rectTransform.anchoredPosition = new Vector2(42, 122.5f);
                _explanation.text = "This is the Wardrobe tab, where you can customize me and my den!";
                _explanationBubble.rectTransform.anchoredPosition = new Vector2(45, 254.9f);
                _explanationBubble.rectTransform.sizeDelta = new Vector2(218, 136);
                _tapToContinueText.anchoredPosition = new Vector2(42, 170.6f);
                EnableTapToContinue();
                break;
            case 7:
                _mask.sprite = _circleMask;
                _mask.rectTransform.anchoredPosition = new Vector2(118, 106.7f);
                _explanation.text = "Now, click on the profile tab.";
                _explanationBubble.rectTransform.anchoredPosition = new Vector2(117, 199);
                _explanationBubble.rectTransform.sizeDelta = new Vector2(218, 85);
                DisableTapToContinue();
                break;
            case 8:
                SwitchToScreen(_profileScreen);
                _mask.sprite = _bubbleMask;
                _mask.rectTransform.anchoredPosition = new Vector2(118, 121);
                _explanation.text = "This is the Profile tab, where you can check out your stats and show off your achievements!";
                _explanationBubble.rectTransform.anchoredPosition = new Vector2(109, 238);
                _explanationBubble.rectTransform.sizeDelta = new Vector2(234, 146);
                _tapToContinueText.anchoredPosition = new Vector2(9, 146);
                EnableTapToContinue();
                break;
            case 9:
                HideTutorialObjects();
                SwitchToScreen(_endScreen);
                break;
            default: break;
        }
    }

    private void EnableTapToContinue()
    {
        _screenButton.gameObject.SetActive(true);
        _tapToContinueText.gameObject.SetActive(true);
        _circleButton.gameObject.SetActive(false);
    }

    private void DisableTapToContinue()
    {
        _screenButton.gameObject.SetActive(false);
        _tapToContinueText.gameObject.SetActive(false);
        _circleButton.gameObject.SetActive(true);
    }

    private void SwitchToScreen(GameObject screen)
    {
        HideAllScreens();
        screen.SetActive(true);
    }

    private void AnchorToBottomCenter(RectTransform rectTransform)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
    }

    private void AnchorToBottomLeft(RectTransform rectTransform)
    {
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(0f, 0f);
    }
}
