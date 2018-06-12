using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SceneIDList : MonoBehaviour {

    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<string> Scenes;




    private void Awake()
    {
        QuickFind.SceneList = this;
    }



    public int GetSceneIDByString(string ST)
    {
        for(int i= 0; i < Scenes.Count; i++)
        {
            if (Scenes[i] == ST)
                return i;
        }
        Debug.Log("Failed To Find Scene - " + ST);
        if(!Application.isPlaying)
        {
            Debug.Log("Detected Editor Searching for Scene name, and Not Found, Adding to List Now.");
            Scenes.Add(ST);
        }

        return -1;
    }
    public string GetSceneById(int ID)
    {
        if (ID < Scenes.Count)
            return Scenes[ID];
        else
        {
            Debug.Log("Scene ID too High - " + ID.ToString());
            return "";
        }
    }
}
