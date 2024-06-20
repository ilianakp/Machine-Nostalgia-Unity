using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InstantiateLibrary : MonoBehaviour
{
    private GameObject[] prefabs_london;
    private GameObject[] prefabs_kyoto;
    private GameObject[] gos_ldn;
    private GameObject[] gos_kyo;
    private Vector3 scaleChange;
    Renderer rend_ldn;
    Renderer rend_kyo;
    Vector3[] center_ldn;
    Vector3[] center_kyo;

    // for bars on canvas
    private int maxBarLdn;
    private int maxBarKyo;
    public Image barLdn;
    public Image barKyo;

    private int ldnNum;
    private int kyoNum;

    public Text ldnPerc;
    public Text kyoPerc;

    private int radius = 100; //the length of box

    private Vector3 pos_new;

    // EXTRA 06/03
    string[] match_build;
    string[] match_fen;
    string[] match_gr;
    string[] match_rest;
    string[] match_ro;
    string[] match_veg;
    string[] match_veh;
    // list of indices that have been moved
    List<int> moved_models = new List<int>();

    void Start()
    {
        
        // load all prefabs from library
        prefabs_london = Resources.LoadAll<GameObject>("library_london");
        prefabs_kyoto = Resources.LoadAll<GameObject>("library_kyoto");
        // create an empty game object array 
        gos_ldn = new GameObject[prefabs_london.Length];
        gos_kyo = new GameObject[prefabs_kyoto.Length];

        center_ldn = new Vector3[prefabs_london.Length];
        center_kyo = new Vector3[prefabs_kyoto.Length];

        instantiateObjects();

        //place game objects in the correct position/orientation
        movegos(gos_ldn, 'l');
        movegos(gos_kyo, 'k');

        // values for bars
        maxBarLdn = gos_ldn.Length;
        maxBarKyo = gos_kyo.Length;

        ldnNum = maxBarLdn;
        kyoNum = 0;

        // EXTRA 06/03
        //  read txt files
        match_build = System.IO.File.ReadAllLines(@"/Users/ilianakp/Documents/AC/term2/CityAsInterface/Open3d/match_build.txt");
        match_fen = System.IO.File.ReadAllLines(@"/Users/ilianakp/Documents/AC/term2/CityAsInterface/Open3d/match_fen.txt");
        match_gr = System.IO.File.ReadAllLines(@"/Users/ilianakp/Documents/AC/term2/CityAsInterface/Open3d/match_gr.txt");
        match_rest = System.IO.File.ReadAllLines(@"/Users/ilianakp/Documents/AC/term2/CityAsInterface/Open3d/match_rest.txt");
        match_ro = System.IO.File.ReadAllLines(@"/Users/ilianakp/Documents/AC/term2/CityAsInterface/Open3d/match_road.txt");
        match_veg = System.IO.File.ReadAllLines(@"/Users/ilianakp/Documents/AC/term2/CityAsInterface/Open3d/match_veg.txt");
        match_veh = System.IO.File.ReadAllLines(@"/Users/ilianakp/Documents/AC/term2/CityAsInterface/Open3d/match_veh.txt");
    }

    void Update()
    {
        Vector3 pos = this.transform.position;

        //Time.deltaTime

        // if the player position change
        if(pos != pos_new)
        {
            // from the perimeter of the player choose random models to apear
            for (int i = 0; i < gos_ldn.Length; i++)
            {
                // Random.value returns a value from 0.0 to 1.0
                float randFlt = UnityEngine.Random.value;

                // state[i] ==0 
                if (Vector3.Distance(pos, center_ldn[i]) < 20 && randFlt > 0.5)
                {
                    if (gos_ldn[i].activeSelf)
                    {
                        gos_ldn[i].SetActive(false);
                        ldnNum -= 1; 
                        
                        
                    // EXTRA 06/03
                    // the index of the model to move in gos_kyo 
                    int index = searchGo(gos_ldn[i]);
                    // this list to check if they have been moved will be removed when we have all of the cities
                    if(index != -1 && moved_models.IndexOf(index) == -1)
                    {
                        moved_models.Add(index);
                        //MOVEMENT FUNCTION happenes here

                        kyoNum += 1;
                    }
                    }

                   
                }

                // state[i] == 1 is moving



                //state[i] == 2 is moved
            }
        }

        pos_new = this.transform.position;

        // update the bars
        barLdn.fillAmount = (float)ldnNum / (float)maxBarLdn;
        barKyo.fillAmount = (float)kyoNum / (float)maxBarKyo;

        ldnPerc.text = ((((double)ldnNum / (double)maxBarLdn) * 100).ToString() + "%");
        kyoPerc.text = ((((double)kyoNum / (double)maxBarKyo) * 100).ToString() + "%");
    }

    //public GameObject CreateLabel(GameObject model)
    //{
    //    string name;
    //    char[] nameString = model.name.ToCharArray();
    //    if (nameString[0] == 'b') name = "Building";
    //    else if (nameString[0] == 'f') name = "Fence";
    //    else if (nameString[0] == 'g') name = "Ground";
    //    else if (nameString[0] == 'r' && nameString[0] == 'e') name = "Rest";
    //    else if (nameString[0] == 'r' && nameString[0] == 'o') name = "Road";
    //    else if (nameString[0] == 'v' && nameString[0] == 'e' && nameString[0] == 'g') name = "Vegetation";
    //    else name = "Vehicle";

    //    GameObject theText = new GameObject();
    //    var textMesh = theText.AddComponent<TextMesh>();
    //    var meshRenderer = theText.AddComponent<MeshRenderer>();
    //    textMesh.text = name;
    //    if (nameString[9] == 'l') textMesh.color = new Color32(255, 255, 255, 100);
    //    else textMesh.color = new Color32(188, 188, 166, 100);

    //    return theText;
    //}

    void instantiateObjects()
    {
        // loop through all items in the library
        for (int i = 0; i < prefabs_london.Length; i++)
        {
            // instantiate each prefab from library
            GameObject clone = Instantiate(prefabs_london[i], Vector3.zero, Quaternion.identity);
            // fill the array with the game objects
            gos_ldn[i] = clone;
        }

        // loop again to fill array of kyoto fragments
        for (int i = 0; i < prefabs_kyoto.Length; i++)
        {
            GameObject clone2 = Instantiate(prefabs_kyoto[i], Vector3.zero, Quaternion.identity);
            gos_kyo[i] = clone2;
        }
    }

    void movegos(GameObject[] gos, char city)
    {
        // place in correct position
        if (city == 'l')
        {
            for (int i = 0; i < gos.Length; i++)
            {
                gos[i].transform.Rotate(new Vector3(-90, 0, 0));
                //scaleChange = new Vector3(5, 5, 5);
                //gos[i].transform.localScale += scaleChange;
                gos[i].transform.Translate(new Vector3(10.6f, 0, -7f));

                // get the renderer of the game object
                rend_ldn = gos_ldn[i].gameObject.GetComponent<Renderer>();
                //Fetch the center of the renderer
                center_ldn[i] = rend_ldn.bounds.center;
            }
        }
        else if (city == 'k')
        {
            for (int i = 0; i < gos.Length; i++)
            {
                //scaleChange = new Vector3(5, 5, 5);
                //gos[i].transform.localScale += scaleChange;
                gos[i].transform.Translate(new Vector3(0, radius, -radius));

                // get the renderer of the game object
                rend_kyo = gos_kyo[i].gameObject.GetComponent<Renderer>();
                //Fetch the center of the renderer
                center_kyo[i] = rend_kyo.bounds.center;
            }
        }
        //if (gos[i].gameObject.name.Substring(index, 1) == "n")
        //{
        //    gos[i].transform.Rotate(new Vector3(90, 0, 0));
        //    scaleChange = new Vector3(5, 5, 5);
        //    gos[i].transform.localScale += scaleChange;
        //    gos[i].transform.Translate(new Vector3(0, 0, -2 * radius));
        //}
        //if (gos[i].gameObject.name.Substring(index, 1) == "s")
        //{
        //    gos[i].transform.Rotate(new Vector3(180, 0, 0));
        //    scaleChange = new Vector3(5, 5, 5);
        //    gos[i].transform.localScale += scaleChange;
        //    gos[i].transform.Translate(new Vector3(0, -radius, -radius));
        //}
        //if (gos[i].gameObject.name.Substring(index, 1) == "s")
        //{
        //    gos[i].transform.Rotate(new Vector3(-90, 0, 0));
        //    gos[i].transform.Rotate(new Vector3(0, 90, 0));
        //    scaleChange = new Vector3(5, 5, 5);
        //    gos[i].transform.localScale += scaleChange;
        //    gos[i].transform.Translate(new Vector3(-radius / 2, 0, -radius));
        //}
    }

    int searchGo(GameObject london)
    {
        char[] nameString = london.name.ToCharArray();
        // get the label for the london go
        string label = new String(nameString, 0, 3);
        
        int index = -1;
        
        if (label == "bui")
        {
            // get the index of the london model to find the index of the string to match from file
            int index_london = -1;
            if (london.name.Length == 19) index_london = (int)london.name[10] - '0';
            else if (london.name.Length == 20) index_london = (((int)london.name[10] - '0') * 10) + (((int)london.name[11] - '0'));
            else if (london.name.Length == 21) index_london = (((int)london.name[10] - '0') * 100) + (((int)london.name[11] - '0') * 10) + (((int)london.name[12] - '0'));

            string selected = match_build[index_london];
            for (int i = 0; i < gos_kyo.Length; i++)
            {
                if (gos_kyo[i].name == selected + "(Clone)") index = i;
            }
        }
        else if (label == "fen")
        {
            int index_london = -1;
            if (london.name.Length == 17) index_london = (int)london.name[8] - '0';
            else if (london.name.Length == 18) index_london = (((int)london.name[8] - '0') * 10) + (((int)london.name[9] - '0'));
            else if (london.name.Length == 19) index_london = (((int)london.name[8] - '0') * 100) + (((int)london.name[9] - '0') * 10) + (((int)london.name[10] - '0'));

            string selected = match_fen[index_london];
            for (int i = 0; i < gos_kyo.Length; i++)
            {
                if (gos_kyo[i].name == selected + "(Clone)") index = i;
            }
        }
        else if (label == "gr_")
        {
            int index_london = -1;
            if (london.name.Length == 16) index_london = (int)london.name[7] - '0';
            else if (london.name.Length == 17) index_london = (((int)london.name[7] - '0') * 10) + (((int)london.name[8] - '0'));
            else if (london.name.Length == 18) index_london = (((int)london.name[7] - '0') * 100) + (((int)london.name[8] - '0') * 10) + (((int)london.name[9] - '0'));

            string selected = match_gr[index_london];
            for (int i = 0; i < gos_kyo.Length; i++)
            {
                if (gos_kyo[i].name == selected + "(Clone)") index = i;
            }
        }
        else if (label == "res")
        {
            int index_london = -1;
            if (london.name.Length == 18) index_london = (int)london.name[9] - '0';
            else if (london.name.Length == 19) index_london = (((int)london.name[9] - '0') * 10) + (((int)london.name[10] - '0'));
            else if (london.name.Length == 20) index_london = (((int)london.name[9] - '0') * 100) + (((int)london.name[10] - '0') * 10) + (((int)london.name[11] - '0'));

            string selected = match_rest[index_london];
            for (int i = 0; i < gos_kyo.Length; i++)
            {
                if (gos_kyo[i].name == selected + "(Clone)") index = i;
            }
        }
        else if (label == "ro_")
        {
            int index_london = -1;
            if (london.name.Length == 16) index_london = (int)london.name[7] - '0';
            else if (london.name.Length == 17) index_london = (((int)london.name[7] - '0') * 10) + (((int)london.name[8] - '0'));
            else if (london.name.Length == 18) index_london = (((int)london.name[7] - '0') * 100) + (((int)london.name[8] - '0') * 10) + (((int)london.name[9] - '0'));

            string selected = match_ro[index_london];
            for (int i = 0; i < gos_kyo.Length; i++)
            {
                if (gos_kyo[i].name == selected + "(Clone)") index = i;
            }
        }
        else if (label == "veg")
        {
            int index_london = -1;
            if (london.name.Length == 17) index_london = (int)london.name[8] - '0';
            else if (london.name.Length == 18) index_london = (((int)london.name[8] - '0') * 10) + (((int)london.name[9] - '0'));
            else if (london.name.Length == 19) index_london = (((int)london.name[8] - '0') * 100) + (((int)london.name[9] - '0') * 10) + (((int)london.name[10] - '0'));

            string selected = match_veg[index_london];
            for (int i = 0; i < gos_kyo.Length; i++)
            {
                if (gos_kyo[i].name == selected + "(Clone)") index = i;
            }
        }
        else if (label == "veh")
        {
            int index_london = -1;
            if (london.name.Length == 17) index_london = (int)london.name[8] - '0';
            else if (london.name.Length == 18) index_london = (((int)london.name[8] - '0') * 10) + (((int)london.name[9] - '0'));
            else if (london.name.Length == 19) index_london = (((int)london.name[8] - '0') * 100) + (((int)london.name[9] - '0') * 10) + (((int)london.name[10] - '0'));

            string selected = match_veh[index_london];
            for (int i = 0; i < gos_kyo.Length; i++)
            {
                if (gos_kyo[i].name == selected + "(Clone)") index = i;
            }
        }
        return index;
    }
}
