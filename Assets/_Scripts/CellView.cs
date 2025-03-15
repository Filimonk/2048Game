using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class CellView : MonoBehaviour
    {
        private Cell cell;
        [SerializeField] private TextMeshProUGUI numberOfPoints;
        
        private Image backgroundImage;
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;

        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
        }

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
            UpdateColor(cell.GetValue());

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
        
        private void UpdateColor(int value)
        {
            int maxValue = 10;
            float t = (float) (value - 1) / maxValue;
            Color newColor = Color.Lerp(startColor, endColor, t);

            backgroundImage.color = newColor;
        }

        private void UpdateValue(int value)
        {
            numberOfPoints.text = PowInt(value, 2).ToString();
            UpdateColor(value);
        }

        private void UpdatePosition(Vector3 coordinates)
        {
            transform.position = coordinates;
            // было бы красиво добавить плавное передвижение
        }
    }
}
