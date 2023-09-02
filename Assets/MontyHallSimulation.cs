using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MontyHallSimulation : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField] int defaultNumRounds = 1000;
    [SerializeField] int defaultNumDoors = 3;

    [Header("UI ELEMENTS")]
    [SerializeField] TMP_InputField numRoundsInputField;
    [SerializeField] TMP_InputField numDoorsInputField;
    [SerializeField] Toggle contestantAlwaysSwitchesToggle;
    [SerializeField] TMP_Text attemptsLabel;
    [SerializeField] TMP_Text winPercentageLabel;
    [SerializeField] Button simulateButton;

    private float winPercentage;

    private void Start()
    {
        int seed = System.DateTime.Now.Millisecond;
        UnityEngine.Random.InitState(seed);

        numRoundsInputField.text = defaultNumRounds.ToString();
        numDoorsInputField.text = defaultNumDoors.ToString();
        attemptsLabel.text = "Attempt #: 0";
    }

    public void StartSimulate()
    {
        int rounds = int.Parse(numRoundsInputField.text);
        int roundsWon = 0;

        for (int i = 0; i < rounds; i++)
        {
            attemptsLabel.text = string.Format("Attempt #:{0}", (i+1).ToString());
            if (Simulate())
            {
                roundsWon++;
            }
        }

        winPercentage = ((float)roundsWon / (float)rounds) * 100f;
        winPercentageLabel.text = string.Format("{0} of {1} rounds won ({2}%)", roundsWon, rounds, winPercentage.ToString());
    }

    // Returns true if contestant wins.. Simulates One Round
    public bool Simulate()
    {
        int numDoorsInSim = int.Parse(numDoorsInputField.text);
        bool contestantAlwaysSwitches = contestantAlwaysSwitchesToggle.isOn;

        if (numDoorsInSim <= 0) return false;
        else if (numDoorsInSim == 1) return true;

        // Doors Setup
        List<Door> doors = new List<Door>();
        for (int i = 0; i < numDoorsInSim; i++)
        {
            Door door = new Door();
            doors.Add(door);
        }
        int doorWithPrize = Random.Range(0, numDoorsInSim);

        // Debug.Log("doorWithPrize: " + doorWithPrize);
        doors[doorWithPrize].hasPrize = true;

        // Contestant Picks a Random Door
        int contestantDoorChoice = Random.Range(0, numDoorsInSim);
        // Debug.Log("contestantDoorChoice: " + contestantDoorChoice);

        // Host crosses out doors that doesnt have the prize and is not selected by the contestant leaving only one door
        // if doorWithPrize was picked by contestant then the 
        // host closes all but one of the 'noPrize' doors and leave the one with the prize in it
        if (doorWithPrize == contestantDoorChoice)
        {
            int hostSaveDoorChoice = 0;

            while (hostSaveDoorChoice == doorWithPrize || hostSaveDoorChoice == contestantDoorChoice)
            {
                hostSaveDoorChoice = Random.Range(0, numDoorsInSim);
                // Debug.Log("hostSaveDoorChoice: " + hostSaveDoorChoice);
            }

            for (int i = 0; i < numDoorsInSim; i++)
            {
                if (i == doorWithPrize || i == hostSaveDoorChoice)
                {
                    continue;
                }
                else
                {
                    // Debug.Log("CrossedOut: Door " + i);
                    doors[i].crossedOut = true;
                }
            }
        } 
        else // if the doorWithPrize isn't selected by the contestant then you're free to close every 'noPrizeDoor' 
        {
            for (int i = 0; i < numDoorsInSim; i++)
            {
                if (i == doorWithPrize || i == contestantDoorChoice)
                {
                    continue;
                }
                else
                {
                    doors[i].crossedOut = true;
                    // Debug.Log("CrossedOut: Door " + i);
                }
            }
        }


        // Check if the contestant wins
        if (contestantDoorChoice == doorWithPrize)
        {
            if (contestantAlwaysSwitches)
            {
                return false; // contestant loses because he switches
            }
            return true; 

            // return (!contestantAlwaysSwitches);
        }
        else
        {
            
            if (contestantAlwaysSwitches)
            {
                return true; // contestant wins because he switches
            }
            return false;

            // return (contestantAlwaysSwitches);
        }
       
    }
}

public class Door
{
    public bool hasPrize = false;
    public bool crossedOut = false;
}
