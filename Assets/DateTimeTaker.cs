using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DateTimeTaker : MonoBehaviour
{
    public Text text;
    string YearStr, MonthStr, DayStr, WeekStr, HourStr, MinuteStr, SecondStr, MilliStr; //各数値の表示
    float Year, Month, Day, Week, Hour, Minute, Second, Milli; //各数値 

    public GameObject MilliHand; //T=1s
    public GameObject SecondHand; //T=1min
    public GameObject MinuteHand; //T=1h
    public GameObject HourHand; //T=1d
    public GameObject WeekHand; //T=1w
    public GameObject DayHand; //T=1mon
    public GameObject MonthHand; //T=1y

    public float[] MonthDays; //各月の日数
    public float YearDay; //1年の日数

    public float PastDays; //今年の経過日数

    public GameObject[] MonthChars; //閏年かそうでないかで使う月の文字盤が違う

    public GameObject[] MonthDials; //月ごとに異なる日数の文字盤


    public float[] positionChanger = new float[10]; 
    public GameObject[] all = new GameObject[10];


    public Toggle[] timeToggles = new Toggle[7];
    public GameObject[] Hands = new GameObject[7];
    public GameObject[] Dials = new GameObject[10];
    void Start()
    {
        MonthDialsManager();
        DayDialsManager();

        for (int i = 0; i < 10; i++)
        {
            int j = all[i].transform.childCount;
            for(int k = 0; k < j; k++)
            {
                all[i].transform.GetChild(k).gameObject.transform.GetChild(0).transform.localPosition = new Vector3(0, positionChanger[i], 0);
            }
        }
    }
    void Update()
    {
        if (Year < System.DateTime.Now.Year) 
        {
            MonthDialsManager();
        }
        if (Month < System.DateTime.Now.Month)
        {
            DayDialsManager();
        }

        Year = System.DateTime.Now.Year;
        Month = System.DateTime.Now.Month;
        Week = (float)System.DateTime.Now.DayOfWeek;
        Day = System.DateTime.Now.Day;
        Hour = System.DateTime.Now.Hour;
        Minute = System.DateTime.Now.Minute;
        Second = System.DateTime.Now.Second;
        Milli = System.DateTime.Now.Millisecond;
        PastDays = System.DateTime.Now.DayOfYear;

        YearStr = Year + "年";
        MonthStr = Month + "月";
        DayStr = Day + "日";
        HourStr = Hour + "時";
        MinuteStr = Minute + "分";
        SecondStr = Second + "秒";

        if (Milli < 100) //ミリ秒が100未満の時の表示崩れ解消
        {
            MilliStr = "0" + Milli + "ミリ秒";
            if (Milli < 10)
            {
                MilliStr = "00" + Milli + "ミリ秒";
            }
        }
        else
        {
            MilliStr = Milli + "ミリ秒";
        }
    
        switch (Week)
        {
            case 0:
                WeekStr = "日";
                break;
            case 1:
                WeekStr = "月";
                break;
            case 2:
                WeekStr = "火";
                break;
            case 3:
                WeekStr = "水";
                break;
            case 4:
                WeekStr = "木";
                break;
            case 5:
                WeekStr = "金";
                break;
            case 6:
                WeekStr = "土";
                break;
        }
        WeekStr += "曜日\n";



        text.text = YearStr + MonthStr + DayStr + WeekStr + HourStr + MinuteStr + SecondStr + MilliStr; //時刻表示


        //時計盤表示
        MilliHand.transform.rotation = Quaternion.Euler(0, 0, -Milli * 360 / 1000);
        SecondHand.transform.rotation = Quaternion.Euler(0, 0, -Second * 6 - Milli * 6 / 1000);
        MinuteHand.transform.rotation = Quaternion.Euler(0, 0, -Minute * 6 - Second / 10 - Milli / 10000);
        HourHand.transform.rotation = Quaternion.Euler(0, 0, -Hour * 15 - Minute / 4 - Second / 240 - Milli / 240000);
        WeekHand.transform.rotation = Quaternion.Euler(0, 0, -Week * 360 / 7 - Hour * 15 / 7 - Minute / 28 - Second / 1680 - Milli / 1680000);
        DayHand.transform.rotation = Quaternion.Euler(0, 0, -(Day - 1) * (360 / MonthDays[(byte)Month - 1]) - Hour * 15 / MonthDays[(byte)Month - 1] - Minute / 4 / MonthDays[(byte)Month - 1] - Second / 240 / MonthDays[(byte)Month - 1] - Milli / 240000 / MonthDays[(byte)Month - 1]);
        MonthHand.transform.rotation = Quaternion.Euler(0, 0, -PastDays * (360 / YearDay) - Hour * 15 / YearDay - Minute / 4 / YearDay - Second / 240 / YearDay - Milli / 240000 / YearDay);
    }

    public void ToggleManager()
    {
        for(int i = 0; i < 10; i++)
        {
            Dials[i].SetActive(true);
            MonthDialsManager();
            DayDialsManager();
        }

        for(int i = 0; i < 7; i++)
        {
            Hands[i].SetActive(true);
            if (!timeToggles[i].isOn)
            {
                Hands[i].SetActive(false);
                switch (i)
                {
                    case 0:
                        Dials[0].SetActive(false);
                        Dials[1].SetActive(false);
                        continue;

                    case 1:
                        Dials[2].SetActive(false);
                        Dials[3].SetActive(false);
                        Dials[4].SetActive(false);
                        Dials[5].SetActive(false);
                        continue;

                    case 2:
                        Dials[6].SetActive(false);
                        continue;

                    case 3:
                        Dials[7].SetActive(false);
                        continue;

                    case 6:
                        Dials[9].SetActive(false);
                        break;
                }
            }
        }

        if(!timeToggles[4].isOn && !timeToggles[5].isOn)
        {
            Dials[8].SetActive(false);
        }
    }

    void MonthDialsManager()
    {
        Year = System.DateTime.Now.Year;
        //月の文字盤に関して
        if (Year % 4 == 0) //閏年の場合
        {
            MonthDays[1] = 29;
            YearDay = 366;
            MonthChars[0].SetActive(true);
            MonthChars[1].SetActive(false);
        }
        else //そうでない場合
        {
            MonthDays[1] = 28;
            YearDay = 365;
            MonthChars[0].SetActive(false);
            MonthChars[1].SetActive(true);
        }
    }

    void DayDialsManager()
    {
        Month = System.DateTime.Now.Month;
        Year = System.DateTime.Now.Year;
        //日の文字盤に関して
        if (Month == 1 || Month == 3 || Month == 5 || Month == 7 || Month == 8 || Month == 10 || Month == 12) //ひと月31日の場合
        {
            MonthDials[0].SetActive(true);
            MonthDials[1].SetActive(false);
            MonthDials[2].SetActive(false);
            MonthDials[3].SetActive(false);
        }
        else if (Month == 4 || Month == 6 || Month == 9 || Month == 11) //ひと月30日の場合
        {
            MonthDials[0].SetActive(false);
            MonthDials[1].SetActive(true);
            MonthDials[2].SetActive(false);
            MonthDials[3].SetActive(false);
        }
        else if (Month == 2 && Year % 4 != 0) // ひと月28日の場合
        {
            MonthDials[0].SetActive(false);
            MonthDials[1].SetActive(false);
            MonthDials[2].SetActive(true);
            MonthDials[3].SetActive(false);
        }
        else if (Month == 2 && Year % 4 == 0) //ひと月29日の場合
        {
            MonthDials[0].SetActive(false);
            MonthDials[1].SetActive(false);
            MonthDials[2].SetActive(false);
            MonthDials[3].SetActive(true);
        }
    }
}
