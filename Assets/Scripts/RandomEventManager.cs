using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RandomEventManager : MonoBehaviour
{
    public struct FireRoom
    {
        public bool canFire;
        public bool isInFire;
        public float FireProbability;
        public int FireTickToDestoyed;
    }

    [SerializeField] BuildingController buildingController;
    [SerializeField] private PigenShitController pigeonShitController;
    [Header("------EVENT TICKS------")]
    [SerializeField] public int eventComesInTurns;
    [SerializeField] private int eventComesInTurnsMax;
    [SerializeField] private int eventComesInTurnsMin;

    [Header("------NUMBER OF FIRES TO GROW------")]
    [SerializeField] private int spawnNewNumberOfFires;
    [SerializeField] private int spawnNewNumberOfFiresMax;
    [SerializeField] private int spawnNewNumberOfFiresMin;

    [Header("------FIRE TO BE DESTROY TICKS------")]
    [SerializeField] public int fireTicks;

    [Header("------PIGEON SHIT------")]
    [SerializeField] public int ticksToPigeonShit;
    [SerializeField] public int ticksToPigeonShitMax;
    [SerializeField] public int ticksToPigeonShitMin;

    [Header("------CONST------")]
    [SerializeField] public bool sequentialEvents = false;
    [SerializeField] public float probabilityOfNeighbourFire;
    [SerializeField] public bool bombDrop = false;

    [Header("------EVENTS------")]
    public UnityEvent OnPigeonFired;
    public UnityEvent OnPigeonShit;
    public UnityEvent<int, int> OnFireSpawn;
    public UnityEvent<int, int> OnRoomDestroyed;

    public FireRoom[,] ProbabilityFireMatrix;


    [Header("------DEBUG------")]
    public bool tick = false;

    public bool start = true;

    // Start is called before the first frame update
    void Start()
    {
        if (buildingController == null)
        {
            buildingController = FindObjectOfType<BuildingController>();
        }

        if (pigeonShitController == null)
        {
            pigeonShitController = FindObjectOfType<PigenShitController>();
        }
        ProbabilityFireMatrix = new FireRoom[buildingController.width, buildingController.height];

        eventComesInTurns = Random.Range(eventComesInTurnsMin, eventComesInTurnsMax);
        ticksToPigeonShit = Random.Range(ticksToPigeonShitMin, ticksToPigeonShitMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (tick)
        {
            if (start)
            {
                FillFireMatrix();
                start = false;
            }
            tick = false;
            ComputeFireMatrix();
            EvaluateFireMatrix();

        }

        if (ticksToPigeonShit == 0)
        {
            ticksToPigeonShit = Random.Range(ticksToPigeonShitMin, ticksToPigeonShitMax);
            pigeonShitController.Shit();
        }
        

        if (eventComesInTurns == 0)
        {
            tick = true;
            eventComesInTurns = Random.Range(eventComesInTurnsMin, eventComesInTurnsMax);
        }
    }

    public void turnsUpdate()
    {
        eventComesInTurns--;
        ticksToPigeonShit--;
        
    }

    void FillFireMatrix()
    {
        for (int y = 0; y < buildingController.map.GetLength(1); y++)
        {
            for (int x = 0; x < buildingController.map.GetLength(0); x++)
            {
                BuildingController.Room val;
                val = buildingController.map[x, y];

                if (val.onFire && val.type != BuildingController.RoomType.Empty)
                {
                    ProbabilityFireMatrix[x, y] = new FireRoom
                    {
                        canFire = true,
                        isInFire = true,
                        FireProbability = 1f,
                        FireTickToDestoyed = 0
                    };

                }
                else
                {
                    ProbabilityFireMatrix[x, y] = new FireRoom
                    {
                        canFire = true,
                        isInFire = false,
                        FireProbability = 0f,
                        FireTickToDestoyed = 0
                    };
                }
            }
            ProbabilityFireMatrix[0, y].canFire = false;
            ProbabilityFireMatrix[buildingController.width - 1, y].canFire = false;
        }
    }

    void EvaluateFireMatrix()
    {
        for (int k = 0; k < ProbabilityFireMatrix.GetLength(0); k++)
        {
            for (int l = 0; l < ProbabilityFireMatrix.GetLength(1); l++)
            {
                if (buildingController.map[k, l].type == BuildingController.RoomType.Empty)
                {
                    ProbabilityFireMatrix[k, l].canFire = false;
                    ProbabilityFireMatrix[k, l].isInFire = false;
                }
                
                if (!buildingController.map[k, l].onFire)
                {
                    ProbabilityFireMatrix[k, l].isInFire = false;
                }
                
                if (ProbabilityFireMatrix[k, l].isInFire)
                {
                    if (ProbabilityFireMatrix[k, l].FireTickToDestoyed == 2 &&
                        l > 1)
                    {
                        OnRoomDestroyed.Invoke(k, l);
                        ColumnUpdate(k, l);
                        pigeonShitController.UpdateBird();
                    }
                    else
                    {
                        var obj = buildingController.map[k, l].innerGameObject.transform;
                        int index = 0;
                        foreach(Transform child in obj)
                        {
                            if(child.gameObject.activeSelf)
                                index = child.GetSiblingIndex();
                        }

                        if (index < 2)
                        {
                            obj.transform.GetChild(index).gameObject.SetActive(false);
                            obj.transform.GetChild(index + 1).gameObject.SetActive(true);
                        }

                        //obj.GetChild(ProbabilityFireMatrix[k, l].FireTickToDestoyed).gameObject.SetActive(true);
                        ProbabilityFireMatrix[k, l].FireTickToDestoyed++;
                    }
                }
            }
        }
    }

    private void ColumnUpdate(int col, int row)
    {
        ProbabilityFireMatrix[col, row].isInFire = false;

        //OnRoomDestroyed.Invoke();

        for (int y = row; y < buildingController.height - 1; y++)
        {
            ProbabilityFireMatrix[col, y] = ProbabilityFireMatrix[col, y + 1];
            //MoveRoomObject(column, y, column, y + 1);
        }

    }

    void ComputeFireMatrix()
    {
        spawnNewNumberOfFires = Random.Range(spawnNewNumberOfFiresMin, spawnNewNumberOfFiresMax);
        List<Tuple<int, int>> fires = new List<Tuple<int, int>>();


        for (int k = 0; k < ProbabilityFireMatrix.GetLength(0); k++)
        {
            for (int l = 0; l < ProbabilityFireMatrix.GetLength(1); l++)
            {
                if (!buildingController.map[k, l].onFire)
                {
                    ProbabilityFireMatrix[k, l].isInFire = false;
                }
                
                if (ProbabilityFireMatrix[k, l].isInFire)
                {
                    //fires.Add(new Tuple<int, int>(k, l));

                    if (k != 0 &&
                        ProbabilityFireMatrix[k - 1, l].canFire &&
                        !ProbabilityFireMatrix[k - 1, l].isInFire)
                    {
                        ProbabilityFireMatrix[k - 1, l].FireProbability += probabilityOfNeighbourFire;
                        fires.Add(new Tuple<int, int>(k - 1, l));
                    }
                    if (k != ProbabilityFireMatrix.GetLength(0) - 1 &&
                        ProbabilityFireMatrix[k + 1, l].canFire &&
                        !ProbabilityFireMatrix[k + 1, l].isInFire)
                    {
                        ProbabilityFireMatrix[k + 1, l].FireProbability += probabilityOfNeighbourFire;
                        fires.Add(new Tuple<int, int>(k + 1, l));
                    }
                    if (l != 0 &&
                        ProbabilityFireMatrix[k, l - 1].canFire &&
                        !ProbabilityFireMatrix[k, l - 1].isInFire)
                    {
                        ProbabilityFireMatrix[k, l - 1].FireProbability += probabilityOfNeighbourFire;
                        fires.Add(new Tuple<int, int>(k, l - 1));
                    }
                    if (l != ProbabilityFireMatrix.GetLength(1) - 1 &&
                        ProbabilityFireMatrix[k, l + 1].canFire &&
                        !ProbabilityFireMatrix[k, l + 1].isInFire)
                    {
                        ProbabilityFireMatrix[k, l + 1].FireProbability += probabilityOfNeighbourFire;
                        fires.Add(new Tuple<int, int>(k, l + 1));
                    }

                }
            }
        }

        if (fires.Count < spawnNewNumberOfFires)
        {
            spawnNewNumberOfFires = fires.Count;
        }

        for (int i = 0; i < spawnNewNumberOfFires; i++)
        {
            var rnd = Random.Range(0, fires.Count);
            Tuple<int, int> finalFire = new Tuple<int, int>(fires[rnd].Item1, fires[rnd].Item2);
            //float mostProbability = 0;

            //var k = fires[rnd].Item1;
            //var l = fires[rnd].Item2;

            // if (k != 0 && 
            //     ProbabilityFireMatrix[k - 1, l].canFire && 
            //     !ProbabilityFireMatrix[k - 1, l].isInFire && 
            //     ProbabilityFireMatrix[k - 1, l].FireProbability >= mostProbability)
            // {
            //     finalFire = new Tuple<int, int>(k - 1, l);
            // }
            //
            // if (k != ProbabilityFireMatrix.GetLength(0) - 1 &&
            //     ProbabilityFireMatrix[k + 1, l].canFire &&
            //     !ProbabilityFireMatrix[k + 1, l].isInFire && 
            //     ProbabilityFireMatrix[k + 1, l].FireProbability >= mostProbability)
            // {
            //     finalFire = new Tuple<int, int>(k + 1, l);
            // }
            // if (l != 0 &&
            //     ProbabilityFireMatrix[k, l - 1].canFire &&
            //     !ProbabilityFireMatrix[k, l - 1].isInFire && 
            //     ProbabilityFireMatrix[k, l - 1].FireProbability >= mostProbability)
            // {
            //     finalFire = new Tuple<int, int>(k, l - 1);
            // }
            // if (l != ProbabilityFireMatrix.GetLength(1) - 1 && 
            //     ProbabilityFireMatrix[k, l + 1].canFire &&
            //     !ProbabilityFireMatrix[k, l + 1].isInFire && 
            //     ProbabilityFireMatrix[k, l + 1].FireProbability >= mostProbability)
            // {
            //     finalFire = new Tuple<int, int>(k, l + 1);
            // }

            if (buildingController.player.playerPosition.x == finalFire.Item1 &&
                buildingController.player.playerPosition.y == finalFire.Item2)
            {
                fires.RemoveAt(rnd);
                return;
            }

            ProbabilityFireMatrix[finalFire.Item1, finalFire.Item2].isInFire = true;
            ProbabilityFireMatrix[finalFire.Item1, finalFire.Item2].FireProbability = 1;
            OnFireSpawn.Invoke(finalFire.Item1, finalFire.Item2);
            fires.RemoveAt(rnd);

        }

        // for (int k = 0; k < ProbabilityFireMatrix.GetLength(0); k++)
        // {
        //     for (int l = 0; l < ProbabilityFireMatrix.GetLength(1); l++)
        //     {
        //         if (ProbabilityFireMatrix[k, l].canFire)
        //         {
        //             float rnd = Random.value;
        //             if (rnd <= ProbabilityFireMatrix[k, l].FireProbability)
        //             {
        //                 ProbabilityFireMatrix[k,l].isInFire = true;
        //                 ProbabilityFireMatrix[k, l].FireProbability = 1;
        //                 OnFireSpawn.Invoke(k, l);
        //                 Debug.Log("Fire Spawned: " + rnd + " / " + ProbabilityFireMatrix[k, l].FireProbability);
        //             }
        //         }
        //     }
        // }
    }

}
