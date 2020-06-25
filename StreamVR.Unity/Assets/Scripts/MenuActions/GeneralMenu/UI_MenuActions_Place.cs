using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuActions_Place : MonoBehaviour
{
    public GameObject placeMenu;
    public GameObject generalMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Execute()
    {
        Debug.Log($"PLACING");
        placeMenu.SetActive(true);
        generalMenu.SetActive(false);
    }
}
