using System;
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
       getAudioFilePathOfAgeGroup: ageGroup =>
       {
           return ageGroup switch
           {
               AgeGroup.Preschool => "LessonAudio/FirstSteps/L1A0",
               AgeGroup.Child => "LessonAudio/FirstSteps/L1A1",
               AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L1A2",
               AgeGroup.OldTeen => "LessonAudio/FirstSteps/L1A3",
               AgeGroup.Adult => "LessonAudio/FirstSteps/L1A4",
               _ => "LessonAudio/FirstSteps/L1A4"
           };
       },
       bannerFile: "BannerFiles/FirstSteps/L1L",
       mapPosition: new Vector2(129, 200.525f),
       capyPosition: new Vector2(-84, -440),
       enumName: LevelEnum.W1L1);

    public static readonly Level World1Level2 = new(
        shortName: "Level 1-2",
        name: "The Fading Bell",
        description: "Investigating awareness with a fading bell",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/L2A0",
                AgeGroup.Child => "LessonAudio/FirstSteps/L2A1",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L2A2",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/L2A3",
                AgeGroup.Adult => "LessonAudio/FirstSteps/L2A4",
                _ => "LessonAudio/FirstSteps/L2A4"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/L2L",
        mapPosition: new Vector2(129, 200.525f),
        capyPosition: new Vector2(-84, -248),
        enumName: LevelEnum.W1L2);

    public static readonly Level World1Level3 = new(
        shortName: "Level 1-3",
        name: "Capy Meets the Breathing Cloud",
        description: "Learning to focus on the breath",
       getAudioFilePathOfAgeGroup: ageGroup =>
       {
           return ageGroup switch
           {
               AgeGroup.Preschool => "LessonAudio/FirstSteps/L3A0",
               AgeGroup.Child => "LessonAudio/FirstSteps/L3A1",
               AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L3A2",
               AgeGroup.OldTeen => "LessonAudio/FirstSteps/L3A3",
               AgeGroup.Adult => "LessonAudio/FirstSteps/L3A4",
               _ => "LessonAudio/FirstSteps/L3A4"
           };
       },
        bannerFile: "BannerFiles/FirstSteps/L3L",
        mapPosition: new Vector2(9, 66),
        capyPosition: new Vector2(33, -165),
        enumName: LevelEnum.W1L3);

    public static readonly Level World1Level4 = new(
        shortName: "Level 1-4",
        name: "Mailbox of Friendly Wishes",
        description: "Extending compassion to ourself and others",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/L4A0",
                AgeGroup.Child => "LessonAudio/FirstSteps/L4A1",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L4A2",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/L4A3",
                AgeGroup.Adult => "LessonAudio/FirstSteps/L4A4",
                _ => "LessonAudio/FirstSteps/L4A4"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/L4L",
        mapPosition: new Vector2(-210, 66),
        capyPosition: new Vector2(259, -165),
        enumName: LevelEnum.W1L4);

    public static readonly Level World1Level5 = new(
        shortName: "Level 1-5",
        name: "Capy’s Cloud Thoughts",
        description: "Discussing the nature of our thoughts",
       getAudioFilePathOfAgeGroup: ageGroup =>
       {
           return ageGroup switch
           {
               AgeGroup.Preschool => "LessonAudio/FirstSteps/L5A0",
               AgeGroup.Child => "LessonAudio/FirstSteps/L5A1",
               AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L5A2",
               AgeGroup.OldTeen => "LessonAudio/FirstSteps/L5A3",
               AgeGroup.Adult => "LessonAudio/FirstSteps/L5A4",
               _ => "LessonAudio/FirstSteps/L5A4"
           };
       },
        bannerFile: "BannerFiles/FirstSteps/L5L_1",
        mapPosition: new Vector2(-286, -59),
        capyPosition: new Vector2(355.4f, -47),
        enumName: LevelEnum.W1L5);

    public static readonly Level World1Level6 = new(
        shortName: "Level 1-6",
        name: "Rainy Day Shelter",
        description: "Observing the sensations across our body",
       getAudioFilePathOfAgeGroup: ageGroup =>
       {
           return ageGroup switch
           {
               AgeGroup.Preschool => "LessonAudio/FirstSteps/L6A0",
               AgeGroup.Child => "LessonAudio/FirstSteps/L6A1",
               AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L6A2",
               AgeGroup.OldTeen => "LessonAudio/FirstSteps/L6A3",
               AgeGroup.Adult => "LessonAudio/FirstSteps/L6A4",
               _ => "LessonAudio/FirstSteps/L6A4"
           };
       },
        bannerFile: "BannerFiles/FirstSteps/L6LD",
        mapPosition: new Vector2(-286, -200.525f),
        capyPosition: new Vector2(355.4f, 139),
        enumName: LevelEnum.W1L6);

    public static readonly Level World1Level7 = new(
        shortName: "Level 1-7",
        name: "Mirror Pool Check-In",
        description: "Learning to check-in with our emotions",
         getAudioFilePathOfAgeGroup: ageGroup =>
         {
             return ageGroup switch
             {
                 AgeGroup.Preschool => "LessonAudio/FirstSteps/L7A0",
                 AgeGroup.Child => "LessonAudio/FirstSteps/L7A1",
                 AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L7A2",
                 AgeGroup.OldTeen => "LessonAudio/FirstSteps/L7A3",
                 AgeGroup.Adult => "LessonAudio/FirstSteps/L7A4",
                 _ => "LessonAudio/FirstSteps/L7A4"
             };
         },
        bannerFile: "BannerFiles/FirstSteps/L7L",
        mapPosition: new Vector2(-191, -200.525f),
        capyPosition: new Vector2(249, 219),
        enumName: LevelEnum.W1L7);

    public static readonly Level World1Level8 = new(
        shortName: "Level 1-8",
        name: "The Star Within",
        description: "Uncovering our inner wellbeing",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/L8A0",
                AgeGroup.Child => "LessonAudio/FirstSteps/L8A1",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L8A2",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/L8A3",
                AgeGroup.Adult => "LessonAudio/FirstSteps/L8A4",
                _ => "LessonAudio/FirstSteps/L8A4"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/L8L",
        mapPosition: new Vector2(16, -200.525f),
        capyPosition: new Vector2(44, 219),
        enumName: LevelEnum.W1L8);

    public static readonly Level World1Level9 = new(
        shortName: "Level 1-9",
        name: "Capy at the Crossroads",
        description: "Creating space between stimulus and response",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/L9A0",
                AgeGroup.Child => "LessonAudio/FirstSteps/L9A1",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L9A2",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/L9A3",
                AgeGroup.Adult => "LessonAudio/FirstSteps/L9A4",
                _ => "LessonAudio/FirstSteps/L9A4"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/L9L",
        mapPosition: new Vector2(141, -200.525f),
        capyPosition: new Vector2(-84, 346),
        enumName: LevelEnum.W1L9);

    public static readonly Level World1Level10 = new(
        shortName: "Level 1-10",
        name: "Campfire Reflections",
        description: "Reflecting on the journey so far",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/L10A0",
                AgeGroup.Child => "LessonAudio/FirstSteps/L10A1",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/L10A2",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/L10A3",
                AgeGroup.Adult => "LessonAudio/FirstSteps/L10A4",
                _ => "LessonAudio/FirstSteps/L10A4"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/L10L",
        mapPosition: new Vector2(141, -200.525f),
        capyPosition: new Vector2(-84, 537),
        enumName: LevelEnum.W1L10);

    public static readonly Level World1SideLevel1 = new(
        shortName: "Side Level 1-1",
        name: "Capy on the Calm River",
        description: "Sounds as objects of awareness",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/SL1A0",
                AgeGroup.Child => "LessonAudio/FirstSteps/SL1A1",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/SL1A2",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/SL1A3",
                AgeGroup.Adult => "LessonAudio/FirstSteps/SL1A4",
                _ => "LessonAudio/FirstSteps/SL1A4"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/SL1L",
        mapPosition: new Vector2(-74, -27),
        capyPosition: new Vector2(145.9f, -7),
        enumName: LevelEnum.W1SL1);

    public static readonly Level World1SideLevel2 = new(
        shortName: "Side Level 1-2",
        name: "Capy’s River Reflection",
        description: "The theory behind mindfulness",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/SL2A0",
                AgeGroup.Child => "LessonAudio/FirstSteps/SL2A1",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/SL2A2",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/SL2A3",
                AgeGroup.Adult => "LessonAudio/FirstSteps/SL2A4",
                _ => "LessonAudio/FirstSteps/SL2A4"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/SL2L",
        mapPosition: new Vector2(-19, -29),
        capyPosition: new Vector2(-31.7f, 48.6f),
        enumName: LevelEnum.W1SL2);

    public static readonly Level World1SideLevel3 = new(
        shortName: "Side Level 1-3",
        name: "Capy’s Tea Chat",
        description: "Clarifying the practice of mindfulness",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/SL3A0",
                AgeGroup.Child => "LessonAudio/FirstSteps/SL3A1",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/SL3A2",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/SL3A3",
                AgeGroup.Adult => "LessonAudio/FirstSteps/SL3A4",
                _ => "LessonAudio/FirstSteps/SL3A4"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/SL3L",
        mapPosition: new Vector2(-485, -6),
        capyPosition: new Vector2(566, 48.9f),
        enumName: LevelEnum.W1SL3);

    public static readonly Level World1MiniLevel1 = new(
        shortName: "Mini Meditation 1-1",
        name: "Feeling Overwhelmed",
        description: "Using acceptance and letting go as tools during stressful times",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/MM1",
                AgeGroup.Child => "LessonAudio/FirstSteps/MM1",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/MM1",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/MM1",
                AgeGroup.Adult => "LessonAudio/FirstSteps/MM1",
                _ => "LessonAudio/FirstSteps/MM1"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/MM1L",
        mapPosition: new Vector2(381, -200.525f),
        capyPosition: new Vector2(-307, 437),
        enumName: LevelEnum.W1MM1,
        isMiniMeditation: true);

    public static readonly Level World1MiniLevel2 = new(
        shortName: "Mini Meditation 1-2",
        name: "Capy’s Inspiration",
        description: "Reminding you that small steps and gentle care make a big difference",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/MM2",
                AgeGroup.Child => "LessonAudio/FirstSteps/MM2",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/MM2",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/MM2",
                AgeGroup.Adult => "LessonAudio/FirstSteps/MM2",
                _ => "LessonAudio/FirstSteps/MM2"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/MM2L",
        mapPosition: new Vector2(381, -200.525f),
        capyPosition: new Vector2(-453, 499),
        enumName: LevelEnum.W1MM2,
        isMiniMeditation: true);

    public static readonly Level World1MiniLevel3 = new(
        shortName: "Mini Meditation 1-3",
        name: "A Moment of Rest",
        description: "Softening the mind and body for a moment of calm",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/MM3",
                AgeGroup.Child => "LessonAudio/FirstSteps/MM3",
                AgeGroup.YoungTeen => "LessonAudio/FirstSteps/MM3",
                AgeGroup.OldTeen => "LessonAudio/FirstSteps/MM3",
                AgeGroup.Adult => "LessonAudio/FirstSteps/MM3",
                _ => "LessonAudio/FirstSteps/MM3"
            };
        },
        bannerFile: "BannerFiles/FirstSteps/MM3L",
        mapPosition: new Vector2(381, -200.525f),
        capyPosition: new Vector2(-450, 348),
        enumName: LevelEnum.W1MM3,
        isMiniMeditation: true);

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
    public Func<AgeGroup, string> GetAudioFilePathOfAgeGroup { get; }
    public string BannerFile { get; }
    public Vector2 MapPosition { get; }
    public Vector2 CapyPosition { get; }
    public LevelEnum EnumName { get; }
    public bool IsMiniMeditation { get; }
    public World World
    {
        get
        {
            foreach (World world in World.WorldLookup.Values)
            {
                if (world.Levels.Contains(this)) return world;
            }
            throw new Exception("Level does not belong to any world");
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

    public Level(string shortName,
        string name,
        string description,

        // using this instead of dictionary to ensure all AgeGroups are handled
        Func<AgeGroup, string> getAudioFilePathOfAgeGroup,

        string bannerFile,
        Vector2 mapPosition,
        Vector2 capyPosition,
        LevelEnum enumName,
        bool isMiniMeditation = false)
    {
        ShortName = shortName;
        Name = name;
        Description = description;
        GetAudioFilePathOfAgeGroup = getAudioFilePathOfAgeGroup;
        BannerFile = bannerFile;
        MapPosition = mapPosition;
        CapyPosition = capyPosition;
        EnumName = enumName;
        IsMiniMeditation = isMiniMeditation;
    }
}
