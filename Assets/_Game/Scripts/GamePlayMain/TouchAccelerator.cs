using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;
public class TouchAccelerator : MonoBehaviour
{
	[SerializeField] private CircleCollider2D _collider;
	[SerializeField] private Camera _cam;
	private List<PlanetController> planets = new List<PlanetController>();
	private bool isChecking;
	private int currentFinger;
	private PlanetController currentPlanet;
	private IEnumerator IEDelay;
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (Popups.IsShowed) return;
        if (collision.GetComponent<PlanetController>() != null&& collision.tag != GameConstants.ENEMY_TAG
            && gameObject.tag != GameConstants.PLANET_TAG && !collision.GetComponent<PlanetController>().isFlying
            && collision.GetComponent<PlanetController>().Type!= TypePlanet.Destroy)
		{
            //Sound
            Sounds.Instance.PlaySelect();
            planets.Add(collision.GetComponent<PlanetController>());
		}
		if (planets.Count > 0)
		{
			CameraManager.Instance.SetIsClick(false);
		}

	}


	private IEnumerator DelayDisableCollider()
	{
     

        isChecking = true;
		yield return new WaitForSeconds(0.2f);
		_collider.enabled = false;
		isChecking = false;
		if (planets.Count > 0)
		{
			
			float dist = float.MaxValue;
			int currentIndex = 0;
			for (int i = 0; i < planets.Count; i++)
			{
				if (Vector2.Distance(transform.position, planets[i].transform.position) < dist)
				{
					dist = Vector2.Distance(transform.position, planets[i].transform.position);
					currentIndex = i;
				}
			}
			planets[currentIndex].StartDrag();
			currentPlanet = planets[currentIndex];
		}
		if (!Input.GetMouseButton(0) || Input.touchCount == 0)
		{
			currentPlanet = null;
		}
	}

	private IEnumerator DelayEnableCollider()
	{
		yield return new WaitForSeconds(0.1f);
		_collider.enabled = true;
	}
	private void FixedUpdate()
	{
        
		if ((Input.touchCount == 1 || Input.GetMouseButtonDown(0)) && !isChecking)
		{
			Vector3 currentPos = _cam.ScreenToWorldPoint(Input.mousePosition);
			planets.Clear();
			transform.position = new Vector3(currentPos.x, currentPos.y, -5);
			IEDelay = DelayDisableCollider();
			StartCoroutine(IEDelay);
			if (!ShootPath.Instance.pointGroup.activeSelf)
			{
				StartCoroutine(DelayEnableCollider());
			}
		}
		
		if (currentPlanet != null)
		{
			if (!Input.GetMouseButton(0) || Input.touchCount == 0)
			{ 
				currentPlanet = null;
			}
			currentPlanet.UpdatePath();
			
		}

	}
}
