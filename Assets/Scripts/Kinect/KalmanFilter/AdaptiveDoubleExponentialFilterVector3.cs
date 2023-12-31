﻿using System;
using UnityEngine;

namespace Kinect
{
    [Serializable]
    public class AdaptiveDoubleExponentialFilterVector3
    {
        [SerializeField]
        private AdaptiveDoubleExponentialFilterFloat x;

        [SerializeField]
        private AdaptiveDoubleExponentialFilterFloat y;

        [SerializeField]
        private AdaptiveDoubleExponentialFilterFloat z;

        public Vector3 Value
        {
            get { return new Vector3(x.Value, y.Value, z.Value); }
            set { Update(value); }
        }

        public AdaptiveDoubleExponentialFilterVector3()
        {
            x = new AdaptiveDoubleExponentialFilterFloat();
            y = new AdaptiveDoubleExponentialFilterFloat();
            z = new AdaptiveDoubleExponentialFilterFloat();
        }

        private void Update(Vector3 v)
        {
            x.Value = v.x;
            y.Value = v.y;
            z.Value = v.z;
        }
    }
}