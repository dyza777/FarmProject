using UnityEngine;
using DG.Tweening;

public class WheatTrigger : MonoBehaviour
{
    private float _initZScale;
    private WheatBlock _wheatBlock;
    [SerializeField] private GameObject _wheatCutVFXPrefab;

    private void Awake()
    {
        _initZScale = transform.localScale.z;
        _wheatBlock = transform.parent.gameObject.GetComponent<WheatBlock>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Sickle") return;
        _wheatBlock.HandleCut(this, other.gameObject.transform.root.gameObject.GetComponent<CharacterController>(), _initZScale);
        GameObject vfx = Instantiate(_wheatCutVFXPrefab, transform);
        Destroy(vfx, 2f);
    }

    public void GrowUpAgain()
    {
        transform.DOScale(new Vector3(transform.localScale.x, transform.localScale.y, _initZScale), 1).SetEase(Ease.Linear);
    }
}
