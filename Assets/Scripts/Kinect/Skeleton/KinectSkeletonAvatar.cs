using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Sirenix.OdinInspector;
using System;

namespace Kinect
{
    public class KinectSkeletonAvatar : SerializedMonoBehaviour
    {
        [SerializeField]
        [InfoBox("Real position of Root, and it is the offset value should be minus to predict the position")]
        public Vector3 rootPosition = new Vector3(4, 1, 0);

        [SerializeField]
        [InfoBox("Every Kinect Component should refer the controller")]
        private KinectController kinectController;

        [ListDrawerSettings(IsReadOnly = true)]
        [InfoBox("Kinect Skeleton Data in the Record, Update, using this!")]
        public List<KinectSkeletonRecord> skeletonRecords;

        public KinectFilteredSkeletonRegistry kinectFilteredSkeletonRegistry;

        [InfoBox("position in the world space")]
        public Dictionary<JointId, Vector3> bonesWorldPosition;

        [InfoBox("position in the world space no depend on parent nodes")]
        public Dictionary<JointId, Quaternion> bonesWorldRotation;

        [AssetsOnly]
        [SerializeField]
        private GameObject jointLinkagePrefab;

        [AssetsOnly]
        [SerializeField]
        private GameObject jointBallPrefab;

        const float timeSkeletonCanBeDeactiveBeforeDelete = 3f;
        const float maxDistanceToAssumeSamePersonReentry = 2;

        // the hips/pelvis position in the world space,
        // to keep hip in my own space always be Vector3.Zero, translate it with formula bone_position = ( bone_position - m_HipsPosition)
        [SerializeField]
        private Vector3 m_RootPosition;

        public KinectAnglePairRegistry kinectAnglePairRegistryCopy;
        public KinectLengthPairRegistry kinectLengthPairRegistryCopy;

        public bool recordAngles = false;
        public bool drawStickyMan = false;

        [InfoBox("use to flip right/left hand")]
        public bool flipMovements = false;

        public bool inLive = true;

        public bool frameConfidencePick = true;
        public int confidenceTor = 4;

        void Awake()
        {
            // init resources
            skeletonRecords = new List<KinectSkeletonRecord>();
            kinectFilteredSkeletonRegistry = new KinectFilteredSkeletonRegistry();

            // ensure the kinect controller at the first
            if (!kinectController)
            {
                throw new Exception("Requires a kinect controller");
            }
        }

