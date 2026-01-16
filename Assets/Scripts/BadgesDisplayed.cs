using NUnit.Framework;
using System.Collections.Generic;

public class BadgesDisplayed
{
    public Badge firstBadge = null;
    public Badge secondBadge = null;
    public Badge thirdBadge = null;

    public void RemoveFirstBadge()
    {
        firstBadge = secondBadge;
        secondBadge = thirdBadge;
        thirdBadge = null;
    }

    public void RemoveSecondBadge()
    {
        secondBadge = thirdBadge;
        thirdBadge = null;
    }

    public void RemoveThirdBadge()
    {
        thirdBadge = null;
    }

    public void AddBadge(Badge badge)
    {
        if (firstBadge == null)
        {
            firstBadge = badge;
        }
        else if (secondBadge == null)
        {
            secondBadge = badge;
        }
        else if (thirdBadge == null)
        {
            thirdBadge = badge;
        }
    }

    public int NumBadgesDisplayed()
    {
        int count = 0;
        if (firstBadge != null) count++;
        if (secondBadge != null) count++;
        if (thirdBadge != null) count++;
        return count;
    }

    public List<Badge> GetBadges()
    {
        List<Badge> badges = new();
        if (firstBadge != null) badges.Add(firstBadge);
        if (secondBadge != null) badges.Add(secondBadge);
        if (thirdBadge != null) badges.Add(thirdBadge);
        return badges;
    }
}
