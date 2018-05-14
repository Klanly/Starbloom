using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ColorCodes : MonoBehaviour {

    [System.Serializable]
	public class ColorCode
    {
        public string ColorID;
        public string DevNotes;
        public int DatabaseID;
        public Color Color;   
    }

    public ColorCode[] Codes;


    private void Awake()
    {
        QuickFind.ColorDatabase = this;
    }




    public Color GetColorByID(int ID)
    {
        ColorCode CC = GetColorCode(ID);
        return CC.Color;
    }
    public string GetTextColorByID(int ID)
    {
        ColorCode CC = GetColorCode(ID);
        return "<color=#" + CC.ColorID + ">";
    }
    ColorCode GetColorCode(int ID)
    {
        ColorCode ReturnItem;
        for (int i = 0; i < Codes.Length; i++)
        {
            ReturnItem = Codes[i];
            if (ReturnItem.DatabaseID == ID)
                return ReturnItem;
        }
        Debug.Log("Get Color By ID Failed");
        return null;
    }
}
