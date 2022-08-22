using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource sfxPlayer;
    public AudioSource bgmPlayer;

    public float masterVolumeSfx = 1f;
    public float masterVolumeBgm = 1f;

    [SerializeField]
    private AudioClip TitleAudioClip;
    [SerializeField]
    private AudioClip TutorialAudioClip;
    [SerializeField]
    private AudioClip PlayerHomeAudioClip;
    [SerializeField]
    private AudioClip StageAudioClip;
    [SerializeField]
    private AudioClip Stage3AudioClip;


    [SerializeField]
    private AudioClip[] sfxAudioClips;      //기본 효과음 오디오 클립

    [SerializeField]
    private AudioClip[] playerAudioClips;      //플레이어 오디오 클립

    [SerializeField]
    private AudioClip[] enemyAudioClips;      //적 오디오 클립


    Dictionary<string, AudioClip> sfxClipsDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> playerClipsDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> enemyClipsDic = new Dictionary<string, AudioClip>();



    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        for(int i=0; i<sfxAudioClips.Length; i++)   //기본 효과음 오디오 저장
        {
            sfxClipsDic.Add(sfxAudioClips[i].name,sfxAudioClips[i]);
        }
        for(int i=0; i<playerAudioClips.Length; i++)    //플레이어 효과음 오디오 저장
        {
            playerClipsDic.Add(playerAudioClips[i].name,playerAudioClips[i]);
        }
        for(int i=0; i<enemyAudioClips.Length; i++)     //적 효과음 오디오 저장
        {
            enemyClipsDic.Add(enemyAudioClips[i].name,enemyAudioClips[i]);
        }

    }


    //---------사운드 재생 함수-----------
    public void SfxSound(string name, float volume = 0.7f)
    {
        if(sfxClipsDic.ContainsKey(name) == false)
        {
            Debug.Log(name + "의 오디오가 없습니다");
            return;
        }
        if(sfxPlayer.isPlaying == false)
            sfxPlayer.PlayOneShot(sfxClipsDic[name], volume * masterVolumeSfx);
    }

    public void PlayerSfxSound(AudioSource audiosource, string name, float volume = 0.7f)
    {
        if(playerClipsDic.ContainsKey(name) == false)
        {
            Debug.Log(name + "의 오디오가 없습니다");
            return;
        }
        if(sfxPlayer.isPlaying == false)
            audiosource.PlayOneShot(playerClipsDic[name], volume * masterVolumeSfx);
    }
    public void EnemySfxSound(AudioSource audiosource, string name, float volume = 0.7f)
    {
        if(enemyClipsDic.ContainsKey(name) == false)
        {
            Debug.Log(name + "의 오디오가 없습니다");
            return;
        }
        if(sfxPlayer.isPlaying == false)
            audiosource.PlayOneShot(enemyClipsDic[name], volume * masterVolumeSfx);
    }  

    public void BgmSound(float volum = 0.7f)
    {
        bgmPlayer.loop = true;
        bgmPlayer.volume = volum * masterVolumeBgm;

        if(SceneManager.GetActiveScene().name == "TitleScene")
        {
            bgmPlayer.clip = TitleAudioClip;
            bgmPlayer.Play();
        }
        else if(SceneManager.GetActiveScene().name == "TutorialScene")
        {
            bgmPlayer.clip = TutorialAudioClip;
            bgmPlayer.Play();
        }
        else if(SceneManager.GetActiveScene().name == "PlayerHome")
        {
            bgmPlayer.clip = PlayerHomeAudioClip;
            bgmPlayer.Play();
        }
        else if(SceneManager.GetActiveScene().name == "Stage1" || SceneManager.GetActiveScene().name == "Stage2")
        {
            bgmPlayer.clip = StageAudioClip;
            bgmPlayer.Play();
        }
        else if(SceneManager.GetActiveScene().name == "Stage3")
        {
            bgmPlayer.clip = Stage3AudioClip;
            bgmPlayer.Play();
        }

    }

}
