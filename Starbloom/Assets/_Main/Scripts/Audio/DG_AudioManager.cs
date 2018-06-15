using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AudioManager : MonoBehaviour {



    private void Awake()
    {
        QuickFind.AudioManager = this;
    }
}
