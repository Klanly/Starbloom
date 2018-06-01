using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_WateringSystem : MonoBehaviour {


    private void Awake()
    {
        QuickFind.WateringSystem = this;
    }






    public void WaterObject(DG_ContextObject CO)
    {
        NetworkObject No = CO.transform.parent.GetComponent<NetworkObject>();
        if (!No.HasBeenWatered)
        {
            int[] Sent = new int[2];
            Sent[0] = QuickFind.NetworkSync.CurrentScene;
            Sent[1] = CO.transform.parent.GetSiblingIndex();

            QuickFind.NetworkSync.WaterNetworkObject(Sent);
        }
    }
    public void WaterOne(int[] Data)
    {
        NetworkObject No = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        No.HasBeenWatered = true;
        AdjustWateredObjectVisual(No);
    }

    public void AdjustWateredObjectVisual(NetworkObject No)
    {
        //Debug Purposes
        Material MR = No.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        Color C = MR.color;
        C.r = .3f; C.g = .3f; C.b = .3f;
        MR.color = C;
    }
}
