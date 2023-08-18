using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

namespace Kinect
{
/*
 * In my animation files, one clip humanoid usually has 13-50+ bones.
 * At the same time, the kinect depth camera has a fixed number of bones in 33 for use.
 * Meanwhile, names of bones in these two kits are not totally same.
 * Have to overcome this point, I have to write a dict to compare then collect right and necessary bones information.
 *
 * And, I am not so familiar with C#, it seems have such thing in C# like constexpr std::unordered_map in C++.
 * So I have to write as follows.
 *
 * One more thing, here we used the kinect device, it will have 33 bones in total, but the bones in animation clip may have or may dont have,
 * for capturing the data from user, we should to take kinect skeleton as the reference
 *
 * kinect: https://learn.microsoft.com/en-us/azure/kinect-dk/body-joints
 * unity: directly goes into the editor then open a configuration of any avatar 
 */
    internal static class MatchingTable
    {
        // Kinect joints that reflect to Unity joints
        public static readonly Dictionary<JointId, HumanBodyBones> Kinect2Unity = new Dictionary<JointId, HumanBodyBones>
        {
            { JointId.Pelvis, HumanBodyBones.Hips },
            { JointId.SpineNavel, HumanBodyBones.Spine },
            { JointId.SpineChest, HumanBodyBones.Chest },
            { JointId.Neck, HumanBodyBones.Neck },
            { JointId.ClavicleLeft, HumanBodyBones.LeftShoulder },
            { JointId.ShoulderLeft, HumanBodyBones.LeftUpperArm },
            { JointId.ElbowLeft, HumanBodyBones.LeftLowerArm },
            { JointId.WristLeft, HumanBodyBones.LeftHand },
            { JointId.HandTipLeft, HumanBodyBones.LeftMiddleDistal },
            { JointId.ThumbLeft, HumanBodyBones.LeftThumbDistal },
            { JointId.ClavicleRight, HumanBodyBones.RightShoulder },
            { JointId.ShoulderRight, HumanBodyBones.RightUpperArm },
            { JointId.ElbowRight, HumanBodyBones.RightLowerArm },
            { JointId.WristRight, HumanBodyBones.RightHand },
            { JointId.HandTipRight, HumanBodyBones.RightMiddleDistal },
            { JointId.ThumbRight, HumanBodyBones.RightThumbDistal },
            { JointId.HipLeft, HumanBodyBones.LeftUpperLeg },
            { JointId.KneeLeft, HumanBodyBones.LeftLowerLeg },
            { JointId.AnkleLeft, HumanBodyBones.LeftFoot },
            { JointId.FootLeft, HumanBodyBones.LeftToes },
            { JointId.HipRight, HumanBodyBones.RightUpperLeg },
            { JointId.KneeRight, HumanBodyBones.RightLowerLeg },
            { JointId.AnkleRight, HumanBodyBones.RightFoot },
            { JointId.FootRight, HumanBodyBones.RightToes },
            { JointId.Head, HumanBodyBones.Head },
        };

        // Kinect joints in string that reflect to Unity joints
        public static readonly Dictionary<string, HumanBodyBones> KinectStr2Unity = new Dictionary<string, HumanBodyBones>
        {
            { "Pelvis", HumanBodyBones.Hips },
            { "SpineNavel", HumanBodyBones.Spine },
            { "SpineChest", HumanBodyBones.Chest },
            { "Neck", HumanBodyBones.Neck },
            { "ClavicleLeft", HumanBodyBones.LeftShoulder },
            { "ShoulderLeft", HumanBodyBones.LeftUpperArm },
            { "ElbowLeft", HumanBodyBones.LeftLowerArm },
            { "WristLeft", HumanBodyBones.LeftHand },
            { "HandTipLeft", HumanBodyBones.LeftMiddleDistal },
            { "ThumbLeft", HumanBodyBones.LeftThumbDistal },
            { "ClavicleRight", HumanBodyBones.RightShoulder },
            { "ShoulderRight", HumanBodyBones.RightUpperArm },
            { "ElbowRight", HumanBodyBones.RightLowerArm },
            { "WristRight", HumanBodyBones.RightHand },
            { "HandTipRight", HumanBodyBones.RightMiddleDistal },
            { "ThumbRight", HumanBodyBones.RightThumbDistal },
            { "HipLeft", HumanBodyBones.LeftUpperLeg },
            { "KneeLeft", HumanBodyBones.LeftLowerLeg },
            { "AnkleLeft", HumanBodyBones.LeftFoot },
            { "FootLeft", HumanBodyBones.LeftToes },
            { "HipRight", HumanBodyBones.RightUpperLeg },
            { "KneeRight", HumanBodyBones.RightLowerLeg },
            { "AnkleRight", HumanBodyBones.RightFoot },
            { "FootRight", HumanBodyBones.RightToes },
            { "Head", HumanBodyBones.Head },

            // unity humanoid has no nose! A better practice is to compute a solving position!
            // { JointId.Nose, HumanBodyBones.Nose},
            { "EyeLeft", HumanBodyBones.LeftEye },
            // { JointId.EarLeft, HumanBodyBones.LeftEar },
            { "EyeRight", HumanBodyBones.RightEye },
            // { JointId.EarRight, HumanBodyBones.RightEar },
        };

