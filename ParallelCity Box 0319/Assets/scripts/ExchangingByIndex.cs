using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;
using System.IO;

public class ExchangingByIndex : MonoBehaviour
{
    static bool CountVisual = true;
    static bool CountMove = true;
    static bool CountApproxi = true;

    static private int radius = 500; //the length of box
    public GameObject playerbody;
    static private Vector3 pos;
    private Vector3 pos_new;

    // as the folder names in the library in resources
    // order of cities influence on which side of the box the city will be places
    static string[] cityNames = { "library_london", "library_newYork", "library_tokyo", "library_paul", "library_sydney", "library_johannesburg"};

    // box to place
    public GameObject[] faces;
    static Vector3[] axis; 
    static int[] angles;

    string pathToMatch = "/Users/ilianakp/Documents/AC/term2/CityAsInterface/Open3d/";
    string[] matchFiles = { "match_build", "match_fen", "match_gr", "match_rest", "match_road", "match_veg", "match_veh" };
    List<string> matchTwin1 = new List<string>();
    List<string> matchTwin2 = new List<string>();

    static GameObject[] fragmentsGos;
    List<movetwins> twins = new List<movetwins>();
    List<SelectTwinsMove> Movebility = new List<SelectTwinsMove>();
    static List<Vector3> playerpos = new List<Vector3>();

    //for Extra enteractions
    bool pause = true;
    public bool StartStage;///
    int playerstate = 0;

    //for Bars
    public GameObject[] bars;
    static int[] barsMax = new int[cityNames.Length];
    static int[] barsCurrent = new int[cityNames.Length];

    //Canvases
    public GameObject OpeningCanvas;

