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

    //private static Tier? _lastBasketTier = null;

    /// <summary>
    /// The last accessory obtained from a basket purchase. 
    /// Null if no basket has been purchased in the current session.
    /// </summary>
    //private static Accessory _lastAccessoryObtained = null;

    public static readonly int NumWorlds = 1;

    /// <summary>
    /// Whether to go to the "Waiting for parent confirmation" screen after registering.
    /// </summary>
    public static bool NeedParentConfirmation = false;

    public static bool LaunchAsGuest = false;

    public static void UpdateWorldAndLevel()
    {
        DataManager.WithStats(stats =>
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

        DataManager.WithStats(stats =>
        {
            stats.CompleteLevel(CurrLevel, BadgeManager.HandleBadgesEarned);
            stats.IncreaseSecondsMeditated(lessonDuration);
            MakeNextLevelsAvailable(stats);
            numExercisesCompleted = stats.NumLevelsCompleted;
            numWorldsCompleted = stats.NumWorldsCompleted;
            stats.UpdateStreakForCompletedActivity(BadgeManager.HandleBadgesEarned);
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
        DataManager.WithStats(stats =>
        {
            stats.CompleteQuincy(CurrWorld, BadgeManager.HandleBadgesEarned);
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
        DataManager.WithStats(stats => result = stats.GetLevelStatus(level) == LevelStatus.Completed, false);
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
        DataManager.WithStats(stats => stats.ChangeLevel(level), true);
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
        DataManager.WithStats(stats => result = stats.IsLevelBookmarked(CurrLevel), false);
        return result;
    }

    public static void ToggleBookmark()
    {
        DataManager.WithStats(stats => stats.ToggleBookmark(CurrLevel), true);
    }

    public static void Bookmark(bool bookmark)
    {
        DataManager.WithStats(stats => stats.Bookmark(CurrLevel, bookmark), true);
    }

    public static Dictionary<Level, LevelStatus> GetWorldStatus(World world)
    {
        Dictionary<Level, LevelStatus> result = null;
        DataManager.WithStats(stats => result = stats.GetWorldStatus(world), false);
        return result;
    }

    public static bool IsQuincyUnlocked()
    {
        bool result = false;
        DataManager.WithStats(stats => result = stats.QuincyStatusOfWorld[CurrWorld.EnumName] == LevelStatus.Available, false);
        return result;
    }

    public static int GetNumWorldsCompleted()
    {
        int result = 0;
        DataManager.WithStats(stats => result = stats.NumWorldsCompleted, false);
        return result;
    }

    public static int GetNumLessonsCompleted()
    {
        int result = 0;
        DataManager.WithStats(stats => result = stats.NumLevelsCompleted, false);
        return result;
    }

    public static int GetCurrStreak()
    {
        int result = 0;
        DataManager.WithStats(stats => result = stats.CurrStreak, false);
        return result;
    }

    public static int GetBestStreak()
    {
        int result = 0;
        DataManager.WithStats(stats => result = stats.BestStreak, false);
        return result;
    }

    public static float GetTotalMinutesMeditated()
    {
        float result = 0;
        DataManager.WithStats(stats => result = MathF.Floor(stats.SecondsMeditated / 60f), false);
        return result;
    }

    public static AgeGroup GetAgeGroup()
    {
        // todo - migrate to playerstats
        int age = PlayerPrefs.GetInt("age", 0);
        return AgeGroupMethods.FromAge(age);
    }


    public static string GetAudioName(AgeGroup ageGroup)
    {
        // todo - age specific audio
        return CurrLevel.GetAudioFilePathOfAgeGroup(ageGroup);
    }

    public static LinkedList<MoodEntry> GetMoodEntries()
    {
        LinkedList<MoodEntry> result = null;
        DataManager.WithStats(stats => result = stats.MoodLog, false);
        return result ?? new LinkedList<MoodEntry>();
    }

    public static LinkedList<GratitudeEntry> GetGratitudeEntries()
    {
        LinkedList<GratitudeEntry> result = null;
        DataManager.WithStats(stats => result = stats.GratitudeLog, false);
        return result ?? new LinkedList<GratitudeEntry>();
    }
    public static int LogGratitudeAndGetCarrotsEarned(string gratitude, DateTime dateTime)
    {
        int carrotsEarned = 0;
        bool alreadyLoggedToday = false;
        DataManager.WithStats(stats =>
        {
            LinkedList<GratitudeEntry> log = stats.GratitudeLog;
            alreadyLoggedToday = log != null && log.Count > 0 && log.Last().Timestamp.Date == dateTime.Date;
            if (!alreadyLoggedToday)
            {
                CarrotManager.IncreaseCarrots(10);
                carrotsEarned = 10;
                stats.UpdateStreakForCompletedActivity(BadgeManager.HandleBadgesEarned);
            }
            stats.LogGratitude(gratitude, dateTime, BadgeManager.HandleBadgesEarned);
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
        DataManager.WithStats(stats =>
        {
            LinkedList<MoodEntry> log = stats.MoodLog;
            alreadyLoggedToday = log != null && log.Count > 0 && log.Last().Timestamp.Date == dateTime.Date;
            if (!alreadyLoggedToday)
            {
                CarrotManager.IncreaseCarrots(10);
                carrotsEarned = 10;
                stats.UpdateStreakForCompletedActivity(BadgeManager.HandleBadgesEarned);
            }
            stats.LogMood(mood, dateTime, BadgeManager.HandleBadgesEarned);
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
        DataManager.WithStats(stats =>
        {
            bool alreadyCompletedToday = stats.LastBreathworkTime.Date == DateTime.Now.Date;
            if (!alreadyCompletedToday)
            {
                CarrotManager.IncreaseCarrots(10);
                carrotsEarned = 10;
                stats.UpdateStreakForCompletedActivity(BadgeManager.HandleBadgesEarned);
            }
            stats.CompleteBreathworkSession(durationInSeconds, BadgeManager.HandleBadgesEarned);
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


    public static void Login()
    {
        bool isFirstLogin = false;
        DataManager.WithStats(stats =>
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
}
