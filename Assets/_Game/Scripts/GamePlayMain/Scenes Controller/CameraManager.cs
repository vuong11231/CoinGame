using SteveRogers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CameraManager : Singleton<CameraManager>
{
    public int MAX_CAMERA_SCROLL_VALUE = 27;
    public Camera MainCamera;
    public Camera CameraTouch;
    public float Ratio = -1;
    public int RatioZoom = 3;
    public GameObject CameraFollow;
    public Vector2 velocity;
    
    protected int _AmountOrbit = 0;

    bool IsDrag = false;
    bool _IsClick;
    bool _IsTwoClick;
    Vector3 currentDown;
    Vector3 mousePosition;

#if !UNITY_EDITOR
    float touchesPrevPosDifference, touchesCurPosDifference, zoomModifier;
    bool _IsTSwap;
    float fingerDistance = 0;
    Vector2 firstTouchPrevPos, secondTouchPrevPos;
#endif

    private float MaxOrtho
    {
        get
        {
            var def = 50 + RatioZoom * _AmountOrbit;
            float r;

            if (Scenes.IsBattleScene())
            {
                r = Mathf.Max(SpaceManager.Instance.LastPlanetRadius, SpaceEnemyManager.Instance.LastPlanetRadius);
                return Mathf.Max(def, r * 3 + 1, 30);
            }
            else // gameplay scene
            {
                r = SpaceManager.Instance.LastPlanetRadius;
                var w = Screen.width;
                var h = Screen.height;

                return r * h / w + DataScriptableObject.Instance.offsetEmptySpaceGameplay;
            }
        }
    }

    public virtual void Start()
    {
        if (Scenes.Current == SceneName.Gameplay)
        {
            RatioZoom = 3;
        }
        else
        {
            RatioZoom = 6;
        }
    }

    public virtual void Update()
    {
        if (GameStatics.IsAnimating)
            return;

        if (Utilities.ActiveScene.name.Equals("GamePlay") && UIMultiScreenCanvasMan.modeExplore != UIMultiScreenCanvasMan.Mode.Gameplay)
            return;
        
        if (!TutMan.IsDone(TutMan.TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME))
            return;

#if UNITY_EDITOR        
        if (Input.GetKey(KeyCode.T) || Input.GetAxis("Mouse ScrollWheel") > 0f) // Thu nhỏ
        {
            if (MainCamera.orthographicSize > 10)
            {
                //if (!TutMan.IsDone(TutMan.ZoomGuide_TutKey))
                //{
                //    return;
                //}

                CameraTouch.orthographicSize = MainCamera.orthographicSize += Ratio;

                float Max = (MAX_CAMERA_SCROLL_VALUE + 3 * (_AmountOrbit - 3) - MainCamera.orthographicSize);
                if (Max < 0) Max = 0;

                Vector3 result2 = new Vector3(Mathf.Clamp(transform.position.x, -Max, Max), Mathf.Clamp(transform.position.y, -Max, Max), 51);

                transform.position = result2;
                CameraFollow.transform.position = transform.position + Vector3.back * 101;
            }
        }

        else if (Input.GetKey(KeyCode.Y) || Input.GetAxis("Mouse ScrollWheel") < 0f) // Phóng to
        {
            if (ZoomTut())
                return;

            if (MainCamera.orthographicSize < MaxOrtho)
            {
                CameraTouch.orthographicSize = MainCamera.orthographicSize -= Ratio;

                float Max = (MAX_CAMERA_SCROLL_VALUE + 3 * (_AmountOrbit - 3) - MainCamera.orthographicSize);
                if (Max < 0) Max = 0;

                Vector3 result2 = new Vector3(Mathf.Clamp(transform.position.x, -Max, Max), Mathf.Clamp(transform.position.y, -Max, Max), 51);


                transform.position = result2;
                CameraFollow.transform.position = transform.position + Vector3.back * 101;
            }
        }
#elif UNITY_IOS || UNITY_ANDROID
      
        if (Input.touchCount == 2)
        {
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
            secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

            touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
            touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

            //zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed;
            zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude;

            if (touchesPrevPosDifference > touchesCurPosDifference) // Phóng to
            {
                if (MainCamera.orthographicSize < MaxOrtho)
                { 
                    if (ZoomTut())
                        return;

                    CameraTouch.orthographicSize = MainCamera.orthographicSize -= Ratio;
                    float Max = (MAX_CAMERA_SCROLL_VALUE + 3 * (_AmountOrbit - 3) - MainCamera.orthographicSize);
                    if (Max < 0) Max = 0;

                    Vector3 result2 = new Vector3(Mathf.Clamp(transform.position.x, -Max, Max), Mathf.Clamp(transform.position.y, -Max, Max), 51);


                    transform.position = result2;
                    CameraFollow.transform.position = transform.position + Vector3.back * 101;
                }
            }
            else if (touchesPrevPosDifference < touchesCurPosDifference) // Thu nhỏ
            {
                //if (!TutMan.IsDone(TutMan.ZoomGuide_TutKey))
                //{
                //    return;
                //}

                CameraTouch.orthographicSize = MainCamera.orthographicSize += Ratio;

                float Max = (MAX_CAMERA_SCROLL_VALUE + 3 * (_AmountOrbit - 3) - MainCamera.orthographicSize);
                if (Max < 0) Max = 0;

                Vector3 result2 = new Vector3(Mathf.Clamp(transform.position.x, -Max, Max), Mathf.Clamp(transform.position.y, -Max, Max), 51);


                transform.position = result2;
                CameraFollow.transform.position = transform.position + Vector3.back * 101;
            }            
        }
#endif
    }

    public virtual void SetSize(int AmountOrbit)
    {
        _AmountOrbit = AmountOrbit;
        SetSize();
    }

    public void SetSize()
    {
        CameraTouch.orthographicSize = MainCamera.orthographicSize = MaxOrtho;
    }

    private Vector3 startPos;
    private Vector3 oldPos;

    public void SetIsClick(bool isActive)
    {
        _IsClick = isActive;
    }

    void OnMouseDown()
    {
        if (Input.touchCount >= 2 || GameStatics.IsAnimating)
        {
            _IsTwoClick = true;
            return;
        }
        else
        {
            _IsTwoClick = false;
        }
        if (Popups.IsShowed)
            return;

        _IsClick = true;
        LeanTween.cancel(gameObject);

        currentDown = Input.mousePosition;
        currentDown = Camera.main.ScreenToWorldPoint(currentDown);
        startPos = CameraTouch.ScreenToWorldPoint(Input.mousePosition);
        oldPos = transform.position;
    }

    void OnMouseDrag()
    {
        if (Input.touchCount >= 2 || (_IsTwoClick && Input.touchCount == 1) || GameStatics.IsAnimating)
            return;

        if (!_IsClick) return;

        if (Popups.IsShowed) return;

        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (!IsDrag)
        {

            float _Distance = Vector3.Distance(mousePosition, currentDown);
            if (_Distance < .1f)
            {
                return;
            }
        }

        IsDrag = true;

        float Max = (MAX_CAMERA_SCROLL_VALUE + 3 * (_AmountOrbit - 3) - MainCamera.orthographicSize);
        if (Scenes.Current == SceneName.Battle) Max *= 2f;

        if (Max < 0)
            Max = 0;

        Vector3 result = oldPos + (startPos - CameraTouch.ScreenToWorldPoint(Input.mousePosition));
        Vector3 result2 = new Vector3(Mathf.Clamp(result.x, -Max, Max), Mathf.Clamp(result.y, -Max, Max), 51);

        transform.position = result2;
        CameraFollow.transform.position = transform.position + Vector3.back * 101;
    }

    private void OnMouseUp()
    {
        if (Input.touchCount == 0)
            _IsTwoClick = false;

        if (!_IsClick)
            return;
        else
            _IsClick = false;

        if (Popups.IsShowed || GameStatics.IsAnimating)
            return;

        if (Scenes.Current == SceneName.Battle && !IsDrag)
        {
            HomeBottomUI.Instance.ShootBtn.GetComponent<Button>().interactable = false;
            var count = SpaceManager.Instance.ListSpace.Count;

            for (int i = 0; i < count; i++)
            {
                if (SpaceManager.Instance.ListSpace[i].Planet.IEShoot != null && !SpaceManager.Instance.ListSpace[i].Planet.isFlying)
                {
                    SpaceManager.Instance.FxBack.SetActive(false);
                    if (ShootPath.Instance)
                    {
                        ShootPath.Instance.SetActivePath(false);
                    }
                    PointPull.SetActive(null);
                    SpaceManager.Instance.ListSpace[i].Planet.SpinAround();
                    SpaceManager.Instance.ListSpace[i].Planet.IEShoot = null;
                }
            }
        }

        IsDrag = false;
    }

    private bool ZoomTut()
    {
        return false;

        //if (!TutMan.IsDone(TutMan.ZoomGuide_TutKey))
        //{
        //    TutMan.SetTutDone(TutMan.ZoomGuide_TutKey);
            
        //    if (LeanTween.isTweening(gameObject))
        //        return true;

        //    Utilities.ActiveEventSystem = false;
        //    LeanTween.value(gameObject, CameraTouch.orthographicSize, MaxOrtho, 1)
        //        .setOnUpdate((float f) =>
        //        {
        //            CameraTouch.orthographicSize = MainCamera.orthographicSize = f;
        //        })
        //        .setOnComplete(TutGameplayScene.OnZoomDone);
            
        //    return true;
        //}

        //return false;
    }
}