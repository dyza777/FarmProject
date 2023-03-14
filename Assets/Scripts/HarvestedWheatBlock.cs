using UnityEngine;
using DG.Tweening;

public class HarvestedWheatBlock : MonoBehaviour
{
    void Start()
    {
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 destination = new Vector3(transform.position.x + randomDirection.x, transform.position.y, transform.position.z + randomDirection.y);
        transform.DOJump(destination, 0.5f, 1, 0.5f);
        transform.DOLocalRotate(new Vector3(0, 360, 0), 4, RotateMode.FastBeyond360).SetRelative(true).SetLoops(-1);
    }
}
