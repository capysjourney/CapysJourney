using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldMapScript : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] private RectTransform _capy;

    [Header("World Buttons")]
    [SerializeField] private Button _firstStepsButton;
    [SerializeField] private Button _presentMomentButton;
    [SerializeField] private Button _everydayMindfulnessButton;
    [SerializeField] private Button _exploringAwarenessButton;
    [SerializeField] private Button _compassionButton;
    [SerializeField] private Button _treeButton;
    [SerializeField] private Button _sleepButton;

    [Header("Clouds")]
    [SerializeField] private GameObject _sleepCloud;
    [SerializeField] private GameObject _exploringAwarenessCloud;
    [SerializeField] private GameObject _compassionCloud;
    [SerializeField] private GameObject _everydayMindfulnessCloud;
    [SerializeField] private GameObject _presentMomentCloud;

    [Header("Prefabs")]
    [SerializeField] private GameObject _flagPrefab;
    [SerializeField] private GameObject _beginRegionPrefab;
    private readonly Dictionary<Button, World> _worldOfButton = new();
    private World _currWorldEnum = World.FirstSteps;
    private HashSet<World> _unlockedWorlds = new() { World.FirstSteps };
    private HashSet<World> _newlyAvailableWorlds = new() { };
    private HashSet<World> _completedWorlds = new() { };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _worldOfButton[_firstStepsButton] = World.FirstSteps;
        _worldOfButton[_presentMomentButton] = World.PresentMoment;
        _worldOfButton[_everydayMindfulnessButton] = World.EverydayMindfulness;
        //_worldOfButton[_exploringAwarenessButton] = World.ExploringAwareness;
        //_worldOfButton[_compassionButton] = World.Compassion;
        //_worldOfButton[_tree] = World.EverydayMindfulness;
        //_worldOfButton[_sleep] = World.Sleep;
        _currWorldEnum = GameManager.GetCurrWorldInfo().World;
        _unlockedWorlds = GameManager.GetUnlockedWorlds();
        _newlyAvailableWorlds = GameManager.GetNewlyAvailableWorlds();
        _completedWorlds = GameManager.GetCompletedWorlds();
        PlaceFlags();
        SetClouds();
        PlaceCapy();
        PlaceBeginRegionSigns();
        foreach ((Button button, World world) in _worldOfButton)
        {
            button.onClick.AddListener(() =>
            {
                GameManager.SetCurrLevel(world.GetInfo().FirstLevel);
                SceneManager.LoadSceneAsync("Journey");
            });
        }
    }

    private void PlaceFlags()
    {
        Dictionary<World, Vector2> worldPositions = new()
        {
           { World.FirstSteps, new(131, -166) },
            { World.PresentMoment, new(-149.1f, -75.3f) },
            { World.EverydayMindfulness, new(166.9f, 10f) },
            //{ World.ExploringAwareness, new(-24f, 54f) },
            //{ World.Compassion, new(42, 149) },
            //{ World.Sleep, new(67.2f, 232.8f) },
        };
        foreach (World world in _completedWorlds)
        {
            GameObject flagObj = Instantiate(_flagPrefab, transform);
            RectTransform flagRect = flagObj.GetComponent<RectTransform>();
            flagObj.transform.SetParent(_canvas.transform, false);
            flagRect.anchoredPosition = worldPositions[world];
        }
    }

    private void SetClouds()
    {
        GameObject[] clouds = new[] { _sleepCloud, _compassionCloud, _exploringAwarenessCloud, _everydayMindfulnessCloud, _presentMomentCloud };
        _sleepCloud.SetActive(true);
        _compassionCloud.SetActive(true);
        _exploringAwarenessCloud.SetActive(true);
        _everydayMindfulnessCloud.SetActive(!_unlockedWorlds.Contains(World.EverydayMindfulness));
        _presentMomentCloud.SetActive(! _unlockedWorlds.Contains(World.PresentMoment));
        // in case flags go on top of clouds
        foreach(GameObject cloud in clouds)
        {
            cloud.transform.SetAsLastSibling();
        }
    }

    private void PlaceCapy()
    {
        Dictionary<World, Vector2> capyPositionForWorld = new()
        {
            { World.FirstSteps, new(-24,-175) },
            { World.PresentMoment, new(-39,-50) },
            { World.EverydayMindfulness, new(42,29) },
            //{ World.ExploringAwareness, new(-163,79) },
            //{ World.Compassion, new(93,167) },
            //{ World.Sleep, new(131,262) },
        };
        _capy.anchoredPosition = capyPositionForWorld[_currWorldEnum];
    }

    private void PlaceBeginRegionSigns()
    {
        Dictionary<World, Vector2> beginRegionPositions = new()
        {
            {World.FirstSteps, new(92,-122.3f) },
            { World.PresentMoment, new(-94,-49) },
            { World.EverydayMindfulness, new(92,38) },
            //{ World.ExploringAwareness, new(-95,92) },
            //{ World.Compassion, new(102,158) },
            //{ World.Sleep, new(65,263) },
        };
        foreach (World world in _newlyAvailableWorlds)
        {
            GameObject beginRegionObj = Instantiate(_beginRegionPrefab, transform);
            RectTransform beginRegionRect = beginRegionObj.GetComponent<RectTransform>();
            Button beginRegionButton = beginRegionObj.GetComponent<Button>();
            beginRegionObj.transform.SetParent(_canvas.transform, false);
            beginRegionRect.anchoredPosition = beginRegionPositions[world];
            beginRegionButton.onClick.AddListener(() =>
            {
                GameManager.SetCurrLevel(world.GetInfo().FirstLevel);
                SceneManager.LoadSceneAsync("Journey");
            });
        }
    }
}
