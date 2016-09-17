using System.Collections.Generic;
using System.Linq;
using Common.Domain;

namespace OldApp
{
    public class LossJoiner
    {
        readonly Dictionary<int, List<Loss>> _lossesByRound = new Dictionary<int, List<Loss>>();
        public void AddData(List<Round> rounds)
        {
            foreach (var round in rounds)
            {
                if (_lossesByRound.ContainsKey(round.Id))
                {
                    _lossesByRound[round.Id].AddRange(round.Losses);
                }
                else
                {
                    _lossesByRound.Add(round.Id, round.Losses);
                }
            }   
        }

        public List<Round> GetRounds()
        {
            var allLosses = new List<Round>();

            foreach (var l in _lossesByRound.OrderBy(x => x.Key))
            {
                allLosses.Add(new Round {Id = l.Key, Losses = l.Value});
            }

            return allLosses;
        }
    }
}