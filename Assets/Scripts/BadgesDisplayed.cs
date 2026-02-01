using System.Collections.Generic;

public class BadgesDisplayed
{
    public Badge firstBadge = Badge.None;
    public Badge secondBadge = Badge.None;
    public Badge thirdBadge = Badge.None;

    public void AddBadge(Badge badge)
    {
        if (firstBadge == Badge.None)
        {
            firstBadge = badge;
        }
        else if (secondBadge == Badge.None)
        {
            secondBadge = badge;
        }
        else if (thirdBadge == Badge.None)
        {
            thirdBadge = badge;
        }
    }

    public int NumBadgesDisplayed()
    {
        int count = 0;
        if (firstBadge != Badge.None) count++;
        if (secondBadge != Badge.None) count++;
        if (thirdBadge != Badge.None) count++;
        return count;
    }

    public List<Badge> GetBadges()
    {
        List<Badge> badges = new();
        if (firstBadge != Badge.None) badges.Add(firstBadge);
        if (secondBadge != Badge.None) badges.Add(secondBadge);
        if (thirdBadge != Badge.None) badges.Add(thirdBadge);
        return badges;
    }

    public void RemoveBadge(Badge badge)
    {
        if (firstBadge == badge)
        {
            firstBadge = secondBadge;
            secondBadge = thirdBadge;
            thirdBadge = Badge.None;
        }
        else if (secondBadge == badge)
        {
            secondBadge = thirdBadge;
            thirdBadge = Badge.None;
        }
        else if (thirdBadge == badge)
        {
            thirdBadge = Badge.None;
        }
    }
}
