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
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        move = InputSystem.actions.FindAction("Jump");
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
        }
        else
        {
            gameObject.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }

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
            if (collision.gameObject.CompareTag("Player") && Physics.Raycast(transform.position, transform.forward, 10, checkRayPlayer))
            {
                GameMeager.Instance.playroomKit.RpcCall("PlayerHit",collision.gameObject.name);
            }
        }
    }
}