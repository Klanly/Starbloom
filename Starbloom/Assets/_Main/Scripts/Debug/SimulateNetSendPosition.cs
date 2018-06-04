using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateNetSendPosition : MonoBehaviour {

    [Header("Network Send Rate")]
    public float SendRate;

    [Header("Catch up Values")]
    public float SlerpMoveRate;
    public float SlerpTurnRate;
    public float MaxDistance;

    [Header("Objects")]
    public float PlayerMoveSpeed;
    public float PlayerSpinRate;
    public Transform ActualPosition;
    public Transform ShowPosition;
    public Transform PointA;
    public Transform PointB;

    [Header("DO Testing")]
    public bool DoMove = true;
    public bool DoSpin = true;

    Transform TargetPoint;
    float Timer;
    bool MoveTowardsA = false;
    Vector3 KnownPosition;
    Vector3 KnownHeading;
    float PauseTime = 3;


    private void Awake()
    {
        TargetPoint = PointB;
    }


    private void Update()
    {
        //Actual Position
        if(DoMove) ActualPosition.position = Vector3.MoveTowards(ActualPosition.position, TargetPoint.position, PlayerMoveSpeed);
        if(DoSpin) ActualPosition.Rotate(new Vector3(0, PlayerSpinRate, 0));
        if (QuickFind.WithinDistance(ActualPosition, TargetPoint, .2f))
        {
            //Simulated Position
            PauseTime = PauseTime - Time.deltaTime;
            if (PauseTime < 0)
            {
                PauseTime = 3;
                if (MoveTowardsA) { MoveTowardsA = false; TargetPoint = PointB; }
                else { MoveTowardsA = true; TargetPoint = PointA; }
            }
        }


        //Simulated Position
        Timer = Timer - Time.deltaTime;
        if (Timer < 0)
        {
            Timer = SendRate;
            SendOutPlayerPosition();
        }

        int AdditiveSpeedMultiplier = 1;
        float Distance = Vector3.Distance(ShowPosition.position, KnownPosition);
        if (Distance > 2) AdditiveSpeedMultiplier = (int)Distance;

        ShowPosition.position = Vector3.MoveTowards(ShowPosition.position, KnownPosition, SlerpMoveRate * AdditiveSpeedMultiplier);
        ShowPosition.eulerAngles = QuickFind.AngleLerp(ShowPosition.eulerAngles, KnownHeading, SlerpTurnRate);
    }


    void SendOutPlayerPosition()
    {
        Vector3 Pos = ActualPosition.position;
        int X = QuickFind.ConvertFloatToInt(Pos.x);
        int Y = QuickFind.ConvertFloatToInt(Pos.y);
        int Z = QuickFind.ConvertFloatToInt(Pos.z);
        int Heading = QuickFind.ConvertFloatToInt(ActualPosition.eulerAngles.y);

        KnownPosition = QuickFind.ConvertIntsToPosition(X, Y, Z);
        KnownHeading.y = QuickFind.ConvertIntToFloat(Heading);
    }
}
