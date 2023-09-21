using System;
using System.Collections;
using System.Collections.Generic;
using Drone.Builder.Text3D;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Drone.Builder
{
    public sealed class LoadLevelSystem
    {
        private readonly List<GameObject> _objectsPool = new();

        public event Action<List<GameObject>> ObjectsCreatedEvent;
        public event Action LoadingCompleteEvent;
        
        public IEnumerator LoadScene(string levelName)
        {
            var loadedData = LevelManager.LoadLevel(levelName);

            foreach (var objInfo in loadedData)
            {
                var loadOp = Addressables.LoadAssetAsync<GameObject>(objInfo.ObjectName);
                yield return loadOp;
                
                if (loadOp.Status != AsyncOperationStatus.Succeeded || loadOp.Result == null)
                {
                    Debug.LogError("Failed to load asset: " + objInfo.ObjectName);
                    continue;
                }
                
                var newObj = Object.Instantiate(loadOp.Result, objInfo.Position, Quaternion.Euler(objInfo.Rotation));

                yield return new WaitForSeconds(0.01f);
                TrackBuilderUtils.ChangeLayerRecursively(newObj.transform, objInfo.Layer);
                TrackBuilderUtils.OffOutlineRecursively(newObj.transform);
                newObj.transform.localScale = objInfo.Scale;
                var trackObj = newObj.GetComponent<TrackObject>();
                trackObj.yOffset = objInfo.YOffset;
                trackObj.maxMouseDistance = objInfo.MaxMouseDistance;
                trackObj.damage = objInfo.Damage;
                trackObj.previousRotationXValue = objInfo.PreviousXRotation;

                switch (trackObj.interactiveType)
                {
                    case InteractiveType.Windmill:
                        ((Windmill)trackObj.interactiveObject).windMillRotateSpeed = objInfo.WindMillRotateSpeed;
                        break;
                    case InteractiveType.Magnet:
                        ((RigidbodyMagnet)trackObj.interactiveObject).magnetForce = objInfo.MagnetForce;
                        break;
                    case InteractiveType.Pendulum:
                        ((Pendulum)trackObj.interactiveObject).pendulumMoveSpeed = objInfo.PendulumMoveSpeed;
                        ((Pendulum)trackObj.interactiveObject).leftPendulumAngle = objInfo.LeftPendulumAngle;
                        ((Pendulum)trackObj.interactiveObject).rightPendulumAngle = objInfo.RightPendulumAngle;
                        break;
                    case InteractiveType.Wind:
                        ((WindZoneScript)trackObj.interactiveObject).windForce = objInfo.WindForce;
                        break;
                    case InteractiveType.Battery:
                        ((Battery)trackObj.interactiveObject).batteryEnergy = objInfo.BatteryEnergy;
                        break;
                    case InteractiveType.Boost:
                        ((BoostTrigger)trackObj.interactiveObject).boostSpeed = objInfo.BoostSpeed;
                        break;
                    case InteractiveType.Hint:
                        ((Hint)trackObj.interactiveObject).hintText.text = objInfo.HintText;
                        break;
                    case InteractiveType.ElectroGate:
                        break;
                    case InteractiveType.Text3D:
                        ((TextWriter3D)trackObj.interactiveObject).text3D = objInfo.Text3d;
                        break;
                    case InteractiveType.Port:
                        ((Port)trackObj.interactiveObject).portPassword.Password = objInfo.PortPassword;
                        ((Port)trackObj.interactiveObject).hasPassword = objInfo.HasPassword;
                        break;
                    case InteractiveType.TrMessage:
                        ((TriggerMessage)trackObj.interactiveObject).SetSound(objInfo.SoundName);
                        ((TriggerMessage)trackObj.interactiveObject).triggerText = objInfo.TriggerMessageText;
                        break;
                    case InteractiveType.MagnetKiller:
                        ((MagnetKiller)trackObj.interactiveObject).magnetForce = objInfo.MagnetForce;
                        ((MagnetKiller)trackObj.interactiveObject).rotationSpeed = objInfo.MagnetKillerRotateSpeed;
                        ((MagnetKiller)trackObj.interactiveObject).baseDamage = objInfo.MagnetKillerDamage;
                        ((MagnetKiller)trackObj.interactiveObject).damageInterval = objInfo.MagnetKillerDamageInterval;
                        break;
                    case InteractiveType.Portal:
                        ((PortalObject)trackObj.interactiveObject).SetMap(objInfo.PortalMap);
                        break;
                    case InteractiveType.None:
                        break;
                }
                
                if (trackObj.interactiveType != InteractiveType.None)
                {
                    trackObj.interactiveObject.SetColorIndex(objInfo.ColorIndex);
                    trackObj.interactiveObject.SetActive(objInfo.IsActive);
                }

                _objectsPool.Add(newObj);
            }
            
            yield return null;
            
            ObjectsCreatedEvent?.Invoke(_objectsPool);
            LoadingCompleteEvent?.Invoke();
        }
    }
}