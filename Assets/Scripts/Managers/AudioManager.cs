using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //����ģʽ��ȷ��ֻ��һ��ʵ��
    public static AudioManager instance;

    public AudioSource[] sfx;    //���ڴ����ЧSFX��BGM������
    [SerializeField] private AudioSource[] bgm; //��������Դ

    [Space]
    [SerializeField] private float sfxMinHearableDistance; //��Ч��������С����

    public bool playingBGM; //�Ƿ����ڲ��ű�������
    private int bgmIndex; //��ǰ���ŵı�����������

    private bool canPlaySFX = false; //�Ƿ���Բ�����Ч

    private void Awake()
    {
        // ���ʵ��Ϊ�գ�������Ϊ��ǰʵ�������ʵ���Ѿ����ڣ����ٵ�ǰ����ȷ��ֻ��һ�� AudioManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //�ӳ�0.2�����������Ч
        Invoke("AllowPlayingSFX", 0.2f);
    }

    private void Update()
    {
        //���û�в��ű������֣���ֹͣ���б�������
        if (!playingBGM)
        {
            StopAllBGM();
        }
        else
        {
            //�����ǰ��������û�в��ţ���ʼ����
            if (bgm[bgmIndex].isPlaying == false)
            {
                PlayBGM(bgmIndex);
            }
        }
    }

    //����ָ����Ч
    public void PlaySFX(int _sfxIndex, Transform _sfxSourceTransform)
    {
        //�������������Ч��ֱ�ӷ���
        if (!canPlaySFX)
        {
            return;
        }

        //��ֹ�ظ�������ͬ����Ч��������ֽŲ���Ч���ص����ŵ����
        if (sfx[_sfxIndex].isPlaying == true)
        {
            return;
        }

        //�����ЧԴ�������̫Զ���򲻲��Ÿ���Ч
        if (_sfxSourceTransform != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _sfxSourceTransform.position) > sfxMinHearableDistance)
        {
            return;
        }

        //����ָ����������Ч
        if (_sfxIndex < sfx.Length)
        {
            //���������Ч������
            sfx[_sfxIndex].pitch = Random.Range(0.85f, 1.15f);
            sfx[_sfxIndex].Play(); //������Ч
        }
    }

    //ֹͣ����ָ������Ч
    public void StopSFX(int _sfxIndex)
    {
        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].Stop(); //ָֹͣ����������Ч
        }
    }

    //�𽥽���������ֱ����Чֹͣ
    public IEnumerator DecreaseVolumeGradually(AudioSource _audio)
    {
        float defaultVolume = _audio.volume; //��¼��Ƶ�ĳ�ʼ����

        //����������0.1ʱ�������𲽽�������
        while (_audio.volume > 0.1f)
        {
            //ÿ0.25�뽵�������� 20%
            _audio.volume -= _audio.volume * 0.2f;

            yield return new WaitForSeconds(0.25f);

            //���������0.1���£�ֹͣ��Ч���ָ�����
            if (_audio.volume <= 0.1f)
            {
                _audio.Stop(); //ֹͣ��Ч
                _audio.volume = defaultVolume; //�ָ���ʼ����
                break;
            }
        }
    }

    //����ָ�������ı�������
    public void PlayBGM(int _bgmIndex)
    {
        //ֹͣ���б������֣����ⲥ�Ŷ����������
        StopAllBGM();

        //����ָ�������ı�������
        if (_bgmIndex < bgm.Length)
        {
            bgmIndex = _bgmIndex; //���õ�ǰ�������ֵ�����
            bgm[bgmIndex].Play(); //���Ÿñ�������
        }
        else
        {
            Debug.Log("BGM������Χ"); 
        }
    }

    //���������������
    public void PlayRandomBGM()
    {
        //���ѡ��һ���������ֲ�����
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    //ֹͣ���б�������
    public void StopAllBGM()
    {
        //�������б������ֲ�ֹͣ����
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop(); //ֹͣ���ŵ�ǰ��������
        }
    }

    //��������Ч
    public void AllowPlayingSFX()
    {
        canPlaySFX = true; 
    }
}
