using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    /// <summary>
    /// The current world that Capy is standing in.
    /// </summary>
    private static World CurrWorld = World.FirstSteps;

    /// <summary>
    /// The current level that Capy is standing on.
    /// </summary>
    private static Level CurrLevel = Level.FirstSteps_L1;

    /// <summary>
    /// Whether to go to the "Waiting for parent confirmation" screen after registering.
    /// </summary>
    public static bool NeedParentConfirmation = false;
    private static bool HasVisitedJourney = false;

    public static bool GetHasVisitedJourney()
    {
        return HasVisitedJourney;
    }

    public static void VisitJourney()
    {
        HasVisitedJourney = true;
    }

    public static void UpdateWorldAndLevel()
    {
        DataManager.WithStats(stats =>
        {
            CurrWorld = stats.CurrWorld;
            CurrLevel = stats.CurrLevel;
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
        LevelInfo completedLevelInfo = completedLevel.GetInfo();
        PostHogManager.Instance.Capture("level_completed", new Dictionary<string, object>
        {
            { "level_name", completedLevelInfo.ShortName },
            { "level_enum", completedLevelInfo.Level.ToString() },
            { "world_name", completedLevelInfo.World.GetInfo().Name },
            { "lesson_duration_seconds", lessonDuration },
            { "total_exercises_completed", numExercisesCompleted },
            { "total_worlds_completed", numWorldsCompleted }
        });
    }

    // expose world/level info details to UI
    public static WorldInfo GetCurrWorldInfo()
    {
        return CurrWorld.GetInfo();
    }

    public static LevelInfo GetCurrLevelInfo()
    {
        return CurrLevel.GetInfo();
    }

    public static bool HasCompletedLevel(Level level)
    {
        bool result = false;
        DataManager.WithStats(stats => result = stats.GetLevelStatus(level) == LevelStatus.Completed, false);
        return result;
    }

    public static void SetCurrLevel(Level level)
    {
        CurrLevel = level;
        CurrWorld = CurrLevel.GetWorld();
        DataManager.WithStats(stats => stats.ChangeLevel(level), true);
    }

    private static void MakeNextLevelsAvailable(PlayerStats stats)
    {
        Level[] nextLevels = CurrLevel.GetNextLevels();
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

    public static void BookmarkCurrLevel(bool bookmark)
    {
        DataManager.WithStats(stats => stats.Bookmark(CurrLevel, bookmark), true);
    }

    public static Dictionary<Level, LevelStatus> GetWorldStatus(World world)
    {
        Dictionary<Level, LevelStatus> result = null;
        DataManager.WithStats(stats => result = stats.GetWorldStatus(world), false);
        return result;
    }

    public static HashSet<World> GetUnlockedWorlds()
    {
        HashSet<World> result = new();
        DataManager.WithStats(stats =>
        {
            foreach (World world in Enum.GetValues(typeof(World)))
            {
                if (stats.GetWorldStatus(world)[world.GetInfo().FirstLevel] != LevelStatus.Locked)
                {
                    result.Add(world);
                }
            }
        }, false);
        return result;
    }

    public static int GetNumWorldsCompleted()
    {
        int result = 0;
        DataManager.WithStats(stats => result = stats.NumWorldsCompleted, false);
        return result;
    }

    public static bool IsWorldCompleted(World world)
    {
        bool result = false;
        DataManager.WithStats(stats => result = stats.IsWorldCompleted(world), false);
        return result;
    }

    public static HashSet<World> GetCompletedWorlds()
    {
        HashSet<World> result = new();
        DataManager.WithStats(stats =>
        {
            foreach (World world in Enum.GetValues(typeof(World)))
            {
                if (stats.IsWorldCompleted(world))
                {
                    result.Add(world);
                }
            }
        }, false);
        return result;
    }

    public static HashSet<World> GetNewlyAvailableWorlds()
    {
        HashSet<World> result = new();
        DataManager.WithStats(stats =>
        {
            foreach (World world in Enum.GetValues(typeof(World)))
            {
                WorldInfo worldInfo = world.GetInfo();
                if (stats.GetLevelStatus(worldInfo.FirstLevel) == LevelStatus.Available)
                {
                    result.Add(world);
                }
            }
        }, false);
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

    public static bool GetIsFirstLogin()
    {
        bool isFirstLogin = false;
        DataManager.WithStats(stats =>
        {
            isFirstLogin = stats.LastLogin == DateTime.MinValue;
        }, false);
        return isFirstLogin;
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
        }, true);


        // Track login with PostHog
        PostHogManager.Instance.Capture("user_logged_in", new Dictionary<string, object>
        {
            { "is_first_login", isFirstLogin },
            { "current_streak", GetCurrStreak() },
            { "best_streak", GetBestStreak() }
        });
    }

    public static bool GetHasSeenNewWorldNotif(World world)
    {
        bool result = false;
        DataManager.WithStats(stats => result = stats.HasSeenNewWorldNotif[world], false);
        return result;
    }

    public static void OnSeenNewWorldNotif(World world)
    {
        DataManager.WithStats(stats => stats.HasSeenNewWorldNotif[world] = true, true);
    }
}
