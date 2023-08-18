using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Kinect;

// public struct BoneRotation

[RequireComponent(typeof(Animator))]
public class HumanoidAvatar : SerializedMonoBehaviour
{
    [Title("Avatar Animator")]
    [SceneObjectsOnly]
    private Animator unityAvatar;

    [SerializeField]
    private GameObject root;

    private Vector3 rootPos;

    [Title("Bones of Avatar")]
    private Dictionary<HumanBodyBones, Transform> m_BoneTransforms;

    [Title("Previous Rotation in Quaternions of Bones of an Avatar")]
    private Dictionary<HumanBodyBones, Quaternion> m_PreviousBoneRotations;

    [Title("Originial Rotation in Quaternions of Bones of an Avatar")]
    private Dictionary<HumanBodyBones, Quaternion> m_OriginalRotations;

    [SerializeField] public Kinect.KinectSkeletonAvatar kinectSkeletonAvatar;
    // [SerializeField] public KinectSkeleton kinectSkeleton;

    [Header("Update individual joints")]
    [SerializeField] private float _smoothDelta = 0.4f;

    [SerializeField] private bool _updateHead = true;
    [SerializeField] private bool _updateNeck = true;
    [SerializeField] private bool _updateChest = true;
    [SerializeField] private bool _updateSpine = true;
    [SerializeField] private bool _updateHips = true;
    [SerializeField] private bool _updateShoulderRight = true;
    [SerializeField] private bool _updateShoulderLeft = true;
    [SerializeField] private bool _updateElbowRight = true;
    [SerializeField] private bool _updateElbowLeft = true;
    [SerializeField] private bool _updateWristRight;
    [SerializeField] private bool _updateWristLeft;
    [SerializeField] private bool _updateHipRight = true;
    [SerializeField] private bool _updateHipLeft = true;
    [SerializeField] private bool _updateKneeRight = true;
    [SerializeField] private bool _updateKneeLeft = true;
    [SerializeField] private bool _updateAnkleRight = true;
    [SerializeField] private bool _updateAnkleLeft = true;

