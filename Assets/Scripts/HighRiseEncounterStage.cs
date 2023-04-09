using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine.SceneManagement;
public class HighRiseEncounterStage : MonoBehaviour, IStage
{
    // Start is called before the first frame update
    public GameObject[][] Windows;
    public GameObject[][] Windows2;
    public GameObject[] ChorReqruits;
    public GameObject[] PoliceReqruits;
    public GameObject Building;
    public GameObject Building2;
    public GameObject ExitMenu;
    public GameObject OptionsMenu;
    public GameObject MusicOption;
    public GameObject InGameOption;
    int NumberofPlayersToPlace =5;
    bool isPlayerPlacement = true;
    bool isShootout = false;
    GameObject ResetPlayer;
    GameObject PlayerPlayMenu;
    GameObject WinLoseMenu;
    GameObject CurrentObject;
    GameObject ObjectToBeDeleted;
    [SerializeField] GameObject WindowBrokenAnimPre;
    [SerializeField] GameObject ChorHitPre;
    [SerializeField] GameObject PoliceHitPre;
    bool IsOpponentsturn =false;
    int player1kills =0;
    
    int player2kills =0;
    int CurrentShots = 0;
    AnimationStatus CurrentAnimation = AnimationStatus.None;
    AudioSource Gunshot;
    void Start()
    {
        GameSystemManager.stageSession.NumberofPlayersToPlace = 5;  
        NumberofPlayersToPlace = GameSystemManager.stageSession.NumberofPlayersToPlace;
        ResetPlayer = GameObject.FindGameObjectWithTag("ResetPlayer");
        ResetPlayer.SetActive(false);
        Windows = new GameObject[7][];
        Windows2 = new GameObject[7][];
        GameSystemManager.stageSession.Player1Grid = new CellStatus[7][];
        GameSystemManager.stageSession.Player2Grid = new CellStatus[7][];
        Building =GameObject.FindGameObjectWithTag("Building1");
        Building2 =GameObject.FindGameObjectWithTag("Building2");

        ChorReqruits = GameObject.FindGameObjectsWithTag("ChorReqruits");
        PoliceReqruits = GameObject.FindGameObjectsWithTag("PoliceReqruits");

        for(int i=0;i< ChorReqruits.Length; i++){
            ChorReqruits[i].SetActive(GameSystemManager.stageSession.CurrentSide.Equals("chor"));
            PoliceReqruits[i].SetActive(!GameSystemManager.stageSession.CurrentSide.Equals("chor"));
        }
        
        for(int i=0;i< 7;i++){
            GameObject floor = Building.transform.Find((i+1)+"floor").gameObject;
            GameObject floor2 = Building2.transform.Find((i+1)+"floor").gameObject;
            Windows[i] = new GameObject[8];
            Windows2[i] = new GameObject[8];
            GameSystemManager.stageSession.Player1Grid[i] = new CellStatus[8];
            GameSystemManager.stageSession.Player2Grid[i] = new CellStatus[8];
            for(int j=0;j< 8;j++){
                Windows[i][j] = floor.transform.Find("Window"+(j+1)).gameObject;
                Windows2[i][j] = floor2.transform.Find("Window"+(j+1)).gameObject;
                GameSystemManager.stageSession.Player1Grid[i][j] = CellStatus.Empty;
                GameSystemManager.stageSession.Player2Grid[i][j] = CellStatus.Empty;
            }    
        }
        Building2.SetActive(false);
        
        PlayerPlayMenu = GameObject.FindGameObjectWithTag("PlayerPlayMenu");
        PlayerPlayMenu.SetActive(false);
        WinLoseMenu = GameObject.FindGameObjectWithTag("WinLoseMenu");
        WinLoseMenu.SetActive(false);
        ExitMenu = GameObject.FindGameObjectWithTag("ExitMenu");
        ExitMenu.SetActive(false);

        
        MusicOption =GameObject.FindGameObjectWithTag("MusicOption");
        InGameOption =GameObject.FindGameObjectWithTag("InGameOption");
        OptionsMenu = GameObject.FindGameObjectWithTag("OptionsMenu");
        OptionsMenu.SetActive(false);
        

        Gunshot = GameObject.FindGameObjectWithTag("Gunshot").GetComponent<AudioSource>();
        Gunshot.volume = GameSystemManager.InGameVolume;
    }

