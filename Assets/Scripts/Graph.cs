using System;
using System.Collections;
using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;

public class Graph : MonoBehaviour
{

    public GraphChart chart;

    private float Timer = 1f;
    private float X = 4f;
    private System.Random random = new System.Random();
    public AntSpawner antSpawner;

    public float interval = 1f;


    // Start is called before the first frame update
    void Start()
    {
        // bisogna includere i cambiamenti del grafo nelle chiamate di StartBatch e EndBatch
        chart.DataSource.StartBatch();
        //chart.DataSource.ClearCategory("Tutorial 1");

        chart.DataSource.EndBatch();

    }

    // Update is called once per frame
    void Update()
    {
        
        // now let's add a streaming data update the goes every 1 second.
        Timer -= Time.deltaTime; // each update we deacrese the time that has passed
        if (Timer <= 0f)
        {
            Timer = interval; // set the time to one again and :
            chart.DataSource.AddPointToCategoryRealtime("Tutorial 1", X, this.antSpawner.GetNumberOfAnts(), interval/2); // now we can also set the animation time for the streaming value
            X++; // increase the X value so the next point 
        }
    }

}
