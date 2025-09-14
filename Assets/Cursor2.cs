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

        this.GetComponent<Image>().rectTransform.DORotate(new Vector3(0, 0, 90f), 0.7f).SetEase(Ease.InCubic);
        this.GetComponent<Image>().rectTransform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f).SetEase(Ease.OutSine);
        this.GetComponent<Image>().DOColor(new Color(0, 0, 0,0), 0.7f).OnStepComplete(() => { Die(); });
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void Die()
    {
        GameObject.DestroyObject(this.gameObject); 
    }
}
