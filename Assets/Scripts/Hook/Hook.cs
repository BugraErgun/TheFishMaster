using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Device;


public class Hook : MonoBehaviour
{
    public Transform hookedTransform;

    private Camera mainCam;
    private CircleCollider2D coll;

    private int length;
    private int strenght;
    private int fishCount;

    private bool canMove=true;

    private List<Fish> hookedFishes;


    private Tweener cameraTween;

    private void Awake()
    {
        mainCam = Camera.main;
        coll = GetComponent<CircleCollider2D>();
        hookedFishes = new List<Fish>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (canMove && Input.GetMouseButton(0))
        {
            Vector3 vec = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 pos = transform.position;

            pos.x = vec.x;
            transform.position = pos;
        }
    }

   public void StartFishing()
    {
        length = IdleManager.instance.length - 20;
        strenght = IdleManager.instance.strength;
        fishCount = 0;

        float time = (-length) * .1f;

        cameraTween = mainCam.transform.DOMoveY(length, 1 + time * .25f, false).OnUpdate(delegate
        {
            if (mainCam.transform.position.y <= -11)
            {
                transform.SetParent(mainCam.transform);
            }
        }).OnComplete(delegate
        {

            coll.enabled = true;
            cameraTween = mainCam.transform.DOMoveY(0, time * 5, false).OnUpdate(delegate
            {

                if (mainCam.transform.position.y >= -25f)
                {
                    StopFishing();
                }
            });
        });

        ScreensManager.instance.ChangeScreen(Screens.GAME);
        coll.enabled = false;
        canMove = true;
        hookedFishes.Clear();



    }
    public void StopFishing()
    {
        canMove = false;
        cameraTween.Kill(false);
        cameraTween = mainCam.transform.DOMoveY(0, 2, false).OnUpdate(delegate
        {

            if (mainCam.transform.position.y >= -11)
            {
                transform.SetParent(null);
                transform.position = new Vector2(transform.position.x, -6);
            }
        }).OnComplete(delegate
        {

            transform.position = Vector2.down * 6;
            coll.enabled = true;
            int num = 0;
            for (int i = 0; i < hookedFishes.Count; i++)
            {
                hookedFishes[i].transform.SetParent(null);
                hookedFishes[i].ResetFish();
                num += hookedFishes[i].Type.price;
            }
            IdleManager.instance.totalGain = num;
            ScreensManager.instance.ChangeScreen(Screens.END);

        });
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.CompareTag("Fish") && fishCount !=strenght)
        {
            fishCount++;
            Fish component = target.GetComponent<Fish>();
            component.Hooked();
            hookedFishes.Add(component);
            target.transform.SetParent(transform);
            target.transform.position = hookedTransform.position;
            target.transform.rotation = hookedTransform.rotation;
            target.transform.localScale = Vector3.one;

            target.transform.DOShakePosition(5, Vector3.forward * 45, 10, 90, false).SetLoops(1, LoopType.Yoyo).OnComplete(delegate
            {

                target.transform.rotation = Quaternion.identity;
            });
            if (fishCount == strenght)
            {
                StopFishing();
            }
        }
    }

}
