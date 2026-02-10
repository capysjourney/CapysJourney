using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
/// <summary>
/// Scene controller for the Journey screen.
/// Manages scene-level UI, session state, and map selection.
/// </summary>
public class JourneyScript : MonoBehaviour
{
    [Header("Map Configuration")]
    [SerializeField] private RectTransform _mapContainer;
    [SerializeField] private Image _firstStepsMap;
    [SerializeField] private Image _presentMomentMap;
    [SerializeField] private Image _everydayMindfulnessMap;
    [SerializeField] private FirstStepsScript _firstStepsScript;
    [SerializeField] private PresentMomentScript _presentMomentScript;
    [SerializeField] private EverydayMindfulnessScript _everydayMindfulnessScript;

    [Header("Scene UI")]
    [SerializeField] private Image _gradientBackground;
    [SerializeField] private NavBarScript _navBarScript;
    [SerializeField] private GameObject _loginBonus;
    [SerializeField] private Button _loginBonusButton;
    [SerializeField] private Button _worldButton;
    [SerializeField] private TMP_Text _loginBonusName;
    [SerializeField] private GameObject _newRegionNotif;
    [SerializeField] private Button _newRegionNotifWorldButton;

    [Header("Quincy")]
    [SerializeField] private Button _quincyMask;
    [SerializeField] private QuincyScript _quincyScript;

    private MapScript _activeMapScript;

    void Start()
    {
        InitializeSession();
        InitializeMap();
        InitializeSceneUI();
        InitializeAudio();
    }

    #region Session Management

    private void InitializeSession()
    {
        bool hasVisitedJourney = GameManager.GetHasVisitedJourney();
        if (hasVisitedJourney)
        {
            ToggleLoginBonusUI(false);
        }
        else if (GameManager.GetIsFirstLogin())
        {
            ToggleLoginBonusUI(false);
            GameManager.Login();
        }
        else
        {
            ShowLoginBonus();
        }
        GameManager.VisitJourney();
        GameManager.UpdateWorldAndLevel();
    }

    private async void ShowLoginBonus()
    {
        await CarrotManager.IncreaseCarrots(10);
        ToggleLoginBonusUI(true);
        string username = PlayerPrefs.GetString("username", "Friend");

        _loginBonusName.text = "Welcome back, " + username + "!";
        _loginBonusButton.onClick.RemoveAllListeners();
        _loginBonusButton.onClick.AddListener(() =>
        {
            ToggleLoginBonusUI(false);
        });
    }

    private void ToggleLoginBonusUI(bool showLoginBonus)
    {
        _navBarScript.gameObject.SetActive(!showLoginBonus);
        _loginBonus.SetActive(showLoginBonus);
    }

    #endregion

    #region Map Management

    private async void InitializeMap()
    {
        WorldInfo currentWorld = GameManager.GetCurrWorldInfo();
        LevelInfo currentLevel = GameManager.GetCurrLevelInfo();
        Dictionary<Level, LevelStatus> levelStatuses = await GameManager.GetWorldStatus(currentWorld.World);
        bool isQuincyUnlocked = QuincyManager.IsQuincyUnlocked(currentWorld);

        // Registry of all available maps
        Dictionary<WorldInfo, (Image mapImage, MapScript mapScript)> mapRegistry = new()
        {
            { WorldInfo.FirstSteps, (_firstStepsMap, _firstStepsScript) },
            { WorldInfo.PresentMoment, (_presentMomentMap, _presentMomentScript) },
            { WorldInfo.EverydayMindfulness, (_everydayMindfulnessMap, _everydayMindfulnessScript) }
        };

        if (!mapRegistry.ContainsKey(currentWorld))
        {
            Debug.LogError($"No map configured for world: {currentWorld}");
            return;
        }

        // Activate the appropriate map
        var (activeMapImage, activeMapScript) = mapRegistry[currentWorld];
        _activeMapScript = activeMapScript;
        Debug.Log(_activeMapScript);
        // Show only the active map
        foreach (var (_, (mapImage, _)) in mapRegistry)
        {
            mapImage.gameObject.SetActive(mapImage == activeMapImage);
        }

        // Initialize the map with game state and shared references
        _activeMapScript.Initialize(
            mapContainer: _mapContainer,
            currentLevel: currentLevel.Level,
            levelStatuses: levelStatuses,
            isQuincyUnlocked: isQuincyUnlocked,
            onQuincyDone: ConfigureNewRegionNotif,
            gradientBackground: _gradientBackground,
            navBarScript: _navBarScript,
            quincyMask: _quincyMask,
            quincyScript: _quincyScript
        );
    }

    #endregion

    #region Scene UI Configuration

    private void InitializeSceneUI()
    {
        ConfigureWorldButton();
        ConfigureNewRegionNotif();
    }

    private void ConfigureWorldButton()
    {
        _worldButton.onClick.RemoveAllListeners();
        _worldButton.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync("WorldMap");
        });
        _worldButton.gameObject.SetActive(true);
    }

    private void ConfigureNewRegionNotif()
    {
        WorldInfo currentWorld = GameManager.GetCurrWorldInfo();
        bool isWorldCompleted = GameManager.IsWorldCompleted(currentWorld.World);
        bool hasSeenNewWorldNotif = GameManager.GetHasSeenNewWorldNotif(currentWorld.World);

        if (isWorldCompleted && !hasSeenNewWorldNotif)
        {
            // Mark the new world notification as seen
            GameManager.OnSeenNewWorldNotif(currentWorld.World);
            _newRegionNotif.SetActive(true);
        }
        else
        {
            _newRegionNotif.SetActive(false);
        }
        _newRegionNotifWorldButton.onClick.RemoveAllListeners();
        _newRegionNotifWorldButton.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync("WorldMap");
        });
    }

    #endregion

    #region Audio

    private void InitializeAudio()
    {
        if (!AudioManager.Instance.IsMusicPlaying)
        {
            AudioManager.Instance.PlayMusic(Sound.MainTheme);
        }
    }

    #endregion
}
