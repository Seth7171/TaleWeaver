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

[System.Serializable]
public class APIResponse
{
    public string id;
}

[System.Serializable]
public class ThreadMessageResponse
{
    public List<ThreadMessageData> data;
}

[System.Serializable]
public class ThreadMessageData
{
    public string id;
    public string role;
    public List<ThreadMessageContent> content;
}

[System.Serializable]
public class ThreadMessageContent
{
    public ThreadMessageText text;
}

[System.Serializable]
public class ThreadMessageText
{
    public string value;
}

[System.Serializable]
public class ImageResponse
{
    public List<ImageData> data;
}

[System.Serializable]
public class ImageData
{
    public string url;
}

[System.Serializable]
public class MessageData
{
    public string role;
    public string content;
}

[System.Serializable]
public class RunsData
{
    public string assistant_id;
}

[System.Serializable]
public class ImageGenerationRequest
{
    public string prompt;
    public int n;
    public string size;
}

[System.Serializable]
public class ConfigData
{
    public string openAIKey;
    public string assistantID;
}

[System.Serializable]
public class AssistantResponse
{
    public string id;
}

[System.Serializable]
public class RollOption
{
    public string description;
    public string effect;
}


public class OpenAIInterface : MonoBehaviour
{
    public static OpenAIInterface Instance { get; private set; }
    
    private string user_APIKey = null;
    private string assistant_ID = null;
    private string apiBaseUrl = "https://api.openai.com/v1/threads";
    private string game_APIThread;
    public int current_Page = 0;
    public string current_Narrative;
    public string current_BookName;
    private bool _isEnded;
    private bool _isConclusionSaved;

