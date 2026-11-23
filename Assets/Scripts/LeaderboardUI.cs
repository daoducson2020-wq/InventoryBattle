using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Transform rowsContainer;
    [SerializeField] private LeaderboardRow rowPrefab;
    [SerializeField] private TMP_Text statusText;

    private void OnEnable()
    {
        if (NetworkManager.Instance != null)
        {
            networkManager = NetworkManager.Instance;
        }

        if (networkManager == null ||
            rowsContainer == null ||
            rowPrefab == null)
        {
            Debug.LogError($"Leaderboard UI references are missing. networkManager={networkManager != null}, rowsContainer={rowsContainer != null}, rowPrefab={rowPrefab != null}");
            return;
        }

        networkManager.LeaderboardLoaded += DisplayLeaderboard;
        networkManager.LeaderboardFailed += DisplayError;

        RefreshLeaderboard();
    }

    private void OnDisable()
    {
        if (networkManager == null) return;

        networkManager.LeaderboardLoaded -= DisplayLeaderboard;
        networkManager.LeaderboardFailed -= DisplayError;
    }

    public void RefreshLeaderboard()
    {
        if (networkManager == null) return;

        if (statusText != null)
        {
            statusText.text = "Loading leaderboard...";
        }

        networkManager.LoadLeaderboard();
    }

    private void DisplayLeaderboard(LeaderboardResponse response)
    {
        ClearRows();

        if (response == null || response.players == null)
        {
            DisplayError("Invalid response.");
            return;
        }

        int count = Mathf.Min(response.players.Length, 5);

        for (int i = 0; i < count; i++)
        {
            LeaderboardRow row = Instantiate(
            rowPrefab,
            rowsContainer
            );

            row.SetData(i + 1, response.players[i]);
        }

        if (statusText != null)
        {
            statusText.text = count == 0
            ? "No records available."
            : "";
        }
    }

    private void ClearRows()
    {
        foreach (Transform child in rowsContainer)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }

    private void DisplayError(string error)
    {
        if (statusText != null)
        {
            statusText.text = "Unable to load leaderboard.";
        }

        Debug.LogWarning(error);
    }
}