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
    private readonly Dictionary<Button, World> _worldOfButton = new();
    private World _currWorldEnum = World.FirstSteps;
    private HashSet<World> _unlockedWorldEnums = new() { World.FirstSteps };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _worldOfButton[_firstStepsButton] = World.FirstSteps;
        //_worldOfButton[_presentMomentButton] = World.PresentMoment;
        //_worldOfButton[_everydayMindfulnessButton] = World.EverydayMindfulness;
        //_worldOfButton[_exploringAwarenessButton] = World.ExploringAwareness;
        //_worldOfButton[_compassionButton] = World.Compassion;
        //_worldOfButton[_tree] = World.EverydayMindfulness;
        //_worldOfButton[_sleep] = World.Sleep;
        _currWorldEnum = GameManager.GetCurrWorldInfo().World;
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
            World.FirstSteps => new(131, -166),
            //World.PresentMoment => new(-149.1f, -75.3f),
            //World.EverydayMindfulness => new(166.9f, 10f),
            //World.ExploringAwareness => new(-24f, 54f),
            //World.Compassion => new(42, 149),
            //World.Sleep => new(67.2f, 232.8f),
            _ => throw new ArgumentOutOfRangeException()
        };
        _flag.anchoredPosition = pos;
    }

    private void SetClouds()
    {
        //if (_unlockedWorldEnums.Contains(World.ExploringAwareness))
        //{
        //    _topClouds.SetActive(false);
        //    _bottomClouds.SetActive(false);
        //}
        //else if (_unlockedWorldEnums.Contains(World.EverydayMindfulness))
        //{
        //    _topClouds.SetActive(false);
        //    _bottomClouds.SetActive(false);
        //}
        //else
        //{
            _topClouds.SetActive(true);
            _bottomClouds.SetActive(true);
        //}
    }
}
