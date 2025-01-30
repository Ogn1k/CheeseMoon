using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;

    bool flag = false;

    //public Transform combatLookAt;

    public GameObject thirdPersonCam;
   // public GameObject combatCam;
    //public GameObject topDownCam;

    public GameObject InventoryObj;

    public CameraStyle currentStyle;
    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }

    private void Start()
    {
        LockCursor(true);
    }

    public void LockCursor(bool _lock)
    {
        if(_lock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void FlagThing()
    {
        flag = !flag;
    }

    private void FixedUpdate()
    {
        // switch styles
        if (Input.GetKeyDown(KeyCode.Escape)) flag = !flag;
        //if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
        //if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Combat);
        //if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCameraStyle(CameraStyle.Topdown);

        if(flag)
        { 
            LockCursor(false); 
            InventoryObj.SetActive(true); 
            //InventoryManager.Instance.InlistInventory(); 

        } else 
        { 
            LockCursor(true); 
            InventoryObj.SetActive(false); 
        }

        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z); //+ new Vector3(0, 0, (float)-0.0935)
        orientation.forward = viewDir.normalized;

        // roate player object
        if (currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput; //if you search for a problem - here it is :)

            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        /*else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;
        }*/
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        //combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        //topDownCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        //if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);
        //if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);

        currentStyle = newStyle;
    }
}
