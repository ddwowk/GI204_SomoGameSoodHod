using UnityEngine;

public class Area : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.CompareTag("Player"))
        {
            GameMeager.Instance.playroomKit.RpcCall("PlayerOutArea",other.gameObject.name);
        }
    }
}
