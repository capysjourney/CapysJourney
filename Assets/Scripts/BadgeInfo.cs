// The order matters for displaying badges in the UI
using System.Collections.Generic;


public enum Badge
{
    None,
    GettingCozy, SettlingIn, ZenMaster, AdventureAwaits, MindfulBeginnings, MeetingQuincy,
    TrailBlazer, TinyButMighty, JustBreathe, DeepBreatheDevotee,
    BreathSage, FeelingFeeler, MoodMapper, InnerWeatherWatcher,
    SeedsOfGratitude, BloomingThanks, GratitudeGardener,
    CapysClosetBegins, DressedToImpress, FreshFit,
    OneStepToCozy, CapyLovesHome, TinyTrader, CapyCollector, CarrotTycoon
}

public class BadgeInfo
{
    public string Name;
    public string Description;
    public string SpritePath;
    public Badge Badge;
    public BadgeInfo(string name, string description, string spritePath, Badge badge)
    {
        Name = name;
        Description = description;
        SpritePath = spritePath;
        Badge = badge;
    }

    // Need to override these 4 operators/methods since
    // writing back from JSON does not preserve reference equality

    public static bool operator ==(BadgeInfo a, BadgeInfo b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return Equals(a, b);
    }

    public static bool operator !=(BadgeInfo a, BadgeInfo b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj is not BadgeInfo badge)
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

    public static readonly BadgeInfo GettingCozy = new("Getting Cozy", "3-day streak", "Badges/gettingCozy", Badge.GettingCozy);
    public static readonly BadgeInfo SettlingIn = new("Settling In", "7-day streak", "Badges/settlingIn", Badge.SettlingIn);
    public static readonly BadgeInfo ZenMaster = new("Zen Master", "50-day streak", "Badges/zenMaster", Badge.ZenMaster);
    public static readonly BadgeInfo AdventureAwaits = new("Adventure Awaits", "Complete Level 1", "Badges/adventureAwaits", Badge.AdventureAwaits);
    public static readonly BadgeInfo MindfulBeginnings = new("Mindful Beginnings", "Complete First Steps", "Badges/mindfulBeginnings", Badge.MindfulBeginnings);
    public static readonly BadgeInfo MeetingQuincy = new("Meeting Quincy", "Complete Quincy's Questions", "Badges/meetingQuincy", Badge.MeetingQuincy);
    public static readonly BadgeInfo TrailBlazer = new("Trailblazer", "Begin a territory after First Steps", "Badges/trailBlazer", Badge.TrailBlazer);
    // TODO - add back later
    //public static readonly Badge MindfulVoyager = new("Mindful Voyager", "Complete Capy's Landing", "Badges/mindfulVoyager");
    //public static readonly Badge SereneExplorer = new("Serene Explorer", "Complete Island 2", "Badges/sereneExplorer");
    //public static readonly Badge CapyConqueror = new("Capy Conqueror", "Complete Island 3", "Badges/capyConqueror");
    public static readonly BadgeInfo TinyButMighty = new("Tiny but Mighty", "Finish 3 Mini Meditations", "Badges/tinyButMighty", Badge.TinyButMighty);
    public static readonly BadgeInfo JustBreathe = new("Just Breathe", "Complete 5 daily breathing exercises", "Badges/justBreathe", Badge.JustBreathe);
    public static readonly BadgeInfo DeepBreatheDevotee = new("Deep Breathe Devotee", "Complete 25 daily breathing exercises", "Badges/deepBreatheDevotee", Badge.DeepBreatheDevotee);
    public static readonly BadgeInfo BreathSage = new("Breath Sage", "Complete 50 daily breathing exercises", "Badges/breathSage", Badge.BreathSage);
    public static readonly BadgeInfo FeelingFeeler = new("Feeling Feeler", "Complete 5 daily mood check-ins", "Badges/feelingFeeler", Badge.FeelingFeeler);
    public static readonly BadgeInfo MoodMapper = new("Mood Mapper", "Complete 25 daily mood check-ins", "Badges/moodMapper", Badge.MoodMapper);
    public static readonly BadgeInfo InnerWeatherWatcher = new("Inner Weather Watcher", "Complete 50 daily mood check-ins", "Badges/innerWeatherWatcher", Badge.InnerWeatherWatcher);
    public static readonly BadgeInfo SeedsOfGratitude = new("Seeds of Gratitude", "Complete 5 daily gratitudes", "Badges/seedsOfGratitude", Badge.SeedsOfGratitude);
    public static readonly BadgeInfo BloomingThanks = new("Blooming Thanks", "Complete 25 daily gratitudes", "Badges/bloomingThanks", Badge.BloomingThanks);
    public static readonly BadgeInfo GratitudeGardener = new("Gratitude Gardener", "Complete 50 daily gratitudes", "Badges/gratitudeGardener", Badge.GratitudeGardener);
    public static readonly BadgeInfo TinyTrader = new("Tiny Trader", "Spend 100 carrots", "Badges/tinyTrader", Badge.TinyTrader);
    public static readonly BadgeInfo CapyCollector = new("Capy Collector", "Spend 500 carrots", "Badges/capyCollector", Badge.CapyCollector);
    public static readonly BadgeInfo CarrotTycoon = new("Carrot Tycoon", "Spend 1,000 carrots", "Badges/carrotTycoon", Badge.CarrotTycoon);
    public static readonly BadgeInfo CapysClosetBegins = new("Capy's Closet Begins", "Obtain 1 closet item", "Badges/capysClosetBegins", Badge.CapysClosetBegins);
    public static readonly BadgeInfo DressedToImpress = new("Dressed to Impress", "Obtain 10 closet items", "Badges/dressedToImpress", Badge.DressedToImpress);
    public static readonly BadgeInfo OneStepToCozy = new("One Step to Cozy", "Obtain 1 furniture item", "Badges/oneStepToCozy", Badge.OneStepToCozy);
    public static readonly BadgeInfo CapyLovesHome = new("Capy Loves Home", "Obtain 10 furniture items", "Badges/capyLovesHome", Badge.CapyLovesHome);
    public static readonly BadgeInfo FreshFit = new("Fresh Fit", "Customize Capy for the first time", "Badges/freshFit", Badge.FreshFit);

