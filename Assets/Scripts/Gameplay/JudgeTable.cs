using System;
using System.Collections.Generic;
using System.Linq;
using Humanoid;
using Kinect;
using Microsoft.Azure.Kinect.BodyTracking;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Joint = UnityEngine.Joint;

[Serializable]
public struct Scores
{
    public float minScore;
    public float maxScore;
    public float avgScore;

    [HideInInspector]
    public int frameCount;
}

public struct AngleContent
{
    public float humanoidAngle;
    public float kinectAngle;
    public float differAngle;

    public AngleContent(float humanoid, float kinect, float differ)
    {
        humanoidAngle = humanoid;
        kinectAngle = kinect;
        differAngle = differ;
    }
}


public class JudgeTable : SerializedMonoBehaviour
{
    [Title("Reference Humanoid in Scene")]
    [SceneObjectsOnly]
    public BoneInfoRecord _humanoidData;

    [Title("Kinect Skeleton Data")]
    [SceneObjectsOnly]
    public KinectSkeletonAvatar _kinectData;

    [Title("Joint Angles Compare Table")]
    [DictionaryDrawerSettings(KeyLabel = "Pair Name", ValueLabel = "Angles")]
    [SerializeField]
    private Dictionary<string, AngleContent> _anglesComparison;

    public Dictionary<JointId, float> lengthScalars = new Dictionary<JointId, float>();

    public Dictionary<JointId, Vector3> kinectWorldPositions = new Dictionary<JointId, Vector3>();
    public Dictionary<HumanBodyBones, Vector3> jointsWorldPositionCopy = new Dictionary<HumanBodyBones, Vector3>();
    public Dictionary<JointId, Vector3> diff_posistions = new Dictionary<JointId, Vector3>();

    [SerializeField]
    public Scores _scores;

    private void Awake()
    {
        if (MatchingTable.KinectStr2Unity == null)
            throw new InvalidOperationException("Matching Table is not initialized");

        _anglesComparison = new Dictionary<string, AngleContent>();
        kinectWorldPositions = new Dictionary<JointId, Vector3>();
        diff_posistions = new Dictionary<JointId, Vector3>();
    }

