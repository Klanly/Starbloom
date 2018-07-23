using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_SFXHandler : MonoBehaviour {



    public void TriggerRegularSoundEffect(int ID, Vector3 Position)
    {
        DG_SFXObject MO = QuickFind.SFXDatabase.GetItemFromID(ID);
        if (MO.SFXPieces.Length == 1)
            PlaySFX(MO, MO.SFXPieces[0], Position);
        else
        {
            DG_SFXObject.GreaterThan1SFXItemLogic ItemLogic = MO.ItemLogic;
            if(ItemLogic == DG_SFXObject.GreaterThan1SFXItemLogic.PlayAllAtOnce)
                { for(int i = 0; i < MO.SFXPieces.Length; i++) PlaySFX(MO, MO.SFXPieces[i], Position); }
            if (ItemLogic == DG_SFXObject.GreaterThan1SFXItemLogic.PlayRandom)
                PlaySFX(MO, MO.SFXPieces[Random.Range(0, MO.SFXPieces.Length)], Position);
            if (ItemLogic == DG_SFXObject.GreaterThan1SFXItemLogic.PlaySequential)
                { PlaySFX(MO, MO.SFXPieces[MO.Index], Position); MO.Index = QuickFind.GetValueInArrayLoop(MO.Index, MO.SFXPieces.Length, true, true); }
            if (ItemLogic == DG_SFXObject.GreaterThan1SFXItemLogic.PlayAllInOrder)
                Debug.Log("TODO Fishing Play All in ORder Logic");
        }
    }
    void PlaySFX(DG_SFXObject MO, DG_SFXObject.SFXItem SFX, Vector3 Position)
    {
        GameObject SFXPoolObject = QuickFind.PrefabPool.GetPoolItemByPrefabID(SFX.PrefabID);
        SFXPoolObject.transform.position = Position;

        AudioSource AS = SFXPoolObject.GetComponent<AudioSource>();
        AS.outputAudioMixerGroup = QuickFind.SFXDatabase.GetMixerGroupByZone(MO);
        AS.volume = SFX.Volume;
        float Pitch = SFX.PitchValue;
        if (SFX.RandomizePitchWithinRangeOfValue) Pitch += Random.Range(-SFX.RandomPitchRange, SFX.RandomPitchRange);
        AS.pitch = Pitch;
        AS.Play();
    }
}
