using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace YanevyukLibrary{
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class OnEvent : System.Attribute
    {
        // See the attribute guidelines at
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string eventName;

        // This is a positional argument
        public OnEvent(string eventName)
        {
            this.eventName = eventName;
        }

        public string EventName
        {
            get { return eventName; }
        }

        // This is a named argument
        public int NamedInt { get; set; }
    }
}