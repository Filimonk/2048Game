using UnityEngine;

namespace _Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameField gameField;
        
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                gameField.CreateCell();
            }
        }
    }
}