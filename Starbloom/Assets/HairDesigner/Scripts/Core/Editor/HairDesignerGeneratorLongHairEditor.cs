using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [CustomEditor(typeof(HairDesignerGeneratorLongHairBase), true)]
        public class HairGeneratorLongHairBaseEditor : HairGeneratorEditor
        {


            //GUIStyle bgStyle = null;

            enum eTab
            {
                //INFO,
                DESIGN,
                MATERIAL,
                MOTION,
                ADVANCED,
                DEBUG = 100
            }
            static eTab m_tab = eTab.MATERIAL;
            Vector3 m_lastHairPos = Vector3.zero;
            ePaintingTool m_currentTool = ePaintingTool.ADD;

            static bool m_showSelectionButtons = false;
            static bool m_showSelectionOnly = true;
            static bool m_changeSeedOnDuplicate = true;
            static Color m_whiteAlpha = new Color(1f, 1f, 1f, .8f);




            Vector2 scroll;
            public override void OnInspectorGUI()
            {
                hideWireframe = false;
                eTab lastTab = m_tab;
                base.OnInspectorGUI();

                HairDesignerGeneratorLongHairBase g = target as HairDesignerGeneratorLongHairBase;

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

                
                if (g == null)
                    return;

                if (g.m_layerType == HairDesignerBase.eLayerType.NONE)//fix old versions
                    g.m_layerType = HairDesignerBase.eLayerType.LONG_HAIR_POLY;

                g.m_params.m_gravityFactor = 0f;//gravity disabled for shader - use bone physics instead


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

                        //bool updateAll = false;
                        //EditorGUI.BeginChangeCheck();
                        g.m_scale = EditorGUILayout.FloatField(new GUIContent("Global scale", "strand scale for this layer"), g.m_scale);
                        

                        g.m_atlasSizeX = EditorGUILayout.IntSlider("Atlas size", g.m_atlasSizeX, 1, 10);
                        g.m_allowFastMeshDraw = EditorGUILayout.Toggle("Fast draw", g.m_allowFastMeshDraw);
                        //g.m_enableClothPhysics = EditorGUILayout.Toggle("Cloth physics", g.m_enableClothPhysics);
                        g.m_editorData.HandlesUIOrthoSize = EditorGUILayout.FloatField( "Handles ortho size", g.m_editorData.HandlesUIOrthoSize);
                        g.m_editorData.HandlesUIPerspSize = EditorGUILayout.FloatField("Handles persp size", g.m_editorData.HandlesUIPerspSize);

                        //if (EditorGUI.EndChangeCheck())
                        //    updateAll = true;

                        GUILayout.Label(new GUIContent("Long hair groups", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                        m_showSelectionOnly = EditorGUILayout.Toggle("Show edition only", m_showSelectionOnly);

                        int polyCount = 0;


                        
                        //Draw group options
                        for ( int i=0; i<g.m_groups.Count; ++i )
                        {                            

                            if(g.m_groups[i].m_mCurv == null )
                            {
                                //g.m_groups[i].m_mCurv = new MBZCurv( g.m_groups[i].m_curv );
                                g.m_groups[i].m_mCurv = new MBZCurv();
                                g.m_groups[i].Generate();
                            }
                            
                            int triCount = 0;
                            for( int k=0; k< g.m_groups[i].m_strands.Count; ++k)
                            {
                                if( g.m_groups[i].m_strands[k].mesh != null &&  g.m_editorLayers[g.m_groups[i].m_layer].visible )
                                    triCount += g.m_groups[i].m_strands[k].mesh.triangles.Length / 3;
                            }
                            polyCount += triCount;


                            /*
                            if (m_showSelectionOnly && !g.m_groups[i].m_edit)
                            {
                                if (updateAll)
                                    g.m_groups[i].Generate();
                                continue;
                            }
                            */

                            
                            GUILayout.BeginHorizontal();

                            GUILayout.Label("" + (i + 1) + ". ", GUILayout.Width(20));
                            GUILayout.Label("" + triCount + " tri", EditorStyles.boldLabel );
                            
                            /*
                            //if (GUILayout.Button("Generate"))
                            //    g.m_groups[i].Generate();
                            if (GUILayout.Button(g.m_groups[i].m_edit?"Save":"Edit", GUILayout.Width(50f)))
                                g.m_groups[i].m_edit = !g.m_groups[i].m_edit;

                            if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.DUPLICATE), GUILayout.Height(20), GUILayout.Width(25)))
                            {
                                if (EditorUtility.DisplayDialog("Duplicate", "Duplicate this hair group?\n"+(i+1)+" -> "+ (g.m_groups.Count+1), "Ok", "Cancel"))
                                    g.m_groups.Add(g.m_groups[i].Copy());
                            }

                            if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH), GUILayout.Height(20), GUILayout.Width(25)))
                            {
                                if(EditorUtility.DisplayDialog("Delete", "Delete this hair group?", "Ok", "Cancel"))
                                    g.m_groups.RemoveAt(i--);
                            }
                            */
                            

                            GUILayout.EndHorizontal();

                            /*
                            if (g.m_groups[i].m_edit)
                            {
                                List<HairDesignerGeneratorLongHairBase.HairGroup> lst = new List<HairDesignerGeneratorLongHairBase.HairGroup>();
                                lst.Add(g.m_groups[i]);
                                EditGroupParams(lst);
                                g.m_groups[i].m_parentOffset = g.m_groups[i].m_parent.InverseTransformPoint(g.m_hd.transform.TransformPoint(g.m_groups[i].m_mCurv.startPosition));
                                g.m_groups[i].m_parentRotation = Quaternion.Inverse(g.m_groups[i].m_parent.rotation);
                            }
                            
                            if ( (g.m_groups[i].m_edit && GUI.changed) || updateAll)
                                g.m_groups[i].Generate();
                            */
                            
                        }

                        /*
                        if (GUILayout.Button(new GUIContent("new curve", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))))
                            g.m_groups.Add(new HairDesignerGeneratorLongHairBase.HairGroup());
                        */
                        
                            
                            //g.m_scale = Mathf.Clamp(g.m_scale, 0, g.m_scale);

                            //g.m_strandSpacing = EditorGUILayout.FloatField("Min spacing", g.m_strandSpacing / g.strandScaleFactor) * g.strandScaleFactor;
                            //g.m_strandSpacing = Mathf.Clamp(g.m_strandSpacing, 0, g.m_strandSpacing);


                        
                        
                        

                        if (g.m_hairStrand != null)
                        {
                            //int triCount = (g.m_hairStrand.triangles.Length / 3) * g.GetStandCount();
                            
                            if (polyCount < 30000)
                                EditorGUILayout.HelpBox("Mesh info : " + g.GetStrandCount() + " strands | " + polyCount + " tri", MessageType.Info);
                            else if (polyCount < 65208)
                                EditorGUILayout.HelpBox("You should reduce poly count!\n Check Strand subdivisions or remove some strand\n Mesh info : " + polyCount + " tri", MessageType.Warning);
                            else
                                EditorGUILayout.HelpBox("Too many polygons!\n Reduce Strand resolution or remove some strand\n Mesh info : " + polyCount + " tri", MessageType.Error);
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Clear layer", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH)))))
                        {
                            if (EditorUtility.DisplayDialog("Clear groups", "Delete all strands in current layer ?", "Ok", "Cancel"))
                            {
                                Undo.RecordObject(g, "clear strands");
                                for( int i=0; i<g.m_groups.Count; ++i )
                                    g.m_groups[i].m_strands.Clear();
                                g.m_groups.Clear();
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
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        

                        GUILayout.Space(20);

                        if (!g.m_materialInitialized)
                        {
                            DrawMaterialSettings();
                            g.m_materialInitialized = true;                            
                        }


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

                //
                for (int i = 0; i < g.m_groups.Count; ++i)
                {
                    if (g.m_groups[i].m_parent != null)
                    {
                        if (g.m_groups[i].m_bones.Count > 0 && g.m_groups[i].m_bones[0] != null && g.m_groups[i].m_parent != g.transform)
                        {
                            //g.m_groups[i].m_bones[0].transform.position = g.m_groups[i].m_parent.TransformPoint(g.m_groups[i].m_parentOffset);
                            //g.m_groups[i].m_bones[0].transform.rotation = g.m_groups[i].m_parentRotation * g.m_groups[i].m_parent.rotation;
                        }
                    }
                }


                //HairDesignerEditor.m_showMotionZone = false;

                if (m_tab == eTab.MOTION)
                {
                    if (m_hairDesignerEditor != null)
                        m_hairDesignerEditor.CreateColliderAndTpose();

                    hideWireframe = true;
                    HairDesignerEditor.m_showSceneMenu = false;
                    //HairDesignerEditor.m_showMotionZone = true;
                                        
                    MotionZoneInspectorGUI();
                }

               

                //DrawDefaultInspector();

            }



            int[] layerIdx = null;
            string[] layerIdxName = null;
            public void EditGroupParams(List<HairDesignerGeneratorLongHairBase.HairGroup> hgLst )
            {
                if (hgLst.Count == 0)
                    return;

                HairDesignerGeneratorLongHairBase g = target as HairDesignerGeneratorLongHairBase;
                HairDesignerGeneratorLongHairBase.HairGroup hg = hgLst[0];
                bool newB;
                float newF;
                int newI;
                //AnimationCurve newAC;
                //bool guiChanged = false;

                EditorGUI.BeginChangeCheck();
                scroll = GUILayout.BeginScrollView(scroll);

                if (g.m_editorLayers != null)
                {
                    if(layerIdx == null || layerIdx.Length != g.m_editorLayers.Count )
                    {
                        layerIdxName = new string[g.m_editorLayers.Count];
                        layerIdx = new int[g.m_editorLayers.Count];
                        for (int i = 0; i < g.m_editorLayers.Count; ++i)
                        {
                            layerIdxName[i] = "" + (i + 1);
                            layerIdx[i] = i;
                        }
                    }
                    newI = EditorGUILayout.IntPopup("Layer", hg.m_layer, layerIdxName, layerIdx);
                    //newI = EditorGUILayout.IntField("Layer", hg.m_layer+1);
                    newI = Mathf.Clamp(newI, 0, g.m_editorLayers.Count-1);
                    if (hg.m_layer != newI)
                    {
                        Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                        for (int i = 0; i < hgLst.Count; ++i)
                            hgLst[i].m_layer = newI;
                    }
                }

                //if (hgLst.Count == 1)//update doesn't work with multi selection
                {
                    /*
                    float[] eval = new float[10];
                    for (int test = 0; test < 10; ++test)
                        eval[test] = hg.m_shape.Evaluate((float)test / 10f);
                    */
                    EditorGUI.BeginChangeCheck();
                    hg.m_shape = EditorGUILayout.CurveField("Shape", hg.m_shape);

                    /*
                    bool ACchanged = false;
                    for( int test =0; test<10; ++test )
                    {
                        if (eval[test] != newAC.Evaluate((float)test / 10f))
                            ACchanged = true;
                    }*/
                    //if(ACchanged)
                    if (EditorGUI.EndChangeCheck())
                    {
                        //Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit shape");
                        for (int i = 1; i < hgLst.Count; ++i)
                        {
                            hgLst[i].m_shape = new AnimationCurve(hg.m_shape.keys);
                        }
                        //guiChanged = true;
                    }
                }

                newF = EditorGUILayout.FloatField("Scale", hg.m_scale*100f)/100f;
                if (hg.m_scale != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_scale = newF;
                }

                newF = EditorGUILayout.FloatField("Start angle", hg.m_mCurv.m_startAngle);
                if (hg.m_mCurv.m_startAngle != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_mCurv.m_startAngle = newF;
                }

                newF = EditorGUILayout.FloatField("End angle", hg.m_mCurv.m_endAngle);
                if (hg.m_mCurv.m_endAngle != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_mCurv.m_endAngle = newF;
                }
                newI = EditorGUILayout.IntField("strand count", hg.m_strandCount);
                newI = Mathf.Clamp(newI, 1, 50);
                if (hg.m_strandCount != newI)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_strandCount = newI;
                }

                newF = EditorGUILayout.FloatField("strand max angle", hg.m_strandMaxAngle);
                if (hg.m_strandMaxAngle != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_strandMaxAngle = newF;
                }

                newF = EditorGUILayout.FloatField("Normal offset", hg.m_normalOffset);
                if (hg.m_normalOffset != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_normalOffset = newF;
                }

                newI = EditorGUILayout.IntField("subdivision X", hg.m_subdivisionX);
                newI = Mathf.Clamp(newI, 1, 10);
                if (hg.m_subdivisionX != newI)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_subdivisionX = newI;
                }

                newI = EditorGUILayout.IntField("subdivision Y", hg.m_subdivisionY);
                newI = Mathf.Clamp(newI, 1, 50);
                if (hg.m_subdivisionY != newI)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_subdivisionY = newI;
                }

                newF = EditorGUILayout.FloatField("bend start", hg.m_bendAngleStart);
                if (hg.m_bendAngleStart != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_bendAngleStart = newF;
                }

                newF = EditorGUILayout.FloatField("bend end", hg.m_bendAngleEnd);
                if (hg.m_bendAngleEnd != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_bendAngleEnd = newF;
                }

                newF = EditorGUILayout.FloatField("folding", hg.m_folding);
                if (hg.m_folding != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_folding = newF;
                }

                newF = EditorGUILayout.FloatField("Wave amplitude", hg.m_waveAmplitude);
                if (hg.m_waveAmplitude != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_waveAmplitude = newF;
                }

                newF = EditorGUILayout.FloatField("Wave period", hg.m_wavePeriod);
                if (hg.m_wavePeriod != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_wavePeriod = newF;
                }

                newF = EditorGUILayout.FloatField("Start offset", hg.m_startOffset);
                if (hg.m_startOffset != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_startOffset = newF;
                }

                newF = EditorGUILayout.FloatField("End offset", hg.m_endOffset);
                if (hg.m_endOffset != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_endOffset = newF;

                }

                newI = EditorGUILayout.IntField("Rnd seed", hg.m_rndSeed);
                if (hg.m_rndSeed != newI)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_rndSeed = newI;
                }

                newF = EditorGUILayout.Slider("Rnd length", hg.m_rndStrandLength, 0f, 1f);
                if (hg.m_rndStrandLength != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_rndStrandLength = newF;
                }


                hg.m_normalSwitch = 0f;
                /*
                newF = EditorGUILayout.Slider("Normal switch", hg.m_normalSwitch, 0f, 1f);
                if (hg.m_normalSwitch != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_normalSwitch = newF;
                }*/

                newI = EditorGUILayout.IntField("UV X", hg.m_UVX);
                newI = Mathf.Clamp(newI, 1, 10);
                if (hg.m_UVX != newI)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_UVX = newI;
                }

                //hg.m_parent = EditorGUILayout.ObjectField("parent", hg.m_parent, typeof(Transform),true) as Transform;
                Transform selectedBone = SelectBone(hg.m_parent);
                if (selectedBone != null)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    //hg.m_parent = selectedBone;
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_parent = selectedBone;
                }


                newB = EditorGUILayout.Toggle("Snap surface", hg.m_snapToSurface);
                if (hg.m_snapToSurface != newB)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_snapToSurface = newB;
                }

                newB = EditorGUILayout.Toggle("Dynamic", hg.m_dynamic);
                if (hg.m_dynamic != newB)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_dynamic = newB;
                }

                
                GUI.enabled = hg.m_dynamic;

                newF = EditorGUILayout.Slider("Gravity", hg.m_gravityFactor, 0f, 1f);
                if (hg.m_gravityFactor != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_gravityFactor = newF;
                }


                newF = EditorGUILayout.Slider("Root rigidity", hg.m_rootRigidity, 0f, 1f);
                if (hg.m_rootRigidity != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_rootRigidity = newF;
                }

                newF = EditorGUILayout.Slider("Rigidity", hg.m_rigidity, 0f, 1f);
                if (hg.m_rigidity != newF)
                {
                    Undo.RecordObject(target as HairDesignerGeneratorLongHairBase, "Edit hair");
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].m_rigidity = newF;
                }


                GUI.enabled = true;

                //int splitCount = EditorGUILayout.IntField("Curve split", hg.m_mCurv.m_curves.Length);                               
                int splitCount = 1;
                if (hg.m_mCurv.m_curves.Length != splitCount)
                    hg.m_mCurv.Split(splitCount);


                GUILayout.EndScrollView();

                if (EditorGUI.EndChangeCheck())
                {
                    
                    for (int i = 0; i < hgLst.Count; ++i)
                        hgLst[i].Generate();
                }

                
            }


            static bool m_showSelectionPanel = true;
            /// <summary>
            /// DrawSceneMenu
            /// </summary>
            /// <param name="width"></param>
            public override void DrawSceneMenu(float width)
            {
                
                HairDesignerGeneratorLongHairBase g = target as HairDesignerGeneratorLongHairBase;

                //m_currentTool = ePaintingTool.ADD;
                
                GUILayout.BeginHorizontal();


                GUI.color = m_currentTool == ePaintingTool.ADD ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Add hair", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.PAINT)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = m_currentTool== ePaintingTool.NONE ? ePaintingTool.ADD : ePaintingTool.NONE; ;
                }

                GUI.color = m_currentTool == ePaintingTool.NONE ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Mirror", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.MIRROR)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = m_currentTool == ePaintingTool.ADD ? ePaintingTool.NONE : ePaintingTool.ADD;
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
                

                //m_showSelectionButtons = EditorGUILayout.Toggle("Show hair selection", m_showSelectionButtons);
                //m_showSelectionButtons = Event.current.control;

                if (m_currentTool == ePaintingTool.ADD)
                {
                    //spline tools

                    EditorGUILayout.HelpBox(
                        "Ctrl  : add hair\n" +
                        "Shift : hair selection\n" +
                        "Ctrl + Shift : delete hair\n" +
                        "Alt : move all curve nodes\n" +
                        "Alt + Shift : duplicate hair"
                        , MessageType.Info);

                    m_brushRadius = .01f;
                    m_brushFalloff = 1f;

                    bool selectAll = false;
                    bool unselectAll = false;

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button((m_showSelectionPanel ? "-" : "+") + " Selection", EditorStyles.helpBox, GUILayout.Width(70)))
                        m_showSelectionPanel = !m_showSelectionPanel;
                    //GUILayout.Label("Selected");
                    if (m_showSelectionPanel)
                    {
                        GUILayout.FlexibleSpace();
                        GUI.color = Color.grey;
                        if (GUILayout.Button("all", EditorStyles.helpBox, GUILayout.Width(50)))
                            selectAll = true;
                        if (GUILayout.Button("none", EditorStyles.helpBox, GUILayout.Width(50)))
                            unselectAll = true;
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    int count = 0;

                    if (m_showSelectionPanel)
                    {
                        int groupID = 0;

                        for (int i = 0; i < g.m_groups.Count; ++i)
                        {
                            if (selectAll)
                                g.m_groups[i].m_edit = true;

                            if (unselectAll)
                                g.m_groups[i].m_edit = false;

                            if (g.m_groups[i].m_modifierId == -1)
                                groupID++;
                            else
                                continue;//don't show hair group generated by modifier

                            if (g.m_groups[i].m_layer != m_currentEditorLayer)
                            {
                                g.m_groups[i].m_edit = false;
                                continue;
                            }

                            GUI.color = g.m_groups[i].m_edit ? Color.white : Color.grey;

                            if (GUILayout.Button("" + (groupID), GUILayout.Width(25)))
                                g.m_groups[i].m_edit = !g.m_groups[i].m_edit;

                            count++;
                            if (count % 8 == 0)
                            {
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                            }

                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUI.color = Color.white;
                    List<HairDesignerGeneratorLongHairBase.HairGroup> lst;

                    lst = g.m_groups.Where(group => group.m_edit && group.m_modifierId==-1).ToList();


                    EditGroupParams(lst);
                }


                if (m_currentTool == ePaintingTool.NONE)
                {
                    //modifiers menu
                    
                    for( int i=0; i< g.m_mirrorModifiers.Count; ++i)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();
                        g.m_mirrorModifiers[i].name = EditorGUILayout.TextField(g.m_mirrorModifiers[i].name, EditorStyles.boldLabel);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH), GUILayout.Height(20)))
                        {
                            for( int j=0; j<g.m_groups.Count; ++j )
                            {
                                if (g.m_groups[j].m_modifierId == g.m_mirrorModifiers[i].id)
                                    g.m_groups.RemoveAt(j--);
                            }
                            g.m_mirrorModifiers.Remove(g.m_mirrorModifiers[i--]);
                            continue;
                        }                        
                        GUILayout.EndHorizontal();
                        DrawMirrorModifierUI( g.m_mirrorModifiers[i] );
                        GUILayout.EndVertical();                        
                    }

                    

                    if (GUILayout.Button("New mirror"))
                    {
                        HairDesignerGeneratorLongHairBase.MirrorModifier mirror = new HairDesignerGeneratorLongHairBase.MirrorModifier();
                        mirror.id = g.GenerateModifierId();
                        mirror.name = "mirror " + ( g.m_mirrorModifiers.Count+1);
                        g.m_mirrorModifiers.Add(mirror);
                        
                    }
                }

                //update modifiers
                for (int i = 0; i < g.m_mirrorModifiers.Count; ++i)
                    if (g.m_mirrorModifiers[i].autoUpdate && g.m_mirrorModifiers[i].layers.Contains(m_currentEditorLayer))
                        g.m_mirrorModifiers[i].Update(g);
            }



            /// <summary>
            /// Draw motion zone tools in scene view
            /// </summary>
            void MotionZoneInspectorGUI()
            {
                HairDesignerGeneratorLongHairBase g = target as HairDesignerGeneratorLongHairBase;

                GUILayout.Label(new GUIContent("Bones motion ", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                //g.m_gravityFactor = EditorGUILayout.FloatField("Gravity", g.m_gravityFactor);
                //g.m_motionFactor = EditorGUILayout.FloatField("Motion factor", g.m_motionFactor);
                EditorGUI.BeginChangeCheck();

                g.m_motionData.gravity = EditorGUILayout.FloatField("Gravity", g.m_motionData.gravity);
                g.m_motionData.rootRigidity = EditorGUILayout.Slider("Root rigidity", g.m_motionData.rootRigidity,0f,1f);
                g.m_motionData.rigidity = EditorGUILayout.Slider("Rigidity", g.m_motionData.rigidity, 0f, 1f);
                g.m_motionData.elasticity = EditorGUILayout.Slider("Elasticity", g.m_motionData.elasticity, 0f, 1f);
                g.m_motionData.smooth = EditorGUILayout.Slider("Smooth", g.m_motionData.smooth, 0f, 1f);
                g.m_motionData.accurateBoneRotation = EditorGUILayout.Toggle("Accurate bone rotation", g.m_motionData.accurateBoneRotation);

                g.m_motionData.bonePID.m_params.kp = EditorGUILayout.FloatField("Damping", g.m_motionData.bonePID.m_params.kp);
                g.m_motionData.bonePID.m_params.ki = EditorGUILayout.FloatField("Bouncing", g.m_motionData.bonePID.m_params.ki);

                HairDesignerPID bonepid = new HairDesignerPID(g.m_motionData.bonePID.m_params.kp, g.m_motionData.bonePID.m_params.ki, 0);
                GUILayout.Box(" ", GUILayout.Height(50f), GUILayout.ExpandWidth(true));
                HairDesignerEditor.GUIDrawPidResponse(bonepid, GUILayoutUtility.GetLastRect(), 5f);
                g.m_motionData.wind = EditorGUILayout.FloatField("Wind", g.m_motionData.wind);
                
                if ( EditorGUI.EndChangeCheck() )
                {
                    EditorUtility.SetDirty(g);
                }

                GUILayout.Space(10);

                //-------------------
                //colliders

                GUILayout.Label(new GUIContent("Colliders", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                var serializedObject = new SerializedObject(g.m_hd);
                var property = serializedObject.FindProperty("m_capsuleColliders");
                serializedObject.Update();
                EditorGUILayout.PropertyField(property, true);
                serializedObject.ApplyModifiedProperties();

                GUILayout.Space(10);

                //----------------------
                //cloth
#if UNITY_2017_1_OR_NEWER
                GUILayout.Label(new GUIContent("Cloth Physics", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("\n Generate Cloth Physics \n"))
                {                    
                    GenerateClothPhysics(g);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (!g.m_meshLocked)
                    EditorGUILayout.HelpBox("Layer is not locked.\nthe cloth component will be destroyed when you'll edit the polygons.", MessageType.Warning);
#endif
                //----------------------



                GUILayout.Label(new GUIContent("Motion zones" , (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                for (int i = 0; i < g.m_motionZones.Count; ++i)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.BeginHorizontal(  );
                    GUILayout.Label("Motion zones " + (i+1), EditorStyles.boldLabel);
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


                    GUILayout.EndVertical();


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
            /// Draw tools
            /// </summary>
            public override void DrawSceneTools()
            {
                if (m_tab == eTab.DESIGN)
                {
                    if ( m_currentTool == ePaintingTool.ADD )
                        DrawCurveSceneGUI();
                    if (m_currentTool == ePaintingTool.NONE)
                        DrawModifierSceneTool();

                    DrawLayerPanel();
                }

                if (m_tab == eTab.MOTION)
                    MotionZoneSceneGUI();

            }


            /// <summary>
            /// Draw curve in scene
            /// </summary>
            public void DrawCurveSceneGUI()
            {
                

                HairDesignerGeneratorLongHairBase g = target as HairDesignerGeneratorLongHairBase;
                if (g.m_groups.Count == 0 || g.m_meshLocked )
                    return;

                

                for (int i = 0; i < g.m_groups.Count; ++i)
                {
                    if (g.m_groups[i] == null)
                    {
                        g.m_groups.RemoveAt(i--);
                        continue;
                    }

                    if (g.m_groups[i].m_layer != m_currentEditorLayer)
                        continue;

                    if (g.m_groups[i].m_modifierId != -1)
                        continue;

                    HairDesignerGeneratorLongHairBase.HairGroup hg = g.m_groups[i];
                    Transform trans = g.m_hd.transform;

                    m_showSelectionButtons = Event.current.shift;
                    if (hg.m_mCurv != null && hg.m_mCurv.m_curves.Length>0 && m_showSelectionButtons )
                    {
                        float dot = Vector3.Dot(trans.TransformDirection(hg.m_mCurv.startTangent).normalized, SceneView.currentDrawingSceneView.camera.transform.forward );
                        if (dot < 0f || hg.m_edit )
                        {
                            Handles.BeginGUI();
                            Vector3 UIpos = trans.TransformPoint(hg.m_mCurv.endPosition);
                            UIpos = SceneView.currentDrawingSceneView.camera.WorldToScreenPoint(UIpos);
                            GUILayout.BeginArea(new Rect(UIpos.x, SceneView.currentDrawingSceneView.camera.pixelHeight - UIpos.y, 30f, 30f));
                            //GUILayout.Label("("+(i+1)+")");
                            GUI.color = hg.m_edit ? Color.white : Color.gray;

                            if (!Event.current.control && !Event.current.alt )
                            {
                                if (GUILayout.Button("" + (i + 1)))
                                {
                                    Undo.RecordObject(g, "Edit hair");
                                    hg.m_edit = !hg.m_edit;
                                    if (hg.m_edit)
                                    {                                        
                                        g.m_referenceId = i;
                                    }
                                }
                            }
                            else if(Event.current.control)
                            {
                                if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH)))
                                {
                                    Undo.RecordObject(g, "Delete hair");
                                    g.m_groups.RemoveAt(i--);
                                }
                            }
                            else if (Event.current.alt)
                            {
                                if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.DUPLICATE)))
                                {
                                    Undo.RecordObject(g, "Duplicate hair");
                                    g.m_groups.Add(g.m_groups[i].Copy());
                                    g.m_groups[i].m_edit = false;
                                    if (m_changeSeedOnDuplicate)
                                        g.m_groups[g.m_groups.Count - 1].m_rndSeed =Random.Range(0,int.MaxValue);

                                }
                            }
                            GUI.color = Color.white;
                            GUILayout.EndArea();
                            Handles.EndGUI();
                        }
                    }

                    if (hg.m_parent == null)
                        hg.m_parent = g.m_hd.transform;

                    float size = HandleUtility.GetHandleSize(SceneView.currentDrawingSceneView.camera.transform.position + SceneView.currentDrawingSceneView.camera.transform.forward) * .2f ;
                    if (!SceneView.currentDrawingSceneView.camera.orthographic)
                        size *= g.m_hd.globalScale * .3f * g.m_editorData.HandlesUIPerspSize;
                    else
                        size *= g.m_editorData.HandlesUIOrthoSize;

                    if (m_showSelectionButtons)
                    {
                        float dot = Vector3.Dot(trans.TransformDirection(hg.m_mCurv.startTangent).normalized, SceneView.currentDrawingSceneView.camera.transform.forward);
                        if (dot < 0f || hg.m_edit)
                        {
                            float max = 30f;
                            for (float a = 1; a < max; ++a)
                            {
                                Handles.color = hg.m_edit ? Color.white : Color.grey;
                                Vector3 p1 = trans.TransformPoint(hg.m_mCurv.GetPosition((a - 1) / max));
                                Vector3 p2 = trans.TransformPoint(hg.m_mCurv.GetPosition(a / max));
                                Handles.DrawLine(p1, p2);
                            }
                        }
                    }


                    if (!hg.m_edit || m_showSelectionButtons )                                                               
                        continue;
                    
                    

                    bool changed = false;                    
                    
                                       
                    Vector3 old;


                    //draw bone link
                    Handles.color = m_whiteAlpha;
                    Handles.DrawDottedLine(trans.TransformPoint(hg.m_mCurv.startPosition), hg.m_parent.position, size*100f);

                    {
                        float max = 10f;
                        for (float a = 1; a < max; ++a)
                        {
                            Handles.color = Color.Lerp(Color.green, Color.red, a / max);
                            Vector3 p1 = trans.TransformPoint(hg.m_mCurv.GetPosition((a - 1) / max));
                            Vector3 p2 = trans.TransformPoint(hg.m_mCurv.GetPosition(a / max));
                            Handles.DrawLine(p1, p2);
                            //Handles.DrawLine(p1, p1 + trans.TransformDirection(hg.m_mCurv.GetUp((a - 1) / max/*, Vector3.up*/)) * size * 1f);
                        }
                    }


                    for( int n=0; n< hg.m_mCurv.m_curves.Length; ++n )
                    {
                        Handles.color = Color.Lerp( Color.green, Color.red, (float)n/(float)hg.m_mCurv.m_curves.Length);
                        old = hg.m_mCurv.startPosition;

                        /*
                        pos = Handles.FreeMoveHandle(trans.TransformPoint(hg.m_mCurv.m_curves[n].m_startPosition), Quaternion.identity, size, Vector3.zero, Handles.CircleCap);                        
                        //hg.m_mCurv.m_curves[n].m_startPosition = trans.InverseTransformPoint(pos);                       
                        Vector3 newPos = trans.InverseTransformPoint(pos);

                        if (Vector3.Distance(old, newPos) > .0001f)
                            hg.m_mCurv.m_curves[n].m_startPosition = newPos;
                        */

                        

                        //draw move handle
                        Vector3 oldPos = hg.m_mCurv.m_curves[n].m_startPosition;
                        hg.m_mCurv.m_curves[n].m_startPosition = FreeMoveHandleTransformPoint(trans, hg.m_mCurv.m_curves[n].m_startPosition, Quaternion.identity, size, Vector3.zero,CircleCap);
                        if (n > 0)
                        {
                            hg.m_mCurv.m_curves[n - 1].m_endPosition = hg.m_mCurv.m_curves[n].m_startPosition;
                        }
                        else if(oldPos != hg.m_mCurv.m_curves[n].m_startPosition && hg.m_snapToSurface)
                        {
                            //snap to surface
                            Ray r = new Ray(trans.TransformPoint(hg.m_mCurv.m_curves[n].m_startPosition + hg.m_mCurv.m_curves[n].m_startTangent), trans.TransformDirection(-hg.m_mCurv.m_curves[n].m_startTangent));                            
                            RaycastHit[] hits = Physics.RaycastAll(r, 10 );
                            for(int h=0; h< hits.Length; ++h)
                            {
                                if(hits[h].collider==HairDesignerEditor.m_meshCollider )
                                {
                                    hg.m_mCurv.m_curves[n].m_startPosition = trans.InverseTransformPoint(hits[h].point);
                                    break;
                                }
                            }
                        }

                        
                        if (hg.m_mCurv.startPosition != old)
                            changed = true;

                                               
                        if (Event.current.alt)
                        {
                            //move all curves nodes
                            for (int o = 0; o < hg.m_mCurv.m_curves.Length; ++o)
                            {
                                if (o != n)
                                    hg.m_mCurv.m_curves[o].m_startPosition += hg.m_mCurv.startPosition - old;
                                if(o== hg.m_mCurv.m_curves.Length-1)
                                    hg.m_mCurv.m_curves[o].m_endPosition += hg.m_mCurv.startPosition - old;
                            }
                        }

                        //Handles.color = Color.white;
                        /*
                        tan = Handles.FreeMoveHandle(trans.TransformPoint(hg.m_mCurv.m_curves[n].m_startPosition + hg.m_mCurv.m_curves[n].m_startTangent), Quaternion.identity, size / 2f, Vector3.zero, Handles.RectangleCap);
                        hg.m_mCurv.m_curves[n].m_startTangent = trans.InverseTransformPoint(tan) - hg.m_mCurv.m_curves[n].m_startPosition;
                         
                        */
                        hg.m_mCurv.m_curves[n].m_startTangent = FreeMoveHandleTransformPoint(trans, hg.m_mCurv.m_curves[n].m_startPosition + hg.m_mCurv.m_curves[n].m_startTangent, Quaternion.identity, size / 2f, Vector3.zero,RectangleCap) - hg.m_mCurv.m_curves[n].m_startPosition;
                        Handles.DrawLine(trans.TransformPoint(hg.m_mCurv.m_curves[n].m_startPosition), trans.TransformPoint(hg.m_mCurv.m_curves[n].m_startPosition + hg.m_mCurv.m_curves[n].m_startTangent));

                        if (n > 0)
                            hg.m_mCurv.m_curves[n-1].m_endTangent = -hg.m_mCurv.m_curves[n].m_startTangent;


                        //
                    }


                    //----------------------------------
                    //draw rotate handle
                    Vector3 rp0 = hg.m_mCurv.startPosition + hg.m_mCurv.GetUp(0) * size * 3f / g.m_hd.globalScale;
                    Vector3 rp1 = trans.TransformPoint(FreeMoveHandleTransformPoint(trans, rp0, Quaternion.identity, size * .5f, Vector3.zero, SphereCap));

                    if (Vector3.Distance(rp1, trans.TransformPoint(rp0)) > .0001f)
                    {
                        float a = Vector3.Angle(trans.InverseTransformPoint(rp1), rp0);
                        hg.m_mCurv.m_startAngle += a * Mathf.Sign(Vector3.Dot(trans.TransformDirection(hg.m_mCurv.startTangent), Vector3.Cross(trans.TransformPoint(rp0) - trans.TransformPoint(hg.m_mCurv.startPosition), rp1 - trans.TransformPoint(hg.m_mCurv.startPosition)).normalized)) * 2f;
                    }
                    Handles.DrawWireArc(trans.TransformPoint(hg.m_mCurv.startPosition), trans.TransformDirection(hg.m_mCurv.startTangent), trans.TransformDirection(hg.m_mCurv.GetUp(0)), 45f, size * 3f);
                    Handles.DrawWireArc(trans.TransformPoint(hg.m_mCurv.startPosition), trans.TransformDirection(hg.m_mCurv.startTangent), trans.TransformDirection(hg.m_mCurv.GetUp(0)), -45f, size * 3f);

                    //----------------------------------



                    Handles.color = Color.red;
                    old = hg.m_mCurv.endPosition;
                    
                    /*
                    pos = Handles.FreeMoveHandle(trans.TransformPoint(hg.m_mCurv.endPosition), Quaternion.identity, size, Vector3.zero, Handles.CircleCap);
                    hg.m_mCurv.endPosition = trans.InverseTransformPoint(pos);
                    */
                    hg.m_mCurv.endPosition = FreeMoveHandleTransformPoint(trans, hg.m_mCurv.endPosition, Quaternion.identity, size, Vector3.zero, CircleCap);




                    //draw rotate handle
                    rp0 = hg.m_mCurv.endPosition + hg.m_mCurv.GetUp(1) * size * 3f / g.m_hd.globalScale;
                    rp1 = trans.TransformPoint(FreeMoveHandleTransformPoint(trans, rp0, Quaternion.identity, size * .5f, Vector3.zero, SphereCap));

                    if (Vector3.Distance(rp1, trans.TransformPoint(rp0)) > .0001f)
                    {
                        float a = Vector3.Angle(trans.InverseTransformPoint(rp1), rp0);
                        hg.m_mCurv.m_endAngle -= a * Mathf.Sign(Vector3.Dot(trans.TransformDirection(hg.m_mCurv.endTangent), Vector3.Cross(trans.TransformPoint(rp0) - trans.TransformPoint(hg.m_mCurv.endPosition), rp1 - trans.TransformPoint(hg.m_mCurv.endPosition)).normalized)) * 2f;
                    }
                    Handles.DrawWireArc(trans.TransformPoint(hg.m_mCurv.endPosition), trans.TransformDirection(hg.m_mCurv.endTangent), trans.TransformDirection(hg.m_mCurv.GetUp(1)), 45f, size * 3f);
                    Handles.DrawWireArc(trans.TransformPoint(hg.m_mCurv.endPosition), trans.TransformDirection(hg.m_mCurv.endTangent), trans.TransformDirection(hg.m_mCurv.GetUp(1)), -45f, size * 3f);





                    /*
                    //draw rotate handle
                    Vector3 p0 = hg.m_mCurv.startPosition + hg.m_mCurv.GetUp(0) * size * .3f;
                    Vector3 p1 = trans.TransformPoint(FreeMoveHandleTransformPoint(trans, p0, Quaternion.identity, size * .5f, Vector3.zero, SphereCap));

                    if (Vector3.Distance(p1, trans.TransformPoint(p0)) > .0001f)
                    {
                        float a = Vector3.Angle(trans.InverseTransformPoint(p1), p0);
                        hg.m_mCurv.m_startAngle += a * Mathf.Sign(Vector3.Dot(trans.TransformDirection(hg.m_mCurv.startTangent), Vector3.Cross(trans.TransformPoint(p0) - trans.TransformPoint(hg.m_mCurv.startPosition), p1 - trans.TransformPoint(hg.m_mCurv.startPosition)).normalized)) * 2f;
                    }
                    Handles.DrawWireArc(trans.TransformPoint(hg.m_mCurv.startPosition), trans.TransformDirection(hg.m_mCurv.startTangent), trans.TransformDirection(hg.m_mCurv.GetUp(0)), 45f, size * 3f);
                    Handles.DrawWireArc(trans.TransformPoint(hg.m_mCurv.startPosition), trans.TransformDirection(hg.m_mCurv.startTangent), trans.TransformDirection(hg.m_mCurv.GetUp(0)), -45f, size * 3f);
                    */



                    /*
                    if (hg.m_mCurv.endPosition != old)
                        changed = true;
                        */
                    if (Event.current.alt)
                    {
                        //move all curves nodes
                        for (int o = 0; o < hg.m_mCurv.m_curves.Length; ++o)
                        {
                                hg.m_mCurv.m_curves[o].m_startPosition += hg.m_mCurv.endPosition - old;
                        }
                    }


                    //Handles.color = Color.white;
                    
                    /*
                    tan = Handles.FreeMoveHandle(trans.TransformPoint(hg.m_mCurv.endPosition - hg.m_mCurv.endTangent), Quaternion.identity, size / 2f, Vector3.zero, Handles.RectangleCap);
                    hg.m_mCurv.endTangent = -(trans.InverseTransformPoint(tan) - hg.m_mCurv.endPosition);
                    //Handles.DrawLine(pos, tan);
                    */
                    
                    hg.m_mCurv.endTangent = -( FreeMoveHandleTransformPoint(trans, hg.m_mCurv.endPosition - hg.m_mCurv.endTangent, Quaternion.identity, size / 2f, Vector3.zero, RectangleCap) - hg.m_mCurv.endPosition );
                    //Handles.DrawLine(hg.m_mCurv.endPosition, hg.m_mCurv.endPosition + hg.m_mCurv.endTangent);
                    Handles.DrawLine(trans.TransformPoint(hg.m_mCurv.endPosition), trans.TransformPoint(hg.m_mCurv.endPosition - hg.m_mCurv.endTangent));

                    if (GUI.changed)
                        changed = true;

                    if (hg.m_edit && changed)
                    {
                        hg.Generate();
                        if (!g.m_allowFastMeshDraw)
                            g.m_needRefresh = true;
                    }
                    
                }
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

                /*
                if (m_currentTool != ePaintingTool.ADD)
                    return;
                */ 

                //Debug.Log("PaintToolAction");

                m_CtrlMode = Event.current.control;
                m_ShiftMode = Event.current.shift;
                m_AltMode = Event.current.alt;

                if ( !m_CtrlMode || m_ShiftMode )
                    return;

                HairDesignerBase hd = (target as HairDesignerGenerator).m_hd;

                Vector3 mp = Event.current.mousePosition* EditorGUIUtility.pixelsPerPoint;
                mp.y = svCam.pixelHeight - mp.y;

                /*
                if (m_currentTool == ePaintingTool.ADD)
                {
                    float pixelSize = Vector3.Distance(svCam.WorldToScreenPoint(svCam.transform.position + svCam.transform.forward), svCam.WorldToScreenPoint(svCam.transform.position + svCam.transform.forward + svCam.transform.right * m_brushSize));
                    Vector2 rnd = Random.insideUnitCircle * pixelSize;
                    mp.x += rnd.x;
                    mp.y += rnd.y;
                }*/

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
                bt.CtrlMode = m_CtrlMode;
                bt.ShiftMode = m_ShiftMode;
                bt.brushSize = m_brushSize;
                bt.brushScreenDir = (svCam.transform.right * Event.current.delta.x - svCam.transform.up * Event.current.delta.y).normalized;
                bt.brushFalloff = m_brushFalloff;
                bt.brushIntensity = m_brushIntensity;
                if (m_currentTool == ePaintingTool.ADD)
                {
                    Vector3 worldPos = Vector3.zero;
                    bool collisionFound = false;
                    RaycastHit[] hitInfos = Physics.RaycastAll(r);
                    foreach (RaycastHit hitInfo in hitInfos)
                    {
                        if (hitInfo.collider.gameObject != HairDesignerEditor.m_meshCollider.gameObject)
                            continue;//get only our custom collider
                        worldPos = hitInfo.point;                        
                        if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            m_lastHairPos = worldPos;

                        dt.normal = hd.transform.InverseTransformDirection(hitInfo.normal);
                        dt.rotation = Quaternion.FromToRotation(Vector3.forward, hd.transform.InverseTransformDirection(hitInfo.normal));
                        collisionFound = true;
                    }

                    if (collisionFound && !Event.current.alt && Event.current.type == EventType.MouseUp && Event.current.button == 0)
                    {
                        Undo.RecordObject(hd.generator, "hairDesigner brush");
                        dt.localpos = hd.transform.InverseTransformPoint(worldPos);

                        bt.brushDir = hd.transform.InverseTransformDirection(worldPos - m_lastHairPos).normalized;

                        hd.generator.PaintTool(dt, bt);
                        hd.generator.m_hair = null;
                        //m_needUndo = true;
                    }
                }
            }


            /// <summary>
            /// Generate Cloth Physics
            /// </summary>
            /// <param name="g"></param>
            public void GenerateClothPhysics( HairDesignerGeneratorLongHairBase g )
            {

                EditorUtility.DisplayProgressBar("Compute cloth weights", "Please wait", 0f);
                float progress = 0f;
                bool computeCloth = true;

                if (g.m_meshInstance.GetComponent<Cloth>() != null)
                {
                    computeCloth = EditorUtility.DisplayDialog("Cloth component already set", "Replace the cloth component?", "Ok", "Cancel");
                    if (computeCloth)
                        DestroyImmediate(g.m_meshInstance.GetComponent<Cloth>());

                }

                //cloth settings
                if (computeCloth)
                {

                    Cloth clth = g.m_meshInstance.AddComponent<Cloth>();
                    clth.useVirtualParticles = 0;
                    clth.damping = .3f;
                    clth.bendingStiffness = 1f;
                    clth.worldAccelerationScale = .01f;
                    clth.worldVelocityScale = .01f;
                    clth.friction = 1f;
                    //clth.clothSolverFrequency = 120;
                    clth.capsuleColliders = g.m_hd.m_capsuleColliders.ToArray();
                    //clth.enableContinuousCollision = false;

                    ClothSkinningCoefficient[] coef = clth.coefficients;//  new ClothSkinningCoefficient[m_skinnedMesh.sharedMesh.vertexCount];
                    Vector3[] Mvtc = g.m_skinnedMesh.sharedMesh.vertices;
                    Vector3[] Cvtc = clth.vertices;
                    Vector2[] uv = g.m_skinnedMesh.sharedMesh.uv;

                    for (int c = 0; c < coef.Length; ++c)
                    {
                        float distMin = float.MaxValue;
                        int id = -1;
                        for (int v = 0; v < Mvtc.Length; ++v)
                        {
                            float dist = Vector3.Distance(g.m_skinnedMesh.transform.TransformPoint(Mvtc[v]), g.m_skinnedMesh.rootBone.transform.TransformPoint(Cvtc[c] * .1f));
                            if (dist < distMin)
                            {
                                distMin = dist;
                                id = v;
                                if (dist == 0)
                                    break;
                            }
                        }
                        //coef[c].maxDistance = uv[id].y * .15f;
                        coef[c].maxDistance = uv[id].y * .01f;

                        float f = (float)c / (float)coef.Length;
                        if (f >= progress + .05f)
                        {
                            progress = f;
                            //EditorUtility.DisplayProgressBar("Compute cloth weights", "Please wait", progress);
                            if (EditorUtility.DisplayCancelableProgressBar("Compute cloth weights", "Please wait", progress))
                            {
                                DestroyImmediate(g.m_meshInstance.GetComponent<Cloth>());
                                EditorUtility.ClearProgressBar();
                                return;
                            }

                        }
                    }

                    clth.coefficients = coef;


                    
                }

                EditorUtility.ClearProgressBar();
            }



            public override void DeleteEditorLayer( int idx)
            {
                HairDesignerGeneratorLongHairBase g = target as HairDesignerGeneratorLongHairBase;
                bool isEmpty = true;
                for (int i=0; i<g.m_groups.Count; ++i)
                {
                    if(g.m_groups[i].m_layer == idx)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                bool delete = true;
                if( !isEmpty )
                {
                    delete = EditorUtility.DisplayDialog("Delete layer", "Remove all the curves defined in layer "+(idx+1)+" ?", "ok", "cancel");
                }
                else
                {
                    delete = EditorUtility.DisplayDialog("Delete layer", "Remove layer" + (idx + 1) + " ?", "ok", "cancel");
                }

                if( delete )
                {
                    Undo.RecordObject(g, "Delete layer");//save generator for undo

                    for (int i = 0; i < g.m_groups.Count; ++i)
                    {
                        if (g.m_groups[i].m_layer == idx)
                        {
                            g.m_groups.RemoveAt(i--);
                            continue;
                        }

                        if (g.m_groups[i].m_layer > idx)                        
                            g.m_groups[i].m_layer--;//update layer idx                        
                    }

                    g.m_editorLayers.RemoveAt(idx);
                }
            }



            //-------------------------------------------------------------

            public void DrawModifierSceneTool()
            {
                //HairDesignerGeneratorLongHairBase g = target as HairDesignerGeneratorLongHairBase;
                /*
                for (int i=0; i<g.m_mirrorModifiers.Count; ++i)
                {                    
                    DrawMirrorModifierSceneTool(g.m_mirrorModifiers[i]);
                }*/
            }

            /*
            public void DrawMirrorModifierSceneTool( HairDesignerGeneratorLongHairBase.MirrorModifier m )
            {
                if (m == null) return;

                HairDesignerGeneratorLongHairBase g = target as HairDesignerGeneratorLongHairBase;
                m.pos = g.transform.InverseTransformPoint( Handles.PositionHandle(g.transform.TransformPoint(m.pos), Quaternion.identity) );
            }
            */

            public void DrawMirrorModifierUI(HairDesignerGeneratorLongHairBase.MirrorModifier m)
            {
                if (m == null) return;
                HairDesignerGeneratorLongHairBase g = target as HairDesignerGeneratorLongHairBase;
                EditorGUI.BeginChangeCheck();

                m.m_axis = (HairDesignerGeneratorLongHairBase.MirrorModifier.eMirrorAxis)EditorGUILayout.EnumPopup("Axis", m.m_axis);
                m.m_offset = EditorGUILayout.Vector3Field("Offset", m.m_offset);
                GUILayout.Label("Layers");
                GUILayout.BeginHorizontal();
                
                for (int l = 0; l < g.m_editorLayers.Count; ++l)
                {
                    GUIStyle s = EditorStyles.miniButtonMid;
                    if( l==0 ) s = EditorStyles.miniButtonLeft;
                    if (l == g.m_editorLayers.Count-1) s = EditorStyles.miniButtonRight;

                    bool layerEnable = m.layers.Contains(l);
                    GUI.color = layerEnable ? Color.white : Color.grey;
                    if (GUILayout.Button("" + (l + 1), s ))
                    {
                        if (layerEnable)
                            m.layers.Remove(l);
                        else
                            m.layers.Add(l);
                    }
                    GUI.color = Color.white;
                }
                if(EditorGUI.EndChangeCheck() )
                    m.Update(g);
                GUILayout.EndHorizontal();

                m.autoUpdate = EditorGUILayout.ToggleLeft("auto update", m.autoUpdate);

                GUILayout.BeginHorizontal();

                if (m != null && GUILayout.Button("Update"))
                    m.Update(g);

                if (m != null && GUILayout.Button("Apply"))
                {
                    if (EditorUtility.DisplayDialog("Apply modifier", "The mirror modifier will be destroyed and the hair strands will become selectable.", "Ok", "Cancel"))
                    {
                        m.Apply(g);
                        g.m_mirrorModifiers.Remove(m);
                    }
                }
                GUILayout.EndHorizontal();
            }


        }

        
    }
}