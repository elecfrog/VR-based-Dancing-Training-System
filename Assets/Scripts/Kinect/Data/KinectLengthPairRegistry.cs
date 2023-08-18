using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

namespace Kinect
{
    [Serializable]
    public struct PathNode
    {
        public JointId id;
        public float length;
        public float scaling;
        public Vector3 position;

        public PathNode(JointId jointId)
        {
            id = jointId;
            length = 0.0f;
            scaling = 0.0f;
            position = new Vector3();
        }
    }

    public class KinectLengthPath
    {
        // public JointId start;
        public List<PathNode> pathNodes;
        
        public KinectLengthPath()
        {
            pathNodes = new List<PathNode>();
        }
    };


    public class KinectLengthPairRegistry
    {
        public List<KinectLengthPath> lengthPaths;

        public KinectLengthPairRegistry()
        {

            /*
             * Left Wrist Path
             */
            KinectLengthPath leftWristPath = new KinectLengthPath();
            leftWristPath.pathNodes.Add(new PathNode(JointId.Pelvis));
            leftWristPath.pathNodes.Add(new PathNode(JointId.ShoulderLeft));
            leftWristPath.pathNodes.Add(new PathNode(JointId.ElbowLeft));
            leftWristPath.pathNodes.Add(new PathNode(JointId.WristLeft));

            /*
             * Right Wrist Path
             */
            KinectLengthPath rightWristPath = new KinectLengthPath();
            rightWristPath.pathNodes.Add(new PathNode(JointId.Pelvis));
            rightWristPath.pathNodes.Add(new PathNode(JointId.ShoulderRight));
            rightWristPath.pathNodes.Add(new PathNode(JointId.ElbowRight));
            rightWristPath.pathNodes.Add(new PathNode(JointId.WristRight));

            /*
             * Left Ankle Path
             */
            KinectLengthPath leftAnklePath = new KinectLengthPath();
            leftAnklePath.pathNodes.Add(new PathNode(JointId.Pelvis));
            leftAnklePath.pathNodes.Add(new PathNode(JointId.HipLeft));
            leftAnklePath.pathNodes.Add(new PathNode(JointId.KneeLeft));
            leftAnklePath.pathNodes.Add(new PathNode(JointId.AnkleLeft));
            
            /*
             * Right Ankle Path
             */
            KinectLengthPath rightAnklePath = new KinectLengthPath();
            rightAnklePath.pathNodes.Add(new PathNode(JointId.Pelvis));
            rightAnklePath.pathNodes.Add(new PathNode(JointId.HipRight));
            rightAnklePath.pathNodes.Add(new PathNode(JointId.KneeRight));
            rightAnklePath.pathNodes.Add(new PathNode(JointId.AnkleRight));
            
            
            lengthPaths = new List<KinectLengthPath>
            {
                leftWristPath,
                rightWristPath,
                leftAnklePath,
                rightAnklePath
            };
        }
    }
}