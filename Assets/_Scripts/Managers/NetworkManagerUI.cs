using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Game.Extensions;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private TextMeshProUGUI connTypeTxt;

    private void Awake()
    {
        if (!IsHost)
            SceneManager.LoadSceneAsync("Player " + NetworkManager.Singleton.LocalClientId);

        connTypeTxt.gameObject.Off();
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            buttonsPanel.Off();
            connTypeTxt.gameObject.On();
            connTypeTxt.text = "Connected as server...";
        }
        );

        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            buttonsPanel.Off();
            connTypeTxt.gameObject.On();
            connTypeTxt.text = "Connected as host...";
        }
        );

        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            buttonsPanel.Off();
            connTypeTxt.gameObject.On();
            connTypeTxt.text = "Connected as client...";

        }
        );
    }
}
