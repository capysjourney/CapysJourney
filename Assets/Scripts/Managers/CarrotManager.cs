public static class CarrotManager
{
    public static int GetNumCarrots()
    {
        int result = 0;
        GameManager.WithStats(stats => result = stats.NumCarrots, false);
        return result;
    }

    public static void IncreaseCarrots(int numCarrots)
    {
        if (numCarrots <= 0) return;
        GameManager.WithStats(stats => stats.IncreaseCarrots(numCarrots, GameManager.HandleBadgesEarned), true);
    }
}