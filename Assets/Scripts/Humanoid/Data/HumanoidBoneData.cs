using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Humanoid
{
    [RequireComponent(typeof(Animator))]
public class HumanoidBoneData : SerializedMonoBehaviour
{
    // Animator component of the GameObject
    private Animator m_Animator;

    [InfoBox("public record contains all data for motion comparison")]
    public BoneInfoRecord boneInfoRecord;

    private Dictionary<string, LineRenderer> m_LineRenderers;
    
    private void Awake() {
        m_LineRenderers = new Dictionary<string, LineRenderer>();
    }

    private void Start() {
        m_Animator = GetComponent<Animator>();

        if (m_Animator.isHuman) {
            UpdateBoneInfos();
        } else {
            Debug.Log("Not a humanoid rig!");
        }
    }

    private void Update() {
        UpdateBoneInfos();
        
        // TODO apply a full registry here
        for (int i = 0; i < boneInfoRecord.jointAnglePairRegistry.jointAnglePairs.Count; i++)
        {
            var pair = boneInfoRecord.jointAnglePairRegistry.jointAnglePairs[i];
            pair.angle = ComputeJointAngleScalar(pair);
            boneInfoRecord.jointAnglePairRegistry.jointAnglePairs[i] = pair;
        }

    }


    float ComputeJointAngleScalar(JointAnglePair pair) {
        var startPos = boneInfoRecord.GetRealPosition(pair.start);
        var end1Pos = boneInfoRecord.GetRealPosition(pair.end1);
        var end2Pos = boneInfoRecord.GetRealPosition(pair.end2);

        Vector3 startToEnd1 = end1Pos - startPos;
        Vector3 startToEnd2 = end2Pos - startPos;

        return Vector3.Angle(startToEnd1, startToEnd2);
    }

    void DrawDebugArcs(HumanBodyBones start, HumanBodyBones end1, HumanBodyBones end2) {
        string key_1 = start.ToString() + end1.ToString();
        string key_2 = start.ToString() + end2.ToString();
        // Create a new empty game object
        GenerateLineRenderer(key_1, start, end1);
        GenerateLineRenderer(key_2, start, end2);

        string arcKey = start.ToString() + end1.ToString() + end2.ToString();
        if (!m_LineRenderers.ContainsKey(arcKey)) {
            GameObject arcObj = new GameObject(arcKey);
            LineRenderer arcLine = arcObj.AddComponent<LineRenderer>();

            arcLine.startWidth = 0.1f;
            arcLine.endWidth = 0.1f;

            m_LineRenderers.Add(arcKey, arcLine);
        }

        LineRenderer arc = m_LineRenderers[arcKey];
        UpdateArc(arc, boneInfoRecord.GetRealPosition(start), boneInfoRecord.GetRealPosition(end1), boneInfoRecord.GetRealPosition(end2));
    }

    void UpdateArc(LineRenderer lineRenderer, Vector3 start, Vector3 end1, Vector3 end2) {
        Vector3 startToEnd1 = end1 - start;
        Vector3 startToEnd2 = end2 - start;

        int segments = 20; // the number of line segments you want to use for the arc
        lineRenderer.positionCount = segments + 1;

        float angle = Vector3.Angle(startToEnd1, startToEnd2);
        Quaternion rotation = Quaternion.AngleAxis(angle / segments, Vector3.Cross(startToEnd1, startToEnd2));

        for (int i = 0; i <= segments; i++) {
            Vector3 segmentStart = rotation * startToEnd1;
            lineRenderer.SetPosition(i, start + segmentStart);
            startToEnd1 = segmentStart;
        }
    }

    private LineRenderer GenerateLineRenderer(string key, HumanBodyBones start, HumanBodyBones end) {
        LineRenderer line;
        if (!m_LineRenderers.ContainsKey(key)) {
            GameObject lineObj = new GameObject(key);
            line = lineObj.AddComponent<LineRenderer>();

            line.startWidth = 0.15f;
            line.endWidth = 0.1f;

            line.positionCount = 2;

            m_LineRenderers.Add(key, line);
        } else {
            line = m_LineRenderers[key];
        }

        line.SetPosition(0, boneInfoRecord.GetRealPosition(start));
        line.SetPosition(1, boneInfoRecord.GetRealPosition(end));

        return line;
    }

    private void UpdateBoneInfos() {
        foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones))) {
            // LastBone is not an actual bone, but a value to mark the end of the enum
            if (bone != HumanBodyBones.LastBone) {
                // get TRS with Unity API, it sucks
                Transform boneTransform = m_Animator.GetBoneTransform(bone);

                // record the world position of root bone, do retarget next.
                boneInfoRecord.RecordBonePosition(bone, boneTransform);
            }
        }
    }
}
}
