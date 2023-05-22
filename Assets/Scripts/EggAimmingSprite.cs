using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggAimmingSprite : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float duration = 2f;

    private float startTime;
    private Vector3 initialLocalPosition;

    private void Start()
    {
        startTime = Time.time;
        initialLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        float timePassed = Time.time - startTime;
        float t = Mathf.PingPong(timePassed / duration, 1f);

        Vector3 targetLocalPosition = Vector3.Lerp(pointA.localPosition, pointB.localPosition, t);

        transform.localPosition = initialLocalPosition + targetLocalPosition;
    }
}

