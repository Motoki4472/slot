using UnityEngine;
using TMPro;

namespace Assets.UI
{
    public class ScoreText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        void Start()
        {
            UpdateScore(0);
        }

        public void UpdateScore(int score)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}