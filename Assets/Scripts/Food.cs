using System.Collections;
using UnityEngine;

public class Food : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "food" ||
            col.gameObject.tag == "spawn")
        {
            GeneticAlgorithm.Instance.ChangeFoodPosition(gameObject);
        }        
    }

    public IEnumerator Respawn()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(5.0f);

        GeneticAlgorithm.Instance.ChangeFoodPosition(gameObject);
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }
}
