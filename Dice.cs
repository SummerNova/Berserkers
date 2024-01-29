using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berserkers
{
    public struct Dice
    {
        uint Amount;
        uint BaseDie;
        int modifier;
        Random rnd = new Random();

        public Dice() : this(1, 6, 0)
        {

        }

        public Dice(uint amount, uint baseDIe, int modifier)
        {
            this.Amount = amount;
            this.BaseDie = baseDIe;
            this.modifier = modifier;
        }

        public int Roll()
        {
            int output = modifier;
            for (int i = 0; i < Amount; i++) { output += rnd.Next((int)BaseDie) + 1; }
            return output;
        }

        public override string ToString()
        {
            string Plus = "";
            if (modifier >= 0) Plus = "+";
            return $"{Amount}d{BaseDie}{Plus}{modifier}";
        }

        public override int GetHashCode()
        {
            int output = (int)(Amount << 4) + (int)(BaseDie << 8) + modifier;
            return output;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return Amount == ((Dice)obj).Amount && BaseDie == ((Dice)obj).BaseDie && modifier == ((Dice)obj).modifier;
        }


    }
}
