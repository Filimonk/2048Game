using System;
using UnityEngine;

namespace _Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameField gameField;

        public void Start()
        {
            Canvas.ForceUpdateCanvases();
            
            gameField.CreateCell();
            gameField.CreateCell();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                gameField.MoveCells(GameField.Direction.Up);
                gameField.CreateCell();
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gameField.MoveCells(GameField.Direction.Left);
                gameField.CreateCell();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                gameField.MoveCells(GameField.Direction.Down);
                gameField.CreateCell();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                gameField.MoveCells(GameField.Direction.Right);
                gameField.CreateCell();
            }
        }
    }
}