    public static readonly List<Badge> BadgesInOrder = new()
    {
        Badge.GettingCozy,
        Badge.SettlingIn,
        Badge.ZenMaster,
        Badge.AdventureAwaits,
        Badge.MindfulBeginnings,
        Badge.MeetingQuincy,
        Badge.TrailBlazer,
        //Badge.MindfulVoyager,
        //Badge.SereneExplorer,
        //Badge.CapyConqueror,
        Badge.TinyButMighty,
        Badge.JustBreathe,
        Badge.DeepBreatheDevotee,
        Badge.BreathSage,
        Badge.FeelingFeeler,
        Badge.MoodMapper,
        Badge.InnerWeatherWatcher,
        Badge.SeedsOfGratitude,
        Badge.BloomingThanks,
        Badge.GratitudeGardener,
        Badge.TinyTrader,
        Badge.CapyCollector,
        Badge.CarrotTycoon,
        Badge.CapysClosetBegins,
        Badge.DressedToImpress,
        Badge.OneStepToCozy,
        Badge.CapyLovesHome,
        Badge.FreshFit
    };
}

public static class BadgeExtensions
{
    private static Dictionary<Badge, BadgeInfo> BadgeOfEnum = new()
    {
        { Badge.GettingCozy, BadgeInfo.GettingCozy },
        { Badge.SettlingIn, BadgeInfo.SettlingIn },
        { Badge.ZenMaster, BadgeInfo.ZenMaster },
        { Badge.AdventureAwaits, BadgeInfo.AdventureAwaits },
        { Badge.MindfulBeginnings, BadgeInfo.MindfulBeginnings },
        { Badge.MeetingQuincy, BadgeInfo.MeetingQuincy },
        { Badge.TrailBlazer, BadgeInfo.TrailBlazer },
        { Badge.TinyButMighty, BadgeInfo.TinyButMighty },
        { Badge.JustBreathe, BadgeInfo.JustBreathe },
        { Badge.DeepBreatheDevotee, BadgeInfo.DeepBreatheDevotee },
        { Badge.BreathSage, BadgeInfo.BreathSage },
        { Badge.FeelingFeeler, BadgeInfo.FeelingFeeler },
        { Badge.MoodMapper, BadgeInfo.MoodMapper },
        { Badge.InnerWeatherWatcher, BadgeInfo.InnerWeatherWatcher },
        { Badge.SeedsOfGratitude, BadgeInfo.SeedsOfGratitude },
        { Badge.BloomingThanks, BadgeInfo.BloomingThanks },
        { Badge.GratitudeGardener, BadgeInfo.GratitudeGardener },
        { Badge.TinyTrader, BadgeInfo.TinyTrader },
        { Badge.CapyCollector, BadgeInfo.CapyCollector },
        { Badge.CarrotTycoon, BadgeInfo.CarrotTycoon },
        { Badge.CapysClosetBegins, BadgeInfo.CapysClosetBegins },
        { Badge.DressedToImpress, BadgeInfo.DressedToImpress },
        { Badge.OneStepToCozy, BadgeInfo.OneStepToCozy },
        { Badge.CapyLovesHome, BadgeInfo.CapyLovesHome },
        { Badge.FreshFit, BadgeInfo.FreshFit } };
    public static BadgeInfo GetInfo(this Badge badge) => BadgeOfEnum[badge];

}