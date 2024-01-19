using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGameplay
{
    public class PlayerPathView : MonoBehaviour
    {
        [SerializeField] private Vector3 SizeChangeMultiplier;
        [SerializeField] private Transform Body;

        private float _currentSize;
        private readonly List<ObstacleView> _obstaclesList = new();

        private float CurrentSize
        {
            get => _currentSize;
            set
            {
                _currentSize = value;
                OnSizeChanged();
            }
        }

        public Action OnPathFreeAction;

        public void Clear()
        {
            _obstaclesList.Clear();
        }

        private void OnSizeChanged()
        {
            //Debug.Log($"SizeChangeMultiplier {SizeChangeMultiplier * CurrentSize}");
            Body.localScale = Body.localScale.ChangeFromMultipliers(SizeChangeMultiplier * CurrentSize);
        }

        public void OnCurrentSizeChanged(float value)
        {
            CurrentSize = value;
        }

        private void OnTriggerEnter(Collider other)
        {
            //var hittable = other.gameObject.GetComponent<IHittable>();
            var obstacle = other.gameObject.GetComponentInParent<ObstacleView>();

            if (obstacle == null)
                return;

            _obstaclesList.Add(obstacle);
            Debug.Log($"OnTriggerEnter _obstaclesList {_obstaclesList.Count}");
        }

        private void OnTriggerExit(Collider other)
        {
            var obstacle = other.gameObject.GetComponentInParent<ObstacleView>();

            if (obstacle == null)
                return;

            RemoveObstacle(obstacle);
        }

        private void CheckListLenght()
        {
            if (_obstaclesList.Count > 0)
                return;

            OnPathFreeAction?.Invoke();
        }

        public void OnRemoveObstacle(ObstacleView obstacle)
        {
            RemoveObstacle(obstacle);
        }

        private void RemoveObstacle(ObstacleView obstacle)
        {
            _obstaclesList.Remove(obstacle);
            Debug.Log($"OnRemoveObstacle _obstaclesList {_obstaclesList.Count}");
            CheckListLenght();
        }
    }
}