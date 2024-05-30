using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
public class RewardMaterialFX : MonoBehaviour
{
	[SerializeField] private TextMesh valueTxt;
	[SerializeField] private SpriteRenderer render;
	[SerializeField] private Sprite[] sprites;
	[SerializeField] private float speed;
	[SerializeField] private AnimationCurve curveY;
	public static RewardMaterialFX Prefab;
	public bool isMoving;
	private Vector3 dir;
	public static List<RewardMaterialFX> fxs;

	public float elapsedTime;
	float oldY;

	public static void AddPool(RewardMaterialFX reward)
	{
		if (fxs == null)
		{
			fxs = new List<RewardMaterialFX>();
		}
		if (!fxs.Contains(reward))
		{
			fxs.Add(reward);
		}
	}

	public static void ClearPool()
	{
		if (fxs == null)
		{
			fxs = new List<RewardMaterialFX>();
		}
		fxs.Clear();
	}

	public static void RemovePool(RewardMaterialFX reward)
	{
		if (fxs == null)
		{
			fxs = new List<RewardMaterialFX>();
		}
		if (fxs.Contains(reward))
		{
			fxs.Remove(reward);
		}
	}

	public static void GetAllReward()
	{
		if (fxs == null)
		{
			fxs = new List<RewardMaterialFX>();
		}
		for (int i = 0; i < fxs.Count; i++)
		{
			fxs[i].RewardMove();
			fxs[i].elapsedTime = -i * 0.1f;
		}
	}




	public void Init(int index, int value, Vector3 position)
	{
		render.sprite = sprites[System.Math.Min(index, sprites.Length)];
		valueTxt.text = value.ToString();
		transform.position = position;

		oldY = transform.position.y;
		LeanTween.value(0, 1, 1f).setOnUpdate((float val) =>
		{
			transform.position = new Vector3(transform.position.x, oldY + curveY.Evaluate(val));
		}).setOnComplete(() =>
		{
			RewardMove();
		});
	}
	public void RewardMove()
	{
		isMoving = true;
		dir = SpaceManager.Instance.transform.position - transform.position;
	}

	private void OnDestination()
	{

		LeanPool.Despawn(this);
	}

	private void Update()
	{
		if (isMoving)
		{
			if (elapsedTime < 0)
			{
				elapsedTime += Time.deltaTime;
				return;
			}
			transform.position += dir.normalized * Time.deltaTime * speed;
			if (Vector3.Distance(transform.position, SpaceManager.Instance.transform.position) <= 0.5f)
			{
				OnDestination();
			}
		}
	}

	public static RewardMaterialFX CreateReward()
	{
		if (Prefab == null)
			Prefab = Resources.Load<RewardMaterialFX>("Prefabs/Fx/FxMaterial");

		return Prefab;
	}
}
