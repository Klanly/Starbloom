using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_TargetingController : MonoBehaviour {

    public Transform PlayerTarget;
    public Transform TargetingCanvas;
    [Header("SphereCast")]
    public LayerMask EnemyDetection;
    public float SphereCastRadius;
    public float FromCharDistance;
    public float FromCamDistance;



    private void Awake()
    {
        QuickFind.TargetingController = this;
    }




    void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player Player = QuickFind.InputController.Players[i];
            if (Player.CharLink == null) continue;

            if (!QuickFind.CombatHandler.WeaponActive) return;
            if (QuickFind.GUI_OverviewTabs.UIisOpen) return;
            if (QuickFind.GUI_Inventory.InventoryIsOpen) return;

            CameraLogic.UserCameraMode CamMode = Player.CharLink.PlayerCam.CurrentCameraAngle;
            CameraLogic.ContextDetection DetectionMode = CameraLogic.ContextDetection.InfrontPlayer;
            if (CamMode == CameraLogic.UserCameraMode.Isometric) DetectionMode = QuickFind.UserSettings.IsometricEnemyDetectionMode;
            if (CamMode == CameraLogic.UserCameraMode.Thirdperson) DetectionMode = QuickFind.UserSettings.ThirdPersonEnemyDetectionMode;

            if (DetectionMode == CameraLogic.ContextDetection.MousePosition) //Detect Enemies from Mouse
            {
                Ray ray = Player.CharLink.PlayerCam.MainCam.ScreenPointToRay(Input.mousePosition);
                SphereCastAll(ray.origin, ray.direction, FromCamDistance);
            }
            else if (DetectionMode == CameraLogic.ContextDetection.MiddleScreenPosition)
                SphereCastAll(Player.CharLink.PlayerCam.CamTrans.position, Player.CharLink.PlayerCam.CamTrans.forward, FromCamDistance);
            else //Detect Enemies In Front of Player
            {
                if (QuickFind.PlayerTrans == null) return;
                SphereCastAll(QuickFind.PlayerTrans.position, QuickFind.PlayerTrans.forward, FromCharDistance);
            }

            if (PlayerTarget != null)
            {
                TargetingCanvas.gameObject.SetActive(true);
                TargetingCanvas.position = PlayerTarget.position;
                TargetingCanvas.LookAt(Player.CharLink.PlayerCam.CamTrans);
            }
            else
                TargetingCanvas.gameObject.SetActive(false);
        }
    }

    void SphereCastAll(Vector3 Origin, Vector3 Direction, float Distance)
    {
        RaycastHit[] HitObjects = Physics.SphereCastAll(Origin, SphereCastRadius, Direction, Distance, EnemyDetection);

        if 
            (HitObjects.Length == 0) PlayerTarget = null;
        else if
            (HitObjects.Length == 1) PlayerTarget = HitObjects[0].transform;
        else
            PlayerTarget = QuickFind.GetClosestRayHitObject(QuickFind.PlayerTrans.position, HitObjects).transform;
    }
}