    void Start()
    {
        // for each face of the box find the equivalent axis and angle of rotation
        axis = new Vector3[faces.Length];
        angles = new int[faces.Length];
        SetUpBox();

        // loop through the cities and place them
        for (int i = 0; i < cityNames.Length; i++)
        {
            GameObject[] prefabs = Resources.LoadAll<GameObject>(cityNames[i]);
            Vector3 cityCenter = getCityCenter(prefabs);
            Vector3 faceCenter = GetCenter(faces[i]);
            GameObject[] gos_instantiated = instantiateObjects(prefabs,i);
            barsMax[i] = prefabs.Length;
        }

        // loop through the text files and store them in two lists
        for (int i = 0; i < matchFiles.Length; i++)
        {
            string[] match1 = System.IO.File.ReadAllLines(pathToMatch + matchFiles[i] + "1.txt");
            for (int j = 0; j < match1.Length; j++)
            {
                // add the first twin in the twin1 list
                matchTwin1.Add(match1[j]);
            }
            string[] match2 = System.IO.File.ReadAllLines(pathToMatch + matchFiles[i] + "2.txt");
            for (int k = 0; k < match2.Length; k++)
            {
                matchTwin2.Add(match2[k]);
            }
        }

        // retrieve all the game object fragments from their tag
        fragmentsGos = GameObject.FindGameObjectsWithTag("fragment");
        // loop through the match twins lists, find the game objects and create a moveintwins instance for each couple
        for (int i = 0; i < matchTwin1.Count; i++)
        {
            // try to find the game objects
            int?[] indices = searchGo(matchTwin1[i], matchTwin2[i], fragmentsGos);

            if (indices[0].HasValue && indices[1].HasValue)
            {
                int index1 = (int)indices[0];
                int index2 = (int)indices[1];
                twins.Add(new movetwins(fragmentsGos[index1], fragmentsGos[index2]));
                Movebility.Add(new SelectTwinsMove(twins[twins.Count-1]));
            }               
        }


        // Entering game with start stage
        if (StartStage)
        {
            playerstate = 0;
            OpeningCanvas.SetActive(true);
            playerbody.GetComponent<BoxCollider>().enabled = false;
            playerbody.GetComponent<rotateCamera>().enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {

        pos = playerbody.transform.position;
        playerpos.Add(pos);

        if (StartStage&&Input.GetKeyDown(KeyCode.Return) && playerstate==0)
        {
            //Change everything to game stage
            playerstate = 1;
            OpeningCanvas.SetActive(false);
            playerbody.GetComponent<BoxCollider>().enabled = true;
            playerbody.GetComponent<rotateCamera>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && playerstate==1)
        {
            pause = !pause;
        }

        //Moving Segments
        if (!pause)
        {
            for (int i = 0; i < twins.Count; i++)
            {
                Movebility[i].update_poss();
            }

            // if the player position change
            if (pos != pos_new)
            {
                float randFlt = 0.99F;
                for (int i = 0; i < twins.Count; i++)
                {//(Vector3.Distance(GetCenter(twins[i].pieceA), pos) < radius / 10 || Vector3.Distance(GetCenter(twins[i].pieceB), pos) < radius / 10) &&
                    if ( Movebility[i].IfMove(randFlt) && twins[i].state == 0)
                    {
                        //print(Movebility[i].posibility);
                        twins[i].SetMove();
                        UpdateBars(twins[i]);
                    }
                }
            }

            for (int i = 0; i < twins.Count; i++)
            {
                if (twins[i].state == 1)
                {
                    twins[i].UpdateMove();
                    twins[i].IfArrived();
                }
            }
        }
        
        pos_new = pos;
                    
    }

    void UpdateBars(movetwins twins)
    {
        //Update barCurrent by piece A
        {
            int indexA = twins.pieceA.name.Length - 8;
            print(twins.pieceA.name[indexA]);
            if (twins.pieceA.name[indexA] == 'l')
            {barsCurrent[0]++;}
            if (twins.pieceA.name[indexA] == 'n')
            {barsCurrent[1]++;}
            if (twins.pieceA.name[indexA] == 'p')
            {barsCurrent[2]++;}
            if (twins.pieceA.name[indexA] == 's')
            {barsCurrent[3]++;}
            if (twins.pieceA.name[indexA] == 't')
            {barsCurrent[4]++;}
            if (twins.pieceA.name[indexA] == 'j')
            {barsCurrent[5]++;}
        }
        //Update barCurrent by piece B
        {
            int indexB = twins.pieceB.name.Length - 8;
            
            if (twins.pieceB.name[indexB] == 'l')
            { barsCurrent[0]++; }
            if (twins.pieceB.name[indexB] == 'n')
            { barsCurrent[1]++; }
            if (twins.pieceB.name[indexB] == 'p')
            { barsCurrent[2]++; }
            if (twins.pieceB.name[indexB] == 's')
            { barsCurrent[3]++; }
            if (twins.pieceB.name[indexB] == 't')
            { barsCurrent[4]++; }
            if (twins.pieceB.name[indexB] == 'j')
            { barsCurrent[5]++; }
        }
        //Pass percentage to bars
        for (int i = 0; i < barsCurrent.Length; i++)
        {
            bars[i].transform.GetChild(3).transform.GetChild(0).GetComponent<Image>().fillAmount = (float)barsCurrent[i] / barsMax[i];
            //print(barsCurrent[i]+" / "+barsMax[i]+" = "+ (float)barsCurrent[i] / barsMax[i]);

            if (barsCurrent[i] == barsMax[i])
            { bars[i].transform.GetChild(1).GetComponent<Text>().text = 100.ToString(); }
            else
            { bars[i].transform.GetChild(1).GetComponent<Text>().text = ((int)((float)barsCurrent[i] / barsMax[i] * 100)).ToString(); 
            }
        }
        
    }

    void SetUpBox()
    {
        axis[0] = new Vector3(1, 0, 0);
        angles[0] = -90;
        axis[1] = axis[0];
        angles[1] = 90;
        axis[2] = new Vector3(0, 1, 0);
        angles[2] = 180;
        axis[3] = new Vector3(0, 0, 0);
        angles[3] = 0;
        axis[4] = axis[2];
        angles[4] = 90;
        axis[5] = axis[2];
        angles[5] = -90;
    }

    // second input int num may be not needed when all the cities are placed
    GameObject[] instantiateObjects(GameObject[] pref, int num)
    {
        GameObject[] gos_current = new GameObject[pref.Length];
        for (int i = 0; i < pref.Length; i++)
        {
            GameObject clone = Instantiate(pref[i], Vector3.zero, Quaternion.identity);
            // fill the array with the game objects
            gos_current[i] = clone;
            //give each object a tag
            gos_current[i].tag = "fragment";
        }
        gos_current = moveTo(gos_current, num);
        return gos_current;
    }

    GameObject[] moveTo(GameObject[] gos, int face)
    {
        Vector3 citycenter = getCityCenter(gos);
        Vector3 faceCent = GetCenter(faces[face]);
        // place in correct position
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i].transform.Translate(faceCent - citycenter, Space.World);
            // extra translation so the player can see
            if (face == 0)
            {
                gos[i].transform.Translate(new Vector3(0, 0, -12.6f), Space.World);
            }
            gos[i].transform.RotateAround(faceCent, axis[face], angles[face]);
        }
        return gos;
    }

