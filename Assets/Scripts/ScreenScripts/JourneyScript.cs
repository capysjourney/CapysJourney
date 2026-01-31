using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JourneyScript : MonoBehaviour
{
    [SerializeField] private Image _firstStepsMap;
    [SerializeField] private RectTransform _capy;
    [SerializeField] private RectTransform _mapContainer;
    [SerializeField] private FirstStepsScript _firstStepsScript;
    [SerializeField] private GameObject _worldButtonGO;
    [SerializeField] private GameObject _navBar;
    [SerializeField] private GameObject _loginBonus;
    [SerializeField] private Button _loginBonusButton;
    private Button _worldButton;
    private Image _map;

    void Start()
    {
        bool hasVisitedJourney = GameManager.GetHasVisitedJourney();
        if (hasVisitedJourney)
        {
            _navBar.SetActive(true);
            _loginBonus.SetActive(false);
        }
        else if (GameManager.GetIsFirstLogin())
        {
            _navBar.SetActive(true);
            _loginBonus.SetActive(false);
            GameManager.Login();
        }
        else
        {
            CarrotManager.IncreaseCarrots(10);
            _navBar.SetActive(false);
            _loginBonus.SetActive(true);
            _loginBonusButton.onClick.RemoveAllListeners();
            _loginBonusButton.onClick.AddListener(() =>
            {
                _navBar.SetActive(true);
                _loginBonus.SetActive(false);
            });

        }
        GameManager.VisitJourney();
        _worldButton = _worldButtonGO.GetComponent<Button>();
        GameManager.UpdateWorldAndLevel();
        InitializeMap();
        ConfigureWorldButton();
        if (!AudioManager.Instance.IsMusicPlaying)
        {
            AudioManager.Instance.PlayMusic(Sound.MainTheme);
        }
    }

    private void InitializeMap()
    {
        World world = GameManager.GetCurrWorld();
        Level level = GameManager.GetCurrLevel();
        Dictionary<Level, LevelStatus> statuses = GameManager.GetWorldStatus(world);
        bool isQuincyUnlocked = QuincyManager.IsQuincyUnlocked(world);
        Dictionary<World, Image> maps = new()
        {
            { World.FirstSteps, _firstStepsMap }
        };
        _map = maps[world];
        foreach (Image image in maps.Values)
        {
            image.gameObject.SetActive(image == _map);
        }
        Dictionary<World, MapScript> scripts = new() { { World.FirstSteps, _firstStepsScript } };
        scripts[world].Initialize(level, statuses, isQuincyUnlocked);
        _worldButtonGO.SetActive(false);
    }

    private void ConfigureWorldButton()
    {
        _worldButton.onClick.RemoveAllListeners();
        _worldButton.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync("WorldMap");
            // todo - make map
        });
    }
}