        /*
            Update checks for any skeletons?
            Found a skeleton! Checks if we have a real skeleton whos name is bodyID?
                Yes: Update that skeletons joints
                No: Check if we have any recently deactivated skeletons that are close in position to this new one (maybe its the same?)
        */
        void Update()
        {
            if (!kinectController) return;

            if (inLive)
            {
                List<KinectSkeletonRecord> toDelete = new List<KinectSkeletonRecord>();
                // Queue the expired skeletons for deletion - user probably walked away
                foreach (KinectSkeletonRecord skeleton in skeletonRecords)
                {
                    if (Time.time > skeleton.lastUpdateTime + timeSkeletonCanBeDeactiveBeforeDelete)
                    {
                        Debug.Log("deleting skeleton " + skeleton.root.name);
                        toDelete.Add(skeleton);
                    }
                }

                // Do the actual delete
                foreach (KinectSkeletonRecord skeleton in toDelete)
                {
                    // destory internal objects

                    foreach (var kvp in skeleton.bodyLinks)
                    {
                        Destroy(kvp.Value);
                    }

                    skeleton.bodyLinks.Clear();
                    Destroy(skeleton.stickyManRoot);

                    Destroy(skeleton.root);
                    skeletonRecords.Remove(skeleton);
                }

                toDelete.Clear();

                lock (kinectController.m_bufferLock)
                {
                    // directly get skeleton information from kinect controller 
                    List<SkeletonInfo> capturedSkeletonList = kinectController.m_currentSkeletons;

                    // m_SkeletonCount = kinectController.m_currentSkeletons.Count;

                    foreach (SkeletonInfo sk in capturedSkeletonList)
                    {
                        uint bodyId = sk.id;

                        // Check if skeleton already exists with same ID
                        KinectSkeletonRecord existingKinectSkeleton =
                            skeletonRecords.FirstOrDefault(skeleton => skeleton.root?.name == bodyId.ToString());


                        // Found this skeleton
                        if (existingKinectSkeleton != null)
                        {
                            // Apply Skeleton Movement
                            ApplyJointDataToSkeleton(sk.skeleton, existingKinectSkeleton);

                            // Loop up from ReferenceStr Table
                            // TODO apply a full registry here
                            if (recordAngles)
                            {
                                for (int i = 0;
                                     i < skeletonRecords[0].kinectAnglePairRegistry.kinectAnglePairs.Count;
                                     i++)
                                {
                                    var pair = skeletonRecords[0].kinectAnglePairRegistry.kinectAnglePairs[i];
                                    Vector3 startToEnd1 =
                                        bonesWorldPosition[pair.end1] - bonesWorldPosition[pair.start];
                                    Vector3 startToEnd2 =
                                        bonesWorldPosition[pair.end2] - bonesWorldPosition[pair.start];

                                    pair.angle = MathUtils.Angle.ComputeAngle(startToEnd1, startToEnd2);

                                    skeletonRecords[0].kinectAnglePairRegistry.kinectAnglePairs[i] = pair;

                                    // DrawDebugArcs(pair.start, pair.end1, pair.end2);
                                }
                            }


                            if (drawStickyMan)
                            {
                                if (skeletonRecords[0].stickyManRoot == null)
                                {
                                    skeletonRecords[0].stickyManRoot = new GameObject("StickyManRoot");
                                    skeletonRecords[0].stickyManRoot.transform.position = Vector3.zero;
                                    skeletonRecords[0].stickyManRoot.transform.parent = transform;
                                }


                                if (skeletonRecords.Count > 0 && skeletonRecords[0].bodyLinks != null)
                                {
                                    foreach (var kvp in MatchingTable.LinkedJoints)
                                    {
                                        var lhs = kvp.Key;
                                        var rhs = kvp.Value;
                                        DrawJointLine(existingKinectSkeleton, lhs, rhs);
                                    }
                                }
                            }

                            kinectAnglePairRegistryCopy = skeletonRecords[0].kinectAnglePairRegistry;
                            kinectLengthPairRegistryCopy = skeletonRecords[0]._lengthPathRegistry;
                        }
                        else
                        {
                            // Unidentified skeleton
                            // Is there a recently disappeared one that was close to this new one? ie the same person?
                            foreach (KinectSkeletonRecord skeleton in skeletonRecords)
                            {
                                if (Time.time > skeleton.lastUpdateTime + .5f)
                                {
                                    if (Vector3.Distance(skeleton.lastPosition,
                                            new Vector3(sk.GetRawJoint(0).Position.X,
                                                sk.GetRawJoint(0).Position.Y,
                                                sk.GetRawJoint(0).Position.Z))
                                        < maxDistanceToAssumeSamePersonReentry)
                                    {
                                        skeleton.root.name = bodyId.ToString();
                                        skeleton.root.transform.localPosition = rootPosition;
                                        return;
                                    }
                                }
                            }

                            // Else it must really be a new person
                            CreateDebugSkeletons(bodyId.ToString());
                        }
                    }
                }
            }
        }

