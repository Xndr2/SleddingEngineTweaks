using MelonLoader;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using Il2Cpp;
using HarmonyLib;

namespace SleddingEngineTweaks.Mods
{
    public class FirstPerson
    {
        private static KeyCode firstPersonToggle;
        private static bool isFirstPerson = false;
        private static bool firstPersonSetup = false;

        // camera
        private static GameObject mainCameraObj;
        private static GameObject cinemachineCameraObj;
        private static GameObject networkedPlayerObj;
        private static GameObject firstPersonCameraObj;
        private static Camera firstPersonCamera;
        private static CinemachineCamera normalCamera;
        private static int priorityOffset = 1000;
        private static GameObject frog;
        private static List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();


        public FirstPerson()
        {
            firstPersonToggle = KeyCode.F10;
            
            MelonLogger.Msg("First person mod initialized");
        }

        public void OnLateUpdate()
        {
            if (Input.GetKeyDown(firstPersonToggle))
            {
                ToggleFirstPerson();
            }

            if (!firstPersonSetup)
            {
                FirstPersonSetup();
            }
        }

        private static void FirstPersonSetup()
        {
            if (InMenu()) return;
            
            MelonLogger.Msg("First person setup");

            // get main camera
            mainCameraObj = GameObject.Find("Main Camera");
            if (mainCameraObj == null)
            {
                MelonLogger.Msg("Main camera not found. THIS SHOULD NEVER HAPPEN!!");
                return;
            }

            // get "CinemachineCamera (makes parent null on start)"
            // we have to set the follow on this
            cinemachineCameraObj = GameObject.Find("CinemachineCamera (makes parent null on start)");
            if (cinemachineCameraObj == null)
            {
                MelonLogger.Msg("Cinemachine camera not found. THIS SHOULD NEVER HAPPEN!!");
                return;
            }
            normalCamera = cinemachineCameraObj.GetComponent<CinemachineCamera>();
            normalCamera.Priority += priorityOffset;

            // get player
            networkedPlayerObj = GameObject.Find("Player Networked [connId=0]");
            if (networkedPlayerObj == null)
            {
                MelonLogger.Msg("Networked player not found. THIS SHOULD NEVER HAPPEN!!");
                return;
            }
           

           // create a normal camera in the players head
           firstPersonCameraObj = new GameObject("First Person Camera");
           firstPersonCameraObj.transform.parent = networkedPlayerObj.transform;
           firstPersonCameraObj.transform.localPosition = GameObject.Find("Follow Point").transform.localPosition;
           firstPersonCameraObj.transform.localRotation = Quaternion.identity;
           firstPersonCamera = firstPersonCameraObj.AddComponent<Camera>();
           firstPersonCamera.enabled = false;
           // copy the lens setting stats
           LensSettings lensSettings = cinemachineCameraObj.GetComponent<CinemachineCamera>().Lens;
           firstPersonCamera.fieldOfView = lensSettings.FieldOfView;
           firstPersonCamera.nearClipPlane = lensSettings.NearClipPlane;
           firstPersonCamera.farClipPlane = lensSettings.FarClipPlane;

            frog = GameObject.Find("Graphics (all characters start enabled)");
            if (frog == null)
            {
                MelonLogger.Msg("Frog not found. THIS SHOULD NEVER HAPPEN!!");
                return;
            }

            firstPersonSetup = true;
        }

        private static bool InMenu()
        {
            return GameObject.Find("Head Follow Point") == null; // this doesnt exist in the menu
        }

        private static void ToggleFirstPerson()
        {
            isFirstPerson = !isFirstPerson;
            
            if (isFirstPerson)
            {
                // Enable first person camera and controller
                firstPersonCamera.enabled = true;
                mainCameraObj.SetActive(false);
                ToggleVisibility(true);
            }
            else
            {
                // Restore third person camera
                firstPersonCamera.enabled = false;
                mainCameraObj.SetActive(true);
                ToggleVisibility(false);
            }
        }

        public static void OnDeinitialize()
        {
            if (isFirstPerson)
            {
                ToggleFirstPerson(); // reset the first person mode if the melon gets unloaded
            }
        }

        private static void ToggleVisibility(bool isFirstPerson)
        {
            if (isFirstPerson)
            {
                // run through all children of character_frog and set the active state of skinned mesh renderers
                skinnedMeshRenderers.Clear();
                foreach (SkinnedMeshRenderer renderer in frog.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    // only save it if its enabled
                    if (renderer.enabled == true)
                    {
                        // save to list
                        renderer.enabled = false;
                        skinnedMeshRenderers.Add(renderer);
                    }
                }
            }
            else
            {
                foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
                {
                    // remove from list
                    renderer.enabled = true;
                }
            }
        }
    }
}