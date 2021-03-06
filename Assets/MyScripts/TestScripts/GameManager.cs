using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (OVRManager.display.displayFrequenciesAvailable.Contains(90f))
            OVRManager.display.displayFrequency = 90f;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
