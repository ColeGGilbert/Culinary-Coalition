using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class LevelClear : MonoBehaviour
{
    private Animator victoryAnim;

    [SerializeField] private GameObject camObject;
    private Animator victoryPan;

    [SerializeField] private GameObject transitionObject;
    private Animator transitionAnim;

    [SerializeField] private GameObject player;

    Quaternion targetRotation;
    public Transform target;            // target object 
    public float speed = 2F;          // speed scaling factor
    bool rotating = false;              // toggles the rotation, after targeting, toggle true, false after arrives

    float rotationTime; // when rotationTime == 1, will have rotated to our target

    [SerializeField]
    private GameEvent onClear = null;

    // Start is called before the first frame update
    void Start()
    { 
        victoryAnim = GetComponentInChildren<Animator>();
        transitionAnim = transitionObject.GetComponent<Animator>();

        victoryPan = camObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePosition = target.position - transform.position;
        targetRotation = Quaternion.LookRotation(relativePosition);
        rotationTime = 0;

        if (rotating)
        {
            rotationTime += Time.deltaTime * speed;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationTime);
            if (rotationTime > 1)
            {
                rotating = false;

            }
        }
    }
    public void PlayVictoryAnim()
    {
        StartCoroutine(ACTIVENO());

        onClear?.Raise();

        rotating = true;

        victoryPan.SetTrigger("Victory");
        transitionObject.SetActive(true);
        Invoke("victoryJump", .5f);

        GetComponent<Movement>().enabled = false;
    }

    private void OnEnable()
    {

        EndingText.OnStageClear += PlayVictoryAnim;

    }

    private void OnDisable()
    {
        StopAllCoroutines();
        EndingText.OnStageClear -= PlayVictoryAnim;
    }

    IEnumerator ACTIVENO()
    {
        while (true)
        {
            foreach (GameObject bul in GameObject.FindGameObjectsWithTag("Bullet"))
            {
                bul.SetActive(false);
            }
            foreach (GameObject bul in GameObject.FindGameObjectsWithTag("Bad"))
            {
                bul.SetActive(false);
            }
            foreach (GameObject bul in GameObject.FindGameObjectsWithTag("Good"))
            {
                bul.SetActive(false);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
   public void victoryJump()
    {
        victoryAnim.SetTrigger("isWin");
        Invoke("TransitionEnter", 1.4f);
    }

   public void TransitionEnter()
    {
        transitionAnim.SetTrigger("End");
    }
}
