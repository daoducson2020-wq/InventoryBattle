using UnityEngine;

[System.Serializable]
public struct MusicTrack
{
    public string trackName;
    public AudioClip clip;
}

public class MusicLibrary : MonoBehaviour
{
    [SerializeField] private MusicTrack[] tracks;
    public MusicTrack[] Tracks => tracks;
    
    public AudioClip GetClipFromName(string trackName)
    {
        foreach (var item in tracks)
        {
            if (item.trackName == trackName)
            {
                return item.clip;
            }
        }
        return null;
    }

}
