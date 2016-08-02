using System.Collections;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public int health;
    public int max_capacity, current_capacity = 0;
    public float digest_time;
    public int eyesight;
    public int baby_amount;
    public float speed;
    public float mutation_rate;
    public Vector2 direction;
    public bool isAlive = false;
    [HideInInspector] public object[] dna;
    private bool isTicking = false;

    // Apply dna to creature
    public void SetProperties(object[] dna)
    {
        this.health = 100;
        this.max_capacity = (int)dna[0];
        this.digest_time = (float)dna[1];
        this.eyesight = (int)dna[2];
        this.baby_amount = (int)dna[3];
        this.speed = (float)dna[4];

        Vector2 direction = new Vector2(Random.Range(-1f, 1.01f), Random.Range(-1f, 1.01f));
        // Make sure direction is not (0,0)
        while (direction.x == 0 && direction.y == 0)
            direction = new Vector2(Random.Range(-1f, 1.01f), Random.Range(-1f, 1.01f));
        this.direction = direction;

        this.dna = dna;
        this.mutation_rate = Random.Range(0.005f, 0.01f);
    }

    // Main loop
    public void Update()
    {
        if (isAlive)
        {
            // Start coroutines
            if (!isTicking)
            {
                StartCoroutine(LoseHealth());
                StartCoroutine(DigestFood());
                isTicking = true;
            }

            // Die
            if (health <= 0)
            {
                GeneticAlgorithm.Instance.alive_creatures.Remove(this);
                Destroy(gameObject);
            }
        }
    }

    // Main physics loop
    public void FixedUpdate()
    {
        if (isAlive)
        {
            // Move around
            GetComponent<Rigidbody2D>().velocity = direction;
            GetComponent<Rigidbody2D>().velocity = speed * (GetComponent<Rigidbody2D>().velocity.normalized);
        }
    }

    // Handle collision with triggers
    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "food")
        {
            if (current_capacity < max_capacity)
            {
                // Move to random direction
                Vector2 temp_direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                while (temp_direction.x == 0 && temp_direction.y == 0)
                    temp_direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                direction = temp_direction;

                // Add food to capacity
                current_capacity++;

                // Make food respawn
                StartCoroutine(coll.gameObject.GetComponent<Food>().Respawn());
            }            
        }
        if (coll.gameObject.tag == "border")
        {
            // Move away from the border
            float x_min=-1, x_max=1, y_min=-1, y_max=1;
            if (coll.gameObject.name.Contains("up"))
            {
                y_min = 0;
                y_max = -1;
            }
            else if (coll.gameObject.name.Contains("down"))
            {
                y_min = 0;
                y_max = 1;
            }
            if (coll.gameObject.name.Contains("left"))
            {
                x_min = 0;
                x_max = 1;
            }
            else if (coll.gameObject.name.Contains("right"))
            {
                x_min = 0;
                x_max = -1;
            }

            Vector2 temp_direction = new Vector2(Random.Range(x_min, x_max), Random.Range(y_min, y_max));
            while (temp_direction.x == 0 && temp_direction.y == 0)
                temp_direction = new Vector2(Random.Range(x_min, x_max), Random.Range(y_min, y_max));
            direction = temp_direction;
        }
    }

    // Creature loses health over time
    private IEnumerator LoseHealth()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(1.0f / this.speed);
            health--;
        }
    }

    // Converts food into health
    private IEnumerator DigestFood()
    {
        while (isAlive)
        {
            yield return new WaitUntil(() => current_capacity > 0);
            yield return new WaitForSeconds(digest_time);
            current_capacity--;
            health++;

        }
    }
}
