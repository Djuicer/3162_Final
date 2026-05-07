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

    public void IncreaseFocus(int amount) => ApplyStatDelta(nameof(Focus), amount);
    public void DecreaseFocus(int amount) => ApplyStatDelta(nameof(Focus), -amount);

    public void IncreaseEnergy(int amount) => ApplyStatDelta(nameof(Energy), amount);
    public void DecreaseEnergy(int amount) => ApplyStatDelta(nameof(Energy), -amount);

    public void IncreaseKnowledge(int amount) => ApplyStatDelta(nameof(Knowledge), amount);
    public void DecreaseKnowledge(int amount) => ApplyStatDelta(nameof(Knowledge), -amount);

    public void IncreaseConfidence(int amount) => ApplyStatDelta(nameof(Confidence), amount);
    public void DecreaseConfidence(int amount) => ApplyStatDelta(nameof(Confidence), -amount);
    public void IncreaseNetworking(int amount) => ApplyStatDelta(nameof(Networking), amount);
    public void DecreaseNetworking(int amount) => ApplyStatDelta(nameof(Networking), -amount);
    public void IncreasePortfolio(int amount) => ApplyStatDelta(nameof(Portfolio), amount);
    public void DecreasePortfolio(int amount) => ApplyStatDelta(nameof(Portfolio), -amount);

    private void ApplyStatDelta(string statName, int delta)
    {
        if (delta == 0)
            return;

        int before;
        switch (statName)
        {
            case nameof(Focus):
                before = Focus;
                Focus += delta;
                GlobalVars.Instance?.NotifyStatChange("Focus", Focus - before);
                break;
            case nameof(Energy):
                before = Energy;
                Energy += delta;
                GlobalVars.Instance?.NotifyStatChange("Energy", Energy - before);
                break;
            case nameof(Knowledge):
                before = Knowledge;
                Knowledge += delta;
                GlobalVars.Instance?.NotifyStatChange("Knowledge", Knowledge - before);
                break;
            case nameof(Confidence):
                before = Confidence;
                Confidence += delta;
                GlobalVars.Instance?.NotifyStatChange("Confidence", Confidence - before);
                break;
            case nameof(Networking):
                before = Networking;
                Networking += delta;
                GlobalVars.Instance?.NotifyStatChange("Networking", Networking - before);
                break;
            case nameof(Portfolio):
                before = Portfolio;
                Portfolio += delta;
                GlobalVars.Instance?.NotifyStatChange("Portfolio", Portfolio - before);
                break;
        }
    }
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
