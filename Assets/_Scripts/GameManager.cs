using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

namespace _Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameField  gameField;
        [SerializeField] private ScoreField scoreField;
        [SerializeField] private HighScoreField highScoreField;

        private PersistentGameData persistentGameData;
        private PersistentGameDataManager persistentGameDataManager;
        
        public void Awake()
        {
            persistentGameData = new PersistentGameData();
            persistentGameDataManager = new PersistentGameDataManager(); // можно подставить аргумент-путь,
                                                                         // тогда будет читаться другой файл
            
            if (persistentGameDataManager.LoadPersistentGameData(ref persistentGameData))
            {
                Debug.Log("Persistent game data loaded successfully");
            }
            else
            {
                Debug.Log("Failed to load persistent game data");
            }
        }
        
        private void OnApplicationQuit()
        {
            persistentGameDataManager.Close();
        }

        public void Start()
        {
            Canvas.ForceUpdateCanvases();
            
            highScoreField.UpdateValue(persistentGameData.GetHighScore());

            if (!persistentGameData.emptyField())
            {
                gameField.InitField(persistentGameData.GetFieldData());
            }
            else
            {
                gameField.CreateCell();
                gameField.CreateCell();
            }

            UpdateScoreFields();
            
            persistentGameData.UpdateFieldAndHighScore(gameField.GetFieldData(), gameField.GetScore());
            persistentGameDataManager.SavePersistentGameData(persistentGameData);
        }

        private void RestartGame()
        {
            gameField.RestartField();
            
            gameField.CreateCell();
            gameField.CreateCell();
            
            UpdateScoreFields();
                    
            persistentGameData.UpdateFieldAndHighScore(gameField.GetFieldData(), gameField.GetScore());
            persistentGameDataManager.SavePersistentGameData(persistentGameData);
        }

        private void UpdateScoreFields()
        {
            int currentScore = gameField.GetScore();
                    
            scoreField.UpdateValue(currentScore);
            highScoreField.UpdateValue(currentScore);
        }

        private bool HandleStepAndLog(GameField.Direction direction, string nameKey)
        {
            if (gameField.CheckAvailabilityOfDirection(direction) == false)
            {
                Debug.Log("Была нажата кнопка: \"" + nameKey + "\", направление не доступно, движение клеток не произведено! | " + 
                          "Время: " + Time.time);
                return false;
            }
            
            gameField.MoveCells(direction);
            gameField.CreateCell();
            
            Debug.Log("Была нажата кнопка: \"" + nameKey + "\", движение клеток произведено. | " + 
                      "Время: " + Time.time);

            return true;
        }

        private bool ProcessKey(KeyCode key)
        {
            string nameKey = key.ToString();

            switch (key)
            {
                case KeyCode.W:
                case KeyCode.UpArrow:
                    return HandleStepAndLog(GameField.Direction.Up, nameKey);
                case KeyCode.A:
                case KeyCode.LeftArrow:
                    return HandleStepAndLog(GameField.Direction.Left, nameKey);
                case KeyCode.S:
                case KeyCode.DownArrow:
                    return HandleStepAndLog(GameField.Direction.Down, nameKey);
                case KeyCode.D: 
                case KeyCode.RightArrow:
                    return HandleStepAndLog(GameField.Direction.Right, nameKey);
                default:
                    Debug.Log("Была нажата кнопка: \"" + Input.inputString + "\", движение клеток не произведено! | " + 
                              "Время: " + Time.time);
                    return false;
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
                bool success = ProcessKey(keyPressed);

                if (success)
                {
                    if (gameField.CheckGameOver())
                    {
                        Debug.Log("Игра окончена! | Время: " + Time.time);
                        RestartGame();
                    }
                    else
                    {
                        UpdateScoreFields();
                        
                        persistentGameData.UpdateFieldAndHighScore(gameField.GetFieldData(), gameField.GetScore());
                        persistentGameDataManager.SavePersistentGameData(persistentGameData);
                    }
                }
            }
        }
    }
}