    public void BackToMainScreen(){
        SceneManager.LoadScene("MainScreen");
    }
    // Update is called once per frame
    void Update()
    {
        if(CurrentAnimation == AnimationStatus.WindowBroken){
            //fadeout sprite
            Image SR = CurrentObject.GetComponent<Image>();
            Color color= SR.color;
            if(color.a > 0.0f){
                color.a -= Time.deltaTime / 0.5f;
                SR.color = color;
                CurrentObject.transform.Translate(0,-100*Time.deltaTime,0);
            }     
            else{
                Destroy(CurrentObject);
                CurrentAnimation = AnimationStatus.None;
            }

        }
        if(CurrentAnimation == AnimationStatus.MoveRight){
            GameObject.FindGameObjectWithTag("Stage").transform.Translate(-900*Time.deltaTime,0,0);
            if(GameObject.FindGameObjectWithTag("Stage").transform.localPosition.x <= -450)
                CurrentAnimation = AnimationStatus.None;
        }
        if(CurrentAnimation == AnimationStatus.MoveLeft){        
            GameObject.FindGameObjectWithTag("Stage").transform.Translate(900*Time.deltaTime,0,0);
            if(GameObject.FindGameObjectWithTag("Stage").transform.localPosition.x >= 900)
                CurrentAnimation = AnimationStatus.None;
        }
    }
    GameObject previousSelector = null;
    GameObject Selector = null;

    int SelectedX= 0;
    int SelectedY= 0;
    
    public void OnWindowClicked(string index){
        
        if(IsOpponentsturn)
            return;
        string[] args = index.Split(",");
        SelectedY = Convert.ToInt32(args[0]);
        SelectedX = Convert.ToInt32(args[1]);
        
        if(!isShootout && GameSystemManager.stageSession.Player1Grid[SelectedY-1][SelectedX-1] == CellStatus.PlayerPlaced){
            SelectedX = 0;
            SelectedY = 0;
            Selector?.SetActive(false);
            return;
        }else if(isShootout && (GameSystemManager.stageSession.Player2Grid[SelectedY-1][SelectedX-1] == CellStatus.PlayerDead ||
                                GameSystemManager.stageSession.Player2Grid[SelectedY-1][SelectedX-1] == CellStatus.WindowBroken)){
            SelectedX = 0;
            SelectedY = 0;
            Selector?.SetActive(false);
            GameSystemManager.CancelShake =false;
            return;
        }

        if(NumberofPlayersToPlace != 0 || isShootout){
            previousSelector = Selector;
            if(isShootout){
                Selector = Windows2[SelectedY-1][SelectedX-1].transform.Find("Selector").gameObject;
            }else{
                Selector = Windows[SelectedY-1][SelectedX-1].transform.Find("Selector").gameObject;
            }
            Selector?.SetActive(true);            
            previousSelector?.SetActive(false);
        }

    }

    public void OnPlayerPlaced(){
        
        if(SelectedX==0 &&SelectedY==0 && NumberofPlayersToPlace!=0)
            return;

        if(NumberofPlayersToPlace == 5 && isPlayerPlacement){
            ResetPlayer.SetActive(true);
        }
        if(NumberofPlayersToPlace > 0){
            GameSystemManager.stageSession.Player1Grid[SelectedY-1][SelectedX-1] = CellStatus.PlayerPlaced;
            NumberofPlayersToPlace --;
            GameObject.FindGameObjectWithTag("RecruitsLeft").GetComponent<TextMeshProUGUI>().text = NumberofPlayersToPlace+ " recruits left";            
            PlayPlayerPlacedAnimation();
            
        }else{
            GameObject.FindGameObjectWithTag("PlayerPlacementMenu").SetActive(false);
            HideRecruits();
            GameSystemManager.PlaceOpponentPlayers();        
            Building2.SetActive(true);
            ViewRight();
            isPlayerPlacement=false;
            isShootout = true;
            PlayerPlayMenu.SetActive(true);
            SelectedX =0;
            SelectedY =0;        
            GameSystemManager.CancelShake =false;
        }
        
        if(NumberofPlayersToPlace == 0 && isPlayerPlacement){
            GameObject PlacePlayer = GameObject.FindGameObjectWithTag("PlacePlayer");
            PlacePlayer.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "Mission start";
            //isPlayerPlacement = false;
        }

    }

