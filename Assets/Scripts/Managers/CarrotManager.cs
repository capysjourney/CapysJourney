public static class CarrotManager
{
    public static int GetNumCarrots()
    {
        int result = 0;
        DataManager.WithStats(stats => result = stats.NumCarrots, false);
        return result;
    }

    public static void IncreaseCarrots(int numCarrots)
    {
        if (numCarrots <= 0) return;
        DataManager.WithStats(stats => stats.IncreaseCarrots(numCarrots, BadgeManager.HandleBadgesEarned), true);
    }
}