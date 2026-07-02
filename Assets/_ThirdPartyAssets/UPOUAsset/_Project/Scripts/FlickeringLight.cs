using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    
    void Update()
    {
        StartCoroutine(WaitAndInitiate(2));
    }

    private IEnumerator WaitAndInitiate(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        this.gameObject.SetActive(!false);
    }
}
