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
        protected abstract Races Race { get; set; }
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

        public abstract void Defend(Unit otherUnit);

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

    class DracuriArcher : RangedUnit
    {
        protected override Races Race { get; set; } = Races.Dracuri;

        public override void Defend(Unit otherUnit)
        {
            throw new NotImplementedException();
        }
    }

    class DracuriAssasin : MeleeUnit
    {
        protected override Races Race { get; set; } = Races.Dracuri;

        public override void Defend(Unit otherUnit)
        {
            throw new NotImplementedException();
        }
    }

    class DracuriTank : MeleeUnit
    {
        protected override Races Race { get; set; } = Races.Dracuri;

        public override void Defend(Unit otherUnit)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Filrani
    // A race of animal-like people, adept at movement they make amazing hunters.

    class FilraniDruid : RangedUnit
    {
        protected override Races Race { get; set; } = Races.Filrani;

        public override void Defend(Unit otherUnit)
        {
            throw new NotImplementedException();
        }
    }

    class FilraniHunter : MeleeUnit
    {
        protected override Races Race { get; set; } = Races.Filrani;

        public override void Defend(Unit otherUnit)
        {
            throw new NotImplementedException();
        }
    }

    class FilraniWarden : MeleeUnit
    {
        protected override Races Race { get; set; } = Races.Filrani;

        public override void Defend(Unit otherUnit)
        {
            throw new NotImplementedException();
        }
    }

    #endregion



    #region Morgoli 
    // The Morgoli are a race that lives and sustains itself by devouring the life force of other beings.

    class MorgoliMage : RangedUnit
    {
        protected override Races Race { get; set; } = Races.Morgoli;

        public override void Defend(Unit otherUnit)
        {
            throw new NotImplementedException();
        }
    }

    class MorgoliSiphoner : RangedUnit
    {
        protected override Races Race { get; set; } = Races.Morgoli;

        public override void Defend(Unit otherUnit)
        {
            throw new NotImplementedException();
        }
    }

    class MorgoliHusk : MeleeUnit
    {
        protected override Races Race { get; set; } = Races.Morgoli;

        public override void Defend(Unit otherUnit)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
