using System;

public static class DataManager
{
    private static PlayerStats statsCache = null;

    public static void SetStats(PlayerStats stats)
    {
        statsCache = stats;
    }

    public static PlayerStats GetStats()
    {
        if (statsCache != null) return statsCache;

        statsCache = new PlayerStats(GameManager.LaunchAsGuest);
        statsCache.LoadFromFirestore();

        return statsCache;
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
        stats.SaveToFirestore();
    }

}