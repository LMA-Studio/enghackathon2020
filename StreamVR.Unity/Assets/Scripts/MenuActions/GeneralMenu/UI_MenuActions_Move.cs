using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuActions_Move : MonoBehaviour
{
    public GameObject physicsPointer;
    public GameObject itemMover;

    public void Exectue()
    {
        physicsPointer.SetActive(false);
        itemMover.SetActive(true);
    }
}
