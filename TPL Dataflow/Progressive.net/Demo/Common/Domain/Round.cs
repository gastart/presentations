using System.Collections.Generic;
using ProtoBuf;

namespace Common.Domain
{
    [ProtoContract]
    public class Round
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public List<Loss> Losses { get; set; }

        public override string ToString()
        {
            return $"Round {Id} - {Losses.Count} losses";
        }
    }
}