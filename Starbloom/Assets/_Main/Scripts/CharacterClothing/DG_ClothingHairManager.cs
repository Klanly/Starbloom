using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ClothingHairManager : MonoBehaviour {

    [System.Serializable]
    public class AttachedClothing
    {
        public ClothHairType Type;
        public DG_ClothingObject ClothingRef;
        public List<GameObject> ClothingPieces;
    }

    [System.Serializable]
    public class DefaultClothing
    {
        public DG_ClothingHairManager.ClothHairType ClothType;
        public int ID;
    }

    public enum ClothHairType
    {
        Hair,
        Shirt,
        Pants,
        Boots,

        RightHand
    }



    public DefaultClothing[] MaleDefault;
    public DefaultClothing[] FemaleDefault;




    private void Awake() { QuickFind.ClothingHairManager = this; }









    public void AddClothingItem(DG_CharacterLink CharacterRef, int ID)
    {
        int UserID = QuickFind.NetworkSync.GetUserByCharacterLink(CharacterRef).ID;
        ClothingAdd(UserID, ID);

        int[] OutData = new int[2];
        OutData[0] = UserID;
        OutData[1] = ID;

        QuickFind.NetworkSync.SetUserEquipment(OutData);
    }

    public void NetReceivedClothingAdd(int[] InData)
    {   
        ClothingAdd(InData[0], InData[1]);
    }



    public void ClothingAdd(int UserID, int ID)
    {
        DG_CharacterLink CharacterRef = QuickFind.NetworkSync.GetCharacterLinkByUserID(UserID);
        if (!Application.isPlaying) return;
        Transform Char = CharacterRef.transform.GetChild(0);
        
        DG_ClothingObject ClothingObject = QuickFind.ClothingDatabase.GetItemFromID(ID);
        DG_PlayerCharacters.PlayerCharacter PC = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.GetPlayerIDByUserID(UserID)];
        RemoveClothingPiece(CharacterRef, ClothingObject.Type, PC);
        AttachNewClothingPiece(ClothingObject, CharacterRef, ClothingObject.Type, PC);
    }



    public void RemoveClothingPiece(DG_CharacterLink CharacterRef, ClothHairType Type, DG_PlayerCharacters.PlayerCharacter PC)
    {
        for(int i = 0; i < CharacterRef.AttachedClothes.Count; i++)
        {
            AttachedClothing C = CharacterRef.AttachedClothes[i];
            if(C.Type == Type)
            {
                for (int iN = 0; iN < C.ClothingPieces.Count; iN++)
                    Destroy(C.ClothingPieces[iN]);
                PC.Equipment.EquippedClothing.RemoveAt(PC.Equipment.FindClothingIndexByID(C.ClothingRef.DatabaseID));
                C.ClothingPieces.Clear();
                break;
            }
        }
    }










    public void AttachNewClothingPiece(DG_ClothingObject ClothingObject, DG_CharacterLink CharacterRef, ClothHairType Type, DG_PlayerCharacters.PlayerCharacter PC)
    {
        AttachedClothing AC = GetAttachedClothingReference(CharacterRef, Type);
        AC.ClothingRef = ClothingObject;
        PC.Equipment.EquippedClothing.Add(ClothingObject.DatabaseID);

        Transform Char = CharacterRef.transform.GetChild(0);

        for (int i = 0; i < ClothingObject.Prefabs.Length; i++)
        {
            GameObject PrefabRef = ClothingObject.Prefabs[i];
            GameObject Clone = Instantiate(PrefabRef);

            switch(Type)
            {
                case ClothHairType.Boots: Clothing(Char, Clone, CharacterRef, AC); break;
                case ClothHairType.Pants: Clothing(Char, Clone, CharacterRef, AC); break;
                case ClothHairType.Shirt: Clothing(Char, Clone, CharacterRef, AC); break;

                case ClothHairType.Hair:
                    {
                        DG_HairModule HO = Clone.GetComponent<DG_HairModule>();
                        HO.LoadHairColliders(CharacterRef);
                        NonSkinnedObject(Clone, CharacterRef, AC, ClothingObject);
                    }
                    break;

                case ClothHairType.RightHand:
                    {
                        NonSkinnedObject(Clone, CharacterRef, AC, ClothingObject);
                        if (!QuickFind.GameSettings.ShowToolOnEquip) Clone.SetActive(false);
                    }
                    break;
            }
        }
    }
    void Clothing(Transform Char, GameObject Clone, DG_CharacterLink CharacterRef, AttachedClothing AC)
    {
        Clone.transform.SetParent(Char);
        Clone.transform.localPosition = Char.localPosition;
        Clone.transform.localEulerAngles = Vector3.zero;
        Clone.transform.localScale = new Vector3(1, 1, 1);

        Transform ClothingItem = null;
        for (int iN = 0; iN < Clone.transform.childCount; iN++)
        {
            if (Clone.transform.GetChild(iN).name != "Body")
                ClothingItem = Clone.transform.GetChild(iN);
        }

        CloneDemBones(CharacterRef.MainBodyRef, ClothingItem);

        ClothingItem.SetParent(Char);
        Vector3 CurScale = ClothingItem.localScale;
        CurScale.z = 1;
        ClothingItem.localScale = CurScale;
        ClothingItem.GetComponent<SkinnedMeshRenderer>().rootBone = CharacterRef.SkeletonRoot;

        Destroy(Clone);

        AC.ClothingPieces.Add(ClothingItem.gameObject);
    }

    void NonSkinnedObject(GameObject Clone, DG_CharacterLink CharacterRef, AttachedClothing AC, DG_ClothingObject ClothingObject)
    {
        DG_ClothingObject.CharacterOffsetPoints COP = ClothingObject.CharOffsetPoint[0];
        DG_CharacterLink.AttachmentPoints AP = CharacterRef.GetAttachmentByType(ClothingObject.Type);
        Clone.transform.SetParent(AP.AttachmentPoint);
        Vector9.Vector9ToTransform(Clone.transform, ClothingObject.GetPositionByGender(CharacterRef.Gender), true);

        AC.ClothingPieces.Add(Clone);
    }












    public AttachedClothing GetAttachedClothingReference(DG_CharacterLink CharacterRef, ClothHairType Type)
    {
        AttachedClothing AC = null;
        for (int i = 0; i < CharacterRef.AttachedClothes.Count; i++)
        {
            AttachedClothing C = CharacterRef.AttachedClothes[i];
            if (C.Type == Type) { AC = C; break; }
        }
        if(AC == null)
        {
            AC = new AttachedClothing();
            AC.Type = Type;
            CharacterRef.AttachedClothes.Add(AC);
            AC.ClothingPieces = new List<GameObject>();
        }
        return AC;
    }






    void CloneDemBones(Transform Body, Transform ClothingItem)
    {
        SkinnedMeshRenderer targetRenderer = Body.GetComponent<SkinnedMeshRenderer>();
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in targetRenderer.bones)
        {
            boneMap[bone.name] = bone;
        }

        SkinnedMeshRenderer thisRenderer = ClothingItem.GetComponent<SkinnedMeshRenderer>();
        Transform[] boneArray = thisRenderer.bones;
        for (int idx = 0; idx < boneArray.Length; ++idx)
        {
            string boneName = boneArray[idx].name;
            if (false == boneMap.TryGetValue(boneName, out boneArray[idx]))
            {
                Debug.LogError("failed to get bone: " + boneName);
                Debug.Break();
            }
        }
        thisRenderer.bones = boneArray; //take effect
    }


    

    public void PlayerJoined(DG_NetworkSync.Users U)
    {
        DG_PlayerCharacters.PlayerCharacter PC = QuickFind.Farm.PlayerCharacters[U.PlayerCharacterID];
        if (PC.Equipment.EquippedClothing.Count < 2) SetGameStartDefaultValues(U, PC.CharacterGender);
        else
        {
            for (int i = 0; i < PC.Equipment.EquippedClothing.Count; i++)
                ClothingAdd(U.ID, PC.Equipment.EquippedClothing[i]);
        }
    }

    public void SetGameStartDefaultValues(DG_NetworkSync.Users U, DG_PlayerCharacters.GenderValue Gender)
    {
       if (Gender == DG_PlayerCharacters.GenderValue.Male)
       {
           for (int i = 0; i < MaleDefault.Length; i++)
                ClothingAdd(U.ID, MaleDefault[i].ID);
       }
       else
       {
           for (int i = 0; i < FemaleDefault.Length; i++)
                ClothingAdd(U.ID, FemaleDefault[i].ID);
       }
    }
}
