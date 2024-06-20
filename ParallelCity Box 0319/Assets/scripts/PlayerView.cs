using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerView : MonoBehaviour
{
    public GameObject FirstViewCamera;
    public GameObject ThirdViewCamera;
    public GameObject player;
    public GameObject[] boxborder=  new GameObject[6];
    

    private float varia = 20;
    // Start is called before the first frame update
    void Start()
    {
        

        //stay in first view camera when start the game
        FirstViewCamera.SetActive(true);
        ThirdViewCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        LimitPlayer();
        SwitchCamera();
        LimitCamera();
    }

    void SwitchCamera()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (FirstViewCamera.activeSelf)
            {
                FirstViewCamera.SetActive(false);
                ThirdViewCamera.SetActive(true);
            }
            else if (ThirdViewCamera.activeSelf)
            {
                FirstViewCamera.SetActive(true);
                ThirdViewCamera.SetActive(false);
            }
        }
    }

    public float[] Boxbounce()
    {
        float[] scales = { boxborder[0].transform.localScale.x, boxborder[0].transform.localScale.y, boxborder[0].transform.localScale.z };
        float scale = scales.Min();

        float[] coX = new float[boxborder.Length];
        float[] coY = new float[boxborder.Length];
        float[] coZ = new float[boxborder.Length];

        for (var i = 1; i < boxborder.Length; i++)
        {
            coX[i] = boxborder[i].gameObject.GetComponent<Renderer>().bounds.center.x;
            coY[i] = boxborder[i].gameObject.GetComponent<Renderer>().bounds.center.y;
            coZ[i] = boxborder[i].gameObject.GetComponent<Renderer>().bounds.center.z;
        }
        float minx = coX.Min() + scale / 2;
        float maxx = coX.Max() - scale / 2;
        float miny = coY.Min() + scale / 2;
        float maxy = coY.Max() - scale / 2;
        float minz = coZ.Min() + scale / 2;
        float maxz = coZ.Max() - scale / 2;

        float[] bounce = { maxx, maxy, maxz, minx, miny, minz };
        return bounce;
    }

    void LimitPlayer()
    {
        Vector3 pos = player.transform.position;

        //limit player in the box
        if (pos.x > Boxbounce()[0])
        {
            pos.x = Boxbounce()[0];
        }
        if (pos.y > Boxbounce()[1])
        {
            pos.y = Boxbounce()[1];
        }
        if (pos.z > Boxbounce()[2])
        {
            pos.z = Boxbounce()[2];
        }
        if (pos.x < Boxbounce()[3])
        {
            pos.x = Boxbounce()[3];
        }
        if (pos.y < Boxbounce()[4])
        {
            pos.y = Boxbounce()[4];
        }
        if (pos.z < Boxbounce()[5])
        {
            pos.z = Boxbounce()[5];
        }
        Vector3 correction = pos - player.transform.position;
        player.transform.Translate(correction, Space.World);
    }

    void LimitCamera()
    {
        Vector3 pos = ThirdViewCamera.transform.position;
        //limit third view camera in the box and the range is little bit smaller than the box
        if (pos.x > Boxbounce()[0] - varia)
        {
            pos.x = Boxbounce()[0] - varia;
        }
        if (pos.y > Boxbounce()[1] - varia)
        {
            pos.y = Boxbounce()[1] - varia;
        }
        if (pos.z > Boxbounce()[2] - varia)
        {
            pos.z = Boxbounce()[2] - varia;
        }
        if (pos.x < Boxbounce()[3] + varia)
        {
            pos.x = Boxbounce()[3] + varia;
        }
        if (pos.y < Boxbounce()[4] + varia)
        {
            pos.y = Boxbounce()[4] + varia;
        }
        if (pos.z < Boxbounce()[5] + varia)
        {
            pos.z = Boxbounce()[5] + varia;
        }

        Vector3 correction = pos - ThirdViewCamera.transform.position;
        ThirdViewCamera.transform.Translate(correction, Space.World);
    }

 
}
