using Godot;

public partial class PlayerAttributes : RefCounted
{
    public const int MinValue = 0;
    public const int MaxValue = 100;

    private int _focus = 50;
    private int _energy = 70;
    private int _knowledge = 20;
    private int _confidence = 30;
    private int _networking = 0;
    private int _portfolio = 0;

    public int Focus
    {
        get => _focus;
        set => _focus = Mathf.Clamp(value, MinValue, MaxValue);
    }

    public int Energy
    {
        get => _energy;
        set => _energy = Mathf.Clamp(value, MinValue, MaxValue);
    }

    public int Knowledge
    {
        get => _knowledge;
        set => _knowledge = Mathf.Clamp(value, MinValue, MaxValue);
    }

    public int Confidence
    {
        get => _confidence;
        set => _confidence = Mathf.Clamp(value, MinValue, MaxValue);
    }

    public int Networking
    {
        get => _networking;
        set => _networking = Mathf.Clamp(value, MinValue, MaxValue);
    }

    public int Portfolio
    {
        get => _portfolio;
        set => _portfolio = Mathf.Clamp(value, MinValue, MaxValue);
    }

    public void IncreaseFocus(int amount) => Focus += amount;
    public void DecreaseFocus(int amount) => Focus -= amount;

    public void IncreaseEnergy(int amount) => Energy += amount;
    public void DecreaseEnergy(int amount) => Energy -= amount;

    public void IncreaseKnowledge(int amount) => Knowledge += amount;
    public void DecreaseKnowledge(int amount) => Knowledge -= amount;

    public void IncreaseConfidence(int amount) => Confidence += amount;
    public void DecreaseConfidence(int amount) => Confidence -= amount;
    public void IncreaseNetworking(int amount) => Networking += amount;
    public void DecreaseNetworking(int amount) => Networking -= amount;
    public void IncreasePortfolio(int amount) => Portfolio += amount;
    public void DecreasePortfolio(int amount) => Portfolio -= amount;
}


public partial class PlayerAttributes
{
    public void ResetToDefaults()
    {
        Focus = 50;
        Energy = 70;
        Knowledge = 20;
        Confidence = 30;
        Networking = 0;
        Portfolio = 0;
    }
}
