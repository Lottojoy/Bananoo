public static class ScoreManager
{
    public static void CalculateFinalScore(ScoreData data)
    {
        float minutes = data.TimeUsed / 60f;
        if (minutes <= 0f) minutes = 1f;

        data.WPM = (data.correctHits / 5f) / minutes;
        int totalHits = data.correctHits + data.wrongHits + data.corrections;
        data.ACC = (totalHits > 0) ? (float)data.correctHits / totalHits * 100f : 0f;
        
    }
}
