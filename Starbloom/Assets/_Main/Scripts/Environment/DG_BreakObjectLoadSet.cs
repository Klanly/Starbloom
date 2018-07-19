using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_BreakObjectLoadSet : MonoBehaviour {

    public enum BreakObjectLoadType
    {
        Tree
    }

    public BreakObjectLoadType BrokenLoadType;




    public void LoadBrokenType(int SceneID)
    {

        switch(BrokenLoadType)
        {
            case BreakObjectLoadType.Tree: { DG_TreeFall TF = transform.GetComponent<DG_TreeFall>(); TF.QuickSetDestroyed(); } break;
        }
    }
}
