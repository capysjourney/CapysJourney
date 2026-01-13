using System;
using System.Collections.Generic;

public enum LevelStatus
{
    Locked = 0,
    Available,
    Completed
}
public enum Mood
{
    Super, Good, Meh, Bad, Awful
}

public class MoodEntry
{
    public DateTime Timestamp;
    public Mood MoodLevel;
    public MoodEntry(DateTime date, Mood moodLevel)
    {
        Timestamp = date;
        MoodLevel = moodLevel;
    }
}

public class GratitudeEntry
{
    public DateTime Timestamp;
    public string Text;
    public GratitudeEntry(DateTime date, string text)
    {
        Timestamp = date;
        Text = text;
    }
}

public class PlayerStats
{
    public enum Badge
    {
        GettingCozy,
        SettlingIn,
        ZenMaster,
        AdventureAwaits,
        MindfulBeginnings,
        MeetingQuincy,
        TrailBlazer,
        // TODO - add back later
        //MindfulVoyager,
        //SereneExplorer,
        //CapyConqueror,
        TinyButMighty,
        LilyLounge,
        JustBreathe,
        DeepBreatheDevotee,
        BreathSage,
        FeelingFeeler,
        MoodMapper,
        InnerWeatherWatcher,
        SeedsOfGratitude,
        BloomingThanks,
        GratitudeGardener,
        TinyTrader,
        CapyCollector,
        CarrotTycoon,
        CapysClosetBegins,
        DressedToImpress,
        OneStepToCozy,
        CapyLovesHome,
        FreshFit,
    }

    public Dictionary<LevelEnum, LevelStatus> LevelStatuses = new();
    public HashSet<LevelEnum> BookmarkedLevels = new();

    /// <summary>
    /// A log of the player's mood entries, with the most recent entry at the front.
    /// Uses LinkedList for efficient addition/removal from both ends.
    /// </summary>
    public LinkedList<MoodEntry> MoodLog = new();

    /// <summary>
    /// A log of the player's gratitude entries, with the most recent entry at the back.
    /// Uses LinkedList for efficient addition/removal from both ends.
    /// </summary>
    public LinkedList<GratitudeEntry> GratitudeLog = new();

    /// <summary>
    /// The accessories owned by the player.
    /// </summary>
    public HashSet<Accessory> AccessoriesOwned = new();

    /// <summary>
    /// The current world the player is on.
    /// </summary>
    public WorldEnum CurrWorld = World.FirstSteps.EnumName;

    /// <summary>
    /// The current level the player is on.
    /// </summary>
    public LevelEnum CurrLevel = Level.World1Level1.EnumName;

    public Accessory CurrHat = null;
    public Accessory CurrNeckwear = null;
    public Accessory CurrClothing = null;
    public Accessory CurrFacewear = null;
    public Accessory CurrPet = null;

    public int NumCarrots = 0;
    public int BestStreak = 0;
    public int CurrStreak = 0;
    public float SecondsMeditated = 0;
    public int NumExercisesCompleted = 0;
    public int NumWorldsCompleted = 0;
    public DateTime LastBreathworkTime = DateTime.MinValue;
    public DateTime LastLogin = DateTime.MinValue;
    public DateTime TimeOfLastActivity = DateTime.MinValue;

    public HashSet<Badge> BadgesEarned = new();

    private const int MaxMoodEntries = 30;
    private const int MaxGratitudeEntries = 10;
    private const bool DebugMode = true; // todo - set to false for production

    public PlayerStats()
    {
        foreach (World world in World.AllWorlds)
        {
            foreach (Level level in world.Levels)
            {
                LevelStatuses[level.EnumName] = LevelStatus.Locked;
            }
        }
        LevelStatuses[Level.World1Level1.EnumName] = LevelStatus.Available;
        if (DebugMode)
        {
            NumCarrots = 1000;
            foreach (Accessory accessory in Accessory.AllAccesories)
            {
                if (accessory.Tier != Tier.Legendary)
                {
                    AccessoriesOwned.Add(accessory);
                }
            }
        }
    }

    public void UpdateLastLogin()
    {
        LastLogin = DateTime.Now.Date;
    }

    public void UpdateStreakForCompletedActivity()
    {
        if (TimeOfLastActivity.Date == DateTime.Now.Date.AddDays(-1))
        {
            IncreaseCurrStreak();
        }
        else if (TimeOfLastActivity.Date != DateTime.Now.Date)
        {
            CurrStreak = 1;
            BestStreak = Math.Max(BestStreak, 1);
        }
        TimeOfLastActivity = DateTime.Now;
    }

