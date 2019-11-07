using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{
    private GamePiece piece; //Variable to store a reference of the GamePiece script that this GameObject has
    IEnumerator moveCoroutine; //Variable to store the coroutine (Used in the Move Function)

    private void Awake()
    {
        piece = GetComponent<GamePiece>(); //Get the reference of the GamePiece Script in the Gameobject Instance.
    }

    //Move Piece to new position
    //Takes in the new X and Y positions with the time taken for the GameObject to finish moving
    public void Move(int newX, int newY, float time) 
    {
        //Stop the current coroutine if there is a existing coroutine running
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }


        moveCoroutine = MoveCoroutine(newX, newY, time); //Store a new coroutine
        StartCoroutine(moveCoroutine); //Re run the Coroutine
    }

    IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        piece.x = newX; //Update the new X value
        piece.y = newY; //Update the new Y value

        //Storing Variables for Lerping
        Vector3 curentPos = transform.position; //Store the current position of this GameObject
        Vector3 destination = piece.grid.MoveWorldPosition(newX, newY); //Store the Destination which is calculated with the MoveWorldPosition() function

        //Loop throught the time. 0 is at the starting time while 1 * time is the ending time
        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            //I used to use lerp where the last condition i = the fraction of the distance between the start and end
            //This is a new way I have discovered to implement lerp in Unity, glad to have my old study notes
            //Instead of using the fraction of the Destination, the tutorial teaches to use the fraction of the time instead
            piece.transform.position = Vector3.Lerp(curentPos, destination, t / time);

            yield return 0;
        }

        //Confirmation Code that the piece has moved to its destination
        piece.transform.position = destination;

    }
}
