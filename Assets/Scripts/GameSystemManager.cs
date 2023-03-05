using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
public static class GameSystemManager
{
    public static int ClipNo =0;
    public static AudioClip[] GameAudioClips;
    public static StageSession stageSession = null;
    public static string SessionType = "Local";
    public static AudioSource AudioSource = null;
    public static void InitializeSounds(AudioClip[] AudioClips){
        GameAudioClips = AudioClips;
        AudioSource.Play();
        ThreadPool.QueueUserWorkItem(o => { PlayClip(GameAudioClips[ClipNo]); }); 
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

        for(int i=0;i<number;i++){
            int y= Math.Random(stageSession.Player2Grid.Length-1);
            int x= Math.Random(stageSession.Player2Grid[0].Length-1);
        }        
        // for(int i=0; i< stageSession.Player2Grid.Length;i++){
        //     for(int j=0; j< stageSession.Player2Grid[i].Length;j++){
                
        //     }   
        // }
    }
}
