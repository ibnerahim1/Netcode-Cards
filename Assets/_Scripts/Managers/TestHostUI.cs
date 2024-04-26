using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestHostUI : MonoBehaviour
{
    public ActionSO A_Draw, A_Play;
    public Button drawButton, playButton, skipButton;

    public delegate void click();
    public static event click OnDraw;
    public static event click OnPlay;

    private void Awake()
    {
        drawButton.onClick.AddListener(() => OnDraw?.Invoke());
        playButton.onClick.AddListener(() => OnPlay?.Invoke());
    }
}
