using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject drone;

    public List<GameObject> BSlist = new List<GameObject>();
    public List<double> Scanlist = new List<double>();
    public int nowBS = -1;
    object NoneValue = "None";

    float Timer = 0f;
    Vector3 positionlimit;
    float Distance;
    int Handovercount = 0;
    const int Threshold = -60;

    void Start()
    {

        for (int i = 1; i <= 20; i++)
        {
            string temp;
            temp = "Base" + i.ToString();
            this.BSlist.Add(GameObject.Find(temp));
        }

        positionlimit = new Vector3(500, 0, 0);
        drone.transform.position = new Vector3(0, 0, 0);

        Debug.Log("NowBS : "+nowBS);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(Distance);
        //Debug.Log(Base.transform.position.x);
        //rssi = GetSignal(Base1);
        //Debug.Log(rssi);

        if (drone.transform.position.x >= positionlimit.x)
        {
            Time.timeScale = 0;
        }
        
       
        drone.transform.position += Vector3.right*0.1f;
        drone.transform.position += Vector3.up*0.1f;
        //Debug.Log(Time.deltaTime);
        Timer += Time.deltaTime;

        GetSignal(this.BSlist);
        HandoverDecision();
        //Display();

        this.Scanlist.Clear();
    }

    void Display()
    {
        //for (int i = 0; i < 20; i++)
        //{
        //    ///////Show Every RSSI Value
        //    //if (this.Scanlist[i] == -1000.0)
        //    //{
        //    //    Debug.Log(i + "NULL");
        //    //    //Debug.Log(Scanlist[i]);
        //    //}
        //    //else
        //    //{
        //    //    Debug.Log(i + "RSSI:" + this.Scanlist[i]);
        //    //}

        //    //////Show Just RSSI Can Searched
        //    if (this.Scanlist[i] != -1000.0)
        //    {
        //        Debug.Log(i + "BaseStation's RSSI: " + this.Scanlist[i]);
        //    }
        //}
        Debug.Log("Now: "+ this.nowBS+1 + ", BaseStation's RSSI: " + this.Scanlist[this.nowBS]);
        Debug.Log("HO No :" + this.Handovercount);
    }

    void GetSignal(List<GameObject> station)
    {
        double rssi;
        for (int i = 0; i < 20; i++) { 
            Distance = Vector3.Distance(drone.transform.position, station[i].transform.position);
            if(Distance < 100){ rssi = -20 * Math.Log10(Distance * 10); 
              this.Scanlist.Add(rssi);
              //Debug.Log("comin");
            }
            else{
                this.Scanlist.Add(-1000.0);
            }

        }
    }

    void Handover(int BSnumber)
    {
        this.nowBS = BSnumber;
        Debug.Log(nowBS);
        this.Handovercount += 1;
    }

    void HandoverDecision()
    {
        double maxValue = Scanlist.Max();
        if (maxValue == -1000)
        {
            Debug.Log(this.NoneValue);
        }
        else
        {
            int TargetBS;
            TargetBS = Scanlist.ToList().IndexOf(maxValue);
            //Debug.Log(PriorityBS);
            if (TargetBS != this.nowBS && this.nowBS == -1) { this.Handover(TargetBS); }
            else if(TargetBS != this.nowBS && this.Scanlist[nowBS] <= Threshold) { this.Handover(TargetBS); }
            this.Display();
        }

    }

}
