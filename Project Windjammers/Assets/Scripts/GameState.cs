using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour
{


    public static GameState Instance;
    
    private int timer;
    private int setCurrent;
    
    public Joueur j1;
    public Joueur j2;
    
   public Frisbee frisbee;

    private GameManager gameManager;
    
    
    private void Awake()
    {
        Instance = this;
        
    }

    private void FixedUpdate()
    {
        
        if (isCatch(j1, frisbee))
        {
            frisbee.setDirection(new Vector2(0, 0));
        }
        if (isCatch(j2, frisbee))
        {
            frisbee.setDirection(new Vector2(0, 0));
        }
        collisionWall(frisbee);
    }

    private bool isCatch(Joueur j, Frisbee f)
    {
        float distance = Mathf.Sqrt(Mathf.Pow((j.getPosition().x - f.getPosition().x),2)+Mathf.Pow((j.getPosition().y - f.getPosition().y),2));
        if(distance < 2)
        {
            return true;
        }
        return false;
    }

    private void collisionWall(Frisbee f)
    {
        if (f.getPosition().y > 8.9 || f.getPosition().y <-8.1)
        {
            Vector2 newdir = new Vector2(f.GetDirection().x, -f.GetDirection().y);
            f.setDirection(newdir);
        }
    }
}

