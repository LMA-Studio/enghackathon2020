using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPaintScript : MonoBehaviour
{
    public float speed = 40;
    public GameObject paintBlob;
    public Transform barrel;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public void Fire()
    {
        GameObject spawnedPaint = Instantiate(paintBlob, barrel.position, barrel.rotation);
        spawnedPaint.GetComponent<Rigidbody>().velocity = speed * barrel.forward;
        audioSource.PlayOneShot(audioClip);
        Destroy(spawnedPaint, 2);
    }

}
