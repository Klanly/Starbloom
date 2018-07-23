using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ScenePathNode : MonoBehaviour {

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string Name;


    [Button(ButtonSizes.Small)] public void JumpToPoint() { if (!Application.isPlaying) return; Transform PlayerTrans = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(QuickFind.NetworkSync.Player1PlayerCharacter).PlayerTrans; PlayerTrans.position = transform.position; PlayerTrans.eulerAngles = transform.eulerAngles; }


    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, .4f);
    }
}
