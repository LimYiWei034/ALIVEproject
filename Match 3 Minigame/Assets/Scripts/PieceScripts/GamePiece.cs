using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script defines the pieces in the Grid of the Match 3. The value of the variables with functions checking whether this game piece has certain components
public class GamePiece : MonoBehaviour
{
    //Variables Used for References
    public int x; //Use to store the X pos of the piece
    public int y; //Use to store the Y pos of the piece
    public Grid.PieceType type; //Use to store the type of the piece
    public Grid grid; //Use to store the grid of the Match 3

    //Variables used to reference other different script componenets
    public MovablePiece movablePieceScript;
    public DailyActivityPiece dailyActivityPieceScript;
    public ClearablePiece clearablePieceScript;

    private void Awake()
    {
        //Getting and storing the Components at the Start of the game (Caching)
        //If this gameobject doesn't has a component, the variable will store null (For E.g an Empty Game Piece will not have a TypePiece Component. Hence, the value would be null)
        movablePieceScript = GetComponent<MovablePiece>();
        dailyActivityPieceScript = GetComponent<DailyActivityPiece>();
        clearablePieceScript = GetComponent<ClearablePiece>();
    }

    //Initialize Function defines the position of the piece in the grid and type of game piece.
    public void Initialize(int _x, int _y, Grid _grid, Grid.PieceType _type)
    {
        x = _x; //Define the X position in the grid
        y = _y; //Define the Y position in the grid
        grid = _grid; //The Type of Grid Class (For now, there is only 1 instance)
        type = _type; //The type of GamePiece this GameObject is
    }

    //Check if the this GameObject instance has the Movable Piece Script (This Object is Movable)
    //If it doesn't, means that this piece can't be moved.
    //One Example is the Obstacle/Bacteria Pieces which can't be moved
    public bool IsMovable()
    {
        //If there is a Movable Piece Script in this GameObject, it will return true
        //Else it will return false, meaning that the this Gameobject can't be moved
        return movablePieceScript != null;
    }

    //Check if this GameObject Instance is a Type Piece
    //If it is not a Type Piece, it will not have the Type Piece component
    //One Example is the Empty Piece.
    public bool IsDefined()
    {
        //If there is a Type Piece Script in this GameObject Instance, it will return true
        //Else it will return false, meaning that this Gameobject does not have a Daily Activity Type
        return dailyActivityPieceScript != null;
    }

    //Check if this GameObject Instance has the Clearable Piece Script (This Object is Clearable)
    //For Example, a Type Piece will have the Clearable Script as when it matched, it needs to be cleared
    public bool IsClearable()
    {
        //If there is a Clearable Piece Script in the GameObject Instance, it will return true
        //Else it will return false, meaning that this GameObject can't use the clearble script Clear() Function
        return clearablePieceScript != null;
    }

    //When the mouse is hovered onto this GameObject's collider, it will run the MouseToPiece() Function from the Grid Script.
    private void OnMouseEnter()
    {
        grid.MouseToPiece(this);
    }

    //When the Mouse Click on this GameObject's collider, it wil run the PressPiece() Function from the Grid Script.
    private void OnMouseDown()
    {
        grid.PressPiece(this);
    }

    //When the Mouse Click is release on this GameObject's collider, it will run the CheckNeighbour() Function from the Grid Script.
    private void OnMouseUp()
    {
        grid.CheckNeighbour();
    }
}
