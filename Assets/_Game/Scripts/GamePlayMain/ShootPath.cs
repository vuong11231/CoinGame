using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPath : Singleton<ShootPath> {
    public int PointCount = 80;
    public GameObject pointGroup;

    [SerializeField] 
    private SpriteRenderer _pointPreb;
    private List<SpriteRenderer> _points;

    private Vector2 _startPoint;
    private Vector2 _endPoint;
    private Vector2 _dir;

    private float _sunRadius;

    private void Start() {

        _points = new List<SpriteRenderer>();
        for (int i = 0; i < PointCount; i++) {
            _points.Add(GameObject.Instantiate(_pointPreb, pointGroup.transform));
        }
    }

    public void SetActivePath(bool isActive) {
        pointGroup.SetActive(isActive);
    }

    public bool GetActivePath() {
        return pointGroup.activeSelf;
    }

    public void DrawPath(Vector2 startPoint, Vector2 endPoint, Vector2 dir, float sunRadius = 11f, float speed = 0.4f, float sunGravity = 0.4f) {
        if (!pointGroup.activeSelf) {
            return;
        }

        int MAX_POINT = 700;

        _startPoint = startPoint;
        _endPoint = endPoint;
        _dir = dir;
        _sunRadius = sunRadius;

        Vector2[] pointPositions = new Vector2[MAX_POINT];
        pointPositions[0] = _startPoint;
        _points[0].transform.position = pointPositions[0];

        Vector2 direction = _dir.normalized;
        
        int index = 1;
        float space = 1f;
        float distance = 0;

        for (int i = 1; i < MAX_POINT; i++) {
            
            bool drawStraightForward = Vector2.Distance(pointPositions[i - 1], _endPoint) >= _sunRadius || Scenes.Current == SceneName.MeteorBelt;

            if (drawStraightForward) {
                pointPositions[i] = pointPositions[i - 1] + direction.normalized * 2f;
            } else {
                pointPositions[i] = MonoHelper.GetCurve(_endPoint, pointPositions[i - 1], direction, Vector2.Distance(pointPositions[i - 1], _endPoint), sunRadius, speed, sunGravity);
                direction = pointPositions[i] - pointPositions[i - 1];
            }

            if (_points.Count > i) {
                _points[i].gameObject.SetActive(false);
            }

            float currentStep = Vector2.Distance(pointPositions[i], _points[index - 1].transform.position);

            if (index < _points.Count && currentStep >= space) {

                bool showPoint = (Scenes.Current == SceneName.MeteorBelt) || distance < Vector2.Distance(startPoint, endPoint);

                if (showPoint) {
                    distance += currentStep;
                    _points[index].transform.position = pointPositions[i];
                    _points[index].gameObject.SetActive(true);
                    index++;
                }
            }
        }
    }
}
