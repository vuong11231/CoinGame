using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendSystem : MonoBehaviour
{
    public static FriendSystem Instance;

    public List<EnemyDetailBox> listBox;
    public DRotate refreshAnim;

    bool isRefreshing = false;

    private void Start()
    {
        Instance = this;
        refreshAnim.enabled = false;
        LoadData(DataGameSave.listDataFriends);
    }

    public void LoadData(List<DataGameServer> friends)
    {
        for (int i = 0; i < listBox.Count; i++)
        {
            if (i < friends.Count)
            {
                listBox[i].gameObject.SetActive(true);
                listBox[i].LoadData(friends[i],i);
            }
            else
            {
                listBox[i].gameObject.SetActive(false);
            }
        }
    }

    public void Refresh()
    {
        if (isRefreshing)
            return;

        isRefreshing = true;
        refreshAnim.enabled = true;

        WWWForm form = new WWWForm();
        form.AddField("userid", DataGameSave.dataLogin.userid);
        form.AddField("token", DataGameSave.dataLogin.token.ToString());

        ServerSystem.Instance.SendRequest(ServerConstants.GET_FRIEND_LIST, form, () =>
        {
            if (!ServerSystem.Instance.IsResponseOK())
            {
                PopupConfirm.ShowOK("Error", "Error when loading data", "Try Again", () => Refresh());
            }
            else
            {
                string data = ServerSystem.Instance.ReadData();
                List<DataGameServer> list = JsonConvert.DeserializeObject<List<DataGameServer>>(data);
                DataGameSave.listDataFriends = list;

                LoadData(DataGameSave.listDataFriends);

                isRefreshing = false;
                refreshAnim.enabled = false;
            }
        });
    }

    public void Search() {
        PopupSearch.Show(() => {
           
            if (PopupSearch.id == 0 || PopupSearch.id == DataGameSave.dataServer.userid) {
                PopupConfirm.ShowOK("Oops", TextMan.Get("Search not found"), "OK");
                return;
            }

            string url = ServerConstants.GET_REVENGE_ENEMY_DATA;

            WWWForm form = new WWWForm();
            form.AddField("userid", Convert.ToInt32(DataGameSave.dataLogin.userid.ToString()));
            form.AddField("token", DataGameSave.dataLogin.token.ToString());
            form.AddField("enemyid", PopupSearch.id.ToString());

            ServerSystem.Instance.SendRequest(url, form, () => {
                if (!ServerSystem.Instance.IsResponseOK()) {
                    PopupConfirm.ShowOK("Oops", TextMan.Get("Search not found"), "OK");
                } else {
                    try {
                        string jsonData = ServerSystem.Instance.ReadData();

                        DataGameServer dataGameServer = JsonConvert.DeserializeObject<DataGameServer>(jsonData,
                            new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

                        if (dataGameServer.ListEnemy == null) {
                            dataGameServer.ListEnemy = new List<string>();
                        }

                        if (dataGameServer.ListPlanet == null) {
                            dataGameServer.ListPlanet = new List<DataPlanet>();
                        }

                        if (dataGameServer.ListPlanet.Count == 0) {
                            dataGameServer.ListPlanet.Add(new DataPlanet());
                        }

                        List<DataGameServer> list = new List<DataGameServer>();
                        list.Add(dataGameServer);
                        LoadData(list);

                    } catch {
                        PopupConfirm.ShowOK("Oops", TextMan.Get("Search not found"), "OK");
                    }
                }
            });
        });
    }
}
