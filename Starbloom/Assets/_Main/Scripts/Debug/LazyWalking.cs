using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyWalking
{
    public bool isFound;
    public float X;
    public float Z;


	public void InternalUpdate ()
    {
        bool SetNew = false;
        if (Input.GetMouseButton(2)) { isFound = true; SetNew = true; }
        if (Input.GetMouseButtonUp(2)) { isFound = false; return; }
        if (Input.GetMouseButton(1) || Input.GetMouseButton(0)) isFound = false;

        if (SetNew)     
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 PlaneAdjusted = new Vector3(0, QuickFind.PlayerTrans.position.y, 0);
            Plane hPlane = new Plane(Vector3.up, PlaneAdjusted);
            float distance = 0;
            if (hPlane.Raycast(ray, out distance))
            {
                Vector3 Pos1 = QuickFind.PlayerTrans.position;
                Vector3 Pos2 = ray.GetPoint(distance);
                Vector3 delta = (Pos2 - Pos1);
                Vector3 direction = delta.normalized;

                X = direction.x;
                Z = direction.z;
            }
        }
    }
}
