using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    public InputField speed_field;
    public Text time_elapsed, generation_num, creatures_alive, average_fitness;
    private TimeSpan span;
    public static Interface Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log(string.Format("Had to destroy \"{0}\", because it contained the Interface script!", this.gameObject.name));
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        span = new TimeSpan(0, 0, 0);
        StartCoroutine(CountTime());
    }

    public void FixedUpdate()
    {
        time_elapsed.text = span.ToString();
        generation_num.text = GeneticAlgorithm.Instance.generation_number.ToString();
        creatures_alive.text = GeneticAlgorithm.Instance.alive_creatures.Count.ToString();
        average_fitness.text = GeneticAlgorithm.Instance.average_fitness.ToString();
    }

    public void SetSpeed()
    {
        int result = -1;
        int.TryParse(speed_field.text, out result);
        if (result != -1)
            Time.timeScale = result;
    }

    private IEnumerator CountTime()
    {
        while (true)
        {
            TimeSpan new_span = TimeSpan.FromSeconds(1);
            span = span.Add(new_span);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
