using System;
using ProtoBuf;

namespace Common.Domain
{
    [ProtoContract]
    public class Loss
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public decimal Amount { get; set; }

        [ProtoMember(3)]
        public DateTime OccuredAt { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Amount} ({OccuredAt})";
        }

        public void Scale(decimal adjustmentFactor)
        {
            Amount = Amount*adjustmentFactor;
        }
    }
}