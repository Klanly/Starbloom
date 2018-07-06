using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_SceneEntryObject : MonoBehaviour {


    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string Name;
    [Header("0-359")]
    public float CameraFacing;


    [Button(ButtonSizes.Small)] public void JumpToPoint() { if (!Application.isPlaying) return; QuickFind.PlayerTrans.position = transform.position; QuickFind.PlayerTrans.eulerAngles = transform.eulerAngles; }




    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, .6f);
    }
}
