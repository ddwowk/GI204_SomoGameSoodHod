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
    [SerializeField] private AudioClip[] audioClip,deadAudio;
    private int currentSpawnPoint = 0;
    public static readonly Dictionary<string, GameObject> PlayerDick = new();
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();
    private bool isPlayerJoin = false;

    private List<string> losePlayer = new();

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
            SoundManeger.Instance.PlaySound(audioClip[0],true);
           
        });
        playroomKit.RpcRegister("PlayerHit", AddForceOnCollision);
        playroomKit.RpcRegister("PlayerOutArea", OnOutOfArea);
    }

    private void Update()
    {
        if (isPlayerJoin)
        {
            if (playerGameObjects.Count <= 1)
            {
                if (PlayerDick.Keys.Count > 0)
                {

                    playroomKit.SetState("Winner", playroomKit.GetPlayer(PlayerDick.Keys.ToArray()[0]).GetProfile().name);
                }
            }
            var myplayer = playroomKit.MyPlayer();
            var index = players.IndexOf(myplayer);
            if (playerGameObjects[index].GetComponent<PlayerController>() != null)
            {
                playerGameObjects[index].GetComponent<PlayerController>().Move();
            }
            playroomKit.WaitForState("Winner", (string _) => { winningMenu.SetActive(true); winningMenu.GetComponent<WinerUI>().updateData(losePlayer); SoundManeger.Instance.m_AudioSource.Stop(); SoundManeger.Instance.PlaySound(audioClip[1], true); });
            players[index].SetState("Pos", playerGameObjects[index].transform.position);
            players[index].SetState("Rotate", playerGameObjects[index].transform.rotation);
            players[index].SetState("Walk", playerGameObjects[index].GetComponent<PlayerController>().isWalk);

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null && PlayerDick.TryGetValue(players[i].id, out GameObject playerObj))
                {
                    var pos = players[i].GetState<Vector3>("Pos");
                    var rot = players[i].GetState<Quaternion>("Rotate");
                    bool walk = players[i].GetState<bool>("Walk");
                    if (playerGameObjects != null)
                    {
                        playerGameObjects[i].GetComponent<PlayerController>().isWalk = walk;
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
        player.SetState("Walk",false);

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
        tagetObj.GetComponent<Rigidbody>().AddForce(callerObj.GetComponent<Rigidbody>().mass * (100 * (dir * callerObj.GetComponent<PlayerController>().moveSpeed)), ForceMode.Impulse);
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
        SoundManeger.Instance.PlaySound(deadAudio[UnityEngine.Random.Range(0, deadAudio.Length)], false, tagetObj.GetComponent<AudioSource>());
        Debug.Log(playerGameObjects.Count);
    }
    public IEnumerator WaitFor(int secon,Action action)
    {
        yield return new WaitForSeconds(secon);
        action();
    }
}