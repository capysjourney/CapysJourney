// The order matters for displaying badges in the UI
using System.Collections.Generic;


public enum BadgeEnum
{
    GettingCozy, SettlingIn, ZenMaster, AdventureAwaits, MindfulBeginnings, MeetingQuincy,
    TrailBlazer, TinyButMighty, JustBreathe, DeepBreatheDevotee,
    BreathSage, FeelingFeeler, MoodMapper, InnerWeatherWatcher,
    SeedsOfGratitude, BloomingThanks, GratitudeGardener,
    CapysClosetBegins, DressedToImpress, FreshFit,
    OneStepToCozy, CapyLovesHome, TinyTrader, CapyCollector, CarrotTycoon
}

public class Badge
{
    public string Name;
    public string Description;
    public string SpritePath;
    public BadgeEnum EnumName;
    public Badge(string name, string description, string spritePath, BadgeEnum enumName)
    {
        Name = name;
        Description = description;
        SpritePath = spritePath;
        EnumName = enumName;
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

    public static readonly Badge GettingCozy = new("Getting Cozy", "3-day streak", "Badges/gettingCozy", BadgeEnum.GettingCozy);
    public static readonly Badge SettlingIn = new("Settling In", "7-day streak", "Badges/settlingIn", BadgeEnum.SettlingIn);
    public static readonly Badge ZenMaster = new("Zen Master", "50-day streak", "Badges/zenMaster", BadgeEnum.ZenMaster);
    public static readonly Badge AdventureAwaits = new("Adventure Awaits", "Complete Level 1", "Badges/adventureAwaits", BadgeEnum.AdventureAwaits);
    public static readonly Badge MindfulBeginnings = new("Mindful Beginnings", "Complete First Steps", "Badges/mindfulBeginnings", BadgeEnum.MindfulBeginnings);
    public static readonly Badge MeetingQuincy = new("Meeting Quincy", "Complete Quincy's Questions", "Badges/meetingQuincy", BadgeEnum.MeetingQuincy);
    public static readonly Badge TrailBlazer = new("Trailblazer", "Begin a territory after First Steps", "Badges/trailBlazer", BadgeEnum.TrailBlazer);
    // TODO - add back later
    //public static readonly Badge MindfulVoyager = new("Mindful Voyager", "Complete Capy's Landing", "Badges/mindfulVoyager");
    //public static readonly Badge SereneExplorer = new("Serene Explorer", "Complete Island 2", "Badges/sereneExplorer");
    //public static readonly Badge CapyConqueror = new("Capy Conqueror", "Complete Island 3", "Badges/capyConqueror");
    public static readonly Badge TinyButMighty = new("Tiny but Mighty", "Finish 3 Mini Meditations", "Badges/tinyButMighty", BadgeEnum.TinyButMighty);
    public static readonly Badge JustBreathe = new("Just Breathe", "Complete 5 daily breathing exercises", "Badges/justBreathe", BadgeEnum.JustBreathe);
    public static readonly Badge DeepBreatheDevotee = new("Deep Breathe Devotee", "Complete 25 daily breathing exercises", "Badges/deepBreatheDevotee", BadgeEnum.DeepBreatheDevotee);
    public static readonly Badge BreathSage = new("Breath Sage", "Complete 50 daily breathing exercises", "Badges/breathSage", BadgeEnum.BreathSage);
    public static readonly Badge FeelingFeeler = new("Feeling Feeler", "Complete 5 daily mood check-ins", "Badges/feelingFeeler", BadgeEnum.FeelingFeeler);
    public static readonly Badge MoodMapper = new("Mood Mapper", "Complete 25 daily mood check-ins", "Badges/moodMapper", BadgeEnum.MoodMapper);
    public static readonly Badge InnerWeatherWatcher = new("Inner Weather Watcher", "Complete 50 daily mood check-ins", "Badges/innerWeatherWatcher", BadgeEnum.InnerWeatherWatcher);
    public static readonly Badge SeedsOfGratitude = new("Seeds of Gratitude", "Complete 5 daily gratitudes", "Badges/seedsOfGratitude", BadgeEnum.SeedsOfGratitude);
    public static readonly Badge BloomingThanks = new("Blooming Thanks", "Complete 25 daily gratitudes", "Badges/bloomingThanks", BadgeEnum.BloomingThanks);
    public static readonly Badge GratitudeGardener = new("Gratitude Gardener", "Complete 50 daily gratitudes", "Badges/gratitudeGardener", BadgeEnum.GratitudeGardener);
    public static readonly Badge TinyTrader = new("Tiny Trader", "Spend 100 carrots", "Badges/tinyTrader", BadgeEnum.TinyTrader);
    public static readonly Badge CapyCollector = new("Capy Collector", "Spend 500 carrots", "Badges/capyCollector", BadgeEnum.CapyCollector);
    public static readonly Badge CarrotTycoon = new("Carrot Tycoon", "Spend 1,000 carrots", "Badges/carrotTycoon", BadgeEnum.CarrotTycoon);
    public static readonly Badge CapysClosetBegins = new("Capy's Closet Begins", "Obtain 1 closet item", "Badges/capysClosetBegins", BadgeEnum.CapysClosetBegins);
    public static readonly Badge DressedToImpress = new("Dressed to Impress", "Obtain 10 closet items", "Badges/dressedToImpress", BadgeEnum.DressedToImpress);
    public static readonly Badge OneStepToCozy = new("One Step to Cozy", "Obtain 1 furniture item", "Badges/oneStepToCozy", BadgeEnum.OneStepToCozy);
    public static readonly Badge CapyLovesHome = new("Capy Loves Home", "Obtain 10 furniture items", "Badges/capyLovesHome", BadgeEnum.CapyLovesHome);
    public static readonly Badge FreshFit = new("Fresh Fit", "Customize Capy for the first time", "Badges/freshFit", BadgeEnum.FreshFit);

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

    public static Dictionary<BadgeEnum, Badge> BadgeOfEnum = new()
    {
        { BadgeEnum.GettingCozy, GettingCozy },
        { BadgeEnum.SettlingIn, SettlingIn },
        { BadgeEnum.ZenMaster, ZenMaster },
        { BadgeEnum.AdventureAwaits, AdventureAwaits },
        { BadgeEnum.MindfulBeginnings, MindfulBeginnings },
        { BadgeEnum.MeetingQuincy, MeetingQuincy },
        { BadgeEnum.TrailBlazer, TrailBlazer },
        { BadgeEnum.TinyButMighty, TinyButMighty },
        { BadgeEnum.JustBreathe, JustBreathe },
        { BadgeEnum.DeepBreatheDevotee, DeepBreatheDevotee },
        { BadgeEnum.BreathSage, BreathSage },
        { BadgeEnum.FeelingFeeler, FeelingFeeler },
        { BadgeEnum.MoodMapper, MoodMapper },
        { BadgeEnum.InnerWeatherWatcher, InnerWeatherWatcher },
        { BadgeEnum.SeedsOfGratitude, SeedsOfGratitude },
        { BadgeEnum.BloomingThanks, BloomingThanks },
        { BadgeEnum.GratitudeGardener, GratitudeGardener },
        { BadgeEnum.TinyTrader, TinyTrader },
        { BadgeEnum.CapyCollector, CapyCollector },
        { BadgeEnum.CarrotTycoon, CarrotTycoon },
        { BadgeEnum.CapysClosetBegins, CapysClosetBegins },
        { BadgeEnum.DressedToImpress, DressedToImpress },
        { BadgeEnum.OneStepToCozy, OneStepToCozy },
        { BadgeEnum.CapyLovesHome, CapyLovesHome },
        { BadgeEnum.FreshFit, FreshFit } };
}