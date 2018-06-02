using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeRainDropCollision : MonoBehaviour
{

    public bool LoadPoolsOnStart = false;
    public bool IsRaining = false;
    [Header("Ref Transforms")]
    public Transform ParticleContainer = null;
    public GameObject SplashRef = null;
    public GameObject RippleRef = null;

    [Header("Pooling")]
    public int SplashPoolSize = 300;
    public int RipplePoolSize = 100;
    List<ParticleSystem> SplashPool;
    int SplashpoolIndex = 0;
    List<ParticleSystem> RipplePool;
    int RipplePoolIndex = 0;

    [Header("Generic Data")]
    public float DropletRadius = 50f;

    [Header("Dynamic Rain Data")]
    public float UpdateRate = .1f;
    public int AmountPerUpdate = 10;
    public int AmountPerRippleOffUpdate = 10;

    [Header("Raycast Data")]
    public float RaycastMaxDistance;
    public LayerMask RaycastMask;
    public int WaterLayer;


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
    int getIndex(bool isSplash)
    {
        if (isSplash)
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
                ParticleSystem PS = SplashPool[getIndex(true)];
                PS.Stop();
                PS = RipplePool[getIndex(false)];
                PS.Stop();

                TurnOffSplashCount++;
                TurnOffRipplesCount++;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
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

        Timer = Timer - Time.deltaTime;
        bool AllowSuimonoCheck = false;


        Vector3 CamPos = QuickFind.PlayerTrans.position;
        Vector3 CamOffset = new Vector3(CamPos.x, 100, CamPos.z);

        if (Timer < 0)
        {
            Timer = UpdateRate;
            AllowSuimonoCheck = true;
        }
        for (int i = 0; i < AmountPerUpdate; i++)
        {

            Vector3 RandomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Vector3 RandomPoint = CamOffset + RandomDirection * Random.Range(1, DropletRadius);

            Ray ray = new Ray(RandomPoint, -Vector3.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, RaycastMaxDistance, RaycastMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.layer != WaterLayer)
                {
                    Vector3 hp = hit.point;
                    ParticleSystem PS = SplashPool[getIndex(true)];
                    PS.transform.position = hp;
                    PS.Play();
                }
                else if (AllowSuimonoCheck) //Check if Hitting Water.
                {
                    Vector3 hp = hit.point;
                    float SuimonoHeight = QuickFind.WaterModule.SuimonoGetHeight(hp, "height");
                    ParticleSystem PS = RipplePool[getIndex(false)];
                    PS.transform.position = new Vector3(hp.x, SuimonoHeight, hp.z);
                    PS.Play();
                }
            }
        }
    }
}
