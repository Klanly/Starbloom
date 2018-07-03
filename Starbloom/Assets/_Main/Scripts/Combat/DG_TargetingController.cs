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
        if (!QuickFind.CombatHandler.WeaponActive) return;
        if (QuickFind.GUI_OverviewTabs.UIisOpen) return;
        if (QuickFind.GUI_Inventory.InventoryIsOpen) return;

        if (QuickFind.InputController.MainPlayer.Targeting == DG_PlayerInput.ContextDetection.MousePosition) //Detect Enemies from Mouse
        {
            Ray ray = QuickFind.PlayerCam.MainCam.ScreenPointToRay(Input.mousePosition);
            SphereCastAll(ray.origin, ray.direction, FromCamDistance);
        }
        else if (QuickFind.InputController.MainPlayer.Targeting == DG_PlayerInput.ContextDetection.MiddleScreenPosition)
            SphereCastAll(QuickFind.PlayerCam.MainCam.transform.position, QuickFind.PlayerCam.MainCam.transform.forward, FromCamDistance);
        else //Detect Enemies In Front of Player
            SphereCastAll(QuickFind.PlayerTrans.position, QuickFind.PlayerTrans.forward, FromCharDistance);

        if (PlayerTarget != null)
        {
            TargetingCanvas.gameObject.SetActive(true);
            TargetingCanvas.position = PlayerTarget.position;
            TargetingCanvas.LookAt(QuickFind.PlayerCam.MainCam.transform);
        }
        else
            TargetingCanvas.gameObject.SetActive(false);
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