        // Apply Skeleton Movement
        void ApplyJointDataToSkeleton(Skeleton skeletonData, KinectSkeletonRecord realKinectSkeleton)
        {
            frameConfidencePick = true;
            int limit = 0;

            // apply joints movements
            for (JointId i = 0; i < JointId.Count; i++)
            {
                JointId jointID = flipMovements ? MatchingTable.KinectFlipped[i] : i;

                var joint = skeletonData.GetJoint(jointID);
                var pos = joint.Position;
                var rot = joint.Quaternion;
                var confidenceLevel = joint.ConfidenceLevel;
                // apply the scalar factor and flipping
                var v = new Vector3(pos.X, -pos.Y, pos.Z) * 0.001f;
                var r = new Quaternion(rot.X, rot.Y, rot.Z, rot.W);

                if (i == JointId.Pelvis)
                    m_RootPosition = v;

                realKinectSkeleton.joints[jointID].transform.localPosition = v - m_RootPosition;
                realKinectSkeleton.joints[jointID].transform.localRotation = r;

                var rendererComponent = realKinectSkeleton.joints[jointID].GetComponent<Renderer>();
                rendererComponent.material.color = ConfidenceLevelColor.GetSkeletonConfidenceColor(confidenceLevel);

                if (confidenceLevel == JointConfidenceLevel.Low || confidenceLevel == JointConfidenceLevel.None)
                {
                    limit++;
                }
            }

            if (limit >= confidenceTor)
                frameConfidencePick = false;


            realKinectSkeleton.lastUpdateTime = Time.time;
            realKinectSkeleton.lastPosition = new Vector3(skeletonData.GetJoint(0).Position.X,
                skeletonData.GetJoint(0).Position.Y, skeletonData.GetJoint(0).Position.Z);

            if (skeletonRecords.Count != 0)
            {
                bonesWorldPosition = skeletonRecords[0].GetBonePositions();
                bonesWorldRotation = skeletonRecords[0].GetBoneRotations();

                // TODO UPDATE FILTER VALUE HERE
                kinectFilteredSkeletonRegistry.headFiltered.Value = bonesWorldPosition[JointId.Head];
                bonesWorldPosition[JointId.Head] = kinectFilteredSkeletonRegistry.headFiltered.Value;

                kinectFilteredSkeletonRegistry.neckFiltered.Value = bonesWorldPosition[JointId.Neck];
                bonesWorldPosition[JointId.Neck] = kinectFilteredSkeletonRegistry.neckFiltered.Value;

                kinectFilteredSkeletonRegistry.spineShoulderFiltered.Value = bonesWorldPosition[JointId.SpineChest];
                bonesWorldPosition[JointId.SpineChest] = kinectFilteredSkeletonRegistry.spineShoulderFiltered.Value;

                kinectFilteredSkeletonRegistry.spineMidFiltered.Value = bonesWorldPosition[JointId.SpineNavel];
                bonesWorldPosition[JointId.SpineNavel] = kinectFilteredSkeletonRegistry.spineMidFiltered.Value;

                kinectFilteredSkeletonRegistry.spineBaseFiltered.Value = bonesWorldPosition[JointId.Pelvis];
                bonesWorldPosition[JointId.Pelvis] = kinectFilteredSkeletonRegistry.spineBaseFiltered.Value;

                kinectFilteredSkeletonRegistry.leftShoulderFiltered.Value = bonesWorldPosition[JointId.ShoulderLeft];
                bonesWorldPosition[JointId.ShoulderLeft] = kinectFilteredSkeletonRegistry.leftShoulderFiltered.Value;

                kinectFilteredSkeletonRegistry.leftElbowFiltered.Value = bonesWorldPosition[JointId.ElbowLeft];
                bonesWorldPosition[JointId.ElbowLeft] = kinectFilteredSkeletonRegistry.leftElbowFiltered.Value;

                kinectFilteredSkeletonRegistry.leftWristFiltered.Value = bonesWorldPosition[JointId.WristLeft];
                bonesWorldPosition[JointId.WristLeft] = kinectFilteredSkeletonRegistry.leftWristFiltered.Value;

                kinectFilteredSkeletonRegistry.leftHandFiltered.Value = bonesWorldPosition[JointId.HandLeft];
                bonesWorldPosition[JointId.HandLeft] = kinectFilteredSkeletonRegistry.leftHandFiltered.Value;

                kinectFilteredSkeletonRegistry.leftThumbFiltered.Value = bonesWorldPosition[JointId.ThumbLeft];
                bonesWorldPosition[JointId.ThumbLeft] = kinectFilteredSkeletonRegistry.leftThumbFiltered.Value;

                kinectFilteredSkeletonRegistry.leftHandTipFiltered.Value = bonesWorldPosition[JointId.HandTipLeft];
                bonesWorldPosition[JointId.HandTipLeft] = kinectFilteredSkeletonRegistry.leftHandTipFiltered.Value;

                kinectFilteredSkeletonRegistry.leftHipFiltered.Value = bonesWorldPosition[JointId.HipLeft];
                bonesWorldPosition[JointId.HipLeft] = kinectFilteredSkeletonRegistry.leftHipFiltered.Value;

                kinectFilteredSkeletonRegistry.leftKneeFiltered.Value = bonesWorldPosition[JointId.KneeLeft];
                bonesWorldPosition[JointId.KneeLeft] = kinectFilteredSkeletonRegistry.leftKneeFiltered.Value;

                kinectFilteredSkeletonRegistry.leftAnkleFiltered.Value = bonesWorldPosition[JointId.AnkleLeft];
                bonesWorldPosition[JointId.AnkleLeft] = kinectFilteredSkeletonRegistry.leftAnkleFiltered.Value;

                kinectFilteredSkeletonRegistry.leftFootFiltered.Value = bonesWorldPosition[JointId.FootLeft];
                bonesWorldPosition[JointId.FootLeft] = kinectFilteredSkeletonRegistry.leftFootFiltered.Value;

                kinectFilteredSkeletonRegistry.leftHandScreenSpaceFiltered.Value = bonesWorldPosition[JointId.HandLeft];
                bonesWorldPosition[JointId.HandLeft] = kinectFilteredSkeletonRegistry.leftHandScreenSpaceFiltered.Value;

                kinectFilteredSkeletonRegistry.rightElbowFiltered.Value = bonesWorldPosition[JointId.ElbowRight];
                bonesWorldPosition[JointId.ElbowRight] = kinectFilteredSkeletonRegistry.rightElbowFiltered.Value;

                kinectFilteredSkeletonRegistry.rightWristFiltered.Value = bonesWorldPosition[JointId.WristRight];
                bonesWorldPosition[JointId.WristRight] = kinectFilteredSkeletonRegistry.rightWristFiltered.Value;

                kinectFilteredSkeletonRegistry.rightHandFiltered.Value = bonesWorldPosition[JointId.HandRight];
                bonesWorldPosition[JointId.HandRight] = kinectFilteredSkeletonRegistry.rightHandFiltered.Value;

                kinectFilteredSkeletonRegistry.rightThumbFiltered.Value = bonesWorldPosition[JointId.ThumbRight];
                bonesWorldPosition[JointId.ThumbRight] = kinectFilteredSkeletonRegistry.rightThumbFiltered.Value;

                kinectFilteredSkeletonRegistry.rightHandTipFiltered.Value = bonesWorldPosition[JointId.HandTipRight];
                bonesWorldPosition[JointId.HandTipRight] = kinectFilteredSkeletonRegistry.rightHandTipFiltered.Value;

                kinectFilteredSkeletonRegistry.rightHipFiltered.Value = bonesWorldPosition[JointId.HipRight];
                bonesWorldPosition[JointId.HipRight] = kinectFilteredSkeletonRegistry.rightHipFiltered.Value;

                kinectFilteredSkeletonRegistry.rightKneeFiltered.Value = bonesWorldPosition[JointId.KneeRight];
                bonesWorldPosition[JointId.KneeRight] = kinectFilteredSkeletonRegistry.rightKneeFiltered.Value;

                kinectFilteredSkeletonRegistry.rightAnkleFiltered.Value = bonesWorldPosition[JointId.AnkleRight];
                bonesWorldPosition[JointId.AnkleRight] = kinectFilteredSkeletonRegistry.rightAnkleFiltered.Value;

                kinectFilteredSkeletonRegistry.rightFootFiltered.Value = bonesWorldPosition[JointId.FootRight];
                bonesWorldPosition[JointId.FootRight] = kinectFilteredSkeletonRegistry.rightFootFiltered.Value;

                kinectFilteredSkeletonRegistry.rightHandScreenSpaceFiltered.Value =
                    bonesWorldPosition[JointId.HandRight];
                bonesWorldPosition[JointId.HandRight] =
                    kinectFilteredSkeletonRegistry.rightHandScreenSpaceFiltered.Value;


                kinectFilteredSkeletonRegistry.rightShoulderFiltered.Value = bonesWorldPosition[JointId.ShoulderRight];
                bonesWorldPosition[JointId.ShoulderRight] = kinectFilteredSkeletonRegistry.rightShoulderFiltered.Value;

                skeletonRecords[0].ComputeBonePath(bonesWorldPosition);
            }

            // Debug.Log("Hips: " + m_HipsPosition);
            realKinectSkeleton.root.transform.position = new Vector3(4, 1, 0) + m_RootPosition;
        }

