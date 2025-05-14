using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Well well well, if it isn't the consequences of our own actions
public class TutorialManager : MonoBehaviour
{
    ObjectPooler objectPooler;
    GameObject player;
    bool spawnGoodIngredient;

    [System.Serializable]
    public class Speaker
    {
        public string name;
        public Sprite portrait;
        public string[] text;
        [EventRef] public string eSound = "";
    }
    public List<Speaker> speakers;
    private int currentSpeaker = 0;

    [Header("Bullet Type Declaration")]
    [Space(10)]
    [SerializeField] Bullet[] bulletTypes;

    [Header("Wave Value Declarations")]
    [Space(30)]
    [SerializeField] float bulletCountModifier; // Affects how many bullets are spawned in a pattern
    [Range(1, 100)] [SerializeField] int badIngredientSpawnChance; // The chance that a bullet spawns as a bad ingredient as %
    [SerializeField] int goodIngredientWaveGap; // How many waves should we wait before a good ingredient can spawn
    int currWave;
    bool canSpawnWave = true;
    bool canSpawnBadIngredients;
    public static bool isCooking { get; set; }
    public bool doneCooking;
    int currentPhase = 0;
    Coroutine cookingCoroutine;
    Coroutine bulletCoroutine;
    public static int itemsServed = 0;
    [SerializeField] float patternDelay;
    IngredientType[] ingsToSpawn;

    [Header("FMOD")]
    [SerializeField] [EventRef] private string eMinigun = null;
    [SerializeField] [EventRef] private string eShotgun = null;
    //[SerializeField] [EventRef] private string eMusic;
    [SerializeField] [EventRef] private string eSkipPhase = null;
    //EventInstance musicInstance;
    private float pReady = 0;
    int currHealth;
    int pattern;
    Health pHealthScript;
    public static bool dialogueFinished { get; set; }
    private TutorialStage activeStage;
    private DialogueSystem dialogueSystem;
    private bool showText;

    private GameObject Table;
    private GameObject TableZone;
    private GameObject Oven;
    private GameObject OvenZone;
    private GameObject TheBin;
    private GameObject TheBinZone;

    [SerializeField] GameObject arrowObject;
    [SerializeField] Material[] defaultMats;

    private Player controls;

    public enum TutorialStage
    {
        PickUp,
        Cooking,
        Cooked,
        TakeOutIng,
        ServingP1,
        Dodge,
        Dash,
        GoodIng,
        Eating,
        BadIng,
        Bin,
        Outro,
        //Cooking2,
        //ServingAgain,
        //GoodIng,
    }

    enum Patterns
    {
        shotgun,
        minigun,
        normal,
    }

    private void Start()
    {
        dialogueSystem = gameObject.AddComponent<DialogueSystem>();
        player = GameObject.FindGameObjectWithTag("Player");
        pHealthScript = player.GetComponent<Health>();
        activeStage = TutorialStage.PickUp;
        objectPooler = ObjectPooler.Instance;
        BulletSpawning.tutorialLevel = true;
        showText = true;
        ingsToSpawn = IngGameController.instance.SpawnableIngredients;
        Table = GameObject.Find("Serving Table");
        TableZone = Table.transform.Find("TableZone").gameObject;
        Oven = GameObject.Find("Oven");
        OvenZone = Oven.transform.Find("oven/OvenZone").gameObject;
        TheBin = GameObject.Find("Bin");
        TheBinZone = TheBin.transform.Find("BinZone").gameObject;
    }

    public GameObject SpawnBullet(Vector3 dir, Bullet bul, string pool, Vector3 origin, Material mat, IngredientType ingType)
    {
        return objectPooler.SpawnFromPool(pool, origin, Quaternion.identity, dir, bul, mat, ingType);
    }

