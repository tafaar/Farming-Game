using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    [Header("Time")]
    public float globalTime;
    float _localMinutes;
    public int minutes;
    public int hours;
    public string period;
    public int day;
    bool _periodSwitch;


    public enum Season { SPRING, SUMMER, FALL, WINTER}
    [Header("Seasons")]
    public Season season;

    public Material grassMat;
    public Color[] grassColors;
    public Material dirtMat;
    public Color[] dirtColors;

    public Material[] leafMaterials;

    public Material mapleLeaf;
    public Color[] mapleColors;
    public Material oakLeaf;
    public Color[] oakColors;
    public Material pineLeaf;
    public Color[] pineColors;

    [Header("Lighting")]
    public Light2D globalLight;
    public Color morning;
    public Color noon;
    public Color peakDaylight;
    public Color preGoldenHour;
    public Color goldenHour;
    public Color sunset;
    public Color midnight;
    

    [Header("UI")]
    [SerializeField] TextMeshProUGUI uiPlayerClock;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Multiple Time Manager instances found.");
            Destroy(gameObject);
        }

        instance = this;

        
    }

    void Start()
    {
        hours = 6;
        _localMinutes = 00;
        period = "am";

        globalTime = hours * 60 + _localMinutes;
    }

    // Update is called once per frame
    void Update()
    {
        #region seasoncolors

        if (season == Season.WINTER) { mapleLeaf.SetFloat("_Alpha", 0); } else mapleLeaf.SetFloat("_Alpha", 1);

        if (season == Season.SPRING)
        {
            grassMat.color = grassColors[0];
            dirtMat.color = dirtColors[0];

            mapleLeaf.color = mapleColors[0];
            oakLeaf.color = oakColors[0];
            pineLeaf.color = pineColors[0];

        }
        else if(season == Season.SUMMER)
        {
            grassMat.color = grassColors[1];
            dirtMat.color = dirtColors[1];

            mapleLeaf.color = mapleColors[1];
            oakLeaf.color = oakColors[1];
            pineLeaf.color = pineColors[1];


        }
        else if(season == Season.FALL)
        {
            grassMat.color = grassColors[2];
            dirtMat.color = dirtColors[2];

            mapleLeaf.color = mapleColors[2];
            oakLeaf.color = oakColors[2];
            pineLeaf.color = pineColors[2];


        }
        else if (season == Season.WINTER)
        {
            grassMat.color = grassColors[3];
            dirtMat.color = dirtColors[3];

            mapleLeaf.color = mapleColors[3];
            oakLeaf.color = oakColors[3];
            pineLeaf.color = pineColors[3];


        }

        #endregion seasoncolors

        if (!Application.IsPlaying(this)) return;

        globalTime += Time.deltaTime;
        _localMinutes += Time.deltaTime;

        minutes = Mathf.RoundToInt(_localMinutes / 5) * 5;

        string clockMinutes;

        if(minutes >= 60)
        {
            hours += 1;
            _localMinutes = 0;
            minutes = 0;
        }

        if (minutes < 10) clockMinutes = "0" + minutes; else clockMinutes = minutes.ToString();

        if (hours == 12 && _periodSwitch == false)
        {
            if(period == "am")
            {
                period = "pm";
            }
            else
            {
                period = "am";
            }

            _periodSwitch = true;
        }

        if (hours != 12) _periodSwitch = false;

        if (hours == 13) hours = 1;

        uiPlayerClock.text = hours + ":" + clockMinutes + period;

        if (globalTime >= 360f && globalTime < 720f)
        {
            globalLight.color = Color.Lerp(morning, noon, (globalTime - 360f) / 360f);
        }
        if(globalTime >= 720f && globalTime < 960f)
        {
            globalLight.color = Color.Lerp(noon, peakDaylight, (globalTime - 720f) / 240f);
        }
        if (globalTime >= 960f && globalTime < 1140f)
        {
            globalLight.color = Color.Lerp(peakDaylight, preGoldenHour, (globalTime - 960f) / 180f);
        }
        if (globalTime >= 1140f && globalTime < 1200f)
        {
            globalLight.color = Color.Lerp(preGoldenHour, goldenHour, (globalTime - 1140f) / 60f);
        }
        if (globalTime >= 1200f && globalTime < 1230f)
        {
            globalLight.color = Color.Lerp(goldenHour, sunset, (globalTime - 1200f) / 30f);
        }
        if (globalTime >= 1230f && globalTime < 1440f)
        {
            globalLight.color = Color.Lerp(sunset, midnight, (globalTime - 1230f) / 210f);
        }

        if (globalTime >= 1440f) globalTime = 0;

        if (globalTime >= 0f && globalTime < 360f)
        {
            globalLight.color = Color.Lerp(midnight, morning, globalTime / 300f);
        }
    }

    public void AddHour()
    {
        globalTime += 60;
        hours += 1;
    }

    public void AddHalfHour()
    {
        globalTime += 30;
        _localMinutes += 30;

        float diff = _localMinutes - 60;

        if (_localMinutes >= 60)
        {
            hours += 1;
            _localMinutes = diff;
        }
    }
}
