public static class QuincyManager
{
    public static void CompleteQuincy(World world)
    {
        World completedWorld = world;
        DataManager.WithStats(stats =>
        {
            stats.CompleteQuincy(world, BadgeManager.HandleBadgesEarned);
        }, true);
    }

    public static bool IsQuincyUnlocked(World world)
    {
        bool result = false;
        DataManager.WithStats(stats => result = stats.QuincyStatusOfWorld[world.EnumName] == LevelStatus.Available, false);
        return result;
    }
}