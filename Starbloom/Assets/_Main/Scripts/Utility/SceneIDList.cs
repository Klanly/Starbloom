using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class SceneIDList : MonoBehaviour {

    [System.Serializable]
    public class SceneIdentity
    {
        public string SceneName;
        public NetworkScene SceneLink;



        public bool ShowLocalization;

        [Header("Localization")]
        [ShowIf("ShowLocalization")]
        public int LocalizationID;
        [ShowIf("ShowLocalization")]
        public bool AppendFarmName;
        [ShowIf("ShowLocalization")]
        public bool AppendPlayerName;
        [Header("Visuals")]
        public bool AllowEnvironmentParticles;
        public bool AllowSnowShader;

        [Button(ButtonSizes.Small)]
        public void JumpToScene_InGame()
        {
            if (!Application.isPlaying) return;
            QuickFind.SceneTransitionHandler.TriggerSceneChange(SceneName, 0);
        }
    }


    [ListDrawerSettings(ShowIndexLabels = true, NumberOfItemsPerPage = 4)]
    public List<SceneIdentity> Scenes;




    private void Awake()
    {
        QuickFind.SceneList = this;
    }



    public int GetSceneIndexByString(string ST)
    {
        for(int i= 0; i < Scenes.Count; i++)
        {
            if (Scenes[i].SceneName == ST)
                return i;
        }
        Debug.Log("Failed To Find Scene - " + ST);
        if(!Application.isPlaying)
        {
            Debug.Log("Detected Editor Searching for Scene name, and Not Found, Adding to List Now.");
            SceneIdentity SI = new SceneIdentity();
            SI.SceneName = ST;
            Scenes.Add(SI);
        }

        return -1;
    }
    public SceneIdentity GetSceneByString(string ST)
    {
        return Scenes[GetSceneIndexByString(ST)];
    }

    public SceneIdentity GetSceneById(int ID)
    {
        if (ID < Scenes.Count)
            return Scenes[ID];
        else
        {
            Debug.Log("Scene ID too High - " + ID.ToString());
            return null;
        }
    }



    public string GetLocalizedSceneName(string SceneName, DG_ScenePortalTrigger SPT = null)
    {
        SceneIdentity SI = GetSceneByString(SceneName);
        return ReturnLocalized(SI, SPT);
    }
    public string GetLocalizedSceneNameByID(int SceneID)
    {
        SceneIdentity SI = GetSceneById(SceneID);
        return ReturnLocalized(SI);
    }

    string ReturnLocalized(SceneIdentity SI, DG_ScenePortalTrigger SPT = null)
    {
        string ReturnString = string.Empty;

        if (SI.AppendFarmName)
            ReturnString += (QuickFind.Farm.FarmName + " ");
        if (SI.AppendPlayerName)
            ReturnString += (QuickFind.Farm.PlayerCharacters[SPT.Owner].Name + " ");

        ReturnString += QuickFind.WordDatabase.GetWordFromID(SI.LocalizationID);

        return ReturnString;
    }


    public NetworkScene GetSceneByID(int index)
    {
        for (int i = 0; i < Scenes.Count; i++)
        {
            SceneIdentity SI = Scenes[i];
            NetworkScene NS = SI.SceneLink;
            if (NS.SceneID == index)
                return NS;
        }
        Debug.Log("Scene Not Found");
        return null;
    }




#if UNITY_EDITOR
    [HideInEditorMode]
    [Button(ButtonSizes.Medium)]
    public void JumpToPortalJumpTool()
    {
        Selection.activeGameObject = QuickFind.GameSettings.gameObject;
    }
#endif
}