    private void Update()
    {
        CheckTutorialState();
        CheckPhaseEnd();

        /*if (canSpawnWave && pReady == 1)
        {
            canSpawnWave = false;
            pattern = Random.Range(0, 3);
            Invoke("BulletPattern", patternDelay);
        }*/

        //Debug
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    RuntimeManager.PlayOneShot(eSkipPhase);
        //    IncreasePhase();
        //}

        /*
        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E) ||  Input.GetKeyDown(KeyCode.Mouse2)) && dialogue.CheckActive())
        {
            dialogue.HideDialogue();
            showText = false;
            //ResumeMovement();
            //TimeFlip();
            // Dialogue & Enable arrow/text
            dialogueFinished = true;
        }
        */
    }

    public IngredientType DecideIngredient()
    {
        return ingsToSpawn[Random.Range(0, ingsToSpawn.Length)];
    }

    #region Tutorial Loop
    void SendGoodIng()
    {
        if (!Holding.instance.Carrying && !isCooking && (GameObject.FindGameObjectsWithTag("Good").Length <= 0 && GameObject.FindGameObjectsWithTag("Cooked").Length <= 0) && (activeStage == TutorialStage.Eating || activeStage == TutorialStage.GoodIng || activeStage == TutorialStage.PickUp))
        {
            IngredientType randomIng = DecideIngredient();
            SpawnBullet(new Vector3(0 - transform.position.x, 0, 0 - transform.position.z).normalized * 1.4f, bulletTypes[0], "Good_Ingredient", transform.position, randomIng.material, randomIng);
        }
        canSpawnWave = true;
    }

    void SendDashIng()
    {
        if (!Holding.instance.Carrying && !isCooking)
        {
            IngredientType randomIng = DecideIngredient();
            SpawnBullet(new Vector3(15 - transform.position.x, 0, 0f - transform.position.z).normalized * 1.4f, bulletTypes[0], "Good_Ingredient", transform.position, randomIng.material, randomIng);
        }
        canSpawnWave = true;
    }

    void SendBadIng()
    {
        if (!Holding.instance.Carrying && activeStage == TutorialStage.BadIng)
        {
            SpawnBullet(new Vector3(0 - transform.position.x, 0, 0 - transform.position.z).normalized * 1.4f, bulletTypes[0], "Bad_Ingredient", transform.position, defaultMats[1], null);
        }
        canSpawnWave = true;
    }

    void SendPracticeWave()
    {
        if(currWave < 3)
        {
            pattern = currWave % 3;
            BulletPattern();
        }
    }

