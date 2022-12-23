using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Fish : MonoBehaviour
{
    private Fish.FishType type;
    private CircleCollider2D coll;
    private SpriteRenderer renderer;
    private float screenLeft;
    private Tweener tweener;


    public Fish.FishType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
            coll.radius = type.colliderRadius;
            renderer.sprite = type.sprite;
        }
    }
    private void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        renderer = GetComponentInChildren<SpriteRenderer>();
        screenLeft = Camera.main.ScreenToWorldPoint(Vector3.zero).x;


      
    }
    void Start()
    {
        
    }

    public void ResetFish()
    {
        if (tweener !=null)
        {
            tweener.Kill(false);
        }

        float num = UnityEngine.Random.Range(type.minLenght, type.maxLenght);
            coll.enabled = true;
            Vector3 pos = transform.position;
            pos.y = num;
            pos.x = screenLeft;
            transform.position = pos;

            float num2 = 1;
            float y = UnityEngine.Random.Range(num - num2, num + num2);
            Vector2 v = new Vector2(-pos.x, y);
            float num3 = 3;
            float delay = UnityEngine.Random.Range(0, 2 * num3);
            tweener = transform.DOMove(v, num3, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(delay).OnStepComplete(delegate
            {

                Vector3 localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
            });   
    }

    public void Hooked()
    {
        coll.enabled = false;
        tweener.Kill(false);

    }

    [Serializable]
    public class FishType
    {
        public int price;
        public float fishCount;
        public float minLenght;
        public float maxLenght;
        public float colliderRadius;
        public Sprite sprite;

    }
}
