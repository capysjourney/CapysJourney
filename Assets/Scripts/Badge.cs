// The order matters for displaying badges in the UI
using System.Collections.Generic;

public class Badge
{
    public string Name;
    public string Description;
    public string SpritePath;
    public Badge(string name, string description, string spritePath)
    {
        Name = name;
        Description = description;
        SpritePath = spritePath;
    }

    // Need to override these 4 operators/methods since
    // writing back from JSON does not preserve reference equality

    public static bool operator ==(Badge a, Badge b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return Equals(a, b);
    }

    public static bool operator !=(Badge a, Badge b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj is not Badge badge)
        {
            return false;
        }
        return badge.Name == Name
            && badge.Description == Description
            && badge.SpritePath == SpritePath;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(Name, Description, SpritePath);
    }

    public static readonly Badge GettingCozy = new("Getting Cozy", "3-day streak", "Badges/gettingCozy");
    public static readonly Badge SettlingIn = new("Settling In", "7-day streak", "Badges/settlingIn");
    public static readonly Badge ZenMaster = new("Zen Master", "50-day streak", "Badges/zenMaster");
    public static readonly Badge AdventureAwaits = new("Adventure Awaits", "Complete Level 1", "Badges/adventureAwaits");
    public static readonly Badge MindfulBeginnings = new("Mindful Beginnings", "Complete First Steps", "Badges/mindfulBeginnings");
    public static readonly Badge MeetingQuincy = new("Meeting Quincy", "Complete Quincy's Questions", "Badges/meetingQuincy");
    public static readonly Badge TrailBlazer = new("Trailblazer", "Begin a territory after First Steps", "Badges/trailBlazer");
    // TODO - add back later
    //public static readonly Badge MindfulVoyager = new("Mindful Voyager", "Complete Capy's Landing", "Badges/mindfulVoyager");
    //public static readonly Badge SereneExplorer = new("Serene Explorer", "Complete Island 2", "Badges/sereneExplorer");
    //public static readonly Badge CapyConqueror = new("Capy Conqueror", "Complete Island 3", "Badges/capyConqueror");
    public static readonly Badge TinyButMighty = new("Tiny but Mighty", "Finish 3 Mini Meditations", "Badges/tinyButMighty");
    public static readonly Badge LilyLounge = new("Lily Lounge", "Uncover the Lily Pad", "Badges/lilyLounge");
    public static readonly Badge JustBreathe = new("Just Breathe", "Complete 5 daily breathing exercises", "Badges/justBreathe");
    public static readonly Badge DeepBreatheDevotee = new("Deep Breathe Devotee", "Complete 25 daily breathing exercises", "Badges/deepBreatheDevotee");
    public static readonly Badge BreathSage = new("Breath Sage", "Complete 50 daily breathing exercises", "Badges/breathSage");
    public static readonly Badge FeelingFeeler = new("Feeling Feeler", "Complete 5 daily mood check-ins", "Badges/feelingFeeler");
    public static readonly Badge MoodMapper = new("Mood Mapper", "Complete 25 daily mood check-ins", "Badges/moodMapper");
    public static readonly Badge InnerWeatherWatcher = new("Inner Weather Watcher", "Complete 50 daily mood check-ins", "Badges/innerWeatherWatcher");
    public static readonly Badge SeedsOfGratitude = new("Seeds of Gratitude", "Complete 5 daily gratitudes", "Badges/seedsOfGratitude");
    public static readonly Badge BloomingThanks = new("Blooming Thanks", "Complete 25 daily gratitudes", "Badges/bloomingThanks");
    public static readonly Badge GratitudeGardener = new("Gratitude Gardener", "Complete 50 daily gratitudes", "Badges/gratitudeGardener");
    public static readonly Badge TinyTrader = new("Tiny Trader", "Spend 100 carrots", "Badges/tinyTrader");
    public static readonly Badge CapyCollector = new("Capy Collector", "Spend 500 carrots", "Badges/capyCollector");
    public static readonly Badge CarrotTycoon = new("Carrot Tycoon", "Spend 1,000 carrots", "Badges/carrotTycoon");
    public static readonly Badge CapysClosetBegins = new("Capy's Closet Begins", "Obtain 1 closet item", "Badges/capysClosetBegins");
    public static readonly Badge DressedToImpress = new("Dressed to Impress", "Obtain 10 closet items", "Badges/dressedToImpress");
    public static readonly Badge OneStepToCozy = new("One Step to Cozy", "Obtain 1 furniture item", "Badges/oneStepToCozy");
    public static readonly Badge CapyLovesHome = new("Capy Loves Home", "Obtain 10 furniture items", "Badges/capyLovesHome");
    public static readonly Badge FreshFit = new("Fresh Fit", "Customize Capy for the first time", "Badges/freshFit");

    public static readonly List<Badge> BadgesInOrder = new()
    {
        GettingCozy,
        SettlingIn,
        ZenMaster,
        AdventureAwaits,
        MindfulBeginnings,
        MeetingQuincy,
        TrailBlazer,
        //MindfulVoyager,
        //SereneExplorer,
        //CapyConqueror,
        TinyButMighty,
        LilyLounge,
        JustBreathe,
        DeepBreatheDevotee,
        BreathSage,
        FeelingFeeler,
        MoodMapper,
        InnerWeatherWatcher,
        SeedsOfGratitude,
        BloomingThanks,
        GratitudeGardener,
        TinyTrader,
        CapyCollector,
        CarrotTycoon,
        CapysClosetBegins,
        DressedToImpress,
        OneStepToCozy,
        CapyLovesHome,
        FreshFit
    };
}