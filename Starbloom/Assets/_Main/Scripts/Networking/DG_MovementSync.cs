using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_MovementSync : MonoBehaviour {


    [Header("Network Send Rate")]
    public float SendRate;

    [Header("Catch up Values")]
    public float SlerpMoveRate;
    public float SlerpTurnRate;
    public float MaxDistance;


    [HideInInspector] public bool isPlayer;
    float Timer;
    Transform _T;
    DG_NetworkSync.Users UserOwner = null;

    Vector3 KnownPosition;
    Vector3 KnownHeading;
    int[] OutData = new int[5];


    private void Awake()
    {
        _T = transform;
    }




    private void Update()
    {
        if (isPlayer)
        {
            Timer = Timer - Time.deltaTime;
            if (Timer < 0)
            {
                Timer = SendRate;
                SendOutPlayerPosition();
            }
        }
        else
        {
            if (HaveUserOwner() && UserOwner.SceneID != QuickFind.NetworkSync.CurrentScene)
                transform.position = new Vector3(0, 10000, 0);
            else
            {
                if (!QuickFind.WithinDistanceVec(transform.position, KnownPosition, MaxDistance))
                    _T.position = KnownPosition;
                else
                {
                    int AdditiveSpeedMultiplier = 1;
                    float Distance = Vector3.Distance(_T.position, KnownPosition);
                    if (Distance > 2) AdditiveSpeedMultiplier = (int)Distance;
                    _T.position = Vector3.MoveTowards(_T.position, KnownPosition, SlerpMoveRate * AdditiveSpeedMultiplier);
                }
                _T.eulerAngles = QuickFind.AngleLerp(_T.eulerAngles, KnownHeading, SlerpTurnRate);
            }
        }
    }


    void SendOutPlayerPosition()
    {
        if (OutData == null) OutData = new int[5];

        OutData[0] = QuickFind.NetworkSync.PlayerCharacterID;
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

            PhotonView PV = transform.parent.GetComponent<PhotonView>();
            DG_NetworkSync.Users User = QuickFind.NetworkSync.GetUserByPhotonViewID(PV.viewID);
            if (User == null) return false;
            else { UserOwner = User; return true; }
        }
    }
}
