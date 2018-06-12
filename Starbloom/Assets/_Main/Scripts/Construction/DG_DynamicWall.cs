using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_DynamicWall : MonoBehaviour {

    public enum WallMessage
    {
        North,
        South,
        East,
        West
    }

    [Header("Wall Pieces")]
    public GameObject CenterPiece;
    public GameObject NorthPiece;
    public GameObject SouthPiece;
    public GameObject EastPiece;
    public GameObject WestPiece;


    [Header("Wall Detection")]
    public Transform RaycastPoint;
    public LayerMask WallDetection;
    RaycastHit hit;


    bool NorthActive = false;
    bool SouthActive = false;
    bool EastActive = false;
    bool WestActive = false;


    private void Awake() { this.enabled = false; }
    private void Update() { RaycastDirections(false); }

    public void TriggerPlacementMode() { this.enabled = true; }
    public void TriggerPlaceWall() { this.enabled = false; RaycastDirections(true); }


    void SetActiveChildren()
    {
        NorthPiece.SetActive(NorthActive);
        SouthPiece.SetActive(SouthActive);
        EastPiece.SetActive(EastActive);
        WestPiece.SetActive(WestActive);
    }


    void RaycastDirections(bool SendOutNetMessage)
    {
        NorthActive = false; SouthActive = false; EastActive = false; WestActive = false;

        Vector3 CastPos = RaycastPoint.position;
        if (Raycast(CastPos, RaycastPoint.TransformDirection(Vector3.forward)))
        { NorthActive = true; if (SendOutNetMessage) OutGoingWallMessage(WallMessage.South); }
        if (Raycast(CastPos, RaycastPoint.TransformDirection(Vector3.back)))
            { SouthActive = true; if (SendOutNetMessage) OutGoingWallMessage(WallMessage.North); }
        if (Raycast(CastPos, RaycastPoint.TransformDirection(Vector3.right)))
            { EastActive = true; if (SendOutNetMessage) OutGoingWallMessage(WallMessage.West); }
        if (Raycast(CastPos, RaycastPoint.TransformDirection(Vector3.left)))
            { WestActive = true; if (SendOutNetMessage) OutGoingWallMessage(WallMessage.East); }  

        if (SendOutNetMessage)
            QuickFind.NetworkObjectManager.CreateNetSceneObject(QuickFind.NetworkSync.CurrentScene, QuickFind.ObjectPlacementManager.ItemDatabaseReference.DatabaseID, DetermineVisualID(), transform.position, 0);
        else
            SetActiveChildren();
    }

    bool Raycast(Vector3 Position, Vector3 Direction)
    {
        if (Physics.Raycast(Position, Direction, out hit, 1.2f, WallDetection))
        { if (hit.transform.parent.GetComponent<DG_DynamicWall>() != null)
                return true;
            else return false;
        }
        else return false;
    }






    int DetermineVisualID()
    {
        int Return = 0;
        if (NorthActive) Return += 1;
        if (SouthActive) Return += 10;
        if (EastActive) Return += 100;
        if (WestActive) Return += 1000;

        return Return;
    }
    public void DetermineActiveBoolsByID(int VisualID)
    {
        int North = QuickFind.GetIntAtDigit(VisualID, 0);
        int South = QuickFind.GetIntAtDigit(VisualID, 1);
        int East =  QuickFind.GetIntAtDigit(VisualID, 2);
        int West =  QuickFind.GetIntAtDigit(VisualID, 3);

        if (North == 1) NorthActive = true; else NorthActive = false;
        if (South == 1) SouthActive = true; else SouthActive = false;
        if (East == 1) EastActive = true; else EastActive = false;
        if (West == 1) WestActive = true; else WestActive = false;

        SetActiveChildren();
    }







    void OutGoingWallMessage(WallMessage Value)
    {
        int[] OutData = new int[3];
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(hit.transform);
        NetworkScene NS = NO.transform.parent.GetComponent<NetworkScene>();
        OutData[0] = NS.SceneID;
        OutData[1] = NO.NetworkObjectID;
        OutData[2] = (int)Value;
        QuickFind.NetworkSync.WallAdjustMessage(OutData);
    }

    public void IncomingWallMessage(int Value)
    {
        WallMessage Incoming = (WallMessage)Value;
        if (Incoming == WallMessage.North) NorthActive = true;
        if (Incoming == WallMessage.South) SouthActive = true;
        if (Incoming == WallMessage.East) EastActive = true;
        if (Incoming == WallMessage.West) WestActive = true;

        int NewVisualID = DetermineVisualID();
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(transform);
        NO.ItemQualityLevel = NewVisualID;

        DetermineActiveBoolsByID(NewVisualID);
    }
}
