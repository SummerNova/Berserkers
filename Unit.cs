using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Berserkers
{
    enum Races {Dracuri, Filrani, Morgoli}
    abstract class Unit
    {
        protected virtual int Damage { get; set; } = 10;
        protected virtual int HP { get; set; } = 100;
        protected virtual string Name { get; set; }
        protected virtual Races Race { get; set; }
        protected virtual int Speed { get; set; } = 1;
        protected virtual Vector2 Position { get; set; } = new Vector2(0,0);
        
        public virtual void Attack(Unit otherUnit)
        {
            otherUnit.Defend(this);
        }

        public abstract void Defend(Unit otherUnit);

        protected void ApplyDamage(int damage)
        {
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

            if (Math.Abs(distanceX) > Speed)
            {
                distanceX = Speed*Math.Sign(distanceX);
            }
            if (Math.Abs(distanceY) > Speed)
            {
                distanceY = Speed * Math.Sign(distanceY);
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
}
