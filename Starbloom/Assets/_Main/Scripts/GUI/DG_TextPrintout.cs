using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_TextPrintout : MonoBehaviour {

    public class TextPrintObject
    {
        public string FinalString;
        public char[] DisplayCharArray;
        public int CurrentPosition;
        public float TimeTillNextChar;
        public TMPro.TextMeshProUGUI TextMeshObject;
    }


    List<TextPrintObject> PrintObjects;



    private void Awake()
    {
        QuickFind.TextPrintout = this;
        PrintObjects = new List<TextPrintObject>();
    }



    public void AddNewDisplayText(string textToDisplay, TMPro.TextMeshProUGUI TextMeshObject)
    {
        QuickFind.DialogueGUIController.SetGuiState(1);

        TextPrintObject TPO = new TextPrintObject();
        TPO.FinalString = textToDisplay;
        TPO.DisplayCharArray = textToDisplay.ToCharArray();
        TPO.CurrentPosition = 0;
        TPO.TextMeshObject = TextMeshObject;

        float CharDisplayInterval = QuickFind.UserSettings.TextSpeed;
        TPO.TimeTillNextChar = CharDisplayInterval;

        PrintObjects.Add(TPO);
    }
    public void RushAllActiveTextEffects()
    {
        for(int i = 0; i < PrintObjects.Count; i++)
        {
            TextPrintObject TPO = PrintObjects[i];
            TPO.CurrentPosition = TPO.DisplayCharArray.Length;
        }
        Update();
    }



    private void Update()
    {
        if (PrintObjects.Count == 0)
            return;

        float CharDisplayInterval = QuickFind.UserSettings.TextSpeed;
        float DeltaTime = Time.deltaTime;
        List<TextPrintObject> ObjectsToClear = new List<TextPrintObject>();

        for (int i = 0; i < PrintObjects.Count; i++)
        {
            TextPrintObject TPO = PrintObjects[i];
            TPO.TimeTillNextChar -= DeltaTime;
            if(TPO.TimeTillNextChar < 0)
            {
                TPO.CurrentPosition++;
                if(TPO.CurrentPosition < TPO.DisplayCharArray.Length)
                {
                    TPO.TimeTillNextChar = CharDisplayInterval;
                    TPO.TextMeshObject.text = CurrentString(TPO);
                }
                else
                {
                    TPO.TextMeshObject.text = TPO.FinalString;
                    ObjectsToClear.Add(TPO);
                }
            }
        }

        if(ObjectsToClear.Count == PrintObjects.Count)
            QuickFind.DialogueGUIController.SetGuiState(2);

        //Clear Finished Objects
        foreach (TextPrintObject Object in ObjectsToClear)
            PrintObjects.Remove(Object);            
    }

    string CurrentString(TextPrintObject TPO)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder("");
        string CurrLetter = TPO.DisplayCharArray[TPO.CurrentPosition].ToString();

        //Check for Richtext.  Display Richtext Immediately instead of printing it out.
        if (CurrLetter == "<")
        {
            for (int i = TPO.CurrentPosition; i < TPO.DisplayCharArray.Length; i++)
            {
                string value = TPO.DisplayCharArray[i].ToString();
                if (value != ">")
                    TPO.CurrentPosition++;
                else
                    break;
            }
            TPO.CurrentPosition++;
        }

        for(int i = 0; i < TPO.CurrentPosition; i++)
            builder.Append(TPO.DisplayCharArray[i]);


        return builder.ToString();
    }
}
