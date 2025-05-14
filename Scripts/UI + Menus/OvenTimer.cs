using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class OvenTimer : MonoBehaviour
{
	[SerializeField] private Image loadingBar;
	[SerializeField] private GameObject backgroundImage;
	private float timeToCook;
	private float timeToBrun;
	private float currentTime = 0;
	private bool cooking = false;
	private bool burning = false;

    public bool canTake;
    public static OvenTimer instance;

	[SerializeField] private ParticleSystem smokePS = null;
	[SerializeField] private ParticleSystem firePS = null;
	AnimationCurve sizeCurve = new AnimationCurve(); // Size Over Lifetime curve for smoke and fire particle systems

	[Header("FMOD")]
	[EventRef] private string eBurning = "{05ad9a7a-a4fc-4b57-9da1-bd09ea0af095}";
	private EventInstance iBurning;

	// Use this for initialization
	void Start()
	{
        instance = this;  //Allows this script be called in other scripts

        loadingBar.GetComponent<Image>().color = Color.green;
		loadingBar.fillAmount = 0;

		// Initialises FMOD instance
		iBurning = RuntimeManager.CreateInstance(eBurning);
		iBurning.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
		iBurning.start();

		// Initialises curve
		sizeCurve.AddKey(0, 0);
		sizeCurve.AddKey(0.7f, 0);
		sizeCurve.AddKey(1, 0);
	}

	// Update is called once per frame
	void Update()
	{
		if (cooking) CookingTimer();
		if (burning) BurningTimer(timeToBrun);
	}

	void OnEnable()
	{
		Oven.OvenTimer += TimerOn;		//listens out for events from the oven script
		Oven.StopTimer += TimerOff;
	}

	void OnDisable()
	{
		Oven.OvenTimer -= TimerOn;
		Oven.StopTimer -= TimerOff;
	}

	/// <summary>
	/// Resets values for the timer
	/// </summary>
	private void TimerOn(float cookingTime, float burningTime) 
	{
		timeToCook = cookingTime;
		timeToBrun = burningTime;
		burning = false;
		loadingBar.GetComponent<Image>().color = Color.green;   //sets colour to be green while cooking
		backgroundImage.SetActive(true);
		currentTime = 0;
		loadingBar.fillAmount = 0;
		cooking = true;
	}

	private void TimerOff() 
	{
		// Disables particle systems
		smokePS.gameObject.SetActive(false);
		firePS.gameObject.SetActive(false);
		backgroundImage.SetActive(false);

		loadingBar.fillAmount = 0;  //turns off the oven when you take an ingredient out of the oven
		iBurning.setParameterByName("ingredientBurn", 0);
		burning = false;
        canTake = false;
        PopUp.instance.F.SetActive(false);
	}

	private void CookingTimer()
	{
		loadingBar.fillAmount = currentTime / timeToCook;
		currentTime += Time.deltaTime;

		if (currentTime >= timeToCook)
		{
			cooking = false;		//resets values at the burningtimer function shares a lot of the same values
			currentTime = 0;        //runs when the ingredient is finished cooking
			loadingBar.fillAmount = 0;
			burning = true;
            if (!Holding.instance.Carrying)
            {
                canTake = true;
            }
			loadingBar.GetComponent<Image>().color = Color.red;		 //sets colour to be red while burning
		}
	}

	private void BurningTimer(float timeToBurn)
	{
		float burnProgress = currentTime / timeToBurn;
		UpdateBurningParticleSize(burnProgress);
		loadingBar.fillAmount = burnProgress;
		iBurning.setParameterByName("ingredientBurn", burnProgress);
		currentTime += Time.deltaTime;

		if (currentTime >= timeToBurn)
		{
			TimerOff();       //runs when the ingredient burns
		}
	}

	/// <summary>
	/// Enables and sets the size of Smoke and Fire particles to the param passed in
	/// Size is increased over burn progress
	/// SizeOverLifetime modules are cached as local variables for modification
	/// </summary>
	/// <param name="progress"></param>
	private void UpdateBurningParticleSize(float progress)
    {
		smokePS.gameObject.SetActive(true);
		firePS.gameObject.SetActive(true);
		var smokeSize = smokePS.sizeOverLifetime;
		var fireSize = firePS.sizeOverLifetime;
		sizeCurve.MoveKey(1, new Keyframe(0.7f, progress));
		smokeSize.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
		fireSize.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
	}
}