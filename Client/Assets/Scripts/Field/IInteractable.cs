using System;
using System.Collections.Generic;
using UnityEngine;

namespace  Field
{
    public interface IInteractable
    {
        public List<InteractOption> GetOptions(Character character);
    }

    public struct InteractOption
    {
        public string ActionName;
        // TODO : 매개변수에 어떤 캐릭터가 상호작용하는지 추가
        public Action<Character> InteractAction;
    }   
}
