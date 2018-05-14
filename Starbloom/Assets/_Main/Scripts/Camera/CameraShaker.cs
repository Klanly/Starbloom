using UnityEngine;
using System.Collections.Generic;



#if UNITY_EDITOR
namespace EZCameraShake
{
    using UnityEditor;
    /////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
    [CustomEditor(typeof(CameraShaker))]
    class CameraShakerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            //Buttons

            CameraShaker myScript = (CameraShaker)target;
            if (GUILayout.Button("TestShake"))
            {
                myScript.ShakeOnce(myScript.TestShakeModule.magnitude, myScript.TestShakeModule.roughness, myScript.TestShakeModule.fadeInTime, myScript.TestShakeModule.fadeOutTime, myScript.TestShakeModule.posInfluence, myScript.TestShakeModule.rotInfluence, myScript.transform.position, myScript.TestShakeModule.DistanceNearValue, myScript.TestShakeModule.DistanceMaxValue);
            }
        }
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif




namespace EZCameraShake
{
    public class CameraShaker : MonoBehaviour
    {
        [System.Serializable]
        public class CameraShakeModule
        {
            public float magnitude;
            public float roughness;
            public float fadeInTime;
            public float fadeOutTime;
            public Vector3 posInfluence;
            public Vector3 rotInfluence;
            [Header("Distance Modifier")]
            public float DistanceNearValue;
            public float DistanceMaxValue;
        }


        [Header("Debug")]
        public CameraShakeModule TestShakeModule = null;
        public bool PrintDistanceValues = false;


        Transform SelfPointReference;
        Vector3 posAddShake, rotAddShake;
        List<CameraShakeInstance> cameraShakeInstances = new List<CameraShakeInstance>();




        void Awake()
        {
            QuickFind.CameraShake = this;
            this.enabled = false;
            if (SelfPointReference == null)
                SelfPointReference = transform;
        }

        void Update()
        {
            posAddShake = Vector3.zero;
            rotAddShake = Vector3.zero;

            for (int i = 0; i < cameraShakeInstances.Count; i++)
            {
                if (i >= cameraShakeInstances.Count)
                    break;

                CameraShakeInstance c = cameraShakeInstances[i];

                if (c.CurrentState == CameraShakeState.Inactive && c.DeleteOnInactive)
                {
                    cameraShakeInstances.RemoveAt(i);
                    i--;
                }
                else if (c.CurrentState != CameraShakeState.Inactive)
                {
                    posAddShake += CameraUtilities.MultiplyVectors(c.UpdateShake(), c.PositionInfluence);
                    rotAddShake += CameraUtilities.MultiplyVectors(c.UpdateShake(), c.RotationInfluence);
                }
            }

            transform.localPosition = posAddShake;
            transform.localEulerAngles = rotAddShake;
        }

        public CameraShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime, Vector3 posInfluence, Vector3 rotInfluence, Vector3 EffectPosition, float NearDistance, float FarDistance)
        {
            float ShakeMod = GetShakeMultiplier(EffectPosition, NearDistance, FarDistance);
            if (ShakeMod == 0)
                return null;

            float Mag = magnitude * ShakeMod;
            float Rough = roughness * ShakeMod;
            Vector3 PosInf = posInfluence * ShakeMod;
            Vector3 RotInf = rotInfluence * ShakeMod;

            CameraShakeInstance shake = new CameraShakeInstance(Mag, Rough, fadeInTime, fadeOutTime);
            shake.PositionInfluence = PosInf;
            shake.RotationInfluence = RotInf;
            cameraShakeInstances.Add(shake);
            this.enabled = true;

            return shake;
        }

        public List<CameraShakeInstance> ShakeInstances
        { get { return new List<CameraShakeInstance>(cameraShakeInstances); } }



        float GetShakeMultiplier(Vector3 EffectPosition, float NearDistance, float FarDistance)
        {
            float Distance = Vector3.Distance(SelfPointReference.position, EffectPosition);
            if (PrintDistanceValues)
                Debug.Log("Distance To Camera Shake " + Distance.ToString());


            if (Distance < NearDistance)
                return 1;
            else if (Distance > FarDistance)
                return 0;
            else
            {
                float Total = FarDistance - NearDistance;
                float Mid = FarDistance - Distance;
                float Percentage = (Mid / Total);

                if (PrintDistanceValues)
                    Debug.Log("Percentage " + Percentage.ToString());

                return Percentage;
            }
        }
    }
}