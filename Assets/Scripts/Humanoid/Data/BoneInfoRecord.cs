using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// Skeleton Data of an Humanoid Avatar
namespace Humanoid
{
    [RequireComponent(typeof(Animator))]
    public class BoneInfoRecord : SerializedMonoBehaviour
    {
        // animator component
        private Animator m_Animator;


        [SceneObjectsOnly]
        [InfoBox("Usually should be Hips Object below the object!")]
        public GameObject jointRootObject;

        // Take Hips position as the origin, make it to all 
        public Vector3 hipsPosition;

        // bone position in the world space, actually are offset position, cuz I pinned hips into zero.
        public Dictionary<HumanBodyBones, Vector3> bonePositions;

        // static joint angle pair registry, use to compute the joint angle to make comparison
        [InfoBox("Manual Joint Pairs to Support Angle Comparision")]
        public JointAnglePairRegistry jointAnglePairRegistry;

        // static joint length pair registry, use to compute the joint length,
        // so that it could support to solve the human joint position in the Kinect to make comparison
        [InfoBox("Manual Joint Pairs to Support Position Comparision")]
        public JointLengthRegistry boneLengthRegistry;

        private void Awake()
        {
            bonePositions = new Dictionary<HumanBodyBones, Vector3>();
            jointAnglePairRegistry = new JointAnglePairRegistry();
            boneLengthRegistry = new JointLengthRegistry();
            // jointHierarchy = new HumanoidJointHierarchy(jointRootObject);

            m_Animator = GetComponent<Animator>();
            if (!m_Animator)
                throw new Exception("Animator Component Must be Assigned");

            if (!m_Animator.isHuman)
                throw new Exception("Not a humanoid rig!");
        }

        private void Start()
        {
            UpdateBoneRecord();
            UpdateBoneLengths();
        }

        private void Update()
        {
            UpdateBoneRecord();
            UpdateJointAngles();
            // UpdateBoneLengths();
        }

        // compute bone lengths of each hips -> joint
        private void UpdateBoneLengths()
        {
            foreach (var path in boneLengthRegistry.lengthPaths)
            {
                for (int idx = 1; idx < path.pathNodes.Count; idx++)
                {
                    var prevNode = path.pathNodes[idx - 1];
                    var node = path.pathNodes[idx];
                    node.position = bonePositions[node.id];
                    node.length = (bonePositions[node.id] - bonePositions[prevNode.id]).magnitude;

                    // Write the updated node back to path.pathNodes
                    path.pathNodes[idx] = node;
                }
            }
        }
        
        private void UpdateBoneRecord()
        {
            foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                // LastBone is not an actual bone, but a value to mark the end of the enum
                if (bone != HumanBodyBones.LastBone)
                {
                    // get TRS with Unity API, it sucks
                    Transform boneTransform = m_Animator.GetBoneTransform(bone);

                    // record the world position of root bone, do re-target next.
                    RecordBonePosition(bone, boneTransform);
                }
            }
        }

        private void UpdateJointAngles()
        {
            for (int i = 0; i < jointAnglePairRegistry.jointAnglePairs.Count; i++)
            {
                var pair = jointAnglePairRegistry.jointAnglePairs[i];
                pair.angle = ComputeJointAngle(pair);
                jointAnglePairRegistry.jointAnglePairs[i] = pair;
            }
        }

        // compute angle, take start pos as the h
        private float ComputeJointAngle(JointAnglePair pair)
        {
            var startPos = GetRealPosition(pair.start);
            var end1Pos = GetRealPosition(pair.end1);
            var end2Pos = GetRealPosition(pair.end2);

            Vector3 startToEnd1 = end1Pos - startPos;
            Vector3 startToEnd2 = end2Pos - startPos;

            return Vector3.Angle(startToEnd1, startToEnd2);
        }

        // private float ComputeOffsetLength(JointLengthPath path)
        // {
        //
        //     Vector3 offsetPosition = GetOffsetPosition(path.end);
        //
        //     return MathF.Abs(offsetPosition.magnitude);
        // }


        public void RecordBonePosition(HumanBodyBones bone, Transform boneTransform)
        {
            if (bone == HumanBodyBones.Hips)
            {
                // record the world position of root bone, do re-target next.
                hipsPosition = boneTransform.position;
            }

            if (boneTransform != null)
            {
                // get the world position currently, then minus HipsPosition in the world space
                bonePositions[bone] = boneTransform.position - hipsPosition;
            }
        }

        public Vector3 GetRealPosition(Vector3 pos)
        {
            return pos + hipsPosition;
        }

        public Vector3 GetOffsetPosition(Vector3 pos)
        {
            return pos;
        }


        public Vector3 GetRealPosition(HumanBodyBones bone)
        {
            return bonePositions[bone] + hipsPosition;
        }

        public Vector3 GetOffsetPosition(HumanBodyBones bone)
        {
            return bonePositions[bone];
        }

        public float ComputeRelativeLength(HumanBodyBones start, HumanBodyBones end)
        {
            return Mathf.Abs((bonePositions[start] - bonePositions[end]).magnitude);
        }
    }
}