using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

namespace Kinect
{
    public static class ConfidenceLevelColor
    {
        public static Color GetSkeletonConfidenceColor(JointConfidenceLevel confidenceLevel)
        {
            return confidenceLevel switch
            {
                // change the value to meet your need
                JointConfidenceLevel.High => Color.green,
                JointConfidenceLevel.Medium => Color.yellow,
                JointConfidenceLevel.Low => Color.red,
                _ => Color.grey
            };
        }
    }
}