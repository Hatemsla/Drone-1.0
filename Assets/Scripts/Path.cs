using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Transform> nodes;

    private void Awake()
    {
        GetNodes();
    }

    private void GetNodes()
    {
        nodes.Add(transform.GetChild(0).transform);
    }
}