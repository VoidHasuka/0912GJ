using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootUI : MonoBehaviour
{
    public Sprite imgDown;
    public Sprite imgUp;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            gameObject.GetComponent<Image>().sprite = imgDown;
        }

        if (Input.GetMouseButtonUp(1))
        {
            gameObject.GetComponent<Image>().sprite = imgUp;
        }
    }
}
