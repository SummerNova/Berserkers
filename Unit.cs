using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Berserkers
{
    enum Races {Dracuri, Filrani, Morgoli} //each race has a unique race ability. the Dracuri have natural armor, the Morgoli steal life when attacking, and the Filrani move faster.
    abstract class Unit
    {
        protected virtual int Damage { get; set; } = 10;
        protected virtual int HP { get; set; } = 100;
        protected virtual string Name { get; set; }
        protected virtual Races Race { get; set; }
        protected virtual int Speed { get; set; } = 1;
        protected virtual Vector2 Position { get; set; } = new Vector2(0,0);
        
        protected Unit() 
        {
            Random rnd = new Random();
            Position = new Vector2(rnd.Next(-10,10), rnd.Next(-10, 10));
        }
        public virtual void Attack(Unit otherUnit)
        {
            otherUnit.Defend(this);
            if (Race == Races.Morgoli) HP += 2;
        }

        public virtual void Defend(Unit otherUnit)
        {
            ApplyDamage(otherUnit.Damage);
        }

        protected void ApplyDamage(int damage)
        {
            if (Race == Races.Dracuri)
            {
                damage -= 2;
                if (damage < 1) damage = 1;
            }
            HP -= damage;
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

            Position += new Vector2(distanceX, distanceY);
        }

        public float distance(Unit otherUnit)
        {
            return Math.Max(Math.Abs(otherUnit.Position.X - Position.X), Math.Abs(otherUnit.Position.Y - Position.Y));
        }
    }

    abstract class RangedUnit : Unit
    {
        protected virtual float Range { get; set; } = 3;


        public override void Attack(Unit otherUnit)
        {
            if (distance(otherUnit) <= Range)
            {
                base.Attack(otherUnit);
            }
            else
            {
                move(AdjustTargetForRange(otherUnit));
                if (distance(otherUnit) <= Range)
                {
                    base.Attack(otherUnit);
                }
            }
        }
        protected Vector2 AdjustTargetForRange(Unit otherUnit)
        {
            float TargetX = otherUnit.getPosition().X;
            float TargetY = otherUnit.getPosition().Y;

            if (Math.Abs(TargetX-Position.X) > Range)
            {
                TargetX = TargetX + Range*Math.Sign(TargetX-Position.X);
            }
            else TargetX = Position.X;
            if (Math.Abs(TargetY-Position.Y) > Range)
            {
                TargetY = TargetY + Range * Math.Sign(TargetY - Position.Y);
            }
            else TargetY = Position.Y;

            return new Vector2(TargetX, TargetY);
        }

    }

    abstract class MeleeUnit : Unit
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
    // a race of scaled dragon-like people, who's worth is defined by their skill in combat.

    class DracuriArcher : RangedUnit // Ranged warrior skilled for close quarters - gains more damage agains melee targets.
    {
        public DracuriArcher()
        {
            Race = Races.Dracuri;
        }

        public override void Attack(Unit otherUnit) //works because the unit won't get closer if the target is already whithin range.
        {
            if (this.distance(otherUnit) <= 1) Damage += 10;
            base.Attack(otherUnit);
            if (this.distance(otherUnit) <= 1) Damage -= 10;
        }
    }

    class DracuriAssasin : MeleeUnit // draconic assasin, attacks twice for heavy damage
    {
        public DracuriAssasin()
        {
            Race = Races.Dracuri;
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

    class DracuriJuggernaut : MeleeUnit // draconic warrior with spiked scales - fires back at the attacker if hit.
    {
        
        private bool Retributed = false;

        public DracuriJuggernaut()
        {
            Race = Races.Dracuri;
        }

        public override void Defend(Unit otherUnit)
        {
            int temp = Damage;
            Damage = 2;
            if (!Retributed)
            {
                Retributed = true;
                otherUnit.Defend(this);
            }
            Retributed = false;
            Damage = temp;
            base.Defend(otherUnit);
        }
    }
    #endregion

    #region Filrani
    // A race of animal-like people, adept at movement they make amazing hunters.

    class FilraniDruid : RangedUnit // wise ranged caster - uses speed to it's advantage, and keeps distance from the target after firing.
    {
        public FilraniDruid()
        {
            Race = Races.Filrani;
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

    class FilraniHunter : MeleeUnit // a warrior with bloodlust. when kills increases speed and damage, but looses the buff when stops killing. 
    {
        private bool KilledRecently = false;
        private int BaseDamage;
        private int BaseSpeed;

        public FilraniHunter()
        {
            Race = Races.Filrani;
            BaseDamage = Damage;
            BaseSpeed = Speed;
        }
        public override void Attack(Unit otherUnit)
        {

            if (KilledRecently)
            {
                Damage += 5;
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
                Damage = BaseDamage;
                Speed = BaseSpeed;
            }
        }
    }

    class FilraniWarden : MeleeUnit // really abnoxious tank. it's fast, and when it gets into melee, is erally hard to kill.
    {
        public FilraniWarden() 
        {
            Race = Races.Filrani;
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

    class MorgoliMage : RangedUnit //Heals Drastically when he kills.
    {
        public MorgoliMage()
        {
            Race = Races.Morgoli;
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

    class MorgoliSiphoner : RangedUnit //Attacks with many small attacks, but due to racial ability, heals a lot every attack. weak-ish against armored targets.
    {
        public MorgoliSiphoner()
        {
            Damage = 3;
            Race = Races.Morgoli;
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

    class MorgoliHusk : MeleeUnit //undead soldier, has a high chance to resist dying.
    {
        Random rnd = new Random();
        public MorgoliHusk()
        {
            Race = Races.Morgoli;
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
