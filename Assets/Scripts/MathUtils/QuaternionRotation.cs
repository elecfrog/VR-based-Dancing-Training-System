using UnityEngine;

namespace MathUtils
{
    public static class QuaternionRotation
    {
        static UnityEngine.Quaternion Sub(UnityEngine.Quaternion a, UnityEngine.Quaternion b) {
            return new UnityEngine.Quaternion(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }
    }
}