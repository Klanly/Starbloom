using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        /// <summary>
        /// Generator for painting strands on mesh
        /// </summary>
        [System.Serializable]
        public class HairDesignerGeneratorMeshBase : HairDesignerGenerator
        {

            public List<StrandData> m_strands = new List<StrandData>();

            public float m_strandSpacing = .01f;
            


            public override StrandRenderingData GetData(int id)
            {
                m_data.rotation = m_strands[id].rotation;
                m_data.localpos = m_strands[id].localpos;// + m_strands[id].normal * .001f;
                m_data.scale = m_strands[id].scale * m_scale;
                m_data.normal = m_strands[id].normal;
                m_data.strand = m_strands[id];
                m_data.layer = m_strands[id].layer;
                return m_data;
            }

            public override int GetStrandCount()
            {
                return m_strands.Count;
            }

            /*
            public float strandScaleFactor
            {
                get
                {
                    if( m_hd == null )
                    {
                        Debug.LogWarning("HairDesigner instance not set");
                        return 1;
                    }

                    MeshRenderer mr = m_hd.GetComponent<MeshRenderer>();
                    SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();

                    if (mr != null)
                        return mr.bounds.size.magnitude * 0.02f / m_hd.globalScale;
                    if (smr != null)
                        return smr.bounds.size.magnitude * 0.02f / m_hd.globalScale;

                    return 1;
                }
            }*/


            public override void InitEditor()
            {
                base.InitEditor();

                /*
                MeshRenderer mr = m_hd.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();

                if (mr != null)
                    m_scale = mr.bounds.size.magnitude * 0.03f / m_hd.globalScale;
                if (smr != null)
                    m_scale = smr.bounds.size.magnitude * 0.03f / m_hd.globalScale;

                Debug.Log("scale " + m_scale + "  global " + m_hd.globalScale);
                */

                m_scale = strandScaleFactor * .01f;
                m_strandSpacing = m_scale / 4f;
            }



            public override void PaintTool(StrandData data, BrushToolData bt)
            {
                m_needRefresh = true;
                switch (bt.tool)
                {
                    case ePaintingTool.ADD:
                        AddStrand(data, bt);
                        /*
                        if( !bt.secondaryMode )
                            BrushTool(data,bt);//brush on add strand*/
                        break;
                    case ePaintingTool.BRUSH:
                        BrushTool(data, bt);
                        break;
                    case ePaintingTool.SCALE:
                        ScaleTool(data, bt);
                        break;
                }

            }


            //public override void AddTool(StrandpaintingData data, bool erase)
            void AddStrand(StrandData data, BrushToolData bt)
            {
                bool erase = bt.CtrlMode;

                if (!erase)
                {
                    //if (m_strands.Count < 2000)
                    {
                        bool ok = true;
                        for (int i = 0; i < m_strands.Count; ++i)
                            if (Vector3.Distance(m_strands[i].localpos, data.localpos) < m_strandSpacing * (1.5 - bt.brushIntensity) )
                                ok = false;
                        if (ok)
                        {
                            //Quaternion q = Quaternion.LookRotation(bt.brushDir, Quaternion.Euler(0f, 0f, 90f) * data.normal);

                            Vector3 dir = bt.transform.InverseTransformDirection(bt.brushScreenDir);

                            //dir -= Vector3.Dot(dir, data.cam.transform.forward ) * data.cam.transform.forward;

                            float maxAngle = 80f;

                            if (Vector3.Angle(dir, data.normal) > maxAngle)
                            {
                                dir = Quaternion.AngleAxis(maxAngle, Vector3.Cross(data.normal, dir)) * data.normal;
                            }

                            Quaternion q = Quaternion.LookRotation(dir, Quaternion.Euler(0f, 0f, 90f) * data.normal);
                            q = Quaternion.LookRotation(q * Vector3.forward, data.normal);
                            data.rotation = q;

                            float brushWeight = bt.GetBrushWeight(bt.transform.TransformPoint(data.localpos), bt.transform.TransformDirection(data.normal));// * bt.brushIntensity;

                            data.scale *= ( brushWeight * .5f + .5f ) * bt.brushScale;
                            //data.rotation = Quaternion.Lerp(data.rotation, q, .5f);
                            

                            m_strands.Add(data);
                        }
                    }
                }
                else
                {
                    //List<int> idToDelete=new List<int>();
                    for (int i = 0; i < m_strands.Count; ++i)
                        //if (Vector3.Distance(m_strands[i].localpos, data.localpos) < m_strandSpacing)
                        if (bt.GetBrushWeight(bt.transform.TransformPoint(m_strands[i].localpos), bt.transform.TransformDirection(m_strands[i].normal)) > 0)
                        {
                            if (data.layer != m_strands[i].layer)
                                continue;
                            m_strands.RemoveAt(i);
                            i--;
                        }
                }



            }

            void BrushTool(StrandData data, BrushToolData bt)
            {
                List<StrandData> selected = null;
                List<float> weights = null;
                Vector3 globalDir = Vector3.zero;

                if (bt.ShiftMode)
                {
                    selected = new List<StrandData>();
                    weights = new List<float>();
                }

                for (int i = 0; i < m_strands.Count; ++i)
                {
                    //float brushDistance = data.cam.
                    if (Vector3.Dot(bt.transform.TransformDirection(m_strands[i].normal).normalized, bt.cam.transform.forward) > 0)
                        continue;

                    if (data.layer != m_strands[i].layer)
                        continue;

                    float brushWeight = bt.GetBrushWeight(bt.transform.TransformPoint(m_strands[i].localpos), bt.transform.TransformDirection(m_strands[i].normal)) * bt.brushIntensity;
                    if (brushWeight > 0)
                    {
                        if (bt.ShiftMode)
                        {
                            selected.Add(m_strands[i]);
                            weights.Add(brushWeight);
                            globalDir += m_strands[i].rotation * Vector3.forward;
                        }
                        else
                        {
                            Quaternion newRot = m_strands[i].rotation;
                            if (!bt.CtrlMode)
                            {
                                Vector3 dir = bt.transform.InverseTransformDirection(bt.brushScreenDir);
                                float maxAngle = 80f;
                                if (Vector3.Angle(dir, m_strands[i].normal) > maxAngle)
                                {
                                    dir = Quaternion.AngleAxis(maxAngle, Vector3.Cross(m_strands[i].normal, dir)) * m_strands[i].normal;
                                }
                                Quaternion q = Quaternion.LookRotation(dir, Quaternion.Euler(0f, 0f, 90f) * m_strands[i].normal);
                                newRot = Quaternion.Lerp(m_strands[i].rotation, q, .05f);
                                newRot = Quaternion.LookRotation(newRot * Vector3.forward, m_strands[i].normal);
                            }
                            else
                            {
                                newRot = Quaternion.Lerp(m_strands[i].rotation, Quaternion.FromToRotation(m_strands[i].rotation * Vector3.forward, m_strands[i].normal) * m_strands[i].rotation, .1f * brushWeight);
                            }

                            m_strands[i].rotation = Quaternion.Lerp(m_strands[i].rotation, newRot, brushWeight);
                        }
                    }
                }

                if (bt.ShiftMode)
                {
                    globalDir.Normalize();
                    for (int i = 0; i < selected.Count; ++i)
                    {             
                        Quaternion q = Quaternion.LookRotation(globalDir, selected[i].normal);
                        selected[i].rotation = Quaternion.Lerp(selected[i].rotation, q, weights[i] * .1f);
                    }
                }
            }


            void ScaleTool(StrandData data, BrushToolData bt)
            {
                List<StrandData> selected = null;
                List<float> weights = null;
                float globalScale = 0f;
                if (bt.ShiftMode)
                {
                    selected = new List<StrandData>();
                    weights = new List<float>();
                }
                for (int i = 0; i < m_strands.Count; ++i)
                {
                    if (Vector3.Dot(bt.transform.TransformDirection(m_strands[i].normal).normalized, bt.cam.transform.forward) > 0)
                        continue;

                    if (data.layer != m_strands[i].layer)
                        continue;

                    float brushWeight = bt.GetBrushWeight(bt.transform.TransformPoint(m_strands[i].localpos), bt.transform.TransformDirection(m_strands[i].normal)) * bt.brushIntensity;
                    if (brushWeight > 0)
                    {
                        if (bt.ShiftMode)
                        {
                            selected.Add(m_strands[i]);
                            weights.Add(brushWeight);
                            globalScale += m_strands[i].scale;
                        }
                        else
                        {
                            m_strands[i].scale += (bt.CtrlMode ? -.02f : .02f) * brushWeight;
                            m_strands[i].scale = Mathf.Clamp(m_strands[i].scale, 0.1f, m_strands[i].scale);
                        }
                    }
                }

                if (bt.ShiftMode)
                {
                    globalScale /= (float)selected.Count;
                    for (int i = 0; i < selected.Count; ++i)
                    {
                        selected[i].scale = Mathf.Lerp(selected[i].scale, globalScale, weights[i] * .1f);
                    }
                }
            }



        }
    }
}