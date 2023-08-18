using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Drone.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Drone
{
    public static class LevelManager
    {
        public static event Action StartLevelLoading;
        public static event Action LevelLoadingComplete;
        
        public static string[] LoadMaps()
        {
            var jsonFiles = Directory.GetFiles(Path.Combine(Application.dataPath, "Levels"), "*.json");

            for (var i = 0; i < jsonFiles.Length; i++)
            {
                jsonFiles[i] = Path.GetFileNameWithoutExtension(jsonFiles[i]);
            }

            return jsonFiles;
        }
        
        public static bool IsLevelExist(string level)
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
            var data = new JObject();
            foreach (var obj in builderManager.levelScene.GetRootGameObjects())
            {
                var objectData = new JObject();
                var interactiveData = new JObject();
                var trackObj = obj.GetComponent<TrackObject>();
                objectData[Idents.Tags.SaveLoadTags.ObjectName] = obj.name;
                objectData[Idents.Tags.SaveLoadTags.Position] = FormatVector3(obj.transform.position);
                objectData[Idents.Tags.SaveLoadTags.Rotation] = FormatVector3(obj.transform.rotation.eulerAngles);
                objectData[Idents.Tags.SaveLoadTags.Scale] = FormatVector3(obj.transform.localScale);
                objectData[Idents.Tags.SaveLoadTags.Layer] = obj.layer.ToString();
                objectData[Idents.Tags.SaveLoadTags.YOffset] = trackObj.yOffset.ToString(CultureInfo.CurrentCulture);
                objectData[Idents.Tags.SaveLoadTags.MaxMouseDistance] =
                    trackObj.maxMouseDistance.ToString(CultureInfo.CurrentCulture);
                objectData[Idents.Tags.SaveLoadTags.Damage] = trackObj.damage.ToString(CultureInfo.CurrentCulture);

                if (trackObj.interactiveType != InteractiveType.None)
                {
                    interactiveData[Idents.Tags.SaveLoadTags.ColorIndex] =
                        trackObj.interactiveObject.color_index.ToString(CultureInfo.CurrentCulture);
                    interactiveData[Idents.Tags.SaveLoadTags.IsActive] = trackObj.interactiveObject.isActive;
                }
                
                switch (trackObj.interactiveType)
                {
                    case InteractiveType.Windmill:
                        var windmillObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.WindMillRotateSpeed] = trackObj.interactiveObject.windMillRotateSpeed.ToString(CultureInfo.CurrentCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Windmill] = windmillObject;
                        break;
                    case InteractiveType.Magnet:
                        var magnetObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.MagnetForce] = trackObj.interactiveObject.magnetForce.ToString(CultureInfo.CurrentCulture)
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Magnet] = magnetObject;
                        break;
                    case InteractiveType.Pendulum:
                        var pendulumObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.PendulumMoveSpeed] = trackObj.interactiveObject.pendulumMoveSpeed.ToString(CultureInfo.CurrentCulture),
                            [Idents.Tags.SaveLoadTags.LeftPendulumAngle] = trackObj.interactiveObject.leftPendulumAngle.ToString(CultureInfo.CurrentCulture),
                            [Idents.Tags.SaveLoadTags.RightPendulumAngle] = trackObj.interactiveObject.rightPendulumAngle.ToString(CultureInfo.CurrentCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Pendulum] = pendulumObject;
                        break;
                    case InteractiveType.Wind:
                        var windObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.WindForce] = trackObj.interactiveObject.windForce.ToString(CultureInfo.CurrentCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Wind] = windObject;
                        break;
                    case InteractiveType.Battery:
                        var batteryObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.BatteryEnergy] = trackObj.interactiveObject.batteryEnergy.ToString(CultureInfo.CurrentCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Battery] = batteryObject;
                        break;
                    case InteractiveType.Freezing:
                        break;
                    case InteractiveType.Boost:
                        var boostObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.BoostSpeed] = trackObj.interactiveObject.boostSpeed.ToString(CultureInfo.CurrentCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Boost] = boostObject;
                        break;
                    case InteractiveType.Hint:
                        var hintObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.HintText] = trackObj.interactiveObject.hintText.text.ToString(CultureInfo.CurrentCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Hint] = hintObject;
                        break;
                    case InteractiveType.Lamp:
                        break;
                    case InteractiveType.Draw:
                        break;
                    case InteractiveType.Door:
                        break;
                    case InteractiveType.Port:
                        break;
                    case InteractiveType.SecureCamera:
                        break;
                    case InteractiveType.ElectroGate:
                        break;
                    case InteractiveType.Panel:
                        break;
                    case InteractiveType.Button:
                        break;
                    case InteractiveType.Terminal:
                        break;
                    case InteractiveType.TrMessage:
                        break;
                    case InteractiveType.MagnetKiller:
                        var magnetKillerObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.MagnetForce] = trackObj.interactiveObject.magnetForce.ToString(CultureInfo.CurrentCulture)
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Magnet] = magnetKillerObject;
                        break;
                    case InteractiveType.PitStop:
                        break;
                    case InteractiveType.Text3D:
                        var text3dObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.Text3DText] =
                                trackObj.interactiveObject.text3D.ToString(CultureInfo.CurrentCulture)
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Text3D] = text3dObject;
                        break;
                    case InteractiveType.Portal:
                        var portal = (PortalObject)trackObj.interactiveObject;
                        var portalObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.PortalMap] = portal.GetMap()
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Portal] = portalObject;
                        break;
                    case InteractiveType.None:
                        break;
                }

                if(interactiveData.Count > 0)
                    objectData[Idents.Tags.SaveLoadTags.Interactive] = interactiveData;
                data[obj.GetInstanceID().ToString()] = objectData;
            }
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(Application.dataPath + "/Levels/" + levelName + ".json", json);
        }

        public static List<GameObjectInfo> LoadLevel(string levelName)
        {
            StartLevelLoading?.Invoke();

            var filePath = Application.dataPath + "/Levels/" + levelName + ".json";

            var jsonData = File.ReadAllText(filePath);
            var loadedData = JObject.Parse(jsonData);
            var result = new List<GameObjectInfo>();

            foreach (var property in loadedData.Properties())
            {
                var jTokenObjectName = property.Value[Idents.Tags.SaveLoadTags.ObjectName]!.Value<string>();
                var jTokenPosition = property.Value[Idents.Tags.SaveLoadTags.Position]!.Value<string>();
                var jTokenRotation = property.Value[Idents.Tags.SaveLoadTags.Rotation]!.Value<string>();
                var jTokenScale = property.Value[Idents.Tags.SaveLoadTags.Scale]!.Value<string>();
                var jTokenLayer = property.Value[Idents.Tags.SaveLoadTags.Layer]!.Value<string>();
                var jTokenYOffset = property.Value[Idents.Tags.SaveLoadTags.YOffset]!.Value<string>();
                var jTokenMaxMouseDistance = property.Value[Idents.Tags.SaveLoadTags.MaxMouseDistance]!.Value<string>();
                var jTokenDamage = property.Value[Idents.Tags.SaveLoadTags.Damage]!.Value<string>();

                var objectName = jTokenObjectName.Substring(0, jTokenObjectName.IndexOf('('));
                var position = TrackBuilderUtils.ParseVector3(jTokenPosition);
                var rotation = TrackBuilderUtils.ParseVector3(jTokenRotation);
                var scale = TrackBuilderUtils.ParseVector3(jTokenScale);
                var layer = int.Parse(jTokenLayer);
                var yOffset = float.Parse(jTokenYOffset);
                var maxMouseDistance = float.Parse(jTokenMaxMouseDistance);
                var damage = float.Parse(jTokenDamage);
                
                var jTokenInteractive = property.Value[Idents.Tags.SaveLoadTags.Interactive];

                var isActive = false;
                var colorIndex = 0;
                float windMillRotationSpeed = 0, magnetForce = 0, pendulumMoveSpeed = 0, 
                    leftPendulumAngle = 0, rightPendulumAngle = 0, windForce = 0, 
                    batteryEnergy = 0, boostSpeed = 0;
                string hintText = "", text3D = "", portalMap = "";

                if (jTokenInteractive != null)
                {
                    isActive = jTokenInteractive[Idents.Tags.SaveLoadTags.IsActive].Value<bool>();
                    colorIndex = jTokenInteractive[Idents.Tags.SaveLoadTags.ColorIndex].Value<int>();
                    var jTokenWindmill = jTokenInteractive[Idents.Tags.SaveLoadTags.Windmill];
                    var jTokenMagnet = jTokenInteractive[Idents.Tags.SaveLoadTags.Magnet];
                    var jTokenPendulum = jTokenInteractive[Idents.Tags.SaveLoadTags.Pendulum];
                    var jTokenWind = jTokenInteractive[Idents.Tags.SaveLoadTags.Wind];
                    var jTokenBattery = jTokenInteractive[Idents.Tags.SaveLoadTags.Battery];
                    var jTokenBoost = jTokenInteractive[Idents.Tags.SaveLoadTags.Boost];
                    var jTokenHint = jTokenInteractive[Idents.Tags.SaveLoadTags.Hint];
                    var jTokenText3d = jTokenInteractive[Idents.Tags.SaveLoadTags.Text3D];
                    var jTokenPortal = jTokenInteractive[Idents.Tags.SaveLoadTags.Portal];
                    
                    windMillRotationSpeed = jTokenWindmill != null
                        ? jTokenWindmill[Idents.Tags.SaveLoadTags.WindMillRotateSpeed]!.Value<float>()
                        : 0f;

                    magnetForce = jTokenMagnet != null
                        ? jTokenMagnet[Idents.Tags.SaveLoadTags.MagnetForce]!.Value<float>()
                        : 0f;

                    pendulumMoveSpeed = jTokenPendulum != null
                        ? jTokenPendulum[Idents.Tags.SaveLoadTags.PendulumMoveSpeed]!.Value<float>()
                        : 0f;

                    leftPendulumAngle = jTokenPendulum != null
                        ? jTokenPendulum[Idents.Tags.SaveLoadTags.LeftPendulumAngle]!.Value<float>()
                        : 0f;

                    rightPendulumAngle = jTokenPendulum != null
                        ? jTokenPendulum[Idents.Tags.SaveLoadTags.RightPendulumAngle]!.Value<float>()
                        : 0f;

                    windForce = jTokenWind != null
                        ? jTokenWind[Idents.Tags.SaveLoadTags.WindForce]!.Value<float>()
                        : 0f;

                    batteryEnergy = jTokenBattery != null
                        ? jTokenBattery[Idents.Tags.SaveLoadTags.BatteryEnergy]!.Value<float>()
                        : 0f;

                    boostSpeed = jTokenBoost != null
                        ? jTokenBoost[Idents.Tags.SaveLoadTags.BoostSpeed]!.Value<float>()
                        : 0f;

                    hintText = jTokenHint != null
                        ? jTokenHint[Idents.Tags.SaveLoadTags.HintText]!.Value<string>()
                        : "";
                    
                    text3D = jTokenText3d != null
                        ? jTokenText3d[Idents.Tags.SaveLoadTags.Text3DText]!.Value<string>()
                        : "";

                    portalMap = jTokenPortal != null
                        ? jTokenPortal[Idents.Tags.SaveLoadTags.PortalMap]!.Value<string>()
                        : "";
                }

                var objInfo = new GameObjectInfo
                {
                    ObjectName = objectName,
                    Position = position,
                    Rotation = rotation,
                    Scale = scale,
                    Layer = layer,
                    IsActive = isActive,
                    YOffset = yOffset,
                    Damage = damage,
                    ColorIndex = colorIndex,
                    MaxMouseDistance = maxMouseDistance,
                    WindMillRotateSpeed = windMillRotationSpeed,
                    MagnetForce = magnetForce,
                    PendulumMoveSpeed = pendulumMoveSpeed,
                    LeftPendulumAngle = leftPendulumAngle,
                    RightPendulumAngle = rightPendulumAngle,
                    WindForce = windForce,
                    BatteryEnergy = batteryEnergy,
                    BoostSpeed = boostSpeed,
                    HintText = hintText,
                    Text3d = text3D,
                    PortalMap = portalMap,
                };
                
                result.Add(objInfo);
            }
            
            LevelLoadingComplete?.Invoke();

            return result;
        }

        private static string FormatVector3(Vector3 vector)
        {
            return vector.x + " " + vector.y + " " + vector.z;
        }
    }
}