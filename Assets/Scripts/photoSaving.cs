using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;
using System;

public class photoSaving : MonoBehaviour
{
    public GameObject rightHandVisual;
    public GameObject cameraObject;
    public GameObject planeObject;
    public Material planeMaterial;
    public Camera targetCamera;

    public int captureWidth = 1920;
    public int captureHeight = 1080;

    public InputActionReference takePictureAction;
    public InputActionReference activateCamera;
    public InputActionReference loadPictureAction;
    public MeshRenderer render;
    public AudioSource audioSource;
    public AudioClip cameraSound;
    public RawImage texturedImage;

    public TextMeshProUGUI photoText;
    public RawImage texturedSelection;
    public Button nextBtn;
    public Button backBtn;
    public GameObject viewPhotoCanvas;

    private List<Texture2D> allTextures = new List<Texture2D>();
    private int count = 0;
    private int currentCount = 0;
    private int countState;
    private int imageIndexToLoad = 0;

    private void Awake()
    {
       
        countState = PlayerPrefs.GetInt("state");
        if(countState < 0)
        {
            countState = 0;
        }

        count = countState;

    }

    private void OnEnable()
    {
        takePictureAction.action.Enable();
        takePictureAction.action.performed += _ => CaptureAndSave();

        activateCamera.action.Enable();
        activateCamera.action.performed += _ => cameraActivation();

        loadPictureAction.action.Enable();
        loadPictureAction.action.performed += _ => StartCoroutine(LoadTexture());
    }

    private void OnDisable()
    {
        takePictureAction.action.Disable();
        takePictureAction.action.performed -= _ => CaptureAndSave();

        activateCamera.action.Disable();
        activateCamera.action.performed -= _ => cameraActivation();

        loadPictureAction.action.Disable();
        loadPictureAction.action.performed -= _ => StartCoroutine(LoadTexture());
    }

    private void Start()
    {
        nextBtn.onClick.AddListener(NextImage);
        backBtn.onClick.AddListener(PreviousImage);
        viewPhotoCanvas.SetActive(false);
        cameraObject.SetActive(false);
    }

    private void cameraActivation()
    {

        if (rightHandVisual.activeInHierarchy)
        {
            rightHandVisual.SetActive(false);
            cameraObject.SetActive(true);
        }
        else
        {
            rightHandVisual.SetActive(true);
            cameraObject.SetActive(false);
        }
      
    }

    public void CaptureAndSave()
    {
        if(!viewPhotoCanvas.activeInHierarchy && cameraObject.activeInHierarchy)
        {
            audioSource.PlayOneShot(cameraSound);
            cameraObject.SetActive(false);

            if (targetCamera == null)
            {
                Debug.LogError("Target camera not assigned.");
                return;
            }

            RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
            targetCamera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
            targetCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);


            targetCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            

            SaveTextureToFile(screenShot, "CameraCapture_" + count + ".png");
            imageIndexToLoad = count;
            count++;


            cameraObject.SetActive(true);
        }
       
    }


    void SaveTextureToFile(Texture2D texture, string filename)
    {
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.persistentDataPath, filename); 
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Screenshot saved to: " + filePath);
        texturedImage.texture = texture;
        Destroy(texture);
        LoadTempTexture();
        
    }


    IEnumerator showImage()
    {
        planeObject.SetActive(true);
        yield return new WaitForSeconds(3);
        planeObject.SetActive(false);
    }

    void LoadTempTexture()
    {
        byte[] fileData = File.ReadAllBytes(Application.persistentDataPath + "/CameraCapture_" + imageIndexToLoad + ".png");
        Texture2D textureData = new Texture2D(2, 2);

        if (textureData.LoadImage(fileData))
        {
            texturedImage.texture = textureData;
        }

        StartCoroutine(showImage());
    }

    private void NextImage()
    {
        
        if (count < allTextures.Count)
        {
            count++;
        }
        else
        {
            count = 0;
        }

        texturedSelection.texture = allTextures[count];
     
        photoText.text = currentCount + "/" + allTextures.Count.ToString();
    }

    private void PreviousImage()
    {
        
        count--;
        if(count < 0)
        {
            count = allTextures.Count - 1;
        }
        
        texturedSelection.texture = allTextures[count];
        photoText.text = currentCount + "/" + allTextures.Count.ToString();
    }


    IEnumerator LoadTexture()
    {
        if (viewPhotoCanvas.activeInHierarchy)
        {
            viewPhotoCanvas.SetActive(false);
            count = 0;
        }
        else
        {
            viewPhotoCanvas.SetActive(true);
            count = 0;
        }

        Debug.Log("countState : " + countState);
        currentCount = countState;

        while (true)
        {

            try
            {
                
                byte[] fileData = File.ReadAllBytes(Application.persistentDataPath + "/CameraCapture_" + currentCount + ".png");
                Texture2D texture = new Texture2D(2, 2);
                Debug.Log("CurrentCount : " + currentCount);
                if (texture.LoadImage(fileData))
                {
                    allTextures.Add(texture);
                }
                else
                {
                    Debug.LogError("Failed to add texture to elements");
                }

            }catch(Exception e)
            {
             
                break;
            }
            
            
          
            yield return new WaitForSeconds(0.01f);
            currentCount++;
        }

        countState = allTextures.Count;
        PlayerPrefs.SetInt("state", countState);
        PlayerPrefs.Save();
        Debug.Log("Images Saved Locally : " + allTextures.Count.ToString());

        photoText.text = currentCount + "/" + allTextures.Count.ToString();


        yield return null;
    }
}
