using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrVersusText : MonoBehaviour
{
    public TextMeshProUGUI step;
    public TextMeshProUGUI status;
    public ScrVersusLogic logic;

    public void Update()
    {
        step.text = (logic.active) ? "Active" : "Paused";
        step.text += "; Step = " + logic.period + " seconds";

        if (logic.countdown == 100)
            status.text = "Player " + logic.player + ", place your cells. " + (logic.max_placed - ((logic.player == 1) ? logic.localScore.x : logic.localScore.y)) + " remaining.";
        else if (logic.countdown > 50)
            status.text = "Let Life take its course! " + logic.countdown + " cycles remaining.";
        else if (logic.countdown == 50)
        {
            status.text = "Player " + logic.player + ", you can place " + (logic.max_placed - ((logic.player == 1) ? (logic.localScore.x - logic.scoreSnapshot.x) : (logic.localScore.y - logic.scoreSnapshot.y))) + " more cells!";
        }
        else if (logic.countdown > 0)
            status.text = "Let Life take its course! " + logic.countdown + " cycles remaining.";
        else if (logic.countdown == 0)
        {
            if (logic.score.x == logic.score.y)
                status.text = "Finish! Amazingly, it is a tie with a score of " + logic.score.x + "! Life found its way!";
            else
                status.text = "Finish! Score: " + logic.score.x + " / " + logic.score.y + ". Player " + ((logic.score.x > logic.score.y) ? 1 : 2) + " wins. Such is the way of life.";
        }
    }

}
