using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StageSelectManager : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource AudioSource;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBackClicked(){
        SceneManager.LoadScene("MainScreen");
    }
}
