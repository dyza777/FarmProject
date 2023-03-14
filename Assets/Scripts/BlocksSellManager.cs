using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlocksSellManager : MonoBehaviour
{
    public static BlocksSellManager Instance;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform coinLogoTransform;
    [SerializeField] private GameObject CoinPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public void HandleSell()
    {
        var coin = Instantiate(CoinPrefab, canvas.transform);

        coin.transform.DOMove(coinLogoTransform.position, 0.5f).SetDelay(0.3f).OnComplete(delegate ()
        {
            Destroy(coin.gameObject);
            CharacterController.Instance.AddCoins();
            coinLogoTransform.parent.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.1f, 5);
        });
    }
}
