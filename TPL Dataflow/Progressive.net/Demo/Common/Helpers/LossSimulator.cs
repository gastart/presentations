using System;
using System.Collections.Generic;
using Common.Domain;

namespace Common.Helpers
{
    public class LossSimulator
    {
        private readonly Random _random = new Random();
        private DateTime _startDate = new DateTime(2016, 1, 1);

        public IEnumerable<Trial> GenerateStatic(int numberOfRounds, int numberOfLosses)
        {
            for (int i = 0; i < numberOfRounds; i++)
            {
                var round = new Trial(i);
                for (int j = 0; j < numberOfLosses; j++)
                {
                    round.Losses.Add(new Loss
                    {
                        Id = Guid.NewGuid(),
                        Amount = _random.Next(1, 999999),
                        OccuredAt = _startDate.AddDays(_random.Next(365))
                    });
                }

                yield return round;
            }

        }

        public IEnumerable<Trial> GenerateRandom(int numberOfRounds, int maxNumberOfLossesPerRound = 5)
        {
            for (int i = 0; i < numberOfRounds; i++)
            {
                var round = new Trial(i);
                for (int j = 0; j < _random.Next(0, maxNumberOfLossesPerRound); j++)
                {
                    round.Losses.Add(new Loss
                    {
                        Id = Guid.NewGuid(),
                        Amount = _random.Next(1, 100000),
                        OccuredAt = _startDate.AddDays(_random.Next(365))
                    });
                }

                yield return round;
            }
        }
    }
}