        // Unity joints that reflect to Kinect joints in string
        public static readonly Dictionary<HumanBodyBones, string> Unity2KinectStr = new Dictionary<HumanBodyBones, string>
        {
            { HumanBodyBones.Hips, "Pelvis" },
            { HumanBodyBones.Spine, "SpineNavel" },
            { HumanBodyBones.Chest, "SpineChest" },
            { HumanBodyBones.Neck, "Neck" },
            { HumanBodyBones.LeftShoulder, "ClavicleLeft" },
            { HumanBodyBones.LeftUpperArm, "ShoulderLeft" },
            { HumanBodyBones.LeftLowerArm, "ElbowLeft" },
            { HumanBodyBones.LeftHand, "WristLeft" },
            { HumanBodyBones.LeftMiddleDistal, "HandTipLeft" },
            { HumanBodyBones.LeftThumbDistal, "ThumbLeft" },
            { HumanBodyBones.RightShoulder, "ClavicleRight" },
            { HumanBodyBones.RightUpperArm, "ShoulderRight" },
            { HumanBodyBones.RightLowerArm, "ElbowRight" },
            { HumanBodyBones.RightHand, "WristRight" },
            { HumanBodyBones.RightMiddleDistal, "HandTipRight" },
            { HumanBodyBones.RightThumbDistal, "ThumbRight" },
            { HumanBodyBones.LeftUpperLeg, "HipLeft" },
            { HumanBodyBones.LeftLowerLeg, "KneeLeft" },
            { HumanBodyBones.LeftFoot, "AnkleLeft" },
            { HumanBodyBones.LeftToes, "FootLeft" },
            { HumanBodyBones.RightUpperLeg, "HipRight" },
            { HumanBodyBones.RightLowerLeg, "KneeRight" },
            { HumanBodyBones.RightFoot, "AnkleRight" },
            { HumanBodyBones.RightToes, "FootRight" },
            { HumanBodyBones.Head, "Head" },
            { HumanBodyBones.LeftEye, "EyeLeft" },
            { HumanBodyBones.RightEye, "EyeRight" },
        };
        
        // Maps the human joints to their mirrored equivalents.
        public static readonly Dictionary<JointId, JointId> KinectFlipped = new Dictionary<JointId, JointId>()
        {
            { JointId.Pelvis, JointId.Pelvis },
            { JointId.SpineNavel, JointId.SpineNavel },
            { JointId.SpineChest, JointId.SpineChest },
            { JointId.Neck, JointId.Neck },
            { JointId.ClavicleLeft, JointId.ClavicleRight },
            { JointId.ShoulderLeft, JointId.ShoulderRight },
            { JointId.ElbowLeft, JointId.ElbowRight },
            { JointId.WristLeft, JointId.WristRight },
            { JointId.HandLeft, JointId.HandRight },
            { JointId.HandTipLeft, JointId.HandTipRight },
            { JointId.ThumbLeft, JointId.ThumbRight },
            { JointId.ClavicleRight, JointId.ClavicleLeft },
            { JointId.ShoulderRight, JointId.ShoulderLeft },
            { JointId.ElbowRight, JointId.ElbowLeft },
            { JointId.WristRight, JointId.WristLeft },
            { JointId.HandRight, JointId.HandLeft },
            { JointId.HandTipRight, JointId.HandTipLeft },
            { JointId.ThumbRight, JointId.ThumbLeft },
            { JointId.HipLeft, JointId.HipRight },
            { JointId.KneeLeft, JointId.KneeRight },
            { JointId.AnkleLeft, JointId.AnkleRight },
            { JointId.FootLeft, JointId.FootRight },
            { JointId.HipRight, JointId.HipLeft },
            { JointId.KneeRight, JointId.KneeLeft },
            { JointId.AnkleRight, JointId.AnkleLeft },
            { JointId.FootRight, JointId.FootLeft },
            { JointId.Head, JointId.Head },
            { JointId.Nose, JointId.Nose },
            { JointId.EyeLeft, JointId.EyeRight },
            { JointId.EarLeft, JointId.EarRight },
            { JointId.EyeRight, JointId.EyeLeft },
            { JointId.EarRight, JointId.EarLeft }
        };

        // Linked mainly Kinect joints.
        public static readonly Dictionary<JointId, JointId> LinkedJoints = new Dictionary<JointId, JointId>()
        {
            { JointId.ClavicleLeft, JointId.ShoulderLeft },
            { JointId.ShoulderLeft, JointId.ElbowLeft },
            { JointId.ElbowLeft, JointId.WristLeft },
            { JointId.ClavicleRight, JointId.ShoulderRight },
            { JointId.ShoulderRight, JointId.ElbowRight },
            { JointId.ElbowRight, JointId.WristRight },
            { JointId.HipLeft, JointId.KneeLeft },
            { JointId.KneeLeft, JointId.AnkleLeft },
            { JointId.AnkleLeft, JointId.FootLeft },
            { JointId.HipRight, JointId.KneeRight },
            { JointId.KneeRight, JointId.AnkleRight },
            { JointId.AnkleRight, JointId.FootRight }
        };

