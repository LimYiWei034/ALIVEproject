using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyActivityPiece : MonoBehaviour
{
    public enum DailyActivityType //Identification for the Type of Daily Activity
    {
        //The Use of Enums allows codes to be less prones to errors
        EatHealthy, 
        Sleeping,  
        Hygiene,  
        Hydration,   
        EatGreen
    }

    [System.Serializable]
    //Struct created to store the Sprite and Color to their Dictionary Key (the type of piece) from the Inspector (with the help of the function in Awake())
    public struct DailyActivityStruct
    {
        public DailyActivityType dailyActivityKey; //Stores the Daily Activity Type which would be use as the key for the dictionaries
        public Sprite sprite; //Stores the Sprite that represents the Daily Activity
        public Color particleColorVariable; //Store the Color of the Daily Activity
    }

    public DailyActivityStruct[] dailyActivityStructs; //Array to Store the different DailyActivitySprite struct (Displayed in the Inspector). Game Designers that wants to add more PieceTypes can be done in the Inpector.

    private Dictionary<DailyActivityType, Sprite> dailyActivitySpriteDict; //Use to store Sprite respective to thier key (which is the type of Daily Activity) stated in the Inspector
    private Dictionary<DailyActivityType, Color> particleColorDict; //Use to store color of the particle repective to thier key stated in the Inspector

    //Variable to store what type of DailyActivity this current Prefab has 
    public DailyActivityType dailyActivity; //Store the DailyActivityType Set
    public SpriteRenderer spriteRenderer;   //To store the Sprite Renderer in order to reference the sprite and change it according to the dictionary key from the dailyActivitySpriteDict
    public Color particleColor;   //To store the colour set according to the dictionary key from the particleColorDict. Would be use to color its particle instantiate when this piece is being cleared.

    private void Awake()
    {
        dailyActivitySpriteDict = new Dictionary<DailyActivityType, Sprite>(); //New Instance of the DailyActivitySprite Dictionary
        particleColorDict = new Dictionary<DailyActivityType, Color>(); //New Instance of the ParticleColor Dictionary
        spriteRenderer = GetComponent<SpriteRenderer>();    //Get and store the Sprite Renderer Component

        for (int i = 0; i < dailyActivityStructs.Length; i++)//Loop through the dailyActivitysprites Array
        {
            if (!dailyActivitySpriteDict.ContainsKey(dailyActivityStructs[i].dailyActivityKey)) //If the Sprite Dictionary doesn't contain a DailyActivityType in the Array (indicated manually in the Inspector)
            {
                dailyActivitySpriteDict.Add(dailyActivityStructs[i].dailyActivityKey, dailyActivityStructs[i].sprite); //Add the Dictionary key and its respective sprite indicated in the struct through the Inspector.
            }

            if (!particleColorDict.ContainsKey(dailyActivityStructs[i].dailyActivityKey)) //If the Color Dictionary doesn't contain a DailyActivityType in the Array (indicated manually in the Inspector)
            {
                particleColorDict.Add(dailyActivityStructs[i].dailyActivityKey, dailyActivityStructs[i].particleColorVariable); //Add the Dictionary key and its respective color indicated in the struct through the Inspector.
            }
        }
    }

    //This Function is used to set this Gameobject's Piece Type
    //It will then change the sprites and particle color according to the dictionaries.
    public void SetDailyActivity(DailyActivityType newDailyActivity)
    {
        dailyActivity = newDailyActivity; //Set the dailyActivity of this sprite to a newDailyActivity(set in the condition)

        if (dailyActivitySpriteDict.ContainsKey(newDailyActivity)) //If the dictionary contains the daily activity key
        {
            spriteRenderer.sprite = dailyActivitySpriteDict[newDailyActivity]; //Change the Sprite according to the respective sprite in the Sprite Dictionary 

            //I store the particle color here too since both dictionaries have the same key type.
            particleColor = particleColorDict[newDailyActivity]; //Store the Particle Color according to the respective color in the Particle Color Dictionary
        }
    }

}
