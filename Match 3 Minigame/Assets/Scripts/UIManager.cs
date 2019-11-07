using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //Other Components
    public Antibiotics antibiotics; //Use to store the Antibiotics component for reference

    //Score Display
    public Text scoreDisplay; //Use to store the Text display of thescore to tell the players how mcuh store they have attained

    //Progress Bar (Feature for the Player to win the game)
    public Image progressBar; //use to Store and Refeerence the Progress Bar which represent the day span of the player

    //Effectiveness of antibiotics UI
    public Text effectivenessDisplay; //Use to store the display on how effective the antibiotics is


    private void Start()
    {
        progressBar.fillAmount = antibiotics.currentProgress / antibiotics.fullProgress; //Fill amount range is from 0 to 1. This code will input the percantage of the players progress presented as a float.
    }

    //This functions updates the progress bar of the game whenever there are points added(Points are gain by matching pieces on the grid)
    public void UpdateProgressBar(float pointsAdded)
    {
        antibiotics.currentProgress += pointsAdded; //Add the amount of points into the the current progress
        progressBar.fillAmount = antibiotics.currentProgress / antibiotics.fullProgress; //Update the progress bar
        antibiotics.CalculateDosage(); //Calculate the new milestone
        UpdateScore(); //Update the score of the game

        if (progressBar.fillAmount >= 1) //If the progress bar fill amount is = 1, means that the progress is at 100 percent, hence, the player has completed the game
        {
            Debug.Log("u win"); //No Ui at the moment, hence i used the Debug.log (UI was hendled by a UI designer hired by our client. Unfortunately, they stop working on the project with us)
            antibiotics.currentProgress = 0; //Reset the progress to 0 (Restart the game)
            //Restart Function
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Load this scene

            //There is no Game over at the moment as the project was meant to be a unity prototype
            //However, the game would be over if there are no matches on the grid
        }

    }

    //Update the score of the game into the display
    private void UpdateScore()
    {
        scoreDisplay.text = "Your Score: " + antibiotics.currentProgress.ToString();
    }

    //This function is used to update the Effectiveness Display of the antibiotics
    //Run this code whenever the effectiveness of the antibiotics have been changed
    public void UpdateEffectivenessDisplay()
    {
        effectivenessDisplay.text = "Effectiveness: " + antibiotics.antibioticsEffectiveness.ToString(); //To string changes the effectiveness into a string for display
    }
}
