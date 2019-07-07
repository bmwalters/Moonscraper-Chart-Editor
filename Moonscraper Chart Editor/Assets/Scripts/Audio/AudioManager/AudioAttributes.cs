using Un4seen.Bass;

public enum AudioAttributes
{
    Volume = BASSAttribute.BASS_ATTRIB_VOL,
    Pan = BASSAttribute.BASS_ATTRIB_PAN,
}

public enum TempoAudioAttributes
{
    Frequency = BASSAttribute.BASS_ATTRIB_FREQ,
    Tempo = BASSAttribute.BASS_ATTRIB_TEMPO,
    TempoPitch = BASSAttribute.BASS_ATTRIB_TEMPO_PITCH,
}
