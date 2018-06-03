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
        NetworkObject No = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        if (!No.HasBeenWatered)
        {
            int[] Sent = new int[2];
            Sent[0] = QuickFind.NetworkSync.CurrentScene;
            Sent[1] = No.NetworkObjectID;

            QuickFind.NetworkSync.WaterNetworkObject(Sent);
        }
    }
    public void WaterOne(int[] Data)
    {
        NetworkObject No = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        No.HasBeenWatered = true;
        AdjustWateredObjectVisual(No, true);

        if(No.SurrogateObjectIndex != 0)
        {
            NetworkObject NoSurrogate = QuickFind.NetworkObjectManager.GetItemByID(Data[0], No.SurrogateObjectIndex);
            NoSurrogate.HasBeenWatered = true;
            AdjustWateredObjectVisual(NoSurrogate, true);
        }

    }

    public void AdjustWateredObjectVisual(NetworkObject No, bool isWatered)
    {
        //Debug Purposes - To be Better thought out later.
        MeshRenderer MRend = No.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        if (MRend != null)
        {
            Material MR = MRend.material;
            Color C = MR.color;
            if (isWatered)
            {
                C.r = .3f; C.g = .3f; C.b = .3f;
            }
            else
            {
                C.r = 1; C.g = 1; C.b = 1;
            }
            MR.color = C;
        }
    }
}
