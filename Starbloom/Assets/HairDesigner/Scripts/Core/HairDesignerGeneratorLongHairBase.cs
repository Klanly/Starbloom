using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Kalagaan
{    
    namespace HairDesignerExtension
    {
        [System.Serializable]
        //[ExecuteInEditMode]
        public class HairDesignerGeneratorLongHairBase : HairDesignerGenerator
        {


            #region sub-classes definition
            //---------------------------------------------------------------------

            /// <summary>
            /// Hair group data
            /// </summary>
            [System.Serializable]
            public class HairGroup
            {
                public List<StrandData> m_strands = new List<StrandData>();
                //public BZCurv m_curv = new BZCurv();
                public MBZCurv m_mCurv = null;
                public Transform m_parent;

                public Vector3 m_parentOffset;
                public Quaternion m_parentRotation;

                //public MBZCurv m_animationCurv = null;

                public AnimationCurve m_shape = new AnimationCurve();
                public bool m_edit = true;
                public List<Transform> m_bones = new List<Transform>();
                public List<Vector3> m_bonesLastpos = new List<Vector3>();                
                public List<Vector3> m_bonesOriginalPos = new List<Vector3>();
                public List<Quaternion> m_bonesLastRot = new List<Quaternion>();
                public List<Quaternion> m_bonesOriginalRot = new List<Quaternion>();
                public List<Quaternion> m_bonesTmpRot = new List<Quaternion>();
                public List<HairPID_V3> m_bonesPid = new List<HairPID_V3>();
                public List<Vector3> m_bonesInertia = new List<Vector3>();

                public HairPID_V3 m_pid = new HairPID_V3();
                //public Vector3 m_lastPosition;
                public int m_subdivisionX = 5;
                public int m_subdivisionY = 10;
                public int m_strandCount = 1;
                public float m_normalOffset = 0;
                public float m_startOffset = 0;
                public float m_endOffset = 0;
                public int m_rndSeed = 0;
                public int m_UVX = 1;
                public float m_rndStrandLength = 1;
                public float m_normalSwitch = 1;
                public float m_scale = 1f;
                public float m_folding = 0f;
                public float m_waveAmplitude = 0f;
                public float m_wavePeriod = 0f;
                public float m_strandMaxAngle = 180f;
                public float m_bendAngleStart = 0f;
                public float m_bendAngleEnd = 0f;

                public Vector3 m_meshNormal;

                public Vector3 m_motion;
                public Vector3 m_initEndPos;
                public Vector3 m_initStartTan;
                public Vector3 m_initEndTan;
                public Vector3 m_lastEndPos;
                public bool m_dynamic = true;

                public float m_rigidity = 0f;
                public float m_rootRigidity = 0f;
                public float m_gravityFactor = 1f;
                

                public Vector3 m_upReference = Vector3.up;
                
                public bool m_usePhysics = false;
                public bool m_snapToSurface = true;
                public int m_layer = 0;
                public int m_modifierId = -1;

                public void Generate()
                {
                    m_strands.Clear();

                    for (int i=0; i<1; ++i )
                    {
                        //float t = (float)i / 10f;
                        StrandData sd = new StrandData();
                        //sd.normal = m_curv.GetTangent(t);
                        //sd.localpos = m_curv.GetPosition( t );
                        //sd.localpos = m_curv.m_startPosition;
                        //sd.rotation = Quaternion.FromToRotation(Vector3.forward, sd.normal);
                        //sd.curve = m_curv;
                        sd.mCurve = m_mCurv;
                        m_strands.Add(sd);
                    }
                }

                public HairGroup Copy()
                {
                    HairGroup hg = new HairGroup();
                    //hg.m_bones
                    hg.m_mCurv = m_mCurv.Copy();
                    hg.m_parent = m_parent;
                    hg.m_shape = new AnimationCurve(m_shape.keys);
                    hg.m_scale = m_scale;
                    
                    hg.m_strandCount = m_strandCount;
                    hg.m_subdivisionX = m_subdivisionX;
                    hg.m_subdivisionY = m_subdivisionY;
                    hg.m_normalOffset = m_normalOffset;
                    hg.m_startOffset = m_startOffset;
                    hg.m_endOffset = m_endOffset;
                    hg.m_rndSeed = m_rndSeed;
                    hg.m_rndStrandLength = m_rndStrandLength;
                    hg.m_normalSwitch = m_normalSwitch;
                    hg.m_UVX = m_UVX;
                    hg.m_folding = -m_folding;
                    hg.m_waveAmplitude = m_waveAmplitude;
                    hg.m_wavePeriod = -m_wavePeriod;

                    hg.m_strandMaxAngle = m_strandMaxAngle;
                    hg.m_bendAngleStart = m_bendAngleStart;
                    hg.m_bendAngleEnd = m_bendAngleEnd;
                    hg.m_layer = m_layer;
                    hg.m_snapToSurface = m_snapToSurface;
                    hg.m_modifierId = m_modifierId;

                    hg.m_gravityFactor = m_gravityFactor;
                    hg.m_rootRigidity = m_rootRigidity;
                    hg.m_rigidity = m_rigidity;

                    return hg;
                }
            }

            /// <summary>
            /// Bones parameters for motion
            /// </summary>
            [System.Serializable]
            public class MotionBoneData
            {
                public float gravity = .1f;
                public float rootRigidity = .9f;
                public float rigidity = 0f;
                public float elasticity = .1f;
                public float smooth = .1f;
                public bool accurateBoneRotation = true;
                public HairDesignerPID bonePID = new HairDesignerPID(5, 2, 0);
                public float wind = 1f;
            }





            /// <summary>
            /// Mirror modifier for long hair
            /// </summary>
            [System.Serializable]
            public class MirrorModifier : GeneratorModifier
            {
                //public Vector3 pos = Vector3.zero;
                //public Vector3 dir = Vector3.right;

                public enum eMirrorAxis
                {
                    X,
                    Y,
                    Z
                }

                public eMirrorAxis m_axis = eMirrorAxis.X;
                public Vector3 m_offset = Vector3.zero;

                public void Update( HairDesignerGeneratorLongHairBase g )
                {
                    for (int i = 0; i < g.m_groups.Count; ++i)
                    {
                        if (g.m_groups[i].m_modifierId == id)
                        {
                            g.m_groups.RemoveAt(i--);//clean generated by this modifier
                            continue;
                        }
                    }



                    for (int i=0; i<g.m_groups.Count; ++i)
                    {

                        if (g.m_groups[i].m_modifierId != -1 )
                            continue;//don't care about other modifiers

                        if (!layers.Contains(g.m_groups[i].m_layer))
                            continue;//don't care about other layers

                        HairGroup mirrored = g.m_groups[i].Copy();
                        mirrored.m_modifierId = id;
                        mirrored.m_bendAngleStart *= -1f;
                        mirrored.m_bendAngleEnd *= -1f;
                        mirrored.m_startOffset *= -1f;
                        mirrored.m_endOffset *= -1f;
                        mirrored.m_normalOffset *= -1f;
                        mirrored.m_mCurv.m_startAngle *= -1f;
                        mirrored.m_mCurv.m_endAngle *= -1f;

                        mirrored.m_mCurv.Mirror( (MBZCurv.eMirrorAxis)m_axis, m_offset);
                        g.m_groups.Add(mirrored);
                        mirrored.Generate();
                    }
                }


                public void Apply(HairDesignerGeneratorLongHairBase g)
                {
                    Update(g);
                    for (int i = 0; i < g.m_groups.Count; ++i)
                    {
                        if (g.m_groups[i].m_modifierId == id)
                        {
                            g.m_groups[i].m_modifierId = -1;
                        }
                    }
                }

            }


            [System.Serializable]
            public class EditorData
            {
                public float HandlesUIPerspSize = 1f;
                public float HandlesUIOrthoSize = 1f;
            }
            


            //---------------------------------------------------------------------
                #endregion










            public List<HairGroup> m_groups = new List<HairGroup>();            
            public MotionBoneData m_motionData = new MotionBoneData();

            //unity can't serialize polymorphism, so herethe original data for miror modifiers
            public List<MirrorModifier> m_mirrorModifiers = new List<MirrorModifier>();
            public EditorData m_editorData = new EditorData();

            public override StrandRenderingData GetData(int id)
            {
                int c = 0;
                int sId = 0;
                int gId = 0;
                for( int i=0; i< m_groups.Count; ++i )
                {
                    if( id < m_groups[i].m_strands.Count + c )
                    {
                        gId = i;
                        sId = id - c;
                        break;
                    }

                    c += m_groups[i].m_strands.Count;
                }


                m_data.rotation = m_groups[gId].m_strands[sId].rotation;
                m_data.localpos = m_groups[gId].m_strands[sId].localpos;// + m_strands[id].normal * .001f;
                m_data.scale = m_groups[gId].m_strands[sId].scale * m_scale;
                m_data.normal = m_groups[gId].m_strands[sId].normal;
                m_data.strand = m_groups[gId].m_strands[sId];
                m_data.layer = m_groups[gId].m_layer;

                if (m_groups[gId].m_strands[sId].mesh == null)
                {
                    m_currentHairGroup = m_groups[gId];
                    m_groups[gId].m_strands[sId].mesh = GenerateMesh(m_groups[gId].m_strands[sId], null, 0);
                }


                return m_data;
            }


            public override int GetStrandCount()
            {
                int c = 0;
                for (int i = 0; i < m_groups.Count; ++i)
                    c += m_groups[i].m_strands.Count;

                return c;
            }



            public override void DrawHairStrand()
            {


                //Draw hair stand
                HairDesignerBase.InitRandSeed(0);                
                int m_batchingFix = 1;
                for (int i = 0; i < GetStrandCount(); ++i)
                {
                    m_batchingFix++;
                    m_batchingFix = 1;
                    StrandRenderingData dt = GetData(i);

                    if (dt.layer >= m_editorLayers.Count || !m_editorLayers[dt.layer].visible)
                        continue;

                    Vector3 worldPos = m_hd.transform.TransformPoint(dt.localpos + dt.normal * (dt.strand.offset + m_params.m_offset));
                    Matrix4x4 m = Matrix4x4.TRS(worldPos, m_hd.transform.rotation * dt.rotation, m_hd.transform.lossyScale);

                    m_matPropBlkHair.SetFloat("KHD_editor", 0);                    

                    Graphics.DrawMesh(dt.strand.mesh, m, m_hairMeshMaterial, 0, null, 0, m_matPropBlkHair, false, false);
                    if(m_hairMeshMaterialTransparent!=null)
                        Graphics.DrawMesh(dt.strand.mesh, m, m_hairMeshMaterialTransparent, 0, null, 0, m_matPropBlkHair, false, false);
                    
                }


                HairDesignerBase.RestoreRandSeed();
            }


            HairGroup m_currentHairGroup = null;

            /// <summary>
            /// Generate mesh from strand
            /// </summary>
            /// <param name="sd"></param>
            /// <param name="bones"></param>
            /// <param name="startId"></param>
            /// <returns></returns>
            Mesh GenerateMesh( StrandData sd , List<Transform> bones, int startId )
            {               

                Mesh m = new Mesh();

                List<Vector3> v = new List<Vector3>();
                List<Vector3> n = new List<Vector3>();
                List<Vector4> tg = new List<Vector4>();
                List<int> t = new List<int>();
                List<Vector2> uv = new List<Vector2>();
                List<BoneWeight> boneWeights = new List<BoneWeight>();
                List<Color> colors = new List<Color>();

                sd.localpos = Vector3.zero;
                sd.mCurve.SetUpRef(transform.InverseTransformDirection(Vector3.up));

                int X = m_currentHairGroup.m_subdivisionX;
                int Y = m_currentHairGroup.m_subdivisionY;

                Random.InitState(m_currentHairGroup.m_rndSeed);
                int K = m_currentHairGroup.m_strandCount;
                for( int k=0; k<K; ++k )
                {
                    float angle = ((float)k / (float)K) * m_currentHairGroup.m_strandMaxAngle;
                    float rnd = Random.value;
                    Vector3 startOffsetRnd = Random.insideUnitCircle;
                    Vector3 startOffset = startOffsetRnd * m_currentHairGroup.m_startOffset;
                    Vector3 endOffset = startOffsetRnd * m_currentHairGroup.m_endOffset;
                    float rndLength = (1-m_currentHairGroup.m_rndStrandLength) * rnd + m_currentHairGroup.m_rndStrandLength;

                    float nOffset0 = m_currentHairGroup.m_normalOffset * Random.value;
                    float nOffset1 = m_currentHairGroup.m_normalOffset;// * Random.value;
                    nOffset1 = nOffset0;

                    for (int x = 0; x < X; ++x)
                    {
                        for (int y = 0; y < Y; ++y)
                        {
                            //float progress = (float)y / (float)Y;
                            //float progressX = (float)x / (float)X;

                            float XStep = 1f / (float)X;
                            float YStep = 1f / (float)Y;
                            //float f = YStep * y;

                            float t0 = (YStep * y) * rndLength;
                            float t1 = (YStep * y + YStep) * rndLength;

                            //Vector3 dir = Vector3.forward;
                            Vector3 tan0 = sd.mCurve.GetTangent(t0);
                            Vector3 tan1 = sd.mCurve.GetTangent(t1);
                            Vector3 up0 = sd.mCurve.GetUp(t0);
                            Vector3 up1 = sd.mCurve.GetUp(t1);
                            float nOffset = Mathf.Lerp(nOffset0, nOffset1, t0);

                            if (m_currentHairGroup.m_shape.length < 2)
                            {
                                m_currentHairGroup.m_shape.AddKey(new Keyframe(0, 1f));
                                m_currentHairGroup.m_shape.AddKey(new Keyframe(1, 1f));
                            }
                            float taper0 = m_currentHairGroup.m_shape.Evaluate((float)y / (float)Y) * m_scale * strandScaleFactor * m_currentHairGroup.m_scale;
                            float taper1 = m_currentHairGroup.m_shape.Evaluate(((float)y + 1f) / (float)Y) * m_scale * strandScaleFactor * m_currentHairGroup.m_scale;
                            
                            Vector3 pos0 = sd.mCurve.GetPosition(t0) + Vector3.Lerp(startOffset, endOffset, (float)y / (float)Y) * taper0;
                            Vector3 pos1 = sd.mCurve.GetPosition(t1) + Vector3.Lerp(startOffset, endOffset, ((float)y + 1f) / (float)Y) * taper1;

                            Quaternion r0 = Quaternion.AngleAxis(angle, tan0);
                            Quaternion r1 = Quaternion.AngleAxis(angle, tan1);
                            
                            float wave = Mathf.Pow(-1, y) * m_currentHairGroup.m_folding;
                            wave += Mathf.Sin(m_currentHairGroup.m_wavePeriod * Mathf.PI * (float)y/(float)Y) * m_currentHairGroup.m_waveAmplitude;

                            float bendAngle0 = Mathf.Lerp(m_currentHairGroup.m_bendAngleStart, m_currentHairGroup.m_bendAngleEnd, t0);
                            float bendAngle1 = Mathf.Lerp(m_currentHairGroup.m_bendAngleStart, m_currentHairGroup.m_bendAngleEnd, t1);
                            Quaternion tanR0 = Quaternion.AngleAxis(((XStep * x) - .5f) * (-bendAngle0), tan0);
                            Quaternion tanR1 = Quaternion.AngleAxis(((XStep * x) + XStep - .5f) * (-bendAngle0), tan0);
                            Quaternion tanR2 = Quaternion.AngleAxis(((XStep * x) - .5f) * (-bendAngle1), tan1);
                            Quaternion tanR3 = Quaternion.AngleAxis(((XStep * x) + XStep - .5f) * (-bendAngle1), tan1);

                            

                            for (int z = 1; z < 2; z++)
                            {
                                //Add vertices 
                                if (y == 0)
                                {
                                    v.Add(r0 * (tanR0 * (Vector3.Cross(up0, tan0) * ((XStep * x) - .5f) + up0 * wave) * taper0) + pos0 + r0 * up0* nOffset * taper0);
                                    v.Add(r0 * (tanR1 * (Vector3.Cross(up0, tan0) * ((XStep * x) + XStep - .5f) + up0 * wave) * taper0) + pos0 + r0 * up0 * nOffset * taper0);
                                }
                                v.Add(r1 * ( (tanR2 * Vector3.Cross(up1, tan1) * ((XStep * x) - .5f) + up1 * wave ) * taper1) + pos1 + r1 * up1 * nOffset * taper1);
                                v.Add(r1 * ( (tanR3 * Vector3.Cross(up1, tan1) * ((XStep * x) + XStep - .5f) + up1 * wave) * taper1) + pos1 + r1 * up1 * nOffset * taper1);

                                //Add uv
                                float uvx = (float)m_currentHairGroup.m_UVX;
                                if (y == 0)
                                {
                                    uv.Add(new Vector2((XStep * x) * uvx, YStep * y));
                                    uv.Add(new Vector2((XStep * x + XStep) * uvx, YStep * y));
                                }
                                uv.Add(new Vector2((XStep * x) * uvx, YStep * y + YStep));
                                uv.Add(new Vector2((XStep * x + XStep) * uvx, YStep * y + YStep));

                                //Add normals
                                //float n2t = m_params.m_normalToTangent;
                                float n2t = m_currentHairGroup.m_normalSwitch;
                                n2t = 0;

                                //normals
                                if (y == 0)
                                {
                                    n.Add(r0 * tanR0 * Vector3.Lerp(up0,tan0, n2t).normalized * (z == 0 ? 1 : -1));
                                    n.Add(r0 * tanR1 * Vector3.Lerp(up0, tan0, n2t).normalized * (z == 0 ? 1 : -1));
                                }
                                n.Add( r1 * tanR2 * Vector3.Lerp(up1, tan1, n2t).normalized * (z == 0 ? 1 : -1));
                                n.Add( r1 * tanR3 * Vector3.Lerp(up1, tan1, n2t).normalized * (z == 0 ? 1 : -1));
                                 
                                //tangents
                                if (y == 0)
                                {
                                    tg.Add((tan0));
                                    tg.Add((tan0));
                                }
                                tg.Add( (tan1));
                                tg.Add( (tan1));


                                //Colors 
                                if (y == 0)
                                {
                                    colors.Add(new Color(1, 1, 1, rnd));
                                    colors.Add(new Color(1, 1, 1, rnd));
                                }
                                colors.Add(new Color(1, 1, 1, rnd));
                                colors.Add(new Color(1, 1, 1, rnd));



                                //Add triangles
                                if (z == 0)
                                {
                                    t.Add(v.Count - 4);
                                    t.Add(v.Count - 2);
                                    t.Add(v.Count - 3);
                                    t.Add(v.Count - 1);
                                    t.Add(v.Count - 3);
                                    t.Add(v.Count - 2);
                                }
                                else
                                {
                                    t.Add(v.Count - 4);
                                    t.Add(v.Count - 3);
                                    t.Add(v.Count - 2);
                                    t.Add(v.Count - 1);
                                    t.Add(v.Count - 2);
                                    t.Add(v.Count - 3);
                                }



                                if (bones != null)
                                {
                                    int b = (int)(t0 * (float)Y);
                                    //Add bone weights
                                    BoneWeight bw = new BoneWeight();
                                    bw.weight0 = 1f;
                                    if (y == 0)
                                    {
                                        if (b > 0)
                                            bw.boneIndex0 = b + startId - 1;
                                        else
                                            bw.boneIndex0 = b + startId;
                                        boneWeights.Add(bw);
                                        boneWeights.Add(bw);
                                    }
                                    if (bones.Count < b + startId + 1)
                                        bw.boneIndex0 = b + startId + 1;
                                    else
                                        bw.boneIndex0 = b + startId;
                                    boneWeights.Add(bw);
                                    boneWeights.Add(bw);

                                }
                            }

                            

                            

                        }
                    }
                }

                //setup UV from vertices position          
                m.SetVertices(v);
                m.SetNormals(n);
                m.SetTangents(tg);
                m.SetTriangles(t, 0);
                m.SetUVs(0, uv);
                m.SetColors(colors);

                if (bones != null)
                {
                    m.boneWeights = boneWeights.ToArray();

                    Matrix4x4[] bindposes = new Matrix4x4[bones.Count- startId];
                    for( int i= startId; i< bones.Count; ++i )
                    {
                       bindposes[i - startId] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
                    }

                    m.bindposes = bindposes;
                }
                m.name = "HairStrand";
                //m.RecalculateBounds();
                //Bounds b = m.bounds;
                //b.size *= 10f;//force rendering when bound out of Vertex program modification
                //m.bounds = b;
                //m.MarkDynamic();

                //Debug.Log("Hair strand mesh : " + (t.Count / 3) + " tri");

                //m.RecalculateNormals();

                return m;
            }


            List<Transform> m_bones = new List<Transform>();

            public override void CreateHairMesh(Mesh skinRef)
            {
                
                HairDesignerBase.InitRandSeed(0);

                m_hair = new Mesh();

                //float radius = m_startRadius;

                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector4> tangents = new List<Vector4>();
                List<int> triangles = new List<int>();
                List<Vector2> uv = new List<Vector2>();
                List<BoneWeight> boneWeights = new List<BoneWeight>();
                List<Matrix4x4> bindPoses = new List<Matrix4x4>();
                //Matrix4x4[] bindPoses;
                List<Color> colors = new List<Color>();
                m_bones.Clear();
                Transform[] bones = null;
                int startBoneId = 0;

                //add all strand to the mesh


                
                for (int i = 0; i < m_groups.Count; ++i)
                {
                    /*
                    //generate animation curve                    
                    m_groups[i].m_animationCurv = m_groups[i].m_mCurv.Copy();   
                                     
                    for (int c = 0; c < m_groups[i].m_mCurv.m_curves.Length; ++c)
                    {
                        m_groups[i].m_animationCurv.m_curves[c].m_upRef = m_groups[i].m_parent.InverseTransformDirection(m_hd.transform.TransformDirection(m_groups[i].m_mCurv.m_curves[c].m_upRef));
                        m_groups[i].m_animationCurv.m_curves[c].m_startPosition = m_groups[i].m_parent.InverseTransformPoint(m_hd.transform.TransformPoint(m_groups[i].m_mCurv.m_curves[c].m_startPosition));                        
                        m_groups[i].m_animationCurv.m_curves[c].m_startTangent = m_groups[i].m_parent.InverseTransformDirection(m_hd.transform.TransformDirection(m_groups[i].m_mCurv.m_curves[c].m_startTangent));
                        m_groups[i].m_animationCurv.m_curves[c].m_endPosition = m_groups[i].m_parent.InverseTransformPoint(m_hd.transform.TransformPoint(m_groups[i].m_mCurv.m_curves[c].m_endPosition));
                        m_groups[i].m_animationCurv.m_curves[c].m_endTangent = m_groups[i].m_parent.InverseTransformDirection(m_hd.transform.TransformDirection(m_groups[i].m_mCurv.m_curves[c].m_endTangent));


                        //set the animation curve start/end rotation
                        //m_groups[i].m_animationCurv.m_curves[c].m_startAngle = Vector3.Angle(m_groups[i].m_animationCurv.m_curves[c].m_upRef, );

                    }
                    */


                    //generate bones
                    if ( m_GenerateBones )
                    {
                        if (m_editorLayers.Count > m_groups[i].m_layer && !m_editorLayers[m_groups[i].m_layer].visible)
                            continue;

                        bones = GenerateBones(i, m_groups[i].m_subdivisionY);
                        if (bones.Length > 0)
                        {
                            bones[0].parent = m_groups[i].m_parent;
                            bones[0].localScale = Vector3.one;
                        }
                            
                        startBoneId = m_bones.Count;
                        for (int b = 0; b < bones.Length; ++b)
                            m_bones.Add(bones[b]);      
                        
                                          
                    }

                    //generate mesh
                    for (int g = 0; g < m_groups[i].m_strands.Count; ++g)
                    {
                        if (m_editorLayers.Count > m_groups[i].m_layer && !m_editorLayers[m_groups[i].m_layer].visible)
                            continue;

                        //if (m_groups[i].m_strands[g].mesh == null)
                        {
                            m_currentHairGroup = m_groups[i];
                            m_groups[i].m_strands[g].mesh = GenerateMesh(m_groups[i].m_strands[g], m_bones, startBoneId);
                        }

                        if (vertices.Count + m_groups[i].m_strands[g].mesh.vertexCount > 65000)
                        {
                            Debug.LogWarning("Layer '" + m_name + "' Too much vertices " + (vertices.Count + m_groups[i].m_strands[g].mesh.vertexCount));
                            break;
                        }

                        int[] strandTriangles = m_groups[i].m_strands[g].mesh.triangles;
                        Vector3[] strandVertices = m_groups[i].m_strands[g].mesh.vertices;
                        Vector3[] strandNormals = m_groups[i].m_strands[g].mesh.normals;
                        Vector4[] strandTangents = m_groups[i].m_strands[g].mesh.tangents;
                        Vector2[] strandUV = m_groups[i].m_strands[g].mesh.uv;
                        BoneWeight[] strandBoneWeight = m_groups[i].m_strands[g].mesh.boneWeights;
                        Matrix4x4[] strandBindPose = m_groups[i].m_strands[g].mesh.bindposes;
                        Color[] strandColors = m_groups[i].m_strands[g].mesh.colors;

                        for (int t = 0; t < strandTriangles.Length; ++t)
                            triangles.Add(strandTriangles[t] + vertices.Count);

                        Vector2 _uvRnd = new Vector2(Random.Range(0, m_atlasSizeX), Random.Range(0, m_atlasSizeX)) / m_atlasSizeX;

                        for (int v = 0; v < strandVertices.Length; ++v)
                        {
                            vertices.Add(strandVertices[v]);
                            //colors.Add(new Color(1, 1, 1, m_scale));
                            colors.Add(strandColors[v]);
                            if(strandTangents.Length>v)
                                tangents.Add(strandTangents[v]);

                            uv.Add(strandUV[v] / m_atlasSizeX + _uvRnd);

                            if (m_GenerateBones)
                            {
                                boneWeights.Add(strandBoneWeight[v]);
                            }
                        }

                        for (int n = 0; n < strandNormals.Length; ++n)
                            normals.Add(strandNormals[n]);


                        if (m_GenerateBones)
                        {
                            for (int bp = 0; bp < strandBindPose.Length; ++bp)
                                bindPoses.Add(strandBindPose[bp]);                            
                        }
                    }
                }



                m_hair.vertices = vertices.ToArray();
                m_hair.normals = normals.ToArray();//TODO Set up normal
                m_hair.tangents = tangents.ToArray();
                m_hair.triangles = triangles.ToArray();
                m_hair.uv = uv.ToArray();
                m_hair.colors = colors.ToArray();
                m_hair.name = "HairDesignerInstance";

                if (m_GenerateBones)
                {
                    m_hair.boneWeights = boneWeights.ToArray();
                    m_hair.bindposes = bindPoses.ToArray();
                }

                HairDesignerBase.RestoreRandSeed();
                m_hair.RecalculateBounds();
                //m_hair.RecalculateNormals();
            }




            bool m_GenerateBones = false;            

            public override void Start()
            {
                base.Start();
                                
                /*
                for (int i = 0; i < m_colliders.Count; ++i)
                    for (int j = i+1; j < m_colliders.Count; ++j)                                        
                        {
                            Physics.IgnoreCollision(m_colliders[i], m_colliders[j]);
                            //Debug.Log("IgnoreCollision : "+ m_colliders[i].name + " " + m_colliders[j].name);
                        }
                        */
                
            }


            //public Vector3 m_lastPosition;
            public override void UpdateInstance()
            {
                if (this == null)//bug when destroy layer in editor
                    return;

                base.UpdateInstance();


                //return;

                if (!Application.isPlaying)
                    return;

                
            }



            
            public List<Vector3[]> m_capsuleSphereCenters = new List<Vector3[]>();


            public override void LateUpdateInstance()
            {

                if (!m_enable)
                    return;




                //update capsule position
                for (int c = 0; c < m_hd.m_capsuleColliders.Count; ++c)
                {
                    if (m_capsuleSphereCenters.Count == c)
                        m_capsuleSphereCenters.Add(new Vector3[2]);

                    for (int k = 0; k < 2; ++k)
                    {
                        m_capsuleSphereCenters[c][k] = m_hd.m_capsuleColliders[c].transform.position;// + m_capsuleColliders[c].transform.TransformPoint(m_capsuleColliders[c].center);
                        float scl = m_hd.m_capsuleColliders[c].transform.lossyScale.x;
                        float h = m_hd.m_capsuleColliders[c].height * .5f > m_hd.m_capsuleColliders[c].radius ?
                                    m_hd.m_capsuleColliders[c].height * .5f  - m_hd.m_capsuleColliders[c].radius : 0;
                        h *= scl;

                        switch (m_hd.m_capsuleColliders[c].direction)
                        {
                            case 0: m_capsuleSphereCenters[c][k] += m_hd.m_capsuleColliders[c].transform.right * h * (k==0?1:-1); break;
                            case 1: m_capsuleSphereCenters[c][k] += m_hd.m_capsuleColliders[c].transform.up * h * (k == 0 ? 1 : -1); break;
                            case 2: m_capsuleSphereCenters[c][k] += m_hd.m_capsuleColliders[c].transform.forward * h * (k == 0 ? 1 : -1); break;
                        }

                        //Debug.DrawLine(m_capsuleSphereCenters[c][k], m_capsuleSphereCenters[c][k] + Vector3.up * m_capsuleColliders[c].radius* scl, k==0? Color.red:Color.blue);
                    }
                }


                float gFactor = m_motionData.gravity;
               
                float elasticity = m_motionData.elasticity;
                bool fixBoneRotation = m_motionData.accurateBoneRotation;
                
                //Update bones motion
                for (int i = 0; i < m_groups.Count; ++i)
                {
                    if (m_groups[i].m_dynamic)
                    {
                        //Vector3[] initPos = new Vector3[m_groups[i].m_bones.Count];

                        
                        //fix bone rotation
                        if (fixBoneRotation && m_groups[i].m_bonesOriginalPos.Count > 0)
                        {
                            for (int b = 1; b < m_groups[i].m_bones.Count; ++b)
                            //for (int b = m_groups[i].m_bones.Count-5; b < m_groups[i].m_bones.Count; ++b)
                            {
                                m_groups[i].m_bones[b].parent.localRotation = m_groups[i].m_bonesTmpRot[b - 1];
                                m_groups[i].m_bones[b].position = m_groups[i].m_bonesLastpos[b];
                            }
                        }

                        for (int b = 0; b < m_groups[i].m_bones.Count; ++b)
                        {
                            float f = (float)b / (float)m_groups[i].m_bones.Count;
                            float distFromParent = 0f;

                            

                            if (b > 0)
                            {
                                distFromParent = Vector3.Distance(m_groups[i].m_bones[b - 1].position, m_groups[i].m_bones[b].position);
                            }

                            if (m_groups[i].m_bonesLastpos.Count == b)
                            {
                                //initialize tmp data
                                m_groups[i].m_bonesOriginalPos.Add(m_groups[i].m_bones[b].localPosition);
                                m_groups[i].m_bonesOriginalRot.Add(m_groups[i].m_bones[b].localRotation);
                                m_groups[i].m_bonesTmpRot.Add(m_groups[i].m_bones[b].localRotation);
                                m_groups[i].m_bonesLastRot.Add(m_groups[i].m_bones[b].localRotation);

                                m_groups[i].m_bonesLastpos.Add(m_groups[i].m_bones[b].position);
                                m_groups[i].m_bonesInertia.Add(Vector3.zero);
                                m_groups[i].m_bonesPid.Add(new HairPID_V3());
                                m_groups[i].m_bonesPid[b].m_params = m_motionData.bonePID.m_params;
                                m_groups[i].m_bonesPid[b].m_params.limits = new Vector2(-distFromParent, distFromParent) * .5f;
                                m_groups[i].m_bonesPid[b].m_target = Vector3.zero;
                                m_groups[i].m_bonesPid[b].Init();
                                m_groups[i].m_bonesLastpos[b] = m_groups[i].m_bones[b].position;
                            }


                            /*
                            if (b % 2 == 0)//could be done for computing 1/2 bones
                            {
                                m_groups[i].m_bonesLastpos[b] = m_groups[i].m_bones[b].position;
                                continue;
                            }*/

                            if (b == 0)
                            {
                                m_groups[i].m_bonesLastpos[b] = m_groups[i].m_bones[b].position;
                                continue;
                            }

                            m_groups[i].m_bones[b].transform.localRotation = m_groups[i].m_bonesLastRot[b];

                            Quaternion oldRotation = m_groups[i].m_bones[b].transform.localRotation;

                            Vector3 delta = m_groups[i].m_bonesLastpos[b] - m_groups[i].m_bones[b].position;

                            //centrifugal force
                            //delta += Vector3.Normalize(m_groups[i].m_bones[b].position - m_groups[i].m_bones[b].parent.position) * delta.magnitude * 2f;


                            //clamp the distance with the max distance from root
                            delta = delta.normalized * Mathf.Clamp(delta.magnitude, 0, f * distFromParent * (float)m_groups[i].m_bones.Count );

                            m_groups[i].m_bonesPid[b].m_target = delta;// + m_groups[i].m_bonesInertia[b-1];
                            m_groups[i].m_bonesInertia[b] = m_groups[i].m_bonesPid[b].Compute(Vector3.zero);
                                                        
                            Vector3 newPos = m_groups[i].m_bones[b - 1].TransformPoint(m_groups[i].m_bonesOriginalPos[b]) + m_groups[i].m_bonesInertia[b];

                            Vector3 externForces = Physics.gravity * gFactor * m_groups[i].m_gravityFactor;

                            if (m_hd.m_windZone != null && m_hd.m_windZone.gameObject.activeSelf)
                            {
                                
                                //wind force (same as shader)
                                externForces += m_motionData.wind * m_hd.m_windZone.transform.forward * m_hd.m_windZone.windMain * m_hd.globalScale;
                                externForces += m_hd.m_windZone.transform.forward *                                    
                                    ( 
                                        Mathf.Sin(( m_hd.m_windZone.windPulseFrequency+ (float)m_groups[i].m_rndSeed%100f*.001f) * Time.time * 10f ) * 2f
                                      + Mathf.Cos((m_hd.m_windZone.windPulseFrequency + (float)m_groups[i].m_rndSeed%100f*.001f) * Time.time * 20f) * .1f 
                                    )
                                    * m_hd.m_windZone.windPulseMagnitude * m_hd.m_windZone.windMain * .1f * m_hd.globalScale;
                            }

                            //Compute gravity with rotation
                            //m_groups[i].m_bones[b].transform.localRotation = m_groups[i].m_bonesOriginalRot[b];
                            if (delta.magnitude < externForces.magnitude )                            
                            {
                                //float fg = gFactor;// ( 1f - delta.magnitude / externForces.magnitude * gFactor * m_groups[i].m_gravityFactor);
                                Quaternion r = m_groups[i].m_bones[b].transform.localRotation;

                                //Vector3 up = Vector3.Lerp( m_groups[i].m_bones[b - 1].transform.up, -externForces.normalized, fg );
                                Vector3 up = m_groups[i].m_bones[b-1].transform.up;

                                
                                m_groups[i].m_bones[b].transform.LookAt(m_groups[i].m_bones[b].transform.position + (externForces).normalized, up);// m_groups[i].m_bones[b-1].transform.up );
                                r = Quaternion.Slerp(r, m_groups[i].m_bones[b].transform.localRotation, gFactor * (1f-f) );
                                m_groups[i].m_bones[b].transform.localRotation = r;
                                
                               


                                //m_groups[i].m_bones[b].transform.rotation = Quaternion.Lerp( r, m_groups[i].m_bones[b].transform.rotation, Time.deltaTime);
                            }
                            

                                                   

                            //float dist = m_groups[i].m_bones[b].localPosition.magnitude;

                            //fix : capsule collider position                            
                            for (int c = m_hd.m_capsuleColliders.Count-1; c >= 0 ; --c) // first colliders are more important, last of the list are interraction colliders
                            {
                                float scl = m_hd.m_capsuleColliders[c].transform.lossyScale.x;

                                if (!m_hd.m_capsuleColliders[c].enabled)
                                    continue;

                                
                                //test capsule cylinder
                                Vector3 dir = (m_capsuleSphereCenters[c][1] - m_capsuleSphereCenters[c][0]).normalized;
                                float dot = Vector3.Dot(dir, newPos - m_capsuleSphereCenters[c][0]);
                                Vector3 proj = m_capsuleSphereCenters[c][0] + dir * dot;

                                //bool collided = false;

                                //check cylinder radius
                                if (Vector3.Distance(proj, newPos) < m_hd.m_capsuleColliders[c].radius * scl)
                                {
                                    //check cylinder length
                                    if (dot > 0 && dot < (m_hd.m_capsuleColliders[c].height - m_hd.m_capsuleColliders[c].radius * 2f) * scl)
                                    {                                        
                                        newPos = (newPos - proj).normalized * m_hd.m_capsuleColliders[c].radius * scl + proj;
                                        //collided = true;
                                    }
                                }


                                //test each sphere
                                for (int k = 0; k < 2; ++k)
                                {
                                    if (Vector3.Distance(m_capsuleSphereCenters[c][k], newPos) < m_hd.m_capsuleColliders[c].radius * scl)
                                    {
                                        newPos = (newPos - m_capsuleSphereCenters[c][k]).normalized * m_hd.m_capsuleColliders[c].radius * scl + m_capsuleSphereCenters[c][k];                                        
                                        //collided = true;
                                    }
                                }
                            }


                            //clamp max delta : avoid instant position change
                            delta = newPos - m_groups[i].m_bones[b].position;
                            //delta = delta.normalized * Mathf.Clamp(delta.magnitude, 0f, Time.deltaTime * .1f);

                            //apply motion
                            m_groups[i].m_bones[b].position += delta;




                            //spring effect
                            if (m_groups[i].m_bones[b].localPosition.magnitude  <= m_groups[i].m_bonesOriginalPos[b].magnitude * (1 -elasticity) )
                                m_groups[i].m_bones[b].localPosition = m_groups[i].m_bones[b].localPosition.normalized * m_groups[i].m_bonesOriginalPos[b].magnitude * ( 1- elasticity);// * hairLengthFactor;
                            if( m_groups[i].m_bones[b].localPosition.magnitude  > m_groups[i].m_bonesOriginalPos[b].magnitude * (1 + elasticity) )
                                m_groups[i].m_bones[b].localPosition = m_groups[i].m_bones[b].localPosition.normalized * m_groups[i].m_bonesOriginalPos[b].magnitude * (1 + elasticity);// * hairLengthFactor;
                             
                                                                                   
                            //smooth root value to keep haircut shape
                            float rootMotion = (1-m_motionData.rootRigidity) * (1f-m_groups[i].m_rootRigidity* m_groups[i].m_rootRigidity);
                            float motionFactor = (1f-m_motionData.rigidity) * (1-m_groups[i].m_rigidity);
                            m_groups[i].m_bones[b].localPosition = Vector3.Lerp(m_groups[i].m_bonesOriginalPos[b], m_groups[i].m_bones[b].localPosition, ( f*(1f-rootMotion) + rootMotion) * motionFactor);
                            m_groups[i].m_bones[b].localRotation = Quaternion.Lerp(m_groups[i].m_bonesOriginalRot[b], m_groups[i].m_bones[b].localRotation, (f * (1f-rootMotion) + rootMotion) * motionFactor);
                            m_groups[i].m_bones[b].parent.localRotation = Quaternion.Lerp(m_groups[i].m_bonesOriginalRot[b-1], m_groups[i].m_bones[b].parent.localRotation, (f * (1f-rootMotion) + rootMotion) * motionFactor);


                            //max angle rotation : smooth angle rotation
                            float a = Quaternion.Angle(m_groups[i].m_bones[b].transform.localRotation, oldRotation);
                            float maxAnglePerSec = 20f;
                            if (a > maxAnglePerSec * Time.deltaTime)
                            {
                                Quaternion r = Quaternion.Slerp(oldRotation, m_groups[i].m_bones[b].transform.localRotation, maxAnglePerSec / (a / Time.deltaTime));
                                m_groups[i].m_bones[b].transform.localRotation = r;
                            }

                            m_groups[i].m_bonesLastRot[b] = m_groups[i].m_bones[b].transform.localRotation;

                            //Global smooth
                            m_groups[i].m_bones[b].localPosition = Vector3.Lerp(m_groups[i].m_bones[b].localPosition, m_groups[i].m_bones[b-1].InverseTransformPoint( m_groups[i].m_bonesLastpos[b]), m_motionData.smooth * rootMotion );

                            m_groups[i].m_bonesLastpos[b] = m_groups[i].m_bones[b].position;
                        }

                        if (fixBoneRotation)
                        {
                            //fix bone rotation                        
                            for (int b = 1; b < m_groups[i].m_bones.Count; ++b)                            
                            {
                                m_groups[i].m_bonesTmpRot[b - 1] = m_groups[i].m_bones[b].parent.localRotation;
                                m_groups[i].m_bones[b].parent.LookAt(m_groups[i].m_bonesLastpos[b], m_groups[i].m_bones[b].parent.up);
                                m_groups[i].m_bones[b].position = m_groups[i].m_bonesLastpos[b];
                            }
                        }
                        

                        


                        
                    }
                }
            }


            public override void GenerateMeshRenderer()
            {
                if (GetStrandCount() == 0)
                    return;

                if (m_hd == null || m_meshInstance != null || _editorDelete)
                    return;

                if (m_matPropBlkHair == null)
                    m_matPropBlkHair = new MaterialPropertyBlock();

                DestroyMesh();
                //m_colliders.Clear();

                //SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();
                //m_enableSkinMesh = smr != null;
                //generate HairMesh                

                m_meshInstance = new GameObject(m_name);
                m_meshInstance.transform.SetParent(m_hd.transform, true);
                m_meshInstance.transform.localRotation = Quaternion.identity;
                m_meshInstance.transform.localPosition = Vector3.zero;
                m_meshInstance.transform.localScale = Vector3.one;
                //m_mesh = go.AddComponent<HairDesignerMesh>();

                
                m_skinnedMesh = m_meshInstance.AddComponent<SkinnedMeshRenderer>();
                m_GenerateBones = true;
                CreateHairMesh(null);
                m_GenerateBones = false;
                m_skinnedMesh.bones = m_bones.ToArray();
                m_skinnedMesh.rootBone = m_skinnedMesh.transform;
                m_skinnedMesh.sharedMesh = m_hair;
                m_skinnedMesh.material = m_hairMeshMaterial;



                

                //m_skinnedMesh.bones = smr.bones;
                //m_skinnedMesh.rootBone = smr.rootBone;                                
            }



            


            Transform[] GenerateBones( int groupId, int nb )
            {
                MBZCurv c = m_groups[groupId].m_mCurv;
                Transform[] bones = new Transform[nb];
                //generate bones
                Transform parent = m_skinnedMesh.transform;

                m_groups[groupId].m_bones.Clear();

                Vector3[] initPos = new Vector3[nb];

                for (int i = 0; i < nb; ++i)
                {
                    bones[i] = new GameObject("HairDesigner_"+this.m_name+"_bone " + (m_bones.Count + i)).transform;
                    bones[i].hideFlags = HideFlags.HideInHierarchy;
                    m_groups[groupId].m_bones.Add(bones[i]);
                    bones[i].parent = parent;
                    bones[i].localScale = Vector3.one;
                    parent = bones[i].transform;
                    //bones[i].position = m_skinnedMesh.transform.TransformPoint( c.GetPosition((float)i / (float)nb ));
                    //bones[i].rotation = Quaternion.LookRotation(m_skinnedMesh.transform.TransformDirection(c.GetTangent((float)i / (float)nb) ), m_skinnedMesh.transform.up );
                    bones[i].position = m_hd.transform.TransformPoint(c.GetPosition((float)i / (float)nb));
                    //bones[i].rotation = Quaternion.LookRotation(m_hd.transform.TransformDirection(c.GetTangent((float)i / (float)nb) ), m_hd.transform.up );
                    //bones[i].rotation = Quaternion.LookRotation(m_hd.transform.TransformDirection(c.GetTangent((float)i / (float)nb)), Vector3.up);
                    bones[i].rotation = Quaternion.LookRotation(m_hd.transform.TransformDirection(c.GetTangent((float)i / (float)nb)), m_hd.transform.TransformDirection(c.GetUp((float)i / (float)nb)));

                    initPos[i] = bones[i].position;
                }


                //fix bone positions
                for (int i = 1; i < bones.Length; ++i)
                {
                    bones[i].parent.LookAt(initPos[i], bones[i].parent.up);
                    bones[i].position = initPos[i];
                }
                
                
                return bones;
            }



            /// <summary>
            /// PaintTool selection
            /// </summary>
            /// <param name="data"></param>
            /// <param name="bt"></param>
            public override void PaintTool(StrandData data, BrushToolData bt)
            {
                switch (bt.tool)
                {
                    case ePaintingTool.ADD:
                        AddGroup(data, bt);
                        break;
                }
            }



            public int m_referenceId = -1;
            void AddGroup(StrandData data, BrushToolData bt)
            {
                HairGroup hg;

                //if (m_groups.Count == 0)
                {
                    hg = new HairGroup();
                    hg.m_mCurv = new MBZCurv();
                    hg.m_scale = .01f;
                    hg.m_mCurv.startPosition = data.localpos;
                    hg.m_mCurv.endPosition = hg.m_mCurv.startPosition + data.normal * m_scale * 20f * hg.m_scale / m_hd.globalScale;
                    hg.m_mCurv.startTangent = data.normal * 10f * m_scale * hg.m_scale / m_hd.globalScale;
                    hg.m_mCurv.endTangent = -data.normal * 10f * m_scale * hg.m_scale / m_hd.globalScale;
                    hg.m_layer = data.layer;
                }
                /*
                else 
                {
                    if (m_referenceId < 0 || m_referenceId >= m_groups.Count)
                        m_referenceId = m_groups.Count - 1;

                    hg = m_groups[m_referenceId].Copy();
                    Vector3 offset = data.localpos - m_groups[m_referenceId].m_mCurv.startPosition;
                    Quaternion q = Quaternion.FromToRotation(m_groups[m_referenceId].m_meshNormal, data.normal);

                    hg.m_mCurv.startPosition += offset;
                    hg.m_mCurv.endPosition += q*offset;
                    hg.m_mCurv.startTangent = q * hg.m_mCurv.startTangent;
                    hg.m_mCurv.endTangent = q * hg.m_mCurv.endTangent;

                }
                */
                hg.m_meshNormal = data.normal;
                hg.m_parent = transform;
                m_groups.Add(hg);
                m_referenceId = m_groups.Count - 1;
                hg.Generate();
            }



            public override void DestroyMesh()
            {
                //Destroy all bones
                if (m_meshInstance != null)
                {
                    for (int i = 0; i < m_groups.Count; ++i)
                    {
                        for (int j = m_groups[i].m_bones.Count - 1; j >= 0; --j)
                        {
                            if (m_groups[i].m_bones[j] != null)
                            {
                                if (Application.isPlaying)
                                    Destroy(m_groups[i].m_bones[j].gameObject);
                                else
                                    DestroyImmediate(m_groups[i].m_bones[j].gameObject);
                                m_groups[i].m_bones[j] = null;
                            }
                        }
                        m_groups[i].m_bones.Clear();
                    }
                }
                base.DestroyMesh();
            }

        }
    }
}
