﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores all collider information for group select collision detection
public class PrefabGlobals : MonoBehaviour {
    ChartEditor editor;

    static Vector2 noteColliderSize, spColliderSize, chartEventColliderSize, bpmColliderSize, tsColliderSize, sectionColliderSize, eventColliderSize;

    Vector2 GetColliderSize(GameObject gameObject)
    {
        Vector2 size;

        GameObject copy = Instantiate(gameObject);
        copy.SetActive(true);
        Collider col3d = copy.GetComponent<Collider>();
        Collider2D col2d = copy.GetComponent<Collider2D>();

        /************ Note Unity documentation- ************/
        // Bounds: The world space bounding volume of the collider.
        // Note that this will be an empty bounding box if the collider is disabled or the game object is inactive.
        if (col3d)
            size = col3d.bounds.size;
        else if (col2d)
            size = col2d.bounds.size;
        else
            size = Vector2.zero;

        Destroy(copy);
        return size;
    }

    // Use this for initialization
    void Awake () {
        editor = GameObject.FindGameObjectWithTag("Editor").GetComponent<ChartEditor>();

        // Collect prefab collider sizes
        noteColliderSize = GetColliderSize(editor.notePrefab);
        spColliderSize = GetColliderSize(editor.starpowerPrefab);
        chartEventColliderSize = GetColliderSize(editor.chartEventPrefab);
        bpmColliderSize = GetColliderSize(editor.bpmPrefab);
        tsColliderSize = GetColliderSize(editor.tsPrefab);
        sectionColliderSize = GetColliderSize(editor.sectionPrefab);
        eventColliderSize = GetColliderSize(editor.songEventPrefab);
    }

    public static Rect GetCollisionRect(SongObject songObject, float posOfChart = 0, float offset = 0)
    {
        Vector2 colliderSize;
        Vector2 position;

        switch ((SongObject.ID)songObject.classID)
        {
            case (SongObject.ID.Note):
                colliderSize = noteColliderSize;
                if (((Note)songObject).fret_type == Note.Fret_Type.OPEN)
                    colliderSize.x = NoteController.OPEN_NOTE_COLLIDER_WIDTH;
                position = new Vector2(NoteController.noteToXPos((Note)songObject), 0);
                break;
            case (SongObject.ID.Starpower):
                colliderSize = spColliderSize;
                position = new Vector2(StarpowerController.position, 0);
                break;
            case (SongObject.ID.ChartEvent):
                colliderSize = chartEventColliderSize;
                position = new Vector2(ChartEventController.position, 0);
                break;
            case (SongObject.ID.BPM):
                colliderSize = bpmColliderSize;
                position = new Vector2(BPMController.position, 0);
                break;
            case (SongObject.ID.TimeSignature):
                colliderSize = tsColliderSize;
                position = new Vector2(TimesignatureController.position, 0);
                break;
            case (SongObject.ID.Section):
                colliderSize = sectionColliderSize;
                position = new Vector2(SectionController.position, 0);
                break;
            case (SongObject.ID.Event):
                colliderSize = eventColliderSize;
                position = new Vector2(EventController.position, 0);
                break;
            default:
                return new Rect();
        }

        Vector2 min = new Vector2(position.x + posOfChart + offset - colliderSize.x / 2, position.y - colliderSize.y / 2);
        return new Rect(min, noteColliderSize);
    }

    public static bool HorizontalCollisionCheck(Rect rectA, Rect rectB)
    {
        if (rectA.width == 0 || rectB.width == 0)
            return false;

        // AABB, check for any gaps
        if (rectA.x <= rectB.x + rectB.width &&
               rectA.x + rectA.width >= rectB.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
