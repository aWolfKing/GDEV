using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class UnitStatGrowths {

        private CFloat  health = null,
                        strenght = null,
                        magic = null,
                        skill = null,
                        speed = null,
                        luck = null,
                        defence = null,
                        resistance = null,
                        movement = null;



        public CFloat Health {
            get{
                return this.health;
            }
        }

        public CFloat Strenght {
            get{
                return this.strenght;
            }
        }

        public CFloat Magic {
            get{
                return this.magic;
            }
        }

        public CFloat Skill {
            get{
                return this.skill;
            }
        }

        public CFloat Speed {
            get{
                return this.speed;
            }
        }

        public CFloat Luck {
            get{
                return this.luck;
            }
        }

        public CFloat Defence {
            get{
                return this.defence;
            }
        }

        public CFloat Resistance {
            get{
                return this.resistance;
            }
        }

        public CFloat Movement {
            get{
                return this.movement;
            }
        }



        public UnitStatGrowths(float health, float strenght, float magic, float skill, float speed, float luck, float defence, float resistance){
            this.health = new CFloat(health);
            this.strenght = new CFloat(strenght);
            this.magic = new CFloat(magic);
            this.skill = new CFloat(skill);
            this.speed = new CFloat(speed);
            this.luck = new CFloat(luck);
            this.defence = new CFloat(defence);
            this.resistance = new CFloat(resistance);
            this.movement = new CFloat(0);
        }

        public UnitStatGrowths(){
            this.health = new CFloat();
            this.strenght = new CFloat();
            this.magic = new CFloat();
            this.skill = new CFloat();
            this.speed = new CFloat();
            this.luck = new CFloat();
            this.defence = new CFloat();
            this.resistance = new CFloat();
            this.movement = new CFloat();
        }

        public UnitStatGrowths(float health, float strenght, float magic, float skill, float speed, float luck, float defence, float resistance, float movement) {
            this.health = new CFloat(health);
            this.strenght = new CFloat(strenght);
            this.magic = new CFloat(magic);
            this.skill = new CFloat(skill);
            this.speed = new CFloat(speed);
            this.luck = new CFloat(luck);
            this.defence = new CFloat(defence);
            this.resistance = new CFloat(resistance);
            this.movement = new CFloat(movement);
        }

    }

}
