using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuAction_Paint : MonoBehaviour
{
    public GameObject paintMenu;
    public GameObject generalMenu;

    public void Execute()
    {
        paintMenu.SetActive(true);
        generalMenu.SetActive(false);


        GameObject righthand = GameObject.Find("Right Hand");

        var pointer = righthand.GetComponentInChildren<PhysicsPointer>(true)?.gameObject;
        var paintGun = righthand.GetComponentInChildren<ShootingPaintScript>(true)?.gameObject;

        pointer.SetActive(false);
        paintGun.SetActive(true);
    }
}
