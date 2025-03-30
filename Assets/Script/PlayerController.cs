using UnityEngine;
using UnityEngine.InputSystem;
using Playroom;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private InputAction move;
    private Rigidbody rb;
    public float rotateSpeed, moveSpeed;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Canvas canvas;
    [SerializeField] LayerMask checkRayPlayer;
    private Animator animator;
    public bool isWalk = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        move = InputSystem.actions.FindAction("Jump");
        animator = gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
        checkRayPlayer = LayerMask.GetMask("Player");
    }
    private void Start()
    {
        textMesh.text = GameMeager.Instance.playroomKit.GetPlayer(gameObject.name).GetProfile().name;
    }
    public void Move()
    {
        if (move.IsInProgress())
        {
            gameObject.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            isWalk = true;
        }
        else
        {
            gameObject.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
            isWalk = false;
        }
        animator.SetBool("onPressWalk", isWalk);
    }
    private void LateUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        canvas.transform.LookAt(Camera.main.transform);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (move.IsInProgress())
        {
            if (collision.gameObject.CompareTag("Player") && Physics.Raycast(transform.position, transform.forward, 500, checkRayPlayer))
            {
                GameMeager.Instance.playroomKit.RpcCall("PlayerHit", collision.gameObject.name);
            }
        }
    }
}