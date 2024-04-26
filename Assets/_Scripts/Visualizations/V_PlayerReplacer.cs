using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class V_PlayerReplacer : MonoBehaviour
{
    private Camera cam;
    private CardParent selectedCard;
    private V_Player_Host selectedPlayer;
    private float posZ;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        InputCardSelection();
    }

    private void InputCardSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.parent.GetComponent<CardParent>())
                {
                    selectedCard = hit.transform.parent.GetComponent<CardParent>();
                    posZ = selectedCard.target.position.z;
                    selectedCard.RevealCard();
                }
                if (hit.transform.GetComponent<V_Player_Host>())
                {
                    selectedPlayer = hit.transform.GetComponent<V_Player_Host>();
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (selectedCard)
            {
                Vector2 pos = V_DiscardDeck.Instance.transform.position;
                selectedCard.SetPosition(new Vector3(Mathf.Clamp(cam.ScreenToWorldPoint(Input.mousePosition).x, pos.x - 0.3f, pos.x + 0.3f), Mathf.Clamp(cam.ScreenToWorldPoint(Input.mousePosition).y, pos.y - 0.3f, pos.y + 0.3f), -5));
            }
            if (selectedPlayer)
            {
                selectedPlayer.transform.position = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y, selectedPlayer.transform.position.z);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (selectedCard)
            {
                selectedCard.SetPosition(new Vector3(selectedCard.target.position.x, selectedCard.target.position.y, posZ));
                selectedCard.HideCard();
                selectedCard = null;
            }
            if (selectedPlayer)
            {
                List<V_Player_Host> players = FindObjectsOfType<V_Player_Host>().ToList();
                players.Remove(selectedPlayer);
                for (int i = 0; i < players.Count; i++)
                {
                    if (Vector3.Distance(selectedPlayer.transform.position, players[i].transform.position) < 1)
                    {
                        Transform newTarget = players[i].target;
                        players[i].target = selectedPlayer.target;
                        selectedPlayer.target = newTarget;
                        players[i].transform.DOMove(players[i].target.position, 0.2f).SetEase(Ease.Linear);
                        selectedPlayer.transform.DOMove(selectedPlayer.target.position, 0.2f).SetEase(Ease.Linear);
                        players[i].transform.DORotate(players[i].target.eulerAngles, 0.2f).SetEase(Ease.Linear);
                        selectedPlayer.transform.DORotate(selectedPlayer.target.eulerAngles, 0.2f).SetEase(Ease.Linear);
                        selectedPlayer = null;

                        return;
                    }
                }
                selectedPlayer.transform.DOMove(selectedPlayer.target.position, 0.2f).SetEase(Ease.Linear);
                selectedPlayer = null;
            }
        }
    }
}
