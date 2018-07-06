using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AI_Master : MonoBehaviour {



    private void Awake()
    {
        QuickFind.AIMasterRef = this;
    }
}
