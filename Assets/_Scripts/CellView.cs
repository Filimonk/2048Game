using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class CellView : MonoBehaviour
    {
        private Cell cell;
        [SerializeField] private TextMeshProUGUI numberOfPoints;

        private int PowInt(int n, int a)
        {
            if (n == 0)
            {
                return 1;
            }

            int halfPowerValue = PowInt(n / 2, a);
            if (n % 2 == 1)
            {
                return halfPowerValue * halfPowerValue * a;
            }
            else
            {
                return halfPowerValue * halfPowerValue;
            }
        }
    
        public void Init(Cell cell)
        {
            this.cell = cell;
            
            numberOfPoints.text = PowInt(cell.GetValue(), 2).ToString();
            transform.position = cell.GetCoordinates();

            cell.OnDestroy += CellViewDestroy;
            cell.OnValueChanged += UpdateValue;
            cell.OnPositionChanged += UpdatePosition;
        }
        
        private void CellViewDestroy()
        {
            Destroy(gameObject);
        }
        
        private void OnDestroy()
        {
            cell.OnDestroy -= CellViewDestroy;
            cell.OnValueChanged -= UpdateValue;
            cell.OnPositionChanged -= UpdatePosition;
        }

        private void UpdateValue(int value)
        {
            numberOfPoints.text = PowInt(value, 2).ToString();
            // заменить цвет
        }

        private void UpdatePosition(Vector3 coordinates)
        {
            transform.position = coordinates;
            // было бы красиво добавить плавное передвижение
        }
    }
}
