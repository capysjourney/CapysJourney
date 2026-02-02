public static class QuincyManager
{
    public static void CompleteQuincy(WorldInfo world)
    {
        WorldInfo completedWorld = world;
        DataManager.WithStats(stats =>
        {
            stats.CompleteQuincy(world.World, BadgeManager.HandleBadgesEarned);
        }, true);
    }

    public static bool IsQuincyUnlocked(WorldInfo world)
    {
        bool result = false;
        DataManager.WithStats(stats => result = stats.QuincyStatusOfWorld[world.World] == LevelStatus.Available, false);
        return result;
    }
}