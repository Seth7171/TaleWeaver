// Filename: Enviro.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This singleton script manages environmental settings based on the loaded scene.
// It sets time of day and weather conditions for various maps.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enviro : MonoBehaviour
{
    public static Enviro Instance { get; private set; }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            // This means this instance was the singleton and is now being destroyed
            Debug.Log("Enviro instance is being destroyed.");
            Instance = null;
        }
    }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Enable the entire EnviroSkyMgr GameObject
        EnviroSkyMgr.instance.gameObject.SetActive(true);
        EnviroSkyMgr.instance.SetTimeProgress(EnviroTime.TimeProgressMode.None); // Stop the time progression
        EnviroSkyMgr.instance.useFlatClouds = false; // Enable flat clouds
        switch (scene.name) // Ensure the map name is case-insensitive
        {
            case "LibraryMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "OfficeMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "DesertMap":
                EnviroSkyMgr.instance.SetTimeOfDay(12f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "DungeonMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "PiratesMap":
                EnviroSkyMgr.instance.SetTimeOfDay(12f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(1); // weather preset for "cloud1" with ID 1
                break;

            case "VillageMap":
                EnviroSkyMgr.instance.SetTimeOfDay(12f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(1); // weather preset for "cloud1" with ID 1
                break;

            case "SwampMap":
                EnviroSkyMgr.instance.SetTimeOfDay(12f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(6); // weather preset for "foggy" with ID 6                                 
                EnviroSkyMgr.instance.useFlatClouds = true; // Enable flat clouds
                break;

            case "AsiaMap":
                EnviroSkyMgr.instance.SetTimeOfDay(17.5f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0                                
                break;

            case "CaveWaterMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "CaveMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;               

            case "MountainMap":
                EnviroSkyMgr.instance.SetTimeOfDay(9f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0       
                break;
                
            case "SpaceStationMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "SpaceBaseMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "ForestMap":
                EnviroSkyMgr.instance.SetTimeOfDay(12f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(4); // weather preset for "Clear" with ID 0
                break;

            case "HellMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "MeadowMap":
                EnviroSkyMgr.instance.SetTimeOfDay(9f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(4); // weather preset for "Clear" with ID 0
                break;

            case "CityMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "GraveYardMap":
                EnviroSkyMgr.instance.SetTimeOfDay(24f); // 24-hour format
                EnviroSkyMgr.instance.ChangeWeather(0); // weather preset for "Clear" with ID 0
                break;

            case "MainMenu":
                Destroy(gameObject);
                break;

            default:

                break;
        }
    }
}
