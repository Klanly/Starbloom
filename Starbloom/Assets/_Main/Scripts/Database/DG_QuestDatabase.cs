using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_QuestDatabase : MonoBehaviour {

    [System.NonSerialized]
    public DG_QuestObject[] QuestList;

    private void Awake()
    {
        QuickFind.QuestDatabase = this;

        int Counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
                Counter++;
        }


        QuestList = new DG_QuestObject[Counter];
        int index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                QuestList[index] = Child.GetChild(iN).GetComponent<DG_QuestObject>();
                index++;
            }
        }
    }


    public DG_QuestObject GetItemFromID(int ID)
    {
        DG_QuestObject ReturnItem;
        for (int i = 0; i < QuestList.Length; i++)
        {
            ReturnItem = QuestList[i];
            if (ReturnItem.DatabaseID == ID)
                return ReturnItem;
        }
        Debug.Log("Get By ID Failed - Note: this may trigger if a 'Quest Required' has an invalid database number in a quest object.");
        return null;
    }
    public DG_QuestObject GetItemFromIDInEditor(int ID)
    {
        DG_QuestObject ReturnItem;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);

            for (int iN = 0; iN < Child.childCount; iN++)
            {
                ReturnItem = Child.GetChild(iN).GetComponent<DG_QuestObject>();
                if (ReturnItem.DatabaseID == ID)
                    return ReturnItem;
            }
        }
        Debug.Log("Get By ID Failed");
        return null;
    }
}
