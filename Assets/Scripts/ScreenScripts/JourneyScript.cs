using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Scene controller for the Journey screen.
/// Manages scene-level UI, session state, and map selection.
/// </summary>
public class JourneyScript : MonoBehaviour
{
    [Header("Map Configuration")]
    [SerializeField] private RectTransform _mapContainer;
    [SerializeField] private RectTransform _capy;
    [SerializeField] private Image _firstStepsMap;
    [SerializeField] private FirstStepsScript _firstStepsScript;
    
    [Header("Scene UI")]
    [SerializeField] private GameObject _navBar;
    [SerializeField] private GameObject _loginBonus;
    [SerializeField] private Button _loginBonusButton;
    [SerializeField] private Button _worldButton;
    [SerializeField] private GameObject _newRegionNotif;
    [SerializeField] private Button _newRegionNotifWorldButton;
    
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

    private void ShowLoginBonus()
    {
        CarrotManager.IncreaseCarrots(10);
        ToggleLoginBonusUI(true);
        _loginBonusButton.onClick.RemoveAllListeners();
        _loginBonusButton.onClick.AddListener(() =>
        {
            ToggleLoginBonusUI(false);
        });
    }

    private void ToggleLoginBonusUI(bool showLoginBonus)
    {
        _navBar.SetActive(!showLoginBonus);
        _loginBonus.SetActive(showLoginBonus);
    }

    #endregion

    #region Map Management

    private void InitializeMap()
    {
        World currentWorld = GameManager.GetCurrWorld();
        Level currentLevel = GameManager.GetCurrLevel();
        Dictionary<Level, LevelStatus> levelStatuses = GameManager.GetWorldStatus(currentWorld);
        bool isQuincyUnlocked = QuincyManager.IsQuincyUnlocked(currentWorld);
        
        // Registry of all available maps
        Dictionary<World, (Image mapImage, MapScript mapScript)> mapRegistry = new()
        {
            { World.FirstSteps, (_firstStepsMap, _firstStepsScript) }
        };

        if (!mapRegistry.ContainsKey(currentWorld))
        {
            Debug.LogError($"No map configured for world: {currentWorld}");
            return;
        }

        // Activate the appropriate map
        var (activeMapImage, activeMapScript) = mapRegistry[currentWorld];
        _activeMapScript = activeMapScript;
        
        // Show only the active map
        foreach (var (_, (mapImage, _)) in mapRegistry)
        {
            mapImage.gameObject.SetActive(mapImage == activeMapImage);
        }
        
        // Initialize the map with game state and shared references
        _activeMapScript.Initialize(
            mapContainer: _mapContainer,
            capy: _capy,
            currentLevel: currentLevel,
            levelStatuses: levelStatuses,
            isQuincyUnlocked: isQuincyUnlocked,
            onQuincyDone: ConfigureNewRegionNotif
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
            //SceneManager.LoadSceneAsync("WorldMap");
            // todo - make worldmap scene
        });
        _worldButton.gameObject.SetActive(true);
    }

    private void ConfigureNewRegionNotif()
    {
        World currentWorld = GameManager.GetCurrWorld();
        bool isWorldCompleted = GameManager.IsWorldCompleted(currentWorld);
        bool hasSeenNewWorldNotif = GameManager.GetHasSeenNewWorldNotif(currentWorld);
        
        _newRegionNotif.SetActive(isWorldCompleted && !hasSeenNewWorldNotif);
        
        _newRegionNotifWorldButton.onClick.RemoveAllListeners();
        _newRegionNotifWorldButton.onClick.AddListener(() =>
        {
            //SceneManager.LoadSceneAsync("WorldMap");
            // todo - make worldmap scene
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
