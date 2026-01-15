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
    // IMPORTANT: All relevant fields should be public for serialization to work properly.
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

    public HashSet<Accessory> AccessoriesOwned = new();
    public HashSet<Furniture> FurnituresOwned = new();

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
    public int NumLevelsCompleted = 0;
    public int NumWorldsCompleted = 0;
    public int NumMiniMeditationsCompleted = 0;
    public int NumBreathworkSessionsCompleted = 0;
    public int NumCarrotsSpent = 0;

    // Note - this is not equal to MoodLog.Count since MoodLog removes old entries
    public int NumMoodLogEntriesCompleted = 0;

    // Note - this is not equal to GratitudeLog.Count since GratitudeLog removes old entries
    public int NumGratitudesCompleted = 0;

    public bool HasUncoveredLilyPad = false;
    public bool HasUsedAccessory = false;
    public DateTime LastBreathworkTime = DateTime.MinValue;
    public DateTime LastLogin = DateTime.MinValue;
    public DateTime TimeOfLastActivity = DateTime.MinValue;
    public bool HasCompletedQuincy = false;

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
        List<Badge> badges = UpdateAndGetNewStreakBadges();
        // todo - do something with badges
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
        NumLevelsCompleted++;
        if (IsWorldComplete(level.World))
        {
            NumWorldsCompleted++;
        }
        if (level.IsMiniMeditation)
        {
            NumMiniMeditationsCompleted++;
        }
        IncreaseCarrots(10);
        List<Badge> badges = UpdateAndGetNewLevelBadges();
        // todo - do something with badges
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

    private bool CompletedWorld(World world)
    {
        foreach (Level level in world.Levels)
        {
            if (GetLevelStatus(level) != LevelStatus.Completed)
            {
                return false;
            }
        }
        return true;
    }

    public void IncreaseCarrots(int carrots)
    {
        NumCarrots += carrots;
        if (carrots < 0)
        {
            NumCarrotsSpent -= carrots;
        }
        List<Badge> badges = UpdateAndGetNewCarrotsBadges();
        // todo - do something with badges
    }

    public void IncreaseSecondsMeditated(float seconds)
    {
        SecondsMeditated += seconds;
    }

    public void CompleteBreathworkSession(int durationInSeconds)
    {
        NumBreathworkSessionsCompleted++;
        LastBreathworkTime = DateTime.Now;
        IncreaseSecondsMeditated(durationInSeconds);
        List<Badge> badges = UpdateAndGetNewBreathworkBadges();
        // todo - do something with badges
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
        NumMoodLogEntriesCompleted++;
        List<Badge> badges = UpdateAndGetNewMoodBadges();
        // todo - do something with badges
    }

    public void LogGratitude(string text, DateTime date)
    {
        GratitudeEntry newEntry = new(date, text);
        GratitudeLog.AddLast(newEntry);
        while (GratitudeLog.Count > MaxGratitudeEntries)
        {
            GratitudeLog.RemoveFirst();
        }
        NumGratitudesCompleted++;
        List<Badge> badges = UpdateAndGetNewGratitudeBadges();
        // todo - do something with badges
    }

    public void AddAccessory(Accessory accessory)
    {
        AccessoriesOwned.Add(accessory);
        List<Badge> badges = UpdateAndGetNewAccessoryBadges();
        // todo - do something with badges
    }

    public void UseAccessory(Accessory accessory)
    {
        HasUsedAccessory = true;
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
        List<Badge> badges = UpdateAndGetNewAccessoryUseBadge();
        // todo - do something with badges
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

    public void CompleteQuincy()
    {
        HasCompletedQuincy = true;
        List<Badge> badges = UpdateAndGetNewQuincyBadges();
        // todo - do something with badges
    }

    private void IncreaseCurrStreak()
    {
        CurrStreak++;
        BestStreak = Math.Max(BestStreak, CurrStreak);
    }

    public List<Badge> UpdateAndGetNewStreakBadges()
    {
        List<Badge> newBadges = new();
        if ((!BadgesEarned.Contains(Badge.GettingCozy)))
        {
            if (CurrStreak >= 3)
            {
                newBadges.Add(Badge.GettingCozy);
            }
        }
        if (!BadgesEarned.Contains(Badge.SettlingIn))
        {
            if (CurrStreak >= 7)
            {
                newBadges.Add(Badge.SettlingIn);
            }
        }
        if (!BadgesEarned.Contains(Badge.ZenMaster))
        {
            if (CurrStreak >= 50)
            {
                newBadges.Add(Badge.ZenMaster);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewBreathworkBadges()
    {
        List<Badge> newBadges = new();
        if (!BadgesEarned.Contains(Badge.JustBreathe))
        {
            if (NumBreathworkSessionsCompleted >= 5)
            {
                newBadges.Add(Badge.JustBreathe);
            }
        }
        if (!BadgesEarned.Contains(Badge.DeepBreatheDevotee))
        {
            if (NumBreathworkSessionsCompleted >= 25)
            {
                newBadges.Add(Badge.DeepBreatheDevotee);
            }
        }
        if (!BadgesEarned.Contains(Badge.BreathSage))
        {
            if (NumBreathworkSessionsCompleted >= 50)
            {
                newBadges.Add(Badge.BreathSage);
            }
        }
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewGratitudeBadges()
    {
        List<Badge> newBadges = new();
        if (!BadgesEarned.Contains(Badge.SeedsOfGratitude))
        {
            if (NumGratitudesCompleted >= 5)
            {
                newBadges.Add(Badge.SeedsOfGratitude);
            }
        }
        if (!BadgesEarned.Contains(Badge.BloomingThanks))
        {
            if (NumGratitudesCompleted >= 25)
            {
                newBadges.Add(Badge.BloomingThanks);
            }
        }
        if (!BadgesEarned.Contains(Badge.GratitudeGardener))
        {
            if (NumGratitudesCompleted >= 50)
            {
                newBadges.Add(Badge.GratitudeGardener);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewMoodBadges()
    {
        List<Badge> newBadges = new();
        if (!BadgesEarned.Contains(Badge.FeelingFeeler))
        {
            if (NumMoodLogEntriesCompleted >= 5)
            {
                newBadges.Add(Badge.FeelingFeeler);
            }
        }
        if (!BadgesEarned.Contains(Badge.MoodMapper))
        {
            if (NumMoodLogEntriesCompleted >= 25)
            {
                newBadges.Add(Badge.MoodMapper);
            }
        }
        if (!BadgesEarned.Contains(Badge.InnerWeatherWatcher))
        {
            if (NumMoodLogEntriesCompleted >= 50)
            {
                newBadges.Add(Badge.InnerWeatherWatcher);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewCarrotsBadges()
    {
        List<Badge> newBadges = new();
        if (!BadgesEarned.Contains(Badge.TinyTrader))
        {
            if (NumCarrotsSpent >= 100)
            {
                newBadges.Add(Badge.TinyTrader);
            }
        }
        if (!BadgesEarned.Contains(Badge.CapyCollector))
        {
            if (NumCarrotsSpent >= 500)
            {
                newBadges.Add(Badge.CapyCollector);
            }
        }
        if (!BadgesEarned.Contains(Badge.CarrotTycoon))
        {
            if (NumCarrotsSpent >= 1000)
            {
                newBadges.Add(Badge.CarrotTycoon);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewQuincyBadges()
    {
        List<Badge> newBadges = new();
        if (!BadgesEarned.Contains(Badge.MeetingQuincy))
        {
            if (HasCompletedQuincy)
            {
                newBadges.Add(Badge.MeetingQuincy);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewAccessoryBadges()
    {
        List<Badge> newBadges = new();

        if (!BadgesEarned.Contains(Badge.CapysClosetBegins))
        {
            if (AccessoriesOwned.Count >= 1)
            {
                newBadges.Add(Badge.CapysClosetBegins);
            }
        }
        if (!BadgesEarned.Contains(Badge.DressedToImpress))
        {
            if (AccessoriesOwned.Count >= 10)
            {
                newBadges.Add(Badge.CapysClosetBegins);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }

    // todo - use
    public List<Badge> UpdateAndGetNewFurnitureBadges()
    {
        List<Badge> newBadges = new();
        if (!BadgesEarned.Contains(Badge.OneStepToCozy))
        {
            if (FurnituresOwned.Count >= 1)
            {
                newBadges.Add(Badge.OneStepToCozy);
            }
        }
        if (!BadgesEarned.Contains(Badge.CapyLovesHome))
        {
            if (FurnituresOwned.Count >= 10)
            {
                newBadges.Add(Badge.CapyLovesHome);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewLevelBadges()
    {
        List<Badge> newBadges = new();
        if (!BadgesEarned.Contains(Badge.AdventureAwaits))
        {
            if (GetLevelStatus(Level.World1Level1) == LevelStatus.Completed)
            {
                newBadges.Add(Badge.AdventureAwaits);
            }
        }
        if (!BadgesEarned.Contains(Badge.TrailBlazer))
        {
            if (NumLevelsCompleted > World.FirstSteps.Levels.Count)
            {
                newBadges.Add(Badge.TrailBlazer);
            }
        }
        if (!BadgesEarned.Contains(Badge.TinyButMighty))
        {
            if (NumMiniMeditationsCompleted >= 3)
            {
                newBadges.Add(Badge.TinyButMighty);
            }
        }
        if (!BadgesEarned.Contains(Badge.MindfulBeginnings))
        {
            if (CompletedWorld(World.FirstSteps))
            {
                newBadges.Add(Badge.MindfulBeginnings);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }

    // todo - use
    public List<Badge> UpdateAndGetNewLilyPadBadges()
    {
        List<Badge> newBadges = new();
        if (!BadgesEarned.Contains(Badge.LilyLounge))
        {
            if (HasUncoveredLilyPad)
            {
                newBadges.Add(Badge.LilyLounge);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewAccessoryUseBadge()
    {
        List<Badge> newBadges = new();
        if (!BadgesEarned.Contains(Badge.FreshFit))
        {
            if (HasUsedAccessory)
            {
                newBadges.Add(Badge.FreshFit);
            }
        }
        foreach (Badge badge in newBadges)
        {
            EarnBadge(badge);
        }
        return newBadges;
    }
}
