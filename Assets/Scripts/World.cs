using System.Collections.Generic;

public enum WorldEnum
{
    FirstSteps
}

public class World
{
    public static readonly World FirstSteps = new(
        name: "First Steps",
        levels: new HashSet<Level>
        {
            Level.World1Level1,
            Level.World1Level2,
            Level.World1Level3,
            Level.World1Level4,
            Level.World1Level5,
            Level.World1Level6,
            Level.World1Level7,
            Level.World1Level8,
            Level.World1Level9,
            Level.World1Level10,
            Level.World1SideLevel1,
            Level.World1SideLevel2,
            Level.World1SideLevel3,
            Level.World1MiniLevel1,
            Level.World1MiniLevel2,
            Level.World1MiniLevel3
        },
        enumName: WorldEnum.FirstSteps,
        firstLevel: Level.World1Level1,
        nextWorlds: new HashSet<WorldEnum>() // todo - add next worlds when available
    );
    public readonly static Dictionary<WorldEnum, World> WorldLookup = new()
    {
        { WorldEnum.FirstSteps, FirstSteps }
    };

    public string Name { get; }
    public HashSet<Level> Levels { get; }
    public WorldEnum EnumName { get; }
    public Level FirstLevel { get; }
    public static HashSet<World> AllWorlds = new() { FirstSteps };
    public HashSet<WorldEnum> NextWorlds { get; }

    private World(string name, HashSet<Level> levels, WorldEnum enumName, Level firstLevel, HashSet<WorldEnum> nextWorlds)
    {
        Name = name;
        Levels = levels;
        EnumName = enumName;
        FirstLevel = firstLevel;
        NextWorlds = nextWorlds;
    }
}