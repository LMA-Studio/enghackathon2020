using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuActions_Place : MonoBehaviour
{
    public GameObject placeMenu;
    public GameObject generalMenu;

    public void Execute()
    {
        placeMenu.SetActive(true);
        generalMenu.SetActive(false);
    }
}