    void CheckTutorialState()
    {
        Debug.Log("WE ARE ON SPEAKER NUMBER: " + currentSpeaker);
        //Debug.Log(activeStage);
        switch (activeStage)
        {
            case (TutorialStage.PickUp):
                pHealthScript.enabled = false;
                TheBinZone.SetActive(false);
                PickUp();
                if (Holding.instance.Carrying)
                {
                    activeStage++;
                    dialogueFinished = false;
                    showText = true;
                }
                break;

            case (TutorialStage.Cooking):
                Cooking();
                if (activeStage == TutorialStage.Cooking && isCooking)
                {
                    cookingCoroutine = StartCoroutine(CookingTimer());
                    activeStage++;
                    dialogueFinished = false;
                    showText = true;
                }
                else if (activeStage == TutorialStage.Cooking && !Holding.instance.Carrying && !isCooking)
                {
                    StopCoroutine(cookingCoroutine);
                    currentSpeaker = 0;
                    activeStage = 0;
                    showText = true;
                }
                break;

            case (TutorialStage.Cooked):
                Cooked();
                if (activeStage == TutorialStage.Cooked && doneCooking)
                {
                    activeStage++;
                    dialogueFinished = false;
                    doneCooking = false;
                    showText = true;
                }
                else if (activeStage == TutorialStage.Cooked && !Holding.instance.Carrying && !isCooking)
                {
                    Debug.Log("STOP IT! STOP IT ALL" + "   Holding is = " + Holding.instance.Carrying + "    And isCooking = " + isCooking);
                    StopCoroutine(cookingCoroutine);
                    currentSpeaker = 0;
                    activeStage = 0;
                    doneCooking = false;
                    showText = true;
                }
                break;

            case (TutorialStage.TakeOutIng):
                TakeOutIng();
                if (activeStage == TutorialStage.TakeOutIng && !isCooking && GameObject.FindGameObjectsWithTag("Cooked").Length >= 1)
                {
                    activeStage++;
                    dialogueFinished = false;
                    showText = true;
                }
                else if(activeStage == TutorialStage.TakeOutIng && (GameObject.FindGameObjectsWithTag("Good").Length <= 0 && GameObject.FindGameObjectsWithTag("Cooked").Length <= 0))
                {
                    currentSpeaker = 0;
                    activeStage = 0;
                    showText = true;
                }
                break;

            case (TutorialStage.ServingP1):
                ServingP1();
                if (currentPhase == 1)
                {
                    activeStage++;
                    dialogueFinished = false;
                    canSpawnWave = true;
                    showText = true;
                }
                else if (!GameObject.Find("Cooked Ingredient(Clone)") && !isCooking && currentPhase == 0)
                {
                    currentSpeaker = 0;
                    activeStage = 0;
                    showText = true;
                }
                break;

            case (TutorialStage.Dodge):
                pHealthScript.enabled = true;
                OvenZone.SetActive(true);
                Dodge();
                if (currWave == 3)
                {
                    currWave = 4;
                    pReady = 1;
                    canSpawnWave = true;
                    bulletCoroutine = StartCoroutine("afterBullets");
                }
                break;

            case (TutorialStage.Dash):
                Dash();
                if (currWave == 3)
                {
                    currWave = 4;
                    pReady = 1;
                    canSpawnWave = true;
                    bulletCoroutine = StartCoroutine("afterBullets");
                }
                break;

            case (TutorialStage.GoodIng):
                GoodIng2();
                if (Holding.instance.Carrying)
                {
                    activeStage++;
                    dialogueFinished = false;
                    showText = true;
                    //currentSpeaker++;
                }
                break;

            case (TutorialStage.Eating):
                OvenZone.SetActive(false);
                Eating();
                if (!Holding.instance.Carrying)
                {
                    activeStage++;
                    //currentSpeaker++;
                    dialogueFinished = false;
                    currHealth = pHealthScript.CurrentHealth();
                    canSpawnWave = true;
                    showText = true;
                }
                break;

            case (TutorialStage.BadIng):
                BadIng();
                if (Holding.instance.Carrying)
                {
                    if (Holding.instance.heldObj.name == "Bad Ingredient(Clone)")
                    {
                        activeStage++;
                        //currentSpeaker++;
                        dialogueFinished = false;
                        showText = true;
                    }
                }
                break;

            case (TutorialStage.Bin):
                TheBinZone.SetActive(true);
                Bin();
                if (!Holding.instance.Carrying)
                {
                    activeStage++;
                    canSpawnWave = true;
                    dialogueFinished = false;
                    showText = true;
                    //currentSpeaker++;
                }
                break;

            case (TutorialStage.Outro):
                Outro();
                if (!dialogueSystem.CheckActive())
                {
                    FMODUpdateHealth(0);
                    GameManager.instance.sceneToLoad = GameManager.instance.returnToScene;
                    SceneManager.LoadScene(5);
                }
                break;

                /*
            case (TutorialStage.Cooking2):
                Cooking2();
                if (currentPhase == 2)
                {
                    activeStage++;
                    dialogueFinished = false;
                    canSpawnWave = true;
                    showText = true;
                }
                else if (activeStage == TutorialStage.Cooking2 && !Holding.instance.Carrying && !isCooking)
                {
                    activeStage++;
                    showText = true;
                }
                break;

            case (TutorialStage.ServingAgain):
                ServingAgain();
                if (!dialogue.CheckActive())
                {
                    activeStage++;
                    dialogueFinished = false;
                    canSpawnWave = true;
                    showText = true;
                }
                break;

            case (TutorialStage.GoodIng):
                GoodIng();
                if (Holding.instance.Carrying)
                {
                    if (Holding.instance.heldObj.name == "Good Ingredient(Clone)")
                    {
                        activeStage++;
                        dialogueFinished = false;
                        showText = true;
                    }
                }
                break;
                */
        }
    }

