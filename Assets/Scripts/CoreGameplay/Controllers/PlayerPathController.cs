using System;
using System.Collections.Generic;
using CoreGameplay.Components;
using UnityEngine;

namespace CoreGameplay.Controllers
{
    public class PlayerPathController : MonoBehaviour
    {
        [SerializeField] private ChargingComponent ChargingComponent;

        private float _currentSize;
        private readonly List<BaseLevelObject> _levelObjectsList = new();

        public Action OnPathFreeAction;

        public void Clear()
        {
            _levelObjectsList.Clear();
        }

        public void OnCurrentSizeChanged(float value)
        {
            ChargingComponent.CurrentCharge = value;
        }

        private void OnTriggerEnter(Collider other)
        {
            var obstacle = other.gameObject.GetComponentInParent<ObstacleController>();

            if (obstacle == null)
                return;

            _levelObjectsList.Add(obstacle);
        }

        private void OnTriggerExit(Collider other)
        {
            var obstacle = other.gameObject.GetComponentInParent<ObstacleController>();

            if (obstacle == null)
                return;

            RemoveObstacle(obstacle);
        }

        private void CheckListLenght()
        {
            if (_levelObjectsList.Count > 0)
                return;

            OnPathFreeAction?.Invoke();
        }

        public void OnRemoveObstacle(BaseLevelObject obstacle)
        {
            RemoveObstacle(obstacle);
        }

        private void RemoveObstacle(BaseLevelObject obstacle)
        {
            _levelObjectsList.Remove(obstacle);
            CheckListLenght();
        }
    }
}