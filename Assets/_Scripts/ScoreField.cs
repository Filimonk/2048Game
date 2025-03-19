using System;
using UnityEngine;
using TMPro;

namespace _Scripts
{
    public class ScoreField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI value;
        private string prefix = "Score\n";

        public void UpdateValue(int score)
        {
            value.text = prefix + score.ToString();
        }
    }
}
