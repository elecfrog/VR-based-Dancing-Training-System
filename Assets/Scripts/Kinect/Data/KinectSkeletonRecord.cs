using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

namespace Kinect
{
    [Serializable]
    public class KinectSkeletonRecord
    {
        public GameObject root;

        public float lastUpdateTime;
        public Vector3 lastPosition;

        public Dictionary<JointId, GameObject> joints = new Dictionary<JointId, GameObject>();

        public KinectAnglePairRegistry kinectAnglePairRegistry = new KinectAnglePairRegistry();

        public KinectLengthPairRegistry _lengthPathRegistry;

        // render objects
        public GameObject stickyManRoot;

        // draw the debug lines
        public Dictionary<string, LineRenderer> lineRenderers;

        // draw the links
        public Dictionary<string, GameObject> bodyLinks;

        public KinectSkeletonRecord()
        {
            stickyManRoot = null;
            // init other resources
            lineRenderers = new Dictionary<string, LineRenderer>();
            bodyLinks = new Dictionary<string, GameObject>();
            _lengthPathRegistry = new KinectLengthPairRegistry();
        }

        // world position
        public Dictionary<JointId, Vector3> GetBonePositions()
        {
            var ret = new Dictionary<JointId, Vector3>();
            foreach (var c in joints)
            {
                ret.Add(c.Key, c.Value.transform.position);
            }

            return ret;
        }

        public Dictionary<JointId, Vector3> GetRotations()
        {
            var ret = new Dictionary<JointId, Vector3>();
            foreach (var c in joints)
            {
                ret.Add(c.Key, GetRotationInEuler(c.Value));
            }

            return ret;
        }

        public Dictionary<JointId, Quaternion> GetBoneRotations()
        {
            var ret = new Dictionary<JointId, Quaternion>();
            foreach (var c in joints)
            {
                ret.Add(c.Key, GetBoneRotationQuaternion(c.Value));
            }

            return ret;
        }

        public Vector3 GetChildrenPosition(GameObject obj)
        {
            return obj.transform.position;
        }


        public Vector3 GetRotationInEuler(GameObject obj)
        {
            Vector3 eulerAngles = obj.transform.localRotation.eulerAngles;
            return eulerAngles;
        }

        public Quaternion GetBoneRotationQuaternion(GameObject obj)
        {
            var rotation = obj.transform.rotation;
            Quaternion retargetRotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
            return retargetRotation;
        }

        public void ComputeBonePath(Dictionary<JointId, Vector3> worldPositions)
        {
            for (int i = 0; i < _lengthPathRegistry.lengthPaths.Count; i++)
            {
                var path = _lengthPathRegistry.lengthPaths[i];

                for (int idx = 1; idx < path.pathNodes.Count; idx++)
                {
                    var prevNode = path.pathNodes[idx - 1];
                    var node = path.pathNodes[idx];

                    node.position = worldPositions[node.id];
                    node.length = (worldPositions[node.id] - worldPositions[prevNode.id]).magnitude;

                    // Write the updated node back to path.pathNodes
                    path.pathNodes[idx] = node;

                    // Debug.Log($"Length : {node.length.ToString()}");
                }
            }
        }
    }
}