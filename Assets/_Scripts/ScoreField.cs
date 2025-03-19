using System;
using UnityEngine;
using TMPro;

namespace _Scripts
{
    public class ScoreField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI value = null!;
        private string prefix = "Score\n";

        public void Awake()
        {
            value.text = prefix + "0";
        }

        public void UpdateValue(int value)
        {
            this.value.text = prefix + value.ToString();
        }
    }
}