    public void OnPlayerShoot(bool isOpponent =false){
        if(SelectedX==0 &&SelectedY==0){
            GameSystemManager.CancelShake =false;   
            return;
        }
        GameSystemManager.CancelShake =true;
        CellStatus[][] reference = (isOpponent)? GameSystemManager.stageSession.Player1Grid : GameSystemManager.stageSession.Player2Grid;

        if(reference[SelectedY-1][SelectedX-1] == CellStatus.PlayerDead){
            return;
        }else if(reference[SelectedY-1][SelectedX-1] == CellStatus.PlayerPlaced){
            PlayerShootNonEmptyWindow(isOpponent);
            reference[SelectedY-1][SelectedX-1] = CellStatus.PlayerDead;
            
            if(isOpponent) player1kills++; else player2kills++;

            if(player1kills==5 || player2kills==5){
                PlayerPlayMenu.SetActive(false);
                WinLoseMenu.SetActive(true);
                if(player1kills==5){
                    WinLoseMenu.transform.Find("Win").gameObject.SetActive(false);
                }else{
                    WinLoseMenu.transform.Find("Lose").gameObject.SetActive(false);    
                }
                
                Gunshot.Play();
                return;
            }
        }else if(reference[SelectedY-1][SelectedX-1] == CellStatus.Empty){
            PlayerShootEmptyWindow(isOpponent);
            reference[SelectedY-1][SelectedX-1] = CellStatus.WindowBroken;

        }
        Gunshot.Play();
        SelectedX =0;
        SelectedY =0;        
        CurrentShots++;
        if(CurrentShots == GameSystemManager.stageSession.NoOfShotsPerTurn && !isOpponent){
            CurrentShots= 0;
            this.Invoke("PlayOpponentsTurn",1.0f);
            PlayerPlayMenu.SetActive(false);
            IsOpponentsturn=true;
        }
        
    }
    
    void PlayOpponentsTurn(){
        this.Invoke("ViewLeft",1.0f);
        this.Invoke("SelectRandomGrid",2.0f);
        this.Invoke("ViewRight",7.0f);
        this.Invoke("ShowShootControls",7.0f);
    }

    void ShowShootControls(){
        PlayerPlayMenu.SetActive(true);
        SelectedX =0;
        SelectedY =0;
    }
    void SelectRandomGrid(){
            System.Random R = new System.Random();
            do{
                SelectedY= R.Next(GameSystemManager.stageSession.Player1Grid.Length)+1;
                SelectedX= R.Next(GameSystemManager.stageSession.Player1Grid[0].Length)+1;
            }while (GameSystemManager.stageSession.Player1Grid[SelectedY-1][SelectedX-1]==CellStatus.PlayerDead ||
                    GameSystemManager.stageSession.Player1Grid[SelectedY-1][SelectedX-1]==CellStatus.WindowBroken);
            OnPlayerShoot(true);
            if(CurrentShots< GameSystemManager.stageSession.NoOfShotsPerTurn){
                this.Invoke("SelectRandomGrid",1.0f);
            }else{
                CurrentShots=0;
                IsOpponentsturn=false;
                SelectedX=0;
                SelectedY=0;
            }
    }
    void Shoot(){
        
    }

    void ViewRight(){
        CurrentAnimation = AnimationStatus.MoveRight;
        PlayerPlayMenu.transform.localPosition = new Vector3(-900.0f,0.0f,0.0f);
    }
    void ViewLeft(){
        CurrentAnimation = AnimationStatus.MoveLeft;
        PlayerPlayMenu.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
    }
    public void OnPlayerReset(){
        if(NumberofPlayersToPlace == 0 && isPlayerPlacement){
            GameObject PlacePlayer = GameObject.FindGameObjectWithTag("PlacePlayer");
            PlacePlayer.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "Place Player";
            //isPlayerPlacement = true;
        }
        if(NumberofPlayersToPlace < 5){
            GameSystemManager.stageSession.Player1Grid
                [GameSystemManager.stageSession.Player1Ys[GameSystemManager.stageSession.Player1Ys.Count-1]]
                [GameSystemManager.stageSession.Player1Xs[GameSystemManager.stageSession.Player1Xs.Count-1]] 
                    = CellStatus.Empty;

            NumberofPlayersToPlace ++;
            GameObject.FindGameObjectWithTag("RecruitsLeft").GetComponent<TextMeshProUGUI>().text = NumberofPlayersToPlace+ " recruits left";            
            PlayPlayerResetAnimation();
        }        
        if(NumberofPlayersToPlace == 5 && isPlayerPlacement){
            GameObject ResetPlayer = GameObject.FindGameObjectWithTag("ResetPlayer");
            ResetPlayer.SetActive(false);
        }

    }

