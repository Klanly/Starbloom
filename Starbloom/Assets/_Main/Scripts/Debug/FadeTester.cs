using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class FadeTester : MonoBehaviour {

    public DG_GUI_FadeScreen.FadeInSpeeds TestSpeed;

    [ButtonGroup]
    void FadeIn()
    {
        QuickFind.FadeScreen.FadeIn(TestSpeed, this.gameObject, "FadeInComplete");
    }

    [ButtonGroup]
    void FadeOut()
    {
        QuickFind.FadeScreen.FadeOut(TestSpeed, this.gameObject, "FadeOutComplete");
    }




    public void FadeInComplete()
    {
        Debug.Log("Fade In Complete Worked!");
    }
    public void FadeOutComplete()
    {
        Debug.Log("Fade Out Complete Worked!");
    }
}
