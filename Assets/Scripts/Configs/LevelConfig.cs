using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "TestHCGame/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public LevelSegmentConfig[] Segments;
    }
}