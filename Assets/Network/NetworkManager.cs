using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private string apiBaseURL = "http://localhost:9999/api";
    public event System.Action<LeaderboardResponse> LeaderboardLoaded;
    public event System.Action<string> LeaderboardFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLeaderboard()
    {
        StartCoroutine(GetLeaderboard());
    }

    private IEnumerator GetLeaderboard()
    {
        string url = apiBaseURL + "/leaderboard";
        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Leaderboard request failed: " + request.error);
            LeaderboardFailed?.Invoke(request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
        LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(json);
        if(response == null || response.players == null)
        {
            LeaderboardFailed?.Invoke("Invalid leaderboard response.");
            yield break;
        }
        foreach (LeaderboardEntry entry in response.players)
        {
            Debug.Log(entry.playerName + ": " + entry.inventoryValue + entry.floorsClimbed);
        }
        LeaderboardLoaded?.Invoke(response);
    }

    private IEnumerator PostScore(string playerName, int inventoryValue, int floorsClimbed)
    {
        string url = apiBaseURL + "/records";

        LeaderboardRequest leaderboardRequest = new()
        {
            playerName = playerName,
            inventoryValue = inventoryValue,
            floorsClimbed = floorsClimbed
        };

        string json = JsonUtility.ToJson(leaderboardRequest);
        byte[] body = Encoding.UTF8.GetBytes(json);
        using UnityWebRequest request = new(url, "POST");

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Save record failed: " + request.error);
            yield break;
        }

        Debug.Log(request.downloadHandler.text);
        Debug.Log("Record saved successfully!");
    }

    public void SendRecord(string playerName, int inventoryValue, int floorsClimbed)
    {
        StartCoroutine(PostScore(playerName, inventoryValue, floorsClimbed));
    }
}
