using UnityEngine;
using TMPro;

namespace _Scripts
{
    public class HighScoreField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI value;
        private string prefix = "High\n";
        private int score = 0;

        public void UpdateValue(int score)
        {
            if (score > this.score)
            {
                this.score = score;
                value.text = prefix + score.ToString();
            }
        }
    }
}