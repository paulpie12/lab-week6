using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    bool timerIsOn = true;
    float timer = 0;
    
    // Timer that resets infinitely while turned on
    public int LoopingTimer(float maxTimer)
    {
        if(timerIsOn)
        {
            if(timer >= maxTimer)
            {
                timer = 0;
                return 0;
            }
            else
            {
                timer += Time.deltaTime;
                return 1;
            }   
        }
        else
        {
            return 2;
        }
    }

    // Timer that turns off upon completion.
    // Must be turned on again manually with TurnTimerOn()
    public int DiscreteTimer(float maxTimer)
    {
        if(timerIsOn)
        {
            if(timer >= maxTimer)
            {
                timer = 0;
                timerIsOn = false;
                return 0;
            }
            else
            {
                timer += Time.deltaTime;
                return 1;
            }   
        }
        else
        {
            return 2;
        }
    }

    // Turns timer on/off
    public void TurnTimerOn(bool timerCondition)
    {
        timerIsOn = timerCondition;
    }

    // resets timer to 0
    public void ResetTimer()
    {
        timer = 0;
    }
}
