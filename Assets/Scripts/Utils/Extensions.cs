using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        public static Vector3 ChangeFromMultipliers(this Vector3 value, Vector3 multipliers)
        {
            var newValue = Vector3.zero;
            newValue.x = multipliers.x is not (< float.Epsilon and > -float.Epsilon) ? multipliers.x : value.x;
            newValue.y = multipliers.y is not (< float.Epsilon and > -float.Epsilon) ? multipliers.y : value.y;
            newValue.z = multipliers.z is not (< float.Epsilon and > -float.Epsilon) ? multipliers.z : value.z;

            return newValue;
        }

        public static T GetRandomElement<T>(this T[] array)
        {
            if (array.Length <= 0) return default;

            var randomIndex = Random.Range(0, array.Length);

            return array[randomIndex];
        }
    }
}