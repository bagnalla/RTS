using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Threading;

namespace rts
{
    public class UnitCollisionSweeper
    {
        //int delay = 25;//, timeSinceLastSweep = 50;
        //List<Unit> units;
        public readonly Thread Thread;
        List<Unit> Units = new List<Unit>();

        public Unit[] PublicUnits;
        //public List<Unit>[] PublicCollisionLists;
        public List<List<Unit>> PublicCollisionLists;
        public Object PublicLock = new Object();

        public UnitCollisionSweeper()
        {
            Thread = new Thread(UpdatePotentialCollisions);
            Thread.IsBackground = true;
            Thread.Start();
        }

        void UpdatePotentialCollisions()
        {
            while (true)
            {
                Thread.Sleep(25);

                if (Unit.UnitsSorted.Count == 0)
                    continue;

                lock (Unit.UnitsSorted)
                {
                    Units = new List<Unit>(Unit.UnitsSorted.ToArray<Unit>());
                }

                SortByX(Units);

                List<int[]> pairs = new List<int[]>();

                for (int i = 0; i < Units.Count; i++)
                {
                    Unit object1 = Units[i];
                    if (object1.IgnoringCollision)
                        continue;

                    for (int s = i + 1; s < Units.Count; s++)
                    {
                        Unit object2 = Units[s];
                        if (object2.IgnoringCollision)
                            continue;

                        if (object2.RightBound < object1.LeftBound)
                            continue;

                        if (object2.LeftBound > object1.RightBound)
                            break;

                        if (object2.TopBound <= object1.BottomBound &&
                            object2.BottomBound >= object1.TopBound)
                            pairs.Add(new int[2] { i, s });
                    }
                }

                List<List<Unit>> collisionLists = new List<List<Unit>>();

                for (int i = 0; i < Units.Count; i++)
                    collisionLists.Add(new List<Unit>());

                foreach (int[] pair in pairs)
                {
                    collisionLists[pair[0]].Add(Units[pair[1]]);
                    collisionLists[pair[1]].Add(Units[pair[0]]);
                }

                lock (PublicLock)
                {
                    PublicUnits = Units.ToArray<Unit>();
                    PublicCollisionLists = collisionLists;//.ToArray<List<Unit>>();
                }
            }
        }

        public void FulFillCollisionLists()
        {
            if (PublicUnits == null)
                return;

            lock (PublicLock)
            {
                for (int i = 0; i < PublicUnits.Length; i++)
                {
                    Unit unit = PublicUnits[i];

                    unit.ClearPotentialCollisions();

                    foreach (Unit u in PublicCollisionLists[i])
                        unit.AddPotentialCollision(u);
                }
                PublicUnits = null;
            }
        }

        static void SortByX(List<Unit> list)
        {
            //lock (Unit.UnitsSorted)
            //{
            for (int i = 1; i < list.Count; i++)
            {
                for (int j = i; j > 1 && list[j].X < list[j - 1].X; j--)
                {
                    Unit tempItem = list[j];
                    list.RemoveAt(j);
                    list.Insert(j - 1, tempItem);
                }
            }
            //}
        }
    }
}