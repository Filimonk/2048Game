using System;
using UnityEngine;

namespace _Scripts
{
    public class Cell
    {
        public event Action OnDestroy;
        public event Action<int> OnValueChanged;
        public event Action<Vector3> OnPositionChanged;

        private int value;
        private Vector3 coordinates;
    
        public Cell(int value, Vector3 coordinates)
        {
            this.value = value;
            this.coordinates = coordinates;
        }

        public void Destroy()
        {
            OnDestroy?.Invoke();
        }

        public int GetValue()
        {
            return value;
        }

        public void SetValue(int value)
        {
            this.value = value;
            OnValueChanged?.Invoke(this.value);
        }

        public Vector3 GetCoordinates()
        {
            return coordinates;
        }

        public void SetCoordinates(Vector3 coordinates)
        {
            this.coordinates = coordinates;
            OnPositionChanged?.Invoke(this.coordinates);
        }
    }
}
