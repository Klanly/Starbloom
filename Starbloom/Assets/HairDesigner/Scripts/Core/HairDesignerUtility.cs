using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [System.Serializable]
        public class TPoseUtility
        {
            public bool m_inititialized = false;
            public Vector3[] m_initPos;
            public Quaternion[] m_initRot;
            public Vector3[] m_initScale;
            public Vector3 m_rootInitPos;
            public Quaternion m_rootInitRot;
            public Transform m_rootParent;
            public int m_rootBoneID;
            public SkinnedMeshRenderer m_smr;


            public void InitTPose(SkinnedMeshRenderer smr)
            {
                if (m_inititialized)
                    return;

                m_smr = smr;

                m_rootBoneID = -1;
                int parentCount = int.MaxValue;
                for (int i = 0; i < m_smr.bones.Length; ++i)
                {
                    //bool isRoot = true;
                    Transform[] parentLst = m_smr.bones[i].GetComponentsInParent<Transform>(true);
                    if (parentLst.Length < parentCount)
                    {
                        m_rootBoneID = i;
                        parentCount = parentLst.Length;
                    }
                }

                //Find the root transform of the hierarchy
                m_rootParent = m_smr.bones[m_rootBoneID].parent;
                bool isParentOfAllBones = false;
                while (m_rootParent.parent != null && !isParentOfAllBones)
                {
                    isParentOfAllBones = true;
                    Transform[] ChildrenLst = m_rootParent.GetComponentsInChildren<Transform>(true);
                    for (int i = 0; i < m_smr.bones.Length; ++i)
                    {
                        if (!ChildrenLst.Contains(m_smr.bones[i]))
                        {
                            isParentOfAllBones = false;
                            m_rootParent = m_rootParent.parent;
                            break;
                        }
                    }
                    /*
                    if (isParentOfAllBones && !ChildrenLst.Contains(m_smr.transform))
                    {
                        isParentOfAllBones = false;
                        m_rootParent = m_rootParent.parent;
                    }*/
                }




                m_initPos = new Vector3[m_smr.bones.Length];
                m_initRot = new Quaternion[m_smr.bones.Length];
                m_initScale = new Vector3[m_smr.bones.Length];
                //m_rootInitRot = m_smr.bones[m_rootBoneID].parent.localRotation;
                m_rootInitRot = m_rootParent.localRotation;
                m_rootInitPos = m_rootParent.localPosition;

                for (int i = 0; i < m_smr.bones.Length; ++i)
                {
                    m_initPos[i] = m_smr.bones[i].localPosition;
                    m_initRot[i] = m_smr.bones[i].localRotation;
                    m_initScale[i] = m_smr.bones[i].localScale;
                }

                m_inititialized = true;
            }


            public void RevertTpose()
            {
                if (!m_inititialized)
                    return;

                for (int i = 0; i < m_smr.bones.Length; ++i)
                {
                    m_smr.bones[i].localPosition = m_initPos[i];
                    m_smr.bones[i].localRotation = m_initRot[i];
                    m_smr.bones[i].localScale = m_initScale[i];
                    //Debug.Log(m_smr.bones[i].name);
                }

                m_rootParent.localRotation = m_rootInitRot;
                m_rootParent.localPosition = m_rootInitPos;
            }



            public void ApplyTPose(SkinnedMeshRenderer smr, bool unityMode )
            {
                if (!m_inititialized)
                    InitTPose(smr);

                if (unityMode)
                {
                    ReflectionRestoreToBindPose();
                    return;
                }
                
                List<Matrix4x4> bindposes = new List<Matrix4x4>();
#if UNITY_5_6_OR_NEWER
                m_smr.sharedMesh.GetBindposes(bindposes);
#else
                bindposes = m_smr.sharedMesh.bindposes.ToList();
#endif

                Matrix4x4 m = bindposes[m_rootBoneID].inverse;
                Vector3 rootPosition = (m.MultiplyPoint(Vector3.zero));
                rootPosition.x *= m_smr.bones[m_rootBoneID].lossyScale.x;
                rootPosition.y *= m_smr.bones[m_rootBoneID].lossyScale.y;
                rootPosition.z *= m_smr.bones[m_rootBoneID].lossyScale.z;

                Quaternion rootBoneRotation = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
                Vector3 oldRootPos = m_smr.bones[m_rootBoneID].position;
                Quaternion oldRootBoneRot = m_smr.bones[m_rootBoneID].rotation;
                Quaternion q = Quaternion.identity;                
                q = oldRootBoneRot * Quaternion.Inverse(rootBoneRotation);
                Vector3 deltaPos = q * rootPosition - m_smr.bones[m_rootBoneID].position;

                //bones are clild/parent so it require to be ordered                
                List<Transform> bones = m_smr.bones.ToList();
                List<Transform> bonesRefList = m_smr.bones.ToList();
                bones = bones.OrderBy(b => b.GetComponentsInParent<Transform>().Length).ToList();
                
                for (int i = 0; i < bones.Count; ++i)
                {
                    m = bindposes[bonesRefList.IndexOf(bones[i])].inverse;
                    Vector3 pos = (m.MultiplyPoint(Vector3.zero));
                    pos.x *= Mathf.Abs(bones[i].lossyScale.x);
                    pos.y *= Mathf.Abs(bones[i].lossyScale.y);
                    pos.z *= Mathf.Abs(bones[i].lossyScale.z);
                    bones[i].position = pos - deltaPos;
                    bones[i].rotation = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
                    bones[i].localScale = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);                    
                }
                
                m_rootParent.rotation = q * m_rootParent.rotation;
                m_rootParent.position += oldRootPos - m_smr.bones[m_rootBoneID].position;//fix ssg                
                
            }




            private void ReflectionRestoreToBindPose()
            {
                if (m_smr.gameObject == null && !Application.isEditor)
                    return;
                System.Type type = System.Type.GetType("UnityEditor.AvatarSetupTool, UnityEditor");
                if (type != null)
                {
                    System.Reflection.MethodInfo info = type.GetMethod("SampleBindPose", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    if (info != null)
                    {
                        info.Invoke(null, new object[] { m_smr.gameObject });
                    }
                }
            }

        }
    }
}

