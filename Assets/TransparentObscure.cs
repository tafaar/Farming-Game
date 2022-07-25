using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TransparentObscure : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] spriteRenderers;

    [SerializeField] Color _alphaColor = new Color(255, 255, 255, 85);
    [SerializeField] float _targetLerpTime = 0.5f;
    float _lerpTime = 0;
    float _lerpRatio = 0;
    bool _obscuringPlayer = false;
    public float health = 1f;


    void Awake()
    {
        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sortingOrder = -Mathf.RoundToInt(spriteRenderers[i].transform.position.y * 10) + i;
        }
    }

    private void Update()
    {
        int order = 0;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sortingOrder = -Mathf.RoundToInt(spriteRenderers[i].transform.position.y * 10) + i;
        }

        if (_obscuringPlayer)
        {
            _lerpTime += Time.deltaTime;

            _lerpTime = Mathf.Clamp(_lerpTime, 0, _targetLerpTime);

            _lerpRatio = _lerpTime / _targetLerpTime;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = Color.Lerp(Color.white, _alphaColor, _lerpRatio);

                if (_lerpRatio == 1) spriteRenderers[i].color = _alphaColor;
            }
        }
        else
        {
            _lerpTime -= Time.deltaTime;

            _lerpTime = Mathf.Clamp(_lerpTime, 0, _targetLerpTime);

            _lerpRatio = _lerpTime / _targetLerpTime;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = Color.Lerp(Color.white, _alphaColor, _lerpRatio);

                if (_lerpRatio == 0) spriteRenderers[i].color = Color.white;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _obscuringPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _obscuringPlayer = false;
        }
    }
}
