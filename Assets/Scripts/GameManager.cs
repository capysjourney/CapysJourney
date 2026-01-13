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
            numExercisesCompleted = stats.NumLevelsCompleted;
            numWorldsCompleted = stats.NumWorldsCompleted;
            stats.UpdateStreakForCompletedActivity();
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
        DataService.SaveData("player-stats.json", stats);
    }

    public static Dictionary<Level, LevelStatus> GetWorldStatus(World world)
    {
        Dictionary<Level, LevelStatus> result = null;
        WithStats(stats => result = stats.GetWorldStatus(world), false);
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
        WithStats(stats => result = stats.NumLevelsCompleted, false);
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

    private static PlayerStats GetStats()
    {
        try
        {
            return DataService.LoadData<PlayerStats>("player-stats.json");
        }
        catch (Exception)
        {
            return null;
        }
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

    public static LinkedList<GratitudeEntry> GetGratitudeEntries()
    {
        LinkedList<GratitudeEntry> result = null;
        WithStats(stats => result = stats.GratitudeLog, false);
        return result ?? new LinkedList<GratitudeEntry>();
    }
    public static int LogGratitudeAndGetCarrotsEarned(string gratitude, DateTime dateTime)
    {
        int carrotsEarned = 0;
        bool alreadyLoggedToday = false;
        WithStats(stats =>
        {
            LinkedList<GratitudeEntry> log = stats.GratitudeLog;
            alreadyLoggedToday = log != null && log.Count > 0 && log.Last().Timestamp.Date == dateTime.Date;
            if (!alreadyLoggedToday)
            {
                IncreaseCarrots(10);
                carrotsEarned = 10;
                stats.UpdateStreakForCompletedActivity();
            }
            stats.LogGratitude(gratitude, dateTime);
        }, true);

        // Track gratitude completion with PostHog
        PostHogManager.Instance.Capture("daily_activity_completed", new Dictionary<string, object>
        {
            { "activity_type", "gratitude" },
            { "gratitude_length", gratitude.Length },
            { "carrots_earned", carrotsEarned },
            { "is_first_today", !alreadyLoggedToday }
        });
        return carrotsEarned;
    }

    public static int LogMoodAndGetCarrotsEarned(Mood mood, DateTime dateTime)
    {
        int carrotsEarned = 0;
        bool alreadyLoggedToday = false;
        WithStats(stats =>
        {
            LinkedList<MoodEntry> log = stats.MoodLog;
            alreadyLoggedToday = log != null && log.Count > 0 && log.Last().Timestamp.Date == dateTime.Date;
            if (!alreadyLoggedToday)
            {
                IncreaseCarrots(10);
                carrotsEarned = 10;
                stats.UpdateStreakForCompletedActivity();
            }
            stats.LogMood(mood, dateTime);
        }, true);

        // Track mood check-in completion with PostHog
        PostHogManager.Instance.Capture("daily_activity_completed", new Dictionary<string, object>
        {
            { "activity_type", "mood_check_in" },
            { "mood", mood.ToString() },
            { "carrots_earned", carrotsEarned },
            { "is_first_today", !alreadyLoggedToday }
        });
        return carrotsEarned;
    }

    public static int CompleteBreathworkAndGetCarrotsEarned(int durationInSeconds)
    {
        int carrotsEarned = 0;
        WithStats(stats =>
        {
            bool alreadyCompletedToday = stats.LastBreathworkTime.Date == DateTime.Now.Date;
            if (!alreadyCompletedToday)
            {
                IncreaseCarrots(10);
                carrotsEarned = 10;
                stats.UpdateStreakForCompletedActivity();
            }
            stats.CompleteBreathworkSession(durationInSeconds);
            // Track breathwork completion with PostHog
            PostHogManager.Instance.Capture("daily_activity_completed", new Dictionary<string, object>
            {
                { "activity_type", "breathwork" },
                { "duration_seconds", durationInSeconds },
                { "carrots_earned", alreadyCompletedToday ? 0 : 10 },
                { "is_first_today", !alreadyCompletedToday }
            });
        }, true);
        return carrotsEarned;
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
            stats.UpdateLastLogin();
            // Check if this is the first login (LastLogin was DateTime.MinValue)
            isFirstLogin = lastLogin == DateTime.MinValue;
        }, false);

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
