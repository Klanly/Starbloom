using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class DG_CharacterDatabase : MonoBehaviour {

    [System.NonSerialized]
    public DG_CharacterCatagory[] CatagoryList;

    private void Awake()
    {
        QuickFind.CharacterDatabase = this;

        CatagoryList = new DG_CharacterCatagory[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            CatagoryList[i] = transform.GetChild(i).GetComponent<DG_CharacterCatagory>();
    }



    public DG_CharacterObject GetItemFromID(int Cat, int ID)
    {
        for (int i = 0; i < CatagoryList.Length; i++)
        {
            DG_CharacterCatagory Catagory = CatagoryList[i];
            if(Catagory.DatabaseID == ID)
            {
                for (int iN = 0; iN < Catagory.CharacterList.Length; iN++)
                {
                    DG_CharacterObject Char = Catagory.CharacterList[iN];
                    if (Char.DatabaseID == ID)
                        return Char;
                }
            }
        }
        Debug.Log("Get By ID Failed");
        return null;
    }

    public DG_CharacterObject GetItemFromIDInEditor(int Cat, int ID)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DG_CharacterCatagory Catagory = transform.GetChild(i).GetComponent<DG_CharacterCatagory>();
            if (Catagory.DatabaseID == ID)
            {
                for (int iN = 0; iN < Catagory.transform.childCount; iN++)
                {
                    DG_CharacterObject Char = Catagory.transform.GetChild(i).GetComponent<DG_CharacterObject>();
                    if (Char.DatabaseID == ID)
                        return Char;
                }
            }
        }
        Debug.Log("Get By ID Failed");
        return null;
    }
}
