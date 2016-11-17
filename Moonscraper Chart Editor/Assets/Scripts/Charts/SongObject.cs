﻿using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public abstract class SongObject
{
    public Song song;
    public uint position;
    public SongObjectController controller;

    public SongObject (Song _song, uint _position)
    {
        song = _song;
        position = _position;
    }
    
    public float worldYPosition { get { return song.ChartPositionToWorldYPosition(position); } }

    public float time { get { return song.ChartPositionToTime(position); } }

    public abstract string GetSaveString();
    
    public static bool operator ==(SongObject a, SongObject b)
    {
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            else if (!ReferenceEquals(a, null) && !ReferenceEquals(b, null))
                return true;
            else
                return false;
        }
        else
            return a.Equals(b);
    }

    protected virtual bool Equals(SongObject b)
    {
        if (position == b.position)
            return true;
        else
            return false;
    }

    public static bool operator !=(SongObject a, SongObject b)
    {
        return !(a == b);
    }

    protected virtual bool LessThan(SongObject b)
    {
        if (position < b.position)
            return true;
        else
            return false;
    }

    public static bool operator <(SongObject a, SongObject b)
    {
        return a.LessThan(b);
    }

    public static bool operator >(SongObject a, SongObject b)
    {
        if (a != b)
            return !(a < b);
        else
            return false;
    }

    public static int FindClosestPosition<T>(T searchItem, T[] objects) where T : SongObject
    {
        int lowerBound = 0;
        int upperBound = objects.Length - 1;
        int index = Globals.NOTFOUND;

        int midPoint = Globals.NOTFOUND;

        while (lowerBound <= upperBound)
        {
            midPoint = (lowerBound + upperBound) / 2;
            index = midPoint;

            if (objects[midPoint] == searchItem)
            {
                break;
            }
            else
            {
                if (objects[midPoint] < searchItem)
                {
                    // data is in upper half
                    lowerBound = midPoint + 1;
                }
                else
                {
                    // data is in lower half 
                    upperBound = midPoint - 1;
                }
            }
        }

        return index;
    }

    public static int FindClosestPosition<T>(uint position, T[] objects) where T : SongObject
    {
        int lowerBound = 0;
        int upperBound = objects.Length - 1;
        int index = Globals.NOTFOUND;

        int midPoint = Globals.NOTFOUND;

        while (lowerBound <= upperBound)
        {
            midPoint = (lowerBound + upperBound) / 2;
            index = midPoint;

            if (objects[midPoint].position == position)
            {
                break;
            }
            else
            {
                if (objects[midPoint].position < position)
                {
                    // data is in upper half
                    lowerBound = midPoint + 1;
                }
                else
                {
                    // data is in lower half 
                    upperBound = midPoint - 1;
                }
            }
        }

        return index;
    }

    public static T[] FindObjectsAtPosition<T>(uint position, T[] objects) where T : SongObject
    {
        int index = FindClosestPosition(position, objects);

        if (index != Globals.NOTFOUND && objects[index].position == position)
        {
            int lowRange = index, highRange = index;

            while (lowRange > 0 && objects[index].position == objects[lowRange - 1].position)
            {
                --lowRange;
            }

            while (highRange < objects.Length - 1 && objects[index].position == objects[highRange + 1].position)
            {
                ++highRange;
            }

            int length = highRange - lowRange + 1;
            
            T[] objectSelection = new T[length];
            System.Array.Copy(objects, lowRange, objectSelection, 0, length);
            
            return objectSelection;
        }
        else
            return new T[0];
    }

    public static int FindObjectPosition<T>(T searchItem, T[] objects) where T : SongObject
    {
        int pos = FindClosestPosition(searchItem, objects);

        if (pos != Globals.NOTFOUND && objects[pos] != searchItem)
        {
            pos = Globals.NOTFOUND;
        }

        return pos;
    }

    static int FindPreviousPosition<T>(System.Type type, int startPosition, T[] list) where T : SongObject
    {
        // Linear search
        if (startPosition < 0 || startPosition > list.Length - 1)
            return Globals.NOTFOUND;
        else
        {
            --startPosition;

            while (startPosition >= 0)
            {
                if (list[startPosition].GetType() == type)
                    return startPosition;
                --startPosition;
            }

            return Globals.NOTFOUND;
        }
    }

    public static T FindPreviousOfType<T>(System.Type type, int startPosition, T[] list) where T : SongObject
    {
        int pos = FindPreviousPosition(type, startPosition, list);
        if (pos == Globals.NOTFOUND)
            return null;
        else
            return list[pos];
    }

    static int FindNextPosition<T>(System.Type type, int startPosition, T[] list) where T : SongObject
    {
        // Linear search
        if (startPosition < 0 || startPosition > list.Length - 1)
            return Globals.NOTFOUND;
        else
        {
            ++startPosition;

            while (startPosition < list.Length)
            {
                if (list[startPosition].GetType() == type)
                    return startPosition;
                ++startPosition;
            }

            return Globals.NOTFOUND;
        }
    }

    public static T FindNextOfType<T>(System.Type type, int startPosition, T[] list) where T : SongObject
    {
        int pos = FindNextPosition(type, startPosition, list);
        if (pos == Globals.NOTFOUND)
            return null;
        else
            return list[pos];
    }

    public static int Insert<T>(T item, List<T> list) where T : SongObject
    {       
        int insertionPos = FindClosestPosition(item, list.ToArray());   

        // Needs to overwrite
        if (list.Count > 0 && insertionPos != Globals.NOTFOUND)// && list[insertionPos] != item)
        {
            int prevPosition = FindPreviousPosition(item.GetType(), insertionPos, list.ToArray());
            int nextPosition = FindNextPosition(item.GetType(), insertionPos, list.ToArray());

            if (prevPosition != Globals.NOTFOUND && list[prevPosition] == item)
            {
                // Overwrite
                list[prevPosition] = item;
                insertionPos = prevPosition;       
            }
            else if (nextPosition != Globals.NOTFOUND && list[nextPosition] == item)
            {
                // Overwrite
                list[nextPosition] = item;
                insertionPos = nextPosition;
            }
            else if (item == list[insertionPos] && item.GetType() == list[insertionPos].GetType())
            {
                // Overwrite 
                list[insertionPos] = item;
            }
            // Insert into sorted position
            else
            {   
                if (item > list[insertionPos])
                {
                    ++insertionPos;
                }
                list.Insert(insertionPos, item);
            }
        }
        else
        {
            // Adding the first note
            list.Add(item);
            insertionPos = list.Count - 1;
        }

        if (item.GetType() == typeof(Note))
        {
            // Update linked list
            Note current = list[insertionPos] as Note;
            Note previous = FindPreviousOfType(typeof(Note), insertionPos, list.ToArray()) as Note;
            Note next = FindNextOfType(typeof(Note), insertionPos, list.ToArray()) as Note;

            current.previous = previous;
            if (previous != null)
                previous.next = current;

            current.next = next;
            if (next != null)
                next.previous = current;
        }

        return insertionPos;
    }

    public static bool Remove<T>(T item, List<T> list) where T : SongObject
    {
        int pos = FindObjectPosition(item, list.ToArray());

        if (pos != Globals.NOTFOUND)
        {
            if (item.GetType() == typeof(Note))
            {
                // Update linked list
                Note previous = FindPreviousOfType(item.GetType(), pos, list.ToArray()) as Note;
                Note next = FindNextOfType(item.GetType(), pos, list.ToArray()) as Note;

                if (previous != null)
                    previous.next = next;
                if (next != null)
                    next.previous = previous;
            }

            item.song = null;
            list.RemoveAt(pos);

            return true;
        }

        return false;
    }
}

