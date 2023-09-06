using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Drone.Builder;
using Drone.Builder.Text3D;
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
                objectData[Idents.Tags.SaveLoadTags.YOffset] = trackObj.yOffset.ToString(CultureInfo.InvariantCulture);
                objectData[Idents.Tags.SaveLoadTags.MaxMouseDistance] =
                    trackObj.maxMouseDistance.ToString(CultureInfo.InvariantCulture);
                objectData[Idents.Tags.SaveLoadTags.Damage] = trackObj.damage.ToString(CultureInfo.InvariantCulture);

                if (trackObj.interactiveType != InteractiveType.None)
                {
                    interactiveData[Idents.Tags.SaveLoadTags.ColorIndex] =
                        trackObj.interactiveObject.colorIndex.ToString(CultureInfo.InvariantCulture);
                    interactiveData[Idents.Tags.SaveLoadTags.IsActive] = trackObj.interactiveObject.isActive;
                }
                
                switch (trackObj.interactiveType)
                {
                    case InteractiveType.Windmill:
                        var windmillObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.WindMillRotateSpeed] = ((Windmill)trackObj.interactiveObject).windMillRotateSpeed.ToString(CultureInfo.InvariantCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Windmill] = windmillObject;
                        break;
                    case InteractiveType.Magnet:
                        var magnetObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.Magnet] = ((RigidbodyMagnet)trackObj.interactiveObject).magnetForce
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Magnet] = magnetObject;
                        break;
                    case InteractiveType.Pendulum:
                        var pendulum = (Pendulum)trackObj.interactiveObject;
                        var pendulumObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.PendulumMoveSpeed] = pendulum.pendulumMoveSpeed.ToString(CultureInfo.InvariantCulture),
                            [Idents.Tags.SaveLoadTags.LeftPendulumAngle] = pendulum.leftPendulumAngle.ToString(CultureInfo.InvariantCulture),
                            [Idents.Tags.SaveLoadTags.RightPendulumAngle] = pendulum.rightPendulumAngle.ToString(CultureInfo.InvariantCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Pendulum] = pendulumObject;
                        break;
                    case InteractiveType.Wind:
                        var windObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.WindForce] = ((WindZoneScript)trackObj.interactiveObject).windForce.ToString(CultureInfo.InvariantCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Wind] = windObject;
                        break;
                    case InteractiveType.Battery:
                        var batteryObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.BatteryEnergy] = ((Battery)trackObj.interactiveObject).batteryEnergy.ToString(CultureInfo.InvariantCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Battery] = batteryObject;
                        break;
                    case InteractiveType.Boost:
                        var boostObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.BoostSpeed] = ((BoostTrigger)trackObj.interactiveObject).boostSpeed.ToString(CultureInfo.InvariantCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Boost] = boostObject;
                        break;
                    case InteractiveType.Hint:
                        var hintObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.HintText] = ((Hint)trackObj.interactiveObject).hintText.text.ToString(CultureInfo.InvariantCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Hint] = hintObject;
                        break;
                    case InteractiveType.Port:
                        var portObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.HasPassword] = ((Port)trackObj.interactiveObject).hasPassword,
                            [Idents.Tags.SaveLoadTags.PortPassword] =
                                ((Port)trackObj.interactiveObject).portPassword.Password.ToString(CultureInfo
                                    .InvariantCulture)
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.Port] = portObject;
                        break;
                    case InteractiveType.TrMessage:
                        var triggerMessage = (TriggerMessage)trackObj.interactiveObject;
                        var soundObject = new JObject
                        {                            
                            [Idents.Tags.SaveLoadTags.TriggerMessageHint] = triggerMessage.triggerText,
                            [Idents.Tags.SaveLoadTags.TriggerMessageSound] = triggerMessage.GetSound()
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.TriggerMessage] = soundObject;
                        break;
                    case InteractiveType.MagnetKiller:
                        var magnetKillerObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.MagnetForce] = ((MagnetKiller)trackObj.interactiveObject).magnetForce.ToString(CultureInfo.InvariantCulture),
                            [Idents.Tags.SaveLoadTags.MagnetKillerRotateSpeed] = ((MagnetKiller)trackObj.interactiveObject).rotationSpeed.ToString(CultureInfo.InvariantCulture),
                            [Idents.Tags.SaveLoadTags.MagnetKillerDamage] = ((MagnetKiller)trackObj.interactiveObject).rotationSpeed.ToString(CultureInfo.InvariantCulture),
                            [Idents.Tags.SaveLoadTags.MagnetKillerDamageInterval] = ((MagnetKiller)trackObj.interactiveObject).rotationSpeed.ToString(CultureInfo.InvariantCulture),
                        };
                        interactiveData[Idents.Tags.SaveLoadTags.MagnetKiller] = magnetKillerObject;
                        break;
                    case InteractiveType.Text3D:
                        var text3dObject = new JObject
                        {
                            [Idents.Tags.SaveLoadTags.Text3DText] =
                                ((TextWriter3D)trackObj.interactiveObject).text3D.ToString(CultureInfo.InvariantCulture)
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
                var position = ParseVector3(jTokenPosition);
                var rotation = ParseVector3(jTokenRotation);
                var scale =    ParseVector3(jTokenScale);
                var layer = int.Parse(jTokenLayer);
                var yOffset = float.Parse(jTokenYOffset, CultureInfo.InvariantCulture);
                var maxMouseDistance = float.Parse(jTokenMaxMouseDistance, CultureInfo.InvariantCulture);
                var damage = float.Parse(jTokenDamage, CultureInfo.InvariantCulture);
                
                var jTokenInteractive = property.Value[Idents.Tags.SaveLoadTags.Interactive];

                bool isActive = false, hasPassword = false;
                var colorIndex = 0;
                float windMillRotationSpeed = 0,
                    magnetForce = 0,
                    pendulumMoveSpeed = 0,
                    leftPendulumAngle = 0,
                    rightPendulumAngle = 0,
                    windForce = 0,
                    batteryEnergy = 0,
                    boostSpeed = 0,
                    magnetKillerRotateSpeed = 0,
                    magnetKillerDamage = 0,
                    magnetKillerDamageInterval = 0;
                string hintText = "", text3D = "", portalMap = "", triggerMessageText = "",
                    portPassword = "", soundName = "";

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
                    var jTokenTriggerMessage = jTokenInteractive[Idents.Tags.SaveLoadTags.TriggerMessage];
                    var jTokenPort = jTokenInteractive[Idents.Tags.SaveLoadTags.Port];
                    var jTokenMagnetKiller = jTokenInteractive[Idents.Tags.SaveLoadTags.MagnetKiller];
                    
                    windMillRotationSpeed = jTokenWindmill != null
                        ? jTokenWindmill[Idents.Tags.SaveLoadTags.WindMillRotateSpeed]!.Value<float>()
                        : 0f;
                    
                    magnetForce = jTokenMagnet != null
                        ? jTokenMagnet[Idents.Tags.SaveLoadTags.MagnetForce]!.Value<float>()
                        : 0f;

                    if (jTokenMagnet == null)
                        magnetForce = jTokenMagnetKiller != null
                            ? jTokenMagnetKiller[Idents.Tags.SaveLoadTags.MagnetForce]!.Value<float>()
                            : 0f;

                    if (jTokenMagnetKiller != null)
                    {
                        magnetKillerRotateSpeed = jTokenMagnetKiller[Idents.Tags.SaveLoadTags.MagnetKillerRotateSpeed]!
                            .Value<float>();
                        magnetKillerDamage =
                            jTokenMagnetKiller[Idents.Tags.SaveLoadTags.MagnetKillerDamage]!.Value<float>();
                        magnetKillerDamageInterval =
                            jTokenMagnetKiller[Idents.Tags.SaveLoadTags.MagnetKillerDamageInterval]!.Value<float>();
                    }
                    else
                    {
                        magnetKillerRotateSpeed = 0;
                        magnetKillerDamage = 0;
                        magnetKillerDamageInterval = 0;
                    }

                    if (jTokenPendulum != null)
                    {
                        pendulumMoveSpeed = jTokenPendulum[Idents.Tags.SaveLoadTags.PendulumMoveSpeed]!.Value<float>();
                        leftPendulumAngle = jTokenPendulum[Idents.Tags.SaveLoadTags.LeftPendulumAngle]!.Value<float>();
                        rightPendulumAngle = jTokenPendulum[Idents.Tags.SaveLoadTags.RightPendulumAngle]!.Value<float>();
                    }
                    else
                    {
                        pendulumMoveSpeed = 0;
                        leftPendulumAngle = 0;
                        rightPendulumAngle = 0;
                    }

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
                    
                    triggerMessageText = jTokenTriggerMessage != null
                        ? jTokenTriggerMessage[Idents.Tags.SaveLoadTags.TriggerMessageHint]!.Value<string>()
                        : "";

                    soundName = jTokenTriggerMessage != null
                        ? jTokenTriggerMessage[Idents.Tags.SaveLoadTags.TriggerMessageSound]!.Value<string>()
                        : "";

                    hasPassword = jTokenPort != null && jTokenPort[Idents.Tags.SaveLoadTags.HasPassword]!.Value<bool>();

                    portPassword = jTokenPort != null
                        ? jTokenPort[Idents.Tags.SaveLoadTags.PortPassword]!.Value<string>()
                        : "777";
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
                    SoundName = soundName,
                    TriggerMessageText = triggerMessageText,
                    HasPassword = hasPassword,
                    PortPassword = portPassword,
                    MagnetKillerRotateSpeed = magnetKillerRotateSpeed,
                    MagnetKillerDamage = magnetKillerDamage,
                    MagnetKillerDamageInterval = magnetKillerDamageInterval,
                };
                
                result.Add(objInfo);
            }
            
            LevelLoadingComplete?.Invoke();

            return result;
        }

        private static string FormatVector3(Vector3 vector)
        {
            return vector.x.ToString(CultureInfo.InvariantCulture) + " " +
                   vector.y.ToString(CultureInfo.InvariantCulture) + " " +
                   vector.z.ToString(CultureInfo.InvariantCulture);
        }
        
        public static Vector3 ParseVector3(string str)
        {
            var values = str.Split(' ');
            var x = float.Parse(values[0], CultureInfo.InvariantCulture);
            var y = float.Parse(values[1], CultureInfo.InvariantCulture);
            var z = float.Parse(values[2], CultureInfo.InvariantCulture);
            return new Vector3(x, y, z);
        }
    }
}