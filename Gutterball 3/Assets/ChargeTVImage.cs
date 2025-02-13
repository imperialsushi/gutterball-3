using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeTVImage : MonoBehaviour
{
    public string urlImages;

    IEnumerator Start()
    {
        WWW www = new WWW(urlImages);
        yield return www;
        GetComponent<RawImage>().texture = www.texture;
    }
}
