using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_TextLetterDisplay : MonoBehaviour {

    public TMPro.TextMeshProUGUI TextLetter = null;
    public UnityEngine.UI.Image Underline = null;

    [HideInInspector] public bool CurrentlyActive = false;
    [HideInInspector] public bool HasLetter = false;
    [HideInInspector] public bool IsSpace = false;

    float Timer;


    private void Awake()
    {
        this.enabled = false;
    }

    public void QueueActivate(bool Blink)
    {
        CurrentlyActive = true;
        Underline.enabled = true;
        Underline.color = Color.yellow;
        this.enabled = Blink;
    }
    public void QueueDeactivate(bool DisplayUnderline)
    {
        if (DisplayUnderline)
        {
            Underline.color = Color.white;
            Underline.enabled = true;
        }
        else
            Underline.enabled = false;

        this.enabled = false;
    }


    private void Update()
    {
        if (Timer > 0)
            Timer -= Time.deltaTime;
        else
        {
            Underline.enabled = !Underline.isActiveAndEnabled;
            Timer = .5f;
        }
    }
}
