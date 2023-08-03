using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Builder;
using Drone;
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
                    [Idents.Tags.SaveLoadTags.ObjectName] = obj.name,
                    [Idents.Tags.SaveLoadTags.Position] = FormatVector3(obj.transform.position),
                    [Idents.Tags.SaveLoadTags.Rotation] = FormatVector3(obj.transform.rotation.eulerAngles),
                    [Idents.Tags.SaveLoadTags.Scale] = FormatVector3(obj.transform.localScale),
                    [Idents.Tags.SaveLoadTags.Layer] = obj.layer.ToString(),
                    [Idents.Tags.SaveLoadTags.YOffset] = trackObj.yOffset.ToString(CultureInfo.CurrentCulture),
                    [Idents.Tags.SaveLoadTags.MaxMouseDistance] = trackObj.maxMouseDistance.ToString(CultureInfo.CurrentCulture),
                    [Idents.Tags.SaveLoadTags.Damage] = trackObj.damage.ToString(CultureInfo.CurrentCulture),
                    [Idents.Tags.SaveLoadTags.IsLampTurn] = null,
                    [Idents.Tags.SaveLoadTags.WindMillRotateSpeed] = null,
                    [Idents.Tags.SaveLoadTags.MagnetForce] = null,
                    [Idents.Tags.SaveLoadTags.PendulumMoveSpeed] = null,
                    [Idents.Tags.SaveLoadTags.LeftPendulumAngle] = null,
                    [Idents.Tags.SaveLoadTags.RightPendulumAngle] = null,
                    [Idents.Tags.SaveLoadTags.WindForce] = null,
                    [Idents.Tags.SaveLoadTags.BatteryEnergy] = null,
                    [Idents.Tags.SaveLoadTags.IsFreezing] = null,
                    [Idents.Tags.SaveLoadTags.BoostSpeed] = null,
                    [Idents.Tags.SaveLoadTags.HintText] = null,
                    [Idents.Tags.SaveLoadTags.IsLampTurn] = null,
                };

                if (trackObj.interactiveObject)
                {
                    switch (trackObj.interactiveType)
                    {
                        case InteractiveType.Windmill:
                            objData[Idents.Tags.SaveLoadTags.WindMillRotateSpeed] =
                                trackObj.interactiveObject.windMillRotateSpeed.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Magnet:
                            objData[Idents.Tags.SaveLoadTags.MagnetForce] =
                                trackObj.interactiveObject.magnetForce.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Pendulum:
                            objData[Idents.Tags.SaveLoadTags.PendulumMoveSpeed] =
                                trackObj.interactiveObject.pendulumMoveSpeed.ToString(CultureInfo.CurrentCulture);
                            objData[Idents.Tags.SaveLoadTags.LeftPendulumAngle] =
                                trackObj.interactiveObject.leftPendulumAngle.ToString(CultureInfo.CurrentCulture);
                            objData[Idents.Tags.SaveLoadTags.RightPendulumAngle] =
                                trackObj.interactiveObject.rightPendulumAngle.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Wind:
                            objData[Idents.Tags.SaveLoadTags.WindForce] =
                                trackObj.interactiveObject.windForce.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Battery:
                            objData[Idents.Tags.SaveLoadTags.BatteryEnergy] =
                                trackObj.interactiveObject.batteryEnergy.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Freezing:
                            objData[Idents.Tags.SaveLoadTags.IsFreezing] =
                                trackObj.interactiveObject.isActive.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Boost:
                            objData[Idents.Tags.SaveLoadTags.BoostSpeed] = trackObj.interactiveObject.boostSpeed.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Hint:
                            objData[Idents.Tags.SaveLoadTags.HintText] =
                                trackObj.interactiveObject.hintText.text.ToString(CultureInfo.CurrentCulture);
                            break;
                        case InteractiveType.Lamp:
                            objData[Idents.Tags.SaveLoadTags.IsLampTurn] =
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