using System.Collections.Generic;
using ProtoBuf;

namespace Common.Domain
{
    [ProtoContract]
    public class Trial
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public List<Loss> Losses { get; set; }

        public Trial(int id)
        {
            Id = id;
            Losses = new List<Loss>();
        }
        public override string ToString()
        {
            return $"Round {Id} - {Losses.Count} losses";
        }
    }
}