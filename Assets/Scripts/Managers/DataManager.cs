using System;
using UnityEngine;

public static class DataManager
{
    public static JsonDataService DataService = new();

    public static void SetStats(PlayerStats stats)
    {
        DataService.SaveData("player-stats.json", stats);
    }

    public static PlayerStats GetStats()
    {
        bool isGuest = PlayerPrefs.GetInt("isGuest", 1) == 1;
        if (!isGuest)
        {
            return PlayerStats.LoadFromFirestore();
        }
        return DataService.LoadData<PlayerStats>("player-stats.json");
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

    private static void SaveData(PlayerStats stats)
    {
        bool isGuest = PlayerPrefs.GetInt("isGuest", 1) == 1;
        if (!isGuest)
        {
            stats.SaveToFirestore();
        }
        DataService.SaveData("player-stats.json", stats);
    }
}