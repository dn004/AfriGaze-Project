using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using UnityEngine.Events;

public partial class TourManager : MonoBehaviour
{

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
    public GameObject hotspotExitPrefab;
    

    [Header("EXTRA STUFF")]
    [Space(20)]
    public AudioSource AudioPlayer;
    public VideoPlayer videoPlayer;
    public Sprite enabledSprite;
    public Sprite disabledSprite;
    public Transform playerTransform;
    public Transform picnicPoint;
    public Transform pondPoint;
    public GameObject picnicTrailerObj;
    public GameObject numberTrailerObj;
    public GameObject generalSphere;
    public GameObject simbaFirstSphere;
    public GameObject picicFirstSphere;

    [Header("ADDRESSABLE REFERENCES")]
    [Space(20)]
    public List<AssetReference> assets;

    [Header("Events")]
    [Space(20)]
    public UnityEvent StartExperienceParkEvent;
    public UnityEvent StartExperienceSimbaCenterEvent;


    [Header("INFORMATION FOR PARKS")]
    [Space(20)]
    public List<PARKINFO> parkInfo;

    private PARKINFO currentPark;

    private GameObject exitPrefab;
    private GameObject initLocationPrefab;

    private Button[] btns;
    private GameObject toggleObj;


    #endregion

    private void LoadAddressable(AssetReference reference)
    {
        AsyncOperationHandle<VideoClip> operation = Addressables.LoadAssetAsync<VideoClip>(reference);
        operation.Completed += Operation_Completed;

    }

