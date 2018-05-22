using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneIDList : MonoBehaviour {

    public string[] SceneList;





    private void Awake()
    {
        QuickFind.SceneList = this;
    }



    public int GetSceneIDByString(string ST)
    {
        for(int i= 0; i < SceneList.Length; i++)
        {
            if (SceneList[i] == ST)
                return i;
        }
        Debug.Log("Failed To Find Scene - " + ST);
        return -1;
    }
    public string GetSceneById(int ID)
    {
        if (ID < SceneList.Length)
            return SceneList[ID];
        else
        {
            Debug.Log("Scene ID too High - " + ID.ToString());
            return "";
        }
    }
}