        void CreateDebugSkeletons(string rootName)
        {
            KinectSkeletonRecord newKinectSkeleton = new KinectSkeletonRecord();
            Dictionary<JointId, GameObject> joints = new Dictionary<JointId, GameObject>(); // [(int)JointId.Count];

            GameObject skeletonRoot = new GameObject(rootName);
            // skeletonRoot.transform.position = new Vector3();        
            for (JointId currJointId = 0; currJointId < JointId.Count; currJointId++)
            {
                // using a prefab
                var jointObject = Instantiate(jointBallPrefab, skeletonRoot.transform);
                jointObject.transform.localPosition = new Vector3(0, 0, 0);

                // setup name of the joint
                var jointName = Enum.ToObject(typeof(JointId), (int)currJointId).ToString();
                jointObject.name = jointName;

                // add this object into the dictionary
                joints.Add(currJointId, jointObject);
            }

            newKinectSkeleton.root = skeletonRoot;
            newKinectSkeleton.joints = joints;
            newKinectSkeleton.lastUpdateTime = Time.time;

            skeletonRecords.Add(newKinectSkeleton);
            print("created a skeleton " + skeletonRoot.name);


            if (skeletonRecords.Count != 0)
            {
                bonesWorldPosition = skeletonRecords[0].GetBonePositions();
                bonesWorldRotation = skeletonRecords[0].GetBoneRotations();

                //     
                // foreach (var pair in skeletonRecords[0].KinectLengthPairRegistry.kinectLengthPairs)
                // {
                //     pair.ComputeJointLength
                // }
            }

            if (skeletonRecords[0].bodyLinks == null)
            {
                skeletonRecords[0].bodyLinks = new Dictionary<string, GameObject>();
            }
        }

