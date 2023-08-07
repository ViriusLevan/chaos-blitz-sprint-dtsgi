using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed=10;
    private Vector2 movementInput;

    private Rigidbody rb; 

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.Translate(new Vector3(movementInput.x, 0, movementInput.y)*speed*Time.deltaTime);
    }

    private void FixedUpdate() 
    {
        //Debug.Log(movementInput);
        //rb.velocity = (new Vector3(movementInput.x, rb.velocity.y, movementInput.y)* speed);
    }

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext ctx){
        //rb.velocity = new Vector3(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
        rb.AddForce(new Vector3(0,20f,0), ForceMode.Impulse);
    }

    [SerializeField]private PlayerInput playerInput;

    private void OnEnable() {
        playerInput.actions["CameraLook"].performed += OnMove;
        playerInput.actions["Jump"].performed += OnJump;
    }

    public void BuildingMode(){
        CameraController.Instance.EnableBuilding();
    }


}
