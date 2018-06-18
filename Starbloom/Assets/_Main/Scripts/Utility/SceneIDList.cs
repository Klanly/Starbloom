using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SceneIDList : MonoBehaviour {

<<<<<<< .merge_file_a07372
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<string> Scenes;
=======
    [System.Serializable]
    public class SceneIdentity
    {
        public string SceneName;

        public bool ShowLocalization;

        [Header("Localization")]
        [ShowIf("ShowLocalization")]
        public int LocalizationID;
        [ShowIf("ShowLocalization")]
        public bool AppendFarmName;
        [ShowIf("ShowLocalization")]
        public bool AppendPlayerName;
    }


    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<SceneIdentity> Scenes;
>>>>>>> .merge_file_a14792




    private void Awake()
    {
        QuickFind.SceneList = this;
    }



<<<<<<< .merge_file_a07372
    public int GetSceneIDByString(string ST)
    {
        for(int i= 0; i < Scenes.Count; i++)
        {
            if (Scenes[i] == ST)
=======
    public int GetSceneIndexByString(string ST)
    {
        for(int i= 0; i < Scenes.Count; i++)
        {
            if (Scenes[i].SceneName == ST)
>>>>>>> .merge_file_a14792
                return i;
        }
        Debug.Log("Failed To Find Scene - " + ST);
        if(!Application.isPlaying)
        {
            Debug.Log("Detected Editor Searching for Scene name, and Not Found, Adding to List Now.");
<<<<<<< .merge_file_a07372
            Scenes.Add(ST);
=======
            SceneIdentity SI = new SceneIdentity();
            SI.SceneName = ST;
            Scenes.Add(SI);
>>>>>>> .merge_file_a14792
        }

        return -1;
    }
<<<<<<< .merge_file_a07372
    public string GetSceneById(int ID)
=======
    public SceneIdentity GetSceneByString(string ST)
    {
        return Scenes[GetSceneIndexByString(ST)];
    }

    public SceneIdentity GetSceneById(int ID)
>>>>>>> .merge_file_a14792
    {
        if (ID < Scenes.Count)
            return Scenes[ID];
        else
        {
            Debug.Log("Scene ID too High - " + ID.ToString());
<<<<<<< .merge_file_a07372
            return "";
        }
    }
=======
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
>>>>>>> .merge_file_a14792
}
