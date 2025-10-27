using UnityEngine;

namespace Assets.data
{
    public class Score
    {
        private int currentScore;
        private int highScore;

        public Score()
        {
            currentScore = 0;
            highScore = PlayerPrefs.GetInt("HighScore", 0);
        }

        public void AddScore(int score)
        {
            currentScore += score;
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("HighScore", highScore);
            }
        }

        public int GetScore()
        {
            return currentScore;
        }
        public int GetHighScore()
        {
            return highScore;
        }

        public void ResetScore()
        {
            currentScore = 0;
        }
    }
}