public class Event : SongObject
{
    public string title;

    public Event(Song song, string _title, uint _position) : base(song, _position)
    {
        title = _title;
    }

    public override string GetSaveString()
    {
        return Globals.TABSPACE + position + " = E \"" + title + "\"\n";
    }

    public static bool regexMatch(string line)
    {
        return new Regex(@"\d+ = E " + @"""[^""\\]*(?:\\.[^""\\]*)*""").IsMatch(line);
    }
}

public class Section : Event
{
    SectionController _controller = null;
    
    new public SectionController controller
    {
        get { return _controller; }
        set { _controller = value; base.controller = value; }
    }

    public Section(Song song, string _title, uint _position) : base(song, _title, _position) { }

    public override string GetSaveString()
    {
        return Globals.TABSPACE + position + " = E \"section " + title + "\"\n";
    }

    new public static bool regexMatch(string line)
    {
        return new Regex(@"\d+ = E " + @"""section [^""\\]*(?:\\.[^""\\]*)*""").IsMatch(line);
    }
}

public abstract class SyncTrack : SongObject
{
    public uint value;

    public SyncTrack (Song song, uint _position, uint _value) : base (song, _position)
    {
        value = _value;
    }
}

public class TimeSignature : SyncTrack
{
    public TimeSignature(Song song, uint _position = 0, uint _value = 4) : base (song, _position, _value) {}

    override public string GetSaveString()
    {
        //0 = TS 4
        return Globals.TABSPACE + position + " = TS " + value + "\n";
    }

    public static bool regexMatch(string line)
    {
        return new Regex(@"\d+ = TS \d+").IsMatch(line);
    }
}

public class BPM : SyncTrack
{
    public BPM(Song song, uint _position = 0, uint _value = 120000) : base (song, _position, _value) { }

    override public string GetSaveString()
    {
        //0 = B 140000
        return Globals.TABSPACE + position + " = B " + value + "\n";
    }

    public static bool regexMatch(string line)
    {
        return new Regex(@"\d+ = B \d+").IsMatch(line);
    }
}