using System;
using System.Collections.Generic;
using UnityEngine;

namespace Humanoid
{ 
    [Serializable]
    public struct JointAnglePair
    {
        public HumanBodyBones start;
        public HumanBodyBones end1;
        public HumanBodyBones end2;
        public float angle;

        public JointAnglePair(HumanBodyBones _start, HumanBodyBones _end1, HumanBodyBones _end2) {
            start = _start;
            end1 = _end1;
            end2 = _end2;
            angle = 0.0f;
        }
    };
    
    // static joint angle pair registry, use to compute the joint angle to make comparison{
    public class JointAnglePairRegistry
    {
        public List<JointAnglePair> jointAnglePairs;

        public JointAnglePairRegistry() {
            jointAnglePairs = new List<JointAnglePair>();

            // 1. left shoulder
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.Hips));
            // 2. left elbow
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand, HumanBodyBones.Hips));
            // 3. left wrist
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.LeftHand, HumanBodyBones.LeftLowerArm, HumanBodyBones.Hips));
            // 4. right shoulder
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.Hips));
            // 5. right elbow
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand, HumanBodyBones.Hips));
            // 6. right wrist
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.RightHand, HumanBodyBones.RightLowerArm, HumanBodyBones.Hips));
            // 7. left upper leg
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, HumanBodyBones.Hips));
            // 8. left low leg
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot, HumanBodyBones.Hips));
            // 9. left ankle
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.LeftFoot, HumanBodyBones.LeftLowerLeg, HumanBodyBones.Hips));
            // 10. Right upper leg
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, HumanBodyBones.Hips));
            // 11. Right low leg
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot, HumanBodyBones.Hips));
            // 12. Right ankle
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.RightFoot, HumanBodyBones.RightLowerLeg, HumanBodyBones.Hips));
            // 13. Chest
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.Chest, HumanBodyBones.Head, HumanBodyBones.Hips));

            // CUSTOMIZED COMBINATIONS
            // 14. LeftHip - Hips - RightHips NO!
            // 14. LeftLowerLeg - Hips - RightLowerLeg YES!
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.Hips, HumanBodyBones.LeftLowerLeg, HumanBodyBones.RightLowerLeg));
            // 15. LeftShoulder - LeftHand - LeftFoot
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.LeftShoulder, HumanBodyBones.LeftHand, HumanBodyBones.LeftFoot));
            // 16. RightShoulder - RightHand - RightFoot
            jointAnglePairs.Add(new JointAnglePair(HumanBodyBones.RightShoulder, HumanBodyBones.RightHand, HumanBodyBones.RightFoot));
        }
    }
}