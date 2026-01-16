using System.Collections.Generic;
using UnityEngine;

public enum LevelEnum
{
    W1L1,
    W1L2,
    W1L3,
    W1L4,
    W1L5,
    W1L6,
    W1L7,
    W1L8,
    W1L9,
    W1L10,
    W1SL1,
    W1SL2,
    W1SL3,
    W1MM1,
    W1MM2,
    W1MM3
}
public class Level
{
    public static readonly Level World1Level1 = new(
       shortName: "Level 1-1",
       name: "Meet Capy & Planting Seeds",
       description: "Introducing the practice of mindfulness",
       audioFile: new()
       {
           { AgeGroup.Child, "W1L1A0" },
           { AgeGroup.YoungTeen, "W1L1A0" },
           { AgeGroup.OldTeen, "W1L1A3" },
           { AgeGroup.Adult, "W1L1A3" }
       },
       bannerFile: "W1L1L",
       mapPosition: new Vector2(129, 200.525f),
       capyPosition: new Vector2(-84, -440),
       enumName: LevelEnum.W1L1);

    public static readonly Level World1Level2 = new(
        shortName: "Level 1-2",
        name: "The Fading Bell",
        description: "Investigating awareness with a fading bell",
       audioFile: new()
       {
           { AgeGroup.Child, "W1L2A0" },
           { AgeGroup.YoungTeen, "W1L2A0" },
           { AgeGroup.OldTeen, "W1L2A3" },
           { AgeGroup.Adult, "W1L2A3" }
       },
        bannerFile: "W1L2L",
        mapPosition: new Vector2(129, 200.525f),
        capyPosition: new Vector2(-84, -248),
        enumName: LevelEnum.W1L2);

    public static readonly Level World1Level3 = new(
        shortName: "Level 1-3",
        name: "Capy Meets the Breathing Cloud",
        description: "Learning to focus on the breath",
       audioFile: new()
       {
           { AgeGroup.Child, "W1L3A0" },
           { AgeGroup.YoungTeen, "W1L3A0" },
           { AgeGroup.OldTeen, "W1L3A3" },
           { AgeGroup.Adult, "W1L3A3" }
       },
        bannerFile: "W1L3L",
        mapPosition: new Vector2(9, 66),
        capyPosition: new Vector2(33, -165),
        enumName: LevelEnum.W1L3);

    public static readonly Level World1Level4 = new(
        shortName: "Level 1-4",
        name: "Mailbox of Friendly Wishes",
        description: "Extending compassion to ourself and others",
       audioFile: new()
       {
           { AgeGroup.Child, "W1L4A0" },
           { AgeGroup.YoungTeen, "W1L4A0" },
           { AgeGroup.OldTeen, "W1L4A3" },
           { AgeGroup.Adult, "W1L4A3" }
       },
        bannerFile: "W1L4L",
        mapPosition: new Vector2(-210, 66),
        capyPosition: new Vector2(259, -165),
        enumName: LevelEnum.W1L4);

    public static readonly Level World1Level5 = new(
        shortName: "Level 1-5",
        name: "Capy’s Cloud Thoughts",
        description: "Discussing the nature of our thoughts",
       audioFile: new()
       {
           { AgeGroup.Child, "W1L5A0" },
           { AgeGroup.YoungTeen, "W1L5A0" },
           { AgeGroup.OldTeen, "W1L5A3" },
           { AgeGroup.Adult, "W1L5A3" }
       },
        bannerFile: "W1L5L_1",
        mapPosition: new Vector2(-286, -59),
        capyPosition: new Vector2(355.4f, -47),
        enumName: LevelEnum.W1L5);

    public static readonly Level World1Level6 = new(
        shortName: "Level 1-6",
        name: "Rainy Day Shelter",
        description: "Observing the sensations across our body",
       audioFile: new()
       {
           { AgeGroup.Child, "W1L6A0" },
           { AgeGroup.YoungTeen, "W1L6A0" },
           { AgeGroup.OldTeen, "W1L6A3" },
           { AgeGroup.Adult, "W1L6A3" }
       },
        bannerFile: "W1L6LD",
        mapPosition: new Vector2(-286, -200.525f),
        capyPosition: new Vector2(355.4f, 139),
        enumName: LevelEnum.W1L6);

    public static readonly Level World1Level7 = new(
        shortName: "Level 1-7",
        name: "Mirror Pool Check-In",
        description: "Learning to check-in with our emotions",
         audioFile: new()
         {
            { AgeGroup.Child, "W1L7A0" },
            { AgeGroup.YoungTeen, "W1L7A0" },
            { AgeGroup.OldTeen, "W1L7A3" },
            { AgeGroup.Adult, "W1L7A3" }
         },
        bannerFile: "W1L7L",
        mapPosition: new Vector2(-191, -200.525f),
        capyPosition: new Vector2(249, 219),
        enumName: LevelEnum.W1L7);

