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
        [Serializable]
        private class GameData
        {
            public int highScore = 0;
        }

        private string filePath;
        private BinaryFormatter formatter;
        private FileStream stream;
        private GameData gameData;
        
        [SerializeField] private GameField  gameField;
        [SerializeField] private ScoreField scoreField;
        [SerializeField] private HighScoreField highScoreField;
        
        public void Awake()
        {
            filePath = Path.Combine(Application.persistentDataPath, "PersistentDataPath.dat");
            Debug.Log(Application.persistentDataPath + "PersistentDataPath.dat");
            formatter = new BinaryFormatter();
            stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            LoadGameData(ref gameData);
        }
        
        private void SaveGameData(GameData gameData)
        {
            stream.Seek(0, SeekOrigin.Begin);
            formatter.Serialize(stream, gameData);

            if (Random.value < 0.15f) // иногда выгружаем на диск - профилактика от неожиданного завершения 
            {
                stream.Flush();
            }
        }

        private void LoadGameData(ref GameData gameData)
        {
            if (stream.Length > 0)
            {
                stream.Seek(0, SeekOrigin.Begin);
                gameData = (GameData) formatter.Deserialize(stream);
            }
            else
            {
                gameData = new GameData();
            }
        }
        
        private void OnApplicationQuit()
        {
            stream.Close();
        }

        private void UpdateScoreFields()
        {
            int currentScore = gameField.GetScore();
                    
            scoreField.UpdateValue(currentScore);
                
            if (currentScore > gameData.highScore)
            {
                gameData.highScore = currentScore;
                SaveGameData(gameData);
            }
            highScoreField.UpdateValue(gameData.highScore);
        }

        public void Start()
        {
            Canvas.ForceUpdateCanvases();
            
            gameField.CreateCell();
            gameField.CreateCell();
            
            UpdateScoreFields();
        }

        private void RestartGame()
        {
            gameField.RestartField();
            Start();
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

                    UpdateScoreFields();
                }
            }
        }
    }
}