    //New class 0316 A
    class SelectTwinsMove
    {
        //field
        movetwins twins;
        public float posibility;
        public float cityVisCounts ;
        public float cityMoveToCounts;
        public float approximatyCounts;

        ///constructor/////////////////
        public SelectTwinsMove(movetwins thistwins)
        {
            twins = thistwins;

            posibility = 0;
            cityVisCounts = 0;
            cityMoveToCounts = 0;
        }
        ///method//////////////////////
        //update
        public void update_poss()
        {
            posibility *= 0.5f;

            if (CountVisual)
            {
                cityVisCounts += (IsInView(twins.pieceA) + IsInView(twins.pieceB)) / 2; //returns a number within 0-1
                posibility += cityVisCounts /3;
                cityVisCounts = 0;
            }

            if (CountMove && twins.state == 0 && (Vector3.Distance(GetCenter(twins.pieceA), pos) < 100 || Vector3.Distance(GetCenter(twins.pieceB), pos) < 100))
            {
                cityMoveToCounts += (GetMovepreference(twins.pieceA) + GetMovepreference(twins.pieceB)) / 2; //returns a number within 0-1
                posibility += cityMoveToCounts/3;
                cityMoveToCounts = 0;
            }

            if (CountApproxi)
            {
                approximatyCounts += (GetApproximaty(twins.pieceA) + GetApproximaty(twins.pieceB)) / 2;//returns a number within 0-1
                posibility += approximatyCounts/3;
                approximatyCounts = 0;
            }

            posibility = Mathf.Clamp(posibility, 0, 1);

        }

        //ifmove
        public bool IfMove(float randomseed)
        {
            if(randomseed<posibility) return true;
            else return false;
        }

        ///functions///////////////////
        int IsInView(GameObject _piece)
        {
            Vector3 worldPos = GetCenter(_piece);
            Transform camTransform = Camera.main.transform;
            Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
            Vector3 dir = (worldPos - camTransform.position).normalized;
            float dot = Vector3.Dot(camTransform.forward, dir);
            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            { return 1; }
            else
            { return 0; }
        }

        float GetMovepreference(GameObject _piece)
        {
            if (playerpos.Count > 60)//the game needs to run at least 1 second
            {
                //the direction of the player movement for the past 1s
                Vector3 moving = playerpos[playerpos.Count - 1] - playerpos[playerpos.Count - 61];
                //the direction from the player to the cube
                Vector3 direction = GetCenter( _piece) - playerpos[playerpos.Count - 1];

                if (moving.magnitude != 0)
                {
                    float angle = Vector3.Angle(direction, moving);

                    if (angle >= 0 && angle < 90)
                    {
                        float a = (90 - angle) / 90;
                        return a;
                    }
                    else return 0;
                }
                else return 0;
            }
            else return 0;
        }

        float GetApproximaty(GameObject _piece)
        {
            float dis = Vector3.Distance(playerpos[playerpos.Count - 1], GetCenter(_piece));
            float app = Mathf.Clamp(1/dis, 0, 1);
            return app;
        }
    }

    class movetwins
    {
        //field
        public GameObject pieceA, pieceB;
        public Vector3 destA, destB;
        public int state; // 0 = ready to move; 1 = is moving; 2 = finished movement;
        float speed;
        Vector3 A_vlc, B_vlc;

        public movetwins(GameObject _pieceA, GameObject _pieceB)
        {
            state = 0;
            speed = 0.5f;
            pieceA = _pieceA;
            pieceB = _pieceB;
            destA = GetCenter(pieceB);
            destB = GetCenter(pieceA);
        }

