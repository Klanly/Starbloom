using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_SceneEntryPoints : MonoBehaviour {




    [System.Serializable]
	public class PortalPoint
    {
        public int PortalID;
        public Transform PortalTransformReference;
    }


    public PortalPoint[] PortalPoints;





    public PortalPoint GetStartingPositionByID(int ID)
    {
        for(int i = 0; i < PortalPoints.Length; i++)
        {
            if (ID == PortalPoints[i].PortalID)
                return PortalPoints[i];
        }
        Debug.Log("No Portal point found by that ID");
        return null;
    }




    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        if (PortalPoints == null) return;
        if (PortalPoints.Length == 0) return;

        Gizmos.color = Color.magenta;

        for (int i = 0; i < PortalPoints.Length; i++)
            Gizmos.DrawSphere(PortalPoints[i].PortalTransformReference.position, .6f);
    }
}
