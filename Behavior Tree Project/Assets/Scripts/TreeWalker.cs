using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeWalker : MonoBehaviour
{
    public Task root = null;
    bool executing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (root != null && !executing)
    //    {
    //        executing = true;
    //        StartCoroutine(WalkTheTree());
    //    }
    //}

    //IEnumerator WalkTheTree()
    //{

    //}
}