        void DrawDebugArcs(JointId start, JointId end1, JointId end2)
        {
            string key_1 = start.ToString() + end1;
            string key_2 = start.ToString() + end2;

            // Create a new empty game object
            if (!skeletonRecords[0].lineRenderers.ContainsKey(key_1))
            {
                GameObject lineObj = new GameObject(key_1);
                // lineRenderers
                // Add a LineRenderer to it
                LineRenderer line = lineObj.AddComponent<LineRenderer>();

                // Set the line's width
                line.startWidth = 0.0f;
                line.endWidth = 0.025f;

                // Set the number of points to 2
                line.positionCount = 2;

                line.SetPosition(0, bonesWorldPosition[start]);
                line.SetPosition(1, bonesWorldPosition[end1]);
                skeletonRecords[0].lineRenderers.Add(key_1, line);
            }
            else
            {
                var line = skeletonRecords[0].lineRenderers[key_1];
                line.SetPosition(0, bonesWorldPosition[start]);
                line.SetPosition(1, bonesWorldPosition[end1]);
            }

            if (!skeletonRecords[0].lineRenderers.ContainsKey(key_2))
            {
                GameObject lineObj = new GameObject(key_2);
                // lineRenderers
                // Add a LineRenderer to it
                LineRenderer line = lineObj.AddComponent<LineRenderer>();

                // Set the line's width
                line.startWidth = 0.0f;
                line.endWidth = 0.025f;

                // Set the number of points to 2
                line.positionCount = 2;

                line.SetPosition(0, bonesWorldPosition[start]);
                line.SetPosition(1, bonesWorldPosition[end2]);
                skeletonRecords[0].lineRenderers.Add(key_2, line);
            }
            else
            {
                var line = skeletonRecords[0].lineRenderers[key_2];
                line.SetPosition(0, bonesWorldPosition[start]);
                line.SetPosition(1, bonesWorldPosition[end2]);
            }


            string arcKey = start + end1.ToString() + end2;
            if (!skeletonRecords[0].lineRenderers.ContainsKey(arcKey))
            {
                GameObject arcObj = new GameObject(arcKey);
                LineRenderer arcLine = arcObj.AddComponent<LineRenderer>();

                arcLine.startWidth = 0.025f;
                arcLine.startWidth = 0.025f;

                skeletonRecords[0].lineRenderers.Add(arcKey, arcLine);
            }

            LineRenderer arc = skeletonRecords[0].lineRenderers[arcKey];
            MathUtils.Angle.UpdateArc(arc, bonesWorldPosition[start], bonesWorldPosition[end1],
                bonesWorldPosition[end2]);
        }

        void DrawJointLine(KinectSkeletonRecord record, JointId lhs, JointId rhs)
        {
            string keyStr = lhs.ToString() + rhs;

            // Get the positions
            Vector3 startPos = bonesWorldPosition[lhs];
            Vector3 endPos = bonesWorldPosition[rhs];

            record.joints[lhs].transform.position = startPos;
            record.joints[rhs].transform.position = endPos;

            // Create a new empty game object
            GameObject cylinderObj;
            if (!skeletonRecords[0].bodyLinks.ContainsKey(keyStr))
            {
                // Instantiate the cylinder prefab
                cylinderObj = Instantiate(jointLinkagePrefab, skeletonRecords[0].stickyManRoot.transform, true);

                // Scale and position the cylinder
                PositionCylinder(cylinderObj.transform, startPos, endPos);

                // Add to the dictionary
                skeletonRecords[0].bodyLinks.Add(keyStr, cylinderObj);
            }
            else
            {
                // Get existing cylinder
                cylinderObj = skeletonRecords[0].bodyLinks[keyStr];

                // Update the position and scale
                PositionCylinder(cylinderObj.transform, startPos, endPos);
            }
        }

        void PositionCylinder(Transform cylinder, Vector3 start, Vector3 end)
        {
            Vector3 offset = end - start;
            Vector3 position = start + offset / 2.0f;
            cylinder.position = position;
            cylinder.localScale =
                new Vector3(0.025f, offset.magnitude / 2.0f,
                    0.025f); // Assuming the cylinder's height is along the Y axis
            cylinder.rotation =
                Quaternion.FromToRotation(Vector3.up, offset); // Assuming the cylinder's height is along the Y axis
        }


        public Vector3 RootPosition() => m_RootPosition;
    }
}