using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardDetailController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [Header("Enhanced scroller")]
    public EnhancedScroller myScroller;
    [SerializeField]
    private LeaderboardDetailViewRowController leaderboardDetailViewRowPrefab;
    [SerializeField]
    private LeaderboardViewRowController vrController;
    
    public PopupLeaderboard mainPopup;

    List<LeaderboardDetailViewRowData> _data;
    int _dataCount;

    private void Start()
    {
        myScroller.Delegate = this;
        _data = new List<LeaderboardDetailViewRowData>();
    }

    public void GetListData()
    {

        //Indicator.Show();
        //_data = new List<LeaderboardDetailViewRowData>();
        //PlayfabManager.GetLeaderboard(vrController.type.ToString(),
        //    0,
        //    50,
        //    (result) =>
        //    {
        //        int count = result.Leaderboard.Count;
        //        for(int i = 0; i < count; i++)
        //        {
        //            _data.Add(new LeaderboardDetailViewRowData()
        //            {
        //                isYou = (result.Leaderboard[i].PlayFabId == PlayfabManager.PlayFabID) ? true : false,
        //                playfabId = result.Leaderboard[i].PlayFabId,
        //                username = string.IsNullOrEmpty(result.Leaderboard[i].DisplayName) ? GameConstants.DEFAULT_USER_NAME : result.Leaderboard[i].DisplayName,
        //                currentValue = result.Leaderboard[i].StatValue,
        //                currentPosition = result.Leaderboard[i].Position,
        //                avatar = string.IsNullOrEmpty(result.Leaderboard[i].Profile.AvatarUrl) || result.Leaderboard[i].Profile.AvatarUrl.Equals(PlayfabManager.NO_URL_STRING) ? string.Empty : result.Leaderboard[i].Profile.AvatarUrl,
        //                type = vrController.type.ToString()
        //            });
        //            if (_data[i].isYou && _data[i].currentValue > 0)
        //            {
        //                mainPopup.UpdateUserInfo(_data[i]);
        //            }
        //        }
        //        count = _data.Count;
        //        for(int i = count - 1; i >= 0; i--)
        //        {
        //            if (_data[i].currentValue < 0)
        //                _data.RemoveAt(i);
        //        }
        //        myScroller.ReloadData();
        //        Indicator.Hide();
        //    },
        //    (error) =>
        //    {
        //        Debug.LogError("GetLeaderboardAndHandle (Global) fail! " + error.GenerateErrorReport());
        //        myScroller.ReloadData();
        //        Indicator.Hide();
        //    }
        //);
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        LeaderboardDetailViewRowController cellView = scroller.GetCellView(leaderboardDetailViewRowPrefab) as LeaderboardDetailViewRowController;

        cellView.SetData(_data[dataIndex], dataIndex, vrController.index);

        return cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 98.7f;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data.Count;
    }
}