    private void Operation_Completed(AsyncOperationHandle<VideoClip> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            videoPlayer.clip = obj.Result;
            videoPlayer.Prepare();
        }
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
        btns = selectionOfTrackPanel.GetComponentsInChildren<Button>();
        btns[0].onClick.AddListener(SelectionNyandunguEcoPark);
        btns[1].onClick.AddListener(SelectionNyungwe);
    }

    private void SelectionNyungwe()
    {
        currentPark = parkInfo[(int)PARKS.NYUNGWE];
        ShowSpecificDestionation(currentPark);
        selectionOfTrackPanel.SetActive(false);
        RemoveListenersSelection();
    }

    private void SelectionNyandunguEcoPark()
    {
        currentPark = parkInfo[(int)PARKS.NYANDUNGU];
        ShowSpecificDestionation(currentPark);
        selectionOfTrackPanel.SetActive(false);
        RemoveListenersSelection();
    }

    // remove listeners
    private void RemoveListenersSelection()
    {
        btns[0].onClick.RemoveListener(SelectionNyandunguEcoPark);
        btns[1].onClick.RemoveListener(SelectionNyungwe);
    }


    private void ShowSpecificDestionation(PARKINFO park)
    {
        specificDesitnationPanel.SetActive(true);

        specificDesitnationPanel.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(NextShowSpecificationDestination);

        specificDesitnationPanel.GetNamedChild("ExitButton").GetComponent<Button>().onClick.AddListener(ExitButtonSpecificationDestination);

        toggleObj = specificDesitnationPanel.GetNamedChild("volume");
        currentPark = park;
        toggleObj.GetComponent<Toggle>().onValueChanged.AddListener(ToggleChangeVolumeBinder);

        {

            string total = "<b>" + park.headerName + "</b><\n><size=8>" + park.information + "</size>";
            specificDesitnationPanel.GetNamedChild("MainContent").GetComponent<TextMeshProUGUI>().text = total;
            specificDesitnationPanel.GetNamedChild("CardImage").GetComponent<Image>().sprite = park.cardSprite;
            specificDesitnationPanel.GetNamedChild("ParkName").GetComponent<TextMeshProUGUI>().text = park.parkName;
            specificDesitnationPanel.GetNamedChild("ParkImage").GetComponent<Image>().sprite = park.sideSprite;

        }


    }

    private void ToggleChangeVolumeBinder(bool value)
    {
        if (value)
        {
            toggleObj.GetComponent<Image>().sprite = enabledSprite;
            AudioPlayer.clip = currentPark.clips;
            AudioPlayer.loop = true;
            AudioPlayer.Play();
        }
        else
        {
            toggleObj.GetComponent<Image>().sprite = disabledSprite;
            AudioPlayer.Stop();
        }
    }

    private void ExitButtonSpecificationDestination()
    {
        specificDesitnationPanel.SetActive(false);
        ShowSelectionDestinationPanels();
        AudioPlayer.Stop();
        RemoveListenersShowSpecification();
    }

    private void NextShowSpecificationDestination()
    {
        destinationPanel.SetActive(true);
        specificDesitnationPanel.SetActive(false);
        ShowDestinationPanel();
        RemoveListenersShowSpecification();
    }

    // remove listeners
    private void RemoveListenersShowSpecification()
    {
        toggleObj.GetComponent<Toggle>().onValueChanged.RemoveListener(ToggleChangeVolumeBinder);
        specificDesitnationPanel.GetNamedChild("Next").GetComponent<Button>().onClick.RemoveListener(NextShowSpecificationDestination);
        specificDesitnationPanel.GetNamedChild("ExitButton").GetComponent<Button>().onClick.RemoveListener(ExitButtonSpecificationDestination);
    }
    

    private void ShowDestinationPanel()
    {
        // bind the 3 buttons to listeners for various actions based on the current park selected.
        {
            destinationPanel.GetNamedChild("Experience").GetComponent<Button>().onClick.AddListener(ShowDestinationPanelExperience);

            destinationPanel.GetNamedChild("trailer").GetComponent<Button>().onClick.AddListener(ShowDestinationTrailerPanel);

            destinationPanel.GetNamedChild("Back").GetComponent<Button>().onClick.AddListener(ShowDestinationBackPanel);
        }
    }

    private void RemoveListenersShowDestinationPanel()
    {
        destinationPanel.GetNamedChild("Experience").GetComponent<Button>().onClick.RemoveListener(ShowDestinationPanelExperience);

        destinationPanel.GetNamedChild("trailer").GetComponent<Button>().onClick.RemoveListener(ShowDestinationTrailerPanel);

        destinationPanel.GetNamedChild("Back").GetComponent<Button>().onClick.RemoveListener(ShowDestinationBackPanel);
    }

    private void ShowDestinationBackPanel()
    {
        // show previous master menu
        destinationPanel.SetActive(false);
        ShowSpecificDestionation(currentPark);
    }

    private void ShowDestinationTrailerPanel()
    {
        // move to trailer panel
        ShowTrailerPanel();
        destinationPanel.SetActive(false);
    }

    private void ShowDestinationPanelExperience()
    {
        ShowExperiencePanel();
        destinationPanel.SetActive(false);
    }

    private void ShowTrailerPanel()
    {
        trailerPanel.SetActive(true);

        {
            trailerPanel.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(PlayTrailer);
         
        }

        {
            trailerPanel.GetNamedChild("ExitBtn").GetComponent<Button>().onClick.AddListener(ExitButtonTrailer);
        }

        {
            string fullText = "<b> Trailer Mode </b>" + "\n" + "<size=8>" + currentPark.trailerText + "</size>";
            trailerPanel.GetNamedChild("TrailerText").GetComponent<TextMeshProUGUI>().text = fullText;
        }

        {
            trailerPanel.GetNamedChild("CardImage").GetComponent<Image>().sprite = currentPark.cardSprite;
        }
    }

    private void ExitButtonTrailer()
    {
        ShowDestinationPanel();
        destinationPanel.SetActive(true);
        trailerPanel.SetActive(false);
    }

    private void PlayTrailer()
    {
        trailerPanel.SetActive(false);
        generalSphere.SetActive(false);
        switch (currentPark.parks)
        {
            case PARKS.NYANDUNGU:
                StartCoroutine(showTrailerNumber());
                break;
            case PARKS.NYUNGWE:
                StartCoroutine(showTrailerPicnic());
                break;
        }
       
    }

    IEnumerator showTrailerNumber()
    {
        numberTrailerObj.SetActive(true);
        yield return new WaitForSeconds(5);
        numberTrailerObj.SetActive(false);
        generalSphere.SetActive(true);
        trailerPanel.SetActive(true);
    }

    IEnumerator showTrailerPicnic()
    {
        picnicTrailerObj.SetActive(true);
        
        yield return new WaitForSeconds(7);
        picnicTrailerObj.SetActive(false);
        generalSphere.SetActive(true);
        trailerPanel.SetActive(true);

    }

    private void RemoveListenerShowTrailerPanel()
    {
        trailerPanel.GetNamedChild("Next").GetComponent<Button>().onClick.RemoveListener(PlayTrailer);
        trailerPanel.GetNamedChild("ExitBtn").GetComponent<Button>().onClick.RemoveListener(ExitButtonTrailer);
    }

    private void ShowExperiencePanel()
    {
        experiencePanel.SetActive(true);

        {
            experiencePanel.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(NextShowExperiencePanel);
            
        }

        {
            experiencePanel.GetNamedChild("ExitBtn").GetComponent<Button>().onClick.AddListener(ExitBtnShowExperiencePanel);
      
        }

        {
            experiencePanel.GetNamedChild("CardImage").GetComponent<Image>().sprite = currentPark.cardSprite;
        }
    }


    private void ExitBtnShowExperiencePanel()
    {
        ShowDestinationPanel();
        destinationPanel.SetActive(true);
        experiencePanel.SetActive(false);
    }

    private void ShowExperiencePanelRemoveListeners()
    {
        experiencePanel.GetNamedChild("Next").GetComponent<Button>().onClick.RemoveListener(NextShowExperiencePanel);
        experiencePanel.GetNamedChild("ExitBtn").GetComponent<Button>().onClick.RemoveListener(ExitBtnShowExperiencePanel);
    }

    private void NextShowExperiencePanel()
    {
        // launch the experience
        RemoveListenersShowDestinationPanel();
        switch (currentPark.parks)
        {
            case PARKS.NYANDUNGU:
                experiencePanel.SetActive(false);
                // simba area

                simbaFirstSphere.SetActive(true);
                generalSphere.SetActive(false);
                StartExperienceSimbaCenterEvent?.Invoke();


                LoadAddressable(assets[(int)LOCATIONS.ONE]);
                ShowExperiencePanelRemoveListeners();


                break;
            case PARKS.NYUNGWE:


                experiencePanel.SetActive(false);
                picicFirstSphere.SetActive(true);
                generalSphere.SetActive(false);
                StartExperienceParkEvent?.Invoke();

              
                ShowExperiencePanelRemoveListeners();
                break;
        }
    }

    public void CloseCurrentExperience()
    {
        ShowExperiencePanel();
        generalSphere.SetActive(true);
        simbaFirstSphere.SetActive(false);
        picicFirstSphere.SetActive(false);
    }


    private void DestroyN(GameObject obj)
    {
        if(obj != null)
        {
            Destroy(obj.gameObject);
        }
       
    }

}


