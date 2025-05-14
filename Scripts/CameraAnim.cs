using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraAnim : MonoBehaviour
{
    private Animator anim;
    private Player controls;
    public GameObject firstButtonMainMenu;
    private bool canStart;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            ReceiveInput();
        }
    }

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Any.performed += ctx => ReceiveInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }


    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void ReceiveInput() 
    {
        if (canStart)
        {
            anim.SetTrigger("cameraSwing"); //1.1f
        }
    }

    public void SelectButton() 
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonMainMenu);
    }

    private IEnumerator Wait() 
    {
        yield return new WaitForSeconds(0.4f);
        canStart = true;
    }
}
