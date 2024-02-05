using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Berserkers
{
    public enum Races {Dracuri, Filrani, Morgoli} //each race has a unique race ability. the Dracuri have natural armor, the Morgoli steal life when attacking, and the Filrani move faster.

    public enum Weather {Storm, Scorch, Void }

    public abstract class Unit
    {
        public virtual int CarryCapacity { get; protected set; } = 10;
        public abstract IRandomProvider HitChance { get; protected set; }
        public abstract IRandomProvider Damage { get; protected set; }
        public abstract IRandomProvider DefenceRating { get; protected set; }
        public virtual int HP { get; protected set; } = 100;
        public virtual string Name { get; protected set; } = "NamelessUnit";
        public virtual Races Race { get; protected set; }
        public virtual int Speed { get; protected set; } = 1;
        public virtual Vector2 Position { get; protected set; } = new Vector2(0,0);
        public static List<Weather> WeatherList { get; set; } = new();
        
        protected Unit() 
        {
            Random rnd = new Random();
            Position = new Vector2(rnd.Next(-10,10), rnd.Next(-10, 10));
        }
        public virtual void Attack(Unit otherUnit)
        {
            int AttackRoll = HitChance.Roll();
            int DefenseRoll = otherUnit.DefenceRating.Roll();
            if (WeatherList.Contains(Weather.Storm)) AttackRoll -= 5;

            if (AttackRoll >= DefenseRoll)
            {
                Console.WriteLine($"{this} hit {otherUnit} with a roll of {AttackRoll} against a roll of {DefenseRoll}");
                otherUnit.Defend(this);
                if (Race == Races.Morgoli) HP += 2;
            }
            else Console.WriteLine($"{this} missed {otherUnit} with a roll of {AttackRoll} against a roll of {DefenseRoll}");
        }

        public static void WeatherEffect(Weather weather)
        {
            WeatherList.Add(weather);
        }

        public static void EndWeatherEffect(Weather weather)
        {
            if (WeatherList.Contains(weather))
            {
                WeatherList.Remove(weather);
            }
        }

        public virtual void Defend(Unit otherUnit)
        {
            ApplyDamage(otherUnit.Damage.Roll());
        }

        protected void ApplyDamage(int damage)
        {
            if (Race == Races.Dracuri)
            {
                damage -= 2;
                if (damage < 1) damage = 1;
            }
            if (WeatherList.Contains(Weather.Void)) { damage *= 2; }
            HP -= damage;
            Console.WriteLine($"{this} was hit for {damage} damage and now has {HP} HP");
        }

        public virtual bool isAlive()
        {
            return HP > 0;
        }

        public Vector2 getPosition() { return Position; }

        public virtual void move(Vector2 target)
        {
            float distanceX = target.X - Position.X;
            float distanceY = target.Y - Position.Y;
            float filraniMOD = 0;
            if (Race == Races.Filrani) filraniMOD = 1;

            if (Math.Abs(distanceX) > Speed + filraniMOD)
            {
                distanceX = (Speed + filraniMOD) *Math.Sign(distanceX);
            }
            if (Math.Abs(distanceY) > Speed + filraniMOD)
            {
                distanceY = (Speed + filraniMOD) * Math.Sign(distanceY);
            }
            Vector2 OldPos = Position;
            Position += new Vector2(distanceX, distanceY);

            Console.WriteLine($"{Name} Moved from ({OldPos.X},{OldPos.Y}) to ({Position.X},{Position.Y})");

            if (WeatherList.Contains(Weather.Scorch))
            {
                ApplyDamage((int)distanceX + (int)distanceY);
            }


        }

        public float distance(Unit otherUnit)
        {
            return Math.Max(Math.Abs(otherUnit.Position.X - Position.X), Math.Abs(otherUnit.Position.Y - Position.Y));
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public abstract class RangedUnit : Unit
    {
        protected virtual float Range { get; set; } = 4;
        protected virtual float StormMod { get; set; } = 0;


        public override void Attack(Unit otherUnit)
        {
            if (WeatherList.Contains(Weather.Storm))
            {
                StormMod = -1;
            }
            if (distance(otherUnit) <= Range + StormMod)
            {
                base.Attack(otherUnit);
            }
            else
            {
                move(AdjustTargetForRange(otherUnit));
                if (distance(otherUnit) <= Range + StormMod)
                {
                    base.Attack(otherUnit);
                }
            }
            StormMod = 0;
        }
        protected Vector2 AdjustTargetForRange(Unit otherUnit)
        {
            float TargetX = otherUnit.getPosition().X;
            float TargetY = otherUnit.getPosition().Y;

            if (Math.Abs(TargetX-Position.X) > Range + StormMod)
            {
                TargetX = TargetX + (Range+StormMod)*Math.Sign(TargetX-Position.X);
            }
            else TargetX = Position.X;
            if (Math.Abs(TargetY-Position.Y) > Range + StormMod)
            {
                TargetY = TargetY + (Range + StormMod) * Math.Sign(TargetY - Position.Y);
            }
            else TargetY = Position.Y;

            return new Vector2(TargetX, TargetY);
        }

    }

    public abstract class MeleeUnit : Unit
    {
        public override void Attack(Unit otherUnit)
        {
            if (distance(otherUnit) <= 1)
            {
                base.Attack(otherUnit);
            }
            else
            {
                move(otherUnit.getPosition());
                if (distance(otherUnit) <= 1)
                {
                    base.Attack(otherUnit);
                }
            }
        }
    }

    #region Dracuri 
    // a race of scaled dragon-like people, who's worth is defined by their skill in combat. their skill is represented by a controlled bag random in their area of expertiese.

    public class DracuriArcher : RangedUnit // Ranged warrior skilled for close quarters - gains more damage agains melee targets.
    {
        private Dice MeleeDamage = new Dice(2,8,4);
        private Dice RangedDamage = new Dice(1,8,4);

        private static int UnitCount = 0;

        public override IRandomProvider HitChance { get; protected set; } = new Dice(1, 20, 7);
        public override IRandomProvider Damage { get; protected set; } = new Bag(6, 8, 10, 12, 12, 14);
        public override IRandomProvider DefenceRating { get; protected set; } = new Dice(1, 20, 0);

        public DracuriArcher()
        {
            Name = "DracuriArcher" + UnitCount;
            UnitCount++;
            Race = Races.Dracuri;
            Damage = RangedDamage;
            Console.WriteLine($"{Name} Has Joined The Fray at Position ({Position.X} , {Position.Y})");
        }

        public override void Attack(Unit otherUnit) //works because the unit won't get closer if the target is already whithin range.
        {
            
            if (this.distance(otherUnit) <= 1) Damage = MeleeDamage;
            base.Attack(otherUnit);
            Damage = RangedDamage;
        }
    }

    public class DracuriAssasin : MeleeUnit // draconic assasin, attacks twice for heavy damage
    {
        private static int UnitCount = 0;
        public override IRandomProvider HitChance { get; protected set; } = new Bag(11,13, 20, 23,28,29,30,30,31);
        public override IRandomProvider Damage { get; protected set; } = new Dice(3, 6, 6);
        public override IRandomProvider DefenceRating { get; protected set; } = new Dice(1, 20, 0);

        public DracuriAssasin()
        {
            Name = "DracuriAssasin" + UnitCount;
            UnitCount++;
            Race = Races.Dracuri;
            Console.WriteLine($"{Name} Has Joined The Fray at Position ({Position.X},{Position.Y})");
        }

        public override void Attack(Unit otherUnit)
        {
            base.Attack(otherUnit);
            int temp = Speed;
            Speed = 0;
            base.Attack(otherUnit);
            Speed = temp;

        }

    }

    public class DracuriJuggernaut : MeleeUnit // draconic warrior with spiked scales - fires back at the attacker if hit.
    {
        
        private bool Retributed = false;
        private Dice AttackDamage = new Dice(1,12,5);
        private Dice RetributionDamage = new Dice(1,6,0);

        public override IRandomProvider HitChance { get; protected set; } = new Dice(1, 20, 4);
        public override IRandomProvider Damage { get; protected set; } = new Dice(2, 12, 10);
        public override IRandomProvider DefenceRating { get; protected set; } = new Bag( 13, 15, 17, 20, 22, 25, 27);

        private static int UnitCount = 0;

        public DracuriJuggernaut()
        {
            Name = "DracuriJuggernaut" + UnitCount;
            UnitCount++;
            Race = Races.Dracuri;
            Damage = AttackDamage;
            Console.WriteLine($"{Name} Has Joined The Fray at Position ({Position.X},{Position.Y})");
        }

        public override void Defend(Unit otherUnit)
        {
            Damage = RetributionDamage;
            if (!Retributed)
            {
                Retributed = true;
                otherUnit.Defend(this);
            }
            Retributed = false;
            Damage = AttackDamage;
            base.Defend(otherUnit);
        }
    }
    #endregion

    #region Filrani
    // A race of animal-like people, adept at movement they make amazing hunters.

    public class FilraniDruid : RangedUnit // wise ranged caster - uses speed to it's advantage, and keeps distance from the target after firing.
    {
        private static int UnitCount = 0;
        public override IRandomProvider HitChance { get; protected set; } = new Dice(1, 20, 5);
        public override IRandomProvider Damage { get; protected set; } = new Dice(1, 8, 3);
        public override IRandomProvider DefenceRating { get; protected set; } = new Dice(1, 20, 3);



        public FilraniDruid()
        {
            Name = "FilraniDruid" + UnitCount;
            UnitCount++;
            Race = Races.Filrani;
            Console.WriteLine($"{Name} Has Joined The Fray at Position ({Position.X},{Position.Y})");
        }

        public override void Attack(Unit otherUnit)
        {
            base.Attack(otherUnit);
            if (distance(otherUnit) < Range - 1)
            {
                move(Position - (otherUnit.getPosition() - Position));
            }

        }
    }

    public class FilraniHunter : MeleeUnit // a warrior with bloodlust. when kills increases speed and damage, but looses the buff when stops killing. 
    {
        private bool KilledRecently = false;
        private Dice BaseDamage = new Dice(1,10,5);
        private int BaseSpeed;
        private uint DiceAmount = 1;
        private static int UnitCount = 0;

        public override IRandomProvider HitChance { get; protected set; } = new Dice(1, 20, 7);
        public override IRandomProvider Damage { get; protected set; } = new Dice(1, 10, 5);
        public override IRandomProvider DefenceRating { get; protected set; } = new Dice(1, 20, 3);

        public FilraniHunter()
        {
            Name = "FilraniHunter" + UnitCount;
            UnitCount++;
            Race = Races.Filrani;
            Damage = BaseDamage;
            BaseSpeed = Speed;
            Console.WriteLine($"{Name} Has Joined The Fray at Position ({Position.X},{Position.Y})");
        }
        public override void Attack(Unit otherUnit)
        {

            if (KilledRecently)
            {
                DiceAmount++;
                Damage = new Dice(DiceAmount,6,5);
                KilledRecently = false;
                Speed++;
            }
            base.Attack(otherUnit);

            if (!otherUnit.isAlive())
            {
                KilledRecently = true;
            }

            if (!KilledRecently)
            {
                DiceAmount = 1;
                Damage = BaseDamage;
                Speed = BaseSpeed;
            }
        }
    }

    public class FilraniWarden : MeleeUnit // really abnoxious tank. it's fast, and when it gets into melee, is erally hard to kill.
    {
        private static int UnitCount = 0;

        public override IRandomProvider HitChance { get; protected set; } = new Dice(1, 20, 5);
        public override IRandomProvider Damage { get; protected set; } = new Dice(2, 8, 8);
        public override IRandomProvider DefenceRating { get; protected set; } = new Dice(1, 20, 7);

        public FilraniWarden() 
        {
            Name = "FilraniWarden" + UnitCount;
            UnitCount++;
            Race = Races.Filrani;
            Console.WriteLine($"{Name} Has Joined The Fray at Position ({Position.X},{Position.Y})");
        }

        public override void Defend(Unit otherUnit)
        {
            int Delta = HP;
            base.Defend(otherUnit);
            Delta -= HP;

            if(distance(otherUnit) < 2) 
            {
                HP += (Delta * 3) / 4;
            }

        }
    }

    #endregion

    #region Morgoli 
    // The Morgoli are a race that lives and sustains itself by devouring the life force of other beings.

    public class MorgoliMage : RangedUnit //Heals Drastically when he kills.
    {
        private static int UnitCount = 0;
        public override IRandomProvider HitChance { get; protected set; } = new Dice(1, 20, 7);
        public override IRandomProvider Damage { get; protected set; } = new Dice(5, 8, 3);
        public override IRandomProvider DefenceRating { get; protected set; } = new Dice(1, 20, 0);
        public MorgoliMage()
        {
            Name = "MorgoliMage" + UnitCount;
            UnitCount++;
            Race = Races.Morgoli;
            Console.WriteLine($"{Name} Has Joined The Fray at Position ({Position.X},{Position.Y})");
        }
        public override void Attack(Unit otherUnit)
        {
            base.Attack(otherUnit);
            if (!otherUnit.isAlive())
            {
                HP += 30;
            }
        }
    }

    public class MorgoliSiphoner : RangedUnit //Attacks with many small attacks, but due to racial ability, heals a lot every attack. weak-ish against armored targets.
    {
        private static int UnitCount = 0;

        public override IRandomProvider HitChance { get; protected set; } = new Dice(1, 20, 5);
        public override IRandomProvider Damage { get; protected set; } = new Dice(1,4, 0);
        public override IRandomProvider DefenceRating { get; protected set; } = new Dice(1, 20, 3);
        public MorgoliSiphoner()
        {
            Name = "MorgoliSiphoner" + UnitCount;
            UnitCount++;
            Race = Races.Morgoli;
            Console.WriteLine($"{Name} Has Joined The Fray at Position ({Position.X},{Position.Y})");
        }

        public override void Attack(Unit otherUnit)
        {
            base.Attack(otherUnit);
            int temp = Speed;
            Speed = 0;
            base.Attack(otherUnit);
            base.Attack(otherUnit);
            base.Attack(otherUnit);
            base.Attack(otherUnit);
            Speed = temp;
        }
    }

    public class MorgoliHusk : MeleeUnit //undead soldier, has a high chance to resist dying.
    {
        Random rnd = new Random();
        private static int UnitCount = 0;
        public override IRandomProvider HitChance { get; protected set; } = new Dice(1, 20, 3);
        public override IRandomProvider Damage { get; protected set; } = new Dice(2, 6, 5);
        public override IRandomProvider DefenceRating { get; protected set; } = new Dice(1, 20, 5);
        public MorgoliHusk()
        {
            Name = "MorgoliHusk" + UnitCount;
            UnitCount++;
            Race = Races.Morgoli;
            Console.WriteLine($"{Name} Has Joined The Fray at Position ({Position.X},{Position.Y})");
        }
        

        public override void Defend(Unit otherUnit)
        {
            base.Defend(otherUnit);
            if (HP <= 0)
            {
                if (rnd.Next(0, 100) < 75) HP = 10;
            }
        }
    }
    #endregion
}
