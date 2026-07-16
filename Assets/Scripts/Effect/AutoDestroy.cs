using UnityEngine;

public class AutoDestory : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
