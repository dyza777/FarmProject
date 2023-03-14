using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    public static CharacterController Instance;

    [SerializeField] private GameObject _sickle;
    [SerializeField] private Transform _stackPivot;
    [SerializeField] private Transform _barnTransform;

    [Min(1)]
    [Tooltip("Стоимость 1 блока пшеницы")]
    [SerializeField] private int _blockSellPrice = 15;

    [Range(1, 20)]
    [Tooltip("Скорость продажи блока")]
    [SerializeField] private int _blockSellSpeed = 15;

    private DynamicJoystick _joystick;
    private Animator _animator;
    private Tweener _stackTweener;
    Coroutine uploadBlocksCoroutine;

    private const float SMOOTH_BLEND_TIME = 0.04f;
    private const float BLOCKS_SELL_RANGE = 10f;

    [HideInInspector]
    public int currentStackCount = 0;
    [HideInInspector]
    public int currentCoinsCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        _joystick = FindObjectOfType<DynamicJoystick>();
        _animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (_joystick.Direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(_joystick.Direction.x, _joystick.Direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            transform.Rotate(_joystick.Direction);

            _animator.SetTrigger("Run");
            SetAnimatorSpeed(1);
            if (_stackTweener == null)
            {
                _stackTweener = _stackPivot.DOPunchPosition(new Vector3(0, 0.1f, 0), 0.4f, 0, 1f).OnComplete(() => _stackTweener = null);
            }
            _sickle.SetActive(false);
        } else
        {
            if ((transform.position.x > 3 || transform.position.x < -3) && transform.position.z < 15)
            {
                //Игрок на грядке - начинаем косить
                _animator.SetTrigger("Mow");
                _sickle.SetActive(true);
            }
            else
            {
                SetAnimatorSpeed(0);
                _sickle.SetActive(false);
            }
        }

        bool IsInBarnRange = isInBarnRange();
        if (IsInBarnRange && uploadBlocksCoroutine == null)
        {
            uploadBlocksCoroutine = StartCoroutine(UploadBlocksToBarn());
        } else if (!IsInBarnRange)
        {
            uploadBlocksCoroutine = null;
        }
    }


    public void CollectWheat(GameObject harvestedWheatBlock)
    {
        StartCoroutine(CollectWithDelay(harvestedWheatBlock));
    }

    IEnumerator CollectWithDelay(GameObject harvestedWheatBlock)
    {
        yield return new WaitForSeconds(1f);

        while (currentStackCount >= 40 || Vector3.Distance(harvestedWheatBlock.transform.position, _stackPivot.position) > 7)
        {
            yield return null;
        }

        currentStackCount++;
        Vector3 blockPositionInStack = CalculateNextWheatBlockPosition(harvestedWheatBlock);
        Vector3 offsetValues = blockPositionInStack / harvestedWheatBlock.transform.localScale.x;
        harvestedWheatBlock.transform.parent = null;

        int childIndex = (int)(offsetValues.x * 20 + offsetValues.y * 4 + offsetValues.z);
        Tweener tweener = harvestedWheatBlock.transform.DOMove(_stackPivot.position, 40f).SetSpeedBased(true);
        tweener.OnUpdate(delegate () {
            if (Vector3.Distance(harvestedWheatBlock.transform.position, _stackPivot.position) > 0.4f)
            {
                tweener.ChangeEndValue(_stackPivot.position, true);
            }
            else
            {
                tweener.Kill();
                DOTween.Kill(harvestedWheatBlock.transform);
                harvestedWheatBlock.transform.parent = _stackPivot;
                harvestedWheatBlock.transform.localRotation = _stackPivot.localRotation;
                harvestedWheatBlock.transform.localPosition = blockPositionInStack;
                harvestedWheatBlock.transform.SetSiblingIndex(childIndex);
            }
        });
    }

    public void EnableSickleCollider()
    {
        _sickle.GetComponent<MeshCollider>().enabled = true;
    }

    public void DisableSickleCollider()
    {
        _sickle.GetComponent<MeshCollider>().enabled = false;
    }

    void SetAnimatorSpeed(float speed, bool withoutBlend = false)
    {

        _animator.SetFloat("Speed", speed, withoutBlend ? 0 : SMOOTH_BLEND_TIME, Time.fixedDeltaTime);
    }

    private Vector3 CalculateNextWheatBlockPosition(GameObject harvestedWheatBlock)
    {
        harvestedWheatBlock.transform.parent = _stackPivot;
        harvestedWheatBlock.transform.localRotation = _stackPivot.localRotation;
        float sizeOffset = harvestedWheatBlock.transform.localScale.x;
        int zOffset = (currentStackCount - 1) / 20;
        int xOffset = ((currentStackCount - 1) % 20) % 4;
        int yOffset = ((currentStackCount - 1) % 20) / 4;
        Vector3 positionOffset = new Vector3(sizeOffset * zOffset, sizeOffset * yOffset, sizeOffset * xOffset);
        return positionOffset;
    }

    IEnumerator UploadBlocksToBarn()
    {
        if (currentStackCount < 1) {
            uploadBlocksCoroutine = null;
            yield break;
        }
        var nextBlock = _stackPivot.GetChild(_stackPivot.childCount - 1);
        nextBlock.transform.parent = null;
        nextBlock.transform.DOMove(_barnTransform.position, 1).OnComplete(() => Destroy(nextBlock.gameObject));
        currentStackCount--;
        BlocksSellManager.Instance.HandleSell();

        float uploadSpeedPercent = Mathf.InverseLerp(20, 1, _blockSellSpeed);
        float uploadSpeed = Mathf.Lerp(0.05f, 0.4f, uploadSpeedPercent);
        yield return new WaitForSeconds(uploadSpeed);

        if (isInBarnRange())
        {
            uploadBlocksCoroutine = StartCoroutine(UploadBlocksToBarn());
        }
    }

    private bool isInBarnRange()
    {
        return Vector3.Distance(transform.position, _barnTransform.position) < BLOCKS_SELL_RANGE;
    }

    public void AddCoins()
    {
        currentCoinsCount += _blockSellPrice;
    }
}
