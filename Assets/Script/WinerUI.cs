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
        List<string> sortName = otherPlayerID.Distinct().ToList();
        
        for (int i = 0; i < otherName.Length; i++)
        {
            if (i < sortName.Count && otherPlayerID.Count != 0 && GameMeager.Instance.playroomKit.GetPlayer(sortName[i]).GetProfile().name != GameMeager.Instance.playroomKit.GetState<string>("Winner"))
            {
                otherName[i].text = GameMeager.Instance.playroomKit.GetPlayer(sortName[i]).GetProfile().name;
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
    //public void MainMenuBT()
    //{
    //    SceneManager.LoadScene("MainDemo");
    //}
}
