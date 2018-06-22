﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
#if UNITY_EDITOR
using UnityEditor;
#endif
*/

namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [System.Serializable]
        public enum ePaintingTool
        {
            NONE,
            ADD,
            BRUSH,
            SCALE,
            COLOR,
            HEIGHT
        }

        
        [System.Serializable]
        public class GeneratorParams
        {
            public BZCurv m_curv = new BZCurv(Vector3.zero, Vector3.forward, Vector3.up * .5f, Vector3.zero);
            public Vector2 m_taper = new Vector2(1, 1);
            public float m_bendX = 1f;
            public float m_length = 2f;
            public int m_HairResolutionX = 2;
            public int m_HairResolutionY = 3;
            public float m_randomSrandFactor = 0f;
            public float m_offset = 0;
            public float m_gravityFactor = 0f;
            public float m_normalToTangent = 0f;
        }




        [System.Serializable]
        public class StrandRenderingData
        {
            public Vector3 localpos = Vector3.zero;
            public Quaternion rotation = Quaternion.identity;
            public Vector3 normal = Vector3.up;
            public float scale = 1f;
            public StrandData strand;
            public int layer = 0;
        }

        [System.Serializable]
        public class StrandData
        {
            public Vector3 localpos = Vector3.zero;
            public Quaternion rotation = Quaternion.identity;
            public Vector3 normal = Vector3.up;
            public float scale = 1f;
            public float offset = 0f;
            public BZCurv curve = null;
            public MBZCurv mCurve = null;
            public Mesh mesh;
            public int layer = 0;
            public int meshTriId = -1;           
            
            //public Vector2 uv;            
        }

        public class BrushToolData
        { 
            public Camera cam;
            public float brushSize = 0f;
            public Vector3 brushDir = Vector3.zero;
            public Vector3 brushScreenDir = Vector3.zero;
            public float brushIntensity = 1f;
            public float brushFalloff = 0f;
            public float brushScale = 1f;
            public ePaintingTool tool;
            public bool CtrlMode = false;
            public bool ShiftMode = false;
            public Vector3 mousePos;
            public Transform transform;
            

            public float GetBrushWeight( Vector3 pos, Vector3 normal )//return influence factor
            {
                if( cam == null && brushSize > 0f)
                {
                    return 0;
                }

                if (Vector3.Dot(cam.transform.forward, normal)>0)
                    return 0;

                float pixelSize = Vector3.Distance( cam.WorldToScreenPoint(cam.transform.position + cam.transform.forward), cam.WorldToScreenPoint(cam.transform.position + cam.transform.forward + cam.transform.right * brushSize));

                //Debug.Log("pixelSize " + pixelSize);

                Vector3 screenPos = cam.WorldToScreenPoint(pos);
                screenPos.z = 0;
                mousePos.z = 0;

                float dist = Vector3.Distance(screenPos, mousePos);
                if (dist < pixelSize)
                {
                    if (dist < pixelSize * (1-brushFalloff))
                        return 1f;
                    else
                        return 1- ((dist - pixelSize * (1-brushFalloff)) / ( pixelSize * brushFalloff));
                }

                return 0;

            }
        }


        [System.Serializable]
        public class MotionZone
        {
            public Transform parent;
            public Vector3 localPosition = Vector3.zero;
            public Vector3 motion = Vector3.zero;
            public Vector3 lastPosition = Vector3.zero;
            public float radius = 1f;
            public HairPID_V3 pid = new HairPID_V3();
            public float smooth = 20;
            public Vector2 motionLimit = new Vector2(-1, 1);
            public float motionFactor = 1f;
        }





        [System.Serializable]
        public class GeneratorModifier
        {
            public int id = 1;
            public string name;
            public bool autoUpdate = false;
            public List<int> layers = new List<int>();
        }


        




        [System.Serializable]
        public class HairDesignerGenerator : MonoBehaviour
        {
            public HairDesignerBase.eLayerType m_layerType = HairDesignerBase.eLayerType.NONE;

            [HideInInspector]
            public SkinnedMeshRenderer m_skinnedMesh;
            [HideInInspector]
            public MeshRenderer m_meshRenderer;
            //[HideInInspector]
            //public MaterialPropertyBlock m_matPropBlkHairStrand;
            [HideInInspector]
            public MaterialPropertyBlock m_matPropBlkHair;
            bool m_enableSkinMesh = false;

            public HairDesignerBase m_hd;
            public Mesh m_hairStrand;
            public Mesh m_hair;
            //bool m_Started = false;
            public bool m_initialized = false;
            public float m_scale = 1f;

            public GeneratorParams m_params = new GeneratorParams();
            public StrandRenderingData m_data = new StrandRenderingData();
            public bool m_requirePainting = false;

            public Quaternion m_skinMeshGenerationRot = Quaternion.identity;
            public Vector3 m_skinMeshGenerationPos = Vector3.zero;

            public bool m_enable = true;
            public string m_name = "";

            public virtual StrandRenderingData GetData(int id) { return m_data; }
            public virtual int GetStrandCount() { return 0; }
            //public virtual void AddTool(StrandpaintingData data, bool erase) { }
            public virtual void PaintTool(StrandData data, BrushToolData bt) { }

            Vector3 m_gravity = Vector3.down;
            public HairPID_V3 m_pid = new HairPID_V3();
            public int m_rndSeed = 0;

            public Vector3 m_direction;
            public bool _editorDelete = false;

            static int m_batchingFix = -int.MaxValue;//fix batching issue

            public bool m_allowFastMeshDraw = true;//fast mesh draw must be disable for incompatible shaders
            public bool m_needRefresh = false;

            //atlas size : used for the texture generator
            public int m_atlasSizeX = 1;
            public int m_atlasSizeY //the Y is a copy of the X value, until the texture generator provide non-square atlas
            {
                get { return m_atlasSizeX; }
            }

            //List<HairPID_V3> m_motionZonePID = new List<HairPID_V3>();
            protected Vector4[] m_motionZonePos = new Vector4[50];
            protected Vector4[] m_motionZoneDir = new Vector4[50];

            public List<MotionZone> m_motionZones = new List<MotionZone>();










            //public HairDesignerMesh m_mesh = null;

            //public Material m_hairStrandMaterial;
            public Material m_hairMeshMaterial = null;
            public Material m_hairMeshMaterialTransparent = null;
            public GameObject m_meshInstance = null;

            public List<HairDesignerShader> m_shaderParams = new List<HairDesignerShader>();
            public bool m_wasInDesignTab = false;
            public bool m_meshLocked = false;
            public bool m_materialInitialized = false;
            public bool m_shaderNeedUpdate = true;

            public List<HairToolLayer> m_editorLayers = new List<HairToolLayer>();
            //public List<GeneratorModifier> m_modifiers = new List<GeneratorModifier>();
            public int nextModifierID = 1;

            public float m_maxStrandLength = 1f;//set to 1 for compatibility with 1.2.0

            public HairDesignerShader GetShaderParams()
            {
                if (m_hairMeshMaterial == null)
                    return null;
                return GetShaderParams(m_hairMeshMaterial.shader);
            }


            public HairDesignerShader GetShaderParams( Shader shader )
            {
                for (int i = 0; i < m_shaderParams.Count; ++i)
                    if (m_shaderParams[i].m_shader == shader)
                        return m_shaderParams[i];
                return null;
            }



            public float strandScaleFactor
            {
                get
                {
                    if (m_hd == null)
                    {
                        Debug.LogWarning("HairDesigner instance not set");
                        return 1;
                    }

                    MeshRenderer mr = m_hd.GetComponent<MeshRenderer>();
                    SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();

                    if (mr != null)
                        return mr.bounds.size.magnitude / m_hd.globalScale;
                    if (smr != null)
                        return smr.bounds.size.magnitude / m_hd.globalScale;

                    return 1;
                }
            }



            public virtual void Start()
            {
                if (m_hd == null)
                { 
                    if( Application.isPlaying )
                        Destroy(this);
                    else
                        DestroyImmediate(this);
                    return;
                }

                m_pid = new HairPID_V3();
                m_pid.Init();

                if (!m_meshLocked )
                {

                    if (m_hair == null)
                    {
                        SkinnedMeshRenderer smrRef = m_hd.GetComponent<SkinnedMeshRenderer>();
                        CreateHairMesh(smrRef != null ? smrRef.sharedMesh : null);
                        GenerateMeshRenderer();
                    }
                    if (m_meshInstance != null)
                    {

                        MeshFilter mf = m_meshInstance.GetComponent<MeshFilter>();
                        if (mf != null && mf.mesh == null)
                            mf.mesh = m_hair;

                        SkinnedMeshRenderer smr = m_meshInstance.GetComponent<SkinnedMeshRenderer>();
                        if (smr != null && smr.sharedMesh == null)
                            smr.sharedMesh = m_hair;
                    }  
                }
                //m_Started = true;
                m_gravity = Physics.gravity;

                m_motionZonePos = new Vector4[m_motionZones.Count];
                for (int i = 0; i < m_motionZonePos.Length; ++i)
                    m_motionZonePos[i] = Vector4.zero;

                m_motionZoneDir = new Vector4[m_motionZones.Count];
                for (int i = 0; i < m_motionZoneDir.Length; ++i)
                    m_motionZoneDir[i] = Vector4.zero;

                for (int i = 0; i < m_motionZones.Count; ++i)
                    m_motionZones[i].pid.Init();

                m_shaderNeedUpdate = true;

                /*
                //init property block                
                for (int i = 0; i < m_shaderParams.Count; ++i)
                    if (m_shaderParams[i].m_shader == m_hairMeshMaterial.shader)
                        m_shaderParams[i].InitPropertyBlock(ref m_matPropBlkHair, m_layerType); ;
                */
            }







            public virtual void DestroyMesh()
            {
                if (m_meshInstance != null)
                {
                    if (Application.isPlaying)
                        Destroy(m_meshInstance);
                    else
                        DestroyImmediate(m_meshInstance);

                    m_meshInstance = null;
                }
            }




            public virtual void GenerateMeshRenderer()
            {
                if (GetStrandCount() == 0)
                    return;

                if (m_hd == null || m_meshInstance != null || _editorDelete )
                    return;

                if(m_matPropBlkHair == null )
                    m_matPropBlkHair = new MaterialPropertyBlock();

                DestroyMesh();

                SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();
                m_enableSkinMesh = smr != null;
                //generate HairMesh                

                m_meshInstance = new GameObject(m_name);
                m_meshInstance.transform.SetParent(m_hd.transform, true);
                m_meshInstance.transform.localRotation = Quaternion.identity;
                m_meshInstance.transform.localPosition = Vector3.zero;
                m_meshInstance.transform.localScale = Vector3.one;

                //required for undo
                m_meshInstance.AddComponent<HairDesignerMeshInstanceBase>();

                //m_mesh = go.AddComponent<HairDesignerMesh>();

                if (m_enableSkinMesh)
                {
                    m_skinnedMesh = m_meshInstance.AddComponent<SkinnedMeshRenderer>();
                    CreateHairMesh(smr.sharedMesh);
                    m_skinnedMesh.sharedMesh = m_hair;
                    //m_skinnedMesh.sharedMaterial = m_hairMeshMaterial;
                    m_skinnedMesh.bones = smr.bones;
                    m_skinnedMesh.rootBone = smr.rootBone;
                    if (m_hairMeshMaterialTransparent == null)
                    {
                        m_skinnedMesh.sharedMaterial = m_hairMeshMaterial;
                    }
                    else
                    {
                        Material[] mats = new Material[2];
                        mats[0] = m_hairMeshMaterial;
                        mats[1] = m_hairMeshMaterialTransparent;
                        m_skinnedMesh.sharedMaterials = mats;
                    }
                }
                else
                {
                    MeshFilter mf = m_meshInstance.AddComponent<MeshFilter>();
                    CreateHairMesh(null);
                    mf.sharedMesh = m_hair;
                    m_meshRenderer = m_meshInstance.AddComponent<MeshRenderer>();
                    if (m_hairMeshMaterialTransparent == null)
                    {                       
                        m_meshRenderer.sharedMaterial = m_hairMeshMaterial;
                    }                    
                    else
                    {
                        Material[] mats = new Material[2];
                        mats[0] = m_hairMeshMaterial;
                        mats[1] = m_hairMeshMaterialTransparent;
                        m_meshRenderer.sharedMaterials = mats;
                    }


                }          
                 
            }


            public virtual void InitEditor()
            {
                //Debug.Log("Init generator base");
                m_initialized = true;
            }



            public virtual void UpdateInstance()
            {
                if (m_meshInstance != null)
                {                   
                    m_meshInstance.SetActive(m_enable);
                }

                EnableGenerator(m_enable);

                if (!m_enable)
                {                    
                    return;
                }


                /*
                if (Application.isPlaying && !m_Started)
                    Start();*/

                if (!Application.isPlaying && !m_initialized)
                    InitEditor();

                

                                
                if (Application.isPlaying)
                    m_direction = m_pid.Compute(m_direction);
                else
                    m_direction = Vector3.zero;

                m_params.m_taper.x = Mathf.Clamp(m_params.m_taper.x, 0, m_params.m_taper.x);
                m_params.m_taper.y = Mathf.Clamp(m_params.m_taper.y, 0, m_params.m_taper.y);
                
                if (m_matPropBlkHair == null)
                    m_matPropBlkHair = new MaterialPropertyBlock();

                m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[10], m_gravity);
                m_matPropBlkHair.SetFloat(HairDesignerBase.m_shaderIDs[11], m_params.m_gravityFactor);
                m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[12], m_hd.transform.lossyScale);
                m_matPropBlkHair.SetFloat(HairDesignerBase.m_shaderIDs[16], m_maxStrandLength);

                //if (m_meshInstance == null)
                //    GenerateMeshRenderer();

                
                if (!Application.isPlaying && m_meshInstance == null && m_allowFastMeshDraw )
                {
                    if (m_hairStrand == null)
                        m_hairStrand = CreateHairStrandMesh();
                    DrawHairStrand();
                }
                else
                {
                    if (m_meshInstance == null || m_needRefresh)
                    {
                        DestroyMesh();
                        GenerateMeshRenderer();
                        m_needRefresh = false;
                    }


                        m_matPropBlkHair.SetFloat("KHD_editor", 0);
                    //Vector3 dir = m_hd.transform.InverseTransformDirection(m_direction);
                    m_batchingFix++;
                    //m_matPropBlkHair.SetVector(HairDesigner.m_shaderIDs[8], new Vector4(dir.x, dir.y, dir.z, m_batchingFix));
                    m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[8], new Vector4(0, 0, 0, m_batchingFix));//only motion zone
                    
                    //motion zone
                    for (int i = 0; i < m_motionZones.Count; ++i)
                    {
                        Vector3 newpos = m_motionZones[i].parent.TransformPoint(m_motionZones[i].localPosition);
                        m_motionZones[i].motion = (newpos - m_motionZones[i].lastPosition)* m_motionZones[i].motionFactor;
                        m_motionZones[i].lastPosition = newpos;

                        m_motionZones[i].pid.m_params.limits = m_motionZones[i].motionLimit * m_hd.globalScale;

                        /*
                        if (m_motionZonePos.Length <= i)
                        {
                            //m_motionZonePID.Add(new HairPID_V3());
                            m_motionZonePos.Add(Vector4.zero);
                            m_motionZoneDir.Add(Vector3.zero);
                        }*/

                        //m_motionZonePID[i].m_params = m_pid.m_params;
                        m_motionZones[i].pid.m_target = Vector3.Lerp(m_motionZones[i].pid.m_target, m_motionZones[i].motion,Time.deltaTime * m_motionZones[i].smooth);
                        m_motionZoneDir[i] = m_motionZones[i].pid.Compute(m_motionZoneDir[i]);
                        Vector4 v = m_motionZones[i].parent.TransformPoint(m_motionZones[i].localPosition);
                        v.w = m_motionZones[i].radius*m_hd.globalScale;
                        m_motionZonePos[i] = v;

                    }

                    
                    //------------------------------------------                    
                    if (m_motionZonePos.Length > 0)
                    {
                        m_matPropBlkHair.SetVectorArray(HairDesignerBase.m_shaderIDs[14], m_motionZonePos);
                        m_matPropBlkHair.SetVectorArray(HairDesignerBase.m_shaderIDs[15], m_motionZoneDir);
                    }
                    //------------------------------------------
                    
                }

                //Rendering data
                //HairDesignerShader hds = m_shaderParams.FindLast(sp => sp.m_shader == m_hairMeshMaterial.shader); -->memory leak!
                HairDesignerShader hds = null;

                for (int i = 0; i < m_shaderParams.Count; ++i)
                {
                    if (m_shaderParams[i] == null)
                    {
                        m_shaderParams.RemoveAt(i--);
                    }
                    else
                    {
                        if (m_shaderParams[i].m_shader == m_hairMeshMaterial.shader)
                            hds = m_shaderParams[i];
                    }
                }


                if (hds != null && (m_shaderNeedUpdate||Application.isEditor))
                //if (hds != null )
                {                    
                    hds.UpdatePropertyBlock(ref m_matPropBlkHair, m_layerType);
                    m_shaderNeedUpdate = false;
                }

                if (m_hd.m_windZone != null)
                {
                    m_matPropBlkHair.SetVector("KHD_windZoneDir", m_hd.m_windZoneDir);
                    m_matPropBlkHair.SetVector("KHD_windZoneParam", m_hd.m_windZoneParam);
                }

                if (m_skinnedMesh != null)
                    m_skinnedMesh.SetPropertyBlock(m_matPropBlkHair);
                if (m_meshRenderer != null)
                    m_meshRenderer.SetPropertyBlock(m_matPropBlkHair);

                m_gravity = Physics.gravity;
            }







            public virtual void LateUpdateInstance()
            {

            }





                Mesh CreateHairStrandMesh()
            {
                Mesh m = new Mesh();

                List<Vector3> v = new List<Vector3>();
                List<Vector3> n = new List<Vector3>();
                List<int> t = new List<int>();

                int X = m_params.m_HairResolutionX;
                int Y = m_params.m_HairResolutionY;
                float scale = 1f;

                for (int x = 0; x < X; ++x)
                {
                    for (int y = 0; y < Y; ++y)
                    {
                        float XStep = 1f / (float)X;
                        float YStep = 1f / (float)Y;

                        //shape
                        float shp = 0;// Mathf.Abs((XStep * x) - .5f);

                        //Add vertices
                        v.Add(new Vector3((XStep * x) - .5f, 0, (YStep * y)- shp) * scale );
                        v.Add(new Vector3((XStep * x) + XStep - .5f, 0, (YStep * y)- shp) * scale);
                        v.Add(new Vector3((XStep * x) - .5f, 0, (YStep * y) + YStep - shp ) * scale);
                        v.Add(new Vector3((XStep * x) + XStep - .5f, 0, (YStep * y) + YStep - shp) * scale);

                        //Add normals
                        
                        float n2t = m_params.m_normalToTangent;
                        n.Add(new Vector3((XStep * x) - .5f, 1, n2t).normalized);
                        n.Add(new Vector3((XStep * x) + XStep - .5f, 1, n2t).normalized);
                        n.Add(new Vector3((XStep * x) - .5f, 1, n2t).normalized);
                        n.Add(new Vector3((XStep * x) + XStep - .5f, 1, n2t).normalized);
                        


                        //Add triangles
                        t.Add(v.Count - 4);
                        t.Add(v.Count - 2);
                        t.Add(v.Count - 3);


                        t.Add(v.Count - 1);
                        t.Add(v.Count - 3);
                        t.Add(v.Count - 2);


                    }
                }

                //setup UV from vertices position
                List<Vector2> uv = new List<Vector2>();
                for (int i = 0; i < v.Count; ++i)
                    uv.Add(new Vector2(v[i].x / scale + .5f, v[i].z / scale));



                m.SetVertices(v);
                m.SetNormals(n);
                m.SetTriangles(t, 0);
                m.SetUVs(0, uv);
                m.name = "HairStrand";
                //m.RecalculateBounds();
                Bounds b = m.bounds;
                b.size *= 10f;//force rendering when bound out of Vertex program modification
                m.bounds = b;
                //m.MarkDynamic();

                //Debug.Log("Hair strand mesh : " + (t.Count / 3) + " tri");

                //m.RecalculateNormals();

                return m;

            }



            /*
            public void DrawSingleHairStrand(Camera cam, int layer)
            {
                m_matPropBlkHairStrand.SetVector(HairDesigner.m_shaderIDs[0], new Vector4(m_curv.m_startPosition.x, m_curv.m_startPosition.y, m_curv.m_startPosition.z, 0));
                m_matPropBlkHairStrand.SetVector(HairDesigner.m_shaderIDs[1], new Vector4(m_curv.m_startTangent.x, m_curv.m_startTangent.y, m_curv.m_startTangent.z, 0));
                m_matPropBlkHairStrand.SetVector(HairDesigner.m_shaderIDs[2], new Vector4(m_curv.m_endPosition.x, m_curv.m_endPosition.y, m_curv.m_endPosition.z, 0));
                m_matPropBlkHairStrand.SetVector(HairDesigner.m_shaderIDs[3], new Vector4(m_curv.m_endTangent.x, m_curv.m_endTangent.y, m_curv.m_endTangent.z, 0));
                m_matPropBlkHairStrand.SetVector(HairDesigner.m_shaderIDs[4], m_taper);
                m_matPropBlkHairStrand.SetFloat(HairDesigner.m_shaderIDs[5], m_bendX);
                m_matPropBlkHairStrand.SetFloat(HairDesigner.m_shaderIDs[6], m_length);
                m_matPropBlkHairStrand.SetFloat(HairDesigner.m_shaderIDs[7], m_rigidity);

                Graphics.DrawMesh(m_hairStrand, Vector3.zero, Quaternion.identity, m_materialHairStrand, layer, cam, 0, m_matPropBlkHairStrand, true, true);
            }
            */


            public virtual void DrawHairStrand()
            {
                //Draw hair stand
                //float radius = m_startRadius;

                if (m_matPropBlkHair == null)
                    m_matPropBlkHair = new MaterialPropertyBlock();


                HairDesignerBase.InitRandSeed(0);

                m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[0], new Vector4(m_params.m_curv.m_startPosition.x, m_params.m_curv.m_startPosition.y, m_params.m_curv.m_startPosition.z, 0));
                m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[1], new Vector4(m_params.m_curv.m_startTangent.x, m_params.m_curv.m_startTangent.y, m_params.m_curv.m_startTangent.z, 0));
                m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[2], new Vector4(m_params.m_curv.m_endPosition.x, m_params.m_curv.m_endPosition.y, m_params.m_curv.m_endPosition.z, 0));
                m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[3], new Vector4(m_params.m_curv.m_endTangent.x, m_params.m_curv.m_endTangent.y, m_params.m_curv.m_endTangent.z, 0));
                m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[4], m_params.m_taper);
                m_matPropBlkHair.SetFloat(HairDesignerBase.m_shaderIDs[5], m_params.m_bendX);
                m_matPropBlkHair.SetFloat(HairDesignerBase.m_shaderIDs[6], m_params.m_length);
                //m_matPropBlkHair.SetFloat(HairDesigner.m_shaderIDs[7], m_params.m_rigidity);
                m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[8], Vector3.zero);

                m_batchingFix = 1;
                for (int i = 0; i < GetStrandCount(); ++i)
                //for (int i = 0; i < 2; ++i)
                {                    

                    m_batchingFix++;
                    m_batchingFix = 1;
                    //StrandRenderingData dt = GetSphereData();
                    StrandRenderingData dt = GetData(i);
                    if (dt.layer >= m_editorLayers.Count || !m_editorLayers[dt.layer].visible)
                        continue;
                    /*
                    if( dt.strand.curve != null )
                    {
                        m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[0], new Vector4(dt.strand.curve.m_startPosition.x, dt.strand.curve.m_startPosition.y, dt.strand.curve.m_startPosition.z, 0));
                        m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[1], new Vector4(dt.strand.curve.m_startTangent.x, dt.strand.curve.m_startTangent.y, dt.strand.curve.m_startTangent.z, 0));
                        m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[2], new Vector4(dt.strand.curve.m_endPosition.x, dt.strand.curve.m_endPosition.y, dt.strand.curve.m_endPosition.z, 0));
                        m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[3], new Vector4(dt.strand.curve.m_endTangent.x, dt.strand.curve.m_endTangent.y, dt.strand.curve.m_endTangent.z, 0));

                    }*/


                    //Debug.DrawLine(transform.TransformPoint(dt.localpos), transform.TransformPoint(dt.localpos + dt.normal*.01f));

                    Vector3 endPos = m_params.m_curv.m_endPosition;// + dt.rotation * m_hd.transform.InverseTransformDirection((m_hd.m_direction));
                    endPos += Random.insideUnitSphere * m_params.m_randomSrandFactor;

                    Vector3 worldPos = m_hd.transform.TransformPoint(dt.localpos + dt.normal * (dt.strand.offset + m_params.m_offset));
                    //Vector3 worldNormal = m_hd.transform.TransformDirection(dt.normal);
                    //m_matPropBlkHairStrand.SetVector(m_shaderIDs[1], new Vector4(startTan.x, startTan.y, startTan.z, 0));
                    m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[2], new Vector4(endPos.x, endPos.y, endPos.z, m_batchingFix));//need a difference to avoid batching...
                                                                                                                                            //m_matPropBlkHairStrand.SetVector(m_shaderIDs[3], new Vector4(endTan.x, endTan.y, endTan.z, 0));

                    Vector3 scale = m_hd.transform.lossyScale;
                    m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[12], new Vector4(scale.x, scale.y, scale.z, 0));

                    
                    float p = dt.scale;
                    m_matPropBlkHair.SetFloat(HairDesignerBase.m_shaderIDs[9], p );                    
                    Quaternion q = m_hd.transform.rotation * dt.rotation;
                    Vector4 r = new Vector4(q.x, q.y, q.z, q.w);
                    m_matPropBlkHair.SetVector(HairDesignerBase.m_shaderIDs[13], r);                    
                    Matrix4x4 m = Matrix4x4.TRS(worldPos, m_hd.transform.rotation * dt.rotation, Vector3.one);

                    m_matPropBlkHair.SetFloat("KHD_editor", 1);

                    Graphics.DrawMesh(m_hairStrand, m, m_hairMeshMaterial, 0, null, 0, m_matPropBlkHair, false, false);
                    if(m_hairMeshMaterialTransparent!= null )
                        Graphics.DrawMesh(m_hairStrand, m, m_hairMeshMaterialTransparent, 0, null, 0, m_matPropBlkHair, false, false);




                }


                HairDesignerBase.RestoreRandSeed();
            }



            public virtual void EnableGenerator( bool e )
            {

            }




            public virtual void CreateHairMesh(Mesh skinRef)
            {
                if (m_hairStrand == null)
                    m_hairStrand = CreateHairStrandMesh();

                HairDesignerBase.InitRandSeed(0);

                m_hair = new Mesh();

                //float radius = m_startRadius;

                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector4> tangents = new List<Vector4>();
                List<int> triangles = new List<int>();
                List<Vector2> uv = new List<Vector2>();
                List<BoneWeight> boneWeights = new List<BoneWeight>();
                List<Color> colors = new List<Color>();

                Vector3[] strandVertices = m_hairStrand.vertices;
                Vector3[] strandNormals = m_hairStrand.normals;
                Vector4[] strandTangents = new Vector4[m_hairStrand.vertices.Length];
                int[] strandTriangles = m_hairStrand.triangles;
                Vector2[] strandUV = m_hairStrand.uv;

                Vector3[] skinRefVertices = null;
                BoneWeight[] skinRefBoneWeights = null;
                int[] skinRefTriangles = null;

                if (skinRef != null)
                {
                    skinRefVertices = skinRef.vertices;
                    skinRefBoneWeights = skinRef.boneWeights;
                    skinRefTriangles = skinRef.triangles;
                }


                //get the max value
                float maxStrandLength=0;
                for (int i = 0; i < GetStrandCount(); ++i)
                {                    
                    StrandRenderingData dt = GetData(i);
                    if (m_editorLayers[dt.layer].visible)
                    {
                        if (maxStrandLength < dt.scale * m_params.m_length)
                            maxStrandLength = dt.scale * m_params.m_length;
                    }
                }

                //add all strand to the mesh
                for (int i = 0; i < GetStrandCount(); ++i)
                {
                    StrandRenderingData dt = GetData(i);

                    if (!m_editorLayers[dt.layer].visible)
                        continue;
                    //-----------------------------------------------
                    //Apply strand curv
                    strandVertices = m_hairStrand.vertices;
                    strandNormals = m_hairStrand.normals;
                    strandTangents = new Vector4[m_hairStrand.vertices.Length];
                    strandTriangles = m_hairStrand.triangles;
                    strandUV = m_hairStrand.uv;
                    Vector3 rnd = Random.insideUnitSphere * m_params.m_randomSrandFactor;



                    for (int v = 0; v < strandVertices.Length; ++v)
                    {
                        Vector3 vertex = strandVertices[v];
                        vertex.x *= Mathf.Lerp(m_params.m_taper.x, m_params.m_taper.y, strandUV[v].y);//taper
                        vertex.y -= Mathf.Abs(vertex.x) * Mathf.Abs(vertex.x) * m_params.m_bendX * (.1f + strandUV[v].y * .9f);//bend
                        //vertex.z *= m_params.m_length;//length

                        float t = strandUV[v].y;
                        //float t2 = t * t;

                        BZCurv curv = new BZCurv(m_params.m_curv.m_startPosition, m_params.m_curv.m_endPosition + rnd, m_params.m_curv.m_startTangent, m_params.m_curv.m_endTangent);
                        Vector3 c = curv.GetPosition(t);

                        //Vector3 n = Vector3.Lerp(new Vector3(c.x, c.y, c.z), new Vector3(c.x * t2, c.y * t2, c.z), m_params.m_rigidity * t);
                        Vector3 n = new Vector3(c.x, c.y, c.z* m_params.m_length);
                        vertex.x += n.x;
                        vertex.y += n.y;
                        vertex.z = n.z;

                        strandVertices[v] = vertex;
                        strandTangents[v] = m_params.m_curv.GetTangent(t).normalized;
                        strandTangents[v].w = -1;

                        strandTangents[v] = new Vector4(0, 0, 1, -1);


                    }
                    //-----------------------------------------------



                    //StrandRenderingData dt = GetSphereData();
                    //StrandRenderingData dt = GetData(i);

                    /*
                    if (generator.GetStandCount() == 1) // for test only
                    {
                        //rndRot = Quaternion.identity;
                        dt.rotation = Quaternion.identity;
                        radius = 0;
                    }*/


                    if (vertices.Count + strandVertices.Length > 65000)
                    {
                        Debug.LogWarning("Layer '"+m_name+"' Too much vertices " + (vertices.Count + strandVertices.Length));
                        break;
                    }





                    //rndRot* Vector3.forward* radius

                    //dt.strand.meshTriId = -1;
                    int vtxId = -1;
                    if (skinRef != null && dt.strand.meshTriId == -1 )
                    {                        
                        //old way -> now triangle id is stored into strand data
                        float minDist = float.MaxValue;
                        for (int j = 0; j < skinRef.vertexCount; ++j)
                        {
                            float dist = Vector3.Distance(m_skinMeshGenerationRot * (skinRefVertices[j]) + m_skinMeshGenerationPos, dt.localpos);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                vtxId = j;
                            }
                        }
                        
                    }

                    for (int t = 0; t < strandTriangles.Length; ++t)
                        triangles.Add(strandTriangles[t] + vertices.Count);

                    Vector2 _uvRnd = new Vector2(Random.Range(0, m_atlasSizeX), Random.Range(0, m_atlasSizeX)) / m_atlasSizeX;

                    for (int v = 0; v < strandVertices.Length; ++v)
                    {
                        //apply rotation and position

                        Vector3 pos = (dt.rotation * strandVertices[v] * dt.scale ) + dt.localpos  + dt.normal * (dt.strand.offset + m_params.m_offset);
                        //pos *= .1f;

                        if (m_enableSkinMesh)
                        {
                            pos = Quaternion.Inverse(m_skinMeshGenerationRot) * (pos - m_skinMeshGenerationPos);


                            if (dt.strand.meshTriId != -1)
                            {
                                //define skin weight from the triangle
                                //Todo create a % weight of the 3 vertices
                                int v0 = skinRefTriangles[dt.strand.meshTriId*3];
                                //int v1 = skinRefTriangles[dt.strand.meshTriId*3+1];
                                //int v2 = skinRefTriangles[dt.strand.meshTriId*3+2];

                                //dt.localpos - 

                                /*
                                if (vtxId != v0 && vtxId != v1 && vtxId != v2)
                                    Debug.Log("vertex ref not found");
                                    */
                                int v3 = v0;
                                float mindist = Vector3.Distance(skinRefVertices[v0], dt.localpos);
                                for( int k=1; k<3; ++k  )
                                {
                                    if(Vector3.Distance(skinRefVertices[skinRefTriangles[dt.strand.meshTriId * 3+k]], dt.localpos) < mindist)
                                    {
                                        v3 = skinRefTriangles[dt.strand.meshTriId * 3 + k];
                                    }
                                }

                                boneWeights.Add(skinRefBoneWeights[v3]);
                            }
                            else
                            {
                                if (vtxId != -1)
                                    boneWeights.Add(skinRefBoneWeights[vtxId]);
                                else
                                    Debug.LogWarning("Vertex not found");
                            }
                        }


                        /*
                        if (m_enableSkinMesh)
                            pos =  Quaternion.Inverse( rootBone.rotation )* pos;
                            //pos = ( dt.rotation * strandVertices[v] * transform.lossyScale.x * dt.scale) ;
                            */

                        vertices.Add(pos);
                        //colors.Add(new Color(dt.localpos.x, dt.localpos.y, dt.localpos.z, strandVertices[v].magnitude * dt.scale));
                        //colors.Add(new Color(dt.localpos.x, dt.localpos.y, dt.localpos.z, strandVertices[v].magnitude));

                        //store the % of max value in the alpha channel
                        float p = Mathf.Round(((dt.scale * m_params.m_length) / maxStrandLength) * 255f) / 255f;
                        colors.Add(new Color(dt.localpos.x, dt.localpos.y, dt.localpos.z, p ));
                        //Debug.Log( ">" + ((dt.scale * m_params.m_length) / maxStrandLength) );

                        tangents.Add(dt.rotation * strandTangents[v]);

                        
                        uv.Add(strandUV[v]/m_atlasSizeX + _uvRnd );
                    }

                    for (int n = 0; n < strandNormals.Length; ++n)
                        normals.Add(dt.rotation * strandNormals[n]);



                }

                m_maxStrandLength = maxStrandLength;

                m_hair.vertices = vertices.ToArray();
                m_hair.normals = normals.ToArray();
                m_hair.tangents = tangents.ToArray();
                m_hair.triangles = triangles.ToArray();
                m_hair.uv = uv.ToArray();
                m_hair.colors = colors.ToArray();
                m_hair.name = "HairDesignerInstance";
                
                if (skinRef != null)
                {
                    m_hair.boneWeights = boneWeights.ToArray();
                    m_hair.bindposes = skinRef.bindposes;
                }

                HairDesignerBase.RestoreRandSeed();
                m_hair.RecalculateBounds();
                //m_hair.RecalculateNormals();

            }


            public int GenerateModifierId()
            {                
                return nextModifierID++;
            }


            
            public bool m_HairDesignerDeleted = false;

            public void OnDestroy()
            {
                Destroy();
            }

            public virtual void Destroy()
            {

                //Debug.LogWarning("DestroyGenerator");
                if(m_hd!=null)
                    m_hd.m_generators.Remove(this);


                if (Application.isPlaying)
                {
                    Destroy(m_meshInstance);
                }
                else
                {
                    DestroyImmediate(m_meshInstance);                    
                }

                for (int j = 0; j < m_shaderParams.Count; ++j)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(m_shaderParams[j]);
                    }
                    else
                    {
                        //DestroyImmediate(m_shaderParams[j]);
                        if(m_shaderParams[j]!=null)
                            m_shaderParams[j].hideFlags = HideFlags.DontSave;
                    }
                }
                m_shaderParams.Clear();
            }



            public void DestroyShaderParams()
            {
                for (int j = 0; j < m_shaderParams.Count; ++j)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(m_shaderParams[j]);
                    }
                    else
                    {
                        DestroyImmediate(m_shaderParams[j]);
                    }
                }
            }


            /*
            void OnDrawGizmos()
            {
                for (int i = 0; i < m_motionZones.Count; ++i)
                {
                    Vector3 newpos = m_motionZones[i].parent.TransformPoint(m_motionZones[i].localPosition);

                    Gizmos.DrawSphere(newpos, 1f);
                }
            }*/

        }
    }
}
