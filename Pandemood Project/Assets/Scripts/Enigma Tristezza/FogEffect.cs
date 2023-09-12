using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogEffect : MonoBehaviour
{
    public GameObject WaterSurface;
    public GameObject RocksIsland;
    public GameObject Palms;
    public GameObject BlueUnderWater;

    private bool isUnderwater;
    private Color normalColor;
    private Color underwaterColor;

    // Use this for initialization
    void Start()
    {
        normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        underwaterColor = new Color(0.14f, 0.37f, 0.58f, 0.6f);
        BlueUnderWater.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.y < WaterSurface.transform.position.y) != isUnderwater)
        {
            isUnderwater = transform.position.y < WaterSurface.transform.position.y;
            if (isUnderwater) SetUnderwater();
            if (!isUnderwater) SetNormal();
        }
    }

    void SetNormal()
    {
        //RenderSettings.fog = false;
        RenderSettings.fogColor = normalColor;
        RenderSettings.fogDensity = 0f;

    }

    void SetUnderwater()
    {
        //RenderSettings.fog = true;
        RenderSettings.fogColor = underwaterColor;
        RenderSettings.fogDensity = 0.01f;
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        if (RocksIsland != null)
            Destroy(RocksIsland);
        if (Palms != null)
            Destroy(Palms);

        BlueUnderWater.SetActive(true);
    }
}
