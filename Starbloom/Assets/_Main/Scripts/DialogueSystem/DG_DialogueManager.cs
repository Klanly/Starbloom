using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_DialogueManager : MonoBehaviour {

    [System.NonSerialized]
    public NodeLink[] DialogueList;

    private void Awake()
    {
        QuickFind.DialogueManager = this;

        int Counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
                Counter++;
        }


        DialogueList = new NodeLink[Counter];
        int index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                DialogueList[index] = Child.GetChild(iN).GetComponent<NodeLink>();
                index++;
            }
        }
    }

    public NodeLink GetDialogueFromID(int ID)
    {
        NodeLink ReturnConversation;
        for (int i = 0; i < DialogueList.Length; i++)
        {
            ReturnConversation = DialogueList[i];
            if (ReturnConversation.DatabaseID == ID)
                return ReturnConversation;
        }
        return DialogueList[0];
    }
}
