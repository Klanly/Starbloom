using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ActivateGOAfterDelay : MonoBehaviour {

    public GameObject ActivatedGameObject;
    public float Delay = 1;

    float Timer;
    bool CheckTime = false;

    // Use this for initialization
    void OnEnable()
    {
        ActivatedGameObject.SetActive(false);
        CheckTime = true;
        Timer = Delay;
    }

    private void Update()
    {
        if (!CheckTime)
            return;

        Timer = Timer - Time.deltaTime;
        if (Timer < 0)
        {
            ActivatedGameObject.SetActive(true);
            CheckTime = false;
        }
    }
}
