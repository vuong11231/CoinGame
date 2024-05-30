using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SteveRogers;

public class NeighborSystem : MonoBehaviour
{
    public List<EnemyDetailBox> listBox;
    public DRotate refreshAnim;

    bool isRefreshing = false;

    public static string firstRowUserID = null;

    private void Start()
    {
        refreshAnim.enabled = false;
        LoadData(DataGameSave.listDataEnemies);
        
    }

    public void LoadData(List<DataGameServer> neighbors)
    {
        for (int i = 0; i < listBox.Count; i++)
        {
            if (i < neighbors.Count)
            {
                listBox[i].gameObject.SetActive(true);
                listBox[i].LoadData(neighbors[i],i);

                if (i == 0)
                {
                    firstRowUserID = neighbors[i].userid.ToString();
                }
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
        form.AddField("level", DataGameSave.dataServer.level);

        ServerSystem.Instance.SendRequest(ServerConstants.FIND_EIGHT_ENEMY, form, () =>
        {
            if (!ServerSystem.Instance.IsResponseOK())
            {
                PopupConfirm.ShowOK("Error", "Error when loading data", "Try Again", () => Refresh());
            }
            else
            {
                string data = ServerSystem.Instance.ReadData();
                List<DataGameServer> list = JsonConvert.DeserializeObject<List<DataGameServer>>(data);
                DataGameSave.listDataEnemies = list;

                LoadData(DataGameSave.listDataEnemies);

                isRefreshing = false;
                refreshAnim.enabled = false;
            }
        }, ()=> {
            isRefreshing = false;
            refreshAnim.enabled = false;
        });
    }

    public void Search() {
        PopupSearch.Show(()=> {
            //Debug.Log("SEARCH FINISHED");
            //Debug.Log(PopupSearch.id);

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

                        Debug.Log("SEARCH FOUND!!!!");
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
