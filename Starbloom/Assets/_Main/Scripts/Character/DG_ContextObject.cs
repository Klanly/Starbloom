using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NA_ContextObject : MonoBehaviour
{
    public enum ContextTypes
    {
        Conversation,
        Treasure,
        NameChange,
        Vehicle
    }



    public int ContextID;
    public ContextTypes Type;


    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        if (Type == ContextTypes.Conversation)
        {
            Vector3 Pos = transform.position;
            Pos.y += 3;
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(Pos, .6f);
        }
        if (Type == ContextTypes.Treasure)
        {
            Vector3 Pos = transform.position;
            Pos.y += 3;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(Pos, .6f);
        }
        if (Type == ContextTypes.NameChange)
        {
            Vector3 Pos = transform.position;
            Pos.y += 3;
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(Pos, .6f);
        }
        if (Type == ContextTypes.Vehicle)
        {
            Vector3 Pos = transform.position;
            Pos.y += 5;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Pos, 1f);
        }
    }
}