        // https://learn.microsoft.com/en-us/azure/kinect-dk/body-joints
        public static readonly Dictionary<JointId, JointId?> KinectJointHierarchy = new Dictionary<JointId, JointId?>
        {
            { JointId.Pelvis, null },
            { JointId.SpineNavel, JointId.Pelvis },
            { JointId.SpineChest, JointId.SpineNavel },
            { JointId.Neck, JointId.SpineChest },
            { JointId.ClavicleLeft, JointId.SpineChest },
            { JointId.ShoulderLeft, JointId.ClavicleLeft },
            { JointId.ElbowLeft, JointId.ShoulderLeft },
            { JointId.WristLeft, JointId.ElbowLeft },
            { JointId.HandLeft, JointId.WristLeft },
            { JointId.HandTipLeft, JointId.HandLeft },
            { JointId.ThumbLeft, JointId.WristLeft },
            { JointId.ClavicleRight, JointId.SpineChest },
            { JointId.ShoulderRight, JointId.ClavicleRight },
            { JointId.ElbowRight, JointId.ShoulderRight },
            { JointId.WristRight, JointId.ElbowRight },
            { JointId.HandRight, JointId.WristRight },
            { JointId.HandTipRight, JointId.HandRight },
            { JointId.ThumbRight, JointId.WristRight },
            { JointId.HipLeft, JointId.Pelvis },
            { JointId.KneeLeft, JointId.HipLeft },
            { JointId.AnkleLeft, JointId.KneeLeft },
            { JointId.FootLeft, JointId.AnkleLeft },
            { JointId.HipRight, JointId.Pelvis },
            { JointId.KneeRight, JointId.HipRight },
            { JointId.AnkleRight, JointId.KneeRight },
            { JointId.FootRight, JointId.AnkleRight },
            { JointId.Head, JointId.Neck },
            { JointId.Nose, JointId.Head },
            { JointId.EyeLeft, JointId.Head },
            { JointId.EarLeft, JointId.Head },
            { JointId.EyeRight, JointId.Head },
            { JointId.EarRight, JointId.Head }
        };

        // https://learn.microsoft.com/en-us/azure/kinect-dk/body-joints
        public static readonly Dictionary<int, int?> KinectJointHierarchyInt = new Dictionary<int, int?>
        {
            { (int)JointId.Pelvis, null },
            { (int)JointId.SpineNavel, (int)JointId.Pelvis },
            { (int)JointId.SpineChest, (int)JointId.SpineNavel },
            { (int)JointId.Neck, (int)JointId.SpineChest },
            { (int)JointId.ClavicleLeft, (int)JointId.SpineChest },
            { (int)JointId.ShoulderLeft, (int)JointId.ClavicleLeft },
            { (int)JointId.ElbowLeft, (int)JointId.ShoulderLeft },
            { (int)JointId.WristLeft, (int)JointId.ElbowLeft },
            { (int)JointId.HandLeft, (int)JointId.WristLeft },
            { (int)JointId.HandTipLeft, (int)JointId.HandLeft },
            { (int)JointId.ThumbLeft, (int)JointId.WristLeft },
            { (int)JointId.ClavicleRight, (int)JointId.SpineChest },
            { (int)JointId.ShoulderRight, (int)JointId.ClavicleRight },
            { (int)JointId.ElbowRight, (int)JointId.ShoulderRight },
            { (int)JointId.WristRight, (int)JointId.ElbowRight },
            { (int)JointId.HandRight, (int)JointId.WristRight },
            { (int)JointId.HandTipRight, (int)JointId.HandRight },
            { (int)JointId.ThumbRight, (int)JointId.WristRight },
            { (int)JointId.HipLeft, (int)JointId.Pelvis },
            { (int)JointId.KneeLeft, (int)JointId.HipLeft },
            { (int)JointId.AnkleLeft, (int)JointId.KneeLeft },
            { (int)JointId.FootLeft, (int)JointId.AnkleLeft },
            { (int)JointId.HipRight, (int)JointId.Pelvis },
            { (int)JointId.KneeRight, (int)JointId.HipRight },
            { (int)JointId.AnkleRight, (int)JointId.KneeRight },
            { (int)JointId.FootRight, (int)JointId.AnkleRight },
            { (int)JointId.Head, (int)JointId.Neck },
            { (int)JointId.Nose, (int)JointId.Head },
            { (int)JointId.EyeLeft, (int)JointId.Head },
            { (int)JointId.EarLeft, (int)JointId.Head },
            { (int)JointId.EyeRight, (int)JointId.Head },
            { (int)JointId.EarRight, (int)JointId.Head }
        };
    }
}