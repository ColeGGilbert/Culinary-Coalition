using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class BulletSpawning : MonoBehaviour
{
    #region Inspector
    [Header("Bullet Type Declaration")]
    [SerializeField] Bullet[] bulletTypes;

    [Header("Bullet Pattern Declarations")]
    [Space(15)]
    [SerializeField] Phase phase1;
    [SerializeField] Phase phase2;
    [SerializeField] Phase phase3;
    [SerializeField] Phase phase4;
    [SerializeField] Phase phase5;

    [Header("Prefab Declarations")]
    [Space(15)]
    [SerializeField] GameObject boomerangBanana;
    [SerializeField] GameObject boomerangSpawnContainer;

    [SerializeField] TornadoData tornadoData;

    [SerializeField] float patternDelay;
    [SerializeField] GameObject treeGameObject;
    [SerializeField] ParticleSystem treeCutPS = null;

    [SerializeField] BoolScriptableObj debugState = null;
    #endregion

    [SerializeField] GameObject[] sideWall;

    [SerializeField] GameObject[] topWall;

    [SerializeField] LayerMask sniperMask;

    [SerializeField] Material[] defaultMats;

    [SerializeField] float badIngSize;

	[SerializeField] BadIngredient[] badIngs;

	
    [Space(10)]
    [SerializeField] Bullet magnetType;

    #region ScriptOnly
    // TUTORIAL ONLY
    public static bool tutorialLevel { get; set; }

    public static bool isCooking { get; set; }
    public static bool endPhase { get; set; }

    public delegate void EndingText();
    public static event EndingText OnEnding;

    public delegate void PhaseUpdate(int phase);
    public static event PhaseUpdate OnChange;

    IngredientType[] ingsToSpawn;

    [System.Serializable]
    struct TornadoData
    {
        public string[] targetTags;
        public float spiralPower;
        public float attractionPower;
        public float maxPullDistance;
        public float lifeTime;
        public float timeBeforeSuck;
    }

    int currWave;
    bool canSpawnWave = true;
    bool canSpawnBadIngredients;
    bool boomerangNextBullet;
    int bananaBulletIndex;
    int bananaBulletAmount;
    bool fredericoJustPlayed;
    bool tornadoJustPlayed;
    Phase currentPhaseData;

    float bulletCountModifier; // Affects how many bullets are spawned in a pattern
    int badIngredientSpawnChance; // The chance that a bullet spawns as a bad ingredient as %
    int goodIngredientWaveGap; // How many waves should we wait before a good ingredient can spawn
    float minigunFiringDuration;
    float normalShotFiringDuration;
    float sniperFiringDuration;
    float snowFallPowerMod;
    int numberOfGoodIngredientsPerSpawn;

    int fredericoWaveGap;
    bool doSpawnFrederico;

    int tornadoWaveGap;
    bool doSpawnTornado;

    bool doRandomShotgunSpread;
    bool skippedIngSpawn;
    float shotgunSpreadAmount;

    float debugDifficultyMod = 1;

    [HideInInspector] public bool bananaInFlight;
    [HideInInspector] public bool bananaFlightEnded;

    int[] satisfactionValues; // current, then max

    ObjectPooler objectPooler;
    GameObject player;
    int spawnGoodIngredients;

    LineRenderer sniperLine;
    bool sniperFollowPlayer;
    Vector3 sniperLastKnownPos;

    int currentPhase = 1;
    #endregion

    #region Audio
    [Header("FMOD")]
    // Bullet sounds
    private string eMinigun = "{a70d53a8-a743-45ff-8772-6ef8d584d3c2}";
    private string eShotgun = "{fd15c6d4-bb8c-48ea-8072-10c2ed4d4446}";
    private string eOmegaFire = "{b1e50523-3b73-4fc0-9160-cb975db09230}";

    // Frederico sounds
    private string eBananaThrow = "{7bbf507e-50ea-4df0-8e14-87ffabb354e0}";
    private EventInstance iBananaThrow;

    // Game events
    private string eRemoveTree = "{3ad62f20-dd31-4f3a-acb8-a052bc13605e}";

    // Debug
    private string eSkipPhase = "{9d79b881-2c23-46c5-8db1-56705db8e7dc}";

    // Music info
    private float pReady = 0;   // Prevents bullets from spawning until the music is ready
    private bool isMusicValid = false;  // pReady will not affect bullet spawning if music is invalid

    #endregion

    #region Events
    [Header("Events")]
    [SerializeField]
    private GameEventInt onNewPhase = null;
    [SerializeField]
    private GameEvent onLevelCompletion = null; // Invoked after serving final meal
    #endregion

    ///<summary>
    /// Different types of patterns that are currently implemented in the game that can be used in the "Phase" scriptable object
    ///</summary>
    [HideInInspector] public enum Patterns
    {
        shotgun,
        minigun,
        omega,
        normal,
        rest,
        banana,
        sline,
        sniper,
        magnet,
        snowfall,
        snowball,
        notomega,
        tornado
    }

    [System.Serializable]
    public struct BadIngredient 
    {
        public Mesh model;
        public Material mat;
    }

    void OnEnable()
    {
        IngGameController.SatisfactionUpdated += SatisfactionUpdated;
    }

    private void Start()
    {
        sniperLine = GetComponent<LineRenderer>();
        GameManager.instance.currentPhase = 1;
        OnChange(currentPhase);

        player = GameObject.FindGameObjectWithTag("Player");
        objectPooler = ObjectPooler.Instance;

        tutorialLevel = false;

        ingsToSpawn = IngGameController.instance.SpawnableIngredients;

        // Gets the phase values and data from the provided scriptable object
        currentPhaseData = phase1;
        GetNewPhaseVariables();

        SatisfactionUpdated();
    }

    ///<summary>
    /// SpawnBullet() communicates with the Object Pooler to release an object with the properties passed through the parameters
    ///</summary>
    public GameObject SpawnBullet(Vector3 dir, Bullet bul, string pool, Vector3 origin, Material mat, IngredientType ingType)
    {
        return objectPooler.SpawnFromPool(pool, origin, Quaternion.identity, dir, bul, mat, ingType);
    }

    ///<summary>
    /// All it does is get all of the new variables from the phase object when a new phase begins
    /// Big scary function, horrible to look at
    ///</summary>
    private void GetNewPhaseVariables()
    {
        bulletCountModifier = currentPhaseData.bulletCountModifier * debugDifficultyMod;
        badIngredientSpawnChance = currentPhaseData.badIngredientSpawnChance;
        goodIngredientWaveGap = currentPhaseData.goodIngredientWaveGap;
        minigunFiringDuration = currentPhaseData.minigunFiringDuration;
        normalShotFiringDuration = currentPhaseData.normalShotFiringDuration;
        sniperFiringDuration = currentPhaseData.sniperFiringDuration;
        fredericoWaveGap = currentPhaseData.fredericoWaveGap;
        doSpawnFrederico = currentPhaseData.canSpawnFrederico;
        tornadoWaveGap = currentPhaseData.tornadoWaveGap;
        doSpawnTornado = currentPhaseData.canSpawnTornado;
        doRandomShotgunSpread = currentPhaseData.randomShotgunSpread;
        shotgunSpreadAmount = currentPhaseData.maxSpreadAmount;
        snowFallPowerMod = currentPhaseData.snowFallPower;
    }

    private void Update()
    {
        // Updates pReady with corresponding param value phaseReady from FMOD
        RuntimeManager.StudioSystem.getParameterByName("phaseReady", out pReady, out pReady);

        #region Logic Control

        CheckPhaseEnd();

        if (canSpawnWave && (pReady == 1 || !isMusicValid))
        {
            canSpawnWave = false;
            Invoke("BulletPattern", patternDelay);
        }

        FredericoActiveCheck();

        // Controls the positioning of the red "tracer" used in the sniper shot effect
        if(sniperLine.enabled)
        {
            RaycastHit hit;
            if(sniperFollowPlayer)
            {
                if(Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, Mathf.Infinity, sniperMask))
                {
                    sniperLine.SetPositions(new Vector3[2]{transform.position, hit.point});
                }
            }
            else
            {
                if(Physics.Raycast(transform.position, sniperLastKnownPos - transform.position, out hit, Mathf.Infinity, sniperMask))
                {
                    sniperLine.SetPositions(new Vector3[2]{transform.position, hit.point});
                }
            }
        }

        //Debug
        if (Input.GetKeyDown(KeyCode.L) && debugState.GetValue())
        {
            RuntimeManager.PlayOneShot(eSkipPhase);
            IncreasePhase();
        }

        if(Input.GetKeyDown(KeyCode.X) && debugState.GetValue())
        {
            if(debugDifficultyMod < 2.5f)
            {
                debugDifficultyMod *= 1.5f;
                bulletCountModifier = currentPhaseData.bulletCountModifier * debugDifficultyMod;
            }
        }
        else if (Input.GetKeyDown(KeyCode.C) && debugState.GetValue())
        {
            debugDifficultyMod = 1;
            bulletCountModifier = currentPhaseData.bulletCountModifier * debugDifficultyMod;
        }

        #endregion
    }

    public void SetMusicValid(bool value) => isMusicValid = value;

    ///<summary>
    /// This check controls the logic surrounding frederico (the banana attack) in which it checks whether
    /// to fire another bullet if he is still in flight OR to end the flight if the animation has ended
    ///</summary>
    void FredericoActiveCheck()
    {
        if (bananaInFlight && boomerangNextBullet)
        {
            boomerangNextBullet = false;
            Invoke("BananaBullet", 3 / (bananaBulletAmount * bulletCountModifier));
        }
                
        if (bananaFlightEnded)
        {
            bananaBulletIndex = 0;
            canSpawnWave = true;
            boomerangNextBullet = true;
            bananaFlightEnded = false;
        }
    }

    void CheckPhaseEnd()
    {
        if (IngGameController.instance.AmISatisfied())
        {
            IngGameController.instance.ResetSatisfaction();
            IncreasePhase();
        }
    }

    void SatisfactionUpdated()
    {
        satisfactionValues = IngGameController.instance.GetSatisfaction(); 
    }

    void BulletPattern()
    {
        spawnGoodIngredients = 0;

        if(pReady == 0 && isMusicValid)
        {
            canSpawnWave = true;
            return;
        }

        Patterns pattern = phase1.Served0[0].pattern.patternType;
        Phase.PhasePattern data = phase1.Served0[0];


        // This region of code handles decision making for which pattern is called next
        #region Phase Data Check and Pattern Decision Making
        if (doSpawnFrederico)
        {
            if(fredericoWaveGap == 0)
            {
                Debug.LogError("Frederico can't have a wave gap of 0, please change this value to at least 1");
            }
            else if (currWave % (fredericoWaveGap+1) == fredericoWaveGap && !fredericoJustPlayed)
            {
                data = currentPhaseData.Frederico;
                fredericoJustPlayed = true;
                currWave--;
            }
            else if (doSpawnTornado)
            {
                if (tornadoWaveGap == 0)
                {
                    Debug.LogError("Tornado can't have a wave gap of 0, please change this value to at least 1");
                }
                else if (currWave % (tornadoWaveGap + 1) == tornadoWaveGap && !tornadoJustPlayed)
                {
                    data = currentPhaseData.Tornado;
                    tornadoJustPlayed = true;
                    currWave--;
                }
                else
                {
                    data = PickPatternData();
                }
            }
            else 
            {
                data = PickPatternData();
            }
        }
        else if (doSpawnTornado)
        {
            if(tornadoWaveGap == 0)
            {
                Debug.LogError("Tornado can't have a wave gap of 0, please change this value to at least 1");
            }
            else if (currWave % (tornadoWaveGap + 1) == tornadoWaveGap && !tornadoJustPlayed)
            {
                data = currentPhaseData.Tornado;
                tornadoJustPlayed = true;
                currWave--;
            }
            else
            {
                data = PickPatternData();
            }
        }
        else
        {
            data = PickPatternData();
        }

        if(data.pattern != null)
        {
            pattern = data.pattern.patternType;
            canSpawnBadIngredients = data.canSpawnBadIngredients;
        }
        #endregion

        #region PatternLogic and Spawning
        currWave++;

        if (currWave > 4)
        {
            if (goodIngredientWaveGap == 0)
            {
                Debug.LogError("Good ingredients can't have a wave gap of 0, please change this value to at least 1");
            }
            else if (skippedIngSpawn)
            {
                spawnGoodIngredients = data.goodIngredientsToSpawn;
                skippedIngSpawn = false;
            }
            else if((currWave % goodIngredientWaveGap == 0) && data.goodIngredientsToSpawn <= 0)
            {
                skippedIngSpawn = true;
            }
            else if (currWave % goodIngredientWaveGap == 0) // Is it a wave that needs to spawn an ingredient
            {
                spawnGoodIngredients = data.goodIngredientsToSpawn;
            }
        }

        switch (pattern)
        {
            case (Patterns.shotgun): //Shotgun pattern
                StartCoroutine(Shotgun(data.pattern.amountOfBullets, data.pattern.bulletType));
                break;

            case (Patterns.minigun): //Minigun pattern
                StartCoroutine(Minigun(data.pattern.amountOfBullets, data.pattern.bulletType));
                break;

            case (Patterns.omega): //Omega pattern
                int newX;
                int newZ;
                if (Random.Range(0, 2) == 0)
                {
                    newX = -25;
                    newZ = 155;
                }
                else
                {
                    newX = 25;
                    newZ = 205;
                }
                RuntimeManager.PlayOneShot(eOmegaFire, transform.position);
                Vector3 newDir = new Vector3(Mathf.Sin(newX * Mathf.Deg2Rad), 0, Mathf.Cos(newZ * Mathf.Deg2Rad));
                GameObject omega = SpawnBullet(newDir, bulletTypes[1], "Bullet", transform.position, defaultMats[0], null);
                ExplodeBullet vars = omega.AddComponent<ExplodeBullet>();
                vars.spawner = this;
                vars.bul = data.pattern.bulletType;
                vars.bulletCountModifier = bulletCountModifier;
                vars.bulletAmount = data.pattern.amountOfBullets;
                canSpawnWave = true;
                break;

            case (Patterns.normal): //Normal bullet pattern
                StartCoroutine(NormalShots(data.pattern.amountOfBullets, data.pattern.bulletType));
                break;

            case (Patterns.rest): //Rest = do nothing
                Invoke("BulletPattern", patternDelay * 2);
                break;

            case (Patterns.banana): //Banana bullet pattern
                bananaBulletAmount = data.pattern.amountOfBullets;
                boomerangNextBullet = true;
                boomerangBanana.GetComponent<Animator>().SetTrigger("Fire");
                bananaInFlight = true;
                break;
            
            case (Patterns.sline):
                SimpleLine(data.pattern.amountOfBullets, data.pattern.bulletType);
                break;

            case (Patterns.sniper):
                StartCoroutine(SniperShot(data.pattern.amountOfBullets, data.pattern.bulletType));
                break;

            case (Patterns.magnet): //Magnet pattern
                MagnetBullet(new Bullet[2] { magnetType, data.pattern.bulletType });
                break;

            case (Patterns.snowfall): //snowfall pattern
                SnowFall(data.pattern.amountOfBullets, data.pattern.bulletType);
                break;

            case (Patterns.snowball): //snowball pattern
                int newX3;
                int newZ3;
                if (Random.Range(0, 2) == 0)
                {
                    newX3 = -25;
                    newZ3 = 155;
                }
                else
                {
                    newX3 = 25;
                    newZ3 = 205;
                }
                RuntimeManager.PlayOneShot(eOmegaFire, transform.position);
                Vector3 newDir3 = new Vector3(Mathf.Sin(newX3 * Mathf.Deg2Rad), 0, Mathf.Cos(newZ3 * Mathf.Deg2Rad));
                GameObject snowball = SpawnBullet(newDir3, bulletTypes[0], "Bullet", transform.position, defaultMats[0], null);
                SnowballBullet vars3 = snowball.AddComponent<SnowballBullet>();
                vars3.spawner = this;
                vars3.bul = data.pattern.bulletType;
                vars3.bulletCountModifier = bulletCountModifier;
                vars3.bulletAmount = data.pattern.amountOfBullets;
                canSpawnWave = true;
                break;

            case (Patterns.notomega): //notomega pattern
                int newX2;
                int newZ2;
                if (Random.Range(0, 2) == 0)
                {
                    newX2 = -25;
                    newZ2 = 155;
                }
                else
                {
                    newX2 = 25;
                    newZ2 = 205;
                }
                RuntimeManager.PlayOneShot(eOmegaFire, transform.position);
                Vector3 newDir2 = new Vector3(Mathf.Sin(newX2 * Mathf.Deg2Rad), 0, Mathf.Cos(newZ2 * Mathf.Deg2Rad));
                GameObject omegan = SpawnBullet(newDir2, bulletTypes[1], "Bullet", transform.position, defaultMats[0], null);
                ReverseExplodeBullet vars2 = omegan.AddComponent<ReverseExplodeBullet>();
                vars2.spawner = this;
                vars2.bul = data.pattern.bulletType;
                vars2.bulletCountModifier = bulletCountModifier;
                vars2.bulletAmount = data.pattern.amountOfBullets;
                canSpawnWave = true;
                break;

            case (Patterns.tornado):
                int newX4;
                int newZ4;
                if (Random.Range(0, 2) == 0)
                {
                    newX4 = -25;
                    newZ4 = 155;
                }
                else
                {
                    newX4 = 25;
                    newZ4 = 205;
                }
                Vector3 newDir4 = new Vector3(Mathf.Sin(newX4 * Mathf.Deg2Rad), 0, Mathf.Cos(newZ4 * Mathf.Deg2Rad));
                GameObject tornado = SpawnBullet(newDir4, data.pattern.bulletType, "Tornado", transform.position, null, null);
                TornadoBullet tornadoScript = tornado.AddComponent<TornadoBullet>();
                tornadoScript.timeToWait = tornadoData.timeBeforeSuck;
                tornadoScript.lifetime = tornadoData.lifeTime;
                tornadoScript.spiralPower = tornadoData.spiralPower;
                tornadoScript.attractionPower = tornadoData.attractionPower;
                tornadoScript.Tags = tornadoData.targetTags;
                tornadoScript.maxDistance = tornadoData.maxPullDistance;
                tornadoScript.Initiate();
                canSpawnWave = true;
                break;
        }
        #endregion

    }

    Phase.PhasePattern PickPatternData() 
    {
        Phase.PhasePattern data;
        if (satisfactionValues[0] > (satisfactionValues[1] / 3) * 2 && currentPhaseData.Served2.Length > 0)
        {
            if (currentPhaseData.Random2)
            {
                data = currentPhaseData.Served2[Random.Range(0, currentPhaseData.Served2.Length)];
            }
            else
            {
                data = currentPhaseData.Served2[currWave % currentPhaseData.Served2.Length];
            }
        }
        else if (satisfactionValues[0] > (satisfactionValues[1] / 3) && currentPhaseData.Served1.Length > 0)
        {
            if (currentPhaseData.Random1)
            {
                data = currentPhaseData.Served1[Random.Range(0, currentPhaseData.Served1.Length)];
            }
            else
            {
                data = currentPhaseData.Served1[currWave % currentPhaseData.Served1.Length];
            }
        }
        else
        {
            if (currentPhaseData.Random0)
            {
                data = currentPhaseData.Served0[Random.Range(0, currentPhaseData.Served0.Length)];
            }
            else
            {
                data = currentPhaseData.Served0[currWave % currentPhaseData.Served0.Length];
            }
        }
        fredericoJustPlayed = false;
        tornadoJustPlayed = false;
        return data;
    }

    void IncreasePhase()
    {

        foreach (GameObject bul in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            bul.SetActive(false);
        }

        RuntimeManager.StudioSystem.setParameterByName("phaseReady", 0);
        pReady = 0;
        currWave = 0;
        spawnGoodIngredients = 0;
        skippedIngSpawn = false;
        currentPhase++;
        GameManager.instance.currentPhase = currentPhase;
        OnChange(currentPhase);
        onNewPhase?.Raise(currentPhase);

        switch (currentPhase)
        {
            case (2):
                OnEnding?.Invoke();
                currentPhaseData = phase2;
                break;

            case (3):
                OnEnding?.Invoke();
                currentPhaseData = phase3;
                break;

            case (4):
                currentPhaseData = phase4;
                break;

            case (5):
                currentPhaseData = phase5;
                break;
        }

        if(currentPhase < 4)
        {
            GetNewPhaseVariables();
        }

        if (currentPhase == 3)
        {
            StartCoroutine(ByeTree());
        }
        else if (currentPhase == 4)
        {
            OnEnding?.Invoke();
            onLevelCompletion?.Raise();
        }

        Debug.Log("New phase begins! " + currentPhase);
    }

    public void RemoveTree()
    {
        if (treeGameObject.activeSelf)
        {
            RuntimeManager.PlayOneShot(eRemoveTree, boomerangBanana.transform.position);
            treeCutPS.Play();
            treeGameObject.SetActive(false);
        }
    }

    public GameObject DecideBullet(Vector3 dir, int i, Bullet bul, int totalBullets, Vector3 origin)
    {
        bool canSpawnGoodIngredient = true;
        IngredientType randomIng = DecideIngredient();
        IngredientType badIng = new IngredientType(0, "badIng", badIngs[Random.Range(0, badIngs.Length)].model, IngGameController.IngTypes.Apple, badIngs[Random.Range(0, badIngs.Length)].mat);
        if(Holding.instance.heldObj != null)
        {
            if (isCooking || Holding.instance.heldObj.name == "Good Ingredient(Clone)")
            {
                canSpawnGoodIngredient = false;
            }
            else
            {
                canSpawnGoodIngredient = true;
            }
        }
        else if(isCooking)
        {
            canSpawnGoodIngredient = false;
        }
        else
        {
            canSpawnGoodIngredient = true;
        }

        if (canSpawnBadIngredients)
        {
            if (spawnGoodIngredients > 0 && Random.Range(0, totalBullets - i) <= (spawnGoodIngredients - 1) && canSpawnGoodIngredient)
            {
                spawnGoodIngredients--;
                return SpawnBullet(dir, bul, "Good_Ingredient", origin, randomIng.material, randomIng);
            }
            else if (Random.Range(1, 101) <= badIngredientSpawnChance)
            {
                Bullet badIngBullet = new Bullet(badIngSize, bul.speed, bul.damage, bul.lifespan);
                return SpawnBullet(dir, badIngBullet, "Bad_Ingredient", origin, badIng.material, badIng);
            }
            else
            {
                return SpawnBullet(dir, bul, "Bullet", origin, defaultMats[0], null);
            }
        }
        else
        {
            if (spawnGoodIngredients > 0 && Random.Range(0, totalBullets - i) <= (spawnGoodIngredients-1) && canSpawnGoodIngredient)
            {
                spawnGoodIngredients--;
                return SpawnBullet(dir, bul, "Good_Ingredient", origin, randomIng.material, randomIng);
            }
            else
            {
                return SpawnBullet(dir, bul, "Bullet", origin, defaultMats[0], null);
            }
        }
    }

    public IngredientType DecideIngredient()
    {
        return ingsToSpawn[Random.Range(0, ingsToSpawn.Length)];
    }

    void ShotgunPattern(float angX, float angZ, int amount, Bullet bulType)
    {
        int totalBullets = Mathf.RoundToInt(amount * bulletCountModifier);
        float angleDif = (Mathf.Abs(angX)+Mathf.Abs(angZ)) / totalBullets;
        float startAngleX = angX;
        float startAngleZ = angZ;
        for (int i = 0; i < totalBullets; i++)
        {
            float newAngX = startAngleX + (angleDif * i);
            float newAngZ = startAngleZ + (angleDif * i);
            Vector3 newDir = new Vector3(Mathf.Sin(newAngX * Mathf.Deg2Rad), 0, Mathf.Cos(newAngZ * Mathf.Deg2Rad));
            DecideBullet(newDir, i, bulType, totalBullets, transform.position);
        }
    }

    void MagnetBullet(Bullet[] type)
    {
        Transform currentWall = sideWall[0].transform;
        GameObject leftMagnet = SpawnBullet(Vector3.right, type[0], "Bullet", new Vector3(currentWall.position.x, currentWall.position.y, (currentWall.position.z + currentWall.localScale.z / 2) - 7), defaultMats[0], null);
        currentWall = sideWall[1].transform;
        GameObject rightMagnet = SpawnBullet(Vector3.left, type[0], "Bullet", new Vector3(currentWall.position.x, currentWall.position.y, (currentWall.position.z + currentWall.localScale.z / 2) - 7), defaultMats[0], null);
        leftMagnet.tag = "Magnet"; rightMagnet.tag = "Magnet";
        MagnetBullet vars = rightMagnet.AddComponent<MagnetBullet>();
        vars.magnetLifespan = type[0].lifespan;
        vars.sisterMagnet = leftMagnet;
        vars.spawner = this;
        vars.bul = type[1];
        vars.homingBullet = false;
        vars.isIng = false;
        canSpawnWave = true;
    }

    void BananaBullet()
    {
        if (bananaInFlight)
        {
            Vector3 origin;
            Vector3 dir;
            if (Random.Range(0, 2) == 0)
            {
                origin = boomerangSpawnContainer.transform.GetChild(0).position;
                dir = boomerangSpawnContainer.transform.GetChild(0).transform.TransformDirection(0, 0, 1);
            }
            else
            {
                origin = boomerangSpawnContainer.transform.GetChild(1).position;
                dir = boomerangSpawnContainer.transform.GetChild(1).transform.TransformDirection(0, 0, 1);
            }

            DecideBullet(dir, bananaBulletIndex, bulletTypes[0], Mathf.RoundToInt(bananaBulletAmount * bulletCountModifier), origin);

            boomerangNextBullet = true;
        }
    }

    void SimpleLine(int amount, Bullet type)
    {
        int totalBullets = Mathf.RoundToInt(amount * bulletCountModifier);
        //float startZ = sideWall[0].transform.position.z + sideWall[0].transform.localScale.z/2;
        float zInc = sideWall[0].transform.localScale.z / (totalBullets/2);
        Transform currentWall = sideWall[0].transform;
        for (int i = 0; i < totalBullets/2; i++)
        {
            DecideBullet(Vector3.right, i, type, totalBullets, new Vector3(currentWall.position.x, currentWall.position.y, (currentWall.position.z + currentWall.localScale.z/2) - (zInc * i)));
        }
        currentWall = sideWall[1].transform;
        for (int i = 0; i < totalBullets/2; i++)
        {
            DecideBullet(Vector3.left, i, type, totalBullets, new Vector3(currentWall.position.x, currentWall.position.y, (currentWall.position.z + currentWall.localScale.z/2) - (zInc * i)));
        }
        canSpawnWave = true;
    }

    void SnowFall(int amount, Bullet type)
    {
        int totalBullets = Mathf.RoundToInt(amount * bulletCountModifier);
        float zInc = topWall[0].transform.localScale.z / (totalBullets);
        Transform currentWall = topWall[0].transform;
        for (int i = 0; i < totalBullets; i++)
        {
            GameObject bullet = DecideBullet(Vector3.down, i, type, totalBullets, new Vector3(currentWall.position.x + ((currentWall.localScale.z / 2) - (zInc * i)), currentWall.position.y, currentWall.position.z));
            BulletMovement BM = bullet.GetComponent<BulletMovement>();
            BM.isSnowfall = true;
            BM.snowfallPower = -snowFallPowerMod;
        }
        canSpawnWave = true;
    }

    IEnumerator SniperShot(int amount, Bullet type)
    {
        canSpawnWave = true;
        int totalBullets = Mathf.RoundToInt(amount * bulletCountModifier);

        sniperLine.enabled = true;
        sniperFollowPlayer = true;

        yield return new WaitForSeconds(1);

        sniperLastKnownPos = sniperLine.GetPosition(1);
        sniperFollowPlayer = false;
        yield return new WaitForSeconds(.25f);

        sniperLine.enabled = false;
        for(int i=0; i<totalBullets; i++)
        {
            float newDirX = sniperLastKnownPos.x - transform.position.x;
            float newDirZ = sniperLastKnownPos.z - transform.position.z;
            Vector3 newDir = new Vector3(newDirX, 0, newDirZ).normalized * 1.4f;
            DecideBullet(newDir, 0, type, amount, transform.position);
            yield return new WaitForSeconds(sniperFiringDuration / totalBullets);
        }
    }

    IEnumerator ByeTree()
    {
        if (isMusicValid)
        {
            while (pReady == 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        iBananaThrow = RuntimeManager.CreateInstance(eBananaThrow);
        RuntimeManager.AttachInstanceToGameObject(iBananaThrow, boomerangBanana.transform, boomerangBanana.GetComponent<Rigidbody>());
        iBananaThrow.start();
        boomerangBanana.GetComponent<Animator>().SetTrigger("GetRidOfTree");
    }

    IEnumerator Minigun(int amount, Bullet type)
    {
        int totalBullets = Mathf.RoundToInt(amount * bulletCountModifier);
        for (int i = 0; i < totalBullets; i++)
        {
            RuntimeManager.PlayOneShot(eMinigun, transform.position);
            float newDirX = player.transform.position.x - transform.position.x;
            float newDirZ = player.transform.position.z - transform.position.z;
            Vector3 newDir = new Vector3(newDirX, 0, newDirZ).normalized * 1.4f;//new Vector3(Mathf.Sin(ang), 0, Mathf.Cos(ang));
            DecideBullet(newDir, i, type, totalBullets, transform.position);
            yield return new WaitForSeconds(minigunFiringDuration / totalBullets);
        }
        canSpawnWave = true;
    }

    IEnumerator NormalShots(int amount, Bullet type)
    {
        int totalBullets = Mathf.RoundToInt(amount * bulletCountModifier);
        for (int i = 0; i < totalBullets; i++)
        {
            RuntimeManager.PlayOneShot(eMinigun, transform.position);
            float newDirX = player.transform.position.x - transform.position.x;
            float newDirZ = player.transform.position.z - transform.position.z;
            Vector3 newDir = new Vector3(newDirX, 0, newDirZ).normalized * 1.4f;//new Vector3(Mathf.Sin(ang), 0, Mathf.Cos(ang));
            DecideBullet(newDir, i, type, totalBullets, transform.position);
            yield return new WaitForSeconds(normalShotFiringDuration / totalBullets);
        }
        canSpawnWave = true;
    }

    IEnumerator Shotgun(int amount, Bullet type)
    {
        RuntimeManager.PlayOneShot(eShotgun, transform.position);
        if(doRandomShotgunSpread)
        {
            float random = Random.Range(0, shotgunSpreadAmount);
            ShotgunPattern(-88 + random, 92 + random, amount, type);
            yield return new WaitForSeconds(0.35f);
            ShotgunPattern(-70 - random, 110 - random, amount, type);
        }
        else
        {
            ShotgunPattern(-88, 92, amount, type);
            yield return new WaitForSeconds(0.35f);
            ShotgunPattern(-70, 110, amount, type);
        }
        canSpawnWave = true;
    }

    void SnowBall(int amount, Bullet type)
    {
        canSpawnWave = true;
    }

    IEnumerator Omgenantnot(int amount, Bullet type)
    {
        yield return new WaitForSeconds(0.35f);
        canSpawnWave = true;
    }
}