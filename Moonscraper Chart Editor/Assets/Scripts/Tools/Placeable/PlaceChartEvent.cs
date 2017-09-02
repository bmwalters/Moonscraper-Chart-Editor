﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceChartEvent : PlaceSongObject
{
    public ChartEvent chartEvent { get { return (ChartEvent)songObject; } set { songObject = value; } }
    new public ChartEventController controller { get { return (ChartEventController)base.controller; } set { base.controller = value; } }

    protected override void SetSongObjectAndController()
    {
        chartEvent = new ChartEvent(0, "Default");

        controller = GetComponent<ChartEventController>();
        controller.chartEvent = chartEvent;
    }

    protected override void Update()
    {
        base.Update();
        chartEvent.chart = editor.currentChart;
    }

    protected override void AddObject()
    {
        ActionHistory.Add action;
        string debugMessage = string.Empty;
       
        // Add chart event
        ChartEvent chartEvent = new ChartEvent(this.chartEvent);
        editor.currentChart.Add(chartEvent);

        action = new ActionHistory.Add(chartEvent);

        debugMessage = "Added Chart Event \"";

        debugMessage += chartEvent.eventName + "\"";

        Debug.Log(debugMessage);

        editor.actionHistory.Insert(action);
        editor.currentSelectedObject = chartEvent;
    }

    public static void AddObjectToCurrentChart(ChartEvent chartEvent, ChartEditor editor, bool update = true)
    {
        ChartEvent eventToAdd = new ChartEvent(chartEvent);

        editor.currentChart.Add(eventToAdd, update);
        editor.currentSelectedObject = eventToAdd;
    }

    protected override void Controls()
    {
        if (!Globals.lockToStrikeline)
        {
            if (Toolpane.currentTool == Toolpane.Tools.SongEvent && Globals.applicationMode == Globals.ApplicationMode.Editor && Input.GetMouseButtonDown(0))
            {
                int pos = SongObject.FindObjectPosition(chartEvent, editor.currentChart.events);
                if (pos == SongObject.NOTFOUND)
                {
                    //RecordAddActionHistory(chartEvent, editor.currentChart.events);

                    AddObject();
                }
                // Link to the event already in
                else
                    editor.currentSelectedObject = editor.currentChart.events[pos];
            }
        }
        else if (Input.GetButtonDown("Add Object"))
        {
            SongObject[] searchArray = editor.currentChart.events;
            int pos = SongObject.FindObjectPosition(chartEvent, searchArray);
            if (pos == SongObject.NOTFOUND)
            {
                editor.actionHistory.Insert(new ActionHistory.Add(chartEvent));
                AddObject();
            }
            else
            {
                editor.actionHistory.Insert(new ActionHistory.Delete(searchArray[pos]));
                searchArray[pos].Delete();
                editor.currentSelectedObject = null;
            }
        }
    }
}