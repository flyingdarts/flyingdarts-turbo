namespace Flyingdarts.Persistence;

public class X01GameSettings : IGameSettings
{
    public bool DoubleIn { get; set; }
    public bool DoubleOut { get; set; }
    public int StartingScore { get; set; }

    public X01GameSettings()
    {
        // will be required for dynamodb
    }
    public X01GameSettings(int sets, int legs, bool doubleIn, bool doubleOut, int startingScore)
    {
        Sets = sets;
        Legs = legs;
        DoubleIn = doubleIn;
        DoubleOut = doubleOut;
        StartingScore = startingScore;
    }

    public static X01GameSettings Create(int sets, int legs)
    {
        return new X01GameSettings
        {
            Sets = sets,
            Legs = legs,
            DoubleIn = false,
            DoubleOut = true,
            StartingScore = 501
        };
    }

    public int Legs { get; set; }
    public int Sets { get; set; }
}