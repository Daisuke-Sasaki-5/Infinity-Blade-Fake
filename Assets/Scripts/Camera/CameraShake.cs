using System;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] private float duration = 0.15f;
    [SerializeField] private float magnitude = 0.15f;

    private Vector3 origionPos;

    private void Awake()
    {
        instance = this;
    }

    public void Shake()
    {
        StopAllCoroutines();

        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        origionPos = transform.localPosition;

        float timer = 0f;

        while (timer < duration)
        {
            float x = UnityEngine.Random.Range(-1f,1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f,1f) * magnitude;

            transform.localPosition = origionPos + new Vector3(x, y, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = origionPos;
    }
}