    private void Awake()
    {
        m_BoneTransforms = new Dictionary<HumanBodyBones, Transform>();
        m_PreviousBoneRotations = new Dictionary<HumanBodyBones, Quaternion>();
        m_OriginalRotations = new Dictionary<HumanBodyBones, Quaternion>();
        unityAvatar = GetComponent<Animator>();

        rootPos = root.transform.position;

        if (unityAvatar.isHuman)
        {
            foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                // LastBone is not an actual bone, but a value to mark the end of the enum
                if (bone != HumanBodyBones.LastBone)
                {
                    // get TRS with Unity API, it sucks
                    Transform boneTransform = unityAvatar.GetBoneTransform(bone);
                    if (boneTransform != null)
                    {
                        m_BoneTransforms[bone] = unityAvatar.GetBoneTransform(bone);

                        m_OriginalRotations[bone] = m_BoneTransforms[bone].rotation;
                        m_PreviousBoneRotations[bone] = m_BoneTransforms[bone].rotation;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Not a humanoid rig!");
        }
    }

    private void Start()
    {
        if (kinectSkeletonAvatar == null)
            throw new Exception("Kinect Skeleton not assigned!");
    }

    private void Update()
    {
        if (kinectSkeletonAvatar.bonesWorldPosition != null && kinectSkeletonAvatar.frameConfidencePick)
        {
            CalculateHumanoidRotations();

            // m_BoneTransforms[HumanBodyBones.Hips].position = -1.0f * kinectSkeleton.bonesWorldPosition[JointId.Pelvis];
            m_BoneTransforms[HumanBodyBones.Hips].rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }
    }

    private static Vector3 GetNormal(Vector3 j1, Vector3 j2, Vector3 j3) => Vector3.Cross(j2 - j1, j3 - j1).normalized;

    /// <summary>Updates the rotation of the current bone.</summary>
    /// <param name="newRotation">The new rotation.</param>
    /// <param name="smoothDelta">The motion smoothing factor (0.0 - 1.0)</param>
    private void UpdateRotation(HumanBodyBones humanBoneEnum, Quaternion newRotation, float smoothDelta)
    {
        smoothDelta = Mathf.Clamp01(smoothDelta);
        Quaternion quaternion = Quaternion.Lerp(m_PreviousBoneRotations[humanBoneEnum],
            (newRotation * m_OriginalRotations[humanBoneEnum]), smoothDelta);
        m_PreviousBoneRotations[humanBoneEnum] = quaternion;
        m_BoneTransforms[humanBoneEnum].rotation = quaternion;
    }

    private void UpdateJointRotation(JointId JointId, Quaternion newRotation, bool update)
    {
        if (!update || !MatchingTable.Kinect2Unity.ContainsKey(JointId) ||
            !m_BoneTransforms.ContainsKey(MatchingTable.Kinect2Unity[JointId]))
            return;
        var currentHumanBoneEnum = MatchingTable.Kinect2Unity[JointId];
        // Debug.Log(currentHumanBoneEnum);
        UpdateRotation(currentHumanBoneEnum, newRotation, _smoothDelta);
    }

    private static Vector3 GetDirection(Vector3 to, Vector3 from) => (to - from).normalized;


    private void CalculateHumanoidRotations()
    {
        /*
         * Position of all joints
         */
        var kinectPositions = kinectSkeletonAvatar.bonesWorldPosition;
        var posHead = kinectPositions[JointId.Head];
        var posNose = kinectPositions[JointId.Nose];
        var posNeck = kinectPositions[JointId.Neck];
        var posHips = kinectPositions[JointId.Pelvis];
        var posSpine2 = kinectPositions[JointId.SpineNavel];
        var posLeftShoulder = kinectPositions[JointId.ShoulderLeft];
        var posRightShoulder = kinectPositions[JointId.ShoulderRight];
        var posLeftElbow = kinectPositions[JointId.ElbowLeft];
        var posRightElbow = kinectPositions[JointId.ElbowRight];
        var posLeftWrist = kinectPositions[JointId.WristLeft];
        var posRightWrist = kinectPositions[JointId.WristRight];
        var posLeftHandEnd = kinectPositions[JointId.HandTipLeft];
        var posRightHandEnd = kinectPositions[JointId.HandTipRight];
        var posLeftHandThumb = kinectPositions[JointId.ThumbLeft];
        var posRightHandThumb = kinectPositions[JointId.ThumbRight];
        var posLeftUpperLeg = kinectPositions[JointId.HipLeft];
        var posRightUpperLeg = kinectPositions[JointId.HipRight];
        var posLeftLowerLeg = kinectPositions[JointId.KneeLeft];
        var posRightLowerLeg = kinectPositions[JointId.KneeRight];
        var posLeftAnkle = kinectPositions[JointId.AnkleLeft];
        var posRightAnkle = kinectPositions[JointId.AnkleRight];
        var posLeftFoot = kinectPositions[JointId.FootLeft];
        var posRightFoot = kinectPositions[JointId.FootRight];
        var posSolvedUpperChest = Vector3.Lerp(posLeftShoulder, posRightShoulder, 0.5f);


        /*
         * Body
         */
        Vector3 normRightBody = GetNormal(posHips, posSolvedUpperChest, posRightShoulder);
        Vector3 normLeftBody = (posSolvedUpperChest - posSpine2).normalized;
        UpdateJointRotation(JointId.SpineNavel,
            Quaternion.LookRotation(normLeftBody, normRightBody) * Quaternion.Euler(90f, 0.0f, 180f), _updateSpine);

        /*
         * Chest (Both Sides of Shoulders)
         */
        Vector3 dirLeftShoulder2RightShoulder = GetDirection(posRightShoulder, posLeftShoulder);
        UpdateJointRotation(JointId.SpineChest,
            Quaternion.LookRotation(dirLeftShoulder2RightShoulder, normRightBody) * Quaternion.Euler(90f, 0.0f, -90f),
            _updateChest);

        /*
         * Head & Face
         */
        Vector3 dirNeck2Head = GetDirection(posHead, posNeck);
        UpdateJointRotation(JointId.Neck,
            Quaternion.LookRotation(dirNeck2Head, normRightBody) * Quaternion.Euler(90f, 0.0f, 180f), _updateNeck);
        Vector3 normFace = GetNormal(posNeck, posHead, posNose);
        Vector3 dirNeck2Nose = GetDirection(posNose, posNeck);
        UpdateJointRotation(JointId.Head,
            Quaternion.LookRotation(dirNeck2Nose, normFace) * Quaternion.Euler(0.0f, 220f, 90f), _updateHead);

        /*
         * Right Arm
         */
        Vector3 normRightArm = GetNormal(posRightShoulder, posRightElbow, posRightWrist);
        Vector3 direction4 = GetDirection(posRightElbow, posRightShoulder);
        UpdateJointRotation(JointId.ShoulderRight,
            Quaternion.LookRotation(direction4, normRightArm) * Quaternion.Euler(180f, 90f, 0.0f),
            _updateShoulderRight);
        Vector3 dirElbow2Wrist = GetDirection(posRightWrist, posRightElbow);
        UpdateJointRotation(JointId.ElbowRight,
            Quaternion.LookRotation(dirElbow2Wrist, normRightArm) * Quaternion.Euler(180, 90, 0), _updateElbowRight);

        /*
         * Right Hand
         */
        Vector3 normRightHand = GetNormal(posRightHandThumb, posRightWrist, posRightHandEnd);
        Vector3 dirRightWrist2Hand = GetDirection(posRightHandEnd, posRightWrist);
        UpdateJointRotation(JointId.WristRight,
            Quaternion.LookRotation(dirRightWrist2Hand, normRightHand) * Quaternion.Euler(180f, 90f, 0.0f),
            _updateWristRight);

        /*
         * Left Arm
         */
        Vector3 normLeftArm = GetNormal(posLeftShoulder, posLeftElbow, posLeftWrist);
        Vector3 dirLeftShoulder2LeftElbow = GetDirection(posLeftElbow, posLeftShoulder);
        UpdateJointRotation(JointId.ShoulderLeft,
            Quaternion.LookRotation(dirLeftShoulder2LeftElbow, normLeftArm) * Quaternion.Euler(0.0f, -90f, 0.0f),
            _updateShoulderLeft);
        Vector3 dirLeftElbow2LeftWrist = GetDirection(posLeftWrist, posLeftElbow);
        UpdateJointRotation(JointId.ElbowLeft,
            Quaternion.LookRotation(dirLeftElbow2LeftWrist, normLeftArm) * Quaternion.Euler(0.0f, -90f, 0.0f),
            _updateElbowLeft);

        /*
         * Left Hand
         */
        Vector3 normLeftHand = GetNormal(posLeftHandThumb, posLeftWrist, posLeftHandEnd);
        Vector3 dirLeftWrist2Hand = GetDirection(posLeftHandEnd, posLeftWrist);
        UpdateJointRotation(JointId.WristLeft,
            Quaternion.LookRotation(dirLeftWrist2Hand, normLeftHand) * Quaternion.Euler(0.0f, -90f, 0.0f),
            _updateWristLeft);

        /*
         * Hips
         */
        Vector3 normSpine = GetNormal(posSpine2, posHips, posRightUpperLeg);
        Vector3 dirHips2Spine = GetDirection(posSpine2, posHips);
        UpdateJointRotation(JointId.Pelvis,
            Quaternion.LookRotation(dirHips2Spine, normSpine) * Quaternion.Euler(90f, 0.0f, 0.0f), _updateHips);


        /*
         * Right Leg
         */
        double lengthRightFoot = (posRightFoot - posRightAnkle).magnitude;
        double lengthRightLowerLeg = (posRightAnkle - posRightLowerLeg).magnitude;

        Vector3 normRightLeg = lengthRightFoot < lengthRightLowerLeg
            ? GetNormal(posRightFoot, posRightAnkle, posRightLowerLeg)
            : GetNormal(posRightUpperLeg, posRightLowerLeg, posRightAnkle);

        Vector3 dirRightUpperLeg2RightLowerLeg = GetDirection(posRightLowerLeg, posRightUpperLeg);
        UpdateJointRotation(JointId.HipRight,
            Quaternion.LookRotation(dirRightUpperLeg2RightLowerLeg, normRightLeg) *
            Quaternion.Euler(0.0f, 90.0f, -90.0f), _updateHipRight);
        Vector3 dirRightAnkle2RightLowerLeg = GetDirection(posRightAnkle, posRightLowerLeg);
        UpdateJointRotation(JointId.KneeRight,
            Quaternion.LookRotation(dirRightAnkle2RightLowerLeg, normRightLeg) * Quaternion.Euler(0.0f, 90.0f, -90.0f),
            _updateKneeRight);
        Vector3 dirRightAnkle2RightFoot = GetDirection(posRightFoot, posRightAnkle);
        UpdateJointRotation(JointId.AnkleRight,
            Quaternion.LookRotation(dirRightAnkle2RightFoot, normRightLeg) * Quaternion.Euler(0, 150, 90),
            _updateAnkleRight);


        /*
         * Left Leg
         */
        double lengthLeftFoot = (posLeftFoot - posLeftAnkle).magnitude;
        double lengthLeftLowerLeg = (posLeftAnkle - posLeftLowerLeg).magnitude;
        Vector3 normLeftLeg = lengthLeftFoot < lengthLeftLowerLeg
            ? GetNormal(posLeftFoot, posLeftAnkle, posLeftLowerLeg)
            : GetNormal(posLeftUpperLeg, posLeftLowerLeg, posLeftAnkle);

        Vector3 dirLeftUpperLeg2LeftLowerLeg = GetDirection(posLeftLowerLeg, posLeftUpperLeg);
        UpdateJointRotation(JointId.HipLeft,
            Quaternion.LookRotation(dirLeftUpperLeg2LeftLowerLeg, normLeftLeg) * Quaternion.Euler(0.0f, 90f, -90f),
            _updateHipLeft);
        Vector3 dirLeftAngle2LeftLowerLeg = GetDirection(posLeftAnkle, posLeftLowerLeg);
        UpdateJointRotation(JointId.KneeLeft,
            Quaternion.LookRotation(dirLeftAngle2LeftLowerLeg, normLeftLeg) * Quaternion.Euler(0.0f, 90f, -90f),
            _updateKneeLeft);
        Vector3 dirKneeLeft2LeftFoot = GetDirection(posLeftFoot, posLeftAnkle);
        UpdateJointRotation(JointId.AnkleLeft,
            Quaternion.LookRotation(dirKneeLeft2LeftFoot, normLeftLeg) * Quaternion.Euler(0.0f, 150f, 90f),
            _updateAnkleLeft);


        /*
         * Update the Translation of the Root Object
         */
        root.transform.position = rootPos + kinectSkeletonAvatar.RootPosition();
    }
}