    public static readonly Level World1Level8 = new(
        shortName: "Level 1-8",
        name: "The Star Within",
        description: "Uncovering our inner wellbeing",
        audioFile: new()
        {
            { AgeGroup.Child, "W1L8A0" },
            { AgeGroup.YoungTeen, "W1L8A0" },
            { AgeGroup.OldTeen, "W1L8A3" },
            { AgeGroup.Adult, "W1L8A3" }
        },
        bannerFile: "W1L8L",
        mapPosition: new Vector2(16, -200.525f),
        capyPosition: new Vector2(44, 219),
        enumName: LevelEnum.W1L8);

    public static readonly Level World1Level9 = new(
        shortName: "Level 1-9",
        name: "Capy at the Crossroads",
        description: "Creating space between stimulus and response",
        audioFile: new()
        {
            { AgeGroup.Child, "W1L9A0" },
            { AgeGroup.YoungTeen, "W1L9A0" },
            { AgeGroup.OldTeen, "W1L9A3" },
            { AgeGroup.Adult, "W1L9A3" }
        },
        bannerFile: "W1L9L",
        mapPosition: new Vector2(141, -200.525f),
        capyPosition: new Vector2(-84, 346),
        enumName: LevelEnum.W1L9);

    public static readonly Level World1Level10 = new(
        shortName: "Level 1-10",
        name: "Campfire Reflections",
        description: "Reflecting on the journey so far",
        audioFile: new()
        {
            { AgeGroup.Child, "W1L10A0" },
            { AgeGroup.YoungTeen, "W1L10A0" },
            { AgeGroup.OldTeen, "W1L10A3" },
            { AgeGroup.Adult, "W1L10A3" }
        },
        bannerFile: "W1L10L",
        mapPosition: new Vector2(141, -200.525f),
        capyPosition: new Vector2(-84, 537),
        enumName: LevelEnum.W1L10);

    public static readonly Level World1SideLevel1 = new(
        shortName: "Side Level 1-1",
        name: "Capy on the Calm River",
        description: "Sounds as objects of awareness",
        audioFile: new()
        {
            { AgeGroup.Child, "W1SL1A0" },
            { AgeGroup.YoungTeen, "W1SL1A0" },
            { AgeGroup.OldTeen, "W1SL1A3" },
            { AgeGroup.Adult, "W1SL1A3" }
        },
        bannerFile: "W1SL1L",
        mapPosition: new Vector2(-74, -27),
        capyPosition: new Vector2(145.9f, -7),
        enumName: LevelEnum.W1SL1);

    public static readonly Level World1SideLevel2 = new(
        shortName: "Side Level 1-2",
        name: "Capy’s River Reflection",
        description: "The theory behind mindfulness",
        audioFile: new()
        {
            { AgeGroup.Child, "W1SL2A0" },
            { AgeGroup.YoungTeen, "W1SL2A0" },
            { AgeGroup.OldTeen, "W1SL2A3" },
            { AgeGroup.Adult, "W1SL2A3" }
        },
        bannerFile: "W1SL2L",
        mapPosition: new Vector2(-19, -29),
        capyPosition: new Vector2(-31.7f, 48.6f),
        enumName: LevelEnum.W1SL2);

    public static readonly Level World1SideLevel3 = new(
        shortName: "Side Level 1-3",
        name: "Capy’s Tea Chat",
        description: "Clarifying the practice of mindfulness",
        audioFile: new()
        {
            { AgeGroup.Child, "W1L8A0" },
            { AgeGroup.YoungTeen, "W1SL3A0" },
            { AgeGroup.OldTeen, "W1SL3A3" },
            { AgeGroup.Adult, "W1SL3A3" }
        },
        bannerFile: "W1SL3L",
        mapPosition: new Vector2(-485, -6),
        capyPosition: new Vector2(566, 48.9f),
        enumName: LevelEnum.W1SL3);

    public static readonly Level World1MiniLevel1 = new(
        shortName: "Mini Meditation 1-1",
        name: "Feeling Overwhelmed",
        description: "Using acceptance and letting go as tools during stressful times",
        audioFile: new()
        {
            { AgeGroup.Child, "W1MM1" },
            { AgeGroup.YoungTeen, "W1MM1" },
            { AgeGroup.OldTeen, "W1MM1" },
            { AgeGroup.Adult, "W1MM1" }
        },
        bannerFile: "W1MM1L",
        mapPosition: new Vector2(381, -200.525f),
        capyPosition: new Vector2(-307, 437),
        enumName: LevelEnum.W1MM1);

    public static readonly Level World1MiniLevel2 = new(
        shortName: "Mini Meditation 1-2",
        name: "Capy’s Inspiration",
        description: "Reminding you that small steps and gentle care make a big difference",
        audioFile: new()
        {
            { AgeGroup.Child, "W1MM2" },
            { AgeGroup.YoungTeen, "W1MM2" },
            { AgeGroup.OldTeen, "W1MM2" },
            { AgeGroup.Adult, "W1MM2" }
        },
        bannerFile: "W1MM2L",
        mapPosition: new Vector2(381, -200.525f),
        capyPosition: new Vector2(-453, 499),
        enumName: LevelEnum.W1MM2);

