using System.Collections.Generic;
using UnityEngine;

namespace Humanoid
{
    public class HumanoidJoint
    {
        public GameObject gameObject;
        public List<HumanoidJoint> children;

        public HumanoidJoint(GameObject gameObject) {
            this.gameObject = gameObject;
            children = new List<HumanoidJoint>();
        }
    }
}