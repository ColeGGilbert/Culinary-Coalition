using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Oven : MonoBehaviour
{
    public static Oven instance;
    public bool isCooking = false;

    [SerializeField] bool allowIngredientSwapout;

    [SerializeField] ParticleSystem cookedPS;
    [SerializeField] ParticleSystem outovenPS;

    [SerializeField] Animator ovenAnim;

    [SerializeField] private GameObject cookedIngredient;
    [SerializeField] private float cookingTime = 4;
    [SerializeField] private float burningTime = 8;
    public bool isBurning;
    private bool insideOven; // inside Oven Zone
    private Player controls;
    GameObject player;

    IngredientInteraction IngredientScript;

    public IngredientType ingredientType;

    [SerializeField] Shader defShader;

    public delegate void OvenOn(float cookTime, float burnTime);
    public static event OvenOn OvenTimer;
    public delegate void OvenOff();
    public static event OvenOff StopTimer;
    public delegate void OvenDing(IngredientType ing);
    public static event OvenDing ItemCooked;

    [Header("FMOD")]
    [EventRef] private string ePutInOven = "{ccb13a80-d325-422f-8b02-1d8af5c48a5a}";
    [EventRef] private string eCooking = "{cfab9ef8-a206-438f-9ccb-7839df9dabeb}";
    [EventRef] private string eCooked = "{3d8fe9bc-e02f-4e60-97e5-9ea57c341bc2}";
    [EventRef] private string eBurnt = "{83d086da-f425-4053-ae87-1bc7a6045ebe}";
    [EventRef] private string eTakeOutOfOven = "{18c899f2-5ae7-47ce-88b4-b498dadd1997}";
    private EventInstance iCooking;

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Interaction.performed += ctx => ReceivedInput();
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.instance.paused)
        {
            if (Input.GetKeyDown(KeyCode.F)) ReceivedInput();

            // Debug.Log("INSIDE: " + insideOven);
            // Debug.Log("BURNING: " + isBurning);
        }
    }

    private IEnumerator Cooking(IngredientType ingInOven)
    {
        RuntimeManager.PlayOneShot(ePutInOven, transform.position);
        iCooking = RuntimeManager.CreateInstance(eCooking);
        iCooking.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        iCooking.start();
        if (!BulletSpawning.tutorialLevel)
        {
            BulletSpawning.isCooking = true;
        }
        else
        {
            TutorialManager.isCooking = true;
        }
        OvenTimer?.Invoke(cookingTime, burningTime);        //event to start the oven timer UI
        isCooking = true;
        ovenAnim.SetTrigger("Cooking");
        ingredientType = ingInOven;
        Holding.instance.heldIngType = null;
        Holding.instance.Carrying = false;
        Holding.instance.heldObj = null;
        yield return new WaitForSeconds(cookingTime);   //cooking delay
        ovenAnim.SetTrigger("FinishedCooking");
        iCooking.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        iCooking.release();
        RuntimeManager.PlayOneShot(eCooked, transform.position);
        cookedPS.Play();

        ItemCooked?.Invoke(ingInOven);
        StartCoroutine(Burning());
    }

    public IEnumerator Burning()
    {
        isBurning = true;
        yield return new WaitForSeconds(burningTime);       //burn delay

        if (!BulletSpawning.tutorialLevel)
        {
            BulletSpawning.isCooking = false;
        }
        else
        {
            TutorialManager.isCooking = false;
        }

        RuntimeManager.PlayOneShot(eBurnt, transform.position);
        isBurning = false;
        isCooking = false;

        foreach (Transform child in transform)  //destroys the child ingredient, when it is burned
        {
            child.parent = null;
            child.gameObject.SetActive(false);
        }
    }

    public void PickUp() 
    {
        if (insideOven && isBurning)
        {
            outovenPS.Play();
            RuntimeManager.PlayOneShot(eTakeOutOfOven, transform.position);
            GameObject instCooked = Instantiate(cookedIngredient, new Vector3(transform.position.x + 2.5f, transform.position.y, transform.position.z), Quaternion.identity);
            Material replaceMat = ingredientType.cookedMaterial;
            instCooked.GetComponent<Renderer>().material = replaceMat; instCooked.GetComponent<MeshFilter>().mesh = ingredientType.modelPrefab;
            instCooked.GetComponent<PickUp>().ingredientType = ingredientType;
            instCooked.transform.localScale = ingredientType.size;
            instCooked.transform.parent = player.GetComponent<IngredientInteraction>().fryingPanObjectRef.transform;
            instCooked.transform.localPosition = new Vector3(0, 1.5f, 0);
            Holding.instance.Carrying = true;
            Holding.instance.heldObj = instCooked;
            Holding.instance.heldIngType = ingredientType;
            player.GetComponent<IngredientInteraction>().UpdateHeldUI();
            StopAllCoroutines();    //stops Burning and Cooking functions, so food doesn't continue to burn once it is out the oven
            isCooking = false;
            isBurning = false;
            StopTimer?.Invoke();    //event to turn off the oven ui

            foreach (Transform child in transform)  //destroys the child ingredient, when the player picks up a cooked ingredient
            {
                child.parent = null;
                child.gameObject.SetActive(false);
            }

            if (!BulletSpawning.tutorialLevel)
            {
                BulletSpawning.isCooking = false;
            }
            else
            {
                TutorialManager.isCooking = false;
            }
        }
    }

    public void SwapOut() 
    {
        if (insideOven && isBurning)
        {
            outovenPS.Play();
            RuntimeManager.PlayOneShot(eTakeOutOfOven, transform.position);
            GameObject instCooked = Instantiate(cookedIngredient, new Vector3(transform.position.x + 2.5f, transform.position.y, transform.position.z), Quaternion.identity);
            Material replaceMat = new Material(ingredientType.material); replaceMat.color += Color.magenta;
            instCooked.GetComponent<Renderer>().material = replaceMat; instCooked.GetComponent<MeshFilter>().mesh = ingredientType.modelPrefab;
            instCooked.GetComponent<PickUp>().ingredientType = ingredientType;
            isBurning = false;
            isCooking = false;
            foreach (Transform child in transform)  //destroys the child ingredient, when the player picks up a cooked ingredient
            {
                child.parent = null;
                child.gameObject.SetActive(false);
            }
            if (!BulletSpawning.tutorialLevel)
            {
                BulletSpawning.isCooking = false;
            }
            else
            {
                TutorialManager.isCooking = false;
            }
            StopTimer?.Invoke();    //event to turn off the oven ui
            StopAllCoroutines();    //stops Burning and Cooking functions, so food doesn't continue to burn once it is out the oven
            //Cooking();
            instCooked.transform.parent = GameObject.Find("Player").transform;
            instCooked.transform.localPosition = new Vector3(1, 0.5f, 0);
            Holding.instance.Carrying = true;
            Holding.instance.heldObj = instCooked;
        }
    }

    private void StartOven()
    {
        if (!isCooking && !isBurning) 
        {
            StartCoroutine(Cooking(Holding.instance.heldIngType));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the player is inside the oven zone
        if (other.CompareTag("Player"))
        {
            insideOven = true;      
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Checks if the player has left the oven zone
        if (other.CompareTag("Player"))
        {
            insideOven = false;
        }
    }

    void OnEnable()
    {
        IngredientInteraction.OnOvenStart += StartOven;        //listens out for the event from the Ingredient script
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        IngredientInteraction.OnOvenStart -= StartOven;
        controls.Gameplay.Disable();
    }

    private void ReceivedInput() 
    {
        if (Time.timeScale > 0)
        {
            if (!Holding.instance.Carrying && insideOven)
            {
                PickUp(); //takes ingredient out of the oven and spawns cooked ingredient 
            }
            else if (Holding.instance.heldObj.tag == "Good" && isBurning && insideOven)
            {
                GameObject goodIngredient = Holding.instance.heldObj;
                IngredientType goodIngredientType = Holding.instance.heldIngType;
                goodIngredient.transform.parent = null;
                PickUp();
                GameObject newIng = Holding.instance.heldObj;
                IngredientType newIngType = Holding.instance.heldIngType;
                goodIngredient.transform.parent = gameObject.transform;
                goodIngredient.transform.localPosition = new Vector3(0, 1, 0);
                StartCoroutine(Cooking(goodIngredientType));
                Holding.instance.Carrying = true;
                Holding.instance.heldObj = newIng;
                Holding.instance.heldIngType = newIngType;
            }
        }
    }
}
