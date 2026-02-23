using System;
using System.Collections.Generic;
using UnityEngine;

namespace  Field
{
    public interface IInteractable
    {
        /// <summary>
        /// character와 상호작용가능한 옵션을 불러옴
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public InteractOption GetOption(Character character);
    }

    public record InteractOption
    {
        public string ActionName;
        // TODO : 매개변수에 어떤 캐릭터가 상호작용하는지 추가
        public Action<Character> InteractAction;
    }   
}
