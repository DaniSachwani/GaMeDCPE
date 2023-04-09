using System.Collections;
using System.Collections.Generic;
public class StageSession{

    public StageSession(string currentSide, string currentStage){
        CurrentSide = currentSide;
        CurrentStage = currentStage;
    }
    
    public string CurrentSide;
    public string CurrentStage;
    public CellStatus[][] Player1Grid;
    public CellStatus[][] Player2Grid;
    public int NumberofPlayersToPlace = 0;
    
    public List<int> Player1Xs= new List<int>();
    public List<int> Player1Ys= new List<int>();
    public List<int> Player2Xs= new List<int>();
    public List<int> Player2Ys= new List<int>();

    public int NoOfShotsPerTurn = 3;
     
}