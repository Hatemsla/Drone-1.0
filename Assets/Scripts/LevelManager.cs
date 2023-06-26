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

                var objData = new Dictionary<string, string>();
                objData["name"] = obj.name;
                objData["position"] = FormatVector3(obj.transform.position);
                objData["rotation"] = FormatVector3(obj.transform.rotation.eulerAngles);
                objData["scale"] = FormatVector3(obj.transform.localScale);
                objData["layer"] = obj.layer.ToString();
                var trackObj = obj.GetComponent<TrackObject>();
                objData["yOffset"] = trackObj.yOffset.ToString(CultureInfo.CurrentCulture);
                objData["maxMouseDistance"] = trackObj.maxMouseDistance.ToString(CultureInfo.CurrentCulture);
                objData["damage"] = trackObj.damage.ToString(CultureInfo.CurrentCulture);
                objData["rotSpeed"] = trackObj.windmill != null ? trackObj.windmill.rotateSpeed.ToString(CultureInfo.CurrentCulture) : null;
                objData["magnetForce"] = trackObj.magnet != null ? trackObj.magnet.magnetForce.ToString(CultureInfo.CurrentCulture) : null;
                objData["pendulumMoveSpeed"] = trackObj.pendulum != null ? trackObj.pendulum.moveSpeed.ToString(CultureInfo.CurrentCulture) : null;
                objData["windForce"] = trackObj.windZone != null ? trackObj.windZone.windForce.ToString(CultureInfo.CurrentCulture) : null;
                objData["batteryEnergy"] = trackObj.battery != null ? trackObj.battery.energy.ToString(CultureInfo.CurrentCulture) : null;
                objData["freezing"] = trackObj.freezingBall != null ? trackObj.freezingBall.isFreezing.ToString(CultureInfo.CurrentCulture) : null;
                objData["boost"] = trackObj.boost != null ? trackObj.boost.boost.ToString(CultureInfo.CurrentCulture) : null;
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