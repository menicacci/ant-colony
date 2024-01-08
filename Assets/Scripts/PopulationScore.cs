using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulationScore : MonoBehaviour
{

    public static PopulationScore instance;

    // 6 elements: populationA, FoodCountA, PopulationB, FoodCountB, PopulationC, FoodCountC
    public Text[] texts;

    public int[] populations = { 0, 0, 0 };
    public int[] foodFound = { 0, 0, 0 };


    public void Awake()
    {
        instance= this;
    }
    // Start is called before the first frame update
    void Start()
    {
        this.texts[0].text = "Population A: 0";
        this.texts[1].text = "Food found: 0";
        this.texts[2].text = "Population B: 0";
        this.texts[3].text = "Food found: 0";
        this.texts[4].text = "Population C: 0";
        this.texts[5].text = "Food found: 0";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePopulation(int population, int index)
    {
        if(index == 0)
        {

            texts[0].text = "Population A: " + population.ToString();
            
        }
        else
        {
            if (index == 1)
            {
                texts[2].text = "Population B: " + population.ToString();

            }
            else
            {
                texts[4].text = "Population C: " + population.ToString();
            }

        }
    }

    public void ChangeFoodFound(int food, int index)
    {
        if (index == 0)
        {

            texts[1].text = "Food found: " + food.ToString();

        }
        else
        {
            if (index == 1)
            {
                texts[3].text = "Food found: " + food.ToString();

            }
            else
            {
                texts[5].text = "Food found: " + food.ToString();
            }

        }
    }
}
