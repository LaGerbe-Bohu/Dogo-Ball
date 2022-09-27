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
        frisbeeBody.velocity += direction.normalized*Time.fixedDeltaTime*30;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TopWall"))
        {
            Vector2 newdir = new Vector2(frisbeeBody.velocity.x, -frisbeeBody.velocity.y);
            newdir = Vector2.ClampMagnitude(newdir, 15);
            frisbeeBody.velocity = newdir;
            direction = new Vector2(direction.x, -direction.y);
        }

        if (collision.CompareTag("BottomWall"))
        {
            Vector2 newdir = new Vector2(frisbeeBody.velocity.x, -frisbeeBody.velocity.y);
            newdir = Vector2.ClampMagnitude(newdir, 15);
            frisbeeBody.velocity = newdir;
            direction = new Vector2(direction.x, -direction.y);
        }

        if (collision.CompareTag("Player"))
        {
            frisbeeBody.velocity = new Vector2(0, 0);
            direction = new Vector2(0, 0);
        }
    }


}
