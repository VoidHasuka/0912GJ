using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Cursor2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        this.GetComponent<Image>().rectTransform.DORotate(new Vector3(0,0,180f),0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
