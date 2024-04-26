using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class RedirectScene : MonoBehaviour
{
    public GlobalStateSO spawner;
    public TextMeshProUGUI text;
    private void Awake()
    {
        if (NetworkManager.Singleton.LocalClientId == 0)
            text.text = "Waiting for Players...";
        else
            SceneManager.LoadSceneAsync("Player " + NetworkManager.Singleton.LocalClientId);
    }

    public void Play()
    {
        //   spawner.EnterState(new ComponentMetaData());
        //   gameObject.SetActive(false);
        SceneManager.LoadSceneAsync("Host");
    }
}