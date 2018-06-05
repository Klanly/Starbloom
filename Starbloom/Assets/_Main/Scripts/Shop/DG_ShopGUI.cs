using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShopGUI : MonoBehaviour {



    [Header("Canvases")]
    public CanvasGroup UICanvas = null;


    private void Awake()
    {
        QuickFind.ShopGUI = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
    }
}
