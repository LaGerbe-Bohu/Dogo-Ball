using UnityEngine;

public class Frisbee: MonoBehaviour
{

    private Vector2 position;
    private Vector2 direction;
    private Joueur joueur;

    public Rigidbody2D frisbeeBody;
    public CircleCollider2D frisbeeCollider;
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
        frisbeeBody.transform.position += (Vector3)direction.normalized*Time.fixedDeltaTime*30;
        
        
    }

    public Vector2 getPosition()
    {
        return this.transform.position;
    }

    public Vector2 GetDirection()
    {
        return this.direction;
    }
    
    public void setPosition(Vector2 pos)
    {
        this.transform.position = pos;
    }
    public void setDirection(Vector2 dir)
    {
        this.direction = dir;
    }

    

}
