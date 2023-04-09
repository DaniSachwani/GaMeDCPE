using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class PlayerPreferences{
    public float MusicVolume =1f;
    public float InGameVolume =1f;
    public bool firsttime =true;
}