using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;


namespace YanevyukLibrary.Events
{
    public interface IEventStruct
    {
        void Subscribe(MethodInfo methodInfo,object obj);
        void Exit(MethodInfo methodInfo, object obj);
    }
    public struct Event : IEventStruct
    {
        public delegate void deleg();
        public List<deleg>  _event;

        public void Call(){
            if( _event == null){
                Debug.Log("Event is null. Instance ID: "+this);
                return;
            }
            foreach(var method in _event){
                method();
            }
        }

        public void Subscribe(deleg method){
            Debug.Log("Registering. Instance ID: "+this);
            if(this._event == null){
                this._event = new List<deleg>();
            }
            this._event.Add(method);
        }

        public void Subscribe(MethodInfo methodInfo,object obj){
            deleg function = (deleg)System.Delegate.CreateDelegate(typeof(deleg),obj, methodInfo);
            Subscribe(function);
        }

        public void Exit(deleg method){
            if(this._event == null){
                this._event = new List<deleg>();
                return;
            }
            if(this._event.Contains(method)){
                this._event.Remove(method);
            }
        }

        public void Exit(MethodInfo methodInfo,object obj){
            deleg function = (deleg)System.Delegate.CreateDelegate(typeof(deleg),obj, methodInfo);
            Exit(function);
        }
    }

    public struct Event<T> : IEventStruct{
        public delegate void deleg(T arg1);
        public List<deleg>  _event;

        public void Call(T arg1){
            if( _event == null){
                return;
            }
            foreach(var method in _event){
                method(arg1);
            }
        }

        public void Subscribe(deleg method){
            if(this._event == null){
                this._event = new List<deleg>();
            }
            this._event.Add(method);
        }

        public void Subscribe(MethodInfo methodInfo,object obj){
            deleg function = (deleg)System.Delegate.CreateDelegate(typeof(deleg),obj, methodInfo);
            Subscribe(function);
        }

        public void Exit(deleg method){
            if(this._event == null){
                this._event = new List<deleg>();
                return;
            }
            if(this._event.Contains(method)){
                this._event.Remove(method);
            }
        }

        public void Exit(MethodInfo methodInfo,object obj){
            deleg function = (deleg)System.Delegate.CreateDelegate(typeof(deleg),obj, methodInfo);
            Exit(function);
        }

    }

    public struct Event<T,K> : IEventStruct{
        public delegate void deleg(T arg1,K arg2);
        public List<deleg>  _event;

        public void Call(T arg1, K arg2){
            if( _event == null){
                return;
            }
            foreach(var method in _event){
                method(arg1,arg2);
            }
        }

        public void Subscribe(deleg method){
            if(this._event == null){
                this._event = new List<deleg>();
            }
            this._event.Add(method);
        }

        public void Subscribe(MethodInfo methodInfo,object obj){
            deleg function = (deleg)System.Delegate.CreateDelegate(typeof(deleg),obj, methodInfo.Name,false);
            Subscribe(function);
        }

        public void Exit(deleg method){
            if(this._event == null){
                this._event = new List<deleg>();
                return;
            }
            if(this._event.Contains(method)){
                this._event.Remove(method);
            }
        }

        public void Exit(MethodInfo methodInfo,object obj){
            deleg function = (deleg)System.Delegate.CreateDelegate(typeof(deleg),obj, methodInfo);
            Exit(function);
        }

    }

    public struct Event<T,K,C> : IEventStruct{
        public delegate void deleg(T arg1,K arg2,C arg3);
        public List<deleg>  _event;

        public void Call(T arg1, K arg2, C arg3){
            if( _event == null){
                return;
            }
            foreach(var method in _event){
                method(arg1,arg2,arg3);
            }
        }

        public void Subscribe(deleg method){
            if(this._event == null){
                this._event = new List<deleg>();
            }
            this._event.Add(method);
        }

        public void Subscribe(MethodInfo methodInfo,object obj){
            deleg function = (deleg)System.Delegate.CreateDelegate(typeof(deleg),obj, methodInfo);
            Subscribe(function);
        }

        public void Exit(deleg method){
            if(this._event == null){
                this._event = new List<deleg>();
                return;
            }
            if(this._event.Contains(method)){
                this._event.Remove(method);
            }
        }
        public void Exit(MethodInfo methodInfo,object obj){
            deleg function = (deleg)System.Delegate.CreateDelegate(typeof(deleg),obj, methodInfo);
            Exit(function);
        }
    }



    [System.Serializable]
    public class EventManagerException : System.Exception
    {
        public EventManagerException() { }
        public EventManagerException(string message) : base(message) { }
        public EventManagerException(string message, System.Exception inner) : base(message, inner) { }
        protected EventManagerException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }



}

public static class EventManager{
    private static bool initialized = false;
    private static YanevyukLibrary.Events.Events eventClassInstance;
    public static void Initialize(){
        eventClassInstance = new YanevyukLibrary.Events.Events();
        YanevyukLibrary.Events.Events.Instance = eventClassInstance;
        initialized = true;
    }

    public static void Reset(){
        initialized = false;
    }
    
    public static void Register(this object target){
        if(initialized == false){
            return;
        }


        MethodInfo[] arr = target.GetType().GetMethods((BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));
        //arr contains all the methods of the target

        foreach (var method in arr)
        {

            YanevyukLibrary.OnEvent[] eventAttributes = (YanevyukLibrary.OnEvent[])method.GetCustomAttributes(typeof(YanevyukLibrary.OnEvent),true);
            foreach (var eventBind in eventAttributes)
            {
                string eventName = eventBind.EventName;
                string methodToRegister = method.Name;

                Debug.Log("Requested to bind event. "+methodToRegister+" to "+eventName);

                //finding the field of event method
                System.Reflection.FieldInfo field = eventClassInstance.GetType().GetField(eventName);
                if(field==null){
                    Debug.LogWarning("The event ["+eventName+"] wasn't found. Skipping.");
                    continue;
                }
                YanevyukLibrary.Events.IEventStruct eventValue = field.GetValue(eventClassInstance) as YanevyukLibrary.Events.IEventStruct; //since it is a static class


                if(eventValue!=null){
                    eventValue.Subscribe(method,target);
                }
                field.SetValue(eventClassInstance,eventValue);
            }
        }


    }

    public static void Unregister(this object target){
        if(initialized == false){
            return;
        }


        MethodInfo[] arr = target.GetType().GetMethods((BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));
        //arr contains all the methods of the target

        foreach (var method in arr)
        {

            YanevyukLibrary.OnEvent[] eventAttributes = (YanevyukLibrary.OnEvent[])method.GetCustomAttributes(typeof(YanevyukLibrary.OnEvent),true);
            foreach (var eventBind in eventAttributes)
            {
                string eventName = eventBind.EventName;
                string methodToRegister = method.Name;

                Debug.Log("Requested to unbind event. "+methodToRegister+" to "+eventName);

                //finding the field of event method
                System.Reflection.FieldInfo field = eventClassInstance.GetType().GetField(eventName);
                if(field==null){
                    Debug.LogWarning("The event ["+eventName+"] wasn't found. Skipping.");
                    continue;
                }
                YanevyukLibrary.Events.IEventStruct eventValue = field.GetValue(eventClassInstance) as YanevyukLibrary.Events.IEventStruct; //since it is a static class


                if(eventValue!=null){
                    eventValue.Exit(method,target);
                }
                field.SetValue(eventClassInstance,eventValue);
            }
        }
    }
}