using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Berserkers;

Random rnd = new Random();

Combat SimulatedFight = new Combat(GeneratePlayer(3),GeneratePlayer(3));

SimulatedFight.Fight();








Actor GeneratePlayer(int ArmySize)
{
    int StartingResources = rnd.Next(100, 500);
    int index = rnd.Next(3);
    Races race = Races.Dracuri;
    switch (index) 
    {
        case 0: race = Races.Dracuri; break;
        case 1: race = Races.Filrani; break;
        case 2: race = Races.Morgoli; break;
    }


    return new Actor(ArmySize, race, StartingResources);

}







