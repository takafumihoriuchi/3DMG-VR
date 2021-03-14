using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // test script
    //[SerializeField] GameObject otakuCity = null;
    //[SerializeField] Material manga = null;

    void Awake()
    {
        if (OVRManager.display.displayFrequenciesAvailable.Contains(90f))
            OVRManager.display.displayFrequency = 90.0f;

        // Off|Low|Medium|High|HighTop ("HighTop" is the most low-resolution)
        OVRManager.fixedFoveatedRenderingLevel
            = OVRManager.FixedFoveatedRenderingLevel.Medium;
        OVRManager.useDynamicFixedFoveatedRendering = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //MeshRenderer[] otakuCityMeshRends;
        //otakuCityMeshRends = otakuCity.GetComponentsInChildren<MeshRenderer>();
        //foreach (MeshRenderer rend in otakuCityMeshRends)
        //{
        //    int cnt = rend.materials.Length;
        //    Material[] mangaRendArr = new Material[cnt];
        //    for (int i = 0; i < cnt; i++)
        //    {
        //        mangaRendArr[i] = manga;
        //    }
        //    rend.materials = mangaRendArr;
        //    Debug.Log("changed materials for " + rend);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
