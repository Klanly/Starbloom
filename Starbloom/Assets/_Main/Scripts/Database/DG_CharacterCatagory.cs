using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterCatagory : MonoBehaviour {

    public int DatabaseID;

    [System.NonSerialized]
    public DG_CharacterObject[] CharacterList;

    private void Awake()
    {
        CharacterList = new DG_CharacterObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            CharacterList[i] = transform.GetChild(i).GetComponent<DG_CharacterObject>();
    }
}
