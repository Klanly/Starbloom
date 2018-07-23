using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_TreeFall : MonoBehaviour {

    [Header("Reward")]
    public int TopClusterGroupID;
    public int BottomClusterGroupID;

    [Header("Trans Refs")]
    public Transform PivotPoint;
    public Transform TopVisuals;
    public Transform TreeTopDetectionPoint;
    public Transform CastHelper;

    [Header("Speed")]
    public float FallSpeedAcceleration;
    [Header("Spin")]
    public float SpinRateWhileFalling;

    [Header("ScatterPointReferences")]
    public DG_ScatterPointReference TopScatterRef;
    public DG_ScatterPointReference BottomScatterRef;


    [Header("Break Detection")]
    public float ImpactMinDistance;
    [Header("Water Detection")]
    public int[] WaterLayers;

    float CurrentFallSpeed;

    bool TopBroke;
    bool isFalling = false;


    bool IsOwner = false;




    private void Awake()
    {
        this.enabled = false;
    }




    [Button(ButtonSizes.Small)]
    public void TriggerBreakDebug()
    {
        if(Application.isPlaying)
            BreakMessage(QuickFind.NetworkSync.Player1PlayerCharacter);
    }





    public void QuickSetDestroyed()
    {
        TopBroke = true;
        TopVisuals.gameObject.SetActive(false);
    }





    public void BreakMessage(int PlayerID)
    {
        if (isFalling) return;
        if (!TopBroke) TriggerInitialBreak(PlayerID);
        else SpawnTrunkReward();

    }


    public void TriggerInitialBreak(int PlayerID)
    {
        IsOwner = true;
        isFalling = true;
        TopBroke = true;
        CurrentFallSpeed = 0;
        DetermineDirectionToFall(PlayerID);
    }
    private void Update()
    {
        Vector3 Angle = PivotPoint.transform.localEulerAngles;
        CurrentFallSpeed += ((FallSpeedAcceleration + (Angle.x / 4)) * Time.deltaTime);

        Angle.x += CurrentFallSpeed;
        PivotPoint.transform.localEulerAngles = Angle;

        DetectForImpact();
    }



    void DetermineDirectionToFall(int PlayerID)
    {
        TreeTopDetectionPoint.LookAt(QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).PlayerTrans);

        float FinalDirection = RaycastLoop(180 + TreeTopDetectionPoint.eulerAngles.y, 8, 45);

        SendTreefallEvent(FinalDirection);
    }
    float RaycastLoop(float InitialDirection, int count, float AngleShift)
    {
        for (int i = 0; i < count; i++)
        {
            CastHelper.transform.localEulerAngles = new Vector3(45, 0, 0);
            float YDir = InitialDirection + (AngleShift * i);
            TreeTopDetectionPoint.eulerAngles = new Vector3(0, YDir, 0);
            if (Raycast(100))
            {
                CastHelper.transform.localEulerAngles = new Vector3(45, 30, 0);
                if (!Raycast(100)) continue;
                CastHelper.transform.localEulerAngles = new Vector3(45, -30, 0);
                if (!Raycast(100)) continue;
                return YDir;
            }
        }
        Debug.Log("Jack Shit Places were found to drop this tree, just dropping it at the player. out of the tree's woodland anger, and spite.");
        return 180 + InitialDirection;
    }


    void DetectForImpact()
    {
        if (Raycast(ImpactMinDistance) || PivotPoint.position.y > CastHelper.position.y) TreeHitTheGround();
    }


    void TreeHitTheGround()
    {
        if (IsOwner)
        {
            SpawnReward(TopClusterGroupID, TopScatterRef);
            NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(transform);
            NO.GrowthValue = -1;
            NO.HealthValue = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID).EnvironmentValues[0].ObjectHealth;
        }

        TopScatterRef.GetComponent<DG_FXContextObjectReference>().TriggerBreak();

        isFalling = false;
        TopVisuals.gameObject.SetActive(false);
        this.enabled = false;
    }


    void SpawnReward(int RewardID, DG_ScatterPointReference ScatterRef)
    {
        DG_BreakableObjectItem BOI = QuickFind.BreakableObjectsCompendium.GetItemFromID(RewardID);
        DG_BreakableObjectItem.ItemClump[] IC = BOI.GetBreakReward();
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(transform);

        for (int i = 0; i < IC.Length; i++)
        {
            DG_BreakableObjectItem.ItemClump Clump = IC[i];
            for (int iN = 0; iN < Clump.Value; iN++)
                QuickFind.NetworkObjectManager.CreateNetSceneObject(NO.Scene.SceneID, NetworkObjectManager.NetworkObjectTypes.Item, Clump.ItemID, Clump.ItemQuality, ScatterRef.GetSpawnPoint(), 0, true, ScatterRef.RandomVelocity());
        }
    }

    void SpawnTrunkReward()
    {
        SpawnReward(BottomClusterGroupID, BottomScatterRef);
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(transform);

        //Make this networked?
        BottomScatterRef.GetComponent<DG_FXContextObjectReference>().TriggerBreak();

        QuickFind.NetworkSync.RemoveNetworkSceneObject(NO.Scene.SceneID, NO.NetworkObjectID);
    }




    bool Raycast(float Range)
    {
        RaycastHit hit;
        if (Physics.Raycast(CastHelper.position, CastHelper.TransformDirection(Vector3.forward), out hit, Range))
        {
            for (int i = 0; i < WaterLayers.Length; i++)
            {
                if (hit.transform.gameObject.layer == WaterLayers[i])
                    return false;
            }
            return true;
        }
        return false;
    }





    public void SendTreefallEvent(float FinalDirection)
    {
        int[] OutData = new int[3];
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(transform);
        NetworkScene NS = NO.transform.parent.GetComponent<NetworkScene>();
        OutData[0] = NS.SceneID;
        OutData[1] = NO.NetworkObjectID;
        OutData[2] = QuickFind.ConvertFloatToInt(FinalDirection);

        QuickFind.NetworkSync.SignalTreeFall(OutData);
    }


    public void ReceiveTreefallEvent(int[] Data)
    {
        float FinalDirection = QuickFind.ConvertIntToFloat(Data[2]);

        isFalling = true;
        TopBroke = true;

        PivotPoint.transform.eulerAngles = new Vector3(0, FinalDirection, 0);
        TreeTopDetectionPoint.transform.eulerAngles = new Vector3(0, FinalDirection, 0);
        CastHelper.transform.localEulerAngles = new Vector3(0, 0, 0);
        TopVisuals.SetParent(PivotPoint);
        TreeTopDetectionPoint.SetParent(PivotPoint);

        transform.GetComponent<DG_FXContextObjectReference>().TriggerImpact();

        this.enabled = true;
    }
}
