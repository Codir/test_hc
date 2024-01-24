using System;
using CoreGameplay;
using CoreGameplay.Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Configs
{
    [Serializable]
    public class LevelSegmentConfig
    {
        public float Width;

        [Tooltip("If seed value is 0. Will be random")]
        public int Seed;

        [Range(0, 1)] public float Density;
        [FormerlySerializedAs("LevelObject")] public BaseLevelObject BaseLevelObject;
    }
}