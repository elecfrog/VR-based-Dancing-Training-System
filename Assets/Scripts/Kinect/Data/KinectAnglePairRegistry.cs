using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;

namespace Kinect
{
    public class KinectAnglePair
    {
        public JointId start;
        public JointId end1;
        public JointId end2;
        public float angle;

        public KinectAnglePair(JointId _start, JointId _end1, JointId _end2) {
            start = _start;
            end1 = _end1;
            end2 = _end2;
            angle = 0.0f;
        }
    }
    
    public class KinectAnglePairRegistry
    {
        public List<KinectAnglePair> kinectAnglePairs;

        public KinectAnglePairRegistry() {
            kinectAnglePairs = new List<KinectAnglePair>();

            // 1. left shoulder
            kinectAnglePairs.Add(new KinectAnglePair(JointId.ShoulderLeft, JointId.ElbowLeft, JointId.Pelvis));
            // 2. left elbow
            kinectAnglePairs.Add(new KinectAnglePair(JointId.ElbowLeft, JointId.HandLeft, JointId.Pelvis));
            // 3. left wrist
            kinectAnglePairs.Add(new KinectAnglePair(JointId.WristLeft, JointId.ElbowLeft, JointId.Pelvis));
            // 4. right shoulder
            kinectAnglePairs.Add(new KinectAnglePair(JointId.ShoulderRight, JointId.ElbowRight, JointId.Pelvis));
            // 5. right elbow
            kinectAnglePairs.Add(new KinectAnglePair(JointId.ElbowRight, JointId.WristRight, JointId.Pelvis));
            // 6. right wrist
            kinectAnglePairs.Add(new KinectAnglePair(JointId.WristRight, JointId.ElbowRight, JointId.Pelvis));
            // 7. left upper leg
            kinectAnglePairs.Add(new KinectAnglePair(JointId.HipLeft, JointId.KneeLeft, JointId.Pelvis));
            // 8. left low leg
            kinectAnglePairs.Add(new KinectAnglePair(JointId.KneeLeft, JointId.AnkleLeft, JointId.Pelvis));
            // 9. left ankle
            kinectAnglePairs.Add(new KinectAnglePair(JointId.AnkleLeft, JointId.KneeLeft, JointId.Pelvis));
            // 10. Right upper leg
            kinectAnglePairs.Add(new KinectAnglePair(JointId.HipRight, JointId.KneeRight, JointId.Pelvis));
            // 11. Right low leg
            kinectAnglePairs.Add(new KinectAnglePair(JointId.KneeRight, JointId.AnkleRight, JointId.Pelvis));
            // 12. Right ankle
            kinectAnglePairs.Add(new KinectAnglePair(JointId.AnkleRight, JointId.KneeRight, JointId.Pelvis));
            // 13. Chest
            kinectAnglePairs.Add(new KinectAnglePair(JointId.SpineChest, JointId.Head, JointId.Pelvis));
            
            // CUSTOMIZED COMBINATIONS
            // 14. LeftHip - Pelvis - RightPelvis NO!
            // 14. KneeLeft - Pelvis - KneeRight YES!
            kinectAnglePairs.Add(new KinectAnglePair(JointId.Pelvis, JointId.KneeLeft, JointId.KneeRight));
            // 15. LeftShoulder - WristLeft - AnkleLeft
            kinectAnglePairs.Add(new KinectAnglePair(JointId.ShoulderLeft, JointId.WristLeft, JointId.AnkleLeft));
            // 16. RightShoulder - WristRight - AnkleRight
            kinectAnglePairs.Add(new KinectAnglePair(JointId.ShoulderRight, JointId.WristRight, JointId.AnkleRight));
        }
    }
}