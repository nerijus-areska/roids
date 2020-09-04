using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Init : MonoBehaviour
{

    // keep equal to 100
    int[] roidDistributionWeights = {20, 15, 10, 25, 30};
    SpawnAnchor[] anchors = {
        new SpawnAnchor(-80,30,6,30),
        new SpawnAnchor(-10,50,3,30),
        new SpawnAnchor(40,50,7,30),
        new SpawnAnchor(70,70,10,20),
        new SpawnAnchor(-40,-10,4,30),
        new SpawnAnchor(20,10,1,30),
        new SpawnAnchor(70,20,4,30),
        new SpawnAnchor(-50,-60,5,30),
        new SpawnAnchor(-10,-40,12,10),
        new SpawnAnchor(20,-30,2,20),
        new SpawnAnchor(60,-20,2,20),
        new SpawnAnchor(0,-70,8,20),
        new SpawnAnchor(50,-70,7,40),
    };

    public GameObject spaceship;
    public Spaceship spaceshipScript;
    public GameObject roid;
    public GameObject boost;
    public GameObject currentBoost;
    float nextBoostAt;

    public static List<GameObject> roidList = new List<GameObject>();

    

    // Start is called before the first frame update
    void Start()
    {
        spaceshipScript = spaceship.GetComponent<Spaceship>();
        foreach( SpawnAnchor anchor in anchors ) {
            //randomize polar coordinates - get magnitude to fall within one of roidDistributionWeights.Length zones.
            // direction should be completely random
            // assume sum of weights is always 100 (easier to think about it that way too)
            // we should maybe also have sparcity property ( using radius * coef now )
            for(int k = 0; k < anchor.RADIUS * 3; k++) {
                int tmpRand = UnityEngine.Random.Range(1, 100);
                int zone = 0;
                foreach(int weight in roidDistributionWeights) {
                    tmpRand -= weight;
                    if(tmpRand <= 0) {
                        break;
                    }
                    zone++;
                }
                
                float oreAmount = (float)Math.Pow(UnityEngine.Random.Range(5, 10), anchor.STRENGTH);
                int angle = UnityEngine.Random.Range(0, 360);
                int distance = UnityEngine.Random.Range(1, anchor.RADIUS / roidDistributionWeights.Length) * (zone+1);
                Tuple<float, float> coords = PolarToCartesian(angle, distance);
                float x = coords.Item1 + anchor.X;
                float y = coords.Item2 + anchor.Y;
                int shipRadius = 3;
                // Ship always stars at 0,0, exclude radius around it
                //TODO: this is temp'ish solution, we will have anti-duplication code later on, this can be a part of that code
                if(x > shipRadius || x < -shipRadius || y > shipRadius || y < -shipRadius) {
                    GameObject tmp = Instantiate(roid);
                    tmp.transform.position = new Vector3(x, y, 0.0f);
                    var roidScript = tmp.GetComponent<Roid>();
                    roidScript.initialOreAmount = oreAmount;
                    roidScript.oreAmount = oreAmount;
                    roidScript.refreshOreAmount();
                    roidList.Add(tmp);
                }
            }
        }
        // Big Bad Roid
        GameObject tmp2 = Instantiate(roid);
        tmp2.transform.localScale *= 4;
        tmp2.transform.position = new Vector3(-10, -30, 0.0f);
        var roidScript2 = tmp2.GetComponent<Roid>();
        roidScript2.initialOreAmount = (float)Math.Pow(2000, 7);
        roidScript2.oreAmount = roidScript2.initialOreAmount;
        roidScript2.refreshOreAmount();
        roidList.Add(tmp2);
        // tmp = Instantiate(roid);
        // tmp.transform.position = new Vector3(1, -5, 0.0f);
        // roidList.Add(tmp);
        // tmp = Instantiate(roid);
        // tmp.transform.position = new Vector3(-1, -6, 0.0f);
        // roidList.Add(tmp);
    }

    void Awake() {
        nextBoostAt= generateNextBoost();
        currentBoost = null;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Curr time: " + Time.time + " next boost at: " + nextBoostAt);
        // CREATE a boost
        if(Time.time > nextBoostAt) {
            currentBoost = Instantiate(boost);
            // TODO: This should take into account screen width/height 
            // TODO: random gen with 3/3 hardcode is stupid
            int xOffset = UnityEngine.Random.Range(-5, 5);
            int yOffset = UnityEngine.Random.Range(-5, 5);
            if(xOffset < 1 && xOffset > -1) {
                xOffset = 3;
            }
            if(yOffset < 1 && yOffset > -1) {
                yOffset = 3;
            }
            currentBoost.transform.position = new Vector3(spaceship.transform.position.x + xOffset, spaceship.transform.position.y + yOffset, -7.0f);
            Boost boostScript = currentBoost.GetComponent<Boost>();
            boostScript.diesAt = Time.time + UnityEngine.Random.Range(5, 10);
            boostScript.spaceshipScript = spaceshipScript;
            nextBoostAt = generateNextBoost();
        }
    }

    public readonly struct SpawnAnchor {
        public SpawnAnchor(int x, int y, int strength, int radius)
        {
            X = x;
            Y = y;
            RADIUS = radius;
            STRENGTH = strength;
        }

        public int X { get; }
        public int Y { get; }
        public int RADIUS { get; }
        public int STRENGTH { get; }
    }

    public static Tuple<float,float> PolarToCartesian(float angle, float radius)
    {
        float angleRad = (float)((Math.PI / 180.0) * (angle - 90));
        float x = (float)(radius * Math.Cos(angleRad));
        float y = (float)(radius * Math.Sin(angleRad));
        return Tuple.Create(x, y);
    }

    public static float generateNextBoost() {
        return Time.time + UnityEngine.Random.Range(5, 10);
    }
}
