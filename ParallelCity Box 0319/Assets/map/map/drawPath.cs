using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawPath : MonoBehaviour
{

    bool pause = true;
    static List<GameObject> lines = new List<GameObject>();
    static List<LineRenderer> newLine = new List<LineRenderer>();

    Vector3 position;
    Vector3 position_new;
    static List<Vector3> positions = new List<Vector3>();

    private int countLines = 0;
    private int frameCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("space pressed");
            pause = !pause;
        }

        if (frameCount % 20 == 0)
        {
            position = this.transform.position;
            position.y = 1350;

            // check if the game is not paused and the player has changed position
            //
            if (!pause && position != position_new)
            {
                positions.Add(position);

                if (positions.Count > 1)
                {
                    lines.Add(new GameObject());
                    newLine.Add(lines[countLines].AddComponent<LineRenderer>());
                    newLine[countLines].SetWidth(2f, 2f);
                    newLine[countLines].material = new Material(Shader.Find("Sprites/Default"));
                    newLine[countLines].SetColors(Color.blue, Color.blue);
                    newLine[countLines].positionCount = 2; // start and end
                    newLine[countLines].SetPosition(0, positions[countLines]);
                    newLine[countLines].SetPosition(1, positions[countLines + 1]);
                    countLines++;
                }

            }

            position_new = position;
        }
        frameCount++;
    }
}
