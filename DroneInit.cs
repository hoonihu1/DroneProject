using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Linq;

public class TemplateAgent : Agent
{
    Rigidbody rBody;
    public double direction;
    public int allocatedBS;
    public int Handovercount = 0;
    public List<double> Scanlist = new List<double>();

    //public double thr_x = 20;
    //public double thr_y = 40;

    public List<GameObject> BSlist = new List<GameObject>();
    Vector3 positionlimit;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();

        for (int i = 1; i <= 20; i++)
        {
            string temp;
            temp = "Base" + i.ToString();
            this.BSlist.Add(GameObject.Find(temp));
        }

        positionlimit = new Vector3(500, 0, 0);
        this.transform.position = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {

        //Debug.Log(Distance);
        //Debug.Log(Base.transform.position.x);
        //rssi = GetSignal(Base1);
        //Debug.Log(rssi);

        if (this.transform.position.x >= positionlimit.x)
        {
            Debug.Log("HO No :" + this.Handovercount);
            Time.timeScale = 0;
        }


        this.transform.position += Vector3.right * 0.1f;
        this.transform.position += Vector3.up * 0.1f;
        //Debug.Log(Time.deltaTime);

        //Debug.Log(drone.transform.position.x);

        GetSignal(this.BSlist);
        HandoverDecision();
        //Display();

        this.Scanlist.Clear();


    }

    public Transform Target;
    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.x >=this.positionlimit.x && this.transform.localPosition.y >=this.positionlimit.y)
        {
            this.transform.localPosition = new Vector3(0, 0, 0);
        }

        this.transform.localPosition += Vector3.right * 0.1f;
        this.transform.localPosition += Vector3.up * 0.1f;
    }

    public void GetSignal(List<GameObject> station)
    {
        double rssi;
        for (int i = 0; i < 20; i++)
        {
            double Distance = Vector3.Distance(this.transform.position, station[i].transform.position);
            if (Distance < 100)
            {
                rssi = -20 * Math.Log10((Distance + 0.01) * 10) + 20;
                rssi = this.NomalizationRSSI(rssi);
                this.Scanlist.Add(rssi);
                //Debug.Log("comin");
            }
            else
            {
                this.Scanlist.Add(-1000.0);
            }

        }
    }

    double NomalizationRSSI(double rssi)
    {
        double max = 15.0;
        double min = -40.0;
        double upperbound = 1.0;
        double rowbound = 0.0;

        return ((rssi - min) / (max - min) * (upperbound - rowbound)) - (upperbound - rowbound);

    }

    //public override void CollectObservations(VectorSensor sensor)
    //{
    //    sensor.AddObservation(Target.localPosition);
    //    sensor.AddObservation(this.transform.localPosition);
    //}



}
