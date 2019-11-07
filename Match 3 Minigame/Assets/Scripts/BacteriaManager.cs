using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BacteriaManager : MonoBehaviour
{
    public Grid grid; //Store the Grid component to reference variables and public function
    public Antibiotics antibiotics; //Use to store the Antibiotics component for reference

    //Antibiotics Destroying Bacteria
    //Bacteria Spawning Variables
    public int spawnBacteriaTime; //Time Interval for Bacteria to spawn on the grid

    public List<GamePiece> obstacleList; //List Use to store all the bacteria spawned in the Grid (This list is used for the antibiotics where it allows the antibiotics to randomly choose a bacteria to delete)

    int totalPiecesIntheGrid; //Use to store the total amount of pieces in the grid
    int NoOfBacteriaToGameOver; //Store the value of the number of bacteria where the player loses the game

    public int bacteriaPercentageToGameOver; //Use to store the percentage of how many bacteria are in the scene till game over (Can be edited in the inspector)
    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BacteriaWave()); //Start the Coroutine which spawns bacteria
        totalPiecesIntheGrid = grid.colX * grid.rowY; //Multiply and No of rows and columns of the grid to get the total number of pieces
        NoOfBacteriaToGameOver = totalPiecesIntheGrid * bacteriaPercentageToGameOver / 100; //Calculate the no of bacteria needed for the game to be over
    }


    //This Function Spawns Bacteria randomly onto the Grid
    //It checks whether the randmly selected piece is a normal piece type as there is no point replacing a bacteria with another bacteria
    private void SpawnBacteria()
    {
        bool bacteriaSpawned = false; //Have not spawned a bacteria

        int selectedPieceX = Random.Range(0, grid.colX); //Get a random X position
        int selectedPieceY = Random.Range(0, grid.rowY); //Get a random Y position

        GamePiece selectedPiece = grid.pieces[selectedPieceX, selectedPieceY]; //Get the randomly Selected Piece

        if (selectedPiece.type == Grid.PieceType.NORMAL && bacteriaSpawned == false) //Chekc if the piece selected is a normal piece and whether this bacteria has spawned
        {
            Destroy(selectedPiece.gameObject); //Destroy the current selected piece (I didn't use the clear() function from the clearable piece script as i didn't want particles to be playing)
            GamePiece currentBacteria = grid.SpawnPiece(selectedPieceX, selectedPieceY, Grid.PieceType.OBSTACLE); //Spawn the Bacteria at the selected piece position
            obstacleList.Add(currentBacteria); //Add the bacteria to the list of obstacles (which allows the antibiotics to randomly choose which bacteria to destroy if the antibiotics power is pressed)
            bacteriaSpawned = true; //A bacteria has been spawned
        }

        if (bacteriaSpawned == false) //If a bacteria has not been spawned (cause the randomly selected piece is a bacteria, call this function)
        {
            SpawnBacteria(); //Call this function to find another randomly selected piece.   
        }
    }

    //This Coroutine is use to spawn bacteria onto the grid in a certain time interval
    IEnumerator BacteriaWave()
    {
        while (true) //This coroutine is meant to loop throughout the game hence, the condition is left as true for this coroutine to keep looping
        {
            yield return new WaitForSeconds(spawnBacteriaTime); //Time interval
            SpawnBacteria(); //Spawn a bacteria after a certain time interval
            CheckGameOver();
        }
    }

    //This function is used by the antibiotics power where it destroys the bacteria in the grid
    public void KillBacteria()
    {
        for (int i = 0; i < antibiotics.antibioticsEffectiveness; i++) //Loop throught the amount of effetiveness the bacteria has
        {
            if (obstacleList.Count == 0) //If there are no obstacles, break the loop as there are no obstacles to destroy
            {
                break;
            }

            int randInd = Random.Range(0, obstacleList.Count - 1); //get a random index from the list (minus 1 as the list index starts from 0. Hence, if the count is 4, the list index would be 0, 1, 2, 3.)
            grid.SpawnPiece(obstacleList[randInd].x, obstacleList[randInd].y, Grid.PieceType.EMPTY); //Spawn an Empty Piece in the grid for the pieces to fall
            obstacleList[randInd].clearablePieceScript.Clear(); //Destroy the Bacteria
            obstacleList.RemoveAt(randInd); //Remove the destroyed bacteria from the list

        }

        //After Destroying the bacteria, the pieces will move. Hence, the code has to check:
        grid.MatchAllMatches(); //If there are any matches
        StartCoroutine(grid.FillBoard()); //and if there are for that column, the grid has to have its top piece filled
    }

    //Check if the Player has too many bacteria in the grid
    //If yes, Game over
    public void CheckGameOver()
    {
        if (obstacleList.Count >= NoOfBacteriaToGameOver) //Check if the obstacle list has the amount of bacteria inside which meets the game over requirement
        {
            Debug.Log("u loose"); //No Ui at the moment, hence i used the Debug.log (UI was hendled by a UI designer hired by our client. Unfortunately, they stop working on the project with us)
            antibiotics.currentProgress = 0; //Reset the progress to 0 (Restart the game)
            //Restart Function
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Load this scene

        }
    }
}
