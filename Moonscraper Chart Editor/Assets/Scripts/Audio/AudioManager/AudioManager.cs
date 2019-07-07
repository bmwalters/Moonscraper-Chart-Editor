﻿using System;
using System.Collections.Generic;
using Un4seen.Bass;
using Un4seen.Bass.Misc;

public static class AudioManager {
    public static bool isDisposed { get; private set; }
    static List<AudioStream> liveAudioStreams = new List<AudioStream>();
    static string encoderDirectory = string.Empty;

    #region Memory
    public static bool Init()
    {
        isDisposed = false;

        bool success = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        if (!success)
            UnityEngine.Debug.LogError("Failed Bass.Net initialisation");
        else
            UnityEngine.Debug.Log("Bass.Net initialised");

        encoderDirectory = Globals.realWorkingDirectory +
#if UNITY_EDITOR
        "/StreamingAssets/";
#elif UNITY_STANDALONE_WIN
        "/Moonscraper Chart Editor_Data/StreamingAssets/";
#else
        "/"; // TODO: fix for cross-platform
#endif
        // UnityEngine.Debug.Assert(System.IO.File.Exists(encoderDirectory + "oggenc2.exe"));

        return success;
    }

    public static void Dispose()
    {
        if (liveAudioStreams.Count > 0)
        {
            UnityEngine.Debug.LogWarning("Disposing of audio manager but there are still " + liveAudioStreams.Count + " streams remaining. Remaining streams will be cleaned up by the audio manager.");
        }

        // Free any remaining streams 
        for (int i = liveAudioStreams.Count - 1; i >= 0; --i)
        {
            FreeAudioStream(liveAudioStreams[i]);
        }

        UnityEngine.Debug.Assert(liveAudioStreams.Count == 0, "Failed to free " + liveAudioStreams.Count + " remaining audio streams");

        Bass.BASS_Free();
        UnityEngine.Debug.Log("Freed Bass Audio memory");
        isDisposed = true;
    }

    public static bool FreeAudioStream(AudioStream stream)
    {
        bool success = false;

        if (isDisposed)
        {
            UnityEngine.Debug.LogError("Trying to free a stream when Bass has not been initialised");
            return false;
        }

        if (StreamIsValid(stream))
        {
            if (stream.GetType() == typeof(OneShotSampleStream))
                success = Bass.BASS_SampleFree(stream.audioHandle);
            else
                success = Bass.BASS_StreamFree(stream.audioHandle);

            if (!success)
                UnityEngine.Debug.LogError("Error while freeing audio stream " + stream.audioHandle + ", Error Code " + Bass.BASS_ErrorGetCode());
            else
            {
                UnityEngine.Debug.Log("Successfully freed audio stream");
                if (!liveAudioStreams.Remove(stream))
                {
                    UnityEngine.Debug.LogError("Freed a stream, however it wasn't tracked by the audio manager?");
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Attempted to free an invalid audio stream");
        }

        return success;
    }

    public static void ConvertToOgg(string sourcePath, string destPath)
    {
        const string EXTENTION = ".ogg";
        UnityEngine.Debug.Assert(destPath.EndsWith(EXTENTION));

        if (sourcePath.EndsWith(EXTENTION))
        {
            // Re-encoding is slow as hell, speed this up
            System.IO.File.Copy(sourcePath, destPath);
        }
        else
        {
            // TODO: fix for cross-platform
            /*
            string inputFile = sourcePath;
            string outputFile = destPath;

            EncoderOGG encoder = new EncoderOGG(0);
            encoder.EncoderDirectory = encoderDirectory;

            if (!BaseEncoder.EncodeFile(inputFile, outputFile, encoder, null, true, false))
            {
                UnityEngine.Debug.LogErrorFormat("Unable to encode ogg file from {0} to {1}. Error {2}", sourcePath, destPath, Bass.BASS_ErrorGetCode().ToString());
            }
            */
        }
    }

#endregion

#region Stream Loading

    public static AudioStream LoadStream(string filepath)
    {
        int audioStreamHandle = Bass.BASS_StreamCreateFile(filepath, 0, 0, BASSFlag.BASS_STREAM_DECODE);

        var newStream = new AudioStream(audioStreamHandle);
        liveAudioStreams.Add(newStream);
        return newStream;
    }

    public static TempoStream LoadTempoStream(string filepath)
    {
        int audioStreamHandle = Bass.BASS_StreamCreateFile(filepath, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_ASYNCFILE | BASSFlag.BASS_STREAM_PRESCAN);
        audioStreamHandle = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_TempoCreate(audioStreamHandle, BASSFlag.BASS_FX_FREESOURCE);
     
        var newStream = new TempoStream(audioStreamHandle);
        liveAudioStreams.Add(newStream);
        return newStream;
    }

    public static OneShotSampleStream LoadSampleStream(string filepath, int maxSimultaneousPlaybacks)
    {
        UnityEngine.Debug.Assert(System.IO.File.Exists(filepath), "Filepath " + filepath + " does not exist");

        int audioStreamHandle = Bass.BASS_SampleLoad(filepath, 0, 0, maxSimultaneousPlaybacks, BASSFlag.BASS_DEFAULT);

        var newStream = new OneShotSampleStream(audioStreamHandle, maxSimultaneousPlaybacks);
        liveAudioStreams.Add(newStream);
        return newStream;
    }

    public static OneShotSampleStream LoadSampleStream(UnityEngine.AudioClip clip, int maxSimultaneousPlaybacks)
    {
        var newStream = LoadSampleStream(clip.GetWavBytes(), maxSimultaneousPlaybacks);

        return newStream;
    }

    public static OneShotSampleStream LoadSampleStream(byte[] streamBytes, int maxSimultaneousPlaybacks)
    {
        int audioStreamHandle = Bass.BASS_SampleLoad(streamBytes, 0, streamBytes.Length, maxSimultaneousPlaybacks, BASSFlag.BASS_DEFAULT);

        var newStream = new OneShotSampleStream(audioStreamHandle, maxSimultaneousPlaybacks);
        liveAudioStreams.Add(newStream);
        return newStream;
    }

    public static void RegisterStream(AudioStream stream)
    {
        liveAudioStreams.Add(stream);
    }

#endregion

#region Attributes

    public static float GetAttribute(AudioStream audioStream, AudioAttributes attribute)
    {
        float value = 0;
        Bass.BASS_ChannelGetAttribute(audioStream.audioHandle, (BASSAttribute)attribute, ref value);
        return value;
    }

    public static void SetAttribute(AudioStream audioStream, AudioAttributes attribute, float value)
    {
        Bass.BASS_ChannelSetAttribute(audioStream.audioHandle, (BASSAttribute)attribute, value);
    }

    public static float GetAttribute(TempoStream audioStream, TempoAudioAttributes attribute)
    {
        float value = 0;
        Bass.BASS_ChannelGetAttribute(audioStream.audioHandle, (BASSAttribute)attribute, ref value);
        return value;
    }

    public static void SetAttribute(TempoStream audioStream, TempoAudioAttributes attribute, float value)
    {
        Bass.BASS_ChannelSetAttribute(audioStream.audioHandle, (BASSAttribute)attribute, value);
    }

#endregion

#region Helper Functions

    public static bool StreamIsValid(AudioStream audioStream)
    {
        return audioStream != null && audioStream.isValid;
    }

#endregion

}
