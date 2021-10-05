using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float startMoveSpeed;
    public float dragSpeedMultiplier;
    public float rotateSpeed;
    public float detonationTime;
    public Explosion grenadeExplosion;
    public AudioClip grenadeSound;

    bool played;
    AudioSource source;
    float currentDetonationTime;

    void Start()
    {
        source = GetComponent<AudioSource>();

        if (grenadeSound != null)
            source.clip = grenadeSound;
    }

    void Update()
    {
        if (!played && grenadeSound != null)
        {
            source.Play();
            played = true;
        }

        if (startMoveSpeed > 0.01f)
            startMoveSpeed = Mathf.Lerp(startMoveSpeed, 0f, dragSpeedMultiplier * Time.deltaTime);

        transform.Translate(Vector3.right * Time.deltaTime * startMoveSpeed);

        if (currentDetonationTime >= detonationTime)
        {
            Instantiate(grenadeExplosion.gameObject, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

        else
            currentDetonationTime += Time.deltaTime;
    }
}
