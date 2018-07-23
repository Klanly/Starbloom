using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_TargetingController : MonoBehaviour {

    [System.Serializable]
    public class PlayerTargeting
    {
        public Transform PlayerTarget;
        public Transform TargetingCanvas;
    }

    public PlayerTargeting[] Targeting;

    [Header("SphereCast")]
    public LayerMask EnemyDetection;
    public float SphereCastRadius;
    public float FromCharDistance;
    public float FromCamDistance;



    private void Awake()
    {
        QuickFind.TargetingController = this;
        Targeting[1].TargetingCanvas.gameObject.SetActive(false);
    }




    void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player Player = QuickFind.InputController.Players[i];
            if (Player.CharLink == null) continue;

            UserSettings.PlayerSettings PS = QuickFind.UserSettings.SingleSettings;
            if (QuickFind.NetworkSync.Player2PlayerCharacter != -1) PS = QuickFind.UserSettings.CoopSettings[i];

            if (!QuickFind.CombatHandler.Combats[i].WeaponActive) return;
            if (QuickFind.GUI_OverviewTabs.OverviewTabs[i].UIisOpen) return;
            if (QuickFind.GUI_Inventory.PlayersInventory[i].InventoryIsOpen) return;

            CameraLogic.UserCameraMode CamMode = Player.CharLink.PlayerCam.CurrentCameraAngle;
            CameraLogic.ContextDetection DetectionMode = CameraLogic.ContextDetection.InfrontPlayer;
            if (CamMode == CameraLogic.UserCameraMode.Isometric) DetectionMode = PS.IsometricEnemyDetectionMode;
            if (CamMode == CameraLogic.UserCameraMode.Thirdperson) DetectionMode = PS.ThirdPersonEnemyDetectionMode;

            if (DetectionMode == CameraLogic.ContextDetection.MousePosition) //Detect Enemies from Mouse
            {
                Ray ray = Player.CharLink.PlayerCam.MainCam.ScreenPointToRay(Input.mousePosition);
                SphereCastAll(ray.origin, ray.direction, FromCamDistance, Player);
            }
            else if (DetectionMode == CameraLogic.ContextDetection.MiddleScreenPosition)
                SphereCastAll(Player.CharLink.PlayerCam.CamTrans.position, Player.CharLink.PlayerCam.CamTrans.forward, FromCamDistance, Player);
            else //Detect Enemies In Front of Player
            {
                if (Player.CharLink.PlayerTrans == null) return;
                SphereCastAll(Player.CharLink.PlayerTrans.position, Player.CharLink.PlayerTrans.forward, FromCharDistance, Player);
            }

            if (Targeting[i].PlayerTarget != null)
            {
                Targeting[i].TargetingCanvas.gameObject.SetActive(true);
                Targeting[i].TargetingCanvas.position = Targeting[i].PlayerTarget.position;
                Targeting[i].TargetingCanvas.LookAt(Player.CharLink.PlayerCam.CamTrans);
            }
            else
                Targeting[i].TargetingCanvas.gameObject.SetActive(false);
        }
    }

    void SphereCastAll(Vector3 Origin, Vector3 Direction, float Distance, DG_PlayerInput.Player Player)
    {
        int Array = 0;
        if (Player.CharLink.PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        RaycastHit[] HitObjects = Physics.SphereCastAll(Origin, SphereCastRadius, Direction, Distance, EnemyDetection);

        if 
            (HitObjects.Length == 0) Targeting[Array].PlayerTarget = null;
        else if
            (HitObjects.Length == 1) Targeting[Array].PlayerTarget = HitObjects[0].transform;
        else
            Targeting[Array].PlayerTarget = QuickFind.GetClosestRayHitObject(Player.CharLink.PlayerTrans.position, HitObjects).transform;
    }
}