    void HideRecruits(){
        for(int i=0;i<5;i++){
            int selectedX = GameSystemManager.stageSession.Player1Xs[i];
            int selectedY = GameSystemManager.stageSession.Player1Ys[i];

            Windows[selectedY][selectedX].transform.Find("WindowClosed").gameObject.SetActive(true);        
            Windows[selectedY][selectedX].transform.Find("black").gameObject.SetActive(false);
            Windows[selectedY][selectedX].transform.Find("WindowOpen").gameObject.SetActive(false);
            if(GameSystemManager.stageSession.CurrentSide == "chor")
            {
                Windows[selectedY][selectedX].transform.Find("Chor").gameObject.SetActive(false);
            }
            else
            {
                Windows[selectedY][selectedX].transform.Find("Police").gameObject.SetActive(false);            
            }
            
        }
    }

    void PlayPlayerPlacedAnimation(){
        GameSystemManager.stageSession.Player1Xs.Add(SelectedX-1);
        GameSystemManager.stageSession.Player1Ys.Add(SelectedY-1);
        Windows[SelectedY-1][SelectedX-1].transform.Find("Selector").gameObject.SetActive(false);        
        Windows[SelectedY-1][SelectedX-1].transform.Find("WindowClosed").gameObject.SetActive(false);        
        Windows[SelectedY-1][SelectedX-1].transform.Find("black").gameObject.SetActive(true);
        Windows[SelectedY-1][SelectedX-1].transform.Find("WindowOpen").gameObject.SetActive(true);

        this.Invoke("AnimateWindow",0.2f); 

    }

    void PlayerShootEmptyWindow(bool isOpponent){
        GameObject[][] CurrentWindows = isOpponent? Windows : Windows2;
        Debug.Log("Empty");
        CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("Selector").gameObject.SetActive(false);        
        CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("WindowClosed").gameObject.SetActive(false);        
        //Windows2[SelectedY-1][SelectedX-1].transform.Find("black").gameObject.SetActive(true);
        CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("WIndowBroken").gameObject.SetActive(true);

        CurrentObject = CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("WIndowBroken").gameObject;
        CurrentObject = Instantiate(WindowBrokenAnimPre, CurrentObject.transform.position, Quaternion.identity,CurrentObject.transform.parent);
        CurrentAnimation = AnimationStatus.WindowBroken;
    }
    void PlayerShootNonEmptyWindow(bool isOpponent){
        GameObject[][] CurrentWindows = isOpponent? Windows : Windows2;
        
        CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("Selector").gameObject.SetActive(false);        
        CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("WindowClosed").gameObject.SetActive(false);        
        //Windows2[SelectedY-1][SelectedX-1].transform.Find("black").gameObject.SetActive(true);
        CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("WIndowBroken").gameObject.SetActive(true);

        CurrentObject = CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("WIndowBroken").gameObject;
        CurrentObject = Instantiate(WindowBrokenAnimPre, CurrentObject.transform.position, Quaternion.identity,CurrentObject.transform.parent);
        CurrentAnimation = AnimationStatus.WindowBroken;
        GameObject result = null;
        GameObject CurrentObject1 = null;
        if(!isOpponent){
            GameObject Pref= null;
            if(GameSystemManager.stageSession.CurrentSide.Equals("chor")){
                CurrentObject1 = CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("Police").gameObject; 
                Pref= PoliceHitPre;           
            }else{
                CurrentObject1 = CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("Chor").gameObject;
                Pref= ChorHitPre;
            }
            result = Instantiate(Pref, 
                        new Vector3(
                            CurrentObject1.transform.position.x+1,
                            CurrentObject1.transform.position.y-4,
                            CurrentObject1.transform.position.z),
                        Quaternion.identity,CurrentObject1.transform.parent);
            
            
            result.transform.localPosition = new Vector3(
                                                            result.transform.localPosition.x+1,
                                                            result.transform.localPosition.y-3,
                                                            result.transform.localPosition.z
                                                        );
        
        }
        else{
            GameObject Pref= null;
            if(GameSystemManager.stageSession.CurrentSide.Equals("chor")){
                CurrentObject1 = CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("Chor").gameObject; 
                Pref= ChorHitPre;           
            }else{
                CurrentObject1 = CurrentWindows[SelectedY-1][SelectedX-1].transform.Find("Police").gameObject;
                Pref= PoliceHitPre;
            }
            result = Instantiate(Pref, 
                        new Vector3(
                            CurrentObject1.transform.position.x+1,
                            CurrentObject1.transform.position.y-4,
                            CurrentObject1.transform.position.z),
                        Quaternion.identity,CurrentObject1.transform.parent);

                        
            result.transform.localPosition = new Vector3(
                                                            result.transform.localPosition.x+1,
                                                            result.transform.localPosition.y-3,
                                                            result.transform.localPosition.z
                                                        );

        }
        ObjectToBeDeleted = result;
        this.Invoke("DeleteObject",0.5f); 
        
    }

