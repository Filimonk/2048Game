#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public class GameField : MonoBehaviour
    {
        [SerializeField] private int WIDTH;
        [SerializeField] private int HEIGHT;
        
        [SerializeField] private RectTransform slotsPanelTransform = null!;
        [SerializeField] private GridLayoutGroup slotsGridLayoutGroup = null!;
        [SerializeField] private RectTransform cellsPanelTransform = null!;

        [SerializeField] private GameObject slotForCellPrefab = null!;
        [SerializeField] private CellView cellViewPrefab = null!;

        private List<GameObject> slotsForCells = new List<GameObject>();
        private List<Cell?> field = new List<Cell?>();
        
        List<bool> availableDirections = new List<bool>();

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

            for (int i = 0; i < 4; ++i)
            {
                availableDirections.Add(true);
            }
        }

        private void RecalculateAvailableDirections()
        {
            for (int i = 0; i < 4; ++i)
            {
                availableDirections[i] = false;
            }
            
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    if (field[y * WIDTH + x] == null)
                    {
                        continue;
                    }
                        
                    foreach (int offset in new List<int> {-1, 1})
                    {
                        if (y + offset >= 0 && y + offset < HEIGHT)
                        {
                            if (field[(y + offset) * WIDTH + x] == null ||
                                field[(y + offset) * WIDTH + x]!.GetValue() == field[y * WIDTH + x]!.GetValue())
                            {
                                availableDirections[(int) (Direction) (1 + offset)] = true; // индексация в соответствии с enum Direction
                            }
                        }
                        
                        if (x + offset >= 0 && x + offset < WIDTH)
                        {
                            if (field[y * WIDTH + (x + offset)] == null ||
                                field[y * WIDTH + (x + offset)]!.GetValue() == field[y * WIDTH + x]!.GetValue())
                            {
                                availableDirections[(int) (Direction) ((4 + offset) % 4)] = true; // индексация в соответствии с enum Direction
                            }
                        }
                    }
                }
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

        private Vector3 GetSlotCoordinates(int x, int y)
        {
            return slotsForCells[y * WIDTH + x].transform.position;
        }

        private Vector3 GetSlotCoordinates(Vector2Int position)
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

            int value = Random.value < 0.8f ? 1 : 2;

            Cell newCell = new Cell(value, GetSlotCoordinates(emptySlotPosition.Value));
            field[emptySlotPosition.Value.y * WIDTH + emptySlotPosition.Value.x] = newCell;
        
            CellView view = Instantiate(cellViewPrefab, cellsPanelTransform);
            view.Init(newCell);

            RecalculateAvailableDirections();
        }

        private void MoveCellsUp(List<Cell?> rotatedField, int width, int height)
        {
            for (int x = 0; x < width; ++x)
            {
                Vector2Int availableSlotPosition = new Vector2Int(x, 0);
                for (int y = 0; y < height; ++y)
                {
                    if (rotatedField[y * width + x] != null && availableSlotPosition != new Vector2Int(x, y))
                    {
                        Cell currentCell = rotatedField[y * width + x]!;
                        
                        if (rotatedField[availableSlotPosition.y * width + availableSlotPosition.x] == null)
                        {
                            rotatedField[y * width + x] = null;
                            rotatedField[availableSlotPosition.y * width + availableSlotPosition.x] = currentCell;
                        }
                        else if (rotatedField[availableSlotPosition.y * width + availableSlotPosition.x]!.GetValue() == currentCell.GetValue())
                        {
                            currentCell.SetValue(currentCell.GetValue() + 1);
                            
                            rotatedField[availableSlotPosition.y * width + availableSlotPosition.x]!.Destroy();
                            rotatedField[y * width + x] = null;
                            rotatedField[availableSlotPosition.y * width + availableSlotPosition.x] = currentCell;

                            ++availableSlotPosition.y;
                        }
                        else
                        {
                            ++availableSlotPosition.y;
                            
                            rotatedField[y * width + x] = null;
                            rotatedField[availableSlotPosition.y * width + availableSlotPosition.x] = currentCell;
                        }
                    }
                }
            }
        }
        
        public enum Direction
        {
            Up,
            Right,
            Down,
            Left
        }

        private List<Cell?> RotateField(Direction direction, List<Cell?> rotatableField, int width, int height)
        {
            if ((int) direction == 0)
            {
                return rotatableField;
            }
            
            List<Cell?> rotatedField = new List<Cell?>();
            for (int i = 0; i < width * height; ++i)
            {
                rotatedField.Add(null);
            }

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    rotatedField[(width - x - 1) * height + y] = rotatableField[y * width + x];
                }
            }

            return RotateField((Direction) ((int) direction - 1), rotatedField, height, width);
        }

        public void MoveCells(Direction direction)
        {
            List<Cell?> rotatedField = RotateField(direction, field, WIDTH, HEIGHT);
            if ((int) direction % 2 == 0)
            {
                MoveCellsUp(rotatedField, WIDTH, HEIGHT);
                field = RotateField((Direction) ((4 - (int) direction) % 4), rotatedField, WIDTH, HEIGHT);
            }
            else
            {
                MoveCellsUp(rotatedField, HEIGHT, WIDTH);
                field = RotateField((Direction) ((4 - (int) direction) % 4), rotatedField, HEIGHT, WIDTH);
            }

            for (int i = 0; i < WIDTH * HEIGHT; ++i)
            {
                field[i]?.SetCoordinates(GetSlotCoordinates(i % WIDTH, i / WIDTH));
            }

            RecalculateAvailableDirections();
        }

        public void RestartField()
        {
            for (int i = 0; i < WIDTH * HEIGHT; ++i)
            {
                field[i]?.Destroy();
                field[i] = null;
            }

            RecalculateAvailableDirections();
        }

        public bool CheckAvailabilityOfDirection(Direction direction)
        {
            return availableDirections[(int) direction];
        }

        public bool CheckGameOver()
        {
            bool result = true;

            for (int i = 0; i < availableDirections.Count; ++i)
            {
                if (availableDirections[i])
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
