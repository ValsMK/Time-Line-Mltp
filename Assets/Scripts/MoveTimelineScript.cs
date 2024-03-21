using Mirror;
using UnityEngine;

public class MoveTimelineScript : NetworkBehaviour
{
    public GameObject Line;

    public void  MoveLeft()
    {
        Line.transform.position += new Vector3(1.5f, 0f, 0f);
    }

    public void MoveRight()
    {
        Line.transform.position -= new Vector3(1.5f, 0f, 0f);
    }
}