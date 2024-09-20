// Filename: TeleportManager.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script manages teleportation between different scenes in the game, handling fade effects and player positioning.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// This class manages teleportation mechanics, including fading effects and player positioning across different scenes.
/// </summary>
public class TeleportManager : MonoBehaviour
{
    public Image fadeImage;  // image Assign
    public List<Image> imageList;  // List of sprites to randomly pick from
    public float fadeDuration = 1.0f; // Duration of the fade effect

    public Transform player; // player GameObject
    public Transform playerConsole; // player GameObject console
    public MonoBehaviour characterController; // character controller script

    // Teleportation target settings
    public Vector3 currentTargetPosition = new Vector3(0, 0, 0);
    public Quaternion currentTargetRotation = Quaternion.Euler(0, -139.195f, 0); 

    public static TeleportManager Instance { get; private set; }
    private HandBookController handBookController;

    System.Random random = new System.Random();
    bool randomBool;
    bool hasFallen = false;

    private void OnDestroy()
    {
        if (Instance == this)
        {
            // This means this instance was the singleton and is now being destroyed
            Debug.Log("TeleportManager instance is being destroyed.");
            Instance = null;
        }
    }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            handBookController = FindObjectOfType<HandBookController>();
            // Ensure the fade image is fully transparent at the start
            //fadeImage.color = new Color(0, 0, 0, 0);

