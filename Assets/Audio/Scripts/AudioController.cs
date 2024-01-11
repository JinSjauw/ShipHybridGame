using System;
using System.Collections;
using Audio;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    //members

    public static AudioController Instance;

    [SerializeField] private bool debug;
    public AudioTrack[] tracks;
    
    private Hashtable m_AudioTable; //Relationship between audio types (key) and Audio tracks (value)
    private Hashtable m_JobTable; //Relationship between audio types (key) and jobs (value) (Coroutine, IEnumerator)

    [System.Serializable]
    public class AudioObject
    {
        public AudioID type;
        public AudioClip clip;
    }

    [System.Serializable]
    public class AudioTrack
    {
        public AudioSource source;
        public AudioObject[] audio;
    }
    
    private class AudioJob
    {
        public AudioAction action;
        public AudioID type;

        public bool fade;
        public float delay;
        
        public AudioJob(AudioAction _action, AudioID _type, bool _fade, float _delay)
        {
            action = _action;
            type = _type;
            fade = _fade;
            delay = _delay;
        }
    }
    
    private enum AudioAction
    {
        START,
        STOP,
        RESTART,
    }
    
    #region Unity Functions

        private void Awake()
        {
            if (!Instance)
            {
                Configure();
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

        #endregion
    
    #region Public Functions

    public void PlayAudio(AudioID _type, bool _fade = false, float delay = 0.0f)
    {
        AddJob(new AudioJob(AudioAction.START, _type, _fade, delay));
    }

    public void StopAudio(AudioID _type, bool _fade = false, float delay = 0.0f)
    {
        AddJob(new AudioJob(AudioAction.STOP, _type, _fade, delay));
    }

    public void RestartAudio(AudioID _type, bool _fade = false, float delay = 0.0f)
    {
        AddJob(new AudioJob(AudioAction.RESTART, _type, _fade, delay));
    }

    #endregion

    #region Private Functions

    private void Configure()
    {
        Instance = this;
        m_AudioTable = new Hashtable();
        m_JobTable = new Hashtable();
        GenerateAudioTable();
    }

    private void Dispose()
    {
        foreach (DictionaryEntry _entry in m_JobTable)
        {
            IEnumerator job = (IEnumerator)_entry.Value;
           StopCoroutine(job);
        }
    }

    private void GenerateAudioTable()
    {
        foreach (AudioTrack _track in tracks)
        {
            foreach (AudioObject _obj in _track.audio)
            {
                //Don't duplicate keys
                if (m_AudioTable.ContainsKey(_obj.type))
                {
                    LogWarning("Key has already been registered: [" + _obj.type + "]");
                }
                else
                {
                    m_AudioTable.Add(_obj.type, _track);
                    Log("Registered audio: [" + _obj.type + "]");
                }
            }
        }
    }
    
    private IEnumerator RunAudioJob(AudioJob _job)
    {
        yield return new WaitForSeconds(_job.delay);
        
        AudioTrack _track = (AudioTrack)m_AudioTable[_job.type];
        _track.source.clip = GetClipFromAudioTrack(_job.type, _track);
        
        switch (_job.action)
        {
            case AudioAction.START:
                _track.source.Play();
                break;
            case AudioAction.STOP:
                if (!_job.fade)
                {
                    _track.source.Stop();
                }
                break;
            case AudioAction.RESTART:
                _track.source.Stop();
                _track.source.Play();
                break;
        }

        if (_job.fade)
        {
            float _initial = _job.action == AudioAction.START || _job.action == AudioAction.RESTART ? 0.0f : 1.0f;
            float _target = _initial == 0 ? 1 : 0;
            float _duration = 1.0f;
            float _timer = 0.0f;

            while (_timer <= _duration)
            {
                _track.source.volume = Mathf.Lerp(_initial, _target, _timer / _duration);
                _timer += Time.deltaTime;
                yield return null;
            }

            if (_job.action == AudioAction.STOP)
            {
                _track.source.Stop();
            }
        }

        m_JobTable.Remove(_job.type);
        Log("Job Count: " + m_JobTable.Count);

        yield return null;
    }


    private void AddJob(AudioJob _job)
    {
        //Remove conflicting jobs
        RemoveConflictingJobs(_job.type);
        
        //Start Job
        IEnumerator _jobRunner = RunAudioJob(_job);
        m_JobTable.Add(_job.type, _jobRunner);
        StartCoroutine(_jobRunner);
        Log("Starting job on: [" + _job.type + "] with operation " + _job.action);
    }
    
    private void RemoveJob(AudioID _type)
    {
        if (!m_JobTable.ContainsKey(_type))
        {
            LogWarning("Trying to stop a job thats not running [Type:] " + _type);
            return;
        }

        IEnumerator _runningJob = (IEnumerator)m_JobTable[_type];
        StopCoroutine(_runningJob);
        m_JobTable.Remove(_type);
    }

    private AudioClip GetClipFromAudioTrack(AudioID _type, AudioTrack _track)
    {
        foreach (AudioObject _obj in _track.audio)
        {
            if (_obj.type == _type)
            {
                return _obj.clip;
            }
        }

        return null;
    }
    
    private void RemoveConflictingJobs(AudioID _type)
    {
        if (m_JobTable.ContainsKey(_type))
        {
            RemoveJob(_type);
        }

        AudioID _conflictAudio = AudioID.NONE;
        foreach (DictionaryEntry _entry in m_JobTable)
        {
            AudioID _audioID = (AudioID)_entry.Key;
            AudioTrack _trackInUse = (AudioTrack)m_AudioTable[_audioID];
            AudioTrack _trackNeeded = (AudioTrack)m_AudioTable[_type];
            if (_trackNeeded == _trackInUse)
            {
                _conflictAudio = _audioID;
                LogWarning("Audio Conflict: [" + _audioID + "]" + " On Track: [" + _trackInUse + "]");
            }
        }

        if (_conflictAudio != AudioID.NONE)
        {
            RemoveJob(_conflictAudio);
        }
    }

    private void Log(string _msg)
    {
        if (!debug) return;
        Debug.Log("[Audio Controller]: " + _msg);
    }
    
    private void LogWarning(string _msg)
    {
        if (!debug) return;
        Debug.Log("[Audio Controller][Warning]: " + _msg);
    }
    
    #endregion
}

