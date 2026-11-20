using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingManager : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private AudioSettingManager audioManager;
    [SerializeField] private TMP_Dropdown resoDropDown;
    [SerializeField] private Toggle fullscreenToggle;

    private Resolution[] allResolutions;
    private List<Resolution> selectedResoLists = new();
    private bool isFullscreen;
    private bool panelOn = false;
    private int selectedResolution;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFullscreen = true;
        allResolutions = Screen.resolutions;

        List<string> resoStringLists = new();
        string newRes;
        foreach (var item in allResolutions)
        {
            newRes = item.width.ToString() + " x " + item.height.ToString();
            if (!resoStringLists.Contains(newRes))
            {
                resoStringLists.Add(newRes.ToString());
                selectedResoLists.Add(item);
            }
        }
        resoDropDown.AddOptions(resoStringLists);
        resoDropDown.value = selectedResoLists.Count;
        ChangeResolution();
        audioManager.LoadVolume();
        optionPanel.SetActive(panelOn);
    }

    public void ClosePanel()
    {
        panelOn = !panelOn;
        optionPanel.SetActive(panelOn);
    }

    public void ChangeResolution()
    {
        selectedResolution = resoDropDown.value;
        SetReso();
    }

    public void FullscreenToggle()
    {
        isFullscreen = fullscreenToggle.isOn;
        SetReso();
    }

    private void SetReso()
    {
        Screen.SetResolution(selectedResoLists[selectedResolution].width,
                            selectedResoLists[selectedResolution].height,
                            isFullscreen);
    }
}
