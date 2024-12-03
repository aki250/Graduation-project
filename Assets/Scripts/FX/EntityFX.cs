using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Player player;
    [Header("弹出文本")]
    [SerializeField] private GameObject popUpTextPrefab;  //弹出文本预制件

    [Header("闪烁效果")]
    [SerializeField] private float flashDuration;  //闪烁效果持续时间
    [SerializeField] private Material hitMaterial;
    private Material originalMaterial;  //原始材质

    [Header("异常状态颜色")]
    [SerializeField] private Color[] igniteColor;  //点燃
    [SerializeField] private Color chillColor;  //冰冻
    [SerializeField] private Color[] shockColor;  //电击

    private bool canApplyAilmentColor;  //是否应用异常状态颜色

    [Header("异常状态粒子效果")]
    [SerializeField] private ParticleSystem igniteFX;  //点燃效果
    [SerializeField] private ParticleSystem chillFX;  //冰冻效果
    [SerializeField] private ParticleSystem shockFX;  //电击效果

    [Header("击中特效")]
    [SerializeField] private GameObject hitFXPrefab;  //普通击中特效
    [SerializeField] private GameObject critHitFXPrefab;  //暴击击中特效

    protected GameObject HPBar;  //生命条



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
        //随机生成弹出文本的位置偏移量
        float xOffset = Random.Range(-1, 1);  //x偏移
        float yOffset = Random.Range(1.5f, 3);  //y偏移

        //创建偏移量的Vector3位置
        Vector3 postionOffset = new Vector3(xOffset, yOffset, 0);

        //实例化弹出文本预制体，并设置位置
        GameObject newText = Instantiate(popUpTextPrefab, transform.position + postionOffset, Quaternion.identity);

        //弹出文本
        newText.GetComponent<TextMeshPro>().text = _text;

        return newText;
    }

    private IEnumerator FlashFX()
    {
        //禁止应用异常状态颜色
        canApplyAilmentColor = false;

        //设置受击时的材质为hitMaterial
        sr.material = hitMaterial;

        //等待闪烁效果持续时间
        yield return new WaitForSeconds(flashDuration);

        //恢复原始材质
        sr.material = originalMaterial;

        //允许再次应用异常状态颜色
        canApplyAilmentColor = true;
    }

    private void RedColorBlink()
    {
        //切换颜色为红/白，模拟闪烁效果
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
        //取消颜色变更的定时调用，并重置颜色
        CancelInvoke();
        sr.color = Color.white;
        Debug.Log("Set to color white");

        //停止异常状态的粒子效果
        igniteFX.Stop();
        chillFX.Stop();
        shockFX.Stop();
    }

    public void MakeEntityTransparent(bool _transparent)
    {
        //设置实体是否透明
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
        //设置实体及其血条是否透明
        if (_transparent)
        {
            if (HPBar != null)
            {
                HPBar.SetActive(false);  //隐藏血条
            }
            sr.color = Color.clear;  //设置实体透明
        }
        else
        {
            if (HPBar != null)
            {
                HPBar.SetActive(true);  //显示血条
            }
            sr.color = Color.white;  //恢复实体颜色为白色
        }
    }

    #region Ailment FX
    public void EnableIgniteFXForTime(float _seconds)
    {
        //点燃粒子效果，并设置周期性颜色变化
        igniteFX.Play();
        InvokeRepeating("IgniteColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);  //设置时间后停止颜色变化
    }

    private void IgniteColorFX()
    {
        //点燃状态的颜色
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
        //冰冻粒子效果，并设置颜色变化
        chillFX.Play();
        ChillColorFX();
        Invoke("CancelColorChange", _seconds);  //设置时间后停止颜色变化
    }

    private void ChillColorFX()
    {
        //设置冰冻状态颜色
        if (sr.color != chillColor)
        {
            sr.color = chillColor;
        }
    }

    public void EnableShockFXForTime(float _seconds)
    {
        //启动电击粒子效果，并设置周期性颜色变化
        shockFX.Play();
        InvokeRepeating("ShockColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);  //设置时间后停止颜色变化
    }

    private void ShockColorFX()
    {
        //切换电击状态的颜色
        if (sr.color != shockColor[0])
        {
            sr.color = shockColor[0];
        }
        else
        {
            sr.color = shockColor[1];
        }
    }

    //用协程确保在异常状态效果前先应用击中颜色效果
    #region AilmentColorFX_Coroutine
    public IEnumerator EnableIgniteFXForTime_Coroutine(float _seconds)
    {
        //等待直到可以应用异常状态颜色
        yield return new WaitUntil(() => canApplyAilmentColor == true);
        EnableIgniteFXForTime(_seconds);  //点燃效果
    }

    public IEnumerator EnableChillFXForTime_Coroutine(float _seconds)
    {
        yield return new WaitUntil(() => canApplyAilmentColor == true);
        EnableChillFXForTime(_seconds);  //冰冻效果
    }

    public IEnumerator EnableShockFXForTime_Coroutine(float _seconds)
    {
        yield return new WaitUntil(() => canApplyAilmentColor == true);
        EnableShockFXForTime(_seconds);  //电击效果
    }
    #endregion

    #endregion

    public void CreateHitFX(Transform _targetTransform, bool _canCrit)
    {
        //随机生成击中特效的位置和旋转
        float zRotation = Random.Range(-90, 90);  //随机旋转z
        float xOffset = Random.Range(-0.5f, 0.5f);  //随机x偏移
        float yOffset = Random.Range(-0.5f, 0.5f);  //随机y偏移

        //生成特效的位置
        Vector3 generationPosition = new Vector3(_targetTransform.position.x + xOffset, _targetTransform.position.y + yOffset);
        Vector3 generationRotation = new Vector3(0, 0, zRotation);

        //根据是否暴击选择不同的特效预制件
        GameObject prefab = hitFXPrefab;
        if (_canCrit)
        {
            prefab = critHitFXPrefab;

            //如果是暴击，随机调整z轴旋转
            zRotation = Random.Range(-30, 30);

            //根据敌人面朝方向调整暴击特效的旋转
            float yRotation = 0;
            if (GetComponent<Entity>().facingDirection == -1)
            {
                yRotation = 180;  //如果敌人面朝左方，旋转180度
            }

            generationRotation = new Vector3(0, yRotation, zRotation);
        }

        //实例化击中特效
        GameObject newHitFX = Instantiate(prefab, generationPosition, Quaternion.identity);
        newHitFX.transform.Rotate(generationRotation);  //旋转

        //0.5秒后销毁特效
        Destroy(newHitFX, 0.5f);
    }




}
