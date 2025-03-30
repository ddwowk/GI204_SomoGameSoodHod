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
        crediteBT.onClick.AddListener(() => { Application.OpenURL("https://ddwowk.github.io/GI204Credits/GI204.html?fbclid=IwZXh0bgNhZW0CMTEAAR1NYWmsxmb1yDsurbsytsBrr3tz9axXZthghmkVlperPxNkzP-bdxftYuA_aem_ZT0TPiYcVtURXZN20fzOLw"); });
    }
}
