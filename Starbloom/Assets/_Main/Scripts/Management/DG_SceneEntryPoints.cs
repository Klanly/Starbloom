using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_SceneEntryPoints : MonoBehaviour {


    [System.Serializable]
	public class PortalPoint
    {
        public int PortalID;
        public Transform PortalTransformReference;
        [Button(ButtonSizes.Small)] public void JumpToPoint() { if (!Application.isPlaying) return; QuickFind.PlayerTrans.position = PortalTransformReference.position; QuickFind.PlayerTrans.eulerAngles = PortalTransformReference.eulerAngles; }
    }


    public PortalPoint[] PortalPoints;



#if UNITY_EDITOR
    private void Start()
    {
        QuickFind.GameSettings.GetComponent<DG_SceneJumpTool>().LoadScenePortals(this);
    }
#endif



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
