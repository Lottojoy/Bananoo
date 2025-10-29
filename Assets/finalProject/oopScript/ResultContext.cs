// ResultContext.cs
public static class ResultContext
{
    public static ScoreData LastScore;

    public static void Set(ScoreData d) { LastScore = d; }
    public static void Clear() { LastScore = null; }
}
