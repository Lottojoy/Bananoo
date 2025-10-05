[System.Serializable]
public class ScoreData
{
    public int totalSyllables;
    public int correctHits;
    public int wrongHits;
    public int corrections;

    public float TimeUsed;
    public float WPM;
    public float ACC;
    public float FinalScore;

    public void Reset()
    {
        
        totalSyllables = correctHits = wrongHits = corrections = 0;
        TimeUsed = WPM = ACC = FinalScore = 0f;
    }
}
