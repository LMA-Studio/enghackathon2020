using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintWandEndController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "paint")
        {
            Debug.Log("Attaching Paint");

            Material paintMaterial = collision.gameObject.GetComponent<MeshRenderer>().material;

            GameObject parent = this.transform.parent.gameObject;
            ShootingPaintScript paintShooter = parent.GetComponent<ShootingPaintScript>();

            paintShooter.ChangePaint(paintMaterial);
            this.GetComponent<MeshRenderer>().material = paintMaterial;
        }
    }
}
