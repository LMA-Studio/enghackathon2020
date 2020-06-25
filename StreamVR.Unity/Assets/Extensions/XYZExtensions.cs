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

using System;

using UnityEngine;

using LMAStudio.StreamVR.Common.Models;

namespace LMAStudio.StreamVR.Unity.Extensions
{
    public static class XYZExtensions
    {
        public static Matrix4x4 GetRotation(this Matrix4x4 m, XYZ bx, XYZ by, XYZ bz)
        {
            m[0, 0] = (float)bx.X;
            m[0, 1] = (float)bx.Z;
            m[0, 2] = (float)bx.Y;

            m[1, 0] = (float)bz.X;
            m[1, 1] = (float)bz.Z;
            m[1, 2] = (float)bz.Y;

            m[2, 0] = (float)by.X;
            m[2, 1] = (float)by.Z;
            m[2, 2] = (float)by.Y;

            return m;
        }

        public static Matrix4x4 GetRotation(this Common.Models.Transform t)
        {
            XYZ bx = t.BasisX;
            XYZ by = t.BasisY;
            XYZ bz = t.BasisZ;

            float angle = 180 - Mathf.Atan2((float)t.BasisX.Y, (float)t.BasisX.X) * 180 / Mathf.PI;

            Matrix4x4 m = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, Vector3.up));

            //m[0, 0] = (float)bx.X;
            //m[0, 1] = (float)bx.Z;
            //m[0, 2] = (float)bx.Y;

            //m[1, 0] = (float)bz.X;
            //m[1, 1] = (float)bz.Z;
            //m[1, 2] = (float)bz.Y;

            //m[2, 0] = (float)by.X;
            //m[2, 1] = (float)by.Z;
            //m[2, 2] = (float)by.Y;

            return m;
        }

        public static void SetRotation(this Common.Models.Transform t, Matrix4x4 m, bool flip)
        {
            float angle = 0;

            Vector3 axis = Vector3.up;

            m.rotation.ToAngleAxis(out angle, out axis);

            angle = 180 - angle;

            if (flip)
            {
                angle = angle + 180;
            }

            if (angle <= -180)
            {
                angle += 360;
            }
            if (angle > 180)
            {
                angle -= 360;
            }
            Debug.Log("SAVING " + angle);

            m = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));

            t.BasisX.X = m.m00;
            t.BasisX.Z = m.m01;
            t.BasisX.Y = m.m02;

            t.BasisZ.X = m.m10;
            t.BasisZ.Z = m.m11;
            t.BasisZ.Y = m.m12;

            t.BasisY.X = m.m20;
            t.BasisY.Z = m.m21;
            t.BasisY.Y = m.m22;

            Debug.Log(m);

            double newAngle2 = Math.Atan2(t.BasisX.Y, t.BasisX.X) * 180 / Math.PI;
            Debug.Log("SAVING 2 " + newAngle2);
        }
    }
}
