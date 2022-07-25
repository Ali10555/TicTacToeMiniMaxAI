using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Leantween_Custom_Anims : MonoBehaviour
{
    public bool Set_ON_Start=true;
    public Vector3 _start_values;
    public bool To_Current_For_Tranform_Value;
    public enum _motion_type{_scale,_move,_rotate,_fade , fade_canvasGroup,rotate_around};
    public _motion_type anim_type;
    public Vector3 To_vector;
    public float _delay;
    public float _time;
    public float Speed;
    public bool play_automatic=true;
    public LeanTweenType _ease;
    public bool DestroyOnCompleteion = false;
    public UnityEvent OnCompletion;
    void Start()
    {
        

    }
    void OnEnable(){
        if (To_Current_For_Tranform_Value)
        {
            switch (anim_type)
            {
                case _motion_type._scale:
                    To_vector = transform.localScale;
                    break;
                case _motion_type._move:
                    To_vector = transform.position;
                    break;
                case _motion_type._rotate:
                    To_vector = transform.eulerAngles;
                    break;

                case _motion_type.fade_canvasGroup:
                    To_vector.x = GetComponent<CanvasGroup>().alpha;
                     break;
                
            }
        }

      if(play_automatic)
            Play_Anim();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Play_Anim(){
        RectTransform _rect = gameObject.GetComponent<RectTransform>();

        if(_rect == null){

        switch(anim_type){
            case _motion_type._scale:
            if(Set_ON_Start) transform.localScale = _start_values;
            LeanTween.scale(gameObject,To_vector,_time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
            break;
            case _motion_type._move:
            if(Set_ON_Start) transform.position = _start_values;
            LeanTween.move(gameObject,To_vector,_time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
            break;
            case _motion_type._rotate:
            if(Set_ON_Start) transform.rotation = Quaternion.Euler(_start_values);
            LeanTween.rotate(gameObject,To_vector,_time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
            break;
            case _motion_type._fade:
            if(Set_ON_Start) LeanTween.alpha(gameObject,_start_values.x,0f);
            LeanTween.alpha(gameObject,To_vector.x,_time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
            break;

            case _motion_type.fade_canvasGroup:
            if (Set_ON_Start) gameObject.GetComponent<CanvasGroup>().alpha = _start_values.x;
            LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), To_vector.x, _time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
            break;
            case _motion_type.rotate_around:
            if (Set_ON_Start) transform.rotation = Quaternion.Euler(_start_values);
            LeanTween.rotateAround(gameObject, To_vector,Speed, _time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
                    break;
            }
        
        }else{

        switch(anim_type){
            case _motion_type._scale:
            if(Set_ON_Start) _rect.localScale = _start_values;
            LeanTween.scale(_rect,To_vector,_time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
            break;
            case _motion_type._move:
            if(Set_ON_Start) _rect.anchoredPosition = _start_values;
            LeanTween.move(_rect,To_vector,_time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
            break;
            case _motion_type._rotate:
            if(Set_ON_Start) _rect.rotation = Quaternion.Euler(_start_values);
            LeanTween.rotate(_rect,To_vector,_time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
            break;
            case _motion_type._fade:
            if(Set_ON_Start) LeanTween.alpha(_rect,_start_values.x,0f);
            LeanTween.alpha(_rect,To_vector.x,_time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
            break;
            case _motion_type.fade_canvasGroup:
            if (Set_ON_Start) gameObject.GetComponent<CanvasGroup>().alpha = _start_values.x;
            LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), To_vector.x, _time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
                    break;
            case _motion_type.rotate_around:
                    if (Set_ON_Start) transform.rotation = Quaternion.Euler(_start_values);
                    LeanTween.rotateAround(_rect, To_vector, Speed, _time).setDelay(_delay).setEase(_ease).setOnComplete(() => { OnCompletion.Invoke(); if (DestroyOnCompleteion) { Destroy(gameObject); } });
                    break;
            
        }
        }
    }

   
}
