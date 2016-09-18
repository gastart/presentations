using System;
using ProtoBuf;

namespace NewApp.Domain
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

        protected bool Equals(Loss other)
        {
            return Amount == other.Amount &&
                   OccuredAt.Equals(other.OccuredAt) &&
                   Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Loss)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Amount.GetHashCode();
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                hashCode = (hashCode * 397) ^ OccuredAt.GetHashCode();

                return hashCode;
            }
        }
    }
}