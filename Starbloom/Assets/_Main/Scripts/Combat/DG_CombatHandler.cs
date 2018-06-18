using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CombatHandler : MonoBehaviour {

    public enum DamageTypes
    {
        Slashing
    }


    [HideInInspector] public DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    [HideInInspector] public DG_ItemObject ItemDatabaseReference;
    [HideInInspector] public int ActiveSlot;



    private void Awake()
    {
        QuickFind.CombatHandler = this;
    }



    public void MeleeHitTrigger(DG_ContextObject CO)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_EnemyObject EO = QuickFind.EnemyDatabase.GetItemFromID(NO.ItemRefID);


        //New Health Value
        int Hitvalue = CalculateDamageValue(ItemDatabaseReference, EO);
        int newHealthValue = NO.HealthValue - Hitvalue;

        if (newHealthValue <= 0) { SendKill(NO, CO, EO); }
        else { SendHitData(CO, NO, newHealthValue); }
    }
    public void RangedHitTrigger(DG_PlayerCharacters.RucksackSlot RucksackSlotOpen, DG_ItemObject ItemDatabaseReference)
    {
        //Todo Magic :D
    }




    int CalculateDamageValue(DG_ItemObject WeaponReference, DG_EnemyObject EnemyReference)
    {
        DG_ItemObject.Weapon Wep = WeaponReference.WeaponValues[0];
        DamageTypes Type = Wep.DamageType;

        int HitAmount = Random.Range(Wep.DamageMin, (Wep.DamageMax + 1));
        for(int i = 0; i < EnemyReference.Resistances.Length; i++)
        {
            DG_EnemyObject.EnemyResistances R = EnemyReference.Resistances[i];
            if (Type == R.Type)
            {
                //Resistance Calculation.  Positive will reduce incoming damage, Negative will Amplify Incoming Damage.
                float Multiplier = ((float)R.Value / 100);
                int SubtractValue = Mathf.RoundToInt(((float)HitAmount * Multiplier));
                HitAmount = HitAmount - SubtractValue; 
            }
        }
        return HitAmount;
    }







    void SendHitData(DG_ContextObject CO, NetworkObject NO, int NewHealthValue)
    {
        int[] Sent = new int[3];
        Sent[0] = QuickFind.NetworkSync.CurrentScene;
        Sent[1] = NO.NetworkObjectID;
        Sent[2] = NewHealthValue;

        QuickFind.NetworkSync.SendEnemyHit(Sent);
    }

    public void ReceiveHitData(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        NO.HealthValue = Data[2];
        DG_EnemyObject EO = QuickFind.EnemyDatabase.GetItemFromID(NO.ItemRefID);

        //FX
        Transform Child = NO.transform.GetChild(0);
        Debug.Log("Trigger Enemy Hit FX");
        //Child.GetComponent<DG_FXContextObjectReference>().TriggerImpact();
    }




    void SendKill(NetworkObject NO, DG_ContextObject CO, DG_EnemyObject EO)
    {
        int[] OutData = new int[2];
        NetworkScene NS = NO.transform.parent.GetComponent<NetworkScene>();
        OutData[0] = NS.SceneID;
        OutData[1] = NO.NetworkObjectID;

        Debug.Log("Trigger Enemy Destroy FX");
        //QuickFind.NetworkSync.PlayDestroyEffect(OutData);
        QuickFind.NetworkSync.RemoveNetworkSceneObject(QuickFind.NetworkSync.CurrentScene, NO.NetworkObjectID);

        //EXP
        Debug.Log("Reward Player EXP");
        //if (IO.EnvironmentValues[0].ActivateableTypeRequired == HotbarItemHandler.ActivateableTypes.Pickaxe)
        //    QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Mining, DG_ItemObject.ItemQualityLevels.Low);
    }
}
