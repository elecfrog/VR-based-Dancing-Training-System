using UnityEngine;

namespace Humanoid
{
    public class HumanoidJointHierarchy
    {
        public HumanoidJoint root;

        public HumanoidJointHierarchy(GameObject rootGameObject) {
            root = CreateJointTree(rootGameObject.transform);
        }

        private HumanoidJoint CreateJointTree(Transform rootTransform) {
            HumanoidJoint humanoidJoint = new HumanoidJoint(rootTransform.gameObject);
            foreach (Transform childTransform in rootTransform) {
                humanoidJoint.children.Add(CreateJointTree(childTransform));
            }

            return humanoidJoint;
        }
    }
}