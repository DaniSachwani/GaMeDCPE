using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class GameSystemManager
{
    public static int ClipNo =0;
    public static AudioClip[] GameAudioClips;
    public static StageSession stageSession = null;
    public static string SessionType = "Local";
    public static AudioSource AudioSource = null;
    public static bool CancelShake = true;
    public static float InGameVolume = 1f;
    public static bool firsttime = true;
    public static void InitializeSounds(AudioClip[] AudioClips){
        LoadData();
        GameAudioClips = AudioClips;
        AudioSource.Play();
        ThreadPool.QueueUserWorkItem(o => { PlayClip(GameAudioClips[ClipNo]); }); 
    }

    public static void LoadData(){
        if (File.Exists(Application.persistentDataPath+"/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = 
                    File.Open(Application.persistentDataPath 
                    + "/MySaveData.dat", FileMode.Open);
            PlayerPreferences data = (PlayerPreferences)bf.Deserialize(file);
            file.Close();
            AudioSource.volume = data.MusicVolume;
            InGameVolume = data.InGameVolume; 
            firsttime = data.firsttime;  

        }
    }
    public static void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath 
                    + "/MySaveData.dat"); 
        PlayerPreferences data = new PlayerPreferences();
        data.MusicVolume = AudioSource.volume;
        data.InGameVolume = InGameVolume;
        data.firsttime = firsttime;
        bf.Serialize(file, data);
        file.Close();
    }
    public static void PlayClip(AudioClip clip)
    {
        AudioSource.Stop(); //stop previous clip
        AudioSource.clip = clip; //assign new clip
        AudioSource.Play();
    
        ThreadPool.QueueUserWorkItem(o => { Thread.Sleep((int)clip.length * 1000); EventOnEnd(); }); 
        
    }

    public static void EventOnEnd()
    {
        ClipNo++;

        if(ClipNo > GameAudioClips.Length){
            
            ClipNo=0;
        }
        PlayClip(GameAudioClips[ClipNo]);
    }

    public static void SetMusicVolume(float volume){
        AudioSource.volume = volume;
    }
    public static void SetInGameVolume(float volume){
        InGameVolume = volume;
    }
    public static void InitializeSelection(string currentside, string currentstage){
        stageSession = new StageSession(currentside,currentstage);
    }

    public static void PlaceOpponentPlayers(){
        if(SessionType.Equals("Local")){
            PlaceComputerPlayers();
        }
    }
    public static void PlaceComputerPlayers(){

        int number =stageSession.NumberofPlayersToPlace;
        System.Random R = new System.Random();
        for(int i=0;i<number;i++){
            int y=0,x=0;
            while(stageSession.Player2Grid[y][x] != CellStatus.Empty){
                y= R.Next(stageSession.Player2Grid.Length);
                x= R.Next(stageSession.Player2Grid[0].Length);
            }
            stageSession.Player2Grid[y][x]= CellStatus.PlayerPlaced;
            stageSession.Player2Xs.Add(x);
            stageSession.Player2Ys.Add(y);
        }      
        for(int i=0; i< stageSession.Player2Grid.Length;i++){
            String str= "";
            for(int j=0; j< stageSession.Player2Grid[i].Length;j++){
                if(stageSession.Player2Grid[i][j] == CellStatus.Empty){
                    str+=" 0";
                }else{
                    str+=" 1";    
                }
            }   
            Debug.Log(str);
        }
    }
}
