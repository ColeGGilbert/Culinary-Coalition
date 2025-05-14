using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFollow : MonoBehaviour
{

    [SerializeField] GameObject player;
    [SerializeField] Health playerHealth;
    [SerializeField] HatcuumController hatcuum;
    [SerializeField] GameObject shield;
    [SerializeField] float growIncrement = 2.5f;
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;
    private Player controls;

    bool start;
    int foodEaten;

    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if(playerHealth == null)
        {
            playerHealth = player.GetComponent<Health>();
        }
        if (hatcuum == null)
        {
            hatcuum = player.GetComponent<HatcuumController>();
        }
        if(shield == null)
        {
            shield = transform.GetChild(0).gameObject;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        InvokeRepeating("RandomEvent", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(!start)  Open();

        if (Vector3.Distance(player.transform.position, transform.position) > 3)
        {
            Vector3 newDir = player.transform.position - transform.position;
            rb.velocity = Vector3.Lerp(rb.velocity, (speed * newDir * Time.deltaTime), 0.1f);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        transform.LookAt(player.transform.position);

        if (Input.GetKeyDown(KeyCode.M))
        {
            RecivedInput();
        }

    }

    void RandomEvent()
    {
        if(Random.Range(0, 11) == 0)
        {
            switch(Random.Range(0, 2))
            {
                case(0):
                    if (playerHealth.CurrentHealth() < 3 && foodEaten > 0)
                    {
                        foodEaten--;
                        transform.localScale -= Vector3.one * growIncrement;
                        playerHealth.Heal(1);
                    }
                    break;

                case(1):
                    if (foodEaten > 0)
                    {
                        foodEaten--;
                        transform.localScale -= Vector3.one * growIncrement;
                        hatcuum.IncreaseCharges(1);
                    }
                    break;

                case (2):
                    if(foodEaten > 0)
                    {
                        StopAllCoroutines();
                        foodEaten--;
                        transform.localScale -= Vector3.one * growIncrement;
                        StartCoroutine(ActivateShield(4.5f));
                    }
                    break;
            }
            
        }
    }

    IEnumerator ActivateShield(float duration)
    {
        shield.SetActive(true);
        yield return new WaitForSeconds(duration);
        shield.SetActive(false);
    }

    private void Open()
    {
        start = true;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (playerHealth == null)
        {
            playerHealth = player.GetComponent<Health>();
        }
        if (hatcuum == null)
        {
            hatcuum = player.GetComponent<HatcuumController>();
        }
        if (shield == null)
        {
            //shield = transform.GetChild(0).gameObject;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        InvokeRepeating("RandomEvent", 1, 1);
    }

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Up.performed += ctx => RecivedInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void RecivedInput() 
    {
        if (Holding.instance.Carrying)
        {
            if (Holding.instance.heldObj.CompareTag("Good") || Holding.instance.heldObj.CompareTag("Cooked"))
            {
                foodEaten++;
                transform.localScale += Vector3.one * growIncrement;
                Holding.instance.heldObj.SetActive(false);
                Holding.instance.heldObj = null;
                Holding.instance.Carrying = false;
                Holding.instance.heldIngType = null;
            }
        }
    }
}
