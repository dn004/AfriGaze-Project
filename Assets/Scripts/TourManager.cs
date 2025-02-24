using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


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

    [Header("ADDRESSABLE REFERENCES")]
    [Space(20)]
    public List<AssetReference> assets;


    [Header("INFORMATION FOR PARKS")]
    [Space(20)]
    public List<PARKINFO> parkInfo;

    private PARKINFO currentPark;

    private GameObject exitPrefab;
    private GameObject initLocationPrefab;

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
            string fullText = "<b> Trailer Mode </b>" + "\n" + "<size=8>" + currentPark.trailerText + "</size>";
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
                        SpawnHotRight("TWO");
                        SpawnExitLocation();
                        break;
                    case PARKS.NYUNGWE:
                        experiencePanel.SetActive(false);
                        LoadAddressable(assets[(int)LOCATIONS.PICNIC]);
                        SpawnHotRight("PICNICPOND");
                        SpawnExitLocation();
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

    private void SpawnHotRight(string location)
    {
        // first scene spawning
        initLocationPrefab = Instantiate(hotspotPrefab, playerTransform.position + new Vector3(1f, 0.5f, 2f), Quaternion.identity);

        {
            initLocationPrefab.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
            {
                SpawnSetupMidLevel();
                DestroyN(initLocationPrefab.gameObject);
                DestroyN(exitPrefab.gameObject);
            });

            initLocationPrefab.GetNamedChild("location").GetComponent<TextMeshProUGUI>().text = location;
        }

    }

    GameObject objS, objT, objE;


    public void SpawnSetupMidLevel()
    {
        switch (currentPark.parks)
        {
            case PARKS.NYANDUNGU:
                //second scene
         
                LoadAddressable(assets[(int)LOCATIONS.TWO]);

                {
                    //moving to last scene
                    objS = Instantiate(hotspotPrefab, playerTransform.position + new Vector3(1f, 0.5f, 2f), Quaternion.identity);
                    objS.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        LoadAddressable(assets[(int)LOCATIONS.THREE]);
                        {
                            objE = Instantiate(hotspotPrefab, playerTransform.position + new Vector3(-1f, 0.5f, -2f), Quaternion.identity);
                            objE.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
                            {
                                Destroy(exitPrefab);
                                
                                LoadAddressable(assets[(int)LOCATIONS.ONE]);
                                SpawnHotRight("ONE");
                                SpawnExitLocation();
                                DestroyN(objE.gameObject);

                            });

                            objE.GetNamedChild("location").GetComponent<TextMeshProUGUI>().text = "ONE";
                            
                        }

                        DestroyN(objS.gameObject);
                        DestroyN(objT.gameObject);
                    });

                    objS.GetNamedChild("location").GetComponent<TextMeshProUGUI>().text = "THREE";


                }



                {
                    // returning to base scene

                    objT = Instantiate(hotspotPrefab, playerTransform.position + new Vector3(-1f, 0.5f, -2f), Quaternion.identity);
                    objT.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        experiencePanel.SetActive(false);
                        LoadAddressable(assets[(int)LOCATIONS.ONE]);
                        SpawnHotRight("TWO");
                        SpawnExitLocation();
                        DestroyN(objT.gameObject);
                        DestroyN(objE.gameObject);
                        DestroyN(objS.gameObject);
                    });

                    objT.GetNamedChild("location").GetComponent<TextMeshProUGUI>().text = "ONE";
                }



                break;
            case PARKS.NYUNGWE:

                LoadAddressable(assets[(int)LOCATIONS.PICNICORPOND]);
                {
                    //moving to last scene
                    objS = Instantiate(hotspotPrefab, playerTransform.position + new Vector3(1f, 0.5f, 2f), Quaternion.identity);
                    objS.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        LoadAddressable(assets[(int)LOCATIONS.POND]);
                        {
                            objE = Instantiate(hotspotPrefab, playerTransform.position + new Vector3(-1f, 0.5f, -2f), Quaternion.identity);
                            objE.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
                            {
                               
                                experiencePanel.SetActive(false);
                                LoadAddressable(assets[(int)LOCATIONS.PICNIC]);
                                SpawnHotRight("PICNIC");
                                SpawnExitLocation();
                                DestroyN(objE.gameObject);

                            });

                            objE.GetNamedChild("location").GetComponent<TextMeshProUGUI>().text = "PICNIC";
                        }

                        DestroyN(objS.gameObject);
                        DestroyN(objT.gameObject);

                    });

                    objS.GetNamedChild("location").GetComponent<TextMeshProUGUI>().text = "POND";
                 
                }



                {
                    // returning to base scene

                    objT = Instantiate(hotspotPrefab, playerTransform.position + new Vector3(-1f, 0.5f, -2f), Quaternion.identity);
                    objT.GetNamedChild("Next").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        experiencePanel.SetActive(false);
                        LoadAddressable(assets[(int)LOCATIONS.PICNIC]);
                        SpawnHotRight("PICNICPOND");
                        SpawnExitLocation();
                        DestroyN(objT.gameObject);
                        DestroyN(objE.gameObject);
                        DestroyN(objS.gameObject);
                    });

                    objT.GetNamedChild("location").GetComponent<TextMeshProUGUI>().text = "PICNIC";
                }
                break;
        }

        
    }


    private void SpawnExitLocation()
    {
        exitPrefab = Instantiate(hotspotExitPrefab, playerTransform.position + new Vector3(-1f, 0.5f, -2f), Quaternion.identity);

        {
            exitPrefab.GetNamedChild("Exit").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowExperiencePanel();
                // set the default skybox background here
                LoadAddressable(assets[(int)LOCATIONS.PICNIC]);
               
                DestroyN(exitPrefab.gameObject);
                DestroyN(initLocationPrefab.gameObject);
                DestroyN(objT.gameObject);  
                DestroyN(objE.gameObject);
                DestroyN(objS.gameObject);
                
            });
        }
    }


    private void DestroyN(GameObject obj)
    {
        if(obj != null)
        {
            Destroy(obj.gameObject);
        }
       
    }


}


