using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_UICustomGridScroll : MonoBehaviour {

    public RectTransform Mask;
    public RectTransform Grid;

    int ShownGridValue = 0;
    int Positionindex = 0;

    int PlayerID = -2;


    private void Start()
    {
        float GridHeight = Grid.GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize.y;
        float MaskHeight = Mask.GetComponent<RectTransform>().rect.height;
        ShownGridValue = (int)(MaskHeight / GridHeight);
    }

    private void Update()
    {
        float ZoomAxis = QuickFind.InputController.GetPlayerByPlayerID(PlayerID).CamZoomAxis;
        if (ZoomAxis != 0)
        {
            bool Add = false;
            if (ZoomAxis < 0) Add = true;
            AdjustGridPosition(Add);
        }
    }
    public void ResetGrid()
    {
        Positionindex = 0;
        AdjustGridPosition(false);
    }

    void AdjustGridPosition(bool Add)
    {
        int Count = ActiveGridCount();
        float GridHeight = Grid.GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize.y;

        if (Add) Positionindex++;
        else Positionindex--;

        if ((Positionindex + ShownGridValue) > Count) Positionindex--;
        if (Positionindex < 0) Positionindex = 0;

        Grid.localPosition = new Vector3(0, (Positionindex * GridHeight), 0);
    }
    int ActiveGridCount()
    {
        int index = 0;
        for(int i = 0; i < Grid.childCount; i++)
        {
            if (Grid.GetChild(i).gameObject.activeInHierarchy)
                index++;
        }
        return index;
    }
}
