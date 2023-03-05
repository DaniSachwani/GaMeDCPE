using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class StageSelectManager : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource AudioSource;
    GameObject PlayButton;    
    GameObject StageSelect;
    GameObject SideSelect;
    GameObject SideSelector;
    public bool isOnStageSelect = true;
    void Start()
    {
        PlayButton =GameObject.FindGameObjectWithTag("Play");
        StageSelect =GameObject.FindGameObjectWithTag("StageSelect");
        SideSelect =GameObject.FindGameObjectWithTag("SideSelect");
        SideSelector =GameObject.FindGameObjectWithTag("Selector");
        SideSelect.SetActive(false);
        StageSelect.SetActive(true);
        isOnStageSelect = true;
        PlayButton.GetComponent<TextMeshProUGUI>().text = "Done";
    }
    public string currentstage = "";
    public string currentside = "police";
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBackClicked(){
        if(isOnStageSelect){
            SceneManager.LoadScene("MainScreen");
            Debug.Log("Scene changed");
        }else{            
            PlayButton.GetComponent<TextMeshProUGUI>().text = "Done";        
            isOnStageSelect = true;
            SideSelect.SetActive(false);
            StageSelect.SetActive(true);
        }
    }

    public void OnImageClicked(){    
    }

    public void OnChorImageClicked(){
        currentside = "chor";
        Transform T =SideSelector.GetComponent<Transform>();
        T.localPosition = new Vector3(180.0f,20.0f,0.0f);
    }
    
    public void OnDoneClicked(){
        
        if(isOnStageSelect){
            PlayButton.GetComponent<TextMeshProUGUI>().text = "Play";        
            currentstage = "HighRiseEncounter";    
            isOnStageSelect = false;
            SideSelect.SetActive(true);
            StageSelect.SetActive(false);

        }else{            
            GameSystemManager.InitializeSelection(currentside,currentstage);
            SceneManager.LoadScene(currentstage);
        }
    }
    public void OnPoliceImageClicked(){
        currentside = "police";
        Transform T =SideSelector.GetComponent<Transform>();
        T.localPosition = new Vector3(-180.0f,20.0f,0.0f);
    }
}
