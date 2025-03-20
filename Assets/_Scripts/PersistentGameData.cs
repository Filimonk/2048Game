using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts
{
    [Serializable]
    public class PersistentGameData // можно создавать свои реализации (выбирать, что хотим
                                    // сохранять, а что нет) и легко передавать их в PersistentGameDataManager
    {
        [Serializable]
        private class CellData
        {
            private int points;
            private int positionX;
            private int positionY;

            public CellData(int points, Vector2Int position)
            {
                this.points = points;
                positionX = position.x;
                positionY = position.y;
            }

            public int GetPoints()
            {
                return points;
            }

            public Vector2Int GetPosition()
            {
                return new Vector2Int(positionX, positionY);
            }
        }
        
        private int highScore = 0;
        private List<CellData> cellsData = new List<CellData>();

        public int GetHighScore()
        {
            return highScore;
        }

        public bool emptyField()
        {
            return cellsData.Count == 0;
        }

        public void UpdateFieldAndHighScore(List<Tuple<int, Vector2Int>> fieldData, int score)
        {
            cellsData.Clear();
            
            for (int i = 0; i < fieldData.Count; ++i)
            {
                cellsData.Add(new CellData(fieldData[i].Item1, fieldData[i].Item2));
            }

            if (score > highScore)
            {
                highScore = score;
            }
        }

        public List<Tuple<int, Vector2Int>> GetFieldData()
        {
            List<Tuple<int, Vector2Int>> fieldData = new List<Tuple<int, Vector2Int>>();
            
            for (int i = 0; i < cellsData.Count; ++i)
            {
                fieldData.Add(new Tuple<int, Vector2Int>(cellsData[i].GetPoints(), cellsData[i].GetPosition()));
            }

            return fieldData;
        }
    }
}