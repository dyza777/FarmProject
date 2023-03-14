using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _wheatCountText;
    [SerializeField] private TextMeshProUGUI _coinsCountText;
    private int _visibleCoinsCount;
    Coroutine _coinsCounting;

    void Update()
    {
        _wheatCountText.text = CharacterController.Instance.currentStackCount.ToString() + "/40";

        if (_visibleCoinsCount != CharacterController.Instance.currentCoinsCount && _coinsCounting == null)
        {
            _coinsCounting = StartCoroutine(CountUpCoins(_visibleCoinsCount, CharacterController.Instance.currentCoinsCount));
        }
    }

    IEnumerator CountUpCoins(int startValue, int endValue)
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / 0.5f;
            int currentValue = (int)Mathf.Lerp(startValue, endValue, t);
            _visibleCoinsCount = currentValue;
            _coinsCountText.text = currentValue.ToString();
            yield return null;
        }
        _coinsCounting = null;
    }
}
