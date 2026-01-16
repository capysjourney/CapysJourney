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

    private FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
    private string uid;
    private DocumentReference docRef;

    public Dictionary<LevelEnum, LevelStatus> LevelStatuses = new();
    public HashSet<LevelEnum> BookmarkedLevels = new();
    public LinkedList<MoodEntry> MoodLog = new();
    public LinkedList<GratitudeEntry> GratitudeLog = new();
    public HashSet<Accessory> AccessoriesOwned = new();

    public WorldEnum CurrWorld = World.FirstSteps.EnumName;
    public LevelEnum CurrLevel = Level.World1Level1.EnumName;

    public Accessory CurrHat, CurrNeckwear, CurrClothing, CurrFacewear, CurrPet;

    public int NumCarrots = 10;
    public int BestStreak = 0;
    public int CurrStreak = 0;
    public float SecondsMeditated = 0;
    public int NumExercisesCompleted = 0;
    public int NumWorldsCompleted = 0;
    public DateTime LastBreathworkTime = DateTime.MinValue;
    public DateTime LastLogin = DateTime.MinValue;

    public HashSet<Badge> BadgesEarned = new();
    public LinkedList<MeditationEntry> MeditationLog = new();

    private const int MaxMoodEntries = 30;
    private const int MaxGratitudeEntries = 10;

    public PlayerStats(bool isGuest = false)
    {
        IsGuest = isGuest;

        if (!IsGuest)
        {
            uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            docRef = db.Collection("users").Document(uid);
        }

        foreach (World world in World.AllWorlds)
            foreach (Level level in world.Levels)
                LevelStatuses[level.EnumName] = LevelStatus.Locked;

        LevelStatuses[Level.World1Level1.EnumName] = LevelStatus.Available;

        if (DebugMode)
            foreach (Accessory a in Accessory.AllAccesories)
                if (a.Tier != Tier.Legendary)
                    AccessoriesOwned.Add(a);
    }

    public void SaveToFirestore()
    {
        if (!IsGuest)
        {
            UnityEngine.Debug.Log("Saving player stats to Firestore");

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
                { "LevelStatuses", LevelStatuses.ToDictionary(k => k.Key.ToString(), v => (int)v.Value) },
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
            UnityEngine.Debug.Log("Saving guest player stats to PlayerPrefs");

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
                LevelStatuses = LevelStatuses.ToDictionary(k => k.Key.ToString(), v => (int)v.Value)
            };

            string json = JsonUtility.ToJson(guestData);
            PlayerPrefs.SetString(PlayerPrefsKey, json);
            PlayerPrefs.Save();
        }
    }

    public void LoadFromFirestore()
    {
        if (!IsGuest)
        {
            UnityEngine.Debug.Log("Loading player stats from Firestore");

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
            LevelStatuses = guestData.LevelStatuses.ToDictionary()
                .ToDictionary(kv => Enum.Parse<LevelEnum>(kv.Key), kv => (LevelStatus)kv.Value);
        }
    }

    public void CompleteLevel(Level level)
    {
        if (GetLevelStatus(level) == LevelStatus.Completed) return;

        LevelStatuses[level.EnumName] = LevelStatus.Completed;
        NumExercisesCompleted++;

        if (IsWorldComplete(level.World))
            NumWorldsCompleted++;

        NumCarrots += 10;
    }

    public void IncreaseSecondsMeditated(float seconds) => SecondsMeditated += seconds;

    public LevelStatus GetLevelStatus(Level level) => LevelStatuses[level.EnumName];

    public void ChangeLevel(Level level)
    {
        CurrLevel = level.EnumName;
        CurrWorld = level.World.EnumName;
    }

    public void MakeLevelAvailable(Level level)
    {
        if (GetLevelStatus(level) == LevelStatus.Locked)
            LevelStatuses[level.EnumName] = LevelStatus.Available;
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
    }

    public void LogGratitude(string text, DateTime date)
    {
        GratitudeLog.AddLast(new GratitudeEntry(date, text));
        while (GratitudeLog.Count > MaxGratitudeEntries) GratitudeLog.RemoveFirst();
    }

    public void IncreaseCarrots(int carrots) => NumCarrots += carrots;

    public void AddAccessory(Accessory accessory) => AccessoriesOwned.Add(accessory);

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

    private bool IsWorldComplete(World world)
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
}
