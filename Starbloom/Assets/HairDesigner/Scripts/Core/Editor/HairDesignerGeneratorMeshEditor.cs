   using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        
        [CustomEditor(typeof(HairDesignerGeneratorMeshBase), true)]
        public class HairGeneratorMeshEditor : HairGeneratorEditor
        {


            //GUIStyle bgStyle = null;

            public enum eTab
            {
                //INFO,
                DESIGN,
                MATERIAL,
                MOTION,
                ADVANCED,
                DEBUG = 100
            }
            public static eTab m_tab = eTab.DESIGN;






            Vector3 m_lastHairPos = Vector3.zero;

            ePaintingTool m_currentTool = ePaintingTool.ADD;
            //bool m_showMotionZone = false;
            

            
            public override void OnInspectorGUI()
            {
                hideWireframe = false;
                eTab lastTab = m_tab;
                base.OnInspectorGUI();
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;

                if (g == null)
                    return;

                if (PrefabUtility.GetPrefabParent(g.gameObject) == null && PrefabUtility.GetPrefabObject(g.gameObject) != null)
                {
                    //GUILayout.Label("Instantiate the prefab for modifications", EditorStyles.helpBox);
                    return;
                }

                if (g.m_hd == null)
                    return;

                

                GUI.SetNextControlName("TAB");

                GUIContent[] toolbar = new GUIContent[] {
                    //new GUIContent("Info",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.INFO))),
                    new GUIContent("Design",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.DESIGN))),
                    new GUIContent("Material",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.MATERIAL))),
                    new GUIContent("Motion",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.MOTION)))
                };

                m_tab = (eTab)GUILayout.Toolbar((int)m_tab, toolbar);

                

                if (m_tab != lastTab)
                    GUI.FocusControl("TAB");

                

                if (g.m_layerType == HairDesignerBase.eLayerType.NONE)//fix old versions
                    g.m_layerType = HairDesignerBase.eLayerType.SHORT_HAIR_POLY;


                if (!g.m_enable)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Layer is disabled.", MessageType.Warning);
                    if (GUILayout.Button("Enable", GUILayout.Height(40)))
                        g.m_enable = true;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }



                /*
                if (m_tab == eTab.INFO)
                {
                    if (m_hairDesignerEditor != null)
                        m_hairDesignerEditor.RestorePose();

                    //base.OnInspectorGUI();
                    //GUILayout.Label("Mesh Generator");
                    //g.m_requirePainting = false;
                    //HairDesignerEditor.m_showBrush = false;
                }
                */

                

                if (m_tab == eTab.DESIGN)
                {
                    
                    if (Application.isPlaying)
                    {
                       EditorGUILayout.HelpBox("Polygon edition disabled in play mode",MessageType.Info);
                    }
                    else if(g.m_meshLocked)
                    {
                        GUILayout.Space(20);
                        EditorGUILayout.HelpBox( "Layer is locked\nAlways lock a layer when painting is over.", MessageType.Info );
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Unlock layer", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.UNLOCKED)))))
                        {
                            g.m_hd.m_meshSaved = false;
                            g.m_meshLocked = false;
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        hideWireframe = true;
                        g.m_requirePainting = false;
                        HairDesignerEditor.m_showSceneMenu = false;
                    }
                    else
                    {                        
                        g.m_wasInDesignTab = true;

                        

                        if (m_hairDesignerEditor != null)
                        {
                            m_hairDesignerEditor.CreateColliderAndTpose();                            
                        }

                        if( g.m_allowFastMeshDraw )
                            g.DestroyMesh();//force hair mesh as single strand rendering

                        hideWireframe = true;
                        g.m_requirePainting = true;
                        HairDesignerEditor.m_showSceneMenu = true;


                        GUILayout.Label(new GUIContent("Layer parameters", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                        g.m_scale = EditorGUILayout.FloatField(new GUIContent("Global scale","strand scale for this layer"), 100f * g.m_scale / g.strandScaleFactor) * g.strandScaleFactor / 100f;
                        g.m_scale = Mathf.Clamp(g.m_scale, 0, g.m_scale);

                        g.m_strandSpacing = EditorGUILayout.FloatField("Min spacing", 100f* g.m_strandSpacing / g.strandScaleFactor) * g.strandScaleFactor / 100f;
                        g.m_strandSpacing = Mathf.Clamp(g.m_strandSpacing, 0, g.m_strandSpacing);

                        g.m_atlasSizeX = EditorGUILayout.IntSlider("Atlas size", g.m_atlasSizeX, 1, 10);

                        //g.m_allowFastMeshDraw = EditorGUILayout.Toggle("Fast draw", g.m_allowFastMeshDraw);
                        g.m_allowFastMeshDraw = false;//fast draw is finaly slower :p


                        GUILayout.Label(new GUIContent("Strand shape", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                        g.m_params.m_taper = EditorGUILayout.Vector2Field("Taper", g.m_params.m_taper);
                        g.m_params.m_bendX = EditorGUILayout.FloatField("Bend", g.m_params.m_bendX);
                        g.m_params.m_HairResolutionX = Mathf.Clamp(EditorGUILayout.IntField("Strand subdivision X", g.m_params.m_HairResolutionX), 1, 20);
                        g.m_params.m_HairResolutionY = Mathf.Clamp(EditorGUILayout.IntField("Strand subdivision Y", g.m_params.m_HairResolutionY), 1, 20);
                        g.m_params.m_randomSrandFactor = EditorGUILayout.FloatField("Random", g.m_params.m_randomSrandFactor);
                        g.m_params.m_length = Mathf.Clamp(EditorGUILayout.FloatField("Length", g.m_params.m_length), 0, 100);

                        float old = g.m_params.m_normalToTangent;
                        g.m_params.m_normalToTangent = EditorGUILayout.FloatField("Normal switch", g.m_params.m_normalToTangent);
                        if (g.m_params.m_normalToTangent != old)
                            g.m_hairStrand = null;

                        g.m_params.m_gravityFactor = EditorGUILayout.FloatField("Gravity", g.m_params.m_gravityFactor);

                        

                        GUILayout.Label(new GUIContent("Strand curve", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);
                        
                        g.m_params.m_curv.m_startPosition = EditorGUILayout.Vector3Field("Start position", g.m_params.m_curv.m_startPosition);
                        g.m_params.m_curv.m_startTangent = EditorGUILayout.Vector3Field("Start tangent", g.m_params.m_curv.m_startTangent);
                        g.m_params.m_curv.m_endPosition = EditorGUILayout.Vector3Field("End position", g.m_params.m_curv.m_endPosition);
                        g.m_params.m_curv.m_endTangent = EditorGUILayout.Vector3Field("End tangent", g.m_params.m_curv.m_endTangent);


                        if (g.m_hairStrand != null)
                        {
                            int strandCount = 0;
                            for (int i= 0; i < g.m_strands.Count; ++i)
                            {
                                if (g.m_editorLayers[g.m_strands[i].layer].visible)
                                    strandCount += 1;
                            }

                            int triCount = (g.m_hairStrand.triangles.Length / 3) * strandCount;
                            if (triCount < 30000)
                                EditorGUILayout.HelpBox("Mesh info : " + strandCount + " strands | " + triCount + " tri", MessageType.Info);
                            else if (triCount < 65208)
                                EditorGUILayout.HelpBox("You should reduce poly count!\n Check Strand subdivisions or remove some strand\n Mesh info : " + strandCount + " strands | " + triCount + " tri", MessageType.Warning);
                            else
                                EditorGUILayout.HelpBox("Too many polygons!\n Reduce Strand resolution or remove some strand\n Mesh info : " + strandCount + " strands | " + (g.m_hairStrand.triangles.Length / 3) * strandCount + " tri", MessageType.Error);
                        }


                        

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Clear layer", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH)))))
                        {
                            if (EditorUtility.DisplayDialog("Clear strands", "Delete all strands in layer "+(m_currentEditorLayer+1) +" ?", "Ok", "Cancel"))
                            {
                                Undo.RecordObject(g, "clear strands");
                                for( int id=0; id< g.m_strands.Count; ++id  )
                                {
                                    if (g.m_strands[id].layer == m_currentEditorLayer)
                                        g.m_strands.RemoveAt(id--);
                                }
                                
                            }
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        


                        GUILayout.Space(20);

                        EditorGUILayout.HelpBox("Layer is unlocked\nAlways lock a layer when painting is over.", MessageType.Info);

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Lock layer", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.LOCKED)))))
                        {
                            
                            g.GenerateMeshRenderer();
                            //EditorUtility.SetDirty(target);
                            g.m_meshLocked = true;
                            m_tab = eTab.MATERIAL;
                            //g.m_shaderNeedUpdate = true;                            
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        

                        GUILayout.Space(20);

                        if (!g.m_materialInitialized && g.m_matPropBlkHair != null )
                        {                           
                            DrawMaterialSettings();
                            g.m_materialInitialized = true;                            
                        }

                        if (GUI.changed)
                            g.m_needRefresh = true;

                    }

                    
                }
                else
                {
                    if (g.m_meshInstance == null)
                    {
                        g.GenerateMeshRenderer();
                        //EditorUtility.SetDirty(target);
                    }
                }

               

                if (m_tab == eTab.MATERIAL)
                {
                    if (m_hairDesignerEditor != null)
                        m_hairDesignerEditor.RestorePose();                    
                    g.m_requirePainting = false;
                    HairDesignerEditor.m_showSceneMenu = false;
                    hideWireframe = true;
                    DrawMaterialSettings();
                }

                

                    //m_showMotionZone = false;

                if (m_tab == eTab.MOTION)
                {
                    if (m_hairDesignerEditor != null && !Application.isPlaying )
                        m_hairDesignerEditor.CreateColliderAndTpose();

                    hideWireframe = true;
                    HairDesignerEditor.m_showSceneMenu = false;
                    //m_showMotionZone = true;
                                        
                    MotionZoneInspectorGUI();
                }

                //EditorGUILayout.ObjectField(g.m_hairStrand, typeof(Mesh), true);
                //EditorGUILayout.ObjectField(g.m_hair, typeof(Mesh), true);
                //EditorGUILayout.Toggle(g.m_initialized);
                /*
                scroll = EditorGUILayout.BeginScrollView(scroll);
                for( int i=0; i< m_strands.Count; ++i )
                    GUILayout.Label("local pos " + m_strands[i].localpos);

                EditorGUILayout.EndScrollView();
                */

                //DrawDefaultInspector();

                /*
                HairDesignerShader hds = g.m_shaderParams.FindLast(sp => sp.m_shader == g.m_hairMeshMaterial.shader);
                hds.InitPropertyBlock(ref g.m_matPropBlkHair, g.m_layerType);
                */
            }

            

            void MotionZoneInspectorGUI()
            {
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;               


                for (int i = 0; i < g.m_motionZones.Count; ++i)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent("Motion zones " + (i+1), (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);
                    if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH), GUILayout.MaxWidth(25)))
                    {
                        if (EditorUtility.DisplayDialog("Motion zone", "Delete motion zone", "Ok", "Cancel"))
                        {
                            Undo.RecordObject(g, "Delete motion zone");
                            g.m_motionZones.RemoveAt(i--);
                        }
                    }
                    GUILayout.EndHorizontal();


                    GUILayout.Label("Position & hierachy", EditorStyles.boldLabel);
                    g.m_motionZones[i].parent = EditorGUILayout.ObjectField("Parent", g.m_motionZones[i].parent, typeof(Transform), true) as Transform;


                    Transform bone = SelectBone(g.m_motionZones[i].parent);

                    if (bone != null)
                    {
                        g.m_motionZones[i].parent = bone;
                        g.m_motionZones[i].localPosition = Vector3.zero;
                    }

                    g.m_motionZones[i].localPosition = EditorGUILayout.Vector3Field("Offset", g.m_motionZones[i].localPosition);
                    g.m_motionZones[i].radius = EditorGUILayout.FloatField("Radius", g.m_motionZones[i].radius);

                    GUILayout.Label("Motion parameters", EditorStyles.boldLabel);

                    g.m_motionZones[i].pid.m_params.kp = EditorGUILayout.FloatField("Damping", g.m_motionZones[i].pid.m_params.kp);
                    g.m_motionZones[i].pid.m_params.ki = EditorGUILayout.FloatField("Bouncing", g.m_motionZones[i].pid.m_params.ki);
                    g.m_motionZones[i].motionLimit.y = EditorGUILayout.FloatField("Limit", g.m_motionZones[i].motionLimit.y);
                    g.m_motionZones[i].motionLimit.y = Mathf.Clamp(g.m_motionZones[i].motionLimit.y, 0, g.m_motionZones[i].motionLimit.y);
                    g.m_motionZones[i].motionLimit.x = -g.m_motionZones[i].motionLimit.y;

                    g.m_motionZones[i].motionFactor = EditorGUILayout.FloatField("Motion factor", g.m_motionZones[i].motionFactor);

                    g.m_motionZones[i].pid.m_params.kd = 0;
                    GUILayout.Box(" ", GUILayout.Height(50f), GUILayout.ExpandWidth(true));
                    g.m_motionZones[i].pid.m_pidX.m_params = g.m_motionZones[i].pid.m_params;
                    HairDesignerPID pid = new HairDesignerPID(g.m_motionZones[i].pid.m_params.kp, g.m_motionZones[i].pid.m_params.ki, 0);
                    //pid.m_params = g.m_motionZones[i].pid.m_params;
                    //pid.m_params.limits = new Vector2(-10, 10);
                    HairDesignerEditor.GUIDrawPidResponse(pid, GUILayoutUtility.GetLastRect(), 5f);
                }

                if (GUILayout.Button("+ New motion Zone +", GUILayout.Height(40)))
                {
                    Undo.RecordObject(g, "New motion zone");
                    MotionZone mz = new MotionZone();
                    mz.parent = g.m_hd.transform;
                    g.m_motionZones.Add(mz);
                }

                EditorGUILayout.HelpBox("The motion zone must encapsulate the hair strands to enable dynamic animation.\nLock the motion zone to the nearest bone for skeleton animation.", MessageType.Info);


            }

            


            /// <summary>
            /// DrawSceneMenu
            /// </summary>
            /// <param name="width"></param>
            public override void DrawSceneMenu( float width)
            {               

                GUILayout.BeginHorizontal();
                GUI.color = m_currentTool == ePaintingTool.ADD ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Paint", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.PAINT)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.ADD;
                }

                GUI.color = m_currentTool == ePaintingTool.BRUSH ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Brush", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BRUSH)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.BRUSH;
                }
                

                GUI.color = m_currentTool == ePaintingTool.SCALE ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Scale", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.SCALE)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.SCALE;
                }
                

                GUILayout.EndHorizontal();


                //if (GUILayout.Button("Hair Strand Editor"))
                //{
                //    HairStrandPreview.Init();
                //    HairStrandPreview.m_hd = hd;
                //}



                GUI.color = Color.black;
                GUILayout.Label("Brush settings", EditorStyles.centeredGreyMiniLabel);
                GUI.color = Color.white;

                GUILayout.BeginHorizontal();
                GUILayout.Label("Size", GUILayout.MinWidth(width * .3f));
                m_brushRadius = (EditorGUILayout.Slider((m_brushRadius) / .3f, 0.05f, 1f, GUILayout.MaxWidth(width * .7f)) * .3f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Intensity", GUILayout.MinWidth(width * .3f));
                m_brushIntensity = EditorGUILayout.Slider(m_brushIntensity, 0f, 1f, GUILayout.MaxWidth(width * .7f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Falloff", GUILayout.MinWidth(width * .3f));
                m_brushFalloff = EditorGUILayout.Slider(m_brushFalloff, 0f, 1f, GUILayout.MaxWidth(width * .7f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Scale", GUILayout.MinWidth(width * .3f));
                m_brushScale = EditorGUILayout.Slider(m_brushScale, 0f, 2f, GUILayout.MaxWidth(width * .7f));
                GUILayout.EndHorizontal();

                //m_strandSpacing = EditorGUILayout.FloatField("Hair spacing", m_strandSpacing);

                

                GUILayout.Space(20);

                switch (m_currentTool)
                {
                    case ePaintingTool.ADD:
                        EditorGUILayout.HelpBox("Ctrl : remove mode", MessageType.Info);
                        EditorGUILayout.HelpBox("tips : Increase brush intensity to reduce the spacing between strands", MessageType.Info);
                        break;

                    case ePaintingTool.BRUSH:
                        EditorGUILayout.HelpBox("Ctrl  : raise the hair\nShift : smooth direction", MessageType.Info);
                        break;

                    case ePaintingTool.SCALE:
                        EditorGUILayout.HelpBox("Ctrl  : scale down\nShift : smooth", MessageType.Info);
                        break;
                }


                

            }



            public override void DrawSceneTools()
            {
                if (m_tab == eTab.DESIGN)
                    DrawLayerPanel();

                if (m_tab == eTab.MOTION)
                    MotionZoneSceneGUI();
            }

            



            /// <summary>
            /// PaintToolAction
            /// </summary>
            /// <param name="svCam"></param>
            public override void PaintToolAction()
            {

                

                Camera svCam = SceneView.currentDrawingSceneView.camera;
                if (Event.current.alt || HairDesignerEditor.m_meshCollider == null)
                    return;

                m_CtrlMode = Event.current.control;
                m_ShiftMode = Event.current.shift;
                m_AltMode = Event.current.alt;

                HairDesignerBase hd = (target as HairDesignerGenerator).m_hd;
                HairDesignerGeneratorMeshBase g = (target as HairDesignerGeneratorMeshBase);

                if (g.m_editorLayers.Count < m_currentEditorLayer || m_currentEditorLayer == -1)
                    return;

                if (!g.m_editorLayers[m_currentEditorLayer].visible)
                    return;

                Vector3 mp = Event.current.mousePosition* EditorGUIUtility.pixelsPerPoint;
                mp.y = svCam.pixelHeight - mp.y;

                if (m_currentTool == ePaintingTool.ADD)
                {
                    float pixelSize = Vector3.Distance(svCam.WorldToScreenPoint(svCam.transform.position + svCam.transform.forward), svCam.WorldToScreenPoint(svCam.transform.position + svCam.transform.forward + svCam.transform.right * m_brushSize));
                    Vector2 rnd = Random.insideUnitCircle * pixelSize;
                    mp.x += rnd.x;
                    mp.y += rnd.y;
                }

                Ray r = svCam.ScreenPointToRay(mp);




                //Setup painting info
                StrandData dt = new StrandData();
                dt.layer = m_currentEditorLayer;
                Vector2 mp2d = Event.current.mousePosition* EditorGUIUtility.pixelsPerPoint;
                mp2d.y = svCam.pixelHeight - mp2d.y;

                BrushToolData bt = new BrushToolData();
                bt.mousePos = mp2d;
                bt.transform = hd.transform;
                bt.tool = m_currentTool;
                bt.cam = svCam;
                bt.brushScale = m_brushScale;
                bt.CtrlMode = m_CtrlMode;
                bt.ShiftMode = m_ShiftMode;
                bt.brushSize = m_brushSize;
                bt.brushScreenDir = (svCam.transform.right * Event.current.delta.x - svCam.transform.up * Event.current.delta.y).normalized;
                bt.brushFalloff = m_brushFalloff;
                bt.brushIntensity = m_brushIntensity;
                //if (m_currentTool == ePaintingTool.ADD)
                //if (Event.current.type == EventType.MouseDrag )
                {
                    Vector3 worldPos = Vector3.zero;
                    RaycastHit[] hitInfos = Physics.RaycastAll(r);

                    //Debug.DrawRay(r.origin, r.direction, Color.blue, 10f);

                    foreach (RaycastHit hitInfo in hitInfos)
                    {

                        if (hitInfo.collider.gameObject != HairDesignerEditor.m_meshCollider.gameObject)
                            continue;//get only our custom collider

                        worldPos = hitInfo.point;

                        //hitInfo.triangleIndex;

                        if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            m_lastHairPos = worldPos;


                        //if (svCam.orthographic)
                        //    Handles.DrawSolidDisc(worldPos, -svCam.transform.forward, (m_constUnit * .001f + m_constUnit * .0015f));
                        //else
                        //    Handles.DrawSolidDisc(worldPos, -svCam.transform.forward, HandleUtility.GetHandleSize(worldPos) * .02f);

                        //Handles.DrawLine(worldPos, worldPos + hitInfo.normal);

                        dt.normal = hd.transform.InverseTransformDirection(hitInfo.normal);
                        dt.rotation = Quaternion.FromToRotation(Vector3.forward, hd.transform.InverseTransformDirection(hitInfo.normal));


                        if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {

                            //if(g.m_meshInstance!=null)
                            //    DestroyImmediate(g.m_meshInstance.gameObject);                       
                            Undo.RegisterCompleteObjectUndo(g, "hairDesigner brush");
                            EditorUtility.SetDirty(g);
                            //Debug.Log("!! Record Undo !!");
                            
                            return;
                        }

                        if (!Event.current.alt && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                        {
                            dt.localpos = hd.transform.InverseTransformPoint(worldPos);
                            dt.meshTriId = hitInfo.triangleIndex;                            
                            
                            bt.brushDir = hd.transform.InverseTransformDirection(worldPos - m_lastHairPos).normalized;

                            hd.generator.PaintTool(dt, bt);
                            hd.generator.m_hair = null;
                            //m_needUndo = true;
                        }
                        return;
                    }
                }
            }





            public override void DeleteEditorLayer(int idx)
            {
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;
                bool isEmpty = true;
                for (int i = 0; i < g.m_strands.Count; ++i)
                {
                    if (g.m_strands[i].layer == idx)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                bool delete = true;
                if (!isEmpty)
                {
                    delete = EditorUtility.DisplayDialog("Delete layer", "Remove all the strands in layer " + (idx + 1) + " ?", "ok", "cancel");
                }
                else
                {
                    delete = EditorUtility.DisplayDialog("Delete layer", "Remove layer" + (idx + 1) + " ?", "ok", "cancel");
                }

                if (delete)
                {
                    Undo.RecordObject(g, "Delete layer");//save generator for undo

                    for (int i = 0; i < g.m_strands.Count; ++i)
                    {
                        if (g.m_strands[i].layer == idx)
                        {
                            g.m_strands.RemoveAt(i--);
                            continue;
                        }

                        if (g.m_strands[i].layer > idx)
                            g.m_strands[i].layer--;//update layer idx                        
                    }

                    g.m_editorLayers.RemoveAt(idx);
                }
            }

            public override void OnLayerChange()
            {
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;
                DestroyImmediate(g.m_meshInstance);
                g.GenerateMeshRenderer();
            }



        }

    }
}