    // In this update, I will compare the difference from animation clip and human skeleton
    private void Update()
    {
        // Compare Angles
        if (_kinectData.kinectAnglePairRegistryCopy != null && _humanoidData.jointAnglePairRegistry != null)
        {
            // angles
            var kinectAngles = _kinectData.kinectAnglePairRegistryCopy.kinectAnglePairs;
            var humanoidAngles = _humanoidData.jointAnglePairRegistry.jointAnglePairs;

            var kinectLengthPaths = _kinectData.kinectLengthPairRegistryCopy.lengthPaths;
            var humanoidLengthPaths = _humanoidData.boneLengthRegistry.lengthPaths;

            if (kinectAngles != null && humanoidAngles != null && kinectAngles.Count == humanoidAngles.Count &&
                kinectLengthPaths != null && humanoidLengthPaths != null && kinectLengthPaths.Count == humanoidLengthPaths.Count)
            {
                float frameScore = 0.0f;

                for (int i =0 ; i<  kinectLengthPaths.Count; ++i)
                {
                    for (int j = 1; j < kinectLengthPaths[i].pathNodes.Count; ++j)
                    {
                        // path.pathNodes[idx].id;
                        var kinectNode = kinectLengthPaths[i].pathNodes[j];
                        Debug.Log(kinectNode.id);
                        float scale = humanoidLengthPaths[i].pathNodes[j].length / kinectNode.length;
                        kinectNode.scaling = scale;
                        Vector3 direction = (kinectNode.position - kinectLengthPaths[i].pathNodes[j - 1].position).normalized;
                        kinectNode.position = scale * kinectNode.length * direction;

                        // Write the updated kinectNode back to kinectLengthPaths
                        kinectLengthPaths[i].pathNodes[j] = kinectNode;

                        if (j == 3)
                        {
                            // final leaf position
                            float diff = (kinectNode.position - _humanoidData.bonePositions[humanoidLengthPaths[i].pathNodes[j].id]).magnitude -1.0f;
                            if (diff < 0.5)
                            {
                                frameScore += 0.5f;
                            }
                        }
                    }
                }

                for (int idx = 0; idx < humanoidAngles.Count; ++idx)
                {
                    var humanoidAngle = humanoidAngles[idx].angle;
                    var kinectAngle = kinectAngles[idx].angle;

                    string keyName = $"{idx}: {kinectAngles[idx].start}-{kinectAngles[idx].end1}";

                    float diff = MathF.Abs(kinectAngle - humanoidAngle);
                    _anglesComparison[keyName] = new AngleContent(humanoidAngle, kinectAngle, diff);

                    float frameAngleScore = 0.0f;
                    if (diff < 10.0)
                    {
                        frameAngleScore = 1;
                    }
                    else if (diff > 10.0 && diff < 30.0)
                    {
                        frameAngleScore = 1 - (diff - 10.0f) / 20.0f;
                    }

                    if (kinectAngles[idx].start == JointId.WristRight
                        || kinectAngles[idx].start == JointId.WristLeft
                        || kinectAngles[idx].start == JointId.AnkleRight
                        || kinectAngles[idx].start == JointId.AnkleLeft)
                    {
                        frameAngleScore *= 0.5f;
                    }

                    frameScore += frameAngleScore;
                }
                // here, we map it the all score into [0, 100], where,
                // S_angle =
                // {
                //  0.5 * S_angle,  => if joint is wrist or ankle
                //  S_angle,        => otherwise
                // }
                frameScore = frameScore / (float)(humanoidAngles.Count) * 100.0f;

                if (frameScore < _scores.minScore)
                {
                    _scores.minScore = frameScore;
                }
                else if(frameScore > _scores.maxScore)
                {
                    _scores.maxScore = frameScore;
                }

                _scores.frameCount++;
                _scores.avgScore = (_scores.avgScore + frameScore) / (float)_scores.frameCount;
            }
        }


        // Compare Positions
        if (_kinectData.kinectLengthPairRegistryCopy != null && _humanoidData.boneLengthRegistry != null)
        {
            for (int idx = 0; idx < _kinectData.kinectLengthPairRegistryCopy.lengthPaths.Count; ++idx)
            {
                // kinect skeleton, form as joint -> hips
                var capturePair = _kinectData.kinectLengthPairRegistryCopy.lengthPaths[idx];

                // humanoid skeleton, form as joint -> hips
                // var avatarLength = _humanoidData.jointLengthRegistry.lengthPaths[idx].length;

                // lengthScalars[capturePair.end] = avatarLength / capturePair.length;
            }

            /*
             * bonesWorldPositionCopy
             */
            // update to offset position
            foreach (var kvp in _kinectData.bonesWorldPosition.ToList())
            {
                if (lengthScalars.ContainsKey(kvp.Key))
                {
                    kinectWorldPositions[kvp.Key] = kvp.Value - _kinectData.rootPosition;
                }
                else
                {
                    kinectWorldPositions.Remove(kvp.Key);
                }
            }

            jointsWorldPositionCopy = _humanoidData.bonePositions;

            foreach (var kvp in kinectWorldPositions)
            {
                var target = kvp.Key;
                if (MatchingTable.Kinect2Unity.ContainsKey(target))
                {
                    var avatarKey = MatchingTable.Kinect2Unity[target];
                    if (jointsWorldPositionCopy.ContainsKey(avatarKey))
                    {
                        var avatarValue = jointsWorldPositionCopy[avatarKey];
                        var diff = avatarValue - kvp.Value;
                        diff_posistions[target] = diff;
                    }
                }
            }

            // diff_posistions = 
        }


        // bonesWorldPositionCopy = kinectStickyAvatar.bonesWorldPosition;
    }
}