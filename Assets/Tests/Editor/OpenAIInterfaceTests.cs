using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine.Networking;

public class TestableOpenAIInterface : OpenAIInterface
{
    public System.Func<string, string, string, System.Action<UnityWebRequest>, IEnumerator> MockSendWebRequestCoroutine;

    protected IEnumerator SendWebRequestCoroutine(string url, string method, string json, System.Action<UnityWebRequest> callback)
    {
        if (MockSendWebRequestCoroutine != null)
        {
            return MockSendWebRequestCoroutine(url, method, json, callback);
        }
        return base.SendWebRequestCoroutine(url, method, json, callback);
    }
}

public class OpenAIInterfaceTests
{
    private GameObject gameObject;
    private TestableOpenAIInterface openAIInterface;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        openAIInterface = gameObject.AddComponent<TestableOpenAIInterface>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
    }

    private IEnumerator SimulateWebRequest(string url, string method, string json, System.Action<UnityWebRequest> callback, bool success = true)
    {
        // Create a real UnityWebRequest instance
        UnityWebRequest request = UnityWebRequest.Get("mock_url");

        // Use reflection to set the result and error
        if (success)
        {
            SetUnityWebRequestResult(request, UnityWebRequest.Result.Success);
            SetUnityWebRequestError(request, null);
        }
        else
        {
            SetUnityWebRequestResult(request, UnityWebRequest.Result.ConnectionError);
            SetUnityWebRequestError(request, "Network error");
        }

        // Set mock response text
        byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes("{\"id\":\"mock_thread_id\"}");
        typeof(DownloadHandlerBuffer).GetField("m_NativeData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(request.downloadHandler, responseBytes);

        // Simulate a delay before invoking the callback
        yield return null;

        // Pass the request back to the callback
        callback(request);
    }

    private void SetUnityWebRequestResult(UnityWebRequest request, UnityWebRequest.Result result)
    {
        typeof(UnityWebRequest).GetProperty("result", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(request, result);
    }

    private void SetUnityWebRequestError(UnityWebRequest request, string error)
    {
        typeof(UnityWebRequest).GetProperty("error", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(request, error);
    }

    [UnityTest]
    public IEnumerator TestSendNarrativeToAPI_Success()
    {
        // Arrange
        string bookName = "TestBook";
        string narrative = "Test Narrative";
        int pageNum = 1;

        // Mock the web request process
        openAIInterface.MockSendWebRequestCoroutine = (url, method, json, callback) =>
        {
            return SimulateWebRequest(url, method, json, callback, true);
        };

        // Act
        openAIInterface.SendNarrativeToAPI(bookName, narrative, pageNum);
        yield return null;

        // Assert
        Assert.AreEqual("Test Narrative", openAIInterface.current_Narrative);
        Assert.AreEqual(1, openAIInterface.current_Page);
    }

    [UnityTest]
    public IEnumerator TestSendNarrativeToAPI_Failure()
    {
        // Arrange
        string bookName = "TestBook";
        string narrative = "Test Narrative";
        int pageNum = 1;

        // Simulate a failed web request
        openAIInterface.MockSendWebRequestCoroutine = (url, method, json, callback) =>
        {
            return SimulateWebRequest(url, method, json, callback, false); // Pass 'false' to simulate failure
        };

        // Act
        openAIInterface.SendNarrativeToAPI(bookName, narrative, pageNum);
        yield return null;

        // Assert
        Assert.IsNull(openAIInterface.AssistantID, "Assistant ID should be null, leading to failure in the API request.");
    }

    [Test]
    public void OpenAIInterface_SetAndTrigger_IsEndedChangedEvent()
    {
        bool eventTriggered = false;
        openAIInterface.OnIsEndedChanged += (isEnded) =>
        {
            eventTriggered = true;
            Assert.IsTrue(isEnded);
        };

        openAIInterface.is_ended = true;
        Assert.IsTrue(eventTriggered, "Event was not triggered as expected.");
    }

    [Test]
    public void OpenAIInterface_SetAndTrigger_ConclusionSaveEvent()
    {
        bool eventTriggered = false;
        openAIInterface.OnConclusionSave += (isConclusionSaved) =>
        {
            eventTriggered = true;
            Assert.IsTrue(isConclusionSaved);
        };

        openAIInterface.is_ConclusionSaved = true;
        Assert.IsTrue(eventTriggered, "ConclusionSave event was not triggered as expected.");
    }

    [Test]
    public void OpenAIInterface_SetAssistantID_UpdatesCorrectly()
    {
        openAIInterface.AssistantID = "TestAssistantID";
        Assert.AreEqual("TestAssistantID", openAIInterface.AssistantID);

        openAIInterface.AssistantID = "NewAssistantID";
        Assert.AreEqual("NewAssistantID", openAIInterface.AssistantID);
    }
}
