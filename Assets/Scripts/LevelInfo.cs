using System;
using System.Collections.Generic;
using UnityEngine;

public enum Level
{
    FirstSteps_L1, FirstSteps_L2, FirstSteps_L3, FirstSteps_L4, FirstSteps_L5,
    FirstSteps_L6, FirstSteps_L7, FirstSteps_L8, FirstSteps_L9, FirstSteps_L10, 
    FirstSteps_SL1, FirstSteps_SL2, FirstSteps_SL3, FirstSteps_MM1, FirstSteps_MM2, FirstSteps_MM3,
    //PresentMoment_L1, PresentMoment_L2, PresentMoment_L3, PresentMoment_L4, PresentMoment_SL1,

}
public class LevelInfo
{
    public static readonly LevelInfo FirstStepsLevel1 = new(
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
       level: Level.FirstSteps_L1,
       world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsLevel2 = new(
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
        level: Level.FirstSteps_L2,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsLevel3 = new(
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
        level: Level.FirstSteps_L3,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsLevel4 = new(
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
        level: Level.FirstSteps_L4,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsLevel5 = new(
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
        level: Level.FirstSteps_L5,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsLevel6 = new(
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
        level: Level.FirstSteps_L6,
        world: World.FirstSteps);
    public static readonly LevelInfo FirstStepsLevel7 = new(
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
        level: Level.FirstSteps_L7,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsLevel8 = new(
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
        level: Level.FirstSteps_L8,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsLevel9 = new(
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
        level: Level.FirstSteps_L9,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsLevel10 = new(
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
        level: Level.FirstSteps_L10,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsSideLevel1 = new(
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
        level: Level.FirstSteps_SL1,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsSideLevel2 = new(
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
        level: Level.FirstSteps_SL2,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsSideLevel3 = new(
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
        level: Level.FirstSteps_SL3,
        world: World.FirstSteps);

    public static readonly LevelInfo FirstStepsMiniLevel1 = new(
        shortName: "Mini Meditation 1-1",
        name: "Feeling Overwhelmed",
        description: "Using acceptance and letting go as tools during stressful times",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/MM1A0",
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
        level: Level.FirstSteps_MM1,
        world: World.FirstSteps,
    isMiniMeditation: true);

    public static readonly LevelInfo FirstStepsMiniLevel2 = new(
        shortName: "Mini Meditation 1-2",
        name: "Capy’s Inspiration",
        description: "Reminding you that small steps and gentle care make a big difference",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/MM2A0",
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
        level: Level.FirstSteps_MM2,
        world: World.FirstSteps,
        isMiniMeditation: true);

    public static readonly LevelInfo FirstStepsMiniLevel3 = new(
        shortName: "Mini Meditation 1-3",
        name: "A Moment of Rest",
        description: "Softening the mind and body for a moment of calm",
        getAudioFilePathOfAgeGroup: ageGroup =>
        {
            return ageGroup switch
            {
                AgeGroup.Preschool => "LessonAudio/FirstSteps/MM3A0",
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
        level: Level.FirstSteps_MM3,
        world: World.FirstSteps,
        isMiniMeditation: true);


    public string ShortName { get; }
    public string Name { get; }
    public string Description { get; }
    public Func<AgeGroup, string> GetAudioFilePathOfAgeGroup { get; }
    public string BannerFile { get; }
    public Vector2 MapPosition { get; }
    public Vector2 CapyPosition { get; }
    public Level Level { get; }
    public bool IsMiniMeditation { get; }
    public World World { get; }

    public LevelInfo(string shortName,
        string name,
        string description,

        // using this instead of dictionary to ensure all AgeGroups are handled
        Func<AgeGroup, string> getAudioFilePathOfAgeGroup,

        string bannerFile,
        Vector2 mapPosition,
        Vector2 capyPosition,
        Level level,
        World world,
        bool isMiniMeditation = false)
    {
        ShortName = shortName;
        Name = name;
        Description = description;
        GetAudioFilePathOfAgeGroup = getAudioFilePathOfAgeGroup;
        BannerFile = bannerFile;
        MapPosition = mapPosition;
        CapyPosition = capyPosition;
        Level = level;
        World = world;
        IsMiniMeditation = isMiniMeditation;
    }
}

public static class LevelExtensions
{
    private static readonly Dictionary<Level, LevelInfo> InfoByLevel = new()
    {
        { Level.FirstSteps_L1, LevelInfo.FirstStepsLevel1 },
        { Level.FirstSteps_L2, LevelInfo.FirstStepsLevel2 },
        { Level.FirstSteps_L3, LevelInfo.FirstStepsLevel3 },
        { Level.FirstSteps_L4, LevelInfo.FirstStepsLevel4 },
        { Level.FirstSteps_L5, LevelInfo.FirstStepsLevel5 },
        { Level.FirstSteps_L6, LevelInfo.FirstStepsLevel6 },
        { Level.FirstSteps_L7, LevelInfo.FirstStepsLevel7 },
        { Level.FirstSteps_L8, LevelInfo.FirstStepsLevel8 },
        { Level.FirstSteps_L9, LevelInfo.FirstStepsLevel9 },
        { Level.FirstSteps_L10, LevelInfo.FirstStepsLevel10 },
        { Level.FirstSteps_SL1, LevelInfo.FirstStepsSideLevel1 },
        { Level.FirstSteps_SL2, LevelInfo.FirstStepsSideLevel2 },
        { Level.FirstSteps_SL3, LevelInfo.FirstStepsSideLevel3 },
        { Level.FirstSteps_MM1, LevelInfo.FirstStepsMiniLevel1 },
        { Level.FirstSteps_MM2, LevelInfo.FirstStepsMiniLevel2 },
        { Level.FirstSteps_MM3, LevelInfo.FirstStepsMiniLevel3 }
    };
    private static readonly Dictionary<Level, Level[]> NextLevelMap = new()
    {
        {Level.FirstSteps_L1, new Level[]{Level.FirstSteps_L2} },
        {Level.FirstSteps_L2, new Level[]{Level.FirstSteps_L3 } },
        {Level.FirstSteps_L3, new Level[]{Level.FirstSteps_L4, Level.FirstSteps_SL1} },
        {Level.FirstSteps_L4, new Level[]{Level.FirstSteps_L5 } },
        {Level.FirstSteps_L5, new Level[]{Level.FirstSteps_L6, Level.FirstSteps_SL3 } },
        {Level.FirstSteps_L6, new Level[]{Level.FirstSteps_L7 } },
        {Level.FirstSteps_L7, new Level[]{Level.FirstSteps_L8 } },
        {Level.FirstSteps_L8, new Level[]{Level.FirstSteps_L9 } },
        {Level.FirstSteps_L9, new Level[]{Level.FirstSteps_L10, Level.FirstSteps_MM1, Level.FirstSteps_MM2, Level.FirstSteps_MM3 } },
        {Level.FirstSteps_L10, new Level[]{} },
        {Level.FirstSteps_SL1, new Level[]{Level.FirstSteps_SL2 } },
        {Level.FirstSteps_SL2, new Level[]{ } },
        {Level.FirstSteps_SL3, new Level[]{ } },
        {Level.FirstSteps_MM1, new Level[]{ } },
        {Level.FirstSteps_MM2, new Level[]{ } },
        {Level.FirstSteps_MM3, new Level[]{ } },
    };
    public static World GetWorld(this Level level)
    {
        return InfoByLevel[level].World;
    }
    public static Level[] GetNextLevels(this Level level)
    {
        return NextLevelMap[level];
    }
    public static LevelInfo GetInfo(this Level level)
    {
        return InfoByLevel[level];
    }
}
