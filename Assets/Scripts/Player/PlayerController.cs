using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    public LayerMask solidObjetsLayer;
    public LayerMask interactableLayer;
    public LayerMask dangerZoneLayer;

    public event Action OnEncountered;

    private Animator animator;

    private int stepsTaken = 0;
    public int stepsNeededForBattle = 10000;

    private bool isMoving;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void HandleUpdate()
    {

        var targetPos = transform.position;
        Vector3 input = new Vector3(0, 0);
        isMoving = true;

        if (Input.GetKey(KeyCode.W))
        {
            input.x = 0;
            input.y = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            input.x = -1;
            input.y = 0;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            input.x = 0;
            input.y = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            input.x = 1;
            input.y = 0;
        }
        else
        {
            isMoving = false;
        }

        if(input != Vector3.zero)
        {
            animator.SetFloat("moveX", input.x);
            animator.SetFloat("moveY", input.y);

            targetPos += moveSpeed * input;

            if(isWalkable(targetPos))
                transform.position = targetPos;

            CheckForEncounters();
        }


        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

    }

    private void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

       var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    private bool isWalkable(Vector3 targetPos)
    {

        if (Physics2D.OverlapBox(targetPos, new Vector2(0.8f,0.8f), 90, solidObjetsLayer | interactableLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, dangerZoneLayer) != null)
        {

            stepsTaken++;

            if (stepsTaken > stepsNeededForBattle && Random.Range(1, 10000) <= 1000)
            {
                Debug.Log("Encountered an enemy");
                isMoving = false;
                animator.SetBool("isMoving", false);
                OnEncountered();
                stepsTaken = 0;
            }
        }
    }
}
