using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
public static class GameSystemManager
{
    public static int ClipNo =0;
    public static AudioClip[] GameAudioClips;
    
    public static AudioSource AudioSource = null;
    public static void InitializeSounds(AudioClip[] AudioClips ){
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
}
