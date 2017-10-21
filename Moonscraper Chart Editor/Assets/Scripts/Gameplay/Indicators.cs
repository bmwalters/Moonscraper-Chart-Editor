﻿// Copyright (c) 2016-2017 Alexander Ong
// See LICENSE in project root for license information.

#define GAMEPAD

using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Indicators : MonoBehaviour {
    const int FRET_COUNT = 6;

    [SerializeField]
    GameObject[] indicatorParents = new GameObject[FRET_COUNT];
    [SerializeField]
    GameObject[] indicators = new GameObject[FRET_COUNT];
    [SerializeField]
    GameObject[] customIndicators = new GameObject[FRET_COUNT];
    [SerializeField]
    Color[] defaultStikelineFretColors;
    [SerializeField]
    Color[] ghlStikelineFretColors;

    [HideInInspector]
    public HitAnimation[] animations;

    SpriteRenderer[] fretRenders;

    void Start()
    {
        animations = new HitAnimation[FRET_COUNT];
        fretRenders = new SpriteRenderer[FRET_COUNT * 2];

        for (int i = 0; i < animations.Length; ++i)
        {
            if (customIndicators[i].activeSelf)
            {
                animations[i] = customIndicators[i].GetComponent<HitAnimation>();
                indicators[i].transform.parent.gameObject.SetActive(false);
            }
            else
                animations[i] = indicators[i].GetComponent<HitAnimation>();
        }

        for (int i = 0; i < indicators.Length; ++i)
        {
            fretRenders[i * 2] = indicators[i].GetComponent<SpriteRenderer>();
            fretRenders[i * 2 + 1] = indicators[i].transform.parent.GetComponent<SpriteRenderer>();
        }

        UpdateStrikerColors();
        SetStrikerPlacement();

        MenuBar.OnChartReloadTriggerList.Add(UpdateStrikerColors);
        MenuBar.OnChartReloadTriggerList.Add(SetStrikerPlacement);
    }

    // Update is called once per frame
    void Update () {
        //UpdateStrikerColors();

        if (Globals.applicationMode == Globals.ApplicationMode.Playing && !Globals.bot)
        {
#if GAMEPAD
            if (GameplayManager.gamepad != null)
            {
                GamePadState gamepad = (GamePadState)GameplayManager.gamepad;

                if (gamepad.Buttons.A == ButtonState.Pressed)
                    animations[0].Press();
                else
                    animations[0].Release();

                if (gamepad.Buttons.B == ButtonState.Pressed)
                    animations[1].Press();
                else
                    animations[1].Release();

                if (gamepad.Buttons.Y == ButtonState.Pressed)
                    animations[2].Press();
                else
                    animations[2].Release();

                if (gamepad.Buttons.X == ButtonState.Pressed)
                    animations[3].Press();
                else
                    animations[3].Release();

                if (gamepad.Buttons.LeftShoulder == ButtonState.Pressed)
                    animations[4].Press();
                else
                    animations[4].Release();
            }
            else
            {
                // Keyboard controls
                for (int i = 0; i < 5; ++i)
                {
                    if (Input.GetKey((i + 1).ToString()))
                    {
                        animations[i].Press();
                    }
                    else if (!animations[i].running)
                        animations[i].Release();
                }
            }
#else

            for (int i = 0; i < 5; ++i)
            {
                if (Input.GetButton("Fret" + i))
                {
                    //indicators[i].SetActive(true);
                    animations[i].Press();
                }
                else if (!animations[i].running)
                    animations[i].Release();
                //indicators[i].SetActive(false);
            }
#endif
        }
        else
        {
            for (int i = 0; i < animations.Length; ++i)
            {
                if (!animations[i].running)
                    animations[i].Release();
            }
        }
    }

    public void UpdateStrikerColors()
    {
        if (Globals.drumMode)
        {
            for (int i = 0; i < defaultStikelineFretColors.Length; ++i)
            {
                int color = i + 1;
                if (color >= defaultStikelineFretColors.Length)
                    color = 0;

                fretRenders[i * 2].color = defaultStikelineFretColors[color];
                fretRenders[i * 2 + 1].color = defaultStikelineFretColors[color];
            }
        }
        else
        {
            Color[] colors = Globals.ghLiveMode ? ghlStikelineFretColors : defaultStikelineFretColors;
            for (int i = 0; i < colors.Length; ++i)
            {
                fretRenders[i * 2].color = colors[i];
                fretRenders[i * 2 + 1].color = colors[i];
            }
        }
    }

    public void SetStrikerPlacement()
    {
        int range = indicatorParents.Length;

        for (int i = 0; i < range; ++i)
        {
            int number = i;
            if (Globals.notePlacementMode == Globals.NotePlacementMode.LeftyFlip)
            {
                number = range - (number + 1);
            }

            float xPos = NoteController.CHART_CENTER_POS + number * NoteController.positionIncrementFactor + NoteController.noteObjectPositionStartOffset;
            indicatorParents[i].transform.position = new Vector3(xPos, indicatorParents[i].transform.position.y, indicatorParents[i].transform.position.z);

            indicatorParents[i].SetActive(xPos <= NoteController.CHART_CENTER_POS - NoteController.noteObjectPositionStartOffset);
        }
    }
}
