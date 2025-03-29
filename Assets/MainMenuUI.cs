using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playBT,crediteBT;
    [SerializeField] private GameMeager gameMeager;

    private void Awake()
    {
        playBT.onClick.AddListener(() => { gameObject.SetActive(false); gameMeager.gameObject.SetActive(true); });
        crediteBT.onClick.AddListener(() => { Application.OpenURL("https://youtu.be/E4WlUXrJgy4?si=F-2AUNxsOzqMSPqg"); });
    }
}
