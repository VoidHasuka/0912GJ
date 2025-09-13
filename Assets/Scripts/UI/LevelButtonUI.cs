using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    public Sprite noFinished;
    public Sprite showFinished;

    private void Start()
    {
    }

    public void NotFinished()
    {
        gameObject.GetComponent<Image>().sprite = noFinished;
    }

    public void Finished()
    {
        gameObject.GetComponent<Image>().sprite = showFinished;
    }
}
