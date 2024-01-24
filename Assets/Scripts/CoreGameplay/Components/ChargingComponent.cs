using System;
using UnityEngine;
using Utils;

namespace CoreGameplay.Components
{
    public class ChargingComponent : MonoBehaviour
    {
        public event Action<float> OnCurrentSizeChangedEvent;
        
        [SerializeField] private Transform Body;
        [SerializeField] private float SizeChangeAddMultiplier;
        [SerializeField] private Vector3 SizeChangeMultiplier;
        
        public float CurrentCharge
        {
            get => _currentCharge;
            set
            {
                _currentCharge = value;
                OnCurrentSizeChanged();
            }
        }
        
        private float _currentCharge;
        
        public void Add(float value)
        {
            CurrentCharge += value * SizeChangeAddMultiplier;
        }
        
        private void OnCurrentSizeChanged()
        {
            Body.localScale = Body.localScale.ChangeFromMultipliers(SizeChangeMultiplier * CurrentCharge);
            OnCurrentSizeChangedEvent?.Invoke(CurrentCharge);
        }
    }
}