    #region TUTORIAL PHASE FUNCTIONS
    void PickUp()
    {
        if (showText)
        {
            showText = false;
            ShowDialogue();    //0,1,2
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
            SendGoodIng();
            //StartCoroutine(FreezeFrame(1.5f, 1, new Vector3(0.8f, 15.5f, -3.5f), -19.05f));
        }
        if (canSpawnWave && !Holding.instance.Carrying && !isCooking)
        {
            Invoke("SendGoodIng", 5f);
            canSpawnWave = false;
        }
    }

    void GoodIng()  
    {
        if (showText)
        {
            showText = false;
            SendGoodIng();
            ShowDialogue();    //3,4
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
        }
        if (canSpawnWave && !Holding.instance.Carrying)
        {
            Invoke("SendGoodIng", 5f);
            canSpawnWave = false;
        }
    }

    void Cooking()
    {
        if (showText)
        {
            showText = false;
            ShowDialogue();    //5
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
        }
    }

    void Cooked()
    {
        if (showText)
        {
            showText = false;
            ShowDialogue();    //6
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
        }
    }

    void TakeOutIng()
    {
        if (showText)
        {
            showText = false;
            ShowDialogue();    //7,8
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
        }
    }

    void ServingP1()
    {
        if (showText)
        {
            showText = false;
            //StartCoroutine(FreezeFrame(4.1f, 3, new Vector3(-11.7f, 1.76f, -3.5f), -33.5f));
            ShowDialogue();    //9,10,11
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
        }
    }

    void Dodge()
    {
        if (showText)
        {
            showText = false;
            ShowDialogue();    //12
            currHealth = pHealthScript.CurrentHealth();
        }

        if (!showText)
        {
            if (pHealthScript.CurrentHealth() < currHealth)
            {
                StopAllCoroutines();
                canSpawnWave = true;
                currWave = 0;
                pHealthScript.Heal(1);
                GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
                foreach (GameObject bul in bullets)
                {
                    bul.SetActive(false);
                }
                //dialogueFinished = false;
            }

            if (canSpawnWave && currWave < 3)
            {
                Invoke("SendPracticeWave", 1);
                canSpawnWave = false;
            }
        }
    }

    void Dash()
    {
        if (showText)
        {
            showText = false;
            ShowDialogue();    //13
            currHealth = pHealthScript.CurrentHealth();
        }

        if (!showText)
        {
            if (pHealthScript.CurrentHealth() < currHealth)
            {
                StopAllCoroutines();
                canSpawnWave = true;
                currWave = 0;
                pHealthScript.Heal(1);
                GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
                foreach (GameObject bul in bullets)
                {
                    bul.SetActive(false);
                }
                //dialogueFinished = false;
            }

            if (canSpawnWave && currWave < 3)
            {
                Invoke("SendPracticeWave", 1f);
                canSpawnWave = false;
            }
        }
        /*
        if (canSpawnWave && !Holding.instance.Carrying && !isCooking)
        {
            Invoke("SendDashIng", 5f);
            canSpawnWave = false;
        }
        */
    }

    void GoodIng2()
    {
        if (showText)
        {
            pReady = 0;
            showText = false;
            ShowDialogue();    //14
            if (pHealthScript.CurrentHealth() == 3)
            {
                pHealthScript.Damage(1, false);
            }
            Debug.Log("updating currentspeaker");
            //currentSpeaker++;
            SendGoodIng();

            if (canSpawnWave && !Holding.instance.Carrying)
            {
                Invoke("SendGoodIng", 5f);
                canSpawnWave = false;
            }
        }
    }

    void Eating()
    {
        if (showText)
        {
            showText = false;
            //ShowDialogue();    //15
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
        }
    }

