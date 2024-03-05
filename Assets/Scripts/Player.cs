using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance {get; private set;}
    // Game Input
    [SerializeField] private GameInput gameInput; // Input object to get keyboard inputs

    // Physical traits
    [SerializeField] private float playerRadius = 1f; // Player radius
    [SerializeField] private float playerHeight = 2f; // Player height 
    [SerializeField] private float PLAYER_REACH = 3f; // How far Player needs to be to interact

    // Player Movement
    [SerializeField] private float speed = 10f; // How fast a Player moves
    [SerializeField] private float rotateSpeed = 10f; // How fast Player looks in forward direction

    // Player Knowledge
    private Vector3 lastKnownDirection; // Last know direction Player was facing when standing still
    private Counter selectedCounter; // Current counter in-front of Player

    // Events
    public event EventHandler<OnSelectedCounterEventArgs> OnSelectingCounter;
    public class OnSelectedCounterEventArgs:EventArgs{
        public Counter selectedCounter;
    }
    // Event Handleer
    private void Handle_Interaction(object sender, System.EventArgs e){
        if(selectedCounter != null){
            selectedCounter.Interact();
        }  
    }
    // Awake
    private void Awake(){
        if(Instance != null){ // There is more than one player;
            Debug.LogError("There is more than one player");
        }
        Instance = this;
    }
    
    // Start
    private void Start(){
        // Listen for OnInteract event Shout!!!
        gameInput.OnInteract += Handle_Interaction;
    }

    // Update
    private void Update(){
        walk(); //Check if player wants to walk
        CanInteract(); //Check if player in close enough to interact with Counters
    }

    // Walk
    private void walk(){
        // Calulate if the player can move using raycast
        float moveDistance = speed * Time.deltaTime;

        Vector2 inputVector = gameInput.GetMovementVector(); // get movement vector
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y); // Translate to 3d movment

        // RayCast, Rather CapsuleCast to see if player is obstructed
        bool canWalk = !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        // Path clear? Then move
        if (canWalk){
            transform.position += moveDir * moveDistance; // move the player relative to the speed and frame rate
        }

        // Make sure player is facing the direction of movement
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime*rotateSpeed); // smooth transistion with slerp
    }
    // Can Interact?
    private void CanInteract(){
        Vector2 inputVector = gameInput.GetMovementVector(); // get movement vector
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y); // Translate to 3d movment

        if(moveDir != Vector3.zero){
            lastKnownDirection = moveDir;
        }
        // RayCast to know if the player is close enough to interact with anything, Counter, Bin, etc
        if(Physics.Raycast(transform.position, lastKnownDirection, out RaycastHit raycastHit, PLAYER_REACH)){
            // If the Player is infront of a Counter
            if(raycastHit.transform.TryGetComponent(out Counter counter)){
                // If the counter we are looking at is not our selected counter? i.e. each player has 1 selected counter; Null/counter.
                if(counter != selectedCounter){
                    // Make it our selected counter; Then trigger an event for the counter to change color.
                    SetSelectedCounter(counter);
                }
            }
            // If we are not in front of a Counter
            else{
                // Set our selected counter to be Null; e.g. we aren't close to a counter to select.
                // If we don't do this, once we approach a counter and walk away, it would always be selected until we go to another counter.
                SetSelectedCounter(null);
            }
        }
        // If the RayCast doesn't hit anything within PLAYER_REACH
        else{
            // Set our selected counter to be Null;
            SetSelectedCounter(null);
        };
    }

    private void SetSelectedCounter(Counter counter){
        selectedCounter = counter;

        OnSelectingCounter?.Invoke(this, new OnSelectedCounterEventArgs{
            selectedCounter = selectedCounter
        });
    }
}
