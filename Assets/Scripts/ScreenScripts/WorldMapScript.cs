using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapScript : MonoBehaviour
{
    [SerializeField] private Button _firstStepsButton;
    [SerializeField] private Button _presentMomentButton;
    [SerializeField] private Button _everydayMindfulnessButton;
    [SerializeField] private Button _exploringAwarenessButton;
    [SerializeField] private Button _compassionButton;
    [SerializeField] private Button _tree;
    [SerializeField] private Button _sleep;
    [SerializeField] private RectTransform _flag;
    [SerializeField] private GameObject _topClouds;
    [SerializeField] private GameObject _bottomClouds;
    private readonly Dictionary<Button, WorldEnum> _worldOfButton = new();
    private WorldEnum _currWorldEnum = WorldEnum.FirstSteps;
    private HashSet<WorldEnum> _unlockedWorldEnums = new() { WorldEnum.FirstSteps };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _worldOfButton[_firstStepsButton] = WorldEnum.FirstSteps;
        _worldOfButton[_presentMomentButton] = WorldEnum.PresentMoment;
        _worldOfButton[_everydayMindfulnessButton] = WorldEnum.EverydayMindfulness;
        _worldOfButton[_exploringAwarenessButton] = WorldEnum.ExploringAwareness;
        _worldOfButton[_compassionButton] = WorldEnum.Compassion;
        _worldOfButton[_tree] = WorldEnum.EverydayMindfulness;
        _worldOfButton[_sleep] = WorldEnum.Sleep;
        _currWorldEnum = GameManager.GetCurrWorld().EnumName;
        _unlockedWorldEnums = GameManager.GetUnlockedWorlds();
        PlaceFlag();
        SetClouds();
        foreach (var pair in _worldOfButton)
        {
            pair.Key.onClick.AddListener(() =>
            {
                // todo - change world
            });
        }
    }

    private void PlaceFlag()
    {
        Vector2 pos = _currWorldEnum switch
        {
            WorldEnum.FirstSteps => new(131, -166),
            WorldEnum.PresentMoment => new(-149.1f, -75.3f),
            WorldEnum.EverydayMindfulness => new(166.9f, 10f),
            WorldEnum.ExploringAwareness => new(-24f, 54f),
            WorldEnum.Compassion => new(42, 149),
            WorldEnum.Sleep => new(67.2f, 232.8f),
            _ => throw new ArgumentOutOfRangeException()
        };
        _flag.anchoredPosition = pos;
    }

    private void SetClouds()
    {
        if (_unlockedWorldEnums.Contains(WorldEnum.ExploringAwareness))
        {
            _topClouds.SetActive(false);
            _bottomClouds.SetActive(false);
        }
        else if (_unlockedWorldEnums.Contains(WorldEnum.EverydayMindfulness))
        {
            _topClouds.SetActive(false);
            _bottomClouds.SetActive(false);
        }
        else
        {
            _topClouds.SetActive(true);
            _bottomClouds.SetActive(true);
        }
    }
}
