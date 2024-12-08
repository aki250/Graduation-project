using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //单例模式：确保只有一个实例
    public static AudioManager instance;

    public AudioSource[] sfx;    //用于存放音效SFX和BGM的数组
    [SerializeField] private AudioSource[] bgm; //背景音乐源

    [Space]
    [SerializeField] private float sfxMinHearableDistance; //音效可听的最小距离

    public bool playingBGM; //是否正在播放背景音乐
    private int bgmIndex; //当前播放的背景音乐索引

    private bool canPlaySFX = false; //是否可以播放音效

    private void Awake()
    {
        // 如果实例为空，则设置为当前实例；如果实例已经存在，销毁当前对象，确保只有一个 AudioManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //延迟0.2秒后允许播放音效
        Invoke("AllowPlayingSFX", 0.2f);
    }

    private void Update()
    {
        //如果没有播放背景音乐，则停止所有背景音乐
        if (!playingBGM)
        {
            StopAllBGM();
        }
        else
        {
            //如果当前背景音乐没有播放，则开始播放
            if (bgm[bgmIndex].isPlaying == false)
            {
                PlayBGM(bgmIndex);
            }
        }
    }

    //播放指定音效
    public void PlaySFX(int _sfxIndex, Transform _sfxSourceTransform)
    {
        //如果不允许播放音效，直接返回
        if (!canPlaySFX)
        {
            return;
        }

        //防止重复播放相同的音效，避免出现脚步音效被重叠播放的情况
        if (sfx[_sfxIndex].isPlaying == true)
        {
            return;
        }

        //如果音效源距离玩家太远，则不播放该音效
        if (_sfxSourceTransform != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _sfxSourceTransform.position) > sfxMinHearableDistance)
        {
            return;
        }

        //播放指定索引的音效
        if (_sfxIndex < sfx.Length)
        {
            //随机调整音效的音调
            sfx[_sfxIndex].pitch = Random.Range(0.85f, 1.15f);
            sfx[_sfxIndex].Play(); //播放音效
        }
    }

    //停止播放指定的音效
    public void StopSFX(int _sfxIndex)
    {
        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].Stop(); //停止指定索引的音效
        }
    }

    //逐渐降低音量，直到音效停止
    public IEnumerator DecreaseVolumeGradually(AudioSource _audio)
    {
        float defaultVolume = _audio.volume; //记录音频的初始音量

        //当音量大于0.1时，继续逐步降低音量
        while (_audio.volume > 0.1f)
        {
            //每0.25秒降低音量的 20%
            _audio.volume -= _audio.volume * 0.2f;

            yield return new WaitForSeconds(0.25f);

            //如果音量降0.1以下，停止音效并恢复音量
            if (_audio.volume <= 0.1f)
            {
                _audio.Stop(); //停止音效
                _audio.volume = defaultVolume; //恢复初始音量
                break;
            }
        }
    }

    //播放指定索引的背景音乐
    public void PlayBGM(int _bgmIndex)
    {
        //停止所有背景音乐，避免播放多个背景音乐
        StopAllBGM();

        //播放指定索引的背景音乐
        if (_bgmIndex < bgm.Length)
        {
            bgmIndex = _bgmIndex; //设置当前背景音乐的索引
            bgm[bgmIndex].Play(); //播放该背景音乐
        }
        else
        {
            Debug.Log("BGM超出范围"); 
        }
    }

    //播放随机背景音乐
    public void PlayRandomBGM()
    {
        //随机选择一个背景音乐并播放
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    //停止所有背景音乐
    public void StopAllBGM()
    {
        //遍历所有背景音乐并停止播放
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop(); //停止播放当前背景音乐
        }
    }

    //允许播放音效
    public void AllowPlayingSFX()
    {
        canPlaySFX = true; 
    }
}
