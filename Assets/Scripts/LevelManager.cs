using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Builder;
using Newtonsoft.Json;
using UnityEngine;

namespace DroneFootball
{
    public static class LevelManager
    {
        public static string[] LoadMaps()
        {
            var jsonFiles = Directory.GetFiles(Path.Combine(Application.dataPath, "Levels"), "*.json");

            for (var i = 0; i < jsonFiles.Length; i++)
            {
                jsonFiles[i] = Path.GetFileNameWithoutExtension(jsonFiles[i]);
            }

            return jsonFiles;
        }
        
        public static bool LoadLevel(string level)
        {
            return File.Exists(Application.dataPath + "/Levels/" + level + ".json");
        }
        
        public static bool IsValidLevelName(string input)
        {
            const string pattern = @"^(?=.*[a-zA-Z])[a-zA-Z0-9]{2,}$";
            return Regex.IsMatch(input, pattern);
        }
        
        public static void SaveLevel(BuilderManager builderManager, string levelName)
        {
            var data = new Dictionary<string, Dictionary<string, string>>();
            foreach (var obj in builderManager.levelScene.GetRootGameObjects())
            {
                if (obj.layer != LayerMask.NameToLayer("TrackGround") && obj.layer != LayerMask.NameToLayer("Track"))
                    continue;

                var trackObj = obj.GetComponent<TrackObject>();
                var objData = new Dictionary<string, string>
                {
                    [nameof(obj.name)] = obj.name,
                    ["position"] = FormatVector3(obj.transform.position),
                    ["rotation"] = FormatVector3(obj.transform.rotation.eulerAngles),
                    ["scale"] = FormatVector3(obj.transform.localScale),
                    ["layer"] = obj.layer.ToString(),
                    [nameof(trackObj.yOffset)] = trackObj.yOffset.ToString(CultureInfo.CurrentCulture),
                    [nameof(trackObj.maxMouseDistance)] = trackObj.maxMouseDistance.ToString(CultureInfo.CurrentCulture),
                    [nameof(trackObj.damage)] = trackObj.damage.ToString(CultureInfo.CurrentCulture),
                    [nameof(trackObj.interactiveObject.windMillRotateSpeed)] = null,
                    [nameof(trackObj.interactiveObject.magnetForce)] = null,
                    [nameof(trackObj.interactiveObject.pendulumMoveSpeed)] = null,
                    [nameof(trackObj.interactiveObject.leftPendulumAngle)] = null,
                    [nameof(trackObj.interactiveObject.rightPendulumAngle)] = null,
                    [nameof(trackObj.interactiveObject.windForce)] = null,
                    [nameof(trackObj.interactiveObject.batteryEnergy)] = null,
                    ["isFreezing"] = null,
                    [nameof(trackObj.interactiveObject.boostSpeed)] = null,
                    [nameof(trackObj.interactiveObject.hintText)] = null,
                    [nameof(trackObj.interactiveObject.isLampTurn)] = null
                };

                if (trackObj.interactiveObject)
                {
                    switch (trackObj.interactiveType)
                    {
                        case InteractiveType.Windmill:
                            objData[nameof(trackObj.interactiveObject.windMillRotateSpeed)] =
                                trackObj.interactiveObject.windMillRotateSpeed.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Magnet:
                            objData[nameof(trackObj.interactiveObject.magnetForce)] =
                                trackObj.interactiveObject.magnetForce.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Pendulum:
                            objData[nameof(trackObj.interactiveObject.pendulumMoveSpeed)] =
                                trackObj.interactiveObject.pendulumMoveSpeed.ToString(CultureInfo.CurrentCulture);
                            objData[nameof(trackObj.interactiveObject.leftPendulumAngle)] =
                                trackObj.interactiveObject.leftPendulumAngle.ToString(CultureInfo.CurrentCulture);
                            objData[nameof(trackObj.interactiveObject.rightPendulumAngle)] =
                                trackObj.interactiveObject.rightPendulumAngle.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Wind:
                            objData[nameof(trackObj.interactiveObject.windForce)] =
                                trackObj.interactiveObject.windForce.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Battery:
                            objData[nameof(trackObj.interactiveObject.batteryEnergy)] =
                                trackObj.interactiveObject.batteryEnergy.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Freezing:
                            objData["isFreezing"] =
                                trackObj.interactiveObject.isActive.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Boost:
                            objData[nameof(trackObj.interactiveObject.boostSpeed)] = trackObj.interactiveObject.boostSpeed.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Hint:
                            objData[nameof(trackObj.interactiveObject.hintText)] =
                                trackObj.interactiveObject.hintText.text.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Lamp:
                            objData[nameof(trackObj.interactiveObject.isLampTurn)] =
                                trackObj.interactiveObject.isLampTurn.ToString(CultureInfo.CurrentCulture);
                            break;
                    }
                }

                data[obj.GetInstanceID() + ""] = objData;
            }
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(Application.dataPath + "/Levels/" + levelName + ".json", json);
        }

        private static string FormatVector3(Vector3 vector)
        {
            return vector.x + " " + vector.y + " " + vector.z;
        }
    }
}