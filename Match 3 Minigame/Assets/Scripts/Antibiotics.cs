using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Antibiotics : MonoBehaviour
{
    //Antibiotics Dosage Variables
    public float medicationDosage; //Use to store the Dosage of the Medication (For Example, 3 for 3 times a day) 
    float medicationMilestones; //Use to store the score intervals of taking antibiotics at correctly prescripted timings. (How much score the player has to attain inorder to take antibiotics effectively again)
    public float currentMedMilestone; //Stores the next current milestone of when the player can eat his antibiotics effectively again (The next score to get in the progress bar in order to eat antibiotics effectively)
    bool medicationIsReady; //When this boolean is true, it means that it is time for the player to take his antibiotics. Allowing the player to take antibiotiocs efficiently

    //Antibiotics Effectiveness
    public int antibioticsEffectiveness; //Use to store how effective the antibiotics is
    public int effectivenessReduction; //use to store how much effectiveness should be reduced if the player takes adventage of the antibiotics power
    public GameObject reminderDisplay; //Use to store the reminder text (give information to the player telling him that is time to take his medication)

    public float fullProgress; //Use to store the value of the Progress Bar at full progress (This is used to calculate the percentage of the player's progress, more will be explained in the functions)
    public float currentProgress; //Use to store the value of the Player's Current Progress
    float pointsPerPieceDestroyed; //Use to store the No of Points per Pieces destroyed when matched (More information about his variable will be explained later)

    public UIManager uiManager; //Use to store the UI Manager component for reference


    // Start is called before the first frame update
    private void Start()
    {
        //Progress Bar interface
        fullProgress = 300; //Giving the full progress a maximum score of 300 the player has to attain
        currentProgress = 0; //The player has no progress at the start of the game
        //progressBar.fillAmount = currentProgress / fullProgress; //Fill amount range is from 0 to 1. This code will input the percantage of the players progress presented as a float.
        pointsPerPieceDestroyed = 1; //Inicate one point for each Type piece is destroyed in a match. This means when the player matches 3 pieces, he will gain 3 points since 3 pieces were matched and destroyed.

        //Medication Dosage Milestones
        currentMedMilestone = 0; //Current milestone is set to 0. It will have a value after the medication milestone is calculated
        medicationMilestones = fullProgress / (medicationDosage + 1); //This calculates the medication Milestones where the full progress is dvided by the number of times the player has to take the medication (Dosage)
                                                                      //Since the full progress is 300, let's say the medication dosage is 3. After calculation, the MedicationMilestone is 75
        currentMedMilestone += medicationMilestones;                  //in here we add the medication milestone the current medication milestone to meet
                                                                      //The reason why I added 1 when dividing:
                                                                      //If i did not add 1, the player will have a milestone of 100, 200 then 300
                                                                      //means the player will only take the medication 2 times throughout the game when the dosage says 3 times a day (at 300 score, the player completes the game)
                                                                      //Hence adding 1 on allow the player to take the dosage 3 times in the game
                                                                      //This means the medication milestone = 75 which mean the player will have milestones for 75, 150, 225 and 300 (300 is not counted since the game has been completed)

        uiManager.UpdateEffectivenessDisplay();                     //Call this function to show the current effectiveness of the antibiotics
    }

    //This function is use to calculate the next current milestone which allows the player to use antibiotics effectively
    //This function would be called when the Progress bar is being updated as when the progress bar is updated, the current progress get updated
    public void CalculateDosage()
    {
        if (currentProgress >= currentMedMilestone) //if the current progress is higher than the current milestone to meet
        {
            reminderDisplay.SetActive(true); //Set the Reminder to active to tell the player to take his antibiotics
            medicationIsReady = true;   //Mark Medication is ready (this means that the player is able to take the antibiotics without the effectiveness dropping as it is time for thier medication and its part of the dosage)         

            currentMedMilestone += medicationMilestones; //give a new milestone to the current medication milestone
                                                         //For example, if the milestone intervals is 75 
                                                         //At the start,the current milestone = 0 + 75 = 75
                                                         //Now the next milestone is 75 + 75 which is 150. This means the player has to meet a score of 150 to take his next antibiotics dosage

            //Thinking whether to put a progress limit
            //For Example, the player didn't take the medication and continued with the match 3

        }
    }

    //This function is use to check whether the antibiotics has been misused 
    //This antibiotics is misused when the player doesn't follow the daily dosage and the reminder about antibiotics is no active
    //This Function runs when the player click on the Antibiotics Power
    public void CheckMisused()
    {
        if (medicationIsReady != true) //If the Medication is not ready (which means the milestone has not been met)
        {
            antibioticsEffectiveness -= effectivenessReduction; //Deduct the effectiveness of the antibiotics

            //This if statement is to prevent the integer from going below a negative value
            if (antibioticsEffectiveness <= 0) //If the antibioitics effectiveness after deduction is -1 or below
            {
                antibioticsEffectiveness = 0; //Set the effectiveness to 0
            }

            uiManager.UpdateEffectivenessDisplay(); //Update the Effectiveness display to the new effectiveness of the antibiotics
        }
        else //This code runs if the medication is ready. Which means that the player has reached an antibiotics milestone
        {
            medicationIsReady = false; //Set the bool to false so that the player can abuse the antibioitics power

            //Since the player ahs used the power, he doesn't has to be reminded again 
            if (reminderDisplay.activeSelf == true) //If the reminder text is shown to the player on the screen
            {
                reminderDisplay.SetActive(false); //Disable it
            }
        }
    }
}