    void BadIng()
    {
        if (showText)
        {
            showText = false;
            ShowDialogue();    //16,17
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
            SendBadIng();
            //StartCoroutine(FreezeFrame(1.5f, 5, new Vector3(0.8f, 14.37f, -3.5f), -19.05f));
        }
        if (canSpawnWave && !Holding.instance.Carrying)
        {
            Invoke("SendBadIng", 5f);
            canSpawnWave = false;
        }
    }

    void Bin()
    {
        if (showText)
        {
            showText = false;
            ShowDialogue();    //18
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
        }
    }

    void Outro()
    {
        if (showText)
        {
            showText = false;
            ShowDialogue();    //19,20,21
            Debug.Log("updating currentspeaker");
            currentSpeaker++;
        }
    }

    /*
    void Cooking2()
    {
        if (showText)
        {
            ShowDialogue(14);
            showText = false;
        }
    }

    void ServingAgain()
    {
        if (showText)
        {
            ShowDialogue(15);
            showText = false;
        }
    }
    */

    void FinalTest()
    {
        ShowDialogue();    //10 again (last in list)
        pReady = 1;
        canSpawnBadIngredients = true;
        canSpawnWave = true;
        pHealthScript.Heal(0);
    }

    IEnumerator afterBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        while (bullets.Length > 0)
        {
            bullets = GameObject.FindGameObjectsWithTag("Bullet");
            yield return new WaitForSeconds(.1f);
        }

        currWave = 0;
        // Dialogue & Enable arrow/text FOR FINAL TEST
        dialogueFinished = false;


        activeStage++;
        Debug.Log("updating currentspeaker");
        currentSpeaker++;
        showText = true;

