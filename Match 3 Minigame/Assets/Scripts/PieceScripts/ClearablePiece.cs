    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearablePiece : MonoBehaviour
{

    public bool isBeingCleared; //Boolean to check if the object is being cleared
    public GamePiece piece; //us to store the reference of this GameObject's Game Piece script

    private void Awake()
    {
        piece = GetComponent<GamePiece>(); //Store the GamePiece Script of this GameObject.
        
    }

    //This Function Clears this Piece from the Grid
    private void ClearPiece()
    {
        Destroy(gameObject); //Destroy the GameObject

        //If this GamePiece is a normal piece and it is a Daily Activity Piece Type.
        if (piece.type == Grid.PieceType.NORMAL && piece.IsDefined())
        {
            //Start the Coroutine to play the Particle Animation
            StartCoroutine(ParticleAnimation(piece.grid.destroyParticles, piece));
        }
    }

    //THis Function is to prevent the ClearPiece() Function to be public
    //Since we just want to call it in other scripts and not change any variables inside.
    //The ClearPiece Function will stay Private while this function is public
    //Allowing other scripts to excess variables inside this function
    public void Clear()
    {
        isBeingCleared = true; //if it is being cleared, set the boolean to check if the piece is being cleared to be true
        ClearPiece(); //Call the ClearPiece() Function
    }

    //Coroutine for the particle animation in Unity
    //Conditions take in the ParticleSystem from the Grid Class and this GamePiece
    IEnumerator ParticleAnimation(ParticleSystem particle, GamePiece gamePiece)
    {
        ParticleSystem.MainModule main = particle.main; //Reference the Script Interface for the Particle System's Main Module
        main.startColor = gamePiece.dailyActivityPieceScript.particleColor; //Change the Start Color of the Particle
        //Debug.Log(main.startColor.color);
        ParticleSystem thisParticle = Instantiate(particle, new Vector3(gamePiece.transform.position.x, gamePiece.transform.position.y, gamePiece.transform.position.z - 2), Quaternion.identity); //Instantiate the Particle System and the GamePiece's position

        yield return new WaitForSeconds(1); //Wait for 1 second

        Destroy(thisParticle.gameObject); //Destroy the particles after that to prevent inactive particle spams in the scene
    }
}
