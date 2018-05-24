using System.Collections;
using System.Collections.Generic;
using UnityEngine;







public class DG_TextLetterGroup : MonoBehaviour {

    public class LetterRow
    {
        public Transform RowRoot;
        public DG_TextLetterDisplay[] Column;
    }
    [HideInInspector] public LetterRow[] Row;

    
    public int ColumnMaxCount;
    public int RowMaxCount;
    [Header("Adaptive")]
    public bool AdaptiveToString;
    public bool RemoveExcessLetterSpots;
    public bool ShowUnderlines;
    public bool BlinkActive;


    char[] CharCount;
    bool CharDis;
    bool FirstDisplay;
    int CatVal;
    int IDVal;


    Transform RowDefault;
    List<Transform> DeletionRows;




    private void Awake()
    {
        RowDefault = transform.GetChild(0);
        transform.GetChild(0).gameObject.SetActive(false);
        DeletionRows = new List<Transform>();
    }



    public void OpenUI(bool IsChar, int IDValue, int CatValue = 0, bool FirstTimeDisplay = true)
    {
        if (!Application.isPlaying)
            return;

        CharDis = IsChar;
        CatVal = CatValue;
        IDVal = IDValue;
        FirstDisplay = FirstTimeDisplay;
        string WordVal = string.Empty;
        if (CharDis)
            WordVal = QuickFind.WordDatabase.GetNameFromID(1, IDVal, FirstDisplay);
        else
            WordVal = QuickFind.WordDatabase.GetWordFromID(IDVal);
        GenerateTextRegion(WordVal);
    }






    void GenerateTextRegion(string DisplayName)
    {
        if (AdaptiveToString)
            GenerateRowMaxCount(DisplayName);


        MarkRowsForDeletion();
        GenerateRows();
        GenerateColumns();
        DeleteRows();
    }
    void GenerateRowMaxCount(string DisplayName)
    {
        int index = -1;
        int RowCount = 1;

        CharCount = DisplayName.ToCharArray();

        for (int i = 0; i < CharCount.Length; i++)
        {
            index++;
            if(index == ColumnMaxCount)
            {
                RowCount++;
                index = 0;
            }
        }
        RowMaxCount = RowCount;
    }


    void MarkRowsForDeletion()
    {
        DeletionRows.Clear();
        for (int i = 1; i < transform.childCount; i++)
            DeletionRows.Add(transform.GetChild(i));
    }
    void DeleteRows()
    {
        for (int i = 0; i < DeletionRows.Count; i++)
            Destroy(DeletionRows[i].gameObject);
    }
    void GenerateRows()
    {
        Row = new LetterRow[RowMaxCount];
        int ChildCount = transform.childCount;
        for (int i = 0; i < RowMaxCount; i++)
        {
            Transform TransformRef;
            TransformRef = GameObject.Instantiate(transform.GetChild(0));
            TransformRef.SetParent(transform);
            TransformRef.gameObject.SetActive(true);

            Row[i] = new LetterRow();
            Row[i].RowRoot = TransformRef;
        }
    }
    void GenerateColumns()
    {
        int Index = 0;
        int MaxCount = int.MaxValue;
        if (AdaptiveToString && RemoveExcessLetterSpots)
            MaxCount = CharCount.Length + 1;


        for (int i = 0; i < Row.Length; i++)
        {
            LetterRow LR = Row[i];

            LR.Column = new DG_TextLetterDisplay[ColumnMaxCount];
            int ChildCount = LR.RowRoot.childCount;
            for (int iN = 0; iN < ColumnMaxCount; iN++)
            {
                Index++;
                if (Index >= MaxCount)
                    return;

                Transform TransformRef;
                if (iN != 0)
                {
                    TransformRef = GameObject.Instantiate(LR.RowRoot.GetChild(0));
                    TransformRef.SetParent(LR.RowRoot);
                }
                else
                    TransformRef = LR.RowRoot.GetChild(0);

                LR.Column[iN] = TransformRef.GetComponent<DG_TextLetterDisplay>();

                int indexVal = Index - 1;


                QuickFind.LanguageFonts.SetTMProLanguage(LR.Column[iN].TextLetter);


                if (AdaptiveToString && indexVal < CharCount.Length)
                {
                    LR.Column[iN].TextLetter.text = CharCount[indexVal].ToString();
                    LR.Column[iN].HasLetter = true;
                }
                else
                {
                    LR.Column[iN].TextLetter.text = string.Empty;
                    LR.Column[iN].HasLetter = false;
                }

                if (ShowUnderlines)
                    LR.Column[iN].Underline.enabled = true;
                else
                    LR.Column[iN].Underline.enabled = false;

            }
        }
    }
}
