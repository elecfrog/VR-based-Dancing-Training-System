using System;
using UnityEngine;

namespace Kinect
{
    [Serializable]
    public class KinectFilteredSkeletonRegistry
    {
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 headFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 neckFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 spineShoulderFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 spineMidFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 spineBaseFiltered;
        
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftShoulderFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftElbowFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftWristFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftHandFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftThumbFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftHandTipFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftHipFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftKneeFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftAnkleFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftFootFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 leftHandScreenSpaceFiltered;
        
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightShoulderFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightElbowFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightWristFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightHandFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightThumbFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightHandTipFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightHipFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightKneeFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightAnkleFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightFootFiltered;
        [SerializeField]
        public AdaptiveDoubleExponentialFilterVector3 rightHandScreenSpaceFiltered;  

        public KinectFilteredSkeletonRegistry()
        {
            headFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            neckFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            spineShoulderFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            spineMidFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            spineBaseFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            
            leftShoulderFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftElbowFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftWristFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftHandFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftThumbFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftHandTipFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftHipFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftKneeFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftAnkleFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftFootFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            leftHandScreenSpaceFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            
            rightShoulderFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightElbowFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightWristFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightHandFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightThumbFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightHandTipFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightHipFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightKneeFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightAnkleFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightFootFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
            rightHandScreenSpaceFiltered = new AdaptiveDoubleExponentialFilterVector3 ();
        }
    }
}