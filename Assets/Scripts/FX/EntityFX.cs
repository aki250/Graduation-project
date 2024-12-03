using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Player player;
    [Header("�����ı�")]
    [SerializeField] private GameObject popUpTextPrefab;  //�����ı�Ԥ�Ƽ�

    [Header("��˸Ч��")]
    [SerializeField] private float flashDuration;  //��˸Ч������ʱ��
    [SerializeField] private Material hitMaterial;
    private Material originalMaterial;  //ԭʼ����

    [Header("�쳣״̬��ɫ")]
    [SerializeField] private Color[] igniteColor;  //��ȼ
    [SerializeField] private Color chillColor;  //����
    [SerializeField] private Color[] shockColor;  //���

    private bool canApplyAilmentColor;  //�Ƿ�Ӧ���쳣״̬��ɫ

    [Header("�쳣״̬����Ч��")]
    [SerializeField] private ParticleSystem igniteFX;  //��ȼЧ��
    [SerializeField] private ParticleSystem chillFX;  //����Ч��
    [SerializeField] private ParticleSystem shockFX;  //���Ч��

    [Header("������Ч")]
    [SerializeField] private GameObject hitFXPrefab;  //��ͨ������Ч
    [SerializeField] private GameObject critHitFXPrefab;  //����������Ч

    protected GameObject HPBar;  //������



    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        HPBar = GetComponentInChildren<HPBar_UI>()?.gameObject;
    }

    protected virtual void Start()
    {
        originalMaterial = sr.material;
        player = PlayerManager.instance.player;
    }

    protected virtual void Update()
    {

    }


    public GameObject CreatePopUpText(string _text)
    {
        //������ɵ����ı���λ��ƫ����
        float xOffset = Random.Range(-1, 1);  //xƫ��
        float yOffset = Random.Range(1.5f, 3);  //yƫ��

        //����ƫ������Vector3λ��
        Vector3 postionOffset = new Vector3(xOffset, yOffset, 0);

        //ʵ���������ı�Ԥ���壬������λ��
        GameObject newText = Instantiate(popUpTextPrefab, transform.position + postionOffset, Quaternion.identity);

        //�����ı�
        newText.GetComponent<TextMeshPro>().text = _text;

        return newText;
    }

    private IEnumerator FlashFX()
    {
        //��ֹӦ���쳣״̬��ɫ
        canApplyAilmentColor = false;

        //�����ܻ�ʱ�Ĳ���ΪhitMaterial
        sr.material = hitMaterial;

        //�ȴ���˸Ч������ʱ��
        yield return new WaitForSeconds(flashDuration);

        //�ָ�ԭʼ����
        sr.material = originalMaterial;

        //�����ٴ�Ӧ���쳣״̬��ɫ
        canApplyAilmentColor = true;
    }

    private void RedColorBlink()
    {
        //�л���ɫΪ��/�ף�ģ����˸Ч��
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
        {
            sr.color = Color.red;
        }
    }

    private void CancelColorChange()
    {
        //ȡ����ɫ����Ķ�ʱ���ã���������ɫ
        CancelInvoke();
        sr.color = Color.white;
        Debug.Log("Set to color white");

        //ֹͣ�쳣״̬������Ч��
        igniteFX.Stop();
        chillFX.Stop();
        shockFX.Stop();
    }

    public void MakeEntityTransparent(bool _transparent)
    {
        //����ʵ���Ƿ�͸��
        if (_transparent)
        {
            sr.color = Color.clear;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    public void MakeEntityTransparent_IncludingHPBar(bool _transparent)
    {
        //����ʵ�弰��Ѫ���Ƿ�͸��
        if (_transparent)
        {
            if (HPBar != null)
            {
                HPBar.SetActive(false);  //����Ѫ��
            }
            sr.color = Color.clear;  //����ʵ��͸��
        }
        else
        {
            if (HPBar != null)
            {
                HPBar.SetActive(true);  //��ʾѪ��
            }
            sr.color = Color.white;  //�ָ�ʵ����ɫΪ��ɫ
        }
    }

    #region Ailment FX
    public void EnableIgniteFXForTime(float _seconds)
    {
        //��ȼ����Ч������������������ɫ�仯
        igniteFX.Play();
        InvokeRepeating("IgniteColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);  //����ʱ���ֹͣ��ɫ�仯
    }

    private void IgniteColorFX()
    {
        //��ȼ״̬����ɫ
        if (sr.color != igniteColor[0])
        {
            sr.color = igniteColor[0];
            Debug.Log("Set to ignite color 0");
        }
        else
        {
            sr.color = igniteColor[1];
            Debug.Log("Set to ignite color 1");
        }
    }

    public void EnableChillFXForTime(float _seconds)
    {
        //��������Ч������������ɫ�仯
        chillFX.Play();
        ChillColorFX();
        Invoke("CancelColorChange", _seconds);  //����ʱ���ֹͣ��ɫ�仯
    }

    private void ChillColorFX()
    {
        //���ñ���״̬��ɫ
        if (sr.color != chillColor)
        {
            sr.color = chillColor;
        }
    }

    public void EnableShockFXForTime(float _seconds)
    {
        //�����������Ч������������������ɫ�仯
        shockFX.Play();
        InvokeRepeating("ShockColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);  //����ʱ���ֹͣ��ɫ�仯
    }

    private void ShockColorFX()
    {
        //�л����״̬����ɫ
        if (sr.color != shockColor[0])
        {
            sr.color = shockColor[0];
        }
        else
        {
            sr.color = shockColor[1];
        }
    }

    //��Э��ȷ�����쳣״̬Ч��ǰ��Ӧ�û�����ɫЧ��
    #region AilmentColorFX_Coroutine
    public IEnumerator EnableIgniteFXForTime_Coroutine(float _seconds)
    {
        //�ȴ�ֱ������Ӧ���쳣״̬��ɫ
        yield return new WaitUntil(() => canApplyAilmentColor == true);
        EnableIgniteFXForTime(_seconds);  //��ȼЧ��
    }

    public IEnumerator EnableChillFXForTime_Coroutine(float _seconds)
    {
        yield return new WaitUntil(() => canApplyAilmentColor == true);
        EnableChillFXForTime(_seconds);  //����Ч��
    }

    public IEnumerator EnableShockFXForTime_Coroutine(float _seconds)
    {
        yield return new WaitUntil(() => canApplyAilmentColor == true);
        EnableShockFXForTime(_seconds);  //���Ч��
    }
    #endregion

    #endregion

    public void CreateHitFX(Transform _targetTransform, bool _canCrit)
    {
        //������ɻ�����Ч��λ�ú���ת
        float zRotation = Random.Range(-90, 90);  //�����תz
        float xOffset = Random.Range(-0.5f, 0.5f);  //���xƫ��
        float yOffset = Random.Range(-0.5f, 0.5f);  //���yƫ��

        //������Ч��λ��
        Vector3 generationPosition = new Vector3(_targetTransform.position.x + xOffset, _targetTransform.position.y + yOffset);
        Vector3 generationRotation = new Vector3(0, 0, zRotation);

        //�����Ƿ񱩻�ѡ��ͬ����ЧԤ�Ƽ�
        GameObject prefab = hitFXPrefab;
        if (_canCrit)
        {
            prefab = critHitFXPrefab;

            //����Ǳ������������z����ת
            zRotation = Random.Range(-30, 30);

            //���ݵ����泯�������������Ч����ת
            float yRotation = 0;
            if (GetComponent<Entity>().facingDirection == -1)
            {
                yRotation = 180;  //��������泯�󷽣���ת180��
            }

            generationRotation = new Vector3(0, yRotation, zRotation);
        }

        //ʵ����������Ч
        GameObject newHitFX = Instantiate(prefab, generationPosition, Quaternion.identity);
        newHitFX.transform.Rotate(generationRotation);  //��ת

        //0.5���������Ч
        Destroy(newHitFX, 0.5f);
    }




}
