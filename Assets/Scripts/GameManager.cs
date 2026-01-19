using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameManager
{
    /// <summary>
    /// The current world that Capy is standing in.
    /// </summary>
    private static World CurrWorld = null;

    /// <summary>
    /// The current level that Capy is standing on.
    /// </summary>
    private static Level CurrLevel = null;

    private static readonly IDataService DataService = new JsonDataService();
    private static Tier? _lastBasketTier = null;

    /// <summary>
    /// The last accessory obtained from a basket purchase. 
    /// Null if no basket has been purchased in the current session.
    /// </summary>
    private static Accessory _lastAccessoryObtained = null;

    public static readonly int NumWorlds = 1;

    /// <summary>
    /// Whether to go to the "Waiting for parent confirmation" screen after registering.
    /// </summary>
    public static bool NeedParentConfirmation = false;

    private static PlayerStats statsCache = null;

    public static void UpdateWorldAndLevel()
    {
        WithStats(stats =>
        {
            CurrWorld = World.WorldLookup[stats.CurrWorld];
            CurrLevel = Level.LevelLookup[stats.CurrLevel];
        }, false);
    }

    public static void CompleteLevel(float lessonDuration)
    {
        Level completedLevel = CurrLevel;
        int numExercisesCompleted = 0;
        int numWorldsCompleted = 0;

        WithStats(stats =>
        {
            stats.CompleteLevel(CurrLevel);
            stats.IncreaseSecondsMeditated(lessonDuration);
            MakeNextLevelsAvailable(stats);
            numExercisesCompleted = stats.NumExercisesCompleted;
            numWorldsCompleted = stats.NumWorldsCompleted;
        }, true);

        // Track level completion with PostHog
        PostHogManager.Instance.Capture("level_completed", new Dictionary<string, object>
        {
            { "level_name", completedLevel.ShortName },
            { "level_enum", completedLevel.EnumName.ToString() },
            { "world_name", completedLevel.World.Name },
            { "lesson_duration_seconds", lessonDuration },
            { "total_exercises_completed", numExercisesCompleted },
            { "total_worlds_completed", numWorldsCompleted }
        });
    }

    public static void CompleteQuincy()
    {
        World completedWorld = CurrWorld;
        WithStats(stats =>
        {
            stats.CompleteQuincy(CurrWorld);
        }, true);
    }

    public static World GetCurrWorld()
    {
        if (CurrWorld == null)
        {
            UpdateWorldAndLevel();
        }
        return CurrWorld;
    }

    public static Level GetCurrLevel()
    {
        if (CurrLevel == null)
        {
            UpdateWorldAndLevel();
        }
        return CurrLevel;
    }

    public static bool HasCompletedLevel(Level level)
    {
        bool result = false;
        WithStats(stats => result = stats.GetLevelStatus(level) == LevelStatus.Completed, false);
        return result;
    }

    public static void SetCurrWorld(World world)
    {
        CurrWorld = world;
    }

    public static void SetCurrLevel(Level level)
    {
        CurrLevel = level;
        CurrWorld = level.World;
        WithStats(stats => stats.ChangeLevel(level), true);
    }

    private static void MakeNextLevelsAvailable(PlayerStats stats)
    {
        Level[] nextLevels = Level.NextLevelMap[CurrLevel];
        foreach (Level level in nextLevels)
        {
            stats.MakeLevelAvailable(level);
        }
    }

    public static bool IsLevelBookmarked()
    {
        bool result = false;
        WithStats(stats => result = stats.IsLevelBookmarked(CurrLevel), false);
        return result;
    }

    public static void ToggleBookmark()
    {
        WithStats(stats => stats.ToggleBookmark(CurrLevel), true);
    }

    public static void Bookmark(bool bookmark)
    {
        WithStats(stats => stats.Bookmark(CurrLevel, bookmark), true);
    }

    private static void SaveData(PlayerStats stats)
    {
        stats.SaveToFirestore();
    }


    public static Dictionary<Level, LevelStatus> GetWorldStatus(World world)
    {
        Dictionary<Level, LevelStatus> result = null;
        WithStats(stats => result = stats.GetWorldStatus(world), false);
        return result;
    }

    public static bool IsQuincyUnlocked()
    {
        bool result = false;
        WithStats(stats => result = stats.QuincyStatusOfWorld[CurrWorld.EnumName] == LevelStatus.Available, false);
        return result;
    }

    public static int GetNumWorldsCompleted()
    {
        int result = 0;
        WithStats(stats => result = stats.NumWorldsCompleted, false);
        return result;
    }

    public static int GetNumLessonsCompleted()
    {
        int result = 0;
        WithStats(stats => result = stats.NumExercisesCompleted, false);
        return result;
    }

    public static int GetCurrStreak()
    {
        int result = 0;
        WithStats(stats => result = stats.CurrStreak, false);
        return result;
    }

    public static int GetBestStreak()
    {
        int result = 0;
        WithStats(stats => result = stats.BestStreak, false);
        return result;
    }

    public static float GetTotalMinutesMeditated()
    {
        float result = 0;
        WithStats(stats => result = MathF.Floor(stats.SecondsMeditated / 60f), false);
        return result;
    }

    public static AgeGroup GetAgeGroup()
    {
        // todo - migrate to playerstats
        int age = PlayerPrefs.GetInt("age", 0);
        return AgeGroupMethods.FromAge(age);
    }

    public static PlayerStats GetStats()
    {
        if (statsCache != null) return statsCache;

        statsCache = new PlayerStats();
        statsCache.LoadFromFirestore();

        return statsCache;
    }

    public static string GetAudioName(AgeGroup ageGroup)
    {
        // todo - age specific audio
        return CurrLevel.AudioFile[ageGroup];
    }

    public static LinkedList<MoodEntry> GetMoodEntries()
    {
        LinkedList<MoodEntry> result = null;
        WithStats(stats => result = stats.MoodLog, false);
        return result ?? new LinkedList<MoodEntry>();
    }

    public static void LogMood(Mood mood, DateTime dateTime)
    {
        WithStats(stats => stats.LogMood(mood, dateTime), true);
    }

    public static LinkedList<GratitudeEntry> GetGratitudeEntries()
    {
        LinkedList<GratitudeEntry> result = null;
        WithStats(stats => result = stats.GratitudeLog, false);
        return result ?? new LinkedList<GratitudeEntry>();
    }

    public static void LogGratitude(string gratitude, DateTime dateTime)
    {
        WithStats(stats => stats.LogGratitude(gratitude, dateTime), true);
    }

    public static bool LoggedMoodToday()
    {
        bool result = false;
        WithStats(stats =>
        {
            LinkedList<MoodEntry> log = stats.MoodLog;
            result = log != null && log.Count > 0 && log.First().Timestamp.Date == DateTime.Now.Date;
        }, false);
        return result;
    }

    public static bool LoggedGratitudeToday()
    {
        bool result = false;
        WithStats(stats =>
        {
            LinkedList<GratitudeEntry> log = stats.GratitudeLog;
            result = log != null && log.Count > 0 && log.Last().Timestamp.Date == DateTime.Now.Date;
        }, false);
        return result;
    }

    public static bool DidBreathworkToday()
    {
        bool result = false;
        WithStats(stats =>
        {
            result = stats.LastBreathworkTime.Date == DateTime.Now.Date;
        }, false);
        return result;
    }

    public static void DoBreathwork()
    {
        WithStats(stats => stats.LastBreathworkTime = DateTime.Now, true);
    }

    public static int GetNumCarrots()
    {
        int result = 0;
        WithStats(stats => result = stats.NumCarrots, false);
        return result;
    }

    public static void IncreaseCarrots(int numCarrots)
    {
        if (numCarrots <= 0) return;
        WithStats(stats => stats.IncreaseCarrots(numCarrots), true);
    }

    public static void Login()
    {
        bool isFirstLogin = false;
        WithStats(stats =>
        {
            DateTime lastLogin = stats.LastLogin;
            stats.Login();
            // Check if this is the first login (LastLogin was DateTime.MinValue)
            isFirstLogin = lastLogin == DateTime.MinValue;
        }, true);

        // Track login with PostHog
        PostHogManager.Instance.Capture("user_logged_in", new Dictionary<string, object>
        {
            { "is_first_login", isFirstLogin },
            { "current_streak", GetCurrStreak() },
            { "best_streak", GetBestStreak() }
        });
    }

    /// <summary>
    /// Attempts to buy a basket of the given tier. If successful, deducts the cost from the player's carrots. 
    /// Returns true if the purchase was successful. Returns false otherwise (e.g. not enough carrots or no accessories left to obtain).
    /// </summary>
    public static bool BuyBasket(Tier basketTier)
    {
        Dictionary<Tier, int> tierCosts = new()
        {
            { Tier.Common, 50 },
            { Tier.Rare, 100 },
            { Tier.Epic, 250 },
            { Tier.Legendary, 500 }
        };
        bool bought = false;
        HashSet<Accessory> lockedAccessories = new();

        WithStats(stats =>
        {
            if (stats.NumCarrots < tierCosts[basketTier])
            {
                Debug.Log("Not enough carrots to buy basket");
                return;
            }
            foreach (Accessory acc in Accessory.AllAccesories)
            {
                if (!stats.AccessoriesOwned.Contains(acc))
                {
                    lockedAccessories.Add(acc);
                }
            }
            if (lockedAccessories.Count == 0)
            {
                Debug.Log("No locked accessories left to obtain");
                return;
            }
            bool CommonAvailable = lockedAccessories.Any(a => a.Tier == Tier.Common);
            bool RareAvailable = lockedAccessories.Any(a => a.Tier == Tier.Rare);
            bool EpicAvailable = lockedAccessories.Any(a => a.Tier == Tier.Epic);
            bool LegendaryAvailable = lockedAccessories.Any(a => a.Tier == Tier.Legendary);
            System.Random random = new();
            double randomNum = random.NextDouble();
            Tier accessoryTier;
            double commonWeight;
            double rareWeight;
            double epicWeight;
            double legendaryWeight;
            switch (basketTier)
            {
                case Tier.Common:
                    commonWeight = CommonAvailable ? 0.9 : 0;
                    rareWeight = RareAvailable ? 0.095 : 0;
                    epicWeight = EpicAvailable ? 0.005 : 0;
                    //legendaryWeight = 0.00001;
                    legendaryWeight = 0;
                    break;
                case Tier.Rare:
                    commonWeight = CommonAvailable ? 0.00001 : 0;
                    rareWeight = RareAvailable ? 0.8 : 0;
                    epicWeight = EpicAvailable ? 0.2 : 0;
                    //legendaryWeight = LegendaryAvailable ? 0.0000001 : 0;
                    legendaryWeight = 0;
                    break;
                case Tier.Epic:
                    commonWeight = CommonAvailable ? 0.00001 : 0;
                    rareWeight = RareAvailable ? 0.4 : 0;
                    epicWeight = EpicAvailable ? 0.5 : 0;
                    //legendaryWeight = LegendaryAvailable ? 0.1 : 0;
                    legendaryWeight = 0;
                    break;
                case Tier.Legendary:
                    commonWeight = CommonAvailable ? 0.00001 : 0;
                    rareWeight = RareAvailable ? 0.000001 : 0;
                    epicWeight = EpicAvailable ? 0.3 : 0;
                    //legendaryWeight = LegendaryAvailable ? 0.7 : 0;
                    legendaryWeight = 0;
                    break;
                default:
                    throw new Exception("Invalid tier");
            }
            double totalWeight = commonWeight + rareWeight + epicWeight + legendaryWeight;
            if (totalWeight == 0)
            {
                // todo - behavior?
                Debug.Log("No accessories of any tier available to obtain");
                return;
            }
            commonWeight /= totalWeight;
            rareWeight /= totalWeight;
            epicWeight /= totalWeight;
            legendaryWeight /= totalWeight;
            if (randomNum <= commonWeight)
            {
                accessoryTier = Tier.Common;
            }
            else if (randomNum <= commonWeight + rareWeight)
            {
                accessoryTier = Tier.Rare;
            }
            else if (randomNum <= commonWeight + rareWeight + epicWeight)
            {
                accessoryTier = Tier.Epic;
            }
            else
            {
                accessoryTier = Tier.Legendary;
            }
            stats.IncreaseCarrots(-tierCosts[basketTier]);
            List<Accessory> accessoriesOfTier = lockedAccessories.Where(a => a.Tier == accessoryTier).ToList();
            Accessory accessory = accessoriesOfTier[random.Next(accessoriesOfTier.Count)];
            _lastBasketTier = basketTier;
            _lastAccessoryObtained = accessory;
            stats.AddAccessory(accessory);
            bought = true;
        }, true);

        // Track basket purchase with PostHog
        if (bought)
        {
            PostHogManager.Instance.Capture("basket_purchased", new Dictionary<string, object>
            {
                { "basket_tier", basketTier.ToString() },
                { "basket_cost", tierCosts[basketTier] },
                { "accessory_tier", _lastAccessoryObtained.Tier.ToString() },
                { "accessory_name", _lastAccessoryObtained.Name },
                { "accessory_type", _lastAccessoryObtained.Type.ToString() },
                { "remaining_carrots", GetNumCarrots() }
            });
        }

        return bought;
    }

    /// <summary>
    /// Fetches the player stats, performs <c>action</c> on it, and saves the updated stats if <c>update</c> is true.
    /// </summary>
    /// <exception cref="Exception">Thrown when stats cannot be fetched.</exception>
    public static void WithStats(Action<PlayerStats> action, bool update)
    {
        PlayerStats stats = GetStats() ?? throw new Exception("Could not fetch stats");
        action(stats);
        if (update)
        {
            SaveData(stats);
        }
    }

    public static List<Accessory> GetOwnedAccessoriesOfType(AccessoryType type)
    {
        List<Accessory> result = new();
        WithStats(stats =>
        {
            result = stats.AccessoriesOwned.Where(a => a.Type == type).ToList();
        }, false);
        return result;
    }

    public static int NumTotalAccessoriesOfType(AccessoryType type)
    {
        return Accessory.AllAccesories.Count(a => a.Type == type);
    }

    public static void UseAccessory(Accessory accessory)
    {
        WithStats(stats => stats.UseAccessory(accessory), true);
    }

    public static void StopUsingAccessory(AccessoryType type)
    {
        WithStats(stats => stats.StopUsingAccessory(type), true);
    }

    /// <summary>
    /// Gets the currently used accessory of the given type. 
    /// Returns null if no accessory of that type is currently being used.
    /// </summary>
    public static Accessory GetCurrentAccessoryOfType(AccessoryType type)
    {
        Accessory result = null;
        WithStats(stats =>
        {
            result = type switch
            {
                AccessoryType.Hat => stats.CurrHat,
                AccessoryType.Neckwear => stats.CurrNeckwear,
                AccessoryType.Clothing => stats.CurrClothing,
                AccessoryType.Facewear => stats.CurrFacewear,
                AccessoryType.Pet => stats.CurrPet,
                _ => null
            };
        }, false);
        return result;
    }

    public static (Tier?, Accessory) GetLastBasketInfo() => (_lastBasketTier, _lastAccessoryObtained);
}
