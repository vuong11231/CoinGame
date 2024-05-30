using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPull : Singleton<PointPull>
{
	public PlanetController current;


	public static void SetActive(PlanetController planet)
	{
		Instance.current = planet;
		Instance.gameObject.SetActive(Instance.current != null);
	}
	private void OnMouseDrag()
	{
		if (current == null)
			return;
		current.UpdatePath();
	}
}
