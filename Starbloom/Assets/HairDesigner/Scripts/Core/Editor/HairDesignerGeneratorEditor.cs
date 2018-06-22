using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [CustomEditor(typeof(HairDesignerGenerator), true)]
        public class HairGeneratorEditor : Editor
        {

            public bool guiChanged = false;
            public bool hideWireframe = false;
            //public EditorSelectedRenderState renderState = EditorSelectedRenderState.Highlight;
            public HairDesignerEditor m_hairDesignerEditor = null;

            
            public static int m_currentEditorLayer = 0;

            //brush parameters
            protected float m_brushFalloff = .5f;            
            protected float m_brushIntensity = .5f;
            protected float m_brushScale = 1f;
            protected bool m_CtrlMode = false;
            protected bool m_ShiftMode = false;
            protected bool m_AltMode = false;
            protected float m_brushSize = .1f;
            protected float m_brushRadius = .1f;
            protected float m_constUnit = 1;
            protected float m_brushDensity = .1f;
            protected float m_pixelRange = 1f;


#if UNITY_5_6_OR_NEWER
            protected Handles.CapFunction RectangleCap = Handles.RectangleHandleCap;
            protected Handles.CapFunction CircleCap = Handles.CircleHandleCap;
            protected Handles.CapFunction SphereCap = Handles.SphereHandleCap;
            protected Handles.CapFunction ArrowCap = Handles.ArrowHandleCap;
#else
            protected Handles.DrawCapFunction RectangleCap = Handles.RectangleCap;
            protected Handles.DrawCapFunction CircleCap = Handles.CircleCap;
            protected Handles.DrawCapFunction SphereCap = Handles.SphereCap;
            protected Handles.DrawCapFunction ArrowCap = Handles.ArrowCap;
#endif


            public override void OnInspectorGUI()
            {
                
                GUI.color = Color.white;
                HairDesignerGenerator g = target as HairDesignerGenerator;

                if (g == null) return;

                if (PrefabUtility.GetPrefabParent(g.gameObject) == null && PrefabUtility.GetPrefabObject(g.gameObject) != null)
                {
                    GUILayout.Label("Instantiate the prefab for modifications", EditorStyles.helpBox);
                    return;
                }


                if (g.m_hd == null)
                {
                    if (g.m_meshInstance != null)
                    {
                        DestroyImmediate(g.m_meshInstance);
                        g.m_meshInstance = null;
                    }

                    DestroyImmediate(g);                                        
                    return;
                }



                m_currentEditorLayer = Mathf.Clamp(m_currentEditorLayer, 0, g.m_editorLayers.Count-1);

                //GUILayout.Label("Name", GUILayout.MaxWidth(100));
                //GUILayout.Space(10);
                //GUILayout.Label("Edit layer", EditorStyles.centeredGreyMiniLabel);
                GUILayout.Space(-10);
                GUILayout.Label(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.SEPARATOR));
                GUILayout.Space(-6);

                GUILayout.BeginHorizontal(HairDesignerEditor.bgStyle);
                g.m_name = EditorGUILayout.TextField(new GUIContent("Layer", (HairDesignerEditor.Icon(g.m_meshLocked ? HairDesignerEditor.eIcons.LOCKED: HairDesignerEditor.eIcons.UNLOCKED))), g.m_name, GUILayout.MinHeight(25));

                
                
                if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.DUPLICATE), GUILayout.MaxWidth(25)))
                {
                    if (EditorUtility.DisplayDialog("Duplicate layer", "Duplicate layer '" + g.m_name + "' ?", "Ok", "Cancel"))
                    {
                        Undo.RecordObject(g.m_hd, "Duplicate layer");
                        HairDesignerGenerator src = g.m_hd.generator;
                        HairDesignerGenerator dest = g.m_hd.gameObject.AddComponent(src.GetType()) as HairDesignerGenerator;
                        dest.hideFlags = HideFlags.HideInInspector;
                        
                        EditorUtility.CopySerialized(src, dest);
                        g.m_hd.m_generators.Add(dest);
                        dest.m_name += "(copy)";
                        //destroy link to existing mesh
                        dest.m_meshInstance = null;
                        dest.m_hair = null;
                        dest.GenerateMeshRenderer();
                        if( dest.m_layerType != HairDesignerBase.eLayerType.FUR_SHELL )
                            dest.m_meshLocked = false;

                        for (int i = 0; i < dest.m_shaderParams.Count; ++i)
                        {
                            dest.m_shaderParams[i] = g.m_hd.gameObject.AddComponent(dest.m_shaderParams[i].GetType() ) as HairDesignerShader;
                            dest.m_shaderParams[i].hideFlags = HideFlags.HideInInspector;
                            EditorUtility.CopySerialized(src.m_shaderParams[i], dest.m_shaderParams[i]);                             
                        }

                        g.GenerateMeshRenderer();
                        OnDuplicate( dest );
                    }
                }
                


                if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH), GUILayout.MaxWidth(25)))
                {
                    if (EditorUtility.DisplayDialog("Delete layer", "Delete layer '"+g.m_name+"' ?", "Ok", "Cancel"))
                    {
                        Undo.RecordObject(g.m_hd, "Remove layer");

                        g._editorDelete = true;
                        if (g.m_meshInstance != null)
                        {
                            if (Application.isPlaying)
                                Destroy(g.m_meshInstance);
                            else
                                DestroyImmediate(g.m_meshInstance);
                        }


                        for (int j = 0; j < g.m_shaderParams.Count; ++j)
                        {
                            if (Application.isPlaying)
                                Destroy(g.m_shaderParams[j]);
                            else
                                DestroyImmediate(g.m_shaderParams[j]);
                        }
                        g.m_shaderParams.Clear();

                        HairDesignerGenerator component = g.m_hd.generator;
                        g.m_hd.m_generators.Remove(g.m_hd.generator);                        
                        g.m_hd.m_generatorId = -1;
                        g.m_hd = null;
                        if ( Application.isPlaying )
                            Destroy(component);
                        else
                            DestroyImmediate(component);
                    }
                };
                //GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                guiChanged = GUI.changed;

            }



            public virtual void OnDuplicate( HairDesignerGenerator copy )
            {



  
            }



            /// <summary>
            /// Draw Material settings
            /// </summary>
            public void DrawMaterialSettings()
            {
                HairDesignerGenerator g = target as HairDesignerGenerator;
                if (g.m_hd == null)
                    return;

                GUILayout.Label(new GUIContent("Material", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                //Material old = g.m_hairMeshMaterial;
                g.m_hairMeshMaterial = EditorGUILayout.ObjectField("Material", g.m_hairMeshMaterial, typeof(Material), true) as Material;

                //if (g.m_hairMeshMaterial != old)
                //    g.DestroyMesh();//mesh will be regenerated with new material

                if (g.m_hairMeshMaterial == null)
                    return;
                
                string shaderName = g.m_hairMeshMaterial.shader.name;
                GUILayout.Label("Shader : "  + shaderName);
                GUILayout.Space(10f);

                GUILayout.Label(new GUIContent("Instance parameters", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);


                /*
                if (g.m_matPropBlkHair == null)
                    return;
                    */
                

                if( !g.m_shaderParams.Exists(sp => sp.m_shader != null && sp.m_shader.name == g.m_hairMeshMaterial.shader.name ) )
                {
                    string shaderNameWithoutpath = g.m_hairMeshMaterial.shader.name.Split('/').Last();

                    Object[] script = Resources.FindObjectsOfTypeAll(typeof(MonoScript))
                        .Where(x => x.name == "HairDesignerShader" + shaderNameWithoutpath)
                        //.Where(x => x.name.Contains("HairDesignerShader"))
                        .ToArray();

                    if (script.Length > 0)
                    {
                        //Debug.Log("scripts found : " + script.Length);
                        //for ( int i=0; i< script.Length; ++i  )
                        {
                            //ScriptableObject s = ScriptableObject.CreateInstance((script[0] as MonoScript).GetClass());                                                      
                            //(s as HairDesignerShader).m_shader = g.m_hairMeshMaterial.shader;
                            System.Type t = (script[0] as MonoScript).GetClass();
                            HairDesignerShader s = g.m_hd.gameObject.AddComponent(t) as HairDesignerShader;
                            s.hideFlags = HideFlags.HideInInspector;
                            s.m_shader = g.m_hairMeshMaterial.shader;

                            if (s != null)
                            {
                                g.m_shaderParams.Add(s);
                                s.m_generator = g;
                            }
                            else
                                Debug.LogWarning("Shader param instance failed");
                        }
                    }
                    else
                    {
                        if (g.m_hairMeshMaterial.shader.name.Split('/').First() != "HairDesigner")
                        {
                            EditorGUILayout.HelpBox("This shader is not supported by Hair Designer\nDynamic motion won't work.", MessageType.Error);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("There's no instance parameters available for this shader.", MessageType.Info);
                        }
                        
                        if( GUILayout.Button("Open material settings") )
                        {
                            Selection.activeObject = g.m_hairMeshMaterial;
                        }                      
                    }
                }
                else
                {                   

                    HairDesignerShader hds = g.m_shaderParams.FindLast(sp => sp.m_shader == g.m_hairMeshMaterial.shader);
                    hds.m_generator = g;
                    //hds.InitPropertyBlock(ref g.m_matPropBlkHair, g.m_layerType);
                    Editor shaderEditor = Editor.CreateEditor( hds );
                    shaderEditor.OnInspectorGUI();
                    //hds.UpdatePropertyBlock(ref g.m_matPropBlkHair);

                }


            }


            public virtual void DrawSceneTools()
            {                

            }


            public virtual void DrawSceneMenu(float width)
            {
                GUILayout.Label("DRAW SCENE MENU");

            }




            public virtual void PaintToolAction()
            {

            }



            /// <summary>
            /// Draw brush
            /// </summary>
            /// <param name="mp"></param>
            /// <param name="svCam"></param>
            public virtual void DrawBrush()
            {
                HairDesignerBase hd = (target as HairDesignerGenerator).m_hd;
                Event e = Event.current;

                Camera svCam = SceneView.currentDrawingSceneView.camera;
                svCam.nearClipPlane = .01f;
                Vector2 mp2d = e.mousePosition * EditorGUIUtility.pixelsPerPoint;//retina display has 2 pixels per point
                mp2d.y = svCam.pixelHeight - mp2d.y;

                Vector3 mp = Vector3.zero;

                if (svCam.orthographic)
                {
                    mp = svCam.ScreenToWorldPoint(mp2d);
                    mp += svCam.transform.forward;
                    m_constUnit = (svCam.ViewportToWorldPoint(Vector3.zero) - svCam.ViewportToWorldPoint(Vector3.one)).magnitude;
                    m_constUnit = HandleUtility.GetHandleSize(hd.transform.position) * 10f;
                    m_brushSize = m_brushRadius * m_constUnit;
                }
                else
                {
                    Ray r = svCam.ScreenPointToRay(mp2d);
                    mp = svCam.transform.position + r.direction.normalized;
                    m_brushSize = m_brushRadius;
                }



                Color c = m_CtrlMode ? Color.red : Color.blue;
                c = m_ShiftMode ? Color.yellow : c;

                Handles.color = c;
                Handles.BeginGUI();
                //GUILayout.BeginArea(new Rect(mp.x * (float)svCam.pixelWidth, (float)svCam.pixelHeight- mp.y * (float)svCam.pixelHeight*.5f, 200, 50));
                Vector2 posScreen = svCam.WorldToScreenPoint(mp + svCam.transform.up * m_brushSize);
                GUI.color = c;
                GUILayout.BeginArea(new Rect(posScreen.x, svCam.pixelHeight - posScreen.y - 20, 200, 50));

                //GUILayout.Label("brush");
                GUILayout.EndArea();
                Handles.EndGUI();


                c.a = .05f + .1f * m_brushIntensity;
                Handles.color = c;
                //Handles.DrawSolidDisc(mp, -svCam.transform.forward, m_brushSize);

                float gradient = 10f;

                for (float i = 0; i < gradient; ++i)
                {
                    c.a = .1f * m_brushIntensity * i / gradient;
                    Handles.color = c;

                    float f = (1f - (m_brushFalloff * i) / gradient);
                    Handles.DrawSolidDisc(mp, -svCam.transform.forward, m_brushSize * f);
                }

                c.a = .03f * m_brushIntensity;
                Handles.color = c;
                Handles.DrawSolidDisc(mp, -svCam.transform.forward, m_brushSize * (1f - (m_brushFalloff)));

                //c = Color.blue;
                c.a = 1f;
                Handles.color = c;
                Handles.DrawWireDisc(mp, -svCam.transform.forward, m_brushSize);

                c.a = .1f;
                Handles.color = c;
                Handles.DrawWireDisc(mp, -svCam.transform.forward, m_brushSize * (1f - m_brushFalloff));


            }





            /// <summary>
            /// Select bone popup
            /// </summary>
            /// <param name="selected"></param>
            /// <returns></returns>
            public Transform SelectBone(Transform selected)
            {
                HairDesignerGenerator g = target as HairDesignerGenerator;
                SkinnedMeshRenderer smr = g.m_hd.GetComponent<SkinnedMeshRenderer>();

                if (smr != null)
                {
                    int id = 0;
                    List<string> boneNames = new List<string>();
                    boneNames.Add("- none -");
                    for (int i = 0; i < smr.bones.Length; ++i)
                    {
                        boneNames.Add(smr.bones[i].name);
                        if (smr.bones[i] == selected)
                            id = i + 1;
                    }


                    int newid = EditorGUILayout.Popup("Parent Bone", id, boneNames.ToArray());

                    if (newid == id)
                        return null;

                    if (newid == 0)
                        return g.m_hd.transform;
                    else
                        return smr.bones[newid - 1];
                }
                else
                    return null;
            }






           


            public virtual void DeleteEditorLayer(  int idx )
            {

            }






            /// <summary>
            /// MotionZoneSceneGUI
            /// </summary>
            protected void MotionZoneSceneGUI()
            {
                HairDesignerBase hd = (target as HairDesignerGenerator).m_hd;
                Camera SvCam = SceneView.currentDrawingSceneView.camera;

                if (hd.generator == null)
                    return;


                for (int i = 0; i < hd.generator.m_motionZones.Count; ++i)
                {
                    Vector3 handlePos = hd.generator.m_motionZones[i].parent.TransformPoint(hd.generator.m_motionZones[i].localPosition);

                    Color c = new Color(0, 0, 1f, .1f);
                    Handles.color = c;

                    Handles.DrawSolidDisc(handlePos, -SvCam.transform.forward, hd.generator.m_motionZones[i].radius * hd.globalScale * (SvCam.orthographic ? 1f : 1.07f));
                    c.r = 1f;
                    c.g = 1f;
                    c.a = .3f;
                    Handles.color = c;



                    Handles.DrawWireDisc(handlePos, hd.generator.m_motionZones[i].parent.up, hd.generator.m_motionZones[i].radius * hd.globalScale);
                    Handles.DrawWireDisc(handlePos, hd.generator.m_motionZones[i].parent.forward, hd.generator.m_motionZones[i].radius * hd.globalScale);
                    Handles.DrawWireDisc(handlePos, hd.generator.m_motionZones[i].parent.right, hd.generator.m_motionZones[i].radius * hd.globalScale);

                    //Vector3 newPos = Handles.FreeMoveHandle(hd.m_motionZones[i].parent.TransformPoint(hd.m_motionZones[i].localPosition), Quaternion.identity, hd.m_motionZones[i].radius, Vector3.zero, Handles.CircleCap);

                    Vector3 newPos = Handles.FreeMoveHandle(handlePos, Quaternion.identity, HandleUtility.GetHandleSize(handlePos) * .5f, Vector3.zero, SphereCap);
                    hd.generator.m_motionZones[i].localPosition = hd.generator.m_motionZones[i].parent.InverseTransformPoint(newPos);

                    c = Color.yellow;
                    Handles.color = c;
                    Handles.DrawLine(hd.generator.m_motionZones[i].parent.position, handlePos);

                    Handles.Label(hd.generator.m_motionZones[i].parent.position, hd.generator.m_motionZones[i].parent.gameObject.name, EditorStyles.centeredGreyMiniLabel);

                    Vector3 radiusHandle = Handles.FreeMoveHandle(handlePos + SvCam.transform.right * hd.generator.m_motionZones[i].radius * hd.globalScale, Quaternion.identity, HandleUtility.GetHandleSize(handlePos) * .2f, Vector3.zero, CircleCap);
                    if (hd.globalScale > 0)
                        hd.generator.m_motionZones[i].radius = (radiusHandle - handlePos).magnitude / hd.globalScale;
                    else
                        hd.generator.m_motionZones[i].radius = 0f;
                }
            }


#if UNITY_5_6_OR_NEWER            
            public Vector3 FreeMoveHandleTransformPoint(Transform t, Vector3 position, Quaternion rotation, float size, Vector3 snap, Handles.CapFunction capFunc)
            {                
                Vector3 newPos = t.InverseTransformPoint( Handles.FreeMoveHandle(t.TransformPoint( position ), rotation, size, snap, capFunc));
#else
            public Vector3 FreeMoveHandleTransformPoint(Transform t, Vector3 position, Quaternion rotation, float size, Vector3 snap, Handles.DrawCapFunction capFunc)
            {
                Vector3 newPos = t.InverseTransformPoint(Handles.FreeMoveHandle(t.TransformPoint(position), rotation, size, snap, capFunc));
#endif



                //avoid unwanted micro move in scene view
                if (Vector3.Distance(newPos, position) > .000001f)
                    return newPos;

                Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");

                return position;
            }




            public void DrawLayerPanel()
            {

                HairDesignerGenerator g = target as HairDesignerGenerator;

                if (g.m_meshLocked)
                    return;

                float width = 60;
                float Xpos = SceneView.currentDrawingSceneView.camera.pixelWidth- width;
                float Ypos = 120;
                

                Handles.BeginGUI();
                GUILayout.BeginArea(new Rect(Xpos, Ypos, width, SceneView.currentDrawingSceneView.camera.pixelHeight));
                GUILayout.BeginVertical(EditorStyles.helpBox,GUILayout.MaxWidth(width));
                
                GUILayout.Label("Layers", EditorStyles.centeredGreyMiniLabel);

                if (  g.m_editorLayers.Count == 0)
                    g.m_editorLayers.Add(new HairToolLayer());

                for (int i = 0; i < g.m_editorLayers.Count; ++i)
                {   
                    GUILayout.BeginHorizontal();
                    GUI.color = m_currentEditorLayer == i ? Color.white : Color.gray;
                    if (GUILayout.Button("" + (i + 1), EditorStyles.miniButtonLeft, GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        if (Event.current.button == 1)
                        {
                            //right clic : delete                            
                            DeleteEditorLayer( i);                            
                        }
                        else
                        {
                            //left clic : select
                            m_currentEditorLayer = i;
                            g.m_editorLayers[m_currentEditorLayer].visible = true;
                        }
                        OnLayerChange();
                    }

                    //GUILayout.FlexibleSpace();
                    if (GUILayout.Button(HairDesignerEditor.Icon(g.m_editorLayers[i].visible ? HairDesignerEditor.eIcons.VISIBLE : HairDesignerEditor.eIcons.HIDDEN), EditorStyles.miniButtonRight, GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        Undo.RecordObject(g, "hairDesigner layer visibility");
                        g.m_editorLayers[i].visible = !g.m_editorLayers[i].visible;
                        OnLayerChange();
                    }
                    GUILayout.EndHorizontal();
                }
                GUI.color = Color.white;


                if (GUILayout.Button("+", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxWidth(20)))
                {
                    Undo.RecordObject(g, "Delete layer");//save generator for undo
                    g.m_editorLayers.Add(new HairToolLayer());
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            
                GUILayout.EndArea();

                Handles.EndGUI();
            }



            public virtual void OnLayerChange()
            {

            }
        }
    }
}