using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public delegate void CallBackEvent();
    public delegate void CallBackEvent<F>(F arg);
    public delegate void CallBackEvent<F, S>(F arg_1, S arg_2);

    public Hashtable eventHash = new Hashtable();

    // NO ARGUMENT

    public void AddCallBackEvent(CallBackEventType.TYPES _type, CallBackEvent _event)
    {
        var callBacks = (List<CallBackEvent>)eventHash[_type];
        if (callBacks == null)
        {
            callBacks = new List<CallBackEvent>();
            eventHash.Add(_type, callBacks);
        }
        callBacks.Add(_event);
    }

    public void RemoveCallBackEvent(CallBackEventType.TYPES _type, CallBackEvent _event)
    {
        var callBakcs = (List<CallBackEvent>)eventHash[_type];
        if (callBakcs != null)
            callBakcs.Remove(_event);
        else
            PrintRemoveException(_type);
    }

    public void RunEvent(CallBackEventType.TYPES _type)
    {
        var callbacks = (List<CallBackEvent>)eventHash[_type];
        if (callbacks != null)
            foreach (var callback in callbacks)
                callback();
        else
            PrintRunException(_type);
    }


    // 1 ARGUMENT
    public void AddCallBackEvent<F>(CallBackEventType.TYPES _type, CallBackEvent<F> _event)
    {
        var callBacks = (List<CallBackEvent<F>>)eventHash[_type];
        if (callBacks == null)
        {
            callBacks = new List<CallBackEvent<F>>();
            eventHash.Add(_type, callBacks);
        }
        callBacks.Add(_event);
    }

    public void RemoveCallBackEvent<F>(CallBackEventType.TYPES _type, CallBackEvent<F> _event)
    {
        var callBakcs = (List<CallBackEvent<F>>)eventHash[_type];
        if (callBakcs != null)
            callBakcs.Remove(_event);
        else
            PrintRemoveException(_type);
    }

    public void RunEvent<F>(CallBackEventType.TYPES _type, F arg_1)
    {
        var callbacks = (List<CallBackEvent<F>>)eventHash[_type];
        if (callbacks != null)
            foreach (var callback in callbacks)
                callback(arg_1);
        else
            PrintRunException(_type);
    }

    // 2 ARGUMENT
    public void AddCallBackEvent<F, S>(CallBackEventType.TYPES _type, CallBackEvent<F, S> _event)
    {
        var callBacks = (List<CallBackEvent<F, S>>)eventHash[_type];
        if (callBacks == null)
        {
            callBacks = new List<CallBackEvent<F, S>>();
            eventHash.Add(_type, callBacks);
        }
        callBacks.Add(_event);
    }

    public void RemoveCallBackEvent<F, S>(CallBackEventType.TYPES _type, CallBackEvent<F, S> _event)
    {
        var callBakcs = (List<CallBackEvent<F, S>>)eventHash[_type];
        if (callBakcs != null)
            callBakcs.Remove(_event);
        else
            PrintRemoveException(_type);
    }

    public void RunEvent<F, S>(CallBackEventType.TYPES _type, F arg_1, S arg_2)
    {
        var callbacks = (List<CallBackEvent<F, S>>)eventHash[_type];
        if (callbacks != null)
            foreach (var callback in callbacks)
                callback(arg_1, arg_2);
        else
            PrintRunException(_type);
    }

    public int GetCallBackEventCount<T>(CallBackEventType.TYPES _type)
    {
        if(eventHash.ContainsKey(_type))
        {
            var callBacks = (List<T>)eventHash[_type];
            return callBacks.Count;
        }

        return 0;
    }

    void PrintRemoveException(CallBackEventType.TYPES _type)
    {
        Debug.LogError($"등록되어 있지 않은 타입 이므로 <b><color=red> 제거 할 수 없습니다. </color></b>. Null Type : {_type}");
    }

    void PrintRunException(CallBackEventType.TYPES _type)
    {
        Debug.LogError($"등록되어 있지 않은 타입 이므로 <b><color=red> 실행 할 수 없습니다. </color></b> Null Type : {_type}");
    }

    private void Awake()
    {
        SetInstance();
    }

    void SetInstance()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }
}