            // Teleport to the specified location if it exists
            if (BookLoader.Instance.location != "None" && BookLoader.Instance.location != "library")
            {
                TeleportTo(BookLoader.Instance.location);
            }
            else if(BookLoader.Instance.location == "library")
                return;
            else if (BookLoader.Instance.location2 != "None" && BookLoader.Instance.location != "library")
            {
                TeleportTo(BookLoader.Instance.location2);
            }
            else if(BookLoader.Instance.location == "library")
                return;
            else if (BookLoader.Instance.location3 != "None" && BookLoader.Instance.location != "library")
            {
                TeleportTo(BookLoader.Instance.location3);
            }
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
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        /* if (Input.GetKeyDown(KeyCode.Alpha0)) // 0 Key
         {
             TeleportTo("office");
         }
         if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 Key
         {
             TeleportTo("desert");
         }
         if (Input.GetKeyDown(KeyCode.Alpha2)) // 2 Key
         {
             TeleportTo("dungeon");
         }
         if (Input.GetKeyDown(KeyCode.Alpha3)) // 3 Key
         {
             TeleportTo("pirates");
         }
         if (Input.GetKeyDown(KeyCode.Alpha4)) // 4 Key
         {
             TeleportTo("village");
         }
         if (Input.GetKeyDown(KeyCode.Alpha5)) // 5 Key
         {
             TeleportTo("tavern");
         }
         if (Input.GetKeyDown(KeyCode.Alpha6)) // 6 Key
         {
             TeleportTo("swamp");
         }
         if (Input.GetKeyDown(KeyCode.Alpha7)) // 7 Key
         {
             TeleportTo("asia");
         }
         if (Input.GetKeyDown(KeyCode.Alpha8)) // 8 Key
         {
             TeleportTo("cave");
         }
         if (Input.GetKeyDown(KeyCode.Alpha9)) // 9 Key
         {
             TeleportTo("mountain");
         }

         // Special keys section (use shift for symbols)
         if (Input.GetKeyDown(KeyCode.Z)) // ! Key (Shift+1)
         {
             TeleportTo("space");
         }
         if (Input.GetKeyDown(KeyCode.X)) // @ Key (Shift+2)
         {
             TeleportTo("forest");
         }
         if (Input.GetKeyDown(KeyCode.C)) // # Key (Shift+3)
         {
             TeleportTo("hell");
         }
         if (Input.GetKeyDown(KeyCode.V)) // $ Key (Shift+4)
         {
             TeleportTo("meadow");
         }
         if (Input.GetKeyDown(KeyCode.B)) // % Key (Shift+5)
         {
             TeleportTo("city");
         }
         if (Input.GetKeyDown(KeyCode.N)) // ^ Key (Shift+6)
         {
             TeleportTo("graveyard");
         }
         if (Input.GetKeyDown(KeyCode.Ampersand)) // & Key (Shift+7)
         {
             // Add more teleport locations here if needed
         }
         if (Input.GetKeyDown(KeyCode.Asterisk)) // * Key (Shift+8)
         {
             // Add more teleport locations here if needed
         }
         if (Input.GetKeyDown(KeyCode.Q)) // ( Key (Shift+9)
         {

         }*/
        // Updates teleportation if player fell from map.
        if ((player.position.y <= -100 || playerConsole.position.y <= -100) && !hasFallen)
        {
            hasFallen = true; // Set falling state
            // Teleport player
            player.position = currentTargetPosition;
            player.rotation = currentTargetRotation;
            playerConsole.position = currentTargetPosition;
            playerConsole.rotation = currentTargetRotation;
            // Reset character controller
            if (characterController != null)
            {
                characterController.enabled = false;
                characterController.enabled = true;
            }
            hasFallen = false; // Reset falling state
        }
    }

    /// <summary>
    /// Teleports the player to a specified map.
    /// </summary>
    /// <param name="mapName">The name of the map to teleport to.</param>
    public void TeleportTo(string mapName)
    {
        //SetAllMapsOff(); // Deactivate all maps at the beginning
        if (BookLoader.Instance.sceneName.Contains(mapName, StringComparison.OrdinalIgnoreCase))
            return;
        if (Enviro.Instance != null)
            Enviro.Instance.gameObject.SetActive(true); // Activate environment manager

        // Teleport logic based on map name
        switch (mapName.ToLower()) // Ensure the map name is case-insensitive
        {
            case "library":
                fadeImage = imageList[0];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "GameWorld - Copy");
                break;

            case "office":
                fadeImage = imageList[1];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -139.195f, 0), "OfficeMap");
                break;

            case "desert":
                fadeImage = imageList[2];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "DesertMap");
                break;
                    
            case "dungeon":
                fadeImage = imageList[3];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, 148.214f, 0), "DungeonMap");
                break;

            case "pirates":
                fadeImage = imageList[4];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, 26.804f, 0), "PiratesMap");
                break;

            case "village":
                fadeImage = imageList[5];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -25.694f, 0), "VillageMap");
                break;

            case "tavern":
                fadeImage = imageList[5];
                TeleportToMap(new Vector3(-76.636f, -1.625f, 59.129f), Quaternion.Euler(0, -186.16f, 0), "VillageMap");
                break;

            case "swamp":
                fadeImage = imageList[6];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "SwampMap");
                break;

            case "asia":
                fadeImage = imageList[7];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -86.6f, 0), "AsiaMap");
                break;

            case "cave":
                randomBool = random.Next(2) == 1;
                if (randomBool)
                {
                    fadeImage = imageList[8];
                    TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -127.179f, 0), "CaveWaterMap");
                }
                else
                {
                    fadeImage = imageList[9];
                    TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -187.447f, 0), "CaveMap");
                } 
                break;

            case "mountain":
                fadeImage = imageList[10];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, 130.5f, 0), "MountainMap");
                break;

            case "space":
                fadeImage = imageList[11];
                randomBool = random.Next(2) == 1;
                if (randomBool)
                    TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -111.2f, 0), "SpaceStationMap");
                else
                    TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -84.153f, 0), "SpaceBaseMap");
                break;

            case "forest":
                fadeImage = imageList[12];
                randomBool = random.Next(2) == 1;
                if (randomBool)
                    TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, 17.097f, 0), "ForestMap");
                else
                {
                    randomBool = random.Next(2) == 1;
                    if (randomBool)
                        TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, 118.84f, 0), "ForestMap1");
                    else
                        TeleportToMap(new Vector3(201.223f, 17.744f, -26.015f), Quaternion.Euler(0, 253.966f, 0), "ForestMap1");
                }
                break;

            case "hell":
                fadeImage = imageList[13];
                randomBool = random.Next(2) == 1;
                if (randomBool)
                    TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -82, 0), "HellMap");
                else
                    TeleportToMap(new Vector3(160.61f, -2.33f, 6.331f), Quaternion.Euler(0, -215.53f, 0), "HellMap");
                break;

            case "meadow":
                fadeImage = imageList[14];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -18.739f, 0), "MeadowMap");
                break;

            case "city":
                fadeImage = imageList[15];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -131.177f, 0), "CityMap");
                break;

            case "graveyard":
                fadeImage = imageList[16];
                TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, -241.473f, 0), "GraveYardMap");
                break;

            default:
                fadeImage = imageList[0];
                Debug.Log("Map name not recognized: " + mapName);
                //TeleportToMap(new Vector3(0, 0, 0), Quaternion.Euler(0, 113.526f, 0));
                break;
        }
    }

    /// <summary>
    /// Teleports the player to a specified position and rotation on the target map.
    /// </summary>
    /// <param name="targetPosition">The target position for the player.</param>
    /// <param name="targetRotation">The target rotation for the player.</param>
    /// <param name="mapname">The name of the map to teleport to.</param>
    public void TeleportToMap(Vector3 targetPosition, Quaternion targetRotation, string mapname)
    {
        StartCoroutine(TeleportCoroutine(targetPosition, targetRotation, mapname));
    }

    private IEnumerator TeleportCoroutine(Vector3 targetPosition, Quaternion targetRotation, string mapname)
    {
        // Start fade to black with a random image
        yield return StartCoroutine(FadeToBlack());

        if (mapname != "")
        {
            // Ensure that the previous scene has been completely unloaded before loading the new one
            yield return StartCoroutine(LoadSceneWithUnload(mapname));
        }

            

        // Teleport the player
        player.position = targetPosition;
        player.rotation = targetRotation;
        playerConsole.position = targetPosition;
        playerConsole.rotation = targetRotation;

        // Start fade back in
        yield return StartCoroutine(FadeToClear());
    }

    /// <summary>
    /// Unloads the current scene and loads a new one.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <returns>IEnumerator for the coroutine.</returns>
    IEnumerator LoadSceneWithUnload(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        yield return Resources.UnloadUnusedAssets();
        yield return SceneManager.LoadSceneAsync(sceneName);
    }


    /// <summary>
    /// Sets a random image for the fade effect and starts fading to black.
    /// </summary>
    public void SetRandomImageAndFadeToBlack()
    {
        // Choose a random image from the list
        Image randomImage = imageList[UnityEngine.Random.Range(0, imageList.Count)];
        fadeImage = randomImage;  // Set the image
        StartCoroutine(FadeToBlack());
    }

    /// <summary>
    /// Fades the screen to black.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator FadeToBlack()
    {
        float timer = 0;

        // Perform the fade to black
        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeImage.color = new Color(255, 255, 255, alpha);
            yield return null;
        }

        fadeImage.color = new Color(255, 255, 255, 1);
    }

    /// <summary>
    /// Fades the screen to clear.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator FadeToClear()
    {
        float timer = 0;

        // Perform the fade to clear
        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadeImage.color = new Color(255, 255, 255, alpha);
            yield return null;
        }

        fadeImage.color = new Color(255, 255, 255, 0);
    }
}
