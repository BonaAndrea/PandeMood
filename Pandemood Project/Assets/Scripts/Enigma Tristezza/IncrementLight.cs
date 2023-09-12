using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementLight : MonoBehaviour
{
    public Light directionalLight;
    public Material matReflectionMirror;
    public float timeToStartLight = 30f;
    public GameObject WaterMirrorSurface;

    bool start_light = false;
    float lightIntensity = 0.5f;

    Color colorStart = new Color(1f, 1f, 1f, 1f);
    //Color colorEnd = new Color(0.15f, 0.4f, 0.5f, 1f);
    Color colorEnd = new Color(1f, 0f, 0.78f, 1f);
    private float t = 0;
    public float durationChangeColor = 20.0f;


    // Start is called before the first frame update
    void Start()
    {
        matReflectionMirror.SetFloat("_MainAlpha", 0.2f);
        matReflectionMirror.color = colorStart;
        StartCoroutine(StartLight());
    }

    // Update is called once per frame
    void Update()
    {
        if(lightIntensity>0.5f)
           ChangeColorSmoothly();
    }

    private IEnumerator StartLight()
    {
        yield return new WaitForSeconds(timeToStartLight);
        start_light = true;
        StartCoroutine(StartLighting());
    }

    //OGNI 0.5 SECONDI AUMENTA L'INTENSITA DELLA LUCE DI 0.05 e ALPHA DEL MATERIALE DI 0.05
    private IEnumerator StartLighting()
    {

        while (start_light)
        {
            lightIntensity += 0.01f; //intensità di luce directional light
            directionalLight.intensity = lightIntensity;
            yield return new WaitForSeconds(0.5f);
            if (lightIntensity > 0.99f)
            {
                start_light = false;
            }
        }
    }

    void ChangeColorSmoothly()
    {
        matReflectionMirror.color = Color.Lerp(colorStart, colorEnd, t);

        //------------TIME TO CHANGE COLOR----------
        if (t < 1)
        { // while t below the end limit... increment it at the desired rate every update:
            t += Time.deltaTime / durationChangeColor;
        }
    }
}
