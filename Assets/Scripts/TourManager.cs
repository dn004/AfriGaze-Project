using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEditor.UIElements;

public class TourManager : MonoBehaviour
{

    [System.Serializable]
    public class PARKINFO
    {
        public Sprite cardSprite;
        public Sprite sideSprite;
        public string parkName;
        public string headerName;
        public string information;
        public AudioClip clips;

    }


    public enum PARKS : int
    {
        NYANDUNGU = 0,
        NYUNGWE = 1,
    }


    [Header("CORE BUTTONS")]
    [Space(20)]
    public Button enterWelcomeBtn;
    public Button firstPackBtn;
    public Button secondPackBtn;


    [Header("CORE PANELS")]
    [Space(20)]
    public GameObject entryPanel;
    public GameObject selectionOfTrackPanel;
    public GameObject specificDesitnationPanel;
    public GameObject destinationPanel;
    public GameObject trailerPanel;
    public GameObject experiencePanel;
    public GameObject hotspotPanel;

    [Header("EXTRA STUFF")]
    [Space(20)]
    public AudioSource AudioPlayer;
    public Sprite enabledSprite;
    public Sprite disabledSprite;


    [Header("INFORMATION FOR PARKS")]
    [Space(20)]
    public List<PARKINFO> parkInfo;

    private PARKINFO currentPark;


    private void Start()
    {

    }

    private void StartExperience()
    {

    }

    private void ShowStartingPanel()
    {
        entryPanel.SetActive(true);
        entryPanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            ShowSelectionDestinationPanels();
            entryPanel.SetActive(false);
        });
    }

    private void ShowSelectionDestinationPanels()
    {
        selectionOfTrackPanel.SetActive(true);
        Button[] btns = selectionOfTrackPanel.GetComponentsInChildren<Button>();
        btns[0].onClick.AddListener(() =>
        {
            // Nyandungu Eco Park
            currentPark = parkInfo[(int)PARKS.NYANDUNGU];
            ShowSpecificDestionation(currentPark);
            selectionOfTrackPanel.SetActive(false);

        });

        btns[1].onClick.AddListener(() =>
        {
            // Nyungwe National Park
            currentPark = parkInfo[(int)PARKS.NYUNGWE];
            ShowSpecificDestionation(currentPark);
            selectionOfTrackPanel.SetActive(false);
        });
    }

    private void ShowSpecificDestionation(PARKINFO park)
    {
        specificDesitnationPanel.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
        {
            destinationPanel.SetActive(true);
            specificDesitnationPanel.SetActive(false);
        });

        specificDesitnationPanel.GetNamedChild("ExitButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            specificDesitnationPanel.SetActive(false);
            selectionOfTrackPanel.SetActive(true);
        });

        GameObject toggleObj = specificDesitnationPanel.GetNamedChild("volume");
        toggleObj.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                toggleObj.GetComponent<Image>().sprite = enabledSprite;
                AudioPlayer.clip = park.clips;
                AudioPlayer.loop = true;
                AudioPlayer.Play();
            }
            else
            {
                toggleObj.GetComponent<Image>().sprite = disabledSprite;
                AudioPlayer.Stop();
            }
        });

        {

            string total = "<b>" + park.headerName + "</b><\n><size=8>" + park.information + "</size>";
            specificDesitnationPanel.GetNamedChild("MainContent").GetComponent<TextMeshProUGUI>().text = total;
            specificDesitnationPanel.GetNamedChild("CardImage").GetComponent<Image>().sprite = park.cardSprite;


            specificDesitnationPanel.GetNamedChild("parkName").GetComponent<TextMeshProUGUI>().text = park.parkName;
            specificDesitnationPanel.GetNamedChild("ParkImage").GetComponent<Image>().sprite = park.sideSprite;

        }


    }

    private void ShowDestinationPanel()
    {
        // bind the 3 buttons to listeners for various actions based on the current park selected.
    }


    private void SetupEntryPanel()
    {

    }
}


