using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//normally this data should come from database, but because now it isn't, easier to handle everything together.
public class Car
{
    public enum cartype
    {yellow,red,black};
    public float maxSteerAngle { get; set; }
    public float Acceleration { get; set; } 
    public float VehicleMaxSpeed { get; set; }
    public float PeopleRangeCheck { get; set; }
    public float CarsRangeCheck { get; set; }
    public Car(cartype type)
    {
        maxSteerAngle = 60f;
        switch (type)
        {
            case cartype.red:

                Acceleration = 500f;
                VehicleMaxSpeed = 60f;
                break;
            case cartype.yellow:
                Acceleration = 700f;
                VehicleMaxSpeed = 80f;
                break;
            case cartype.black:
                Acceleration = 900f;
                VehicleMaxSpeed = 90f;
                break;
            default:
                break;
        }

        PeopleRangeCheck = 7f;
        CarsRangeCheck = 10f;
    }

}
