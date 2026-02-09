using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
public static class DataManager
{
    public static JsonDataService DataService = new();

    public static void SetStats(PlayerStats stats)
    {
        DataService.SaveData("player-stats.json", stats);
    }

    public static async Task<PlayerStats> GetStats()
    {
        Debug.Log("GetStats() called");
        bool isGuest = PlayerPrefs.GetInt("isGuest", 1) == 1;
        Debug.Log($"isGuest: {isGuest}");

        if (!isGuest)
        {
            var stats = await PlayerStats.LoadFromFirestore();

            if (stats == null)
            {
                // First time authenticated user - create new stats
                stats = new PlayerStats(false);
                stats.SaveToFirestore(); // Save immediately so document exists
            }

            return stats;
        }

        Debug.Log("Loading from JSON...");
        var jsonStats = DataService.LoadData<PlayerStats>("player-stats.json");
        Debug.Log($"JSON stats: {(jsonStats == null ? "NULL" : "OK")}");

        if (jsonStats == null)
        {
            // First time guest user - create new stats
            Debug.Log("Creating new PlayerStats for guest user");
            jsonStats = new PlayerStats(true);
        }

        return jsonStats;
    }   
    /// <summary>
            /// Fetches the player stats, performs <c>action</c> on it, and saves the updated stats if <c>update</c> is true.
            /// </summary>
            /// <exception cref="Exception">Thrown when stats cannot be fetched.</exception>
    public static async Task WithStats(Action<PlayerStats> action, bool update)
    {
        PlayerStats stats = await GetStats() ?? throw new Exception("Could not fetch stats");
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