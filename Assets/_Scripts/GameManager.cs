using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private void RestartGame()
        {
            gameField.RestartField();
            Start();
        }

        private void HandleStepAndLog(GameField.Direction direction, string nameKey)
        {
            if (gameField.CheckAvailabilityOfDirection(direction) == false)
            {
                Debug.Log("Была нажата кнопка: \"" + nameKey + "\", направление не доступно, движение клеток не произведено! | " + 
                          "Время: " + Time.time);
                return;
            }
            
            gameField.MoveCells(direction);
            gameField.CreateCell();
            
            Debug.Log("Была нажата кнопка: \"" + nameKey + "\", движение клеток произведено. | " + 
                      "Время: " + Time.time);

            if (gameField.CheckGameOver())
            {
                Debug.Log("Игра окончена! | Время: " + Time.time);
                RestartGame();
            }
        }

        private void ProcessKey(KeyCode key)
        {
            string nameKey = key.ToString();

            switch (key)
            {
                case KeyCode.W:
                case KeyCode.UpArrow:
                    HandleStepAndLog(GameField.Direction.Up, nameKey);
                    break;
                case KeyCode.A:
                case KeyCode.LeftArrow:
                    HandleStepAndLog(GameField.Direction.Left, nameKey);
                    break;
                case KeyCode.S:
                case KeyCode.DownArrow:
                    HandleStepAndLog(GameField.Direction.Down, nameKey);
                    break;
                case KeyCode.D: 
                case KeyCode.RightArrow:
                    HandleStepAndLog(GameField.Direction.Right, nameKey);
                    break;
                default:
                    Debug.Log("Была нажата кнопка: \"" + Input.inputString + "\", движение клеток не произведено! | " + 
                              "Время: " + Time.time);
                    break;
            }
        }

        public void Update()
        {
            List<KeyCode> keysToTrack = new List<KeyCode> {
                KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, 
                KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow
            };

            int keyPressedCount = 0;
            KeyCode keyPressed = 0;
        
            foreach (KeyCode key in keysToTrack)
            {
                if (Input.GetKeyDown(key))
                {
                    ++keyPressedCount;
                    keyPressed = key;
                }
            }

            if (keyPressedCount > 1)
            {
                Debug.Log("Одновременно нажато несколько кнопок, движение клеток не произведено! | " + 
                          "Время: " + Time.time);
            }
            else if (keyPressedCount == 1)
            {
                ProcessKey(keyPressed);
            }
        }
    }
}