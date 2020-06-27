/*
    This file is part of LMAStudio.StreamVR
    Copyright(C) 2020  Andreas Brake, Lisa-Marie Mueller

    LMAStudio.StreamVR is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ShootingPaintScript : MonoBehaviour
{
    public XRNode inputSource;

    public float speed = 40;

    public GameObject paintBlob;
    public Transform barrel;
    public AudioSource audioSource;
    public AudioClip audioClip;

    private bool clicking = false;

    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);

        bool clicked;
        device.TryGetFeatureValue(CommonUsages.triggerButton, out clicked);

        if (clicked)
        {
            if (!clicking)
            {
                clicking = true;
                Fire();
            }
        }
        else
        {
            clicking = false;
        }

        bool gripped;
        device.TryGetFeatureValue(CommonUsages.gripButton, out gripped);

        if (gripped)
        {
            var pointer = this.transform.parent.gameObject.GetComponentInChildren<LMAStudio.StreamVR.Unity.Scripts.PhysicsPointer>(true)?.gameObject;
            if (pointer != null)
            {
                this.gameObject.SetActive(false);
                pointer.SetActive(true);
            }
        }
    }

    public void ChangePaint(Material newPaint)
    {
        paintBlob.GetComponent<MeshRenderer>().material = newPaint;
    }

    public void Fire()
    {
        GameObject spawnedPaint = Instantiate(paintBlob, barrel.position, barrel.rotation);
        spawnedPaint.GetComponent<Rigidbody>().velocity = speed * barrel.up;
        audioSource.PlayOneShot(audioClip);
        Destroy(spawnedPaint, 2);
    }

}
