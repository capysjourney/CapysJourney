using System.Threading.Tasks;

public static class CarrotManager
{
    public static async Task<int> GetNumCarrots()
    {
        int result = 0;
        await DataManager.WithStats(stats => result = stats.NumCarrots, false);
        return result;
    }

    public static async Task IncreaseCarrots(int numCarrots)
    {
        if (numCarrots <= 0) return;
        await DataManager.WithStats(stats => stats.IncreaseCarrots(numCarrots, BadgeManager.HandleBadgesEarned), true);
        NavBarScript.Instance.RefreshCarrotCount();
    }
}