using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public int errors = 0;
    public float accuracy = 0f;

    public void CalculateScore(string target, string input)
    {
        int correct = 0;
        int minLen = Mathf.Min(target.Length, input.Length);

        for (int i = 0; i < minLen; i++)
        {
            if (target[i] == input[i]) correct++;
            else errors++;
        }

        score = correct * 10;
        accuracy = (float)correct / target.Length * 100f;
    }

    public void ResetScore()
    {
        score = 0;
        errors = 0;
        accuracy = 0f;
    }
}
