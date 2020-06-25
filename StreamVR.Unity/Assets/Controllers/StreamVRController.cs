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
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;

using LMAStudio.StreamVR.Common.Models;
using LMAStudio.StreamVR.Unity.Logic;
using LMAStudio.StreamVR.Common;
using System.Collections;

namespace LMAStudio.StreamVR.Unity.Scripts
{
    public class StreamVRController : MonoBehaviour
    {
        public string natsEndpoint = "192.168.0.119:7002";
        public Text loadingText = null;

        private ICommunicator comms;

        private void Display(string msg)
        {
            if (loadingText == null)
            {
                Debug.Log(msg);
            }
            else
            {
                loadingText.text = msg;
            }
        }

        private void Start()
        {
            comms = new Communicator(natsEndpoint, Debug.Log);
            Display("Not Connected");

            this.TryStartRepeat();
        }

        public void TryStartRepeat()
        {
            StartCoroutine(Connect());
        }

        private IEnumerator Connect()
        {
            Display("Connecting...");

            yield return new WaitForSecondsRealtime(0.2f);

            bool success = false;
            try
            {
                comms.Connect();
                Display("Connected");

                success = true;

                this.LoadAll();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                Display("Failed to connect. Trying Again Shortly...");

                success = false;
            }

            if (!success)
            {
                yield return new WaitForSecondsRealtime(3);
                TryStartRepeat();
            }
        }

        public void LoadAll()
        {
            this.LoadMaterials();
            this.LoadFamilies();
            this.LoadWalls();
            this.LoadFloors();
            this.LoadCeilings();
            this.LoadFamilyInstances();
        }

        public void LoadMaterials()
        {
            List<JObject> dataSet = LoadType("Autodesk.Revit.DB.Material", "Material");
            List<Common.Models.Material> materials = dataSet.Select(x => x.ToObject<Common.Models.Material>()).ToList();
            MaterialLibrary.LoadMaterials(materials);
        }

        public void LoadFamilies()
        {
            List<JObject> dataSet = LoadType("Autodesk.Revit.DB.Family", "Family");
            List<Family> families = dataSet.Select(x => x.ToObject<Family>()).ToList();
            FamilylLibrary.LoadFamilies(families);
        }

        public void LoadWalls()
        {
            List<JObject> dataSet = LoadType("Autodesk.Revit.DB.Wall", "Wall");
            List<Wall> walls = dataSet.Select(x => x.ToObject<Wall>()).ToList();
            this.GetComponent<WallPlacer>().Place(walls);
        }

        public void LoadFloors()
        {
            List<JObject> dataSet = LoadType("Autodesk.Revit.DB.Floor", "Floor");
            List<Floor> floors = dataSet.Select(x => x.ToObject<Floor>()).ToList();
            this.GetComponent<FloorPlacer>().Place(floors);
        }

        public void LoadCeilings()
        {
            List<JObject> dataSet = LoadType("Autodesk.Revit.DB.Ceiling", "Ceiling");
            List<Ceiling> ceilings = dataSet.Select(x => x.ToObject<Ceiling>()).ToList();
            this.GetComponent<CeilingPlacer>().Place(ceilings);
        }

        public void LoadFamilyInstances()
        {
            List<JObject> dataSet = LoadType("Autodesk.Revit.DB.FamilyInstance", "FamilyInstance");
            List<FamilyInstance> familyInstances = dataSet.Select(x => x.ToObject<FamilyInstance>()).ToList();
            this.GetComponent<FamilyPlacer>().Place(familyInstances);
        }

        public void Shutdown()
        {
            Display("Not Connected");
            comms.Publish(Communicator.TO_SERVER_CHANNEL, new Message { Type = "EXIT" });
        }

        public void SaveFamilyInstance(FamilyInstance fam)
        {
            Debug.Log($"Saving {fam.Id} {fam.Name}");
            Display($"Saving {fam.Id} {fam.Name}");
            Message response = comms.RequestSync(Communicator.TO_SERVER_CHANNEL, new Message
            {
                Type = "SET",
                Data = JObject.FromObject(fam)
            }, 5000);
            Debug.Log(JsonConvert.SerializeObject(response));
        }

        private List<JObject> LoadType(string type, string name)
        {
            Display($"Getting {name}s...");

            try
            {
                Message response = comms.RequestSync(Communicator.TO_SERVER_CHANNEL, new Message
                {
                    Type = "GET_ALL",
                    Data = JObject.FromObject(new
                    {
                        Type = type
                    })
                }, 2000);

                Display($"Got {name}s!");
                Debug.Log(JsonConvert.SerializeObject(response));

                if (response.Type == "ERROR")
                {
                    Debug.LogError(response.Data);
                    return new List<JObject>();
                }

                List<JObject> objects = JArray.FromObject(response.Data).Select(x => (JObject)x).ToList();

                List<JObject> errors = objects.Where(o => o["ERROR"] != null).ToList();
                foreach (var e in errors)
                {
                    Debug.LogWarning(e);
                }

                return objects.Where(o => o["ERROR"] == null).ToList();
            }
            catch (NATS.Client.NATSTimeoutException e)
            {
                Display("Timeout Exception");
                Debug.LogWarning(e);
            }
            catch (Exception e)
            {
                Display($"Error {e.Message}");
                Debug.LogError(e);
            }

            return new List<JObject>();
        }
    }
}
