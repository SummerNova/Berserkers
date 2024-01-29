using System.Text;
using System.Threading.Tasks;

namespace Berserkers
{
    public class Combat 
    {
        Actor[] Actors = new Actor[2];
        Unit AttackingUnit;
        Unit DefendingUnit;
        int currentPlayer = 0;
        int winnings = 0;
        Random rnd = new Random();
        Dictionary<Weather, int> WeatherCooldowns = new Dictionary<Weather, int>();

        public Combat(Actor actor1, Actor actor2)
        {
            Actors[0] = actor1;
            Actors[1] = actor2;
            WeatherCooldowns.Add(Weather.Storm, -1);
            WeatherCooldowns.Add(Weather.Scorch, -1);
            WeatherCooldowns.Add(Weather.Void, -1);
        }

        public void fight() 
        { 
            while(Actors[0].Army.Count > 0 && Actors[1].Army.Count > 0) 
            {
                currentPlayer = 1 - currentPlayer;
                Step();
            }

            if (Actors[0].Army.Count == 0 && Actors[1].Army.Count == 0)
            {
                Console.WriteLine("Both Armies Died.");
            }
            else if (Actors[0].Army.Count == 0)
            {
                Console.WriteLine($"{Actors[1].Name} has defeated his opponent");
                
                foreach (Unit unit in Actors[1].Army) 
                {
                    winnings += Actors[0].StealResources(unit.CarryCapacity);
                }

                Actors[1].GainResources(winnings);

                Console.WriteLine($"{Actors[1].Name} has stolen {winnings} Resources");
            }
            else
            {
                Console.WriteLine($"{Actors[0].Name} has defeated his opponent");

                foreach (Unit unit in Actors[0].Army)
                {
                    winnings += Actors[1].StealResources(unit.CarryCapacity);
                }

                Actors[0].GainResources(winnings);

                Console.WriteLine($"{Actors[0].Name} has stolen {winnings} Resources");
            }
        }

        private void Step()
        {
            AttackingUnit = Actors[currentPlayer].Army[rnd.Next(Actors[currentPlayer].Army.Count)];
            DefendingUnit = Actors[1-currentPlayer].Army[rnd.Next(Actors[1-currentPlayer].Army.Count)];

            AttackingUnit.Attack(DefendingUnit);

            if (!AttackingUnit.isAlive())
            {
                Actors[currentPlayer].Army.Remove(AttackingUnit);
            }

            if (!DefendingUnit.isAlive())
            {
                Actors[1-currentPlayer].Army.Remove(DefendingUnit);
            }

            ManageWeather(Weather.Storm, 0.1f, 5);
            ManageWeather(Weather.Scorch, 0.05f, 5);
            ManageWeather(Weather.Void, 0.25f, 2);

        }

        private void ManageWeather(Weather effect, float precenetage, int cooldown)
        {
            if (WeatherCooldowns[effect] > 0)
            {
                WeatherCooldowns[effect]--;
            }
            else if (WeatherCooldowns[effect] == 0)
            {
                WeatherCooldowns[effect]--;
                Unit.EndWeatherEffect(effect);
            }
            else
            {
                if (rnd.NextDouble() < precenetage)
                {
                    Unit.WeatherEffect(effect);
                    WeatherCooldowns[effect] = cooldown;
                }
            }
        }
    }


}


