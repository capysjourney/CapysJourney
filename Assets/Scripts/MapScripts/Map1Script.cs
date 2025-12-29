using UnityEngine;
using UnityEngine.UI;

public class Map1Script : MapScript
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
    [SerializeField] private Sprite _miniLevel1Icon;
    [SerializeField] private Sprite _miniLevel2Icon;
    [SerializeField] private Sprite _miniLevel3Icon;
    // todo - update these icons

    override protected void InitializeMaps()
    {
        _levelButtonMap = new()
        {
            { Level.World1Level1, _level1Btn },
            { Level.World1Level2, _level2Btn },
            { Level.World1Level3, _level3Btn },
            { Level.World1Level4, _level4Btn },
            { Level.World1Level5, _level5Btn },
            { Level.World1Level6, _level6Btn },
            { Level.World1Level7, _level7Btn },
            { Level.World1Level8, _level8Btn },
            { Level.World1Level9, _level9Btn },
            { Level.World1Level10, _level10Btn },
            { Level.World1SideLevel1, _SL1Btn },
            { Level.World1SideLevel2, _SL2Btn },
            { Level.World1SideLevel3, _SL3Btn },
            { Level.World1MiniLevel1, _miniLevel1Btn },
            { Level.World1MiniLevel2, _miniLevel2Btn },
            { Level.World1MiniLevel3, _miniLevel3Btn },
        };
        _levelIconMap = new()
        {
            { Level.World1Level1, _level1Icon },
            { Level.World1Level2, _level2Icon },
            { Level.World1Level3, _level3Icon },
            { Level.World1Level4, _level4Icon },
            { Level.World1Level5, _level5Icon },
            { Level.World1Level6, _level6Icon },
            { Level.World1Level7, _level7Icon },
            { Level.World1Level8, _level8Icon },
            { Level.World1Level9, _level9Icon },
            { Level.World1Level10, _level10Icon },
            { Level.World1SideLevel1, _SL1And2Icon },
            { Level.World1SideLevel2, _SL1And2Icon },
            { Level.World1SideLevel3, _SL3Icon },
            { Level.World1MiniLevel1, _miniLevel1Icon },
            { Level.World1MiniLevel2, _miniLevel2Icon },
            { Level.World1MiniLevel3, _miniLevel3Icon }
        };
        _previousLevel = new()
        {
            { _roadTo2, Level.World1Level1 },
            { _roadTo3, Level.World1Level2 },
            { _roadTo4, Level.World1Level3 },
            { _roadTo5, Level.World1Level4 },
            { _roadTo6, Level.World1Level5 },
            { _roadTo7, Level.World1Level6 },
            { _roadTo8, Level.World1Level7 },
            { _roadTo9, Level.World1Level8 },
            { _roadTo10, Level.World1Level9 },
            { _roadToSL1, Level.World1Level3 },
            { _roadToSL2, Level.World1SideLevel1 },
            { _roadToSL3, Level.World1Level5 },
            { _roadToMini, Level.World1Level9 }
        };
    }
}
