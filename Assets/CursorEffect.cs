using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CursorEffect : MonoBehaviour
{
    public GameObject Eff;
    private GameObject effect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("hello");
            this.gameObject.GetComponent<Image>().rectTransform.DOScale(new Vector3(0.4f, 0.4f, 0.4f), 0.2f).From();


            effect = GameObject.Instantiate(Eff,this.transform.parent.transform);
            effect.transform.position = this.transform.position;
        }
    }
}
