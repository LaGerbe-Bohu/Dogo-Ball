using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager 
{
    public bool MoveUp()
    {
        
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            return true;
        } 
        
        return false;
    }

    public bool MoveDown()
    {
             
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            return true;
        }

        return false;
    }
    
    public bool MoveRight()
    {
             
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            return true;
        }

        return false;
    }
    
    public bool MoveLeft()
    {
             
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            return true;
        }
        
        return false;
    }
    
    
}