    public void MakeLevelAvailable(Level level)
    {
        if (GetLevelStatus(level) == LevelStatus.Locked)
        {
            LevelStatuses[level.EnumName] = LevelStatus.Available;
        }
    }

    private bool IsWorldComplete(World world)
    {
        foreach (Level level in world.Levels)
        {
            if (GetLevelStatus(level) != LevelStatus.Completed) return false;
        }
        return true;
    }

    public void CompleteLevel(Level level)
    {
        LevelStatus currStatus = GetLevelStatus(level);
        if (currStatus == LevelStatus.Completed)
        {
            return;
        }
        LevelStatuses[level.EnumName] = LevelStatus.Completed;
        NumExercisesCompleted++;
        if (IsWorldComplete(level.World))
        {
            NumWorldsCompleted++;
        }
        NumCarrots += 10;
    }

    public void ChangeLevel(Level level)
    {
        CurrLevel = level.EnumName;
        CurrWorld = level.World.EnumName;
    }

    public LevelStatus GetLevelStatus(Level level)
    {
        return LevelStatuses[level.EnumName];
    }

    public Dictionary<Level, LevelStatus> GetWorldStatus(World world)
    {
        Dictionary<Level, LevelStatus> result = new();
        foreach (Level level in world.Levels)
        {
            result[level] = GetLevelStatus(level);
        }
        return result;
    }

    public void IncreaseCarrots(int carrots)
    {
        NumCarrots += carrots;
    }

    public void IncreaseSecondsMeditated(float seconds)
    {
        SecondsMeditated += seconds;
    }

    public void EarnBadge(Badge badge)
    {
        BadgesEarned.Add(badge);
    }

    public void Bookmark(Level level, bool bookmark)
    {
        if (bookmark)
        {
            BookmarkedLevels.Add(level.EnumName);
        }
        else
        {
            BookmarkedLevels.Remove(level.EnumName);
        }
    }

    public void ToggleBookmark(Level level)
    {
        if (BookmarkedLevels.Contains(level.EnumName))
        {
            BookmarkedLevels.Remove(level.EnumName);
        }
        else
        {
            BookmarkedLevels.Add(level.EnumName);
        }
    }

    public bool IsLevelBookmarked(Level level)
    {
        return BookmarkedLevels.Contains(level.EnumName);
    }

    public void LogMood(Mood moodLevel, DateTime date)
    {
        MoodEntry newEntry = new(date, moodLevel);
        MoodLog.AddFirst(newEntry);
        while (MoodLog.Count > MaxMoodEntries)
        {
            MoodLog.RemoveLast();
        }
    }

    public void LogGratitude(string text, DateTime date)
    {
        GratitudeEntry newEntry = new(date, text);
        GratitudeLog.AddLast(newEntry);
        while (GratitudeLog.Count > MaxGratitudeEntries)
        {
            GratitudeLog.RemoveFirst();
        }
    }

    public void AddAccessory(Accessory accessory)
    {
        AccessoriesOwned.Add(accessory);
    }

    public void UseAccessory(Accessory accessory)
    {
        switch (accessory.Type)
        {
            case AccessoryType.Hat:
                CurrHat = accessory;
                break;
            case AccessoryType.Neckwear:
                CurrNeckwear = accessory;
                break;
            case AccessoryType.Clothing:
                CurrClothing = accessory;
                break;
            case AccessoryType.Facewear:
                CurrFacewear = accessory;
                break;
            case AccessoryType.Pet:
                CurrPet = accessory;
                break;
            default:
                throw new Exception("Unknown accessory type");
        }
    }
    public void StopUsingAccessory(AccessoryType type)
    {
        switch (type)
        {
            case AccessoryType.Hat:
                CurrHat = null;
                break;
            case AccessoryType.Neckwear:
                CurrNeckwear = null;
                break;
            case AccessoryType.Clothing:
                CurrClothing = null;
                break;
            case AccessoryType.Facewear:
                CurrFacewear = null;
                break;
            case AccessoryType.Pet:
                CurrPet = null;
                break;
            default:
                throw new Exception("Unknown accessory type");
        }
    }
    private void IncreaseCurrStreak()
    {
        CurrStreak++;
        BestStreak = Math.Max(BestStreak, CurrStreak);
    }

}
