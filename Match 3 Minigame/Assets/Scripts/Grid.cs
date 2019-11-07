using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This Cless manages the events that happens in the Grid
//By Chua Joshua
//Reference from Match 3 in Lynda
public class Grid : MonoBehaviour
{
    public enum PieceType //Enum to identify part of the piecetype
    {
        NORMAL,
        EMPTY,
        OBSTACLE
    };

    [System.Serializable]
    public struct PiecePrefab //Struct to store each prefab for each Piecetype
    {
        public PieceType type;
        public GameObject prefab;
    }

    public PiecePrefab[] piecePrefabStructArray;//Array to Store the different piecePrefabs Types (Displayed in the Inspector under the Grid Script Component). Game Designers that wants to add more PieceTypes can be done in the Inpector.

    //Size of the Grid
    public int colX; //The number of columns the Grid should have
    public int rowY; //The number of rows the Grid should have

    public float movetime; //the time of the moving animation. This also determines the speed of the animation since I am using lerp to move the pieces

    //Background of the Grid
    public GameObject backgroundSprite; //Store the backgorund piece of the grid
    public GameObject gridBackground; //Store the GameObject that later holds all the background sprite of the grid

    public GamePiece[,] pieces; //2D Array to store the GamePieces in the Grid

    private Dictionary<PieceType, GameObject> piecePrefabDict;
    
    //Mouse Interaction Variables
    private GamePiece onMouseDownPiece; //Use to store the game piece that has been clicked on
    private GamePiece mouseDragToPiece; //Use to store the game piece that the mouse has dragged to after clicking on a piece

    private bool inverse = false; //To change direction for looping pieces

    //Other grid Components
    public UIManager uiManager; //Use to store the UI Manager component for reference

    //Death Particle System
    public ParticleSystem destroyParticles; //Use to store the Particle System which would be instantiated in the Clearable Piece Scripte for Daily Activity Type Pieces.

    //Points
    public int pointsGainWhenMatched; //Store the amount of points the player gets for each piece he matches
    
    // Start is called before the first frame update
    private void Start()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();//Store a new instance of the Dictionary

