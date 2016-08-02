using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour {

    private int amount_food = 400;
    private int population_size = 200;
    private GameObject world, spawn;
    public int generation_number = 0;
    public int average_fitness = 0;
    public List<Creature> alive_creatures;
    public static GeneticAlgorithm Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log(string.Format("Had to destroy \"{0}\", because it contained the GeneticAlgorithm script!", this.gameObject.name));
            Destroy(gameObject);
        }
    }

    // Initialize world
	void Start () {
        GenerateWorld(amount_food);
        StartCoroutine(StartGeneticAlgorithm());
	}

    // Generate the world
    private void GenerateWorld(int amount_food)
    {
        world = GameObject.FindGameObjectWithTag("world");        

        // Clear world
        foreach (GameObject food in GameObject.FindGameObjectsWithTag("food"))
            Destroy(food);

        // Place down food
        for (int i = 0; i < amount_food; i++)
        {
            // Create gameobject and set parent to world
            GameObject food_object = Instantiate(Resources.Load<GameObject>("Prefabs/Food"));
            food_object.name = "Food";
            food_object.transform.SetParent(world.transform, false);
            ChangeFoodPosition(food_object);            
        }
        
        spawn = GameObject.FindGameObjectWithTag("spawn");
    }

    // Start the main loop for the algorithm
    private IEnumerator StartGeneticAlgorithm()
    {
        alive_creatures = new List<Creature>();

        // Initialize population
        for (int i = 0; i < population_size; i++)
        {
            object[] dna = new object[5] { Random.Range(3, 5), Random.Range(4f, 6f), 0, Random.Range(1, 3), Random.Range(0.1f, 2.0f) };
            CreateCreature(dna, "Creature " + i);
        }

        // Activate the creatures
        generation_number = 1;
        Interface.Instance.Start();
        foreach (Creature creature in alive_creatures)
            creature.isAlive = true;

        // Main loop
        while (true)
        {
            yield return new WaitForSeconds(120.0f);

            // Freeze the survivors
            int total_health = 0;
            foreach (Creature creature in alive_creatures)
            {
                creature.isAlive = false;
                creature.GetComponent<Rigidbody2D>().isKinematic = true;
                creature.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                total_health += creature.health;
            }
            average_fitness = total_health / alive_creatures.Count;

            /* CALCULATE FITNESS */
            // fitness = health (higher health = better fitness)
            List<Creature> ordered_pool = alive_creatures.OrderByDescending(x => x.health).ToList();

            /* SELECTION */
            // Get the fittest (if possible)
            List<Creature> mating_pool = new List<Creature>();
            try
            {
                for (int i = 0; i < ordered_pool.Count/2; i++) // get half of the list
                    mating_pool.Add(ordered_pool[i]);
            }
            catch (System.IndexOutOfRangeException)
            { }

            foreach (Creature creature in alive_creatures)
                Destroy(creature.gameObject);
            alive_creatures.Clear(); // Clear the list for the next generation

            /* REPRODUCTION */
            List<int> indexes_mated = new List<int>();
            int babies_created = 0; //TEMP
            for (int i = 0; i < mating_pool.ToArray().Length; i+=2)
            {
                indexes_mated.Add(i); // Add current index to list

                // Find a new "free" index for a mate
                int random_index = Random.Range(0, mating_pool.ToArray().Length);
                if (indexes_mated.ToArray().Length == mating_pool.ToArray().Length)
                {
                    // No more mates left..
                    break;
                }
                else if (indexes_mated.Contains(random_index))
                {
                    while (indexes_mated.Contains(random_index))
                        random_index = Random.Range(0, mating_pool.ToArray().Length);
                }

                Creature partner_1 = mating_pool[i];
                Creature partner_2 = mating_pool[random_index];

                // Decide how many babies will be created
                int parent_number = Random.Range(1, 2);
                int num_babies = 2;
                switch (parent_number)
                {
                    case 1:
                        num_babies = partner_1.baby_amount;
                        break;
                    case 2:
                        num_babies = partner_2.baby_amount;
                        break;
                }

                for (int baby = 1; baby <= num_babies; baby++)
                {
                    // Crossover
                    int crossover_point = Random.Range(1, partner_1.dna.Length - 1);
                    object[] new_dna = new object[partner_1.dna.Length];
                    for (int a = 0; a < crossover_point; a++)
                    {
                        new_dna[a] = partner_1.dna[a];
                    }
                    for (int b = crossover_point; b < new_dna.Length; b++)
                    {
                        new_dna[b] = partner_2.dna[b];
                    }

                    // Mutation
                    float rand = Random.Range(0f, 1f);
                    if (rand <= partner_1.mutation_rate ||
                        rand <= partner_2.mutation_rate)
                    {
                        // Pick one dna element to mutate
                        //int index = Random.Range(0, new_dna.Length - 1);
                        //object element = new_dna[index];
                        //float multiplier = Random.Range(0.1f, 0.7f);
                        //if (element is int)
                        //{
                        //    int element_int = int.Parse(element.ToString());
                        //    element_int += Mathf.RoundToInt(element_int * multiplier);
                        //    new_dna[index] = element_int;
                        //}
                        //else if (element is float)
                        //{
                        //    float element_float = float.Parse(element.ToString());
                        //    element_float += Mathf.Round(element_float * multiplier);
                        //    new_dna[index] = element_float;
                        //}

                        // Give all dna elements a chance to mutate
                        int max_capacity = int.Parse(new_dna[0].ToString());
                        float digest_time = float.Parse(new_dna[1].ToString());
                        int eyesight = int.Parse(new_dna[2].ToString());
                        int baby_amount = int.Parse(new_dna[3].ToString());
                        float speed = float.Parse(new_dna[4].ToString());

                        // 33% chance of negative mutation
                        if (Random.Range(0f, 1.0f) < 0.3333f) max_capacity--;
                        if (Random.Range(0f, 1.0f) < 0.3333f) digest_time++;
                        if (Random.Range(0f, 1.0f) < 0.3333f) eyesight--;
                        if (Random.Range(0f, 1.0f) < 0.3333f) baby_amount--;
                        if (Random.Range(0f, 1.0f) < 0.3333f) speed--;

                        // 66% chance of positive mutation
                        if (Random.Range(0f, 1.0f) < 0.6666f) max_capacity++;
                        if (Random.Range(0f, 1.0f) < 0.6666f) digest_time--;
                        if (Random.Range(0f, 1.0f) < 0.6666f) eyesight++;
                        if (Random.Range(0f, 1.0f) < 0.6666f) baby_amount++;
                        if (Random.Range(0f, 1.0f) < 0.6666f) speed++;

                        // Handle minimum values
                        if (max_capacity < 3)
                            max_capacity = 3;
                        if (digest_time > 4)
                            digest_time = 4;
                        if (digest_time < 0.1f)
                            digest_time = 0.1f;
                        if (eyesight < 0)
                            eyesight = 0;
                        if (baby_amount < 1)
                            baby_amount = 1;
                        if (speed < 0.1f)
                            speed = 0.1f;

                        // Update DNA
                        new_dna = new object[5] { max_capacity, digest_time, eyesight, baby_amount, speed };
                    }

                    // Creation
                    CreateCreature(new_dna, "Creature " + babies_created);
                    babies_created++;
                }
            }

            Debug.Log("Babies created: " + babies_created);

            // Re-Generate world
            GenerateWorld(amount_food);

            // Activate the creatures
            generation_number++;
            foreach (Creature creature in alive_creatures)
                creature.isAlive = true;
        }
    }

    // Creates/Instantiates a Creature
    private void CreateCreature(object[] dna, string name)
    {
        // Create creature gameobject
        GameObject creature_object = Instantiate(Resources.Load<GameObject>("Prefabs/Creature"));
        creature_object.name = name;
        creature_object.transform.SetParent(world.transform, false);
        creature_object.GetComponent<Creature>().SetProperties(dna);
        creature_object.transform.position = spawn.transform.position;
        alive_creatures.Add(creature_object.GetComponent<Creature>()); // Add this creature to the list
    }

    // Finds a new random position between ranges
    // and sets the gameObjects position to that
    public void FindPosition(GameObject gameObject, Vector2 min_range, Vector2 max_range)
    {
        // Define world borders where gameObject can spawn inbetween
        float min_x = min_range.x;
        float max_x = max_range.x;
        float min_y = min_range.y;
        float max_y = max_range.y;
            
        // Find a random possible position for food_object
        Vector3 possible_position = new Vector3(Random.Range(min_x, max_x), Random.Range(min_y, max_y), gameObject.transform.parent.position.z);
        gameObject.transform.position = possible_position;
    }

    // Change position of food object
    public void ChangeFoodPosition(GameObject food_object)
    {
        float world_width = world.GetComponent<SpriteRenderer>().sprite.rect.size.x;
        float world_height = world.GetComponent<SpriteRenderer>().sprite.rect.size.y;

        float min_x = (world_width / 2 * -1) * world.transform.localScale.x; // + ((food_object.GetComponent<SpriteRenderer>().sprite.rect.size.x * food_object.transform.lossyScale.x) / 2);
        float max_x = world_width / 2 * world.transform.localScale.x;// - ((food_object.GetComponent<SpriteRenderer>().sprite.rect.size.x * food_object.transform.lossyScale.x) / 2);
        float min_y = (world_height / 2 * -1) * world.transform.localScale.y;// + ((food_object.GetComponent<SpriteRenderer>().sprite.rect.size.y * food_object.transform.lossyScale.y) / 2);
        float max_y = world_height / 2 * world.transform.localScale.y;// - ((food_object.GetComponent<SpriteRenderer>().sprite.rect.size.y * food_object.transform.lossyScale.y) / 2);
        Vector2 min_range = new Vector2(min_x, min_y);
        Vector2 max_range = new Vector2(max_x, max_y);

        FindPosition(food_object, min_range, max_range);
    }
}
