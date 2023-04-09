using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject StartButton;
    public GameObject OptionsButton;
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameObject MusicOption;
    public GameObject InGameOption;
    [SerializeField] public AudioClip[] AudioClips;
    public AudioClip CurrentClip;
    public AudioSource AudioSource;
    void Start()
    {
        StartButton =GameObject.FindGameObjectWithTag("Start");
        OptionsButton =GameObject.FindGameObjectWithTag("Options");
        MainMenu =GameObject.FindGameObjectWithTag("MainMenu");
        OptionsMenu =GameObject.FindGameObjectWithTag("OptionsMenu");
        MusicOption =GameObject.FindGameObjectWithTag("MusicOption");
        InGameOption =GameObject.FindGameObjectWithTag("InGameOption");

        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        AudioSource = this.GetComponent<AudioSource>();
        if(GameSystemManager.AudioSource==null){
            Debug.Log("here");
            GameSystemManager.AudioSource = AudioSource;
            GameSystemManager.InitializeSounds(AudioClips);
            DontDestroyOnLoad(this.gameObject);
        }else{
            Slider slider = MusicOption.GetComponent<Slider>();
            slider.value = GameSystemManager.AudioSource.volume;
        }
    }

    public void OnStartClicked(){
        SceneManager.LoadScene("StageSelect");
    }
    public void OnOptionsBackClicked(){
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        GameSystemManager.SaveData();
    }
    
    public void OnOptionsClicked(){
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
        
        Slider sliderMusic = MusicOption.GetComponent<Slider>();
        sliderMusic.value = GameSystemManager.AudioSource.volume;
        Slider sliderInGame = InGameOption.GetComponent<Slider>();
        sliderInGame.value = GameSystemManager.InGameVolume;
    }

    public void OnMusicVolumeChanged(){
        Slider slider = MusicOption.GetComponent<Slider>();
        GameSystemManager.SetMusicVolume(slider.value);
    }
    public void OnInGameVolumeChanged(){
        Slider slider = InGameOption.GetComponent<Slider>();
        GameSystemManager.SetInGameVolume(slider.value);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

   
 
    
}
