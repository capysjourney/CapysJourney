using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EverydayMindfulnessScript : MapScript
{
    [Header("Roads")]
    [SerializeField] private Image _roadTo2;
    [SerializeField] private Image _roadToSL1;
    [SerializeField] private Image _roadTo3;
    [SerializeField] private Image _roadToSL2And4;

    [Header("Level Buttons")]
    [SerializeField] private Button _level1Btn;
    [SerializeField] private Button _level2Btn;
    [SerializeField] private Button _level3Btn;
    [SerializeField] private Button _level4Btn;
    [SerializeField] private Button _SL1Btn;
    [SerializeField] private Button _SL2Btn;

    [Header("Button Sprites")]
    [SerializeField] private Sprite _level1Icon;
    [SerializeField] private Sprite _level2Icon;
    [SerializeField] private Sprite _level3Icon;
    [SerializeField] private Sprite _level4Icon;
    [SerializeField] private Sprite _SL1Icon;
    [SerializeField] private Sprite _SL2Icon;

    protected override Vector2 QuincyPosition => new(769.9f,-160.9f);

    protected override Dictionary<Level, Button> CreateButtonDictionary()
    {
        return new()
        {
            { Level.EverydayMindfulness_L1, _level1Btn },
            { Level.EverydayMindfulness_L2, _level2Btn },
            { Level.EverydayMindfulness_L3, _level3Btn },
            { Level.EverydayMindfulness_L4, _level4Btn },
            { Level.EverydayMindfulness_SL1, _SL1Btn },
            { Level.EverydayMindfulness_SL2, _SL2Btn },

        };
    }

    protected override Dictionary<Level, Sprite> CreateIconDictionary()
    {
        return new()
        {
            { Level.EverydayMindfulness_L1, _level1Icon },
            { Level.EverydayMindfulness_L2, _level2Icon },
            { Level.EverydayMindfulness_L3, _level3Icon },
            { Level.EverydayMindfulness_L4, _level4Icon },
            { Level.EverydayMindfulness_SL1, _SL1Icon },
            { Level.EverydayMindfulness_SL2, _SL2Icon  },
        };
    }

    protected override Dictionary<Image, Level> CreateLevelBeforeRoadDictionary()
    {
        return new()
        {
            { _roadTo2, Level.EverydayMindfulness_L1 },
            { _roadTo3, Level.EverydayMindfulness_L2 },
            { _roadToSL1, Level.EverydayMindfulness_L2 },
            { _roadToSL2And4, Level.EverydayMindfulness_L3 },
        };
    }
}

