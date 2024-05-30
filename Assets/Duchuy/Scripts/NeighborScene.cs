using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeighborScene : MonoBehaviour
{
    public GameObject neighbor;
    public GameObject friend;
    public GameObject rank;

    public GameObject btn_RefreshFriend;
    public GameObject btn_RefreshNeighbor;

    public GameObject btn_SearchFriend;
    public GameObject btn_SearchNeighbor;

    public FriendSystem friendSystem;
    public NeighborSystem neighborSystem;

    public Sprite highlight;
    public Sprite normal;

    public Image btnNeighbor;
    public Image btnFriend;
    public Image btnRank;

    [HideInInspector]
    public string currentShow = "neighbor";

    private void Start()
    {
        ShowNeighbor();
    }

    public void ShowNeighbor()
    {
        neighbor.SetActive(true);
        friend.SetActive(false);
        rank.SetActive(false);
        btn_RefreshFriend.SetActive(false);
        btn_SearchFriend.SetActive(false);
        btn_RefreshNeighbor.SetActive(true);
        btn_SearchNeighbor.SetActive(true);

        btnNeighbor.sprite = highlight;
        btnFriend.sprite = normal;
        btnRank.sprite = normal;

        currentShow = "neighbor";
        neighborSystem.Refresh();
    }

    public void ShowFriend()
    {
        neighbor.SetActive(false);
        friend.SetActive(true);
        rank.SetActive(false);
        btn_RefreshFriend.SetActive(true);
        btn_SearchFriend.SetActive(true);
        btn_RefreshNeighbor.SetActive(false);
        btn_SearchNeighbor.SetActive(false);

        btnNeighbor.sprite = normal;
        btnFriend.sprite = highlight;
        btnRank.sprite = normal;

        friendSystem.Refresh();
        currentShow = "friend";
        friendSystem.Refresh();
    }

    public void ShowRank()
    {
        neighbor.SetActive(false);
        friend.SetActive(false);
        rank.SetActive(true);

        btnNeighbor.sprite = normal;
        btnFriend.sprite = normal;
        btnRank.sprite = highlight;

        currentShow = "rank";
    }
}
