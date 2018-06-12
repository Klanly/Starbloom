using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_FXHandler : MonoBehaviour {



    [HideInInspector]
    public DG_FXObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;


    private void Awake()
    {
        QuickFind.FXHandler = this;
    }


    public DG_FXObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }

    public void PlayEffect(int ClusterID, Vector3 Position, Vector3 Rotation)
    {
        DG_FXObject FXObj = GetItemFromID(ClusterID);
        DG_FXObject.FXClusterObject[] COs = FXObj.ClusterObjects;

        for(int i = 0; i < COs.Length; i++)
        {
            DG_FXObject.FXClusterObject CO = COs[i];
            GameObject FXObject = QuickFind.PrefabPool.GetPoolItemByFXID(CO.PoolID);
            float ScaleMult = CO.ObjectScale * FXObj.GlobalScale;
            Transform T = FXObject.transform;

            T.position = Position;
            T.eulerAngles = Rotation;

            Vector3 Scale = new Vector3(ScaleMult, ScaleMult, ScaleMult);
            T.localScale = Scale;

            ParticleSystem PS = FXObject.GetComponent<ParticleSystem>();

            if (CO.UseCustomSphereRadius) { var shape = PS.shape; shape.radius = CO.CustomSphereRadius; }
            if(CO.UseCustomGravityModifier) { var Main = PS.main; Main.gravityModifier = CO.CustomGravityModifier; }

            PS.Play();
        }
    }
}
