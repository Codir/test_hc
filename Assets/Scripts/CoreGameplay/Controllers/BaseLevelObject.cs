using UnityEngine;

namespace CoreGameplay.Controllers
{
    public abstract class BaseLevelObject : MonoBehaviour
    {
        public string Name;
        public Vector3 Size = Vector3.one;
    }
}