using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.AddressableAssets.ResourceLocators;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;


public class TourManager : MonoBehaviour
{
    

    #region EXTRA DEFINATIONS
    [System.Serializable]
    public class PARKINFO
    {
        public Sprite cardSprite;
        public Sprite sideSprite;
        public string parkName;
        public string headerName;
        public string information;
        public string trailerText;
        public AudioClip clips;
        public PARKS parks;

    }

    public enum LOCATIONS : Int16
    {
        ONE = 0,
        TWO = 1,
        THREE =2,
        PICNIC = 3,
        PICNICORPOND = 4,
        POND = 5
    }


    public enum PARKS : int
    {
        NYANDUNGU = 0,
        NYUNGWE = 1,
    }

    #endregion


    #region VARIABLES
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
    public GameObject hotspotPrefab;

    [Header("EXTRA STUFF")]
    [Space(20)]
    public AudioSource AudioPlayer;
    public VideoPlayer videoPlayer;
    public Sprite enabledSprite;
    public Sprite disabledSprite;
    public Transform playerTransform;

    [Header("ADDRESSABLE REFERENCES")]
    [Space(20)]
    public List<AssetReference> assets;


    [Header("INFORMATION FOR PARKS")]
    [Space(20)]
    public List<PARKINFO> parkInfo;

    private PARKINFO currentPark;


    #endregion

    private void LoadAddressable(AssetReference reference)
    {
        AsyncOperationHandle<VideoClip> operation = Addressables.LoadAssetAsync<VideoClip>(reference);
        operation.Completed += Operation_Completed;

    }

    private void Operation_Completed(AsyncOperationHandle<VideoClip> obj)
    {
        if(obj.Status == AsyncOperationStatus.Succeeded)
        {
            videoPlayer.clip = obj.Result;
            videoPlayer.Prepare();
        }
    }

    private void processVideo()
    {
       
    }

    private void SetupHotspotPrefab(string name)
    {

    }

    private void Start()
    {
        StartExperience();
    }

    private void StartExperience()
    {
        ShowStartingPanel();
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
        specificDesitnationPanel.SetActive(true);

        specificDesitnationPanel.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
        {
            destinationPanel.SetActive(true);
            specificDesitnationPanel.SetActive(false);
            ShowDestinationPanel();
        });

        specificDesitnationPanel.GetNamedChild("ExitButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            specificDesitnationPanel.SetActive(false);
            ShowSelectionDestinationPanels();
            AudioPlayer.Stop();
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


            specificDesitnationPanel.GetNamedChild("ParkName").GetComponent<TextMeshProUGUI>().text = park.parkName;
            specificDesitnationPanel.GetNamedChild("ParkImage").GetComponent<Image>().sprite = park.sideSprite;

        }


    }

    private void ShowDestinationPanel()
    {
        // bind the 3 buttons to listeners for various actions based on the current park selected.
        {
            destinationPanel.GetNamedChild("Experience").GetComponent<Button>().onClick.AddListener(() =>
            {
                // move to experience panel
                ShowExperiencePanel();
                destinationPanel.SetActive(false);
            });

            destinationPanel.GetNamedChild("trailer").GetComponent<Button>().onClick.AddListener(() =>
            {
                // move to trailer panel
                ShowTrailerPanel();
                destinationPanel.SetActive(false);
            });

            destinationPanel.GetNamedChild("Back").GetComponent<Button>().onClick.AddListener(() =>
            {
                // show previous master menu
                destinationPanel.SetActive(false);
                ShowSpecificDestionation(currentPark);
            });
        }
    }

    private void ShowTrailerPanel()
    {
        trailerPanel.SetActive(true);

        {
            trailerPanel.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
            {
                // launch the trailerVideo
            });
        }

        {
            trailerPanel.GetNamedChild("ExitBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowDestinationPanel();
                destinationPanel.SetActive(true);
                trailerPanel.SetActive(false);
            });
        }

        {
            string fullText = "<b> Trailer Mode </b>" + "\n" + "<size=8>" + currentPark.trailerText  + "</size>";
            trailerPanel.GetNamedChild("TrailerText").GetComponent<TextMeshProUGUI>().text = fullText;
        }

        {
            trailerPanel.GetNamedChild("CardImage").GetComponent<Image>().sprite = currentPark.cardSprite;
        }
    }

    private void ShowExperiencePanel()
    {
        experiencePanel.SetActive(true);

        {
            experiencePanel.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
            {
                // launch the experience
               

                switch (currentPark.parks)
                {
                    case PARKS.NYANDUNGU:
                        experiencePanel.SetActive(false);
                        LoadAddressable(assets[(int)LOCATIONS.ONE]);
                        SpawnHotRight();
                        SpawnHotLeft();
                        break;
                    case PARKS.NYUNGWE:
                        experiencePanel.SetActive(false);
                        LoadAddressable(assets[(int)LOCATIONS.PICNIC]);
                        SpawnHotRight();
                        break;
                }

            });
        }

        {
            experiencePanel.GetNamedChild("ExitBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowDestinationPanel();
                destinationPanel.SetActive(true);
                experiencePanel.SetActive(false);
            });
        }

        {
            experiencePanel.GetNamedChild("CardImage").GetComponent<Image>().sprite = currentPark.cardSprite;
        }
    }

    private void SpawnHotRight()
    {
        GameObject obj = Instantiate(hotspotPrefab, playerTransform.position + new Vector3(1f, 0.5f, 2f), Quaternion.identity);
    }

    private void SpawnHotLeft()
    {
        GameObject obj = Instantiate(hotspotPrefab, playerTransform.position + new Vector3(-1f, 0.5f, -2f), Quaternion.identity);
    }


}


