using System.Collections;
using UnityEngine;

public class WheatBlock : MonoBehaviour
{
    [SerializeField] private GameObject _harvestedWheatBlockPrefab;
    [SerializeField] private float _growingUpTime = 10;

    [Tooltip("Чем выше значение, тем ниже скошенная пшеница")]
    [Range(2, 10)]
    [SerializeField] private int _cutWheatHeightFactor = 5;
    public void HandleCut(WheatTrigger wheat, CharacterController player, float initZScale)
    {
        GameObject harvestedBlock = Instantiate(_harvestedWheatBlockPrefab, Vector3.zero, Quaternion.identity, transform.root);
        harvestedBlock.transform.localPosition = wheat.transform.position + new Vector3(0, 0.5f, 0);
        player.CollectWheat(harvestedBlock);

        wheat.transform.localScale = new Vector3(wheat.transform.localScale.x, wheat.transform.localScale.y, initZScale / _cutWheatHeightFactor);
        StartCoroutine(GrowingUp(wheat));
    }

    IEnumerator GrowingUp(WheatTrigger wheat)
    {
        yield return new WaitForSeconds(_growingUpTime);
        wheat.GrowUpAgain();
    }
}
