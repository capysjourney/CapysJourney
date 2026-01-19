using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LevelStatus { Locked = 0, Available, Completed }
public enum Mood { Super, Good, Meh, Bad, Awful }

[Serializable]
public class GuestPlayerData
{
    public int NumCarrots;
    public int BestStreak;
    public int CurrStreak;
    public float SecondsMeditated;
    public int NumExercisesCompleted;
    public int NumWorldsCompleted;
    public long LastBreathworkTime;
    public long LastLogin;
    public string CurrWorld;
    public string CurrLevel;
    public List<string> BadgesEarned;
    public List<string> BookmarkedLevels;
    public SerializableDictionary LevelStatuses;
}

[Serializable]
public class SerializableDictionary
{
    public List<string> Keys = new();
    public List<int> Values = new();

    public Dictionary<string, int> ToDictionary() => Keys.Zip(Values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
}

[Serializable]
public class MoodEntryDTO { public long Timestamp; public int MoodLevel; }
[Serializable]
public class MeditationEntryDTO { public long Timestamp; public int Duration; public int Interval; public string Chime; public string Effect; }
[Serializable]
public class GratitudeEntryDTO { public long Timestamp; public string Text; }

public class MoodEntry
{
    public DateTime Timestamp;
    public Mood MoodLevel;
    public MoodEntry(DateTime date, Mood moodLevel) { Timestamp = date; MoodLevel = moodLevel; }
}

public class GratitudeEntry
{
    public DateTime Timestamp;
    public string Text;
    public GratitudeEntry(DateTime date, string text) { Timestamp = date; Text = text; }
}

    public class PlayerStats
{
    private string guestId = Guid.NewGuid().ToString();
    private string PlayerPrefsKey => $"GuestPlayerStats_{guestId}";
    private const bool DebugMode = true;

    public bool IsGuest = false;

    public enum Badge
    {
        GettingCozy, SettlingIn, ZenMaster, AdventureAwaits, MindfulBeginnings,
        TrailBlazer, TinyButMighty, LilyLounge, JustBreathe, DeepBreatheDevotee,
        BreathSage, FeelingFeeler, MoodMapper, InnerWeatherWatcher,
        SeedsOfGratitude, BloomingThanks, GratitudeGardener,
        CapysClosetBegins, DressedToImpress, FreshFit,
        OneStepToCozy, CapyLovesHome, TinyTrader, CapyCollector, CarrotTycoon
    }

    private readonly FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
    private readonly string uid;
    private readonly DocumentReference docRef;

    public Dictionary<LevelEnum, LevelStatus> StatusOfLevel = new();
    public Dictionary<WorldEnum, LevelStatus> QuincyStatusOfWorld = new();
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

    public WorldEnum CurrWorld = World.FirstSteps.EnumName;
    public LevelEnum CurrLevel = Level.World1Level1.EnumName;

    public Accessory CurrHat, CurrNeckwear, CurrClothing, CurrFacewear, CurrPet;

    public int NumCarrots = 10;
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
    public LinkedList<MeditationEntry> MeditationLog = new();
    public BadgesDisplayed BadgesCurrentlyDisplayed = new();

    private const int MaxMoodEntries = 30;
    private const int MaxGratitudeEntries = 10;

    public PlayerStats(bool isGuest)
    {
        IsGuest = isGuest;
        if (!IsGuest)
        {
            uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            docRef = db.Collection("users").Document(uid);
        }

        foreach (World world in World.AllWorlds)
        {
            foreach (Level level in world.Levels)
            {
                StatusOfLevel[level.EnumName] = LevelStatus.Locked;
            }
            QuincyStatusOfWorld[world.EnumName] = LevelStatus.Locked;
        }

        StatusOfLevel[Level.World1Level1.EnumName] = LevelStatus.Available;

        if (DebugMode)
        {
            foreach (Accessory a in Accessory.AllAccesories)
                if (a.Tier != Tier.Legendary && a.Tier != Tier.Epic)
                    AccessoriesOwned.Add(a);
            NumCarrots = 1000;
        }
    }

    public void SaveToFirestore()
    {
        if (!IsGuest)
        {
            //UnityEngine.Debug.Log("Saving player stats to Firestore");

            var meditationListDict = MeditationLog.Select(m => new Dictionary<string, object>
            {
                { "Duration", m.duration },
                { "Interval", m.interval },
                { "Chime", m.chime },
                { "Effect", m.effect }
            }).ToList();

            var data = new Dictionary<string, object>
            {
                { "NumCarrots", NumCarrots },
                { "BestStreak", BestStreak },
                { "CurrStreak", CurrStreak },
                { "SecondsMeditated", SecondsMeditated },
                { "NumExercisesCompleted", NumExercisesCompleted },
                { "NumWorldsCompleted", NumWorldsCompleted },
                { "LastBreathworkTime", LastBreathworkTime.Ticks },
                { "LastLogin", LastLogin.Ticks },
                { "CurrWorld", CurrWorld.ToString() },
                { "CurrLevel", CurrLevel.ToString() },
                { "BadgesEarned", BadgesEarned.Select(b => b.ToString()).ToList() },
                { "BookmarkedLevels", BookmarkedLevels.Select(l => l.ToString()).ToList() },
                { "LevelStatuses", StatusOfLevel.ToDictionary(k => k.Key.ToString(), v => (int)v.Value) },
                { "MoodLog", MoodLog.Select(m => new Dictionary<string, object>
                    {
                        { "Timestamp", m.Timestamp.Ticks },
                        { "MoodLevel", (int)m.MoodLevel }
                    }).ToList()
                },
                { "GratitudeLog", GratitudeLog.Select(g => new Dictionary<string, object>
                    {
                        { "Timestamp", g.Timestamp.Ticks },
                        { "Text", g.Text }
                    }).ToList()
                },
                { "MeditationLog", meditationListDict }
            };

            docRef.SetAsync(data);
        }
        else
        {
            //UnityEngine.Debug.Log("Saving guest player stats to PlayerPrefs");

            var guestData = new
            {
                NumCarrots,
                BestStreak,
                CurrStreak,
                SecondsMeditated,
                NumExercisesCompleted,
                NumWorldsCompleted,
                LastBreathworkTime = LastBreathworkTime.Ticks,
                LastLogin = LastLogin.Ticks,
                CurrWorld = CurrWorld.ToString(),
                CurrLevel = CurrLevel.ToString(),
                BadgesEarned = BadgesEarned.Select(b => b.ToString()).ToList(),
                BookmarkedLevels = BookmarkedLevels.Select(l => l.ToString()).ToList(),
                LevelStatuses = StatusOfLevel.ToDictionary(k => k.Key.ToString(), v => (int)v.Value)
            };

            string json = JsonUtility.ToJson(guestData);
            PlayerPrefs.SetString(PlayerPrefsKey, json);
            PlayerPrefs.Save();
        }
    }

