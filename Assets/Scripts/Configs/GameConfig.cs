using Models;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TestHCGame/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public float PlayerMoveSpeed;
        public LevelConfig[] Levels;
    }
}