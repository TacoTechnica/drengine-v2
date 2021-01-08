using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DREngine.Game.VN
{
    public class VNState
    {
        public Stack<VNStackFrame> Stack = new Stack<VNStackFrame>();

        [JsonIgnore] public VNStackFrame CurrentFrame => Stack.Peek();
        [JsonIgnore] public bool IsEmpty => Stack.Count == 0;

        public void Push(VNScript script, int commandIndex = 0)
        {
            Stack.Push(new VNStackFrame(script, commandIndex));
        }

        public void ReplaceTop(VNScript script, int commandIndex = 0)
        {
            if (IsEmpty) throw new InvalidOperationException("Can't replace top of VN Stack when it is empty.");
            Stack.Pop();
            Stack.Push(new VNStackFrame(script, commandIndex));
        }

        public void Pop()
        {
            if (IsEmpty) throw new InvalidOperationException("Tried popping from VN Stack when it was empty!");
            Stack.Pop();
        }
    }
}