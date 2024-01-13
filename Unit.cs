using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berserkers
{
    enum Races {Dracuri, Filrani, Morgoli}
    abstract class Unit
    {
        protected virtual int Damage { get; set; } = 10;
        protected virtual int HP { get; set; } = 100;
        protected virtual string Name { get; set; }
        protected virtual Races Race { get; set; }

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
    }

    abstract class RangedUnit : Unit
    {

    }

    abstract class MeleeUnit : Unit
    {

    }
}
