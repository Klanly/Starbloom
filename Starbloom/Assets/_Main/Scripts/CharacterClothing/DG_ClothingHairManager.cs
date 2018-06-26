using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ClothingHairManager : MonoBehaviour {

    [System.Serializable]
    public class AttachedClothing
    {
        public ClothHairType Type;
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
        Boots
    }



    public DefaultClothing[] MaleDefault;
    public DefaultClothing[] FemaleDefault;




    private void Awake() { QuickFind.ClothingHairManager = this; }





    public void AddClothingItem(DG_CharacterLink CharacterRef, int ID)
    {
        if (!Application.isPlaying) return;
        Transform Char = CharacterRef.transform.GetChild(0);

        DG_ClothingObject ClothingObject = QuickFind.ClothingDatabase.GetItemFromID(ID);
        RemoveClothingPiece(CharacterRef, ClothingObject.Type);
        AttachNewClothingPiece(ClothingObject, CharacterRef, ClothingObject.Type);
    }


    public void RemoveClothingPiece(DG_CharacterLink CharacterRef, ClothHairType Type)
    {
        for(int i = 0; i < CharacterRef.AttachedClothes.Count; i++)
        {
            AttachedClothing C = CharacterRef.AttachedClothes[i];
            if(C.Type == Type)
            {
                for (int iN = 0; iN < C.ClothingPieces.Count; iN++)
                    Destroy(C.ClothingPieces[iN]);
                C.ClothingPieces.Clear();
                break;
            }
        }
    }


    public void AttachNewClothingPiece(DG_ClothingObject ClothingObject, DG_CharacterLink CharacterRef, ClothHairType Type)
    {
        AttachedClothing AC = GetAttachedClothingReference(CharacterRef, Type);
        Transform Char = CharacterRef.transform.GetChild(0);

        for (int i = 0; i < ClothingObject.Prefabs.Length; i++)
        {
            GameObject PrefabRef = ClothingObject.Prefabs[i];
            GameObject Clone = Instantiate(PrefabRef);

            if (Type != ClothHairType.Hair)
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
            else
            {
                Clone.transform.SetParent(CharacterRef.HairAttachpoint);
                DG_HairModule HO = Clone.GetComponent<DG_HairModule>();
                HO.LoadHairColliders(CharacterRef);
                Vector9.Vector9ToTransform(Clone.transform, CharacterRef.TransformData, true);

                AC.ClothingPieces.Add(Clone);
            }
        }
    }


    AttachedClothing GetAttachedClothingReference(DG_CharacterLink CharacterRef, ClothHairType Type)
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
        if (PC.Equipment.EquippedClothing.Count == 0) SetGameStartDefaultValues(PC, PC.CharacterGender);

        for(int i = 0; i < PC.Equipment.EquippedClothing.Count; i++)
            AddClothingItem(U.CharacterLink, PC.Equipment.EquippedClothing[i]);
    }

    public void SetGameStartDefaultValues(DG_PlayerCharacters.PlayerCharacter PC, DG_PlayerCharacters.GenderValue Gender)
    {
        if (Gender == DG_PlayerCharacters.GenderValue.Male)
        {
            for (int i = 0; i < MaleDefault.Length; i++)
                PC.Equipment.EquippedClothing.Add(MaleDefault[i].ID);
        }
        else
        {
            for (int i = 0; i < FemaleDefault.Length; i++)
                PC.Equipment.EquippedClothing.Add(FemaleDefault[i].ID);
        }
    }

}
