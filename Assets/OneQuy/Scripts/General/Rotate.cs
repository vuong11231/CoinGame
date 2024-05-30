using UnityEngine;

namespace SteveRogers
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField]
        private Vector3 min;

        [SerializeField]
        private Vector3 max;

        private void Start()
        {
            if (max != min)
            {
                min = Vector3.Lerp(min, max, UnityEngine.Random.Range(0f, 1f));
            }
        }

        private void Update()
        {
            transform.Rotate(min * Time.deltaTime);
        }
    }
}