using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CombatHandler : MonoBehaviour {



    [System.Serializable]
    public class Damages
    {
        public DG_CombatHandler.DamageTypes DamageType;
        public int DamageMin;
        public int DamageMax;
    }

    [System.Serializable]
    public class Resistances
    {
        public DG_CombatHandler.DamageTypes Type;
        public int Value;
    }




    public enum EquipmentSetups
    {
        SingleSword
    }


    public enum DamageTypes
    {
        Slashing
    }


    [System.Serializable]
    public class PlayerCombat
    {
        public GameObject PlayerDashAttackHitboxes;

        [System.NonSerialized] public DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
        [System.NonSerialized] public DG_ItemObject ItemDatabaseReference;
        [System.NonSerialized] public int ActiveSlot;
        [System.NonSerialized] public bool WeaponActive;
        [System.NonSerialized] public EquipmentSetups ActiveEquipmentSetup;
        [System.NonSerialized] public bool AwaitingResponse;
    }

    public PlayerCombat[] Combats;
    public Transform EnemySurroundHelper = null;
    public Transform EnemySurroundNewPoint = null;
    public float PlayerWeaponDashDistance;
    public float PlayerWeaponDashTime;

    int[] Sent;
    int[] PlayerSent;


    private void Awake()
    {
        QuickFind.CombatHandler = this;
        Combats[1].PlayerDashAttackHitboxes.SetActive(false);
    }





    public void InputDetected(bool isUP, int PlayerID)
    {
        bool AllowAction = false;
        if (isUP || QuickFind.GameSettings.AllowActionsOnHold) AllowAction = true;

        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        if (AllowAction)
        {
            Combats[ArrayNum].AwaitingResponse = true;

            QuickFind.CombatHandler.TriggerMeleeDash(PlayerWeaponDashTime, PlayerWeaponDashDistance, PlayerID);
            Combats[ArrayNum].PlayerDashAttackHitboxes.SetActive(true);

            DG_CharacterLink CL = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

            DG_ClothingObject Cloth = QuickFind.ClothingHairManager.GetAttachedClothingReference(CL, DG_ClothingHairManager.ClothHairType.RightHand).ClothingRef;
            CL.AnimationSync.TriggerAnimation(Cloth.AnimationDatabaseNumber);
        }
    }
    public void DashComplete(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        if (!Combats[ArrayNum].AwaitingResponse) return;
        Combats[ArrayNum].AwaitingResponse = false;

        //Not sure how else to handle this from a callback without changing dash controller completely.
        for(int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++)
            QuickFind.NetworkSync.UserList[i].CharacterLink.CenterCharacterX();

        Combats[ArrayNum].PlayerDashAttackHitboxes.SetActive(false);
    }










    public void SetupForHitting(int PlayerID, DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        Combats[ArrayNum].RucksackSlotOpen = Rucksack;
        Combats[ArrayNum].ItemDatabaseReference = Item;
        Combats[ArrayNum].ActiveSlot = slot;

        Combats[ArrayNum].ActiveEquipmentSetup = EquipmentSetups.SingleSword;
        Combats[ArrayNum].WeaponActive = true;
    }

    public void CancelHittingMode(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        Combats[ArrayNum].WeaponActive = false;
    }







    public void TriggerMeleeDash(float DashTime, float NoTargetDistance, int PlayerID)
    {
        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        Transform PlayerTrans = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).PlayerTrans;
        if (QuickFind.TargetingController.Targeting[Array].PlayerTarget != null) PlayerTrans.LookAt(QuickFind.TargetingController.Targeting[Array].PlayerTarget);
        QuickFind.CharacterDashController.DashAction(PlayerID, PlayerTrans, DashTime, NoTargetDistance, true, this.gameObject);
    }






    public void MeleeHitTrigger(int Index, DG_ContextObject CO)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_EnemyObject EO = QuickFind.EnemyDatabase.GetItemFromID(NO.ItemRefID);

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if(Index == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        //New Health Value
        int newHealthValue = CalculateEnemyHealthAfterDamage(PlayerID, NO, EO);
        if (newHealthValue <= 0) { SendKill(NO, CO, EO); }
        else { SendEnemyHitData(CO, NO, newHealthValue); }
    }
    public void RangedHitTrigger(DG_PlayerCharacters.RucksackSlot RucksackSlotOpen, DG_ItemObject ItemDatabaseReference)
    {
        //Todo Magic :D
    }






    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ObjectHitReturn(Collider EnemyHit, int Index)
    {
        //Todo, determine what kind of attack is being done.
        MeleeHitTrigger(Index, QuickFind.TargetingController.Targeting[Index].PlayerTarget.GetComponent<DG_ContextObject>());
    }
    public int CalculateEnemyHealthAfterDamage(int PlayerID, NetworkObject NO, DG_EnemyObject EO)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_ItemObject.Weapon Wep = Combats[ArrayNum].ItemDatabaseReference.WeaponValues[0];
        int HitAmount = Random.Range(Wep.DamageValues.DamageMin, (Wep.DamageValues.DamageMax + 1));
        HitAmount = ScanResistances(HitAmount, EO.EnemyResistances, Wep.DamageValues.DamageType);
        return NO.HealthValue - HitAmount;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public void PlayerHitReturn(Transform EnemyStriking, Collider PlayerHit)
    {
        DG_CharacterLink CharLink = PlayerHit.transform.parent.GetComponent<DG_CharacterLink>();
        if (!CharLink.MoveSync.isPlayer) return;

        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(EnemyStriking);
        DG_EnemyObject EO = QuickFind.EnemyDatabase.GetItemFromID(NO.ItemRefID);

        DG_NetworkSync.Users U = QuickFind.NetworkSync.GetUserByCharacterLink(CharLink);
        DG_PlayerCharacters.PlayerCharacter PC = QuickFind.Farm.PlayerCharacters[U.PlayerCharacterID];
        int CurrentHealth = PC.Energies.CurrentHealth;
        int DamageReceived = CalculatePlayerDamageReceived(EO, PC);
        CurrentHealth -= DamageReceived;

        if (CurrentHealth <= 0) { Debug.Log("Do something when player runs out of health"); }
        else { SendPlayerHitData(U, CurrentHealth); }
    }
    public int CalculatePlayerDamageReceived(DG_EnemyObject EO, DG_PlayerCharacters.PlayerCharacter PC)
    {
        Damages Damage = EO.Damage;
        int HitAmount = Random.Range(Damage.DamageMin, (Damage.DamageMax + 1));
        for (int i = 0; i < PC.Equipment.EquippedClothing.Count; i++)
            HitAmount = ScanResistances(HitAmount, QuickFind.ClothingDatabase.GetItemFromID(PC.Equipment.EquippedClothing[i]).ClothingResistances, Damage.DamageType);
        return HitAmount;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public int ScanResistances(int CurrentHitAmount, Resistances[] ResistanceArray, DamageTypes DamageType)
    {
        int HitAmount = CurrentHitAmount;
        for (int i = 0; i < ResistanceArray.Length; i++)
        {
            Resistances R = ResistanceArray[i];
            if (DamageType == R.Type)
            {
                //Resistance Calculation.  Positive will reduce incoming damage, Negative will Amplify Incoming Damage.
                float Multiplier = ((float)R.Value / 100);
                int SubtractValue = Mathf.RoundToInt(((float)HitAmount * Multiplier));
                HitAmount = HitAmount - SubtractValue;
            }
        }
        return HitAmount;
    }







    /////////////////////////////////////////////////////////////////////////////////////////////////////

    void SendEnemyHitData(DG_ContextObject CO, NetworkObject NO, int NewHealthValue)
    {
        if(Sent == null) Sent = new int[3];
        Sent[0] = NO.Scene.SceneID;
        Sent[1] = NO.NetworkObjectID;
        Sent[2] = NewHealthValue;

        QuickFind.NetworkSync.SendEnemyHit(Sent);
    }

    public void ReceiveEnemyHitData(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        NO.HealthValue = Data[2];
        DG_EnemyObject EO = QuickFind.EnemyDatabase.GetItemFromID(NO.ItemRefID);

        //FX
        Transform Child = NO.transform.GetChild(0);
        Debug.Log("Trigger Enemy Hit FX");
        //Child.GetComponent<DG_FXContextObjectReference>().TriggerImpact();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    void SendPlayerHitData(DG_NetworkSync.Users U, int NewHealthValue)
    {
        if(PlayerSent == null) PlayerSent = new int[2];
        PlayerSent[0] = U.PlayerCharacterID;
        PlayerSent[1] = NewHealthValue;

        QuickFind.NetworkSync.SendPlayerHit(PlayerSent);
    }

    public void ReceivePlayerHitData(int[] Data)
    {
        DG_NetworkSync.Users U = QuickFind.NetworkSync.GetUserByPlayerID(Data[0]);
        DG_PlayerCharacters.PlayerCharacter PC = QuickFind.Farm.PlayerCharacters[U.PlayerCharacterID];
        PC.Energies.CurrentHealth = Data[1];

        //Update Healthbar
        if(QuickFind.NetworkSync.ThisPlayerBelongsToMe(U.PlayerCharacterID))
            QuickFind.GUI_MainOverview.SetGuiHealthValue((float)PC.Energies.CurrentHealth / (float)PC.Energies.MaxHealth, U.PlayerCharacterID);

        //FX
        DG_CharacterLink CharLink = U.CharacterLink;
        Debug.Log("Trigger Player Hit FX");
        //Child.GetComponent<DG_FXContextObjectReference>().TriggerImpact();
    }




    void SendKill(NetworkObject NO, DG_ContextObject CO, DG_EnemyObject EO)
    {
        //int[] OutData = new int[2];
        //OutData[0] = NO.Scene.SceneID;
        //OutData[1] = NO.NetworkObjectID;

        Debug.Log("Trigger Enemy Destroy FX");
        //QuickFind.NetworkSync.PlayDestroyEffect(OutData);
        QuickFind.NetworkSync.RemoveNetworkSceneObject(NO.Scene.SceneID, NO.NetworkObjectID);

        //EXP
        Debug.Log("Reward Player EXP");
        //if (IO.EnvironmentValues[0].ActivateableTypeRequired == HotbarItemHandler.ActivateableTypes.Pickaxe)
        //    QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Mining, DG_ItemObject.ItemQualityLevels.Low);
    }
}
