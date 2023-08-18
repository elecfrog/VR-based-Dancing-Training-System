using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

namespace Humanoid
{
    [Serializable]
    public struct PathNode
    {
        public HumanBodyBones id;
        public float length;
        public Vector3 position;

        public PathNode(HumanBodyBones HumanBodyBones)
        {
            id = HumanBodyBones;
            length = 0.0f;
            position = new Vector3();
        }
    }

    public class JointLengthPath
    {
        public List<PathNode> pathNodes;

        public JointLengthPath()
        {
            pathNodes = new List<PathNode>();
        }
    };

    // static joint angle pair registry, use to compute the joint angle to make comparison{
    public class JointLengthRegistry
    {
        public List<JointLengthPath> lengthPaths;

        public JointLengthRegistry()
        {
            /*
             * Left Wrist Path
             */
            JointLengthPath leftWristPath = new JointLengthPath();
            leftWristPath.pathNodes.Add(new PathNode(HumanBodyBones.Hips));
            leftWristPath.pathNodes.Add(new PathNode(HumanBodyBones.LeftUpperArm));
            leftWristPath.pathNodes.Add(new PathNode(HumanBodyBones.LeftLowerArm));
            leftWristPath.pathNodes.Add(new PathNode(HumanBodyBones.LeftHand));

            /*
             * Right Wrist Path
             */
            JointLengthPath rightWristPath = new JointLengthPath();
            rightWristPath.pathNodes.Add(new PathNode(HumanBodyBones.Hips));
            rightWristPath.pathNodes.Add(new PathNode(HumanBodyBones.RightUpperArm));
            rightWristPath.pathNodes.Add(new PathNode(HumanBodyBones.RightLowerArm));
            rightWristPath.pathNodes.Add(new PathNode(HumanBodyBones.RightHand));

            /*
             * Left Ankle Path
             */
            JointLengthPath leftAnklePath = new JointLengthPath();
            leftAnklePath.pathNodes.Add(new PathNode(HumanBodyBones.Hips));
            leftAnklePath.pathNodes.Add(new PathNode(HumanBodyBones.LeftUpperLeg));
            leftAnklePath.pathNodes.Add(new PathNode(HumanBodyBones.LeftLowerLeg));
            leftAnklePath.pathNodes.Add(new PathNode(HumanBodyBones.LeftFoot));

            /*
             * Right Ankle Path
             */
            JointLengthPath rightAnklePath = new JointLengthPath();
            rightAnklePath.pathNodes.Add(new PathNode(HumanBodyBones.Hips));
            rightAnklePath.pathNodes.Add(new PathNode(HumanBodyBones.RightUpperLeg));
            rightAnklePath.pathNodes.Add(new PathNode(HumanBodyBones.RightLowerLeg));
            rightAnklePath.pathNodes.Add(new PathNode(HumanBodyBones.RightFoot));


            lengthPaths = new List<JointLengthPath>();
            lengthPaths.Add(leftWristPath);
            lengthPaths.Add(rightWristPath);
            lengthPaths.Add(leftAnklePath);
            lengthPaths.Add(rightAnklePath);
        }
    }
}