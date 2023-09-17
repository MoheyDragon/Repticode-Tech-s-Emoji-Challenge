using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopicExit : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            BenefitsManager3D.Instance.OntopicExit();
        }    
    }
}
