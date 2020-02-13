using System;

namespace SomethingSpecific.ProtoNinja.Events
{
    public class TypedEventArgs<T> : EventArgs
    {
        public T Value { get; set; }
        public TypedEventArgs(T value) => Value = value;
    }
}