    public event System.Action<bool> OnIsEndedChanged;
    public event System.Action<bool> OnConclusionSave;

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
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("OpenAIInterface instance initialized.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadPlayerAPIKeys()
    {
        this.user_APIKey = PlayerSession.SelectedPlayerApiKey;
        this.assistant_ID = PlayerSession.SelectedPlayerassistantID;
    }

    public void SendNarrativeToAPI(string bookName, string narrative, int pagenum)
    {
        Debug.Log($"SendNarrativeToAPI called with bookName: {bookName}, narrative: {narrative}");
        this.current_Page = pagenum;
        this.current_Narrative = narrative;
        this.current_BookName = bookName;
        StartCoroutine(SendNarrativeCoroutine(bookName, narrative, true));
    }

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

        string json = JsonUtility.ToJson(bookData, true);
        Debug.Log("SendNarrative JSON: " + json);

        yield return SendWebRequestCoroutine(apiBaseUrl, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
            }
            else
            {
                var response = JsonUtility.FromJson<APIResponse>(request.downloadHandler.text);
                game_APIThread = response.id;
                Debug.Log($"API Thread ID: {game_APIThread}");
                SendMessageToThread(narrative, bookName, isNewBook);
            }
        });
    }

    private void SendMessageToThread(string narrative, string bookName, bool isNewBook)
    {
        StartCoroutine(SendMessageCoroutine(narrative, bookName, isNewBook));
    }

    private IEnumerator SendMessageCoroutine(string narrative, string bookName, bool isNewBook)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/messages";
        var messageData = new MessageData
        {
            role = "user",
            content = narrative
        };

        string json = JsonUtility.ToJson(messageData);
        Debug.Log("SendMessage JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
            }
            else
            {
                RunThread(bookName, isNewBook);
            }
        });
    }

    private void RunThread(string bookName, bool isNewBook)
    {
        StartCoroutine(RunThreadCoroutine(bookName, isNewBook));
    }

    private IEnumerator RunThreadCoroutine(string bookName, bool isNewBook)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/runs";
        var runData = new RunsData
        {
            assistant_id = $"{assistant_ID}"
        };

        string json = JsonUtility.ToJson(runData);
        Debug.Log("RunThread JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
            }
            else
            {
                GetMessageResponse(bookName, isNewBook);
            }
        });
    }

    private void GetMessageResponse(string bookName, bool isNewBook)
    {
        StartCoroutine(GetMessageResponseCoroutine(bookName, isNewBook));
    }

    private IEnumerator GetMessageResponseCoroutine(string bookName, bool isNewBook)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/messages?limit=1";
        int maxAttempts = 10;
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
                }
                else
                {
                    string responseText = request.downloadHandler.text;
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
                                var messageContent = contentData.text.value;
                                Debug.Log("Received Message Content: " + messageContent);
                                bool isConc = false;
                                if (current_Page == 11)
                                {
                                    isConc = true;
                                }
                                string imageDescription = Parser.Instance.ExtractImageDescription(messageContent, isConc);
                                Debug.Log("imageDescription Message Content: " + imageDescription);
                                if (!string.IsNullOrEmpty(imageDescription))
                                {
                                    SendDescriptionToDalle(imageDescription, messageContent, bookName, isNewBook, isConc);
                                    success = true;
                                }
                            }
                        }
                    }
                }
            });

            if (!success)
            {
                Debug.LogWarning("No valid message content received. Retrying...");
                yield return new WaitForSeconds(3);
                attempt++;
            }
        }

        if (!success)
        {
            Debug.LogError("Failed to get a valid response after multiple attempts. Please try again later.");
            // Notify the player here
        }
    }


    private void SendDescriptionToDalle(string description, string pageText, string bookName, bool isNewBook, bool isConc)
    {
        Debug.Log($"Sending description to DALL-E: {description}");
        if (string.IsNullOrEmpty(description))
        {
            Debug.LogError("Description is empty. Cannot send to DALL-E.");
            return;
        }

        StartCoroutine(SendDescriptionToDalleCoroutine(description, pageText, bookName, isNewBook, isConc));
    }

    private IEnumerator SendDescriptionToDalleCoroutine(string description, string pageText, string bookName, bool isNewBook, bool isConc)
    {
        string url = "https://api.openai.com/v1/images/generations";
        var imageRequest = new ImageGenerationRequest
        {
            prompt = description,
            n = 1,
            size = "1024x1024"
        };

        string json = JsonUtility.ToJson(imageRequest);
        Debug.Log("SendDescriptionToDalle JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
            }
            else
            {
                var response = JsonUtility.FromJson<ImageResponse>(request.downloadHandler.text);
                string imageUrl = response.data[0].url;
                StartCoroutine(DownloadImageCoroutine(imageUrl, pageText, bookName, isNewBook, isConc));
            }
        });
    }

    private IEnumerator DownloadImageCoroutine(string url, string pageText, string bookName, bool isNewBook, bool isConc)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                byte[] imageBytes = texture.EncodeToPNG();
                string imagePath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, bookName, $"page{this.current_Page}_image.png");
                File.WriteAllBytes(imagePath, imageBytes);

                string bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, bookName);
                DataManager.CreateDirectoryIfNotExists(bookFolderPath);

                string bookFilePath = Path.Combine(bookFolderPath, "bookData.json");
                Book bookData;
                if (File.Exists(bookFilePath))
                {
                    string bookJson = File.ReadAllText(bookFilePath);
                    bookData = JsonUtility.FromJson<Book>(bookJson);
                }
                else
                {
                    if (isNewBook)
                    {
                        bookData = new Book(bookName, this.current_Narrative);
                    }
                    else
                    {
                        Debug.LogError("Book file not found when attempting to add a new page.");
                        yield break;
                    }
                }

                // Parse the narrative into the structured format
                var page = new Page();
                if (isConc)
                    page = Parser.Instance.ParseConclusion(pageText, imagePath);
                else
                    page = Parser.Instance.ParsePage(pageText, imagePath);

                if (page != null)
                {
                    bookData.Pages.Add(page);
                    string json = JsonUtility.ToJson(bookData, true);
                    Debug.Log("Final JSON: " + json);
                    File.WriteAllText(bookFilePath, json);
                    if (isConc)
                    {
                        this.is_ConclusionSaved = true;
                        this.is_ConclusionSaved = false;
                    }
                    else
                    {
                        this.is_ended = true;
                        this.is_ended = false;
                        PlayerSession.SelectedBookName = bookName;
                        SceneManager.LoadScene("GameWorld");
                    }
                }
            }
        }
    }

    private IEnumerator SendWebRequestCoroutine(string url, string method, string json, System.Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (json != null)
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {user_APIKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            yield return request.SendWebRequest();
            callback(request);
        }
    }

    public void SendMessageToExistingBook(string bookName, string narrative, int pagenum = -1)
    {
        if (pagenum == -1)
            this.current_Page += 1;
        else
            this.current_Page = pagenum;
        this.current_Narrative = narrative;
        Debug.Log($"SendMessageToExistingBook called with bookName: {bookName}, narrative: {this.current_Narrative}, page: {this.current_Page}");
        StartCoroutine(SendMessageCoroutineForExistingBook(bookName, narrative));
    }

    private IEnumerator SendMessageCoroutineForExistingBook(string bookName, string narrative)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/messages";
        var messageData = new MessageData
        {
            role = "user",
            content = narrative
        };

        string json = JsonUtility.ToJson(messageData);
        Debug.Log("SendMessage JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
            }
            else
            {
                RunThreadForExistingBook(bookName);
            }
        });
    }

    private void RunThreadForExistingBook(string bookName)
    {
        StartCoroutine(RunThreadCoroutineForExistingBook(bookName));
    }

    private IEnumerator RunThreadCoroutineForExistingBook(string bookName)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/runs";
        var runData = new RunsData
        {
            assistant_id = $"{assistant_ID}"
        };

        string json = JsonUtility.ToJson(runData);
        Debug.Log("RunThread JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
            }
            else
            {
                GetMessageResponseForExistingBook(bookName);
            }
        });
    }

    private void GetMessageResponseForExistingBook(string bookName)
    {
        StartCoroutine(GetMessageResponseCoroutineForExistingBook(bookName));
    }

    private IEnumerator GetMessageResponseCoroutineForExistingBook(string bookName)
    {
        string url = $"{apiBaseUrl}/{game_APIThread}/messages?limit=1";
        int maxAttempts = 10;
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
                }
                else
                {
                    string responseText = request.downloadHandler.text;
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
                                var messageContent = contentData.text.value;
                                Debug.Log("Received Message Content: " + messageContent);
                                bool isconc = false;
                                if (current_Page == 11)
                                {
                                    isconc = true;
                                }
                                string imageDescription = Parser.Instance.ExtractImageDescription(messageContent, isconc);
                                Debug.Log("imageDescription Message Content: " + imageDescription);
                                if (!string.IsNullOrEmpty(imageDescription))
                                {
                                        SendDescriptionToDalleForExistingBook(imageDescription, messageContent, bookName, isconc);
                                        success = true;
                                }
                            }
                        }
                    }
                }
            });

            if (!success)
            {
                Debug.LogWarning("No valid message content received. Retrying...");
                yield return new WaitForSeconds(3);
                attempt++;
            }
        }

        if (!success)
        {
            Debug.LogError("Failed to get a valid response after multiple attempts. Please try again later.");
            // TO DO : Notify the player here
        }
    }

    private void SendDescriptionToDalleForExistingBook(string description, string pageText, string bookName, bool isConc)
    {
        Debug.Log($"Sending description to DALL-E: {description}");
        if (string.IsNullOrEmpty(description))
        {
            Debug.LogError("Description is empty. Cannot send to DALL-E.");
            return;
        }

        StartCoroutine(SendDescriptionToDalleCoroutineForExistingBook(description, pageText, bookName, isConc));
    }

    private IEnumerator SendDescriptionToDalleCoroutineForExistingBook(string description, string pageText, string bookName, bool isConc)
    {
        string url = "https://api.openai.com/v1/images/generations";
        var imageRequest = new ImageGenerationRequest
        {
            prompt = description,
            n = 1,
            size = "1024x1024"
        };

        string json = JsonUtility.ToJson(imageRequest);
        Debug.Log("SendDescriptionToDalle JSON: " + json);

        yield return SendWebRequestCoroutine(url, "POST", json, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
            }
            else
            {
                var response = JsonUtility.FromJson<ImageResponse>(request.downloadHandler.text);
                string imageUrl = response.data[0].url;
                StartCoroutine(DownloadImageCoroutineForExistingBook(imageUrl, pageText, bookName, isConc));
            }
        });
    }

    private IEnumerator DownloadImageCoroutineForExistingBook(string url, string pageText, string bookName, bool isConc)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response Text: " + request.downloadHandler.text);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                byte[] imageBytes = texture.EncodeToPNG();
                string imagePath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, bookName, $"page{this.current_Page}_image.png");
                File.WriteAllBytes(imagePath, imageBytes);

                string bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, bookName);
                DataManager.CreateDirectoryIfNotExists(bookFolderPath);

                string bookFilePath = Path.Combine(bookFolderPath, "bookData.json");
                Book bookData;
                if (File.Exists(bookFilePath))
                {
                    string bookJson = File.ReadAllText(bookFilePath);
                    bookData = JsonUtility.FromJson<Book>(bookJson);
                }
                else
                {
                    Debug.LogError("Book file not found when attempting to add a new page.");
                    yield break;
                }

                // Parse the narrative into the structured format
                var page = new Page();
                if (isConc)
                    page = Parser.Instance.ParseConclusion(pageText, imagePath);
                else
                    page = Parser.Instance.ParsePage(pageText, imagePath);

                if (page != null)
                {
                    bookData.Pages.Add(page);
                    string json = JsonUtility.ToJson(bookData, true);
                    Debug.Log("Final JSON: " + json);
                    File.WriteAllText(bookFilePath, json);
                    if (isConc)
                    {
                        this.is_ConclusionSaved = true;
                        this.is_ConclusionSaved = false;
                    }
                    else
                    {
                        this.is_ended = true;
                        this.is_ended = false;
                    }
                }
            }
        }
    }

    public IEnumerator CreateAPI_Assistant(string apiKey, System.Action<bool, string> callback)
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
            this.assistant_ID = assistantResponse.id;
            Debug.Log(this.assistant_ID);
            callback(true, apiKey);
        }
    }
}
