using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform viewPoint;
    [SerializeField] private float mouseSens = 1f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    private float rotYStore;
    private Vector2 mouseInput;
    private Vector3 moveDir;
    private Vector3 movement;

    [SerializeField] private float attackSpeed = 0.1f;

    [SerializeField] private float jumpForce = 3f, gravMod = 2.5f;

    float activeMoveSpeed => charCtrl.isGrounded && Input.GetKey(KeyCode.LeftShift) == true ? runSpeed : walkSpeed;

    private CharacterController charCtrl;

    private Camera cam;

    [SerializeField] LayerMask groundLayers;

    [SerializeField] GameObject bulletHole;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        charCtrl = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        MouseControl();
        Movement();

        if (Input.GetMouseButton(0))
        {
            attackSpeed -= Time.deltaTime;

            if (attackSpeed <= 0)
            {
                Shoot();
            }
        }
    }
    void MouseControl()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y") * mouseSens);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        rotYStore += mouseInput.y;

        rotYStore = Mathf.Clamp(rotYStore, -60f, 60f);

        viewPoint.rotation = Quaternion.Euler(-rotYStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
    }
    void Movement()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        float yVel = movement.y;
        movement = (moveDir.z * transform.forward) + (moveDir.x * transform.right);
        movement.y = yVel;
        

        if (charCtrl.isGrounded)
        {
            movement.y = 0;
        }

        if (Input.GetButton("Jump") && charCtrl.isGrounded)
        {
            movement.y = jumpForce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravMod;

        charCtrl.Move( movement.normalized * Time.deltaTime * activeMoveSpeed);
    }
    void Shoot()
    {
        attackSpeed = 0.1f;
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        ray.origin = cam.transform.position;

        if(Physics.Raycast(ray, out RaycastHit hit, groundLayers))
        {
            GameObject bulletHoleObj= Instantiate(bulletHole, hit.point + (hit.normal * 0.02f), Quaternion.LookRotation(-hit.normal, Vector3.up));
            Destroy(bulletHoleObj, 10f);
        }
    }
}
