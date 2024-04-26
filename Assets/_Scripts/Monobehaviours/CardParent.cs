using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Extensions;
using Unity.Linq;
using System.Linq;

public class CardParent : MonoBehaviour
{
    public CardSO card;
    public CardsListSO owner;
    public Transform graphic, target;
    public int outsideLayer, insideLayer;
    public float tiltAnimSpeed = 20;
    public float selectedCardScale = 1.5f;
    [HideInInspector] public bool canTilt;
    [HideInInspector] public bool OnHost;
    [HideInInspector] public bool DiscardDeck;
    private GameObject world;
    private Animator flat;

    public delegate void Interacted(Vector3 currPos, CardParent cardParent);
    public event Interacted OnInteracted;

    private Camera cam;
    private Vector3 mousePos, dragPoint;
    private float orderinDeck;

    public static float DistanceBetweenTheCardsX = 0.25f;
    public static float DistanceBetweenTheCardsZ = 0.02f;
    public float selectedCardDistanceZ = 1;
    public float dragSensitivity = 3;

    public void RevealCard()
    {
        canTilt = true;
        flat.SetTrigger("active");
        world.SetActive(true);

        SetScale(Vector3.one * selectedCardScale);
        graphic.gameObject.layer = insideLayer;
        flat.gameObject.layer = insideLayer;
    }
    public void HideCard()
    {
        canTilt = false;
        flat.SetTrigger("inActive");
        this.DelayedAction(()=> world.SetActive(false), 0.3f);

        SetScale(Vector3.one);
        this.DelayedAction(() =>
        {
            graphic.gameObject.layer = outsideLayer;
            flat.gameObject.layer = outsideLayer;
        }, 0.3f);
    }

    private void Start()
    {
        cam = Camera.main;
        graphic = transform.GetChild(0);
        target = transform.GetChild(1);
        flat = graphic.GetChild(0).gameObject.GetComponent<Animator>();
        world = graphic.GetChild(1).gameObject;
    }

    private void Update()
    {
        graphic.position = Vector3.Lerp(graphic.position, target.position, Time.deltaTime * 5);
        graphic.rotation = canTilt? Quaternion.LookRotation(Vector3.Lerp(graphic.forward,  graphic.position - target.position + Vector3.forward, Time.deltaTime * tiltAnimSpeed)) : Quaternion.Lerp(graphic.rotation, target.rotation, Time.deltaTime * 50);
        graphic.localScale = Vector3.Lerp(graphic.localScale, target.localScale, Time.deltaTime * 10);
    }

    public void SetPosition(Vector3 position, bool snap = default)
    {
        target.position = position;
        if (snap)
            graphic.position = position;
    }
    public void SetRotation(Vector3 rotation, bool snap = default)
    {
        target.eulerAngles = rotation;
        if (snap)
            graphic.eulerAngles = rotation;
    }
    public void SetScale(Vector3 scale, bool snap = default)
    {
        target.localScale = scale;
        if (snap)
            graphic.localScale = scale;
    }

    public int GetPosX_int()
    {
        return (int)(target.position.x);
    }

    public void MouseDown()
    {
        if (OnHost)
        {
            if (DiscardDeck)
            {
                orderinDeck = target.position.z;
                mousePos = Input.mousePosition - cam.WorldToScreenPoint(new Vector3(target.position.x, target.position.y, -selectedCardDistanceZ));
                RevealCard();
            }
            else
            {
                canTilt = true;
            }
        }
        else
        {
            mousePos = Input.mousePosition - cam.WorldToScreenPoint(new Vector3(target.position.x, target.position.y, -selectedCardDistanceZ));
            RevealCard();
        }
    }
    public void MouseUp()
    {
        if (OnHost)
        {
            if (DiscardDeck)
            {
                HideCard();
                SetPosition(new Vector3(dragPoint.x, dragPoint.y, orderinDeck));
                //SetRotation(new Vector3(target.eulerAngles.x, target.eulerAngles.y, graphic.eulerAngles.z));
            }
            else
            {
                canTilt = false;

                V_Player_Host[] players = FindObjectsOfType<V_Player_Host>();
                players.ForEach((x) =>
                {
                    if (Vector3.Distance(target.position, x.transform.position) < 1.2f)
                    {
                        x.DrawCard();
                        return;
                    }
                });

            }
        }
        else
        {
            HideCard();
            OnInteracted.Invoke(cam.ScreenToViewportPoint(Input.mousePosition), this);
            ArrangeCardsInHand(false);
        }
    }
    public void MouseDrag()
    {
        if (OnHost)
        {
            if (DiscardDeck)
            {
                DragCard();
            }
            else if(transform.GetSiblingIndex() == 0)
            {
                DragCard();
            }
        }
        else
        {
            DragCard();
            ArrangeCardsInHand(true);
        }
    }

    private void DragCard()
    {
        dragPoint = cam.ScreenToWorldPoint(Input.mousePosition - mousePos);
        SetPosition(new Vector3(dragPoint.x, dragPoint.y, -selectedCardDistanceZ));
    }

    private void ArrangeCardsInHand(bool spread)
    {
        if (transform.parent == null)
            return;

        List<CardParent> cardParents = transform.parent.GetComponentsInChildren<CardParent>().ToList();
        cardParents.ForEach((x, i) =>
        {
            if (x != this || !spread)
                x.SetPosition(new Vector3(transform.position.x + (spread ? Mathf.Clamp(x.target.position.x - target.position.x, -1, 1) : 0) + ((i - (cardParents.Count / 2f)) * DistanceBetweenTheCardsX), transform.position.y, transform.position.z - (i * DistanceBetweenTheCardsZ)));
        });
    }
    public static void ArrangeCardsInHand(Transform cardList)
    {
        List<CardParent> cardParents = cardList.GetComponentsInChildren<CardParent>().ToList();
        cardParents.ForEach((x, i) =>
        {
            x.SetPosition(new Vector3(x.transform.position.x + ((i - (cardParents.Count / 2f)) * DistanceBetweenTheCardsX), x.transform.position.y, x.transform.position.z - (i * DistanceBetweenTheCardsZ)));
        });
    }
}