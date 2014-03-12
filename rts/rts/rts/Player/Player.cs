using System.Collections.Generic;
using System.Linq;

namespace rts
{
    public class Player
    {
        public static Player[] Players = new Player[2];

        // Me is assigned by Rts gamestate constructor
        public static Player Me;

        public PlayerStats Stats = new PlayerStats();

        static Player()
        {
            for (short i = 0; i < Players.Length; i++)
            {
                Players[i] = new Player(i);
            }
        }

        public short Team;
        public int Roks;
        public int CurrentSupply;
        public int MaxSupply;

        public List<ScheduledAction> ScheduledActions = new List<ScheduledAction>();

        public Unit[] UnitArray = new Unit[2048];
        public Structure[] StructureArray = new Structure[2048];
        public List<UnitCommand> UnitCommands = new List<UnitCommand>();

        //public short UnitIDCounter;
        public short CommandIDCounter;
        public short StructureIDCounter;

        // units or structures that have died to be set null in arrays after some delay
        public List<KeyValuePair<short, float>> UnitIDsToSetNull = new List<KeyValuePair<short, float>>();
        public List<KeyValuePair<short, float>> StructureIDsToSetNull = new List<KeyValuePair<short, float>>();

        Player(short team)
        {
            Team = team;
        }

        public void AddUnitIDToSetNull(short unitID, float currentClockTime)
        {
            UnitIDsToSetNull.Add(new KeyValuePair<short, float>(unitID, currentClockTime));
        }

        // set IDs to null references if theyve reached delay time
        const float NULL_ID_DELAY = 5f;
        public static void SetNullIDS()
        {
            foreach (Player player in Players)
            {
                for (int i = 0; i < player.UnitIDsToSetNull.Count;)
                {
                    var pair = player.UnitIDsToSetNull[i];

                    if (Rts.GameClock >= pair.Value + NULL_ID_DELAY)
                    {
                        player.UnitArray[pair.Key] = null;
                        player.UnitIDsToSetNull.Remove(pair);
                    }
                    else
                        i++;
                }

                for (int i = 0; i < player.StructureIDsToSetNull.Count; )
                {
                    var pair = player.StructureIDsToSetNull[i];

                    if (Rts.GameClock >= pair.Value + NULL_ID_DELAY)
                    {
                        player.StructureArray[pair.Key] = null;
                        player.StructureIDsToSetNull.Remove(pair);
                    }
                    else
                        i++;
                }
            }
        }

        // ignores targeted commands, which is currently only rally point commands
        public int CountScheduledStructureCommands(Structure structure)
        {
            int count = 0;
            foreach (ScheduledAction action in ScheduledActions)
            {
                ScheduledStructureCommand structureCommand = action as ScheduledStructureCommand;
                if (structureCommand != null && !(structureCommand is ScheduledStructureTargetedCommand) && structureCommand.Structure == structure)
                    count++;
            }
            return count;
        }

        // returns the index to the first free space in the unit array
        // and places dummy unit in that space
        // returns -1 if no free space
        public short NextFreeUnitID()
        {
            for (int i = 0; i < UnitArray.Length; i++)
            {
                if (UnitArray[i] == null)
                {
                    UnitArray[i] = Unit.Reserved;
                    return (short)i;
                }
            }
            return -1;
        }
    }
}
