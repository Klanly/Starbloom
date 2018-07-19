using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeRainDropCollision : MonoBehaviour
{
    enum Type
    {
        Splash,
        Ripple
    }

    public bool LoadPoolsOnStart = false;
    public bool IsRaining = false;
    [Header("Ref Transforms")]
    public Transform ParticleContainer = null;
    public GameObject SplashRef = null;
    public GameObject RippleRef = null;
    [Header("Raycast Data")]
    public LayerMask RaycastMask;
    public LayerMask RippleMask;
    [Header("Pooling")]
    public int SplashPoolSize = 300;
    public int RipplePoolSize = 100;
    List<ParticleSystem> SplashPool;
    int SplashpoolIndex = 0;
    List<ParticleSystem> RipplePool;
    int RipplePoolIndex = 0;

    [Header("Dynamic Rain Data")]
    public float UpdateRate = .1f;
    [Header("TurnOffRipples")]
    public int AmountPerRippleOffUpdate = 10;


    [Header("Rain")]
    public float DropletRadius = 50f;
    public float RippleRadius = 150f;
    public int AmountPerUpdate = 10;
    public int RipplePerUpdate = 10;
    [Header("ThunderStorm")]
    public float StormDropletRadius = 50f;
    public float StormRippleRadius = 150f;
    public int StormAmountPerUpdate = 10;
    public int StormRipplePerUpdate = 10;







    bool PoolsLoaded = false;
    float Timer;

    bool StartedRaining = false;
    int TurnOffSplashCount = 0;
    int TurnOffRipplesCount = 0;



    // Use this for initialization
    void Start()
    {
        QuickFind.RainDropHandler = this;

        if (LoadPoolsOnStart)
            LoadPools();
    }

    void LoadPools()
    {
        if (PoolsLoaded)
            return;

        PoolsLoaded = true;
        SplashPool = new List<ParticleSystem>();
        RipplePool = new List<ParticleSystem>();
        for (int i = 0; i < SplashPoolSize; i++)
        {
            ParticleSystem PS = Instantiate(SplashRef.GetComponent<ParticleSystem>(), ParticleContainer);
            SplashPool.Add(PS);
        }
        for (int i = 0; i < RipplePoolSize; i++)
        {
            ParticleSystem PS = Instantiate(RippleRef.GetComponent<ParticleSystem>(), ParticleContainer);
            RipplePool.Add(PS);
        }
    }

    bool CheckSplashOff()
    {
        bool splashoff = false;
        bool rippleoff = false;

        if (TurnOffSplashCount > SplashPoolSize)
            splashoff = true;
        if (TurnOffRipplesCount > RipplePoolSize)
            rippleoff = true;

        if (splashoff && rippleoff)
        {
            StartedRaining = false;
            Timer = 0;
            return true;
        }

        return false;
    }

    void TurnOffRipples()
    {
        Timer = Timer - Time.deltaTime;
        if (Timer < 0)
        {
            Timer = UpdateRate;

            if (CheckSplashOff())
                return;

            for (int i = 0; i < AmountPerRippleOffUpdate; i++)
            {
                ParticleSystem PS = SplashPool[getIndex(Type.Splash)];
                PS.Stop();
                PS = RipplePool[getIndex(Type.Ripple)];
                PS.Stop();

                TurnOffSplashCount++;
                TurnOffRipplesCount++;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];
            if (P.CharLink == null) continue;


            if (QuickFind.PlayerTrans == null) return;

            if (!IsRaining)
            {
                if (StartedRaining)
                    TurnOffRipples();
                return;
            }
            if (!PoolsLoaded)
                LoadPools();


            StartedRaining = true;
            TurnOffSplashCount = 0;
            TurnOffRipplesCount = 0;

            Vector3 CamPos = QuickFind.PlayerTrans.position;
            Vector3 CamOffset = new Vector3(CamPos.x, 100, CamPos.z);

            Timer = Timer - Time.deltaTime;
            bool isThunderstorm = (QuickFind.WeatherHandler.CurrentWeather == WeatherHandler.WeatherTyps.Thunderstorm);

            if (Timer < 0)
            {
                Timer = UpdateRate;
                int Amount;
                float Radius;

                if (P.CharLink.PlayerCam.CamTrans.position.y > QuickFind.UnderwaterTrigger.WaterLevel)
                {
                    Amount = AmountPerUpdate; Radius = DropletRadius;
                    if (isThunderstorm) { Amount = StormAmountPerUpdate; Radius = StormDropletRadius; }
                    RaycastDetectionLoop(RaycastMask, Amount, CamOffset, Radius, Type.Splash, QuickFind.UnderwaterTrigger.WaterLevel);
                }

                Amount = RipplePerUpdate; Radius = RippleRadius;
                if (isThunderstorm) { Amount = StormRipplePerUpdate; Radius = StormRippleRadius; }
                RaycastDetectionLoop(RippleMask, Amount, CamOffset, Radius, Type.Ripple, -50);
            }
        }
    }
    void RaycastDetectionLoop(LayerMask Mask, int Count, Vector3 CamOffset, float Radius, Type EffectType, float MinimumY)
    {
        for (int i = 0; i < Count; i++)
        {

            Vector3 RandomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Vector3 RandomPoint = CamOffset + RandomDirection * Random.Range(1, Radius);

            Ray ray = new Ray(RandomPoint, -Vector3.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 150, Mask, QueryTriggerInteraction.Collide))
            {
                if (hit.point.y < MinimumY) continue;

                ParticleSystem PS = GetPS(EffectType);
                PS.transform.position = hit.point;
                PS.Play();
            }
        }
    }

    ParticleSystem GetPS(Type EffectType)
    {
        if (EffectType == Type.Splash)
            return SplashPool[getIndex(EffectType)];
        if (EffectType == Type.Ripple)
            return RipplePool[getIndex(EffectType)];

        return null;
    }

    int getIndex(Type EffectType)
    {
        if (EffectType == Type.Splash)
        {
            SplashpoolIndex++;
            if (SplashpoolIndex == SplashPoolSize)
                SplashpoolIndex = 0;
            return SplashpoolIndex;
        }
        else
        {
            RipplePoolIndex++;
            if (RipplePoolIndex == RipplePoolSize)
                RipplePoolIndex = 0;
            return RipplePoolIndex;
        }
    }
}
