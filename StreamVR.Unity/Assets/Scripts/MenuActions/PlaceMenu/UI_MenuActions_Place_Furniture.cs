using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuActions_Place_Furniture : MonoBehaviour
{
    public GameObject physicPointer;
    public GameObject cataloguePointer;
    public GameObject furnatureMenu;

    public void Execute()
    {
        physicPointer.SetActive(false);
        cataloguePointer.SetActive(true);
        furnatureMenu.SetActive(true);
    }
}
