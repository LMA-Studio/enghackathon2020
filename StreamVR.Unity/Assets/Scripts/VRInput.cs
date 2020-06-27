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
using UnityEngine.EventSystems;

public class VRInput : BaseInput
{

    public Camera eventCamera = null;

    public OVRInput.Button clickButton = OVRInput.Button.PrimaryIndexTrigger;
    public OVRInput.Controller controller = OVRInput.Controller.All;

    protected override void Awake()
    {
        GetComponent<BaseInputModule>().inputOverride = this;
    }

    public override bool GetMouseButton(int button)
    {
        return OVRInput.Get(clickButton, controller);
    }

      public override bool GetMouseButtonDown(int button)
    {
        return OVRInput.GetDown(clickButton,controller);
    }

      public override bool GetMouseButtonUp(int button)
    {
        return OVRInput.GetUp(clickButton,controller);
    }

      public override Vector2 mousePosition
      {
        get
        {
          return new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight / 2);
        }
          
      }
}
