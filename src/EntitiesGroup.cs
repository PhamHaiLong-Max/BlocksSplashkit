using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame
{
    public class EntitiesGroup
    {
        private List<GameObject> _entities = new List<GameObject>();

        public EntitiesGroup()
        { }

        public List<GameObject> EntitiesList
        {
            get
            {
                return _entities;
            }
            set
            {
                _entities = value;
            }
        }

        public void AddObject(GameObject a)
        {
            EntitiesList.Add(a);
        }

        public void Display()
        {
            foreach(GameObject a in _entities)
            {
                a.DisplayItself();
            }
        }

        public void DeathCheck()
        {
            foreach (MovableGameObject a in EntitiesList.ToList())
            {
                if (a.HP <= 0)
                {
                    EntitiesList.Remove(a);
                }
            }
        }
    }
}
