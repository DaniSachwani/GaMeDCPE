using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;
using TMPro;

public class HighRiseEncounterStage : MonoBehaviour, IStage
{
    // Start is called before the first frame update
    public GameObject[][] Windows;
    public GameObject[] ChorReqruits;
    public GameObject[] PoliceReqruits;
    int NumberofPlayersToPlace =5;
    bool isPlayerPlacement =true;
    GameObject ResetPlayer;
    void Start()
    {
        GameSystemManager.stageSession.NumberofPlayersToPlace = 5;  
        NumberofPlayersToPlace = GameSystemManager.stageSession.NumberofPlayersToPlace;
        ResetPlayer = GameObject.FindGameObjectWithTag("ResetPlayer");
        ResetPlayer.SetActive(false);
        Windows = new GameObject[7][];
        GameSystemManager.stageSession.Player1Grid = new CellStatus[7][];
        GameObject Building =GameObject.FindGameObjectWithTag("Building1");

        ChorReqruits = GameObject.FindGameObjectsWithTag("ChorReqruits");
        PoliceReqruits = GameObject.FindGameObjectsWithTag("PoliceReqruits");

        for(int i=0;i< ChorReqruits.Length; i++){
            ChorReqruits[i].SetActive(GameSystemManager.stageSession.CurrentSide.Equals("chor"));
            PoliceReqruits[i].SetActive(!GameSystemManager.stageSession.CurrentSide.Equals("chor"));
        }
        
        for(int i=0;i< 7;i++){
            GameObject floor = Building.transform.Find((i+1)+"floor").gameObject;
            Windows[i] = new GameObject[8];
            GameSystemManager.stageSession.Player1Grid[i] = new CellStatus[8];
            GameSystemManager.stageSession.Player2Grid[i] = new CellStatus[8];
            for(int j=0;j< 8;j++){
                Windows[i][j] = floor.transform.Find("Window"+(j+1)).gameObject;
                GameSystemManager.stageSession.Player1Grid[i][j] = CellStatus.Empty;
                GameSystemManager.stageSession.Player2Grid[i][j] = CellStatus.Empty;
            }    
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    GameObject previousSelector = null;
    GameObject Selector = null;

    int SelectedX= 0;
    int SelectedY= 0;
    
    public void OnWindowClicked(string index){
        string[] args = index.Split(",");
        SelectedY = Convert.ToInt32(args[0]);
        SelectedX = Convert.ToInt32(args[1]);

        if(isPlayerPlacement){
            previousSelector = Selector;
            Selector = Windows[SelectedY-1][SelectedX-1].transform.Find("Selector").gameObject;
            previousSelector?.SetActive(false);
            Selector?.SetActive(true);            
        }
    }

    public void OnPlayerPlaced(){
        
        if(SelectedX==0 &&SelectedY==0 && isPlayerPlacement)
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
        }
        
        if(NumberofPlayersToPlace == 0 && isPlayerPlacement){
            GameObject PlacePlayer = GameObject.FindGameObjectWithTag("PlacePlayer");
            PlacePlayer.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "Mission start";
            isPlayerPlacement = false;
        }

    }
    public void OnPlayerReset(){
        if(NumberofPlayersToPlace == 0 && isPlayerPlacement){
            GameObject PlacePlayer = GameObject.FindGameObjectWithTag("PlacePlayer");
            PlacePlayer.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "Place Player";
            isPlayerPlacement = true;
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
}
