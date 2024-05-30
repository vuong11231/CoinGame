using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServerConstants
{
    public static string BASE_URL = "https://universe-master-test2.herokuapp.com/";
    //public static string BASE_URL = "http://localhost:8080";

    public static string GET_USER_DATA = BASE_URL + "/get_data";
    public static string SAVE_USER_DATA = BASE_URL + "/save_data";
    public static string REGISTER_NEW_USER = BASE_URL + "/register_new_user";
    public static string GET_SERVER_TIME = BASE_URL + "/get_server_time";
    public static string UPDATE_ONLINE_TIME = BASE_URL + "/update_online_time";

    public static string FIND_ONE_ENEMY = BASE_URL + "/enemy/find_one";
    public static string FIND_EIGHT_ENEMY = BASE_URL + "/enemy/find_eight";
    public static string FIND_ENEMY_IN_LEVEL_RANGE = BASE_URL + "/enemy/find";

    public static string GET_TOP_RANK = BASE_URL + "/get_top_rank";

    public static string ADD_FRIEND = BASE_URL + "/friend/add";
    public static string DELETE_FRIEND = BASE_URL + "/friend/delete";
    public static string GET_FRIEND_LIST = BASE_URL + "/friend/getfriendlist";
    public static string GET_REVENGE_ENEMY_DATA = BASE_URL + "/get_data_enemy";

    public static string QUERY_FOR_DATA = BASE_URL + "/postData";

    public static string SAVE_RANK_DATA = BASE_URL + "/saveLocalData";
    public static string GET_RANK_DATA = BASE_URL + "/getLocalData";
    public static string INSERT_TO_NEW_RANK_CHART = BASE_URL + "/insertToNewChart2";
    public static string GET_CURRENT_CHART_ID = BASE_URL + "/getCurrentChartId";
    public static string GET_CHART_DATA = BASE_URL + "/getChartData2";
    public static string RESET_RANK_POINT = BASE_URL + "/resetRankPoint";

    public static string WIN_RANK = BASE_URL + "/winRank";
    public static string LOSE_RANK = BASE_URL + "/loseRank";

    public static string GET_USER_DATA_JSON = BASE_URL + "/json/get_data";
    public static string SAVE_USER_DATA_JSON = BASE_URL + "/json/save_data";

    public static string GET_VERSION = BASE_URL + "/getVersion";
    public static string GET_MAIL = BASE_URL + "/getMail";

    public static string GET_UNIVERSE_DATA = BASE_URL + "/universe/get_data";
    public static string GET_UNIVERSE_DATA_BY_FACEBOOK_ID = BASE_URL + "/universe/getDataByFacebookId";
    public static string GET_UNIVERSE_DATA_BY_APPLE_ID = BASE_URL + "/universe/getDataByAppleId";
    public static string SAVE_UNIVERSE_DATA = BASE_URL + "/universe/save_data";

    public static string GET_REMOTE_CONFIG = BASE_URL + "/getRemoteConfig";
    public static string UPDATE_IS_ATTACKED_CODE = BASE_URL + "/updateAttackedCode2";
    public static string GET_IS_ATTACKED_CODE = BASE_URL + "/getAttackedCode2";
    //public static string GET_IS_ATTACKED_CODE = BASE_URL + "/getAttackedCode";
    //public static string UPDATE_IS_ATTACKED_CODE = BASE_URL + "/updateAttackedCode";

    public static string SET_STATUS = BASE_URL + "/universe/setStatus";
    public static string SAVE_META_DATA = BASE_URL + "/universe/saveMetaData";
}