        //01set move
        public void SetMove()
        {
            if (state == 0)
            {
                state = 1;

                Vector3 A_dir = destA - destB;
                Vector3 B_dir = destB - destA;

                A_vlc = A_dir.normalized * speed;
                B_vlc = B_dir.normalized * speed;

            }
        }
        //02 update move
        public void UpdateMove()
        {
            if (state == 1)
            {
                pieceA.transform.Translate(A_vlc, Space.World); //Space.world make the movement happened in world coordinate system.
                pieceB.transform.Translate(B_vlc, Space.World);
            }
        }
        //03 if arrive
        public void IfArrived()
        {
            if ((Vector3.Distance(GetCenter(pieceA), destA) < 1.0f) && (state == 1))
            {
                state = 2;

                Vector3 A_direction = destA - GetCenter(pieceA);
                pieceA.transform.Translate(A_direction, Space.World);

                Vector3 B_direction = destB - GetCenter(pieceB);
                pieceB.transform.Translate(B_direction, Space.World);

                RotatePiece();
            }
        }
        void RotatePiece()
        {
            // find which city
            string nameA = pieceA.name;
            string nameB = pieceB.name;
            int iA = 0; ;
            int iB = 0;
            for (int i = 0; i < cityNames.Length; i++)
            {
                string cityName = cityNames[i];
                // find the city where it belongs to find the face of the box
                if(nameA[nameA.Length - 8] == cityName[8])
                {
                    //barsCurrent[i]++;
                    //bars[i].fillAmount = barsCurrent[i];
                    // first return it to the initial position
                    iA = i;
                   
                }

                if (nameB[nameB.Length - 8] == cityName[8])
                {
                    //barsCurrent[i]++;
                    //bars[i].fillAmount = barsCurrent[i];
                    // first return it to the initial position
                    iB = i;
                }
            }
            pieceA.transform.RotateAround(GetCenter(pieceA), axis[iA], -angles[iA]);
            pieceA.transform.RotateAround(GetCenter(pieceA), axis[iB], angles[iB]);
            pieceB.transform.RotateAround(GetCenter(pieceB), axis[iB], -angles[iB]);
            pieceB.transform.RotateAround(GetCenter(pieceB), axis[iA], angles[iA]);
        }
    }

    int? [] searchGo(string nameMatch1, string nameMatch2, GameObject[] fragments)
    {
        // create a nullable int array
        int?[] indicesFound = new int?[2];
        for (int i = 0; i < fragments.Length; i++)
        {
            if (fragments[i].name == nameMatch1 + "(Clone)")
            {
                indicesFound[0] = i;
            }
            else if (fragments[i].name == nameMatch2 + "(Clone)")
            {
                indicesFound[1] = i;
            }
            // if we found the models already exit the loop
            if (indicesFound[0].HasValue && indicesFound[1].HasValue) break;
        }
        return indicesFound;
    }

    static Vector3 GetCenter(GameObject piece)
    {
        Vector3 center = piece.GetComponent<Renderer>().bounds.center;
        return center;
    }

    Vector3 getCityCenter(GameObject[] gos)
    {
        float[] minX = new float[gos.Length];
        float[] maxX = new float[gos.Length];
        float[] minY = new float[gos.Length];
        float[] maxY = new float[gos.Length];
        float[] minZ = new float[gos.Length];
        float[] maxZ = new float[gos.Length];

        for (var i = 1; i < gos.Length; i++)
        {
            maxX[i] = gos[i].GetComponent<Renderer>().bounds.max.x;
            minX[i] = gos[i].GetComponent<Renderer>().bounds.min.x;
            maxY[i] = gos[i].GetComponent<Renderer>().bounds.max.y;
            minY[i] = gos[i].GetComponent<Renderer>().bounds.min.y;
            maxZ[i] = gos[i].GetComponent<Renderer>().bounds.max.z;
            minZ[i] = gos[i].GetComponent<Renderer>().bounds.min.z;
        }

        float minx = minX.Min();
        float maxx = maxX.Max();
        float miny = minY.Min();
        float maxy = maxY.Max();
        float minz = minZ.Min();
        Vector3 CityCenter = new Vector3((minx + maxx) / 2, (miny + maxy) / 2, minz);
        return CityCenter;
    }
}