using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [ExecuteInEditMode]
        [System.Serializable]        
        public class HairDesignerGeneratorFurShellBase : HairDesignerGenerator
        {
            [System.Serializable]
            public class LODGroup
            {
                public Vector2 m_range = new Vector2(0,100);
                public int m_shellCount = 100;
                public UnityEngine.Rendering.ShadowCastingMode m_shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                public float m_maskLOD = 0f;
            }

            [System.Serializable]
            public class EditorData
            {
                public float m_furMaskMin = 0f;
                public float m_furMaskMax = 1f;
                public bool m_unsavedColorTexFile = false;
                public bool m_unsavedMaskTexFile = false;
            }

            [HideInInspector]
            public EditorData m_editorData;
            public int m_shellCount = 100;
            public UnityEngine.Rendering.ShadowCastingMode m_shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            public float m_furWidthUpscale = 0f;
            //public bool m_needRefresh = false;
            public bool m_recalculateNormals = false;
            public List<bool> m_materialsEnabled = new List<bool>();
            public bool m_useLOD = false;
            public bool m_selectCurrentLOD = true;
            public List<LODGroup> m_LODGroups = new List<LODGroup>();

            //bool m_enableDrawMeshInstanced = false;//don't work yet with LOD
            
            public Material m_furMaterial;
            public Texture2D m_defaultFurDensity;
            MaterialPropertyBlock m_pb;
            MeshFilter m_mf;
            MeshRenderer m_mr;
            SkinnedMeshRenderer m_smr;
            Renderer m_r;
            GameObject m_root;

            [HideInInspector]
            public List<MeshRenderer> m_shells = new List<MeshRenderer>();
            public List<MeshFilter> m_shellsMF = new List<MeshFilter>();
            Mesh m_mesh;
            [HideInInspector]
            Material m_hiddenMaterial;

            public void Awake()
            {                

                InitMotionZones();
                if( !Application.isEditor)
                    ClearShells();

                if (Application.isPlaying)
                {
                    InitMaterial();
                    //Create an instance for this generator                    
                    m_furMaterial = new Material(m_furMaterial);
                }

            }


            void InitMotionZones()
            {
                m_motionZonePos = new Vector4[m_motionZones.Count];
                for (int i = 0; i<m_motionZonePos.Length; ++i)
                    m_motionZonePos[i] = Vector4.zero;

                m_motionZoneDir = new Vector4[m_motionZones.Count];
                for (int i = 0; i<m_motionZoneDir.Length; ++i)
                    m_motionZoneDir[i] = Vector4.zero;

                for (int i = 0; i < m_motionZones.Count; ++i)
                    m_motionZones[i].pid.Init();

            }


            public override void Start()
            {
                Init();
            }



            public void Init()
            {
                ClearShells();               


                m_root = new GameObject("FurShell_root");

                if (Application.isEditor)
                    //m_root.hideFlags = HideFlags.HideAndDontSave;
                    //m_root.hideFlags = HideFlags.DontSave;
                    m_root.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;

                m_root.transform.parent = null;
                m_root.transform.localScale = Vector3.one;
                m_root.transform.position = transform.position;
                m_root.transform.rotation = transform.rotation;

                //m_pb = new MaterialPropertyBlock();
                m_mesh = new Mesh();
                m_mf = GetComponent<MeshFilter>();
                m_mr = GetComponent<MeshRenderer>();
                m_smr = GetComponent<SkinnedMeshRenderer>();

                m_r = m_mr == null ? m_smr as Renderer : m_mr as Renderer;

                //m_matPropBlkHair.SetVectorArray(HairDesignerBase.m_shaderIDs[14], new Vector4[50]);
                //m_matPropBlkHair.SetVectorArray(HairDesignerBase.m_shaderIDs[15], new Vector4[50]);

                m_hiddenMaterial = new Material(Shader.Find("Hidden/HairDesigner/Hidden"));

#if UNITY_5_6_OR_NEWER
                m_hiddenMaterial.enableInstancing = true;
#endif
                
                UpdateShells();            
                   
            }



            public override void UpdateInstance()
            {
                //motion zone                
                if (this == null)
                    return;


                if (m_root == null)
                    Init();

                //UpdateShells();

                m_root.SetActive(m_enable);

                               
                
                m_root.transform.parent = null;
                m_root.transform.localScale = Vector3.one;
                if (m_smr != null)
                    m_root.transform.localScale = Vector3.one;
                else
                    m_root.transform.localScale = transform.lossyScale;                
                m_root.transform.position = transform.position;
                m_root.transform.rotation = transform.rotation;
                


                if (m_motionZonePos.Length != m_motionZones.Count)
                    InitMotionZones();

                for (int i = 0; i < m_motionZones.Count; ++i)
                {
                    Vector3 newpos = m_motionZones[i].parent.TransformPoint(m_motionZones[i].localPosition);
                    m_motionZones[i].motion = (newpos - m_motionZones[i].lastPosition) * m_motionZones[i].motionFactor;
                    m_motionZones[i].lastPosition = newpos;
                    m_motionZones[i].pid.m_params.limits = m_motionZones[i].motionLimit * m_hd.globalScale;
                    m_motionZones[i].pid.m_target = Vector3.Lerp(m_motionZones[i].pid.m_target, m_motionZones[i].motion, Time.deltaTime * m_motionZones[i].smooth);
                    m_motionZoneDir[i] = m_motionZones[i].pid.Compute(m_motionZoneDir[i]);
                    Vector4 v = m_motionZones[i].parent.TransformPoint(m_motionZones[i].localPosition);
                    v.w = m_motionZones[i].radius * m_hd.globalScale;
                    m_motionZonePos[i] = v;
                    //Debug.Log(v);
                }


                if (HairDesignerBase.m_shaderIDs != null && m_furMaterial!=null)
                { 
                  
                    if (m_motionZonePos.Length > 0)              
                    {
                        m_furMaterial.SetVectorArray(HairDesignerBase.m_shaderIDs[14], m_motionZonePos);
                        m_furMaterial.SetVectorArray(HairDesignerBase.m_shaderIDs[15], m_motionZoneDir);
                    }

                    m_furMaterial.SetVector(HairDesignerBase.m_shaderIDs[10],Physics.gravity);
                }
                

                

            }


            public override void LateUpdateInstance()
            {
                UpdateShells();
            }




            //Destroy all shells
            public void ClearShells()
            {
                
                for (int i = 0; i < m_shells.Count; ++i)
                {
                    if (m_shells[i] == null)
                        continue;

                    if (Application.isPlaying)
                        Destroy(m_shells[i].gameObject);
                    else
                        DestroyImmediate(m_shells[i].gameObject);
                }
                if (m_root != null)
                {
                    if (Application.isPlaying)
                        Destroy(m_root.gameObject);
                    else
                        DestroyImmediate(m_root.gameObject);
                }
                m_shells.Clear();
            }



            void InitMaterial()
            {
                if (m_hairMeshMaterial == null)
                    return;
                if (m_hd == null)
                    return;

                if (m_furMaterial == null || m_furMaterial.shader != m_hairMeshMaterial.shader || m_furMaterial == m_hairMeshMaterial )
                {                   
                    
                    HairDesignerShader s = GetShaderParams();
                    if (s == null) return;
                    if (m_hd == null) return;                    

                    
                    if (m_hd.GetComponent<MeshRenderer>() != null)
                    {
                        MeshRenderer mr = m_hd.GetComponent<MeshRenderer>();
                        if (mr == null) return;
                        //s.SetTexture(0, mr.sharedMaterial.mainTexture as Texture2D);
                    }

                    if (m_hd.GetComponent<SkinnedMeshRenderer>() != null )
                    {
                        SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();
                        if (smr == null) return;
                        //s.SetTexture(0, smr.sharedMaterial.mainTexture as Texture2D);
                    }

                    if (m_defaultFurDensity != null)
                        s.SetTexture(1,m_defaultFurDensity);
                    else
                        s.SetTexture(1, m_hd.m_defaultFurDensity);

                    m_furMaterial = new Material(m_hairMeshMaterial);
#if UNITY_5_6_OR_NEWER
                    m_furMaterial.enableInstancing = true;
#endif
                }

            }


            bool allowShellGeneration = true;
            Material[] m_sharedMaterials = null;
            //float[] m_furFactorArray= null;
            //float[] m_maskLODArray = null;
            //Matrix4x4[] m_matrixArray = null;


            //update the shell pool
            public void UpdateShells()
            {
                if (m_hd == null)
                    return;

                InitMaterial();                

                if (m_smr != null)
                {
                    m_smr.BakeMesh(m_mesh);
                    if(m_recalculateNormals)
                        m_mesh.RecalculateNormals();
                }
                
                //update current shader
                //GetShaderParams().UpdatePropertyBlock(ref m_matPropBlkHair, HairDesignerBase.eLayerType.FUR_SHELL);
                if (GetShaderParams()!=null)
                    GetShaderParams().UpdateMaterialProperty(ref m_furMaterial, HairDesignerBase.eLayerType.FUR_SHELL);
                //m_matPropBlkHair = new MaterialPropertyBlock();

                if (m_pb == null)
                    m_pb = new MaterialPropertyBlock();


                //-------------------------------------
                //DrawMesh GPU instancing -> TODO disable rendering when out of frustrum
                //DrawMesh don't care of camera view
                /*
                               if ( SystemInfo.supportsInstancing && m_enableDrawMeshInstanced && Application.isPlaying  )
                               {
                                   if (!m_enable)
                                       return;

                                   if (m_sharedMaterials == null)
                                       m_sharedMaterials = m_smr.sharedMaterials;

                                   if (m_furFactorArray == null)
                                   {
                                       m_furFactorArray = new float[500];
                                       m_maskLODArray = new float[500];
                                       m_matrixArray = new Matrix4x4[500];
                                   }

                                   for (int i = 0; i < m_shellCount; ++i)
                                   {
                                       float f = ((float)i + 1f) / (float)m_shellCount;
                                       m_furFactorArray[i] = f;
                                       m_maskLODArray[i] = m_furWidthUpscale;
                                       m_matrixArray[i] = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

                                   }

                                   m_pb.SetFloatArray("_FurFactor", m_furFactorArray);
                                   m_pb.SetFloatArray("_MaskLOD", m_maskLODArray);

                                   for (int m = 0; m < m_sharedMaterials.Length; ++m)
                                   {
                                       if (m_materialsEnabled.Count == m)
                                           m_materialsEnabled.Add(true);
                                       //m_sharedMaterials[m] = m_materialsEnabled[m] ? m_furMaterial : m_hiddenMaterial;
                                       if(m_materialsEnabled[m])
                                           Graphics.DrawMeshInstanced(m_mesh, m, m_furMaterial, m_matrixArray, m_shellCount, m_pb);
                                   }

                                   return;
                               }               
				*/
               

                //-------------------------------------




                if (allowShellGeneration)
                    while (m_shellCount > m_shells.Count)
                        GenerateShell();


                for (int i = 0; i < m_shells.Count; ++i)                
                {
                    if (m_shells[i] == null)
                        continue;

                    if (m_sharedMaterials == null)
                        m_sharedMaterials = m_shells[i].sharedMaterials;

                    if (i < m_shellCount)
                    {
                        float f = ((float)i + 1f) / (float)m_shellCount;
                        m_pb.SetFloat("_FurFactor", f);
                        m_pb.SetFloat("_MaskLOD", m_furWidthUpscale);
                        m_shells[i].SetPropertyBlock(m_pb);
                        m_shells[i].enabled = true;
                        m_shells[i].shadowCastingMode = m_shadowCastingMode;
                        //m_shells[i].GetComponent<MeshFilter>().sharedMesh = m_mesh;
                        if (m_shellsMF[i] == null)
                            m_shellsMF[i] = m_shells[i].GetComponent<MeshFilter>();
                        m_shellsMF[i].sharedMesh = m_mesh;


                        //Material[] sharedMaterials = m_shells[i].sharedMaterials;
                        for (int m = 0; m < m_sharedMaterials.Length; ++m)
                        {
                            if (m_materialsEnabled.Count == m)
                                m_materialsEnabled.Add(true);
                            m_sharedMaterials[m] = m_materialsEnabled[m] ? m_furMaterial : m_hiddenMaterial;
                        }

                        m_shells[i].sharedMaterials = m_sharedMaterials;

                        
                    }
                    else
                    {
                        //m_shells[i].enabled = false;
                        
                        for (int m = 0; m < m_sharedMaterials.Length; ++m)
                        {
                            if (m_materialsEnabled.Count == m)
                                m_materialsEnabled.Add(false);
                            m_sharedMaterials[m] = m_hiddenMaterial;
                        }
                        m_shells[i].sharedMaterials = m_sharedMaterials;
                    }
                }
                

                UpdateInstance();

            }





            void GenerateShell()
            {
                if(m_pb == null)
                    m_pb = new MaterialPropertyBlock();
                /*
                if(m_mr != null)
                    m_mr.GetPropertyBlock(m_matPropBlkHair);

                if (m_smr!=null)
                    m_smr.GetPropertyBlock(m_matPropBlkHair);
                */
                float f = (float)(m_shells.Count) / (float)m_shellCount;
                //f = 1 - Mathf.Pow(1 - f, 3);
                f = Mathf.Clamp01(f);
                m_pb.SetFloat("_FurFactor", f);
                m_pb.SetFloat("_MaskLOD", m_furWidthUpscale);

                if (m_root == null)
                    Init();
                                               

                GameObject go = new GameObject("shell_" + m_shells.Count);
                go.transform.parent = m_root.transform;
                go.transform.localScale = Vector3.one;
                //go.transform.localScale = transform.localScale;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;

                //go.hideFlags = HideFlags.HideAndDontSave;
                //go.hideFlags = HideFlags.DontSave;
                go.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;
                go.layer = gameObject.layer;

                Renderer r = null;
                if (m_mf != null)
                {
                    m_mesh = m_mf.sharedMesh;
                    r = m_mr;
                }
                if (m_smr != null)
                {
                    m_smr.BakeMesh(m_mesh);
                    r = m_smr;
                }

                if (m_recalculateNormals)
                    m_mesh.RecalculateNormals();
                
                MeshFilter mf = go.AddComponent<MeshFilter>();
                

                mf.sharedMesh = m_mesh;
                


                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                //mr.sharedMaterial = m_furMaterial;

                Material[] sharedMaterials = m_r.sharedMaterials;
                for (int i = 0; i < r.sharedMaterials.Length; ++i)
                {
                    if (m_materialsEnabled.Count == i)
                        m_materialsEnabled.Add(true);
                  sharedMaterials[i] = m_materialsEnabled[i]? m_furMaterial:sharedMaterials[i];
                }

                mr.sharedMaterials = sharedMaterials;
                mr.SetPropertyBlock(m_matPropBlkHair);
                


                m_shells.Add(mr);
                m_shellsMF.Add(mf);
            }



            public int GetLOD(Camera cam )
            {
                float dist = Vector3.Distance(cam.transform.position, transform.position);
                
                for (int i = 0; i < m_LODGroups.Count; ++i)
                {
                    //if (dist >= m_LODGroups[i].m_range.x && dist <= m_LODGroups[i].m_range.y)
                    if ( dist < m_LODGroups[i].m_range.y)
                        return i;
                }
                return m_LODGroups.Count - 1;
            }



            
            void OnWillRenderObject()
            {                
                
                if (!m_enable || m_root == null)
                    return;
                //m_root.SetActive(m_enable);//doesn't work in unity 5.6
                if (!m_useLOD || m_LODGroups.Count==0)
                    return;
                int lodId = GetLOD(Camera.current);
                m_shellCount = m_LODGroups[lodId].m_shellCount;
                m_shadowCastingMode = m_LODGroups[lodId].m_shadowCastingMode;
                m_furWidthUpscale = m_LODGroups[lodId].m_maskLOD;
                allowShellGeneration = false;
                UpdateShells();
                allowShellGeneration = true;
            }




            /*
            public override void EnableGenerator(bool e)
            {
                if(m_root!=null)
                    m_root.SetActive(e);
            }
            */


            void OnDisable()
            {
                if (m_root != null)
                    m_root.gameObject.SetActive(false);
            }

            void OnEnable()
            {
                if (m_root != null)
                    m_root.gameObject.SetActive(m_enable);
            }
        

            public override void Destroy()
            {
                ClearShells();
                base.Destroy();
            }
        }
    }
}
