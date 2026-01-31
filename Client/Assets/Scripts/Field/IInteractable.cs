using System;
using System.Collections.Generic;
using UnityEngine;

namespace  Field
{
    public interface IInteractable
    {
        public List<InteractOption> GetOptions();
    }

    public struct InteractOption
    {
        public string ActionName;
        public Action InteractAction;
    }   
}
