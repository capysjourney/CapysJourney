using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstStepsScript : MapScript
{
    [Header("Roads")]
    [SerializeField] private Image _roadTo2;
    [SerializeField] private Image _roadTo3;
    [SerializeField] private Image _roadTo4;
    [SerializeField] private Image _roadTo5;
    [SerializeField] private Image _roadTo6;
    [SerializeField] private Image _roadTo7;
    [SerializeField] private Image _roadTo8;
    [SerializeField] private Image _roadTo9;
    [SerializeField] private Image _roadTo10;
    [SerializeField] private Image _roadToSL1;
    [SerializeField] private Image _roadToSL2;
    [SerializeField] private Image _roadToSL3;
    [SerializeField] private Image _roadToMini;
    [SerializeField] private Image _roadMiniUp;
    [SerializeField] private Image _roadMiniDown;

    [Header("Level Buttons")]
    [SerializeField] private Button _level1Btn;
    [SerializeField] private Button _level2Btn;
    [SerializeField] private Button _level3Btn;
    [SerializeField] private Button _level4Btn;
    [SerializeField] private Button _level5Btn;
    [SerializeField] private Button _level6Btn;
    [SerializeField] private Button _level7Btn;
    [SerializeField] private Button _level8Btn;
    [SerializeField] private Button _level9Btn;
    [SerializeField] private Button _level10Btn;
    [SerializeField] private Button _SL1Btn;
    [SerializeField] private Button _SL2Btn;
    [SerializeField] private Button _SL3Btn;
    [SerializeField] private Button _miniLevel1Btn;
    [SerializeField] private Button _miniLevel2Btn;
    [SerializeField] private Button _miniLevel3Btn;

    [Header("Button Sprites")]
    [SerializeField] private Sprite _level1Icon;
    [SerializeField] private Sprite _level2Icon;
    [SerializeField] private Sprite _level3Icon;
    [SerializeField] private Sprite _level4Icon;
    [SerializeField] private Sprite _level5Icon;
    [SerializeField] private Sprite _level6Icon;
    [SerializeField] private Sprite _level7Icon;
    [SerializeField] private Sprite _level8Icon;
    [SerializeField] private Sprite _level9Icon;
    [SerializeField] private Sprite _level10Icon;
    [SerializeField] private Sprite _SL1And2Icon;
    [SerializeField] private Sprite _SL3Icon;
    [SerializeField] private Sprite _miniLevelIcon;
    protected override Vector2 QuincyPosition => new(750, -91);

    protected override Dictionary<Level, Button> CreateButtonDictionary()
    {
        return new()
        {
            { Level.FirstSteps_L1, _level1Btn },
            { Level.FirstSteps_L2, _level2Btn },
            { Level.FirstSteps_L3, _level3Btn },
            { Level.FirstSteps_L4, _level4Btn },
            { Level.FirstSteps_L5, _level5Btn },
            { Level.FirstSteps_L6, _level6Btn },
            { Level.FirstSteps_L7, _level7Btn },
            { Level.FirstSteps_L8, _level8Btn },
            { Level.FirstSteps_L9, _level9Btn },
            { Level.FirstSteps_L10, _level10Btn },
            { Level.FirstSteps_SL1, _SL1Btn },
            { Level.FirstSteps_SL2, _SL2Btn },
            { Level.FirstSteps_SL3, _SL3Btn },
            { Level.FirstSteps_MM1, _miniLevel1Btn },
            { Level.FirstSteps_MM2, _miniLevel2Btn },
            { Level.FirstSteps_MM3, _miniLevel3Btn },
        };
    }

    protected override Dictionary<Level, Sprite> CreateIconDictionary()
    {
        return new()
        {
            { Level.FirstSteps_L1, _level1Icon },
            { Level.FirstSteps_L2, _level2Icon },
            { Level.FirstSteps_L3, _level3Icon },
            { Level.FirstSteps_L4, _level4Icon },
            { Level.FirstSteps_L5, _level5Icon },
            { Level.FirstSteps_L6, _level6Icon },
            { Level.FirstSteps_L7, _level7Icon },
            { Level.FirstSteps_L8, _level8Icon },
            { Level.FirstSteps_L9, _level9Icon },
            { Level.FirstSteps_L10, _level10Icon },
            { Level.FirstSteps_SL1, _SL1And2Icon },
            { Level.FirstSteps_SL2, _SL1And2Icon },
            { Level.FirstSteps_SL3, _SL3Icon },
            { Level.FirstSteps_MM1, _miniLevelIcon },
            { Level.FirstSteps_MM2, _miniLevelIcon },
            { Level.FirstSteps_MM3, _miniLevelIcon }
        };
    }

    protected override Dictionary<Image, Level> CreateLevelBeforeRoadDictionary()
    {
        return new()
        {
            { _roadTo2, Level.FirstSteps_L1 },
            { _roadTo3, Level.FirstSteps_L2 },
            { _roadTo4, Level.FirstSteps_L3 },
            { _roadTo5, Level.FirstSteps_L4 },
            { _roadTo6, Level.FirstSteps_L5 },
            { _roadTo7, Level.FirstSteps_L6 },
            { _roadTo8, Level.FirstSteps_L7 },
            { _roadTo9, Level.FirstSteps_L8 },
            { _roadTo10, Level.FirstSteps_L9 },
            { _roadToSL1, Level.FirstSteps_L3 },
            { _roadToSL2, Level.FirstSteps_SL1 },
            { _roadToSL3, Level.FirstSteps_L5 },
            { _roadToMini, Level.FirstSteps_L9 }
        };
    }
}
