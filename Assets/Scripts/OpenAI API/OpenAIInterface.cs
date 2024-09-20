// Filename: OpenAIInterface.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class interfaces with the OpenAI API, handling narrative submissions, image generations, and managing player-specific data.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.Net.Http;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

// Serializable classes for API responses
[System.Serializable]
public class APIResponse { public string id; }
[System.Serializable]
public class ThreadMessageResponse { public List<ThreadMessageData> data; }
[System.Serializable]
public class ThreadMessageData { public string id; public string role; public List<ThreadMessageContent> content; }
[System.Serializable]
public class ThreadMessageContent { public ThreadMessageText text; }
[System.Serializable]
public class ThreadMessageText { public string value; }
[System.Serializable]
public class ImageResponse { public List<ImageData> data; }
[System.Serializable]
public class ImageData { public string url; }
[System.Serializable]
public class MessageData { public string role; public string content; }
[System.Serializable]
public class RunsData { public string assistant_id; }
[System.Serializable]
public class ImageGenerationRequest { public string model; public string prompt; public int n; public string size; }
[System.Serializable]
public class ConfigData { public string openAIKey; public string assistantID; }
[System.Serializable]
public class AssistantResponse { public string id; }
[System.Serializable]
public class RollOption { public string description; public string effect; }

/// <summary>
/// This class manages interactions with the OpenAI API for narrative and image generation.
/// </summary>
public class OpenAIInterface : MonoBehaviour
{
    public static OpenAIInterface Instance { get; internal set; } // Singleton instance

    private string user_APIKey = null; // Player's API key
    private string assistant_ID = null; // Assistant ID
    private string apiBaseUrl = "https://api.openai.com/v1/threads"; // Base URL for API requests
    private string game_APIThread; // Current game thread ID

    public string current_model = "dall-e-2"; // Current model for image generation
    public string current_size = "1024x1024"; // Current image size

    public int current_Page = 0; // Current page number
    public string current_Narrative; // Current narrative text
    public string current_BookName; // Current book name

    private bool _isEnded; // Flag for adventure end status
    private bool _isConclusionSaved; // Flag for conclusion save status

    public event System.Action<bool> OnIsEndedChanged; // Event for end status changes
    public event System.Action<bool> OnConclusionSave; // Event for conclusion save status changes

    // Properties to manage end and conclusion status
    public bool is_ended
    {
        get { return _isEnded; }
        set
        {
            if (_isEnded != value)
            {
                _isEnded = value;
                OnIsEndedChanged?.Invoke(_isEnded);
            }
        }
    }

    public bool is_ConclusionSaved
    {
        get { return _isConclusionSaved; }
        set
        {
            if (_isConclusionSaved != value)
            {
                _isConclusionSaved = value;
                OnConclusionSave?.Invoke(_isConclusionSaved);
            }
        }
    }

    public string AssistantID
    {
        get { return assistant_ID; }
        set { assistant_ID = value; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set this instance as the singleton
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
            Debug.Log("OpenAIInterface instance initialized.");
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    /// <summary>
    /// Loads the player's API keys from the session.
    /// </summary>
    public void LoadPlayerAPIKeys()
    {
        this.user_APIKey = PlayerSession.SelectedPlayerApiKey; // Get the user's API key
        this.assistant_ID = PlayerSession.SelectedPlayerassistantID; // Get the assistant ID
    }

    /// <summary>
    /// Sends a narrative to the OpenAI API.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="narrative">The narrative text to send.</param>
    /// <param name="pagenum">The current page number.</param>
    public void SendNarrativeToAPI(string bookName, string narrative, int pagenum)
    {
        Debug.Log($"SendNarrativeToAPI called with bookName: {bookName}, narrative: {narrative}");
        this.current_Page = pagenum; // Update current page
        this.current_Narrative = narrative; // Update current narrative
        this.current_BookName = bookName; // Update current book name
        StartCoroutine(SendNarrativeCoroutine(bookName, narrative, true)); // Start coroutine to send narrative
    }

    /// <summary>
    /// Coroutine to send narrative data to the API.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="narrative">The narrative text.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    private IEnumerator SendNarrativeCoroutine(string bookName, string narrative, bool isNewBook)
    {
        Debug.Log("SendNarrativeCoroutine started.");
        var bookData = new
        {
            page = new
            {
                book_name = bookName,
                narrative = narrative
            }
        };

        string json = JsonUtility.ToJson(bookData, true); // Convert book data to JSON
        Debug.Log("SendNarrative JSON: " + json);

        yield return SendWebRequestCoroutine(apiBaseUrl, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
                ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
            }
            else
            {
                var response = JsonUtility.FromJson<APIResponse>(request.downloadHandler.text);
                game_APIThread = response.id; // Get the thread ID from the response
                Debug.Log($"API Thread ID: {game_APIThread}");
                SendMessageToThread(narrative, bookName, isNewBook); // Send the message to the thread
            }
        });
    }

