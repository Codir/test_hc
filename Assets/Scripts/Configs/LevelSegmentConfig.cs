using System;
using CoreGameplay;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class LevelSegmentConfig
    {
        public float Width;
        [Tooltip("If seed value is 0. Will be random")]
        public int Seed;
        [Range(0, 1)] public float Density;
        public LevelObjectView LevelObject;
    }
}