using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_MovementSync : MonoBehaviour {

    public DG_AnimationSync AnimSync;
    DG_NetworkSyncRates.SyncRateModule SyncRate;
    [HideInInspector] public bool isPlayer;
    [System.NonSerialized] public DG_NetworkSync.Users UserOwner;
    [HideInInspector] public float Distance;
    float Timer;
    Transform _T;



    Vector3 KnownPosition;
    Vector3 KnownHeading;
    int[] OutData = new int[5];


    private void Awake()
    {
        _T = transform;
    }




    private void Update()
    {
        if (QuickFind.NetworkSync == null) return;
        if (SyncRate == null) SyncRate = QuickFind.NetworkSyncRates.GetSyncModuleByType(DG_NetworkSyncRates.SyncRateTypes.Player);

        if (isPlayer)
        {
            Timer = Timer - Time.deltaTime;
            if (Timer < 0)
            {
                Timer = SyncRate.SendRate;
                SendOutPlayerPosition();
            }
        }
        else
        {
            if (HaveUserOwner() && UserOwner.SceneID != QuickFind.NetworkSync.CurrentScene)
                transform.position = new Vector3(0, 10000, 0);
            else
            {
                if (!QuickFind.WithinDistanceVec(transform.position, KnownPosition, SyncRate.MaxDistance))
                {
                    _T.position = KnownPosition;
                    Distance = 0;
                }
                else
                {
                    int AdditiveSpeedMultiplier = 1;
                    Distance = Vector3.Distance(_T.position, KnownPosition);
                    if (Distance > 2) AdditiveSpeedMultiplier = (int)Distance;
                    float SlerpRate = SyncRate.SlerpMoveRate * AdditiveSpeedMultiplier;
                    _T.position = Vector3.MoveTowards(_T.position, KnownPosition, SlerpRate);
                }
                _T.eulerAngles = QuickFind.AngleLerp(_T.eulerAngles, KnownHeading, SyncRate.SlerpTurnRate);
            }
        }
    }


    void SendOutPlayerPosition()
    {
        if (OutData == null) OutData = new int[5];

        OutData[0] = QuickFind.NetworkSync.UserID;
        Vector3 Pos = _T.position;
        float Dir = _T.eulerAngles.y;
        OutData[1] = QuickFind.ConvertFloatToInt(Pos.x);
        OutData[2] = QuickFind.ConvertFloatToInt(Pos.y);
        OutData[3] = QuickFind.ConvertFloatToInt(Pos.z);
        OutData[4] = QuickFind.ConvertFloatToInt(Dir);

        QuickFind.NetworkSync.UpdatePlayerMovement(OutData);
    }

    public void UpdatePlayerPos(int[] InData)
    {
        KnownPosition = QuickFind.ConvertIntsToPosition(InData[1], InData[2], InData[3]);
        KnownHeading.y = QuickFind.ConvertIntToFloat(InData[4]);
    }





    bool HaveUserOwner()
    {
        if (UserOwner != null) return true;
        else
        {
            if (QuickFind.NetworkSync == null) return false;
            DG_NetworkSync.Users User = QuickFind.NetworkSync.GetUserByCharacterLink(transform.GetComponent<DG_CharacterLink>());
            if (User == null) return false;
            else { UserOwner = User; return true; }
        }
    }
}
