#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public class GameField : MonoBehaviour
    {
        [SerializeField] public int WIDTH;
        [SerializeField] public int HEIGHT;
        
        [SerializeField] private RectTransform slotsPanelTransform = null!;
        [SerializeField] private GridLayoutGroup slotsGridLayoutGroup = null!;
        [SerializeField] private RectTransform cellsPanelTransform = null!;

        [SerializeField] private GameObject slotForCellPrefab = null!;
        [SerializeField] private CellView cellViewPrefab = null!;

        private List<GameObject> slotsForCells = new List<GameObject>();
        private List<Cell?> field = new List<Cell?>();
        private List<Cell> cells = new List<Cell>();

        public void Awake()
        {
            slotsGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            slotsGridLayoutGroup.constraintCount = WIDTH;
        
            for (int i = 0; i < WIDTH * HEIGHT; ++i)
            {
                GameObject newSlotForCell = Instantiate(slotForCellPrefab, slotsPanelTransform);
                slotsForCells.Add(newSlotForCell);
            
                field.Add(null);
            }
        }

        private Vector2Int? GetEmptySlotPosition()
        {
            List<Vector2Int> emptySlotsPositions = new List<Vector2Int>();
        
            for (int i = 0; i < WIDTH * HEIGHT; ++i)
            {
                if (field[i] == null)
                {
                    emptySlotsPositions.Add(new Vector2Int(i % WIDTH, i / WIDTH));
                }
            }

            if (emptySlotsPositions.Count == 0)
            {
                return null;
            }

            return emptySlotsPositions[Random.Range(0, emptySlotsPositions.Count)];
        }

        public Vector3 GetSlotCoordinates(int x, int y)
        {
            return slotsForCells[y * WIDTH + x].transform.position;
        }

        public Vector3 GetSlotCoordinates(Vector2Int position)
        {
            return GetSlotCoordinates(position.x, position.y);
        }

        public void CreateCell()
        {
            Vector2Int? emptySlotPosition = GetEmptySlotPosition();

            if (emptySlotPosition == null)
            {
                Debug.Log("Нет свободных позиций для новой клетки!");
                return;
            }

            int value = Random.value < 0.9f ? 1 : 2;

            Cell newCell = new Cell(value, GetSlotCoordinates(emptySlotPosition.Value));
            field[emptySlotPosition.Value.y * WIDTH + emptySlotPosition.Value.x] = newCell;
            cells.Add(newCell);
        
            CellView view = Instantiate(cellViewPrefab, cellsPanelTransform);
            view.Init(newCell);
        }
    }
}