    public static readonly Level World1MiniLevel3 = new(
        shortName: "Mini Meditation 1-3",
        name: "A Moment of Rest",
        description: "Softening the mind and body for a moment of calm",
        audioFile: new()
        {
            { AgeGroup.Child, "W1MM3" },
            { AgeGroup.YoungTeen, "W1MM3" },
            { AgeGroup.OldTeen, "W1MM3" },
            { AgeGroup.Adult, "W1MM3" }
        },
        bannerFile: "W1MM3L",
        mapPosition: new Vector2(381, -200.525f),
        capyPosition: new Vector2(-450, 348),
        enumName: LevelEnum.W1MM3);

    public static readonly Dictionary<LevelEnum, Level> LevelLookup = new()
    {
        { LevelEnum.W1L1, World1Level1 },
        { LevelEnum.W1L2, World1Level2 },
        { LevelEnum.W1L3, World1Level3 },
        { LevelEnum.W1L4, World1Level4 },
        { LevelEnum.W1L5, World1Level5 },
        { LevelEnum.W1L6, World1Level6 },
        { LevelEnum.W1L7, World1Level7 },
        { LevelEnum.W1L8, World1Level8 },
        { LevelEnum.W1L9, World1Level9 },
        { LevelEnum.W1L10, World1Level10 },
        { LevelEnum.W1SL1, World1SideLevel1 },
        { LevelEnum.W1SL2, World1SideLevel2 },
        { LevelEnum.W1SL3, World1SideLevel3 },
        { LevelEnum.W1MM1, World1MiniLevel1 },
        { LevelEnum.W1MM2, World1MiniLevel2 },
        { LevelEnum.W1MM3, World1MiniLevel3 }
    };

    public string ShortName { get; }
    public string Name { get; }
    public string Description { get; }
    public Dictionary<AgeGroup, string> AudioFile { get; }
    public string BannerFile { get; }
    public Vector2 MapPosition { get; }
    public Vector2 CapyPosition { get; }
    public LevelEnum EnumName { get; }
    public World World
    {
        get
        {
            foreach (World world in World.WorldLookup.Values)
            {
                if (world.Levels.Contains(this)) return world;
            }
            throw new System.Exception("Level does not belong to any world");
        }
    }
    public static Dictionary<Level, Level[]> NextLevelMap = new()
    {
        {World1Level1, new Level[]{World1Level2} },
        {World1Level2, new Level[]{World1Level3 } },
        {World1Level3, new Level[]{World1Level4, World1SideLevel1} },
        {World1Level4, new Level[]{World1Level5 } },
        {World1Level5, new Level[]{World1Level6, World1SideLevel3 } },
        {World1Level6, new Level[]{World1Level7 } },
        {World1Level7, new Level[]{World1Level8 } },
        {World1Level8, new Level[]{World1Level9 } },
        {World1Level9, new Level[]{World1Level10, World1MiniLevel1, World1MiniLevel2, World1MiniLevel3 } },
        {World1Level10, new Level[]{} },
        {World1SideLevel1, new Level[]{World1SideLevel2 } },
        {World1SideLevel2, new Level[]{ } },
        {World1SideLevel3, new Level[]{ } },
        {World1MiniLevel1, new Level[]{ } },
        {World1MiniLevel2, new Level[]{ } },
        {World1MiniLevel3, new Level[]{ } },
    };

    public Level(string shortName, string name, string description, Dictionary<AgeGroup, string> audioFile, string bannerFile, Vector2 mapPosition, Vector2 capyPosition, LevelEnum enumName)
    {
        ShortName = shortName;
        Name = name;
        Description = description;
        AudioFile = audioFile;
        BannerFile = bannerFile;
        MapPosition = mapPosition;
        CapyPosition = capyPosition;
        EnumName = enumName;
    }
}

public enum WorldEnum
{
    FirstSteps
}

public class World
{
    public static readonly World FirstSteps = new(
        name: "First Steps",
        levels: new HashSet<Level>
        {
            Level.World1Level1,
            Level.World1Level2,
            Level.World1Level3,
            Level.World1Level4,
            Level.World1Level5,
            Level.World1Level6,
            Level.World1Level7,
            Level.World1Level8,
            Level.World1Level9,
            Level.World1Level10,
            Level.World1SideLevel1,
            Level.World1SideLevel2,
            Level.World1SideLevel3,
            Level.World1MiniLevel1,
            Level.World1MiniLevel2,
            Level.World1MiniLevel3
        },
        enumName: WorldEnum.FirstSteps
    );
    public string Name { get; }
    public HashSet<Level> Levels { get; }
    public WorldEnum EnumName { get; }
    public readonly static Dictionary<WorldEnum, World> WorldLookup = new()
    {
        { WorldEnum.FirstSteps, FirstSteps }
    };
    public static HashSet<World> AllWorlds = new() { FirstSteps };
    private World(string name, HashSet<Level> levels, WorldEnum enumName)
    {
        Name = name;
        Levels = levels;
        EnumName = enumName;
    }
}