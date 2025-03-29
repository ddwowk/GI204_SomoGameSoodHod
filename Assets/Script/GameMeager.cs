using UnityEngine;
using Playroom;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
using System.Linq;
using System;

public class GameMeager : MonoBehaviour
{
    public static GameMeager Instance { get; private set; }
    public PlayroomKit playroomKit = new();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPosition;
    [SerializeField] private ParticleSystem hitEffect,deadEffect;
    [SerializeField] private GameObject winningMenu,mainMenu;

    private int currentSpawnPoint = 0;
    public static readonly Dictionary<string, GameObject> PlayerDick = new();
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();
    private bool isPlayerJoin = false;

    private List<string> losePlayer = new();

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
            defaultPlayerStates = new() { 
            }
        }, () =>
        {
            playroomKit.OnPlayerJoin(PlayerJoin);
            playroomKit.WaitForState("Winner", (string _) => { winningMenu.SetActive(true); winningMenu.GetComponent<WinerUI>().updateData(losePlayer); });
           
        });
        playroomKit.RpcRegister("PlayerHit", AddForceOnCollision);
        playroomKit.RpcRegister("PlayerOutArea", OnOutOfArea);
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
            players[index].SetState("Animetion", playerGameObjects[index].transform.GetChild(0).gameObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name);

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null && PlayerDick.TryGetValue(players[i].id, out GameObject playerObj))
                {
                    var pos = players[i].GetState<Vector3>("Pos");
                    var rot = players[i].GetState<Quaternion>("Rotate");
                    var walkAnim = players[i].GetState<string>("Animetion");
                    if (playerGameObjects != null)
                    {
                        playerGameObjects[index].transform.GetChild(0).gameObject.GetComponent<Animator>().Play(walkAnim);
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
        playerObj.transform.GetChild(0).gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = player.GetProfile().color;
        playerObj.transform.GetChild(1).GetChild(0).gameObject.GetComponentInChildren<TextMeshProUGUI>().color = player.GetProfile().color; 
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
        GameObject hitEffectObj = Instantiate(hitEffect, tagetObj.transform).gameObject;
        Vector3 dir = Vector3.Normalize((tagetObj.transform.position - callerObj.transform.position));
        tagetObj.GetComponent<Rigidbody>().AddForce(callerObj.GetComponent<Rigidbody>().mass * (80 * (dir * callerObj.GetComponent<PlayerController>().moveSpeed)));
        StartCoroutine("WaitFor", 3);
        Destroy(hitEffect);
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

    void OnOutOfArea(string tagetID, string caller)
    {
        GameObject tagetObj = PlayerDick[tagetID];
        Vector3 dir = tagetObj.GetComponent<Rigidbody>().linearVelocity;
        tagetObj.GetComponent<Rigidbody>().AddForce(dir * 40);
        tagetObj.GetComponent<PlayerController>().moveSpeed = 0;
        losePlayer.Add(tagetID);
        GameObject deadEffectObj = Instantiate(deadEffect, tagetObj.transform).gameObject;
        StartCoroutine(WaitFor(1, () => { RemovePlayer(tagetID); }));
        StartCoroutine(WaitFor(3, () => { Destroy(deadEffectObj); }));
        if (PlayerDick.Count <= 1)
        {
            List<string> playerIDList = new(PlayerDick.Keys.ToList());
            foreach (var item in playerIDList)
            {
                Debug.Log(item);
            }
            playroomKit.SetState("Winner", playroomKit.GetPlayer(playerIDList[0]).GetProfile().name);
        }
    }
    public IEnumerator WaitFor(int secon,Action action)
    {
        yield return new WaitForSeconds(secon);
        action();
    }
}