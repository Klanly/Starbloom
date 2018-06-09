using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_BreakObjectLoadSet : MonoBehaviour {

    public enum BreakObjectLoadType
    {
        Tree
    }

    public BreakObjectLoadType BrokenLoadType;




    public void LoadBrokenType()
    {

        switch(BrokenLoadType)
        {
            case BreakObjectLoadType.Tree: { transform.GetComponent<DG_TreeFall>().QuickSetDestroyed(); } break;
        }
    }
}
