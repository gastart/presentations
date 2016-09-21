using System;
using System.Collections.Generic;
using NewApp.Domain;

namespace Common.Helpers
{
    public class LossSimulator
    {
        private readonly Random _random = new Random();
        private DateTime _startDate = new DateTime(2016, 1, 1);

        public IEnumerable<Trial> Generate(int numberOfRounds, int numberOfLosses)
        {
            for (int i = 0; i < numberOfRounds; i++)
            {
                var round = new Trial {Id = i, Losses = new List<Loss>()};
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

        public IEnumerable<Trial> GenerateBetter(int numberOfRounds)
        {
            for (int i = 0; i < numberOfRounds; i++)
            {
                var round = new Trial {Id = i, Losses = new List<Loss>()};
                for (int j = 0; j < _random.Next(0, 5); j++)
                {
                    round.Losses.Add(new Loss
                    {
                        Id = Guid.NewGuid(),
                        Amount = _random.Next(1, 10000),
                        OccuredAt = _startDate.AddDays(_random.Next(365))
                    });
                }

                yield return round;
            }
        }
    }
}