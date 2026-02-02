using System.Collections.Generic;


// Note: WorldId should be used to reference worlds, rather than World objects directly.
// World objects should only be used when necessary.

public enum World
{
    FirstSteps,
    PresentMoment,
    EverydayMindfulness,
    //ExploringAwareness,
    //Compassion,
    //Sleep

}

public class WorldInfo
{
    public static readonly WorldInfo FirstSteps = new(
        name: "First Steps",
        levels: new HashSet<Level>
        {
            Level.FirstSteps_L1,
            Level.FirstSteps_L2,
            Level.FirstSteps_L3,
            Level.FirstSteps_L4,
            Level.FirstSteps_L5,
            Level.FirstSteps_L6,
            Level.FirstSteps_L7,
            Level.FirstSteps_L8,
            Level.FirstSteps_L9,
            Level.FirstSteps_L10,
            Level.FirstSteps_SL1,
            Level.FirstSteps_SL2,
            Level.FirstSteps_SL3,
            Level.FirstSteps_MM1,
            Level.FirstSteps_MM2,
            Level.FirstSteps_MM3
        },
        world: World.FirstSteps,
        firstLevel: Level.FirstSteps_L1,
        nextWorlds: new HashSet<World> { World.PresentMoment, World.EverydayMindfulness }
        );

    public static readonly WorldInfo PresentMoment = new(
        name: "Present Moment",
        levels: new HashSet<Level> { 
            Level.PresentMoment_L1,
            Level.PresentMoment_L2,
            Level.PresentMoment_L3,
            Level.PresentMoment_L4,
            Level.PresentMoment_SL1,
            Level.PresentMoment_SL2
        },
        world: World.PresentMoment,
        firstLevel: Level.PresentMoment_L1,
        nextWorlds: new HashSet<World>() // todo - add next worlds
    );

    public static readonly WorldInfo EverydayMindfulness = new(
        name: "Everyday Mindfulness",
        levels: new HashSet<Level> { 
            Level.EverydayMindfulness_L1,
            Level.EverydayMindfulness_L2,
            Level.EverydayMindfulness_L3,
            Level.EverydayMindfulness_L4,
            Level.EverydayMindfulness_SL1,
            Level.EverydayMindfulness_SL2
        },
        world: World.EverydayMindfulness,
        firstLevel: Level.EverydayMindfulness_L1,
        nextWorlds: new HashSet<World>() // todo - add next worlds
    );

    //public static readonly WorldInfo ExploringAwareness = new(
    //    name: "Exploring Awareness",
    //    levels: new HashSet<Level>(),
    //    world: World.ExploringAwareness,
    //    firstLevel: null,
    //    nextWorlds: new HashSet<World>()
    //);

    //public static readonly WorldInfo Compassion = new(
    //    name: "Compassion",
    //    levels: new HashSet<Level>(),
    //    world: World.Compassion,
    //    firstLevel: null,
    //    nextWorlds: new HashSet<World>()
    //);

    //public static readonly WorldInfo Sleep = new(
    //    name: "Sleep",
    //    levels: new HashSet<Level>(),
    //    world: World.Sleep,
    //    firstLevel: null,
    //    nextWorlds: new HashSet<World>()
    //);

    public string Name { get; }
    public HashSet<Level> Levels { get; }
    public World World { get; }
    public Level FirstLevel { get; }
    public static HashSet<WorldInfo> AllWorlds = new() { FirstSteps };
    public HashSet<World> NextWorlds { get; }

    private WorldInfo(string name, HashSet<Level> levels, World world, Level firstLevel, HashSet<World> nextWorlds)
    {
        Name = name;
        Levels = levels;
        World = world;
        FirstLevel = firstLevel;
        NextWorlds = nextWorlds;
    }
}

public static class WorldExtensions
{
    private readonly static Dictionary<World, WorldInfo> InfoOfWorld = new()
    {
        { World.FirstSteps, WorldInfo.FirstSteps },
        { World.PresentMoment, WorldInfo.PresentMoment },
        { World.EverydayMindfulness, WorldInfo.EverydayMindfulness },
        //{ World.ExploringAwareness, null },
        //{ World.Compassion, null },
        //{ World.Sleep, null }
    };
    public static HashSet<Level> GetLevels(this World world)
    {
        return InfoOfWorld[world].Levels;
    }
    public static WorldInfo GetInfo(this World world)
    {
        return InfoOfWorld[world];
    }
}