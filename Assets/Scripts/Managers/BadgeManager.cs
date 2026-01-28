using System.Collections.Generic;

public static class BadgeManager
{
    public static HashSet<Badge> GetBadgesOwned()
    {
        HashSet<Badge> result = new();
        DataManager.WithStats(stats =>
        {
            HashSet<BadgeEnum> badgeEnums = stats.BadgesEarned;
            foreach (BadgeEnum badgeEnum in badgeEnums)
            {
                result.Add(Badge.BadgeOfEnum[badgeEnum]);
            }
        }, false);
        return result;
    }

    public static BadgesDisplayed GetBadgesDisplayed()
    {
        BadgesDisplayed result = null;
        DataManager.WithStats(stats =>
        {
            result = stats.BadgesCurrentlyDisplayed;
        }, false);
        return result;
    }

    public static void SetBadgesDisplayed(BadgesDisplayed badgesDisplayed)
    {
        DataManager.WithStats(stats =>
        {
            stats.BadgesCurrentlyDisplayed = badgesDisplayed;
        }, true);
    }

    public static void HandleBadgesEarned(List<BadgeEnum> badges)
    {
        BadgeNotifManager.Instance.ShowBadgeNotifications(badges);
    }
}