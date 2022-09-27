using UnityEngine;

public class Frisbee: MonoBehaviour
{

    private Vector2 position;
    private Vector2 direction;
    private Joueur joueur;
    
    public Transform frisbeeTransform;

    private void Start()
    {
        float random = Random.Range(0, 10);
        if (random > 5)
        {
            direction = new Vector2(1, 5);
        }
        else direction = new Vector2(-1, 5);

    }

    private void FixedUpdate()
    {
        frisbeeTransform.position += (Vector3)direction.normalized*Time.fixedDeltaTime*30;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TopWall"))
        {
            
            direction = new Vector2(direction.x, -direction.y);
        }

        if (collision.CompareTag("BottomWall"))
        {
            
            direction = new Vector2(direction.x, -direction.y);
        }

        
    }

    public Vector2 getPosition()
    {
        return this.position;
    }

    public Vector2 GetDirection()
    {
        return this.direction;
    }
    
    public void setPosition(Vector2 pos)
    {
        this.position = pos;
    }
    public void setDirection(Vector2 dir)
    {
        this.direction = dir;
    }

    

}
