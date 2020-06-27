using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuActions_Place_Lighting : MonoBehaviour
{
    public GameObject physicPointer;
    public GameObject cataloguePointer;
    public GameObject lightingMenu;

    public void Execute()
    {
        Debug.Log("CLICKED PLACE LIGHTING");
        physicPointer.SetActive(false);
        cataloguePointer.SetActive(true);
        lightingMenu.SetActive(true);
    }
}
