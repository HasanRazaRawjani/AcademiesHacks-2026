using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player_Controller : MonoBehaviour
{
    public AudioSource playerWalkSFX;
    public AudioSource playerJumpSFX;
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 5f;
    public float lookSensitivity = 2f;

    [Header("References")]
    public Transform playerCamera;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded;

    [Header("Health")]
    public float maxHealthAmount = 100f;
    private float currentHealthAmount;
    public Text healthText;
    public Image healthImage;

    [Header("Sprint")]
    public float maxSprintAmount = 100f;
    private float currentSprintAmount;
    public float sprintSpeed = 12f;
    public float sprintDepletionRate = 20f;
    public float sprintRegenRate = 15f;
    public Text sprintText;
    public Image sprintImage;
    private float defaultMoveSpeed;

    void Start()
    {
        currentHealthAmount = maxHealthAmount;
        rb = GetComponent<Rigidbody>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb.freezeRotation = true; 
        
        defaultMoveSpeed = moveSpeed;
        currentSprintAmount = maxSprintAmount;
    }

    void Update()
    {

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            if (!playerWalkSFX.isPlaying) playerWalkSFX.Play();
        }
        else
        {
            playerWalkSFX.Stop();
        }

        currentHealthAmount = Mathf.Clamp(currentHealthAmount, 0, maxHealthAmount);
        healthImage.fillAmount = currentHealthAmount / maxHealthAmount;
        healthText.text = "Health : " + Mathf.RoundToInt(currentHealthAmount) + "%";

        currentSprintAmount = Mathf.Clamp(currentSprintAmount, 0, maxSprintAmount);
        sprintImage.fillAmount = currentSprintAmount / maxSprintAmount;
        sprintText.text = "Stamina : " + Mathf.RoundToInt((currentSprintAmount / maxSprintAmount) * 100) + "%";

        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && currentSprintAmount > 0 && moveZ > 0)
        {
            moveSpeed = sprintSpeed;
            currentSprintAmount -= sprintDepletionRate * Time.deltaTime;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            if (currentSprintAmount < maxSprintAmount)
                currentSprintAmount += sprintRegenRate * Time.deltaTime;
        }

        Vector3 moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 targetMoveVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(targetMoveVelocity.x, currentVelocity.y, targetMoveVelocity.z);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if (!playerJumpSFX.isPlaying) playerJumpSFX.Play();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (currentHealthAmount <= 0)
        {
            Die();
        }
    }

    public void takeDamage(int damage)
    {
        currentHealthAmount -= damage;
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f) 
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}