        for (int i = 0; i < piecePrefabStructArray.Length; i++) //Loop through the PiecePrefabStruct Array
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabStructArray[i].type)) //If the PiecePrefab Dictionary doesn't contain a PiecePrefab in the Array (indicated manually in the Inspector)
            {
                piecePrefabDict.Add(piecePrefabStructArray[i].type, piecePrefabStructArray[i].prefab); //Add the Dictionary key and its respective prefab indicated in the struct through the Inspector in the Array.
            }
        }

        GenerateGrid(); //Call the Generate Grid function which generates a grid
        GenerateSprites(); //Call the Generate sprites function which Generates Empty Sprites for the Grid

        StartCoroutine(FillBoard()); //Start the Coroutine which fills the board with pieces


    }

    //This function generates the grid using for loops and instantiate a background sprite
    //It works by looping through the columns and then the rows and instantiate a background sprite
    private void GenerateGrid()
    {
        for (int x = 0; x < colX; x++) //Loop through the Columns
        {
            for (int y = 0; y < rowY; y++) //Loow through the Rows
            {
                GameObject bg = Instantiate(backgroundSprite, MoveWorldPosition(x, y), Quaternion.identity); //Instantiate the background sprite to the position
                bg.transform.parent = gridBackground.transform; //set its parent to an the gridbackground (This makes the hierachy looks neat)
            }
        }
    }

    //This function will generate the Empty sprites onto the grid.
    //This function is called at the start of the game.
    //Type Pieces will fall in with the help from other functions
    private void GenerateSprites()
    {
        pieces = new GamePiece[colX, rowY]; //Store a new instance of the array with the length and height stated

        for (int x = 0; x < colX; x++) //Loop through each columns 
        {
            for (int y = 0; y < rowY; y++) //Loop through each rows
            {
                SpawnPiece(x, y, PieceType.EMPTY); //Spawn an empty piece at that position
            }
        }
    }

    //This Function moves the point of origin of the grid to be the center of the grid
    //With this function, the center of the grid would have a position of x =0 and y = 0
    //Furthermore, it spawns the grid from top to bottom
    //This function was created with the help of Lynda
    public Vector2 MoveWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - colX / 2 + x, transform.position.y + rowY / 2 - y);

        //For X position: Takes the position of the piece and move it to the left by half of the grid, and arrange it accordingly to the x input

        //For Y Position: Takes the position of the piece and move it up by half of the grid, and arrange it according to the y input
    }

    //This Function spawns a piece and returns the GamePiece Spawned.
    //It instantiates and initializes the piece
    public GamePiece SpawnPiece(int x, int y, PieceType type)
    {
        GameObject piece = Instantiate(piecePrefabDict[type], MoveWorldPosition(x, y), Quaternion.identity); //Instantiate the prefab using the key and place it at the position stated in the condition of this function
        piece.transform.parent = transform; //set the parent of the instantiated object to this gameobejct
        //piece.name = "Pieces(" + x + "," + y + ")"; //Rename the piece (This enable me to see the type of piece in the hierachy however, this )

        pieces[x, y] = piece.GetComponent<GamePiece>(); //Store the Gamepiece into the 2D array
        pieces[x, y].Initialize(x, y, this, type); //Initialize its position, grid and the piece type

        return pieces[x, y]; //Return this Game Piece
    }

    #region Filling Board FillBoard() and FillOnce()

    //This coroutine to fill up the board whenever there is an empty space in the board
    public IEnumerator FillBoard()
    {
        bool needsRefill = true; //Set the bool to true as the board needs to be refilled
        while (needsRefill) //While the board needs to be refilled
        {

            while (FillOnce())
            {
                inverse = !inverse; //the inverse bool is used for checking the columns from left and right
                yield return new WaitForSeconds(movetime); //Input the time taken for each piece movement
            }
            needsRefill = MatchAllMatches();
        }

    }

    //This function loops through all the pieces and move the pieces accordingly. 
    //It judges whether a piece should move down, diagonally, not move.
    //This judges all the pieces currently in the grid when this function is called 
    public bool FillOnce()
    {
        bool movedPiece = false; //This bool represents whether a piece has been moved

        for (int y = rowY - 2; y >= 0; y--) //Loop through the rows of the grid inversely (bottom to top , from second row from the bottom to the top), ignore the bottom row since it can't be pushed down anymore.
        {
            for (int loopX = 0; loopX < colX; loopX++) //Loop through the coloumns of the grid
            {
                int x = loopX; //Loop left to right

                if (inverse)
                {
                    x = colX - 1 - loopX; //Loop Right to left (The colx -1(cause the index starts from 0) and  -loopx (to get a selected grid piece from the right)
                }

                GamePiece currentPiece = pieces[x, y]; //Store the current piece why taking its x and y values from the for loop and putting it as the index for the 2d array

                if (currentPiece.IsMovable()) //Check if the current piece is movable
                {
                    GamePiece pieceBelow = pieces[x, y + 1]; //Piece Below the current piece looping

                    if (pieceBelow.type == PieceType.EMPTY) //If the piece below is an empty piece
                    {
                        Destroy(pieceBelow.gameObject); //Destroy the empty piece below as the current piece replaces the position of this piece
                        currentPiece.movablePieceScript.Move(x, y + 1, movetime); //move the cuurent piece below

                        //currentPiece.name = pieceBelow.name;  //Replace the current piece name to the name of the piece below(the name shows the position of the piece in the grid) 

                        pieces[x, y + 1] = currentPiece; //Update the 2d array where the cuurent piece takes position of the bottom piece
                        SpawnPiece(x, y, PieceType.EMPTY); // spawn a new empty piece at the previous position of the current piece which is at the top of the current piece now
                        movedPiece = true; //Marked the bool as true as a piece has been moved

                        
                    }
                    else //If it is not an empty piece below 
                    {
                        for (int diag = -1; diag <= 1; diag++) //This loop is to loop the sides of the current piece, represented visually below:
                                                               //   diag = -1   |    Current Piece      |  diag = 1
                                                               //   diag = -1   | Piece Below != Empty  |  diag = 1
                                                               //The piece beside the current piece and diagonal pieces will have the save x axis as they are on the same column
                        {
                            if (diag != 0) //0 is the center which has been check above
                            {
                                int diagX = x + diag; // Get the x axis beside the current piece (diagonal piece)

                                if (inverse) //if inverse equals true
                                {
                                    diagX = x - diag; //Check the pieces the other way around first so, check the right diagonalX then the left
                                }

                                if (diagX >= 0 && diagX < colX) //Check if the x axis of the diangonal piece is withing the board range
                                {
                                    GamePiece diagonalBottomPiece = pieces[diagX, y + 1]; //Store the reference of the Diagonal Piece

                                    if (diagonalBottomPiece.type == PieceType.EMPTY) //IF the Diagonal Piece is Empty
                                    {
                                        bool hasMovablePieceAbove = true; //This bool is to check if there is a piece above the diagonal piece
                                        
                                        //This loop is to check whether there is an obstacle above
                                        //If there is an obstacle piece above, this current piece would be able to move to the side (marking the boolean as false since there are no movable pieces)
                                        //However, if there are movable pieces, the movable piece will take over the diagonal piece as the game goes on, hence there is no need to fill the diagonal piece

                                        for (int aboveY = y; aboveY >= 0; aboveY--)// loop to all the pieces above the diagonal piece
                                        {
                                            GamePiece pieceAboveDiagonalPiece = pieces[diagX, aboveY]; //Store the piece its looping currently in this loop

                                            if (pieceAboveDiagonalPiece.IsMovable()) //If the piece above the diagonal piece is movable
                                            {
                                                //break the loop as there is a piece and the movalble piece which could be another piece type could move down
                                                break; 
                                            }
                                            else if (!pieceAboveDiagonalPiece.IsMovable() && pieceAboveDiagonalPiece.type != PieceType.EMPTY) //However, if the piece above is not movable and is not empty (obstacle)
                                            {
                                                //This means that there is an obstacle above this piece and no movable pieces, hence, mark it as there is no movable pieces above
                                                hasMovablePieceAbove = false;
                                                break;
                                            }
                                            //This loop will not check for empty pieces
                                        }

                                        if (hasMovablePieceAbove == false) //If there are no movable piece above the diagonal piece and there is an obstacle piece, this means that the current piece can take over the diagonal piece
                                        {
                                            Destroy(diagonalBottomPiece.gameObject);  //Destroy the diagonal bottom piece (which would be an empty piece)
                                            currentPiece.movablePieceScript.Move(diagX, y + 1, movetime); //move the cuurent piece to the diagonal piece position

                                            //currentPiece.name = diagonalBottomPiece.name; //Replace the current piece name to the name of the diagonal piece(the name shows the position of the piece in the grid)

                                            pieces[diagX, y + 1] = currentPiece; //Update the 2d array where the curent piece takes position of the bottom piece
                                            SpawnPiece(x, y, PieceType.EMPTY); // spawn a new empty piece at the previous position of the current piece
                                            movedPiece = true; //Piece has been moved
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        //This code is to fill the top row, where y = 0

        for (int x = 0; x < colX; x++) //loop through the top of the rows
        {
            GamePiece firstRowPiece = pieces[x, 0]; //store the current piece looped
            if (firstRowPiece.type == PieceType.EMPTY) //if the current pice is empty
            {             
                //The code here just replaces the empty top row pieces with a type piece

                GameObject newPiece = Instantiate(piecePrefabDict[PieceType.NORMAL], MoveWorldPosition(x, -1), Quaternion.identity); //Instantiante a piece on top of the empty piece. y = -1 as a temporary row above the top row of the grid
                newPiece.transform.parent = transform; //Make the grid the parent of this object

                //newPiece.name = firstRowPiece.name; //Replacing of names
                Destroy(firstRowPiece.gameObject); //Destroy the empty piece gameobject as a new piece would take its place

                pieces[x, 0] = newPiece.GetComponent<GamePiece>(); //the new piece take position of the current piece in the 2d array
                pieces[x, 0].Initialize(x, -1, this, PieceType.NORMAL); //initialize this new piec
                pieces[x, 0].movablePieceScript.Move(x, 0, movetime); //move the piece to the current piece position
                pieces[x, 0].dailyActivityPieceScript.SetDailyActivity((DailyActivityPiece.DailyActivityType)Random.Range(0, pieces[x, 0].dailyActivityPieceScript.dailyActivityStructs.Length)); // set the dailyActivity of the current piece to a random dailyactiveity piece
                movedPiece = true; //Piece has been moved
            }
        }

        return movedPiece; //return whether the piece has been moved
    }

    #endregion

    //Checking for neghbours using its x and y positions
    public bool IsNeighbour(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.x == piece2.x && Mathf.Abs(piece1.y - piece2.y) == 1) // Check if the X axis of the 2 piece are the same and they are next to eat other top and bottm
            || (piece1.y == piece2.y && Mathf.Abs(piece1.x - piece2.x) == 1);//OR Check if the Y axix of the 2 pieces are the same and they are next to each other. 

        //-1 as for example, if 2 piece are top and bottom with each other, one of the piece would have an axis greater than the other by 1
    }

    //Swapping 2 pieces positions
    //This function takes in the position of the 2 and interchanges them
    //Runs the GetMatch() function to check whether the pieces matches
    //Moves the pieces accoridingly if it matches and revert the movement if there is no matches
    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (piece1.IsMovable() && piece2.IsMovable()) //Check if both pieces are movable
        {
            //Switch the position of the pieces in the 2D array
            pieces[piece1.x, piece1.y] = piece2; //Swap the 2d array position of piece 2 to piece 1
            pieces[piece2.x, piece2.y] = piece1; //Swap the 2d array position of piece 1 to piece 2

            if (GetMatch(piece1, piece2.x, piece2.y) != null || GetMatch(piece2, piece1.x, piece1.y) != null) //Check if the List return from the GetMatch() Function is empty (If is not Empty means there is a match)
            {         
                //When there is a Match
                //Store the 1st piece positions in order for the second piece to move correctly(as when the first piece is moved, its position will change and the second piece is the same place as the first piece)
                int piece1OriginalX = piece1.x;
                int piece1OriginalY = piece1.y;

                piece1.movablePieceScript.Move(piece2.x, piece2.y, movetime); //Move piece 1 to the piece 2 position
                piece2.movablePieceScript.Move(piece1OriginalX, piece1OriginalY, movetime); //Move the piece 2 to the piece 1 original positions


                MatchAllMatches(); //Match all the Matches
                StartCoroutine(FillBoard()); //Fill the board again with the coroutine, since there are matches

            }
            else //if there are no matches (the list return from the GetMatch() function is null)
            {
                //Return the pieces to thier original 2d array positions
                pieces[piece1.x, piece1.y] = piece1; 
                pieces[piece2.x, piece2.y] = piece2;

                StartCoroutine(NoMatchFeedback(piece1, piece2)); //Play the animation where the pieces move to each other's position and move back (to show that there are no matches)
            }           
        }
    }

    //This function is an animation which switches 2 selected pieces and move them to each other's position
    //Then, after 0.25 seconds, it switches back to thier normal positions
    //THis animation is to feedback to the player that there are no Matches
    IEnumerator NoMatchFeedback(GamePiece piece1, GamePiece piece2)
    {
        //Store the 1st piece positions in order for the second piece to move correctly(as when the first piece is moved, its position will change and the second piece is the same place as the first piece)
        int piece1OriginalX = piece1.x;
        int piece1OriginalY = piece1.y;

        piece1.movablePieceScript.Move(piece2.x, piece2.y, movetime); //Move piece 1 to the piece 2 position
        piece2.movablePieceScript.Move(piece1OriginalX, piece1OriginalY, movetime); //Move the piece 2 to the piece 1 original positions

        yield return new WaitForSeconds(0.25f); //After 0.25 seconds, switch back the pieces

        piece2.movablePieceScript.Move(piece1.x, piece1.y, movetime); //Move piece 2 back to its original position where piece 1 is now
        piece1.movablePieceScript.Move(piece1OriginalX, piece1OriginalY, movetime); //Move piece 1 back to its original position stored previously
    }

    //Input the piece that was clicked on mouse down
    public void PressPiece(GamePiece piece)
    {
        onMouseDownPiece = piece;
    }

    //Input the last piece that the player has dragged to
    public void MouseToPiece(GamePiece piece)
    {
        mouseDragToPiece = piece;
    }

    //Check if the pieces are neighbours, if yes, swap them
    public void CheckNeighbour()
    {
        if (IsNeighbour(onMouseDownPiece, mouseDragToPiece)) //if the 2 piece are neghbours
        {
            SwapPieces(onMouseDownPiece, mouseDragToPiece); //Swap the 2 pieces
        }
    }

    //This function returns a list of pieces that have been matched
    //This would be called under the SwapPieces() function as whenever the player swap 2 pieces, this function will run and check for matches around that area.
    public List<GamePiece> GetMatch(GamePiece currentPiece, int newX, int newY) //The conditions take in the current piece and another piece's x and y positions
    {
        if (currentPiece.IsDefined()) //If the current piece is a DailyActivityType Piece
        {
            DailyActivityPiece.DailyActivityType currentDailyActivity = currentPiece.dailyActivityPieceScript.dailyActivity; //Store the DailyActivityType of the current selected piece
            List<GamePiece> horizontalPieces = new List<GamePiece>(); //Use to store all the matching pieces that are in the same row (y) as the current piece
            List<GamePiece> verticalPieces = new List<GamePiece>(); //Use to store all the matching pieces that are in the same coloumn (x) as the current piece
            List<GamePiece> matchingPieces = new List<GamePiece>(); //Use to store the matching pieces that can be destroyed as it meet the criteria where 3 or more pieces are matched

            horizontalPieces.Add(currentPiece); //Add the current piece into the horizontal list for checking

            //This for loop stores the matching pieces that have the same DailyActivityType as the current piece and are next to each other horizontally
            for (int dir = 0; dir <= 1; dir++) //dir = 0 is for left. dir = 1 is for right for horizontal checking. 
            {
                //This loop is to check the columns. 
                for (int xOffeset = 1/*Start with 1 cause 0 was our current piece*/; xOffeset < colX; xOffeset++) //Loop through the column before it goes out of range of the grid
                {
                    int x; //This variable is to store the x position of the pieces that are used for checking 

                    if (dir == 0) //if direction is 0, means the code will check the pieces on the left
                    {
                        x = newX - xOffeset; //loop the left
                    }
                    else //Direction is 1, means the code will check the pieces on the right
                    {
                        x = newX + xOffeset; //loop the right
                    }

                    if (x < 0 || x >= colX) //if the loop goes out of range of the grid, break the loop
                    {
                        break;
                    }

                    if (pieces[x, newY].IsDefined() && pieces[x, newY].dailyActivityPieceScript.dailyActivity == currentDailyActivity) //If the piece looped is a DailyActivityType and it has the same type of DailyActivityType as the current piece
                    {
                        horizontalPieces.Add(pieces[x, newY]); //Add the piece to the horizontal pieces list for checking later
                    }
                    else //Break if the piece looped does not have the same DailyActivityType as the current piece
                    {
                        break; 
                    }
                }
            }

            //This is to check if the pieces meet the match 3 criteria, when there are 3 or more pieces that are the same next to each other, it will be a match
            if (horizontalPieces.Count >= 3) //If the count of the list is 3 or greater,this means that there is a match
            {
                for (int i = 0; i < horizontalPieces.Count; i++) //Loop through the horizintalPieces List
                {
                    matchingPieces.Add(horizontalPieces[i]); //Add them into the matchingPieces
                }
            }

            if (matchingPieces.Count >= 3) //If the matching pieces is more than 3,
            {
                return matchingPieces; //the code returns true that there are matching pieces
            }

            //Next, we check vertically. 

            verticalPieces.Add(currentPiece); //Add the current piece into the Vertical list for checking

            for (int dir = 0; dir <= 1; dir++) //dir = 0 is for down. dir = 1 is for up for Vertical checking. 
            {
                for (int yOffeset = 1/*Start with 1 cause 0 was our current piece*/; yOffeset < colX; yOffeset++) //Loop through the row before it goes out of range of the grid
                {
                    int y; //This variable is to store the y position of the pieces that are used for checking

                    if (dir == 0) //if direction is 0, means the code will check the pieces on the bottom
                    {
                        y = newY - yOffeset; //loop the left
                    }
                    else
                    {
                        y = newY + yOffeset; //Direction is 1, means the code will check the pieces on the top
                    }

                    if (y < 0 || y >= rowY) //if the loop goes out of range of the grid, break the loop
                    {
                        break;
                    }

                    if (pieces[newX, y].IsDefined() && pieces[newX, y].dailyActivityPieceScript.dailyActivity == currentDailyActivity) //If the piece looped is a DailyActivityType and it has the same type of DailyActivityType as the current piece
                    {
                        verticalPieces.Add(pieces[newX, y]); //Add the piece to the vertical pieces list for checking later
                    }
                    else //Break if the piece looped does not have the same DailyActivityType as the current piece
                    {
                        break;
                    }
                }
            }

            //This is to check if the pieces meet the match 3 criteria, when there are 3 or more pieces that are the same next to each other on the top and bottom, it will be a match
            if (verticalPieces.Count >= 3) //If the count of the list is 3 or greater,this means that there is a match
            {
                for (int i = 0; i < verticalPieces.Count; i++) //Loop through the verticalPieces List
                {
                    matchingPieces.Add(verticalPieces[i]); //Add them into the matchingPieces
                }
            }

            if (verticalPieces.Count >= 3) //If the matching pieces is more than 3,
            {
                return matchingPieces; //the code returns true that there are matching pieces
            }
        }
        return null; //Return null when thee are no matching pieces 

    }

    //This function helps to match all the matches in the grid, and returns a boolean on whether the grid needs to be refilled
    //If there are no matches, this function will return false
    //However, if there are matches, the grid needs to be refilled as there are spaces in the grid, hence it returns true
    //The boolean will then be used in the FillOnce() Coroutine to tell the loop whether the board still needs to be filled or not
    public bool MatchAllMatches()
    {
        bool boardNeedsRefill = false; //This bollean is to check whether the grid needs to be refill.

        for (int y = 0; y < rowY; y++) //loop through the rows of the boards
        {
            for (int x = 0; x < colX; x++) //loop through the columns of the boards
            {
                if (pieces[x, y].IsClearable() && pieces[x, y].IsDefined()) //if the current piece is clearable and is a DailyActivityPiece,
                {
                    List<GamePiece> checkCurrentMatch = GetMatch(pieces[x, y], x, y); //If the current piece is clearable, check if there are any matches within this current piece and return a list of matches

                    if (checkCurrentMatch != null) //If the list is not null, means that there are matches
                    {
                        for (int i = 0; i < checkCurrentMatch.Count; i++) //Loop through all the pieces in the matchingList
                        {
                            if (ClearPiece(checkCurrentMatch[i].x, checkCurrentMatch[i].y)) //Clear the current piece selected in the matchingList. ClearPiece
                            {
                                boardNeedsRefill = true; //After clearing, there are spaes in the board, hence set the bool to true as the board needs refilling
                            }

                            uiManager.UpdateProgressBar(pointsGainWhenMatched); //Update the progress bar of the Game. Every pieces matched is one point. Hence, there would be a new score and thus, needing an update.
                        }
                    }
                }
            }
        }
        return boardNeedsRefill; //Return the boolean on whether the board needs to be refilled
    }

    //This function is used to Destroy the current piece (state the x and y position in the condition) and replace it with an Empty Piece
    //This function is used by the MatchAllMatches() function which returns a bool that represents a piece being cleared
    public bool ClearPiece(int x, int y)
    {
        if (pieces[x, y].IsClearable() && !pieces[x, y].clearablePieceScript.isBeingCleared) //if the piece has the clearable piece script component and currently, its not being cleared
        {
            pieces[x, y].clearablePieceScript.Clear(); //Clear the piece using the Clear() Function in the clearable piece script


            SpawnPiece(x, y, PieceType.EMPTY); //Spawn an Empty Piece at the position of the piece that is being cleared
            

            return true; //Return true to represent that the piece has been cleared
        }
        else
        {
            return false; //Return false to represent that the piece is not cleared
        }
    }

}
