using UnityEngine;
using Playroom;
using System.Collections.Generic;

public class GameMeager : MonoBehaviour
{
    public static GameMeager Instance { get; private set; }
    public PlayroomKit playroomKit { get; private set; } = new();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPosition;
    private int currentSpawnPoint = 0;
    public static readonly Dictionary<string, GameObject> PlayerDick = new();
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();
    private bool isPlayerJoin = false;

    private GameMeager() { }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 4,
            defaultPlayerStates = new()
        }, () =>
        {
            playroomKit.OnPlayerJoin(PlayerJoin);
        });
        playroomKit.RpcRegister("PlayerHit", AddForceOnCollision);
    }

    private void Update()
    {
        if (isPlayerJoin)
        {
            var myplayer = playroomKit.MyPlayer();
            var index = players.IndexOf(myplayer);

            playerGameObjects[index].GetComponent<PlayerController>().Move();

            players[index].SetState("Pos", playerGameObjects[index].transform.position);
            players[index].SetState("Rotate", playerGameObjects[index].transform.rotation);

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null && PlayerDick.TryGetValue(players[i].id, out GameObject playerObj))
                {
                    var pos = players[i].GetState<Vector3>("Pos");
                    var rot = players[i].GetState<Quaternion>("Rotate");
                    if (playerGameObjects != null)
                    {
                        playerGameObjects[i].GetComponent<Transform>().position = pos;
                        playerGameObjects[i].GetComponent<Transform>().rotation = rot;
                    }
                }
            }
        }
    }

    void PlayerJoin(PlayroomKit.Player player)
    {
        Transform spawnPos = spawnPosition[currentSpawnPoint];
        currentSpawnPoint++;
        if (currentSpawnPoint >= spawnPosition.Length)
        {
            currentSpawnPoint = 0;
        }
        GameObject playerObj = Instantiate(playerPrefab, spawnPos.position, spawnPos.rotation);
        playerObj.name = player.id;
        //playerObj.GetComponentInChildren<MeshRenderer>().material.color = player.GetProfile().color;
        player.SetState("Name", player.GetProfile().name);

        PlayerDick.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);
        isPlayerJoin = true;
        player.OnQuit(RemovePlayer);
    }
    
    void AddForceOnCollision(string tagetID,string callerID)
    {
        Debug.Log(tagetID);
        GameObject tagetObj = PlayerDick[tagetID];
        GameObject callerObj = PlayerDick[callerID];
        Vector3 dir = Vector3.Normalize((tagetObj.transform.position - callerObj.transform.position));
        tagetObj.GetComponent<Rigidbody>().AddForce(callerObj.GetComponent<Rigidbody>().mass * (100 * (dir * callerObj.GetComponent<PlayerController>().moveSpeed)));
    }

    void RemovePlayer(string playerID)
    {
        if (PlayerDick.TryGetValue(playerID, out GameObject playerObj))
        {
            PlayerDick.Remove(playerID);
            players.Remove(players.Find(p => p.id == playerID));
            playerGameObjects.Remove(playerObj);
            Destroy(playerObj);
        }
    }
}