        //Invoke("FinalTest", 16);
    }

    #endregion
    #endregion

    #region Main Gameplay Loop
    void CheckPhaseEnd()
    {
        if (TableZone.transform.Find("Cooked Ingredient(Clone)") != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReceivedInput();
            }
        }
    }

    private void ReceivedInput()
    {
        if (IngredientInteraction.instance.canSend)
        {
            GameObject ingredient = TableZone.transform.Find("Cooked Ingredient(Clone)").gameObject;
            ingredient.transform.parent = null;
            ingredient.SetActive(false);

            itemsServed = 0;
            IncreasePhase();
        }
    }

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.ServeEarly.performed += ctx => ReceivedInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void BulletPattern()
    {
        currWave++;

        if (currWave % goodIngredientWaveGap == 0) // Is it a wave that needs to spawn an ingredient
        {
            spawnGoodIngredient = true;
        }

        switch (pattern)
        {
            case (0): //Shotgun pattern
                StartCoroutine(Shotgun());
                break;

            case (1): //Minigun pattern
                StartCoroutine(Minigun());
                break;

            case (2): //Normal bullet pattern
                StartCoroutine(NormalShots());
                break;
        }

    }

    void IncreasePhase()
    {
        pReady = 0;
        currWave = 0;
        spawnGoodIngredient = false;
        currentPhase++;

        if (currentPhase == 3)
        {
            FMODUpdateHealth(0);
            GameManager.instance.sceneToLoad = 1;
            SceneManager.LoadScene(5);
        }
        Debug.Log("New phase begins! " + currentPhase);
    }

    public GameObject DecideBullet(Vector3 dir, int i, Bullet bul, int totalBullets, Vector3 origin)
    {
        if (canSpawnBadIngredients)
        {
            if (Random.Range(1, 101) <= badIngredientSpawnChance)
            {
                return SpawnBullet(dir, bul, "Bad_Ingredient", origin, defaultMats[1], null);
            }
            else
            {
                return SpawnBullet(dir, bul, "Bullet", origin, defaultMats[0], null);
            }
        }
        else
        {
            return SpawnBullet(dir, bul, "Bullet", origin, defaultMats[0], null);
        }
    }

    void ShotgunPattern(float angX, float angZ)
    {
        int totalBullets = Mathf.RoundToInt(30 * bulletCountModifier);
        float angleDif = 180 / totalBullets;
        float startAngleX = angX;
        float startAngleZ = angZ;
        for (int i = 0; i < totalBullets; i++)
        {
            float newAngX = startAngleX + (angleDif * i);
            float newAngZ = startAngleZ + (angleDif * i);
            Vector3 newDir = new Vector3(Mathf.Sin(newAngX * Mathf.Deg2Rad), 0, Mathf.Cos(newAngZ * Mathf.Deg2Rad));
            DecideBullet(newDir, i, bulletTypes[0], totalBullets, transform.position);
        }
    }

    IEnumerator Minigun()
    {
        int totalBullets = Mathf.RoundToInt(15 * bulletCountModifier);
        for (int i = 0; i < totalBullets; i++)
        {
            RuntimeManager.PlayOneShot(eMinigun, transform.position);
            float newDirX = player.transform.position.x - transform.position.x;
            float newDirZ = player.transform.position.z - transform.position.z;
            Vector3 newDir = new Vector3(newDirX, 0, newDirZ).normalized * 1.4f;//new Vector3(Mathf.Sin(ang), 0, Mathf.Cos(ang));
            DecideBullet(newDir, i, bulletTypes[0], totalBullets, transform.position);
            yield return new WaitForSeconds(1.5f / totalBullets);
        }
        canSpawnWave = true;
    }

    IEnumerator NormalShots()
    {
        int totalBullets = Mathf.RoundToInt(5 * bulletCountModifier);
        for (int i = 0; i < 5; i++)
        {
            RuntimeManager.PlayOneShot(eMinigun, transform.position);
            float newDirX = player.transform.position.x - transform.position.x;
            float newDirZ = player.transform.position.z - transform.position.z;
            Vector3 newDir = new Vector3(newDirX, 0, newDirZ).normalized * 1.4f;//new Vector3(Mathf.Sin(ang), 0, Mathf.Cos(ang));
            DecideBullet(newDir, i, bulletTypes[0], totalBullets, transform.position);
            yield return new WaitForSeconds(.5f / totalBullets);
        }
        canSpawnWave = true;
    }

    IEnumerator Shotgun()
    {
        RuntimeManager.PlayOneShot(eShotgun, transform.position);
        ShotgunPattern(-90, 90);
        yield return new WaitForSeconds(0.35f);
        ShotgunPattern(-105, 75);
        canSpawnWave = true;
    }

    // Controls the Low Pass filter and heartbeat when low health
    public void FMODUpdateHealth(int isLowHealth)
    {
        RuntimeManager.StudioSystem.setParameterByName("isLowHealth", isLowHealth);
    }

    #endregion

    IEnumerator FreezeFrame(float delay, int textIndex, Vector3 animPosition, float xRotation)
    {
        yield return new WaitForSeconds(delay);
        //ShowDialogue();
        arrowObject.GetComponent<RectTransform>().localPosition = animPosition;
        arrowObject.GetComponent<RectTransform>().localRotation = Quaternion.Euler(xRotation, 0, 0);
        arrowObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        arrowObject.SetActive(false);
    }

    IEnumerator CookingTimer()
    {
        doneCooking = false;
        yield return new WaitForSeconds(4);
        Debug.Log("I HAVE RUN ONLY ONCE MASTER!");
        doneCooking = true;
    }

    private void StopMovement()
    {
        player.GetComponent<Movement>().enabled = false;
    }

    private void ResumeMovement()
    {
        player.GetComponent<Movement>().enabled = true;
    }

    private void ShowDialogue()
    {
        Debug.Log("displaying dialogue");
        dialogueSystem.ShowDialogue(speakers[currentSpeaker].text, true, speakers[currentSpeaker].name, speakers[currentSpeaker].portrait, speakers[currentSpeaker].eSound, gameObject.transform.position);
    }

    /// <summary>
    /// Either stops or starts time in the game (Time.timeScale)
    /// </summary>
    private void TimeFlip()
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        else Time.timeScale = 0;
    }
}
