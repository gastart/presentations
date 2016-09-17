using System.Collections.Generic;
using System.Diagnostics;
using Common.Domain;

namespace OldApp
{
    public class LossLimiter
    {
        public List<AggreatedRound> Limit(List<Round> rounds, int limit)
        {
            var limited = new List<AggreatedRound>();
            foreach (var round in rounds)
            {
                AggreatedRound limitedLoss = new AggreatedRound {RoundId = round.Id};
                
                foreach (var loss in round.Losses)
                {
                    limitedLoss.AggreatedLoss += loss.Amount;

                    if (limitedLoss.AggreatedLoss > limit)
                    {
                        limitedLoss.AggreatedLoss = limit;
                        break;
                    }
                }

                limited.Add(limitedLoss);
            }

            return limited;
        }
    }
}