    void DeleteObject(){
        Destroy(ObjectToBeDeleted);
        ObjectToBeDeleted=null;
    }
    void PlayPlayerResetAnimation(){
        int x = GameSystemManager.stageSession.Player1Xs
                    [GameSystemManager.stageSession.Player1Xs.Count-1];

        int y = GameSystemManager.stageSession.Player1Ys
                    [GameSystemManager.stageSession.Player1Ys.Count-1];
        
        GameSystemManager.stageSession.Player1Xs.RemoveAt
            (
                GameSystemManager.stageSession.Player1Xs.Count-1
            );

        GameSystemManager.stageSession.Player1Ys.RemoveAt
            (
                GameSystemManager.stageSession.Player1Ys.Count-1
            );

        Windows[y][x].transform.Find("WindowClosed").gameObject.SetActive(true);        
        Windows[y][x].transform.Find("black").gameObject.SetActive(false);
        Windows[y][x].transform.Find("WindowOpen").gameObject.SetActive(false);

        if(GameSystemManager.stageSession.CurrentSide == "chor")
        {
            Windows[y][x].transform.Find("Chor").gameObject.SetActive(false);
            ChorReqruits[NumberofPlayersToPlace-1].SetActive(true);
        }
        else
        {
            Windows[y][x].transform.Find("Police").gameObject.SetActive(false);            
            PoliceReqruits[NumberofPlayersToPlace-1].SetActive(true);
        }

    }

    void AnimateWindow(){
        if(GameSystemManager.stageSession.CurrentSide == "chor")
        {
            Windows[SelectedY-1][SelectedX-1].transform.Find("Chor").gameObject.SetActive(true);
            ChorReqruits[NumberofPlayersToPlace].SetActive(false);
        }
        else
        {
            Windows[SelectedY-1][SelectedX-1].transform.Find("Police").gameObject.SetActive(true);            
            PoliceReqruits[NumberofPlayersToPlace].SetActive(false);
        }
        SelectedX=0; 
        SelectedY=0;
    }

    public void ShowExitMenu(){
        ExitMenu.SetActive(true);
    }
    public void ShowOptionsMenu(){
        OptionsMenu.SetActive(true);
        
        Slider sliderMusic = MusicOption.GetComponent<Slider>();
        sliderMusic.value = GameSystemManager.AudioSource.volume;
        Slider sliderInGame = InGameOption.GetComponent<Slider>();
        sliderInGame.value = GameSystemManager.InGameVolume;
    }
    public void ExitMenuYesClicked(){
        SceneManager.LoadScene("MainScreen");
    }
    public void ExitMenuNoClicked(){
        ExitMenu.SetActive(false);
    }

    public void OnOptionsBackClicked(){
        OptionsMenu.SetActive(false);
        Gunshot.volume = GameSystemManager.InGameVolume;
        GameSystemManager.SaveData();
    }
    
    

    public void OnMusicVolumeChanged(){
        Slider slider = MusicOption.GetComponent<Slider>();
        GameSystemManager.SetMusicVolume(slider.value);
    }
    public void OnInGameVolumeChanged(){
        Slider slider = InGameOption.GetComponent<Slider>();
        GameSystemManager.SetInGameVolume(slider.value);
    }
    
}
