using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_EnemyObject : MonoBehaviour {

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string Name;


    [System.Serializable]
    public class EnemyResistances
    {
        public DG_CombatHandler.DamageTypes Type;
        public int Value;
    }




    public GameObject PrefabRef;
    public bool UsePoolIDForSpawn;
    [ShowIf("UsePoolIDForSpawn")]
    public int PoolID;
    public float DefaultScale = 1;
    [Header("AI Shared Variables")]
    public DG_AIEntityMovement.MovementOptions MovementSettings;
    public DG_AIEntityDetection.DetectionOptions DetectionSettings;
    public DG_AIEntityCombat.CombatOptions CombatSettings;
    [Header("Health")]
    public int HealthValue;
    [Header("Resistance")]
    [InfoBox("Resistances are percentage 0 - 100.  100 will be full resistant, Beyond 100 Will Heal the Enemy, Below 0 will Amplified Damage.")]
    public EnemyResistances[] Resistances;


    [Button(ButtonSizes.Small)] public void SetNameToPrefab() { Name = PrefabRef.name; }
}