    /// <summary>
    /// Sends a message to the API thread.
    /// </summary>
    /// <param name="narrative">The narrative text.</param>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    private void SendMessageToThread(string narrative, string bookName, bool isNewBook)
    {
        StartCoroutine(SendMessageCoroutine(narrative, bookName, isNewBook));
    }

    /// <summary>
    /// Coroutine to send a message to the thread.
    /// </summary>
    /// <param name="narrative">The narrative text.</param>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    private IEnumerator SendMessageCoroutine(string narrative, string bookName, bool isNewBook)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/messages"; // Build URL for messages
        var messageData = new MessageData
        {
            role = "user",
            content = narrative // Set the message content
        };

        string json = JsonUtility.ToJson(messageData); // Convert message data to JSON
        Debug.Log("SendMessage JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
                ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
            }
            else
            {
                RunThread(bookName, isNewBook); // Run the thread with the book name
            }
        });
    }

    /// <summary>
    /// Runs the thread after sending a message.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    private void RunThread(string bookName, bool isNewBook)
    {
        StartCoroutine(RunThreadCoroutine(bookName, isNewBook));
    }

    /// <summary>
    /// Coroutine to run the thread.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    private IEnumerator RunThreadCoroutine(string bookName, bool isNewBook)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/runs"; // Build URL for runs
        var runData = new RunsData
        {
            assistant_id = $"{assistant_ID}" // Set the assistant ID
        };

        string json = JsonUtility.ToJson(runData); // Convert run data to JSON
        Debug.Log("RunThread JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
                ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
            }
            else
            {
                GetMessageResponse(bookName, isNewBook); // Get message response
            }
        });
    }

    /// <summary>
    /// Gets the message response from the thread.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    private void GetMessageResponse(string bookName, bool isNewBook)
    {
        StartCoroutine(GetMessageResponseCoroutine(bookName, isNewBook));
    }

    /// <summary>
    /// Coroutine to retrieve the message response.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    private IEnumerator GetMessageResponseCoroutine(string bookName, bool isNewBook)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/messages?limit=1"; // Build URL for messages with limit
        int maxAttempts = 10; // Maximum attempts to get a response
        int attempt = 0;
        bool success = false;

        while (attempt < maxAttempts && !success)
        {
            yield return SendWebRequestCoroutine(url, "GET", null, (request) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                    Debug.LogError("Response Code: " + request.responseCode);
                    Debug.LogError("Response Text: " + request.downloadHandler.text);
                    ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
                }
                else
                {
                    string responseText = request.downloadHandler.text; // Get response text
                    Debug.Log("Response Text: " + responseText);

                    var response = JsonUtility.FromJson<ThreadMessageResponse>(responseText);
                    if (response.data != null && response.data.Count > 0)
                    {
                        var messageData = response.data[0];
                        if (messageData.content != null && messageData.content.Count > 0)
                        {
                            var contentData = messageData.content[0];
                            if (contentData.text != null && !string.IsNullOrEmpty(contentData.text.value))
                            {
                                var messageContent = contentData.text.value; // Get message content
                                Debug.Log("Received Message Content: " + messageContent);
                                bool isConc = current_Page == 11; // Check if it's the conclusion
                                string imageDescription = Parser.Instance.ExtractImageDescription(messageContent, isConc);
                                Debug.Log("imageDescription Message Content: " + imageDescription);
                                if (!string.IsNullOrEmpty(imageDescription))
                                {
                                    SendDescriptionToDalle(imageDescription, messageContent, bookName, isNewBook, isConc); // Send description to DALL-E
                                    success = true; // Set success flag
                                }
                            }
                        }
                    }
                }
            });

            if (!success)
            {
                Debug.LogWarning("No valid message content received. Retrying...");
                yield return new WaitForSeconds(3); // Wait before retrying
                attempt++;
            }
        }

        if (!success)
        {
            Debug.LogError("Failed to get a valid response after multiple attempts. Please try again later.");
            ErrorHandler.Instance.ErrorAccured("Failed to get a valid response after multiple attempts. Please try again later.", "", "");
        }
    }

    /// <summary>
    /// Sends a description to DALL-E for image generation.
    /// </summary>
    /// <param name="description">The image description.</param>
    /// <param name="pageText">The text for the page.</param>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    /// <param name="isConc">Indicates if it's a conclusion.</param>
    private void SendDescriptionToDalle(string description, string pageText, string bookName, bool isNewBook, bool isConc)
    {
        Debug.Log($"Sending description to DALL-E: {description}");
        if (string.IsNullOrEmpty(description))
        {
            Debug.LogError("Description is empty. Cannot send to DALL-E.");
            ErrorHandler.Instance.ErrorAccured("Description is empty. Cannot send to DALL-E.", "", "");
            return;
        }

        StartCoroutine(SendDescriptionToDalleCoroutine(description, pageText, bookName, isNewBook, isConc));
    }

    /// <summary>
    /// Coroutine to send a description to DALL-E.
    /// </summary>
    /// <param name="description">The image description.</param>
    /// <param name="pageText">The text for the page.</param>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    /// <param name="isConc">Indicates if it's a conclusion.</param>
    private IEnumerator SendDescriptionToDalleCoroutine(string description, string pageText, string bookName, bool isNewBook, bool isConc)
    {
        string url = "https://api.openai.com/v1/images/generations"; // URL for image generations
        var imageRequest = new ImageGenerationRequest
        {
            model = this.current_model,
            prompt = description + "detailed, photorealistic", // Prompt for DALL-E
            n = 1,
            size = this.current_size // Size for the generated image
        };

        string json = JsonUtility.ToJson(imageRequest); // Convert request to JSON
        Debug.Log("SendDescriptionToDalle JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
                ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
            }
            else
            {
                var response = JsonUtility.FromJson<ImageResponse>(request.downloadHandler.text);
                string imageUrl = response.data[0].url; // Get the image URL
                StartCoroutine(DownloadImageCoroutine(imageUrl, pageText, bookName, isNewBook, isConc)); // Download the generated image
            }
        });
    }

    /// <summary>
    /// Coroutine to download the generated image from a URL.
    /// </summary>
    /// <param name="url">The URL of the image to download.</param>
    /// <param name="pageText">The text for the page.</param>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isNewBook">Indicates if it's a new book.</param>
    /// <param name="isConc">Indicates if it's a conclusion.</param>
    private IEnumerator DownloadImageCoroutine(string url, string pageText, string bookName, bool isNewBook, bool isConc)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url)) // Create a request for the texture
        {
            yield return request.SendWebRequest(); // Send the request and wait for a response

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
                ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture; // Get the downloaded texture
                byte[] imageBytes = texture.EncodeToPNG(); // Encode texture to PNG format
                string imagePath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, bookName, $"page{this.current_Page}_image.png"); // Create file path
                File.WriteAllBytes(imagePath, imageBytes); // Write image to file

                string bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, bookName);
                DataManager.CreateDirectoryIfNotExists(bookFolderPath); // Ensure the directory exists

                string bookFilePath = Path.Combine(bookFolderPath, "bookData.json");
                Book bookData;
                if (File.Exists(bookFilePath))
                {
                    string bookJson = File.ReadAllText(bookFilePath);
                    bookData = JsonUtility.FromJson<Book>(bookJson); // Load existing book data
                }
                else
                {
                    Debug.LogError("Book file not found when attempting to add a new page.");
                    ErrorHandler.Instance.ErrorAccured("Book file not found when attempting to add a new page.", "", "");
                    yield break;
                }

                // Parse the narrative into the structured format
                var page = new Page();
                if (isConc)
                    page = Parser.Instance.ParseConclusion(pageText, imagePath); // Parse as conclusion
                else
                    page = Parser.Instance.ParsePage(pageText, imagePath); // Parse as regular page

                if (page != null)
                {
                    bookData.Pages.Add(page); // Add the page to the book
                    string json = JsonUtility.ToJson(bookData, true); // Convert book data to JSON
                    Debug.Log("Final JSON: " + json);
                    File.WriteAllText(bookFilePath, json); // Save updated book data
                    if (isConc)
                    {
                        this.is_ConclusionSaved = true; // Set conclusion saved flag
                        this.is_ConclusionSaved = false; // Reset flag for next use
                    }
                    else
                    {
                        this.is_ended = true; // Set adventure ended flag
                        this.is_ended = false; // Reset flag for next use
                        PlayerSession.SelectedBookName = bookName; // Set the current book name
                        SceneManager.LoadScene("GameWorld - Copy"); // Load game world scene
                        Cursor.visible = false; // Hide cursor
                        Cursor.lockState = CursorLockMode.Locked; // Lock cursor state
                    }
                }
            }
        }
    }

    /// <summary>
    /// Coroutine to send a web request to the OpenAI API.
    /// </summary>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="method">The HTTP method (GET, POST, etc.).</param>
    /// <param name="json">The JSON body for the request.</param>
    /// <param name="callback">The callback function to handle the response.</param>
    public virtual IEnumerator SendWebRequestCoroutine(string url, string method, string json, System.Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method)) // Create web request
        {
            if (json != null)
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json); // Convert JSON to byte array
                request.uploadHandler = new UploadHandlerRaw(bodyRaw); // Set upload handler
            }
            request.downloadHandler = new DownloadHandlerBuffer(); // Set download handler
            request.SetRequestHeader("Content-Type", "application/json"); // Set content type
            request.SetRequestHeader("Authorization", $"Bearer {user_APIKey}"); // Set authorization header
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2"); // Set additional header

            yield return request.SendWebRequest(); // Send the request and wait for response
            callback(request); // Call the callback with the request
        }
    }

    /// <summary>
    /// Sends a message to an existing book.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="narrative">The narrative text.</param>
    /// <param name="pagenum">The current page number (optional).</param>
    public void SendMessageToExistingBook(string bookName, string narrative, int pagenum = -1)
    {
        if (pagenum == -1)
            this.current_Page += 1; // Increment current page
        else
            this.current_Page = pagenum; // Set to specified page number
        this.current_Narrative = narrative; // Update current narrative
        Debug.Log($"SendMessageToExistingBook called with bookName: {bookName}, narrative: {this.current_Narrative}, page: {this.current_Page}");
        StartCoroutine(SendMessageCoroutineForExistingBook(bookName, narrative)); // Start coroutine to send message
    }

    /// <summary>
    /// Coroutine to send a message to an existing book.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="narrative">The narrative text.</param>
    private IEnumerator SendMessageCoroutineForExistingBook(string bookName, string narrative)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/messages"; // Build URL for messages
        var messageData = new MessageData
        {
            role = "user",
            content = narrative // Set message content
        };

        string json = JsonUtility.ToJson(messageData); // Convert message data to JSON
        Debug.Log("SendMessage JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
                ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
            }
            else
            {
                RunThreadForExistingBook(bookName); // Run the thread for the existing book
            }
        });
    }

    /// <summary>
    /// Runs the thread for an existing book.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    private void RunThreadForExistingBook(string bookName)
    {
        StartCoroutine(RunThreadCoroutineForExistingBook(bookName)); // Start coroutine to run thread
    }

    /// <summary>
    /// Coroutine to run the thread for an existing book.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    private IEnumerator RunThreadCoroutineForExistingBook(string bookName)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/runs"; // Build URL for runs
        var runData = new RunsData
        {
            assistant_id = $"{assistant_ID}" // Set assistant ID
        };

        string json = JsonUtility.ToJson(runData); // Convert run data to JSON
        Debug.Log("RunThread JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
                ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
            }
            else
            {
                GetMessageResponseForExistingBook(bookName); // Get message response for existing book
            }
        });
    }

    /// <summary>
    /// Gets the message response for an existing book.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    private void GetMessageResponseForExistingBook(string bookName)
    {
        StartCoroutine(GetMessageResponseCoroutineForExistingBook(bookName)); // Start coroutine to get message response
    }

    /// <summary>
    /// Coroutine to retrieve the message response for an existing book.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    private IEnumerator GetMessageResponseCoroutineForExistingBook(string bookName)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/messages?limit=1"; // Build URL for messages with limit
        int maxAttempts = 10; // Maximum attempts to get a response
        int attempt = 0;
        bool success = false;

        while (attempt < maxAttempts && !success)
        {
            yield return SendWebRequestCoroutine(url, "GET", null, (request) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                    Debug.LogError("Response Code: " + request.responseCode);
                    Debug.LogError("Response Text: " + request.downloadHandler.text);
                    ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
                }
                else
                {
                    string responseText = request.downloadHandler.text; // Get response text
                    Debug.Log("Response Text: " + responseText);

                    var response = JsonUtility.FromJson<ThreadMessageResponse>(responseText);
                    if (response.data != null && response.data.Count > 0)
                    {
                        var messageData = response.data[0];
                        if (messageData.content != null && messageData.content.Count > 0)
                        {
                            var contentData = messageData.content[0];
                            if (contentData.text != null && !string.IsNullOrEmpty(contentData.text.value))
                            {
                                var messageContent = contentData.text.value; // Get message content
                                Debug.Log("Received Message Content: " + messageContent);
                                bool isConc = current_Page == 11; // Check if it's the conclusion
                                string imageDescription = Parser.Instance.ExtractImageDescription(messageContent, isConc);
                                Debug.Log("imageDescription Message Content: " + imageDescription);
                                if (!string.IsNullOrEmpty(imageDescription))
                                {
                                    SendDescriptionToDalleForExistingBook(imageDescription, messageContent, bookName, isConc); // Send description to DALL-E
                                    success = true; // Set success flag
                                }
                            }
                        }
                    }
                }
            });

            if (!success)
            {
                Debug.LogWarning("No valid message content received. Retrying...");
                yield return new WaitForSeconds(3); // Wait before retrying
                attempt++;
            }
        }

        if (!success)
        {
            Debug.LogError("Failed to get a valid response after multiple attempts. Please try again later.");
            ErrorHandler.Instance.ErrorAccured("Failed to get a valid response after multiple attempts. Please try again later.", "", "");
        }
    }

    /// <summary>
    /// Sends a description to DALL-E for image generation for an existing book.
    /// </summary>
    /// <param name="description">The image description.</param>
    /// <param name="pageText">The text for the page.</param>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isConc">Indicates if it's a conclusion.</param>
    private void SendDescriptionToDalleForExistingBook(string description, string pageText, string bookName, bool isConc)
    {
        Debug.Log($"Sending description to DALL-E: {description}");
        if (string.IsNullOrEmpty(description))
        {
            Debug.LogError("Description is empty. Cannot send to DALL-E.");
            ErrorHandler.Instance.ErrorAccured("Description is empty. Cannot send to DALL-E.", "", "");
            return;
        }

        StartCoroutine(SendDescriptionToDalleCoroutineForExistingBook(description, pageText, bookName, isConc)); // Start coroutine to send description
    }

    /// <summary>
    /// Coroutine to send a description to DALL-E for an existing book.
    /// </summary>
    /// <param name="description">The image description.</param>
    /// <param name="pageText">The text for the page.</param>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isConc">Indicates if it's a conclusion.</param>
    private IEnumerator SendDescriptionToDalleCoroutineForExistingBook(string description, string pageText, string bookName, bool isConc)
    {
        string url = "https://api.openai.com/v1/images/generations"; // URL for image generations
        var imageRequest = new ImageGenerationRequest
        {
            model = this.current_model,
            prompt = description + "detailed, photorealistic", // Prompt for DALL-E
            n = 1,
            size = this.current_size // Size for the generated image
        };

        string json = JsonUtility.ToJson(imageRequest); // Convert request to JSON
        Debug.Log("SendDescriptionToDalle JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
                ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
            }
            else
            {
                var response = JsonUtility.FromJson<ImageResponse>(request.downloadHandler.text);
                string imageUrl = response.data[0].url; // Get the image URL
                StartCoroutine(DownloadImageCoroutineForExistingBook(imageUrl, pageText, bookName, isConc)); // Download the generated image
            }
        });
    }

    /// <summary>
    /// Coroutine to download the generated image from a URL for an existing book.
    /// </summary>
    /// <param name="url">The URL of the image to download.</param>
    /// <param name="pageText">The text for the page.</param>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="isConc">Indicates if it's a conclusion.</param>
    private IEnumerator DownloadImageCoroutineForExistingBook(string url, string pageText, string bookName, bool isConc)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url)) // Create a request for the texture
        {
            yield return request.SendWebRequest(); // Send the request and wait for a response

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
                ErrorHandler.Instance.ErrorAccured(request.error, "Response Code: " + request.responseCode, "Response Text: " + request.downloadHandler.text);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture; // Get the downloaded texture
                byte[] imageBytes = texture.EncodeToPNG(); // Encode texture to PNG format
                string imagePath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, bookName, $"page{this.current_Page}_image.png"); // Create file path
                File.WriteAllBytes(imagePath, imageBytes); // Write image to file

                string bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, bookName);
                DataManager.CreateDirectoryIfNotExists(bookFolderPath); // Ensure the directory exists

                string bookFilePath = Path.Combine(bookFolderPath, "bookData.json");
                Book bookData;
                if (File.Exists(bookFilePath))
                {
                    string bookJson = File.ReadAllText(bookFilePath);
                    bookData = JsonUtility.FromJson<Book>(bookJson); // Load existing book data
                }
                else
                {
                    Debug.LogError("Book file not found when attempting to add a new page.");
                    ErrorHandler.Instance.ErrorAccured("Book file not found when attempting to add a new page.", "", "");
                    yield break;
                }

                // Parse the narrative into the structured format
                var page = new Page();
                if (isConc)
                    page = Parser.Instance.ParseConclusion(pageText, imagePath); // Parse as conclusion
                else
                    page = Parser.Instance.ParsePage(pageText, imagePath); // Parse as regular page

                if (page != null)
                {
                    bookData.Pages.Add(page); // Add the page to the book
                    string json = JsonUtility.ToJson(bookData, true); // Convert book data to JSON
                    Debug.Log("Final JSON: " + json);
                    File.WriteAllText(bookFilePath, json); // Save updated book data
                    if (isConc)
                    {
                        this.is_ConclusionSaved = true; // Set conclusion saved flag
                        this.is_ConclusionSaved = false; // Reset flag for next use
                    }
                    else
                    {
                        this.is_ended = true; // Set adventure ended flag
                        this.is_ended = false; // Reset flag for next use
                    }
                }
            }
        }
    }

    /// <summary>
    /// Coroutine to create an API assistant.
    /// </summary>
    /// <param name="apiKey">The API key for OpenAI.</param>
    /// <param name="callback">The callback function to handle the response.</param>
    public virtual IEnumerator CreateAPI_Assistant(string apiKey, System.Action<bool, string> callback)
    {
        string configPath = Path.Combine(Application.dataPath, "Scripts/OpenAI API/assistant_create.json");
        string jsonContent = File.ReadAllText(configPath);
        var requestBody = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var request = new UnityWebRequest("https://api.openai.com/v1/assistants", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonContent);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response Code: " + request.responseCode);
            Debug.LogError("Response Text: " + request.downloadHandler.text);
            callback(false, apiKey);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            AssistantResponse assistantResponse = JsonUtility.FromJson<AssistantResponse>(request.downloadHandler.text);
            this.assistant_ID = assistantResponse.id; // Set assistant ID from response
            Debug.Log(this.assistant_ID);
            callback(true, apiKey);
        }
    }
}
