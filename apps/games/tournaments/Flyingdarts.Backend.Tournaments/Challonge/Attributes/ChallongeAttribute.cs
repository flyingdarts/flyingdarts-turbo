public class ChallongeAttribute : Attribute
{
    public string Description { get; }

    public ChallongeAttribute(string description)
    {
        Description = description;
    }
}