using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicBox : MonoBehaviour
{
    private static MusicBox instance;
    private AudioSource _audioSource;
    private bool silent;
    private bool activeCoroutine = false;

    [SerializeField] private AudioClip normal;
    [SerializeField] private AudioClip normal2;
    private AudioClip standard;
    [SerializeField] private AudioClip battle;


    float songTime;

    public static MusicBox Instance
    {
        get
        {
            /*if (instance == null)
            {
                DontDestroyOnLoad(managerHolder);
                //_instance = managerHolder.GetComponent<GameManager>();          
            }*/

            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        newSceneCheck();

    }

    private void Update()
    {
        if (!activeCoroutine)
        {
            if (!silent)
            {
                _audioSource.volume = GameManager.Instance.masterVolume * GameManager.Instance.musicVolume;
            }
            else
            {
                
                _audioSource.volume = 0f;
            }
        }
        
    }

    public void newSceneCheck()
    {
        if (wrongSceneCheck())
        {
            silent = true;
            _audioSource.volume = 0f;
            if (_audioSource.isPlaying)
                _audioSource.Stop();
        }
        else
        {
            silent = false;
            if (GameManager.Instance.towerfall)
            {
                standard = normal2;
            }
            else
            {
                standard = normal;
            }
            _audioSource.clip = standard;
            _audioSource.volume = GameManager.Instance.masterVolume * GameManager.Instance.musicVolume;
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }
    }

    public void StartBattleFade()
    {
        StopAllCoroutines();
        StartCoroutine(DoBattleBegin());
    }

    public void EndBattleFade()
    {
        StopAllCoroutines();
        StartCoroutine(DoBattlePeacefulEnd());
    }

    public void ReturnToNormal()
    {
        StopAllCoroutines();
        StartCoroutine(DoReturnToNormal());
    }

    public void HardStop()
    {
        silent = true;
    }

    private static bool wrongSceneCheck()
    {
        if(SceneManager.GetActiveScene().name == "23_MainMenu" || SceneManager.GetActiveScene().name == "24_Credits" || SceneManager.GetActiveScene().name == "25_GameOver")
        {
            return true;
        }
        if (SceneManager.GetActiveScene().name == "21_Heartsong" || SceneManager.GetActiveScene().name == "20_BeforeHeartsong" || SceneManager.GetActiveScene().name == "0_Exterior")
        {
            return true;
        }
        return false;
    }

    private IEnumerator DoBattlePeacefulEnd()
    {
        activeCoroutine = true;
        while (_audioSource.volume > 0f)
        {
            _audioSource.volume -= Time.deltaTime * 2f;
            yield return null;
        }

        _audioSource.clip = standard;
        _audioSource.time = songTime;
       
    }

    private IEnumerator DoReturnToNormal()
    {
        while (_audioSource.volume < GameManager.Instance.masterVolume * GameManager.Instance.musicVolume)
        {
            _audioSource.volume += Time.deltaTime / 3f;
            yield return null;
        }
        _audioSource.Play();
        _audioSource.time = songTime;
        activeCoroutine = false;
        yield return null;
    }

    private IEnumerator DoBattleBegin()
    {
        activeCoroutine = true;
        songTime = _audioSource.time;
        while (_audioSource.volume > 0f)
        {
            _audioSource.volume -= Time.deltaTime * 2f;
            yield return null;
        }

        _audioSource.clip = battle;
        yield return new WaitForSeconds(.5f);
        _audioSource.volume = GameManager.Instance.masterVolume * GameManager.Instance.musicVolume;
        _audioSource.Play();
        activeCoroutine = false; // Cuz then we go hard into the battle music
        yield return null;
    }
}
