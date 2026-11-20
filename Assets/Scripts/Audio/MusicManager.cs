using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private MusicLibrary musicLibrary;
    [SerializeField] private AudioSource musicSource;


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

    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossfade(musicLibrary.GetClipFromName(trackName), fadeDuration));
    }

    public void PlayMainMenuMusic()
    {
        int rnd = Random.Range(9,11);
        PlayMusic(musicLibrary.Tracks[rnd].trackName);
    }

    public void PlayShopMusic()
    {
        int rnd = Random.Range(7,9);
        PlayMusic(musicLibrary.Tracks[rnd].trackName);
    }

    public void PlayBattleMusic()
    {
        int rnd = Random.Range(0,3);
        PlayMusic(musicLibrary.Tracks[rnd].trackName);
    }

    public void PlayBossMusic()
    {
        PlayMusic("BossBG");
    }
    
    public void PlayTreasureMusic()
    {
        PlayMusic("TreasureBG");
    }
    
    public void PlayCampFireMusic()
    {
        PlayMusic("CampFireBG");
    }

    public void PlayMapMusic()
    {
        PlayMusic("MapBG");
    }

    public void StopPlayMusic()
    {
        musicSource.Stop();
    }
    
    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(1f, 0, percent);
            yield return null;
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, 1f, percent);
            yield return null;
        }
    }
}
