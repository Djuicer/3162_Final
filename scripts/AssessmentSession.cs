public static class AssessmentSession
{
    public static int LastScore { get; set; } = 0;
    public static int TotalQuestions { get; set; } = 5;

    public static int Programming { get; set; } = 0;
    public static int Confidence { get; set; } = 0;
    public static int CareerProgress { get; set; } = 0;

    public static int LastProgrammingGain { get; set; } = 0;
    public static int LastConfidenceGain { get; set; } = 0;
    public static int LastCareerProgressGain { get; set; } = 0;

    public static int LastAssessmentScore
    {
        get => LastScore;
        set => LastScore = value;
    }

    public static void ApplyAssessmentRewards(int score)
    {
        LastAssessmentScore = score;

        int programmingGain;
        int confidenceGain;
        int careerProgressGain;

        if (score == 5)
        {
            programmingGain = 3;
            confidenceGain = 2;
            careerProgressGain = 3;
        }
        else if (score >= 3)
        {
            programmingGain = 2;
            confidenceGain = 1;
            careerProgressGain = 2;
        }
        else if (score >= 1)
        {
            programmingGain = 1;
            confidenceGain = 0;
            careerProgressGain = 0;
        }
        else
        {
            programmingGain = 0;
            confidenceGain = -1;
            careerProgressGain = 0;
        }

        LastProgrammingGain = programmingGain;
        LastConfidenceGain = confidenceGain;
        LastCareerProgressGain = careerProgressGain;

        Programming += programmingGain;
        Confidence += confidenceGain;
        CareerProgress += careerProgressGain;
    }
}
