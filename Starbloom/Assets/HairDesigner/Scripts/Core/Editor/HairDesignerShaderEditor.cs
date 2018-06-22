using UnityEngine;
using UnityEditor;
using System.Collections;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerShaderEditor : Editor
        {
            public void GUILayoutTextureSlot(string label, ref Texture2D tex, ref Vector2 scale, ref Vector2 offset)
            {

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Label(label);
                scale = EditorGUILayout.Vector2Field("Tiling", scale);
                offset = EditorGUILayout.Vector2Field("Offset", offset);
                GUILayout.EndVertical();

                tex = EditorGUILayout.ObjectField("", tex, typeof(Texture2D), false, GUILayout.MaxWidth(70)) as Texture2D;
                GUILayout.EndHorizontal();
            }


            protected bool _destroyed = false;
            public override void OnInspectorGUI()
            {
                HairDesignerShader s = target as HairDesignerShader;

                if (PrefabUtility.GetPrefabParent(s.gameObject) == null && PrefabUtility.GetPrefabObject(s.gameObject) != null)
                {
                    GUILayout.Label("Instantiate the prefab for modifications", EditorStyles.helpBox);
                    _destroyed = true;
                    return;
                }


                if (s.m_hd == null || s.m_generator==null)
                {
                    //Debug.LogWarning("DestroyImmediate Shader");
                    _destroyed = true;
                    DestroyImmediate(s);
                }



        }   
        }


        
    }
}