    public void LoadFromFirestore()
    {
        if (!IsGuest && !DebugMode)
        {
            //UnityEngine.Debug.Log("Loading player stats from Firestore");

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.Result.Exists) return;
                var d = task.Result;

                NumCarrots = d.GetValue<int>("NumCarrots");
                BestStreak = d.GetValue<int>("BestStreak");
                CurrStreak = d.GetValue<int>("CurrStreak");
                SecondsMeditated = d.GetValue<float>("SecondsMeditated");
                NumExercisesCompleted = d.GetValue<int>("NumExercisesCompleted");
                NumWorldsCompleted = d.GetValue<int>("NumWorldsCompleted");

                LastBreathworkTime = new DateTime(d.GetValue<long>("LastBreathworkTime"));
                LastLogin = new DateTime(d.GetValue<long>("LastLogin"));

                if (d.ContainsField("MeditationLog"))
                {
                    var list = d.GetValue<List<MeditationEntryDTO>>("MeditationLog");
                    MeditationLog = new LinkedList<MeditationEntry>();
                    foreach (var dto in list)
                    {
                        MeditationLog.AddLast(new MeditationEntry
                        {
                            duration = dto.Duration,
                            interval = dto.Interval,
                            chime = dto.Chime,
                            effect = dto.Effect
                        });
                    }
                }
            });
        }
        else
        {
            UnityEngine.Debug.Log("Loading guest player stats from PlayerPrefs");

            if (!PlayerPrefs.HasKey(PlayerPrefsKey)) return;

            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            var guestData = JsonUtility.FromJson<GuestPlayerData>(json);

            NumCarrots = guestData.NumCarrots;
            BestStreak = guestData.BestStreak;
            CurrStreak = guestData.CurrStreak;
            SecondsMeditated = guestData.SecondsMeditated;
            NumExercisesCompleted = guestData.NumExercisesCompleted;
            NumWorldsCompleted = guestData.NumWorldsCompleted;
            LastBreathworkTime = new DateTime(guestData.LastBreathworkTime);
            LastLogin = new DateTime(guestData.LastLogin);
            CurrWorld = Enum.Parse<WorldEnum>(guestData.CurrWorld);
            CurrLevel = Enum.Parse<LevelEnum>(guestData.CurrLevel);

            BadgesEarned = new HashSet<Badge>(guestData.BadgesEarned.Select(Enum.Parse<Badge>));
            BookmarkedLevels = new HashSet<LevelEnum>(guestData.BookmarkedLevels.Select(Enum.Parse<LevelEnum>));
            StatusOfLevel = guestData.LevelStatuses.ToDictionary()
                .ToDictionary(kv => Enum.Parse<LevelEnum>(kv.Key), kv => (LevelStatus)kv.Value);
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
        if (GetLevelStatus(level) == LevelStatus.Completed) return;
        StatusOfLevel[level.EnumName] = LevelStatus.Completed;
        NumExercisesCompleted++;
        if (AreAllLevelsCompletedInWorld(level.World))
        {
            QuincyStatusOfWorld[level.World.EnumName] = LevelStatus.Available;
        }
        if(level.IsMiniMeditation) {
            NumMiniMeditationsCompleted++;
        }
        IncreaseCarrots(10);
    }

    public void CompleteQuincy(World world)
    {
        if (QuincyStatusOfWorld[world.EnumName] == LevelStatus.Completed) return;
        QuincyStatusOfWorld[world.EnumName] = LevelStatus.Completed;
        NumWorldsCompleted++;
        foreach (WorldEnum nextWorld in world.NextWorlds)
        {
            StatusOfLevel[World.WorldLookup[nextWorld].FirstLevel.EnumName] = LevelStatus.Available;
        }
        NumCarrots += 10;
    }

    public void IncreaseSecondsMeditated(float seconds) => SecondsMeditated += seconds;

    public LevelStatus GetLevelStatus(Level level) => StatusOfLevel[level.EnumName];

    public void ChangeLevel(Level level)
    {
        CurrLevel = level.EnumName;
        CurrWorld = level.World.EnumName;
    }

    public void MakeLevelAvailable(Level level)
    {
        if (GetLevelStatus(level) == LevelStatus.Locked)
            StatusOfLevel[level.EnumName] = LevelStatus.Available;
    }

    public bool IsLevelBookmarked(Level level) => BookmarkedLevels.Contains(level.EnumName);

    public void ToggleBookmark(Level level)
    {
        if (!BookmarkedLevels.Remove(level.EnumName))
            BookmarkedLevels.Add(level.EnumName);
    }

    public void Bookmark(Level level, bool bookmark)
    {
        if (bookmark) BookmarkedLevels.Add(level.EnumName);
        else BookmarkedLevels.Remove(level.EnumName);
    }

    public Dictionary<Level, LevelStatus> GetWorldStatus(World world)
    {
        Dictionary<Level, LevelStatus> result = new();
        foreach (Level level in world.Levels)
            result[level] = GetLevelStatus(level);
        return result;
    }

    public void LogMood(Mood moodLevel, DateTime date)
    {
        MoodLog.AddFirst(new MoodEntry(date, moodLevel));
        while (MoodLog.Count > MaxMoodEntries) MoodLog.RemoveLast();
        NumMoodLogEntriesCompleted++;
        List<Badge> badges = UpdateAndGetNewMoodBadges();
        // todo - do something with badges
    }

    public void LogGratitude(string text, DateTime date)
    {
        GratitudeLog.AddLast(new GratitudeEntry(date, text));
        while (GratitudeLog.Count > MaxGratitudeEntries) GratitudeLog.RemoveFirst();
        NumGratitudesCompleted++;
        List<Badge> badges = UpdateAndGetNewGratitudeBadges();
        // todo - do something with badges

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
    public void AddAccessory(Accessory accessory) => AccessoriesOwned.Add(accessory);
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

    public void UseAccessory(Accessory accessory)
    {
        switch (accessory.Type)
        {
            case AccessoryType.Hat: CurrHat = accessory; break;
            case AccessoryType.Neckwear: CurrNeckwear = accessory; break;
            case AccessoryType.Clothing: CurrClothing = accessory; break;
            case AccessoryType.Facewear: CurrFacewear = accessory; break;
            case AccessoryType.Pet: CurrPet = accessory; break;
        }
    }

    public void StopUsingAccessory(AccessoryType type)
    {
        switch (type)
        {
            case AccessoryType.Hat: CurrHat = null; break;
            case AccessoryType.Neckwear: CurrNeckwear = null; break;
            case AccessoryType.Clothing: CurrClothing = null; break;
            case AccessoryType.Facewear: CurrFacewear = null; break;
            case AccessoryType.Pet: CurrPet = null; break;
            default:
                throw new Exception("Unknown accessory type");
        }
    }

    public void Login()
    {
        DateTime today = DateTime.Now.Date;
        if (LastLogin.Date == today) return;

        if (LastLogin.Date == today.AddDays(-1))
            IncreaseCurrStreak();
        else
        {
            CurrStreak = 1;
            BestStreak = Math.Max(BestStreak, CurrStreak);
        }

        LastLogin = today;
    }

    private bool AreAllLevelsCompletedInWorld(World world)
    {
        foreach (Level level in world.Levels)
            if (GetLevelStatus(level) != LevelStatus.Completed) return false;
        return true;
    }

    public void IncreaseCurrStreak()
    {
        CurrStreak++;
        BestStreak = Math.Max(BestStreak, CurrStreak);
    }

    public void LoadMeditationsFromFirestore(Action<LinkedList<MeditationEntry>> callback)
    {
        if (IsGuest) { callback(new LinkedList<MeditationEntry>()); return; }

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            var log = new LinkedList<MeditationEntry>();
            if (!task.Result.Exists) { callback(log); return; }
            var d = task.Result;
            if (!d.ContainsField("MeditationLog")) { callback(log); return; }

            var list = d.GetValue<List<MeditationEntryDTO>>("MeditationLog");
            foreach (var dto in list)
            {
                log.AddLast(new MeditationEntry
                {
                    duration = dto.Duration,
                    interval = dto.Interval,
                    chime = dto.Chime,
                    effect = dto.Effect
                });
            }
            MeditationLog = log;
            callback(log);
        });
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
