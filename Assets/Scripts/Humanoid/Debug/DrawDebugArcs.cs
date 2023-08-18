using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using MathUtils;

namespace Humanoid
{
    [RequireComponent(typeof(Animator))]
    public class DrawDebugArcs : SerializedMonoBehaviour
    {
        [InfoBox("Click it to enable debug arcs drawing")]
        public bool m_EnableDebugDraw = false;

        [SceneObjectsOnly]
        [InfoBox("public record contains all data for motion comparison")]
        public BoneInfoRecord boneInfoRecord;

        private Dictionary<string, LineRenderer> lineRenderers;

        private GameObject lineRenderersParent;

        [AssetsOnly]
        [SerializeField]
        private LineRenderer linePrefab;

        private void Awake()
        {
            lineRenderers = new Dictionary<string, LineRenderer>();
        }

        private void Update()
        {
            if (boneInfoRecord && m_EnableDebugDraw)
            {
                if (lineRenderersParent == null)
                {
                    lineRenderersParent = new GameObject("DebugArcs");
                    lineRenderersParent.transform.position = Vector3.zero;
                }

                var jointAnglePairs = boneInfoRecord.jointAnglePairRegistry.jointAnglePairs;

                foreach (var pair in jointAnglePairs)
                {
                    // DrawArc(pair.start, pair.end1, pair.end2);
                }
            }
            else if (!m_EnableDebugDraw && lineRenderersParent != null)
            {
                DestroyAllLineRenderers();
            }
        }


        private void DestroyAllLineRenderers()
        {
            Destroy(lineRenderersParent);
            lineRenderersParent = null;
            lineRenderers.Clear();
        }

        void DrawArc(HumanBodyBones start, HumanBodyBones end1, HumanBodyBones end2)
        {
            string key_1 = start.ToString() + end1.ToString();
            string key_2 = start.ToString() + end2.ToString();
            
            // Create a new empty game object
            GenerateLineRenderer(key_1, start, end1);
            GenerateLineRenderer(key_2, start, end2);

            string arcKey = start.ToString() + end1.ToString() + end2.ToString();
            if (!lineRenderers.ContainsKey(arcKey))
            {
                LineRenderer arcLine = Instantiate(linePrefab, lineRenderersParent.transform, true);
                lineRenderers.Add(arcKey, arcLine);
            }

            LineRenderer arc = lineRenderers[arcKey];
            Angle.UpdateArc(arc, boneInfoRecord.GetRealPosition(start), boneInfoRecord.GetRealPosition(end1),
                boneInfoRecord.GetRealPosition(end2));
        }


        private void GenerateLineRenderer(string key, HumanBodyBones start, HumanBodyBones end)
        {
            LineRenderer lineObj;
            if (!lineRenderers.ContainsKey(key))
            {
                lineObj = Instantiate(linePrefab, lineRenderersParent.transform, true);
                lineObj.positionCount = 2;

                lineRenderers.Add(key, lineObj);
            }
            else
            {
                lineObj = lineRenderers[key];
            }

            lineObj.SetPosition(0, boneInfoRecord.GetRealPosition(start));
            lineObj.SetPosition(1, boneInfoRecord.GetRealPosition(end));
        }
    }
}