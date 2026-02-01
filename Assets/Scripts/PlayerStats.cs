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
    public int NumLevelsCompleted;
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
    private readonly string _guestId = Guid.NewGuid().ToString();
    private string PlayerPrefsKey => $"GuestPlayerStats_{_guestId}";
    private const bool IsDebugMode = true;

    public bool IsGuest = false;

    private readonly FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
    private readonly string uid;
    private readonly DocumentReference docRef;

    public Dictionary<Level, LevelStatus> StatusOfLevel = new();
    public Dictionary<World, LevelStatus> QuincyStatusOfWorld = new();
    public HashSet<Level> BookmarkedLevels = new();

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

    public World CurrWorld = World.FirstSteps;
    public Level CurrLevel = Level.FirstSteps_L1;

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

    public bool HasUsedAccessory = false;
    public DateTime LastBreathworkTime = DateTime.MinValue;
    public DateTime LastLogin = DateTime.MinValue;
    public DateTime TimeOfLastActivity = DateTime.MinValue;
    public bool HasCompletedQuincy = false;
    public HashSet<Badge> BadgesEarned = new();
    public LinkedList<MeditationEntry> MeditationLog = new();
    public BadgesDisplayed BadgesCurrentlyDisplayed = new();
    public Dictionary<World, bool> HasSeenNewWorldNotif = new();

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

        foreach (World world in Enum.GetValues(typeof(World)))
        {
            foreach (Level level in world.GetLevels())
            {
                StatusOfLevel[level] = LevelStatus.Locked;
            }
            QuincyStatusOfWorld[world] = LevelStatus.Locked;
        }

        StatusOfLevel[Level.FirstSteps_L1] = LevelStatus.Available;

        if (IsDebugMode)
        {
            foreach (Accessory a in Accessory.AllAccesories)
                if (a.Tier != Tier.Legendary && a.Tier != Tier.Epic)
                    AccessoriesOwned.Add(a);
            NumCarrots = 1000;
        }
        foreach (World worldEnum in Enum.GetValues(typeof(World)))
        {
            HasSeenNewWorldNotif[worldEnum] = false;
        }
    }

    public void SaveToFirestore()
    {
        if (!IsGuest)
        {
            if (IsDebugMode)
            {
                Debug.Log("Saving player stats to Firestore");
            }

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
                { "NumLevelsCompleted", NumLevelsCompleted },
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
            if (IsDebugMode)
            {
                Debug.Log("Saving guest player stats to PlayerPrefs");
            }

            var guestData = new
            {
                NumCarrots,
                BestStreak,
                CurrStreak,
                SecondsMeditated,
                NumLevelsCompleted,
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
        if (!IsGuest && !IsDebugMode)
        {
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.Result.Exists) return;
                var d = task.Result;

                NumCarrots = d.GetValue<int>("NumCarrots");
                BestStreak = d.GetValue<int>("BestStreak");
                CurrStreak = d.GetValue<int>("CurrStreak");
                SecondsMeditated = d.GetValue<float>("SecondsMeditated");
                NumLevelsCompleted = d.GetValue<int>("NumLevelsCompleted");
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
            if (IsDebugMode)
            {
                Debug.Log("Loading guest player stats from PlayerPrefs");
            }
            if (!PlayerPrefs.HasKey(PlayerPrefsKey)) return;

            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            var guestData = JsonUtility.FromJson<GuestPlayerData>(json);

            NumCarrots = guestData.NumCarrots;
            BestStreak = guestData.BestStreak;
            CurrStreak = guestData.CurrStreak;
            SecondsMeditated = guestData.SecondsMeditated;
            NumLevelsCompleted = guestData.NumLevelsCompleted;
            NumWorldsCompleted = guestData.NumWorldsCompleted;
            LastBreathworkTime = new DateTime(guestData.LastBreathworkTime);
            LastLogin = new DateTime(guestData.LastLogin);
            CurrWorld = Enum.Parse<World>(guestData.CurrWorld);
            CurrLevel = Enum.Parse<Level>(guestData.CurrLevel);

            BadgesEarned = new HashSet<Badge>(guestData.BadgesEarned.Select(Enum.Parse<Badge>));
            BookmarkedLevels = new HashSet<Level>(guestData.BookmarkedLevels.Select(Enum.Parse<Level>));
            StatusOfLevel = guestData.LevelStatuses.ToDictionary()
                .ToDictionary(kv => Enum.Parse<Level>(kv.Key), kv => (LevelStatus)kv.Value);
        }
    }


    public void UpdateLastLogin()
    {
        LastLogin = DateTime.Now.Date;
    }

    /// <summary>
    /// Updates the current streak based on whether the player is active on consecutive days.
    /// Increments streak if active yesterday, resets to 1 if not, and updates best streak.
    /// </summary>
    private void UpdateStreakBasedOnDate(DateTime compareDate)
    {
        if (compareDate == DateTime.Now.Date.AddDays(-1))
        {
            IncreaseCurrStreak();
        }
        else if (compareDate != DateTime.Now.Date)
        {
            CurrStreak = 1;
            BestStreak = Math.Max(BestStreak, CurrStreak);
        }
    }

    public void UpdateStreakForCompletedActivity(Action<List<Badge>> handleNewBadges)
    {
        UpdateStreakBasedOnDate(TimeOfLastActivity.Date);
        TimeOfLastActivity = DateTime.Now;
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewStreakBadges);
    }

    public void MakeLevelAvailable(Level level)
    {
        if (GetLevelStatus(level) == LevelStatus.Locked)
        {
            StatusOfLevel[level] = LevelStatus.Available;
        }
    }

    public void CompleteLevel(Level level, Action<List<Badge>> handleNewBadges)
    {
        if (GetLevelStatus(level) == LevelStatus.Completed) return;
        StatusOfLevel[level] = LevelStatus.Completed;
        NumLevelsCompleted++;
        LevelInfo levelDetails = level.GetInfo();
        World world = levelDetails.World;

        if (AreAllLevelsCompletedInWorld(world))
        {
            QuincyStatusOfWorld[world] = LevelStatus.Available;
        }
        if (levelDetails.IsMiniMeditation)
        {
            NumMiniMeditationsCompleted++;
        }
        IncreaseCarrots(10, handleNewBadges);
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewLevelBadges);
    }

    public void CompleteQuincy(World world, Action<List<Badge>> handleNewBadges)
    {
        if (QuincyStatusOfWorld[world] == LevelStatus.Completed) return;
        QuincyStatusOfWorld[world] = LevelStatus.Completed;
        NumWorldsCompleted++;
        foreach (World nextWorld in world.GetInfo().NextWorlds)
        {
            StatusOfLevel[nextWorld.GetInfo().FirstLevel] = LevelStatus.Available;
        }
        NumCarrots += 10;
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewQuincyBadges);
    }

    // World is completed iff Quincy is completed for that world
    public bool IsWorldCompleted(World world)
    {
        return QuincyStatusOfWorld[world] == LevelStatus.Completed;
    }

    public void IncreaseSecondsMeditated(float seconds) => SecondsMeditated += seconds;

    public LevelStatus GetLevelStatus(Level level) => StatusOfLevel[level];

    public void ChangeLevel(Level level)
    {
        CurrLevel = level;
        CurrWorld = level.GetWorld();
    }
    public bool IsLevelBookmarked(Level level) => BookmarkedLevels.Contains(level);

    public void ToggleBookmark(Level level)
    {
        if (!BookmarkedLevels.Remove(level))
            BookmarkedLevels.Add(level);
    }

    public void Bookmark(Level level, bool bookmark)
    {
        if (bookmark) BookmarkedLevels.Add(level);
        else BookmarkedLevels.Remove(level);
    }

    public Dictionary<Level, LevelStatus> GetWorldStatus(World world)
    {
        Dictionary<Level, LevelStatus> result = new();
        foreach (Level level in world.GetLevels())
            result[level] = GetLevelStatus(level);
        return result;
    }

    public void LogMood(Mood moodLevel, DateTime date, Action<List<Badge>> handleNewBadges)
    {
        MoodLog.AddFirst(new MoodEntry(date, moodLevel));
        while (MoodLog.Count > MaxMoodEntries) MoodLog.RemoveLast();
        NumMoodLogEntriesCompleted++;
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewMoodBadges);
    }

    public void LogGratitude(string text, DateTime date, Action<List<Badge>> handleNewBadges)
    {
        GratitudeLog.AddLast(new GratitudeEntry(date, text));
        while (GratitudeLog.Count > MaxGratitudeEntries) GratitudeLog.RemoveFirst();
        NumGratitudesCompleted++;
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewGratitudeBadges);
    }

    public void IncreaseCarrots(int carrots, Action<List<Badge>> handleNewBadges)
    {
        NumCarrots += carrots;
        if (carrots < 0)
        {
            NumCarrotsSpent -= carrots;
        }
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewCarrotsBadges);
    }

    public void AddAccessory(Accessory accessory, Action<List<Badge>> handleNewBadges)
    {
        AccessoriesOwned.Add(accessory);
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewAccessoryBadges);
    }

    public void UseAccessory(Accessory accessory, Action<List<Badge>> handleNewBadges)
    {
        HasUsedAccessory = true;
        SetCurrentAccessory(accessory.Type, accessory);
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewAccessoryUseBadge);
    }

    public void CompleteBreathworkSession(int durationInSeconds, Action<List<Badge>> handleNewBadges)
    {
        NumBreathworkSessionsCompleted++;
        LastBreathworkTime = DateTime.Now;
        IncreaseSecondsMeditated(durationInSeconds);
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewBreathworkBadges);
    }

    /// <summary>
    /// Helper method to conditionally add a badge to the list if it hasn't been earned yet.
    /// </summary>
    private void TryAddBadge(List<Badge> newBadges, Badge badge, bool condition)
    {
        if (!BadgesEarned.Contains(badge) && condition)
        {
            newBadges.Add(badge);
        }
    }

    /// <summary>
    /// Helper method to check for new badges, earn them, and invoke the handler callback.
    /// </summary>
    private void CheckAndEarnBadges(Action<List<Badge>> handleNewBadges, Func<List<Badge>> getBadgesFunc)
    {
        List<Badge> newBadges = getBadgesFunc();
        foreach (Badge badge in newBadges)
        {
            BadgesEarned.Add(badge);
        }
        handleNewBadges(newBadges);
    }

    /// <summary>
    /// Sets the current accessory of the given type.
    /// </summary>
    private void SetCurrentAccessory(AccessoryType type, Accessory accessory)
    {
        switch (type)
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

    public void Login()
    {
        DateTime today = DateTime.Now.Date;
        if (LastLogin.Date == today) return;

        UpdateStreakBasedOnDate(LastLogin.Date);
        LastLogin = today;
    }

    private bool AreAllLevelsCompletedInWorld(World world)
    {
        foreach (Level level in world.GetLevels())
            if (GetLevelStatus(level) != LevelStatus.Completed) return false;
        return true;
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

    public void CompleteQuincy(Action<List<Badge>> handleNewBadges)
    {
        HasCompletedQuincy = true;
        CheckAndEarnBadges(handleNewBadges, UpdateAndGetNewQuincyBadges);
    }

    private void IncreaseCurrStreak()
    {
        CurrStreak++;
        BestStreak = Math.Max(BestStreak, CurrStreak);
    }

    public List<Badge> UpdateAndGetNewStreakBadges()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.GettingCozy, CurrStreak >= 3);
        TryAddBadge(newBadges, Badge.SettlingIn, CurrStreak >= 7);
        TryAddBadge(newBadges, Badge.ZenMaster, CurrStreak >= 50);
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewBreathworkBadges()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.JustBreathe, NumBreathworkSessionsCompleted >= 5);
        TryAddBadge(newBadges, Badge.DeepBreatheDevotee, NumBreathworkSessionsCompleted >= 25);
        TryAddBadge(newBadges, Badge.BreathSage, NumBreathworkSessionsCompleted >= 50);
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewGratitudeBadges()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.SeedsOfGratitude, NumGratitudesCompleted >= 5);
        TryAddBadge(newBadges, Badge.BloomingThanks, NumGratitudesCompleted >= 25);
        TryAddBadge(newBadges, Badge.GratitudeGardener, NumGratitudesCompleted >= 50);
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewMoodBadges()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.FeelingFeeler, NumMoodLogEntriesCompleted >= 5);
        TryAddBadge(newBadges, Badge.MoodMapper, NumMoodLogEntriesCompleted >= 25);
        TryAddBadge(newBadges, Badge.InnerWeatherWatcher, NumMoodLogEntriesCompleted >= 50);
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewCarrotsBadges()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.TinyTrader, NumCarrotsSpent >= 100);
        TryAddBadge(newBadges, Badge.CapyCollector, NumCarrotsSpent >= 500);
        TryAddBadge(newBadges, Badge.CarrotTycoon, NumCarrotsSpent >= 1000);
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewQuincyBadges()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.MeetingQuincy, HasCompletedQuincy);
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewAccessoryBadges()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.CapysClosetBegins, AccessoriesOwned.Count >= 1);
        TryAddBadge(newBadges, Badge.DressedToImpress, AccessoriesOwned.Count >= 10);
        return newBadges;
    }

    // todo - use
    public List<Badge> UpdateAndGetNewFurnitureBadges()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.OneStepToCozy, FurnituresOwned.Count >= 1);
        TryAddBadge(newBadges, Badge.CapyLovesHome, FurnituresOwned.Count >= 10);
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewLevelBadges()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.AdventureAwaits, GetLevelStatus(Level.FirstSteps_L1) == LevelStatus.Completed);
        TryAddBadge(newBadges, Badge.TrailBlazer, NumLevelsCompleted > WorldInfo.FirstSteps.Levels.Count);
        TryAddBadge(newBadges, Badge.TinyButMighty, NumMiniMeditationsCompleted >= 3);
        TryAddBadge(newBadges, Badge.MindfulBeginnings,
            IsWorldCompleted(World.FirstSteps));
        return newBadges;
    }

    public List<Badge> UpdateAndGetNewAccessoryUseBadge()
    {
        List<Badge> newBadges = new();
        TryAddBadge(newBadges, Badge.FreshFit, HasUsedAccessory);
        return newBadges;
    }
}
