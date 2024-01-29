using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berserkers
{
    public class Actor
    {
        public List<Unit> Army { get; protected set; } = new List<Unit>();
        public Races Race { get; protected set; }
        public int Resources { get; protected set; } = 0;
        public string Name { get; protected set; } = "";
        private Random Rnd { get; set; } = new Random();
        private static int ActorCount = 0;
        public Actor(int ArmySize, Races race, int StartingResources) 
        {
            Race = race;
            
            Resources = StartingResources;
            Name = $"Actor{ActorCount}";
            ActorCount++;

            Console.WriteLine($"Commander {Name} Has Entered The Battlefield, with {Resources} resources");

            switch (Race)
            {
                case Races.Dracuri: GenerateDracuri(ArmySize); break;
                case Races.Filrani: GenerateFilrani(ArmySize);break;
                case Races.Morgoli: GenerateMorgoli(ArmySize); break;
            }


        }

        public int StealResources(int StolenAmount)
        {
            Resources -= StolenAmount;
            if (Resources < 0)
            {
                StolenAmount += Resources;
                Resources = 0;
            }
            return StolenAmount;
        }

        public void GainResources(int StolenAmount)
        {
            Resources += StolenAmount;
        }

        private void GenerateDracuri(int ArmySize)
        {
            for (int i = 0; i < ArmySize; i++)
            {
                int RandomUnitIndex = Rnd.Next(0, 3);
                switch (RandomUnitIndex)
                {
                    case 0: Army.Add(new DracuriArcher()); break;
                    case 1: Army.Add(new DracuriAssasin()); break;
                    case 2: Army.Add(new DracuriJuggernaut()); break;
                    default: Army.Add(new DracuriJuggernaut()); break;
                }
            }
        }

        private void GenerateFilrani(int ArmySize)
        {
            for (int i = 0; i < ArmySize; i++)
            {
                int RandomUnitIndex = Rnd.Next(0, 3);
                switch (RandomUnitIndex)
                {
                    case 0: Army.Add(new FilraniDruid()); break;
                    case 1: Army.Add(new FilraniHunter()); break;
                    case 2: Army.Add(new FilraniWarden()); break;
                    default: Army.Add(new FilraniWarden()); break;
                }
            }
        }

        private void GenerateMorgoli(int ArmySize)
        {
            for (int i = 0; i < ArmySize; i++)
            {
                int RandomUnitIndex = Rnd.Next(0, 3);
                switch (RandomUnitIndex)
                {
                    case 0: Army.Add(new MorgoliMage()); break;
                    case 1: Army.Add(new MorgoliSiphoner()); break;
                    case 2: Army.Add(new MorgoliHusk()); break;
                    default: Army.Add(new MorgoliHusk()); break;
                }
            }
        }



    }
}
