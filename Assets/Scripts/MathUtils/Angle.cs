using UnityEngine;

namespace MathUtils
{
    public static class Angle
    {
        public static float ComputeAngle(Vector3 lhs, Vector3 rhs) => Vector3.Angle(lhs, rhs);
        
        public static void UpdateArc(LineRenderer lineRenderer, Vector3 start, Vector3 end1, Vector3 end2)
        {
            Vector3 startToEnd1 = end1 - start;
            Vector3 startToEnd2 = end2 - start;
        
            int segments = 20; // the number of line segments you want to use for the arc
            lineRenderer.positionCount = segments + 1;
        
            float angle = Vector3.Angle(startToEnd1, startToEnd2);
            Quaternion rotation = Quaternion.AngleAxis(angle / segments, Vector3.Cross(startToEnd1, startToEnd2));
        
            for (int i = 0; i <= segments; i++)
            {
                Vector3 segmentStart = rotation * startToEnd1;
                lineRenderer.SetPosition(i, start + segmentStart);
                startToEnd1 = segmentStart;
            }
        }
        
    }
    

}