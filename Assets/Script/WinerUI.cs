using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinerUI : MonoBehaviour
{
    [SerializeField] Button mainBT,rePlayBP;
    [SerializeField] TextMeshProUGUI[] otherName;
    [SerializeField] TextMeshProUGUI winnerPlayer;
    [SerializeField] Canvas errorImage;

    public void updateData(List<string> otherPlayerID)
    {
        for(int i = 0; i < otherName.Length; i++)
        {
            if (i < otherPlayerID.Count && otherPlayerID.Count != 0)
            {
                otherName[i].text = GameMeager.Instance.playroomKit.GetPlayer(otherPlayerID[i]).GetProfile().name;
            }
            else
            {
                otherName[i].text = "";
            }
        }
        winnerPlayer.text = GameMeager.Instance.playroomKit.GetState<string>("Winner");
    }
    public void onBT()
    {
        errorImage.gameObject.SetActive(true);
        StartCoroutine(GameMeager.Instance.WaitFor(1,() => { Application.Quit(); }));
    }
    public void MainMenuBT()
    {
        SceneManager.LoadScene("MainDemo");
    }
}
