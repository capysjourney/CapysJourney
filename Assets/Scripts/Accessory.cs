public enum Tier
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum AccessoryType
{
    Hat,
    Neckwear,
    Clothing,
    Facewear,
    Pet
}

public class Accessory
{
    public static Accessory OrangeHat = new("Orange Hat", AccessoryType.Hat, Tier.Common);
    public static Accessory PartyHat = new("Party Hat", AccessoryType.Hat, Tier.Rare);
    public static Accessory AstronautHelmet = new("Party Hat", AccessoryType.Hat, Tier.Epic);
    public static Accessory UnicornHorn = new("Unicorn Horn", AccessoryType.Hat, Tier.Legendary);

    public static Accessory Scarf = new("Scarf", AccessoryType.Neckwear, Tier.Common);
    public static Accessory PearlNecklace = new("Pearl Necklace", AccessoryType.Neckwear, Tier.Rare);
    public static Accessory FeatherBoa = new("Feather Boa", AccessoryType.Neckwear, Tier.Epic);
    public static Accessory GoldChain = new("Gold Chain", AccessoryType.Neckwear, Tier.Legendary);


    public static Accessory Tshirt = new("T-shirt", AccessoryType.Clothing, Tier.Common);
    public static Accessory Floatie = new("Floatie", AccessoryType.Clothing, Tier.Rare);
    public static Accessory SpaceSuit = new("Space Suit", AccessoryType.Clothing, Tier.Epic);
    public static Accessory RoyalRobes = new("Royal Robes", AccessoryType.Clothing, Tier.Legendary);

    public static Accessory Mustache = new("Mustache", AccessoryType.Facewear, Tier.Common);
    public static Accessory HeartGlasses = new("Heart Glasses", AccessoryType.Facewear, Tier.Rare);
    public static Accessory SuperheroMask = new("Superhero Mask", AccessoryType.Facewear, Tier.Epic);
    public static Accessory GoldMasqueradeMask = new("Gold Masquerade Mask", AccessoryType.Facewear, Tier.Legendary);

    public static Accessory Rock = new("Rock", AccessoryType.Pet, Tier.Common);
    public static Accessory RubberDuck = new("Rubber Duck", AccessoryType.Pet, Tier.Rare);
    public static Accessory FloatingKoiFish = new("Floating Koi Fish", AccessoryType.Pet, Tier.Epic);
    public static Accessory GoldenBabyCapy = new("Golden Baby Capy", AccessoryType.Pet, Tier.Legendary);

    public static Accessory[] AllAccesories = {
        OrangeHat, PartyHat, AstronautHelmet, UnicornHorn,
        Scarf, PearlNecklace, FeatherBoa, GoldChain,
        Tshirt, Floatie, SpaceSuit, RoyalRobes,
        Mustache, HeartGlasses, SuperheroMask, GoldMasqueradeMask,
        Rock, RubberDuck, FloatingKoiFish, GoldenBabyCapy
    };

    public string Name;
    public AccessoryType Type;
    public Tier Tier;
    public Accessory(string name, AccessoryType type, Tier tier)
    {
        Name = name;
        Type = type;
        Tier = tier;
    }

    public static bool operator==(Accessory a1, Accessory a2)
    {
        if (a1 is null && a2 is null) return true;
        if (a1 is null || a2 is null) return false;
        return a1.Equals(a2);
    }

    public static bool operator !=(Accessory a1, Accessory a2) => !(a1 == a2);


    /// <summary>
    /// Returns whether <c>obj</c> has the same name, type, and tier as <c>this</c>.
    /// </summary>
    /// <param name="obj"></param>
    public override bool Equals(object obj)
    {
        if (obj is not Accessory acc) return false;
        return acc.Name == Name && acc.Type == Type && acc.Tier == Tier;
    }

    /// <summary>
    /// Returns a hash code based on Name, Type, and Tier.
    /// </summary>
    public override int GetHashCode()
    {
        return System.HashCode.Combine(Name, Type, Tier);
    }

}
