public enum AgeGroup
{
    /// <summary>
    /// 4-6 years old
    /// </summary>
    Preschool,

    /// <summary>
    /// 7-10 years old
    /// </summary>
    Child,

    /// <summary>
    /// 11-14 years old
    /// </summary>
    YoungTeen,

    /// <summary>
    /// 15-17 years old
    /// </summary>
    OldTeen,

    /// <summary>
    /// 18+ years old
    /// </summary>
    Adult
}

public static class AgeGroupMethods
{
    public static AgeGroup FromAge(int age)
    {
        return age switch
        {
            >= 18 => AgeGroup.Adult,
            >= 15 => AgeGroup.OldTeen,
            >= 11 => AgeGroup.YoungTeen,
            >= 7 => AgeGroup.Child,
            _ => AgeGroup.Preschool,
        };
    }
}