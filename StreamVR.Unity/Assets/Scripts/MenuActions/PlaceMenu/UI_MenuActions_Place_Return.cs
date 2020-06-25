using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuActions_Place_Return : MonoBehaviour
{
    public GameObject placeMenu;
    public GameObject generalMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Execute()
    {
        Debug.Log($"RETURNING");
        placeMenu.SetActive(false);
        generalMenu.SetActive(true);
    }
}
