using System.Collections.Generic;
using System.Threading.Tasks;

public static class BadgeManager
{
    public static async Task<HashSet<Badge>> GetBadgesOwned()
    {
        HashSet<Badge> result = new();
        await DataManager.WithStats(stats =>
        {
            HashSet<Badge> badgeEnums = stats.BadgesEarned;
            foreach (Badge badge in badgeEnums)
            {
                result.Add(badge);
            }
        }, false);
        return result;
    }

    public static async Task<BadgesDisplayed> GetBadgesDisplayed()
    {
        BadgesDisplayed result = null;
        await DataManager.WithStats(stats =>
        {
            result = stats.BadgesCurrentlyDisplayed;
        }, false);
        return result;
    }

    public static async Task SetBadgesDisplayed(BadgesDisplayed badgesDisplayed)
    {
        await DataManager.WithStats(stats =>
        {
            stats.BadgesCurrentlyDisplayed = badgesDisplayed;
        }, true);
    }

    public static void HandleBadgesEarned(List<Badge> badges)
    {
        BadgeNotifManager.Instance.ShowBadgeNotifications(badges);
    }
}