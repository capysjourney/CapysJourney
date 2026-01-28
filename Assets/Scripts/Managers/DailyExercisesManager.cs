using System;
using System.Collections.Generic;
using System.Linq;

public static class  DailyExercisesManager
{
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
}