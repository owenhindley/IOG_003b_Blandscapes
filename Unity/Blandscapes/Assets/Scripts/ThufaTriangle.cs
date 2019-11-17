using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThufaTriangle : MonoBehaviour
{
    public Collider triggerCollider;
    public AnimationCurve flutterAnimation;

    // public Light pointLight;

    public float openAngle = 30.0f;
    public float openTime = 0.5f;

    public bool isOpen = false;

    private float normalLightIntens = 0.0f;

    [InspectorButton("DoFlutter")] public bool DoFlutterButton;

    private Quaternion originalRotation;
    private Quaternion openRotation;

    public Transform glowSphere;

    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.localRotation;
        openRotation = originalRotation * Quaternion.Euler(-openAngle, 0.0f, 0.0f);
    //     if (pointLight == null) { pointLight = GetComponentInChildren<Light>(); }
    //     normalLightIntens= pointLight.intensity;
    //     pointLight.intensity = 0.0f;
    //     pointLight.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var isOpenNow = triggerCollider.bounds.Contains(transform.position);
        if (isOpenNow != isOpen){
            if (isOpenNow) { DoFlutter(); }
            isOpen = isOpenNow;
        }
    }

    public void SetMidpoint(Vector3 midpoint){
        glowSphere.localPosition = midpoint;
    }

    public void DoFlutter(){
        // pointLight.enabled = true;
        // pointLight.DOIntensity(normalLightIntens, 1.0f);
        transform.DOLocalRotate(openRotation.eulerAngles, openTime, RotateMode.Fast).OnComplete(()=>{
            if (isOpen){
                transform.DOLocalRotate(originalRotation.eulerAngles, openTime * 2.0f).SetEase(Ease.InOutBounce).OnComplete(()=>{
                    if (isOpen){
                        DoFlutter();
                    }
                });
            } else {
                transform.DOLocalRotate(originalRotation.eulerAngles, openTime/4.0f).SetEase(Ease.InOutCubic);
                //  pointLight.DOIntensity(normalLightIntens, 1.0f).OnComplete(()=>{
                //       pointLight.enabled = false;
                //  });
            }
        }).SetEase(Ease.InOutCubic).SetDelay(Random.Range(0.0f, openTime));

    }

}
