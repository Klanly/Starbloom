using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_DebugSwordSwing : MonoBehaviour {

    public Transform SwingPivot = null;
    public float StartPoint;
    public float EndPoint;
    public float Speed;


    private void OnEnable()
    {
        SyncPos();
        SwingPivot.localEulerAngles = new Vector3(0, StartPoint, 0);
    }

    private void Update()
    {
        SyncPos();
        SwingPivot.localEulerAngles = new Vector3(0, SwingPivot.localEulerAngles.y + (Speed * Time.deltaTime), 0);
        if (SwingPivot.localEulerAngles.y < 180 && SwingPivot.localEulerAngles.y > EndPoint) { this.gameObject.SetActive(false); }
    }

    void SyncPos()
    {
        transform.position = QuickFind.PlayerTrans.position;
        transform.eulerAngles = QuickFind.PlayerTrans.eulerAngles;
    }

}
