using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berserkers
{
    public interface IRandomProvider 
    {
        public int Roll();
    }

    public struct Bag : IRandomProvider
    {
        private int[] _Bag;
        private int Index = 0;
        private Random _Random = new Random();


        public Bag(params int[] Inputs)
        {
            _Bag = new int[Inputs.Length];

            for (int i = 0;i<_Bag.Length;i++)
            {
                _Bag[i] = Inputs[i];
            }

            Shuffle();
        }

        private void Shuffle()
        {
            int temp;
            int shuffle;
            for (int i = 0;i<_Bag.Length; i++)
            {
                temp = _Bag[i];
                _Bag[i] = temp;
                shuffle = _Random.Next(_Bag.Length);
                _Bag[i] = _Bag[shuffle];
                _Bag[shuffle] = temp;
            }
        }

        public int Roll()
        {
            int output = _Bag[Index];
            Index++;

            if (Index >= _Bag.Length)
            {
                Index = 0;
                Shuffle();
            }

            return output;
        }
    }

    public struct Dice : IRandomProvider
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
