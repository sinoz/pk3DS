﻿using System;
using System.IO;
using System.Linq;

namespace pk3DS
{
    public class EvolutionMethod
    {
        public int Method;
        public int Species;
        public int Argument;
        public int Form = -1;
        public int Level;
    }

    public abstract class EvolutionSet
    {
        public EvolutionMethod[] PossibleEvolutions;
        public abstract byte[] Write();
    }
    public class EvolutionSet6 : EvolutionSet
    {
        private const int SIZE = 6;
        public EvolutionSet6(byte[] data)
        {
            if (data.Length < SIZE || data.Length % SIZE != 0) return;
            int[] argEvos = { 6, 8, 16, 17, 18, 19, 20, 21, 22, 29, 30, 31, 32, 33, 34 };
            PossibleEvolutions = new EvolutionMethod[data.Length / SIZE];
            for (int i = 0; i < data.Length; i += SIZE)
            {
                PossibleEvolutions[i / SIZE] = new EvolutionMethod
                {
                    Method = BitConverter.ToUInt16(data, i + 0),
                    Argument = BitConverter.ToUInt16(data, i + 2),
                    Species = BitConverter.ToUInt16(data, i + 4),

                    // Copy
                    Level = BitConverter.ToUInt16(data, i + 2),
                };

                // Argument is used by both Level argument and Item/Move/etc. Clear if appropriate.
                if (argEvos.Contains(PossibleEvolutions[i / SIZE].Method))
                    PossibleEvolutions[i / SIZE].Level = 0;
            }
        }
        public override byte[] Write()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach (EvolutionMethod evo in PossibleEvolutions)
                {
                    bw.Write((ushort)evo.Method);
                    bw.Write((ushort)evo.Argument);
                    bw.Write((ushort)evo.Species);
                }
                return ms.ToArray();
            }
        }
    }
    public class EvolutionSet7 : EvolutionSet
    {
        private const int SIZE = 8;
        public EvolutionSet7(byte[] data)
        {
            if (data.Length < SIZE || data.Length % SIZE != 0) return;
            PossibleEvolutions = new EvolutionMethod[data.Length / SIZE];
            for (int i = 0; i < data.Length; i += SIZE)
            {
                PossibleEvolutions[i / SIZE] = new EvolutionMethod
                {
                    Method = BitConverter.ToUInt16(data, i + 0),
                    Argument = BitConverter.ToUInt16(data, i + 2),
                    Species = BitConverter.ToUInt16(data, i + 4),
                    Form = (sbyte)data[i + 6],
                    Level = data[i + 7],
                };
            }
        }
        public override byte[] Write()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach (EvolutionMethod evo in PossibleEvolutions)
                {
                    bw.Write((ushort)evo.Method);
                    bw.Write((ushort)evo.Argument);
                    bw.Write((ushort)evo.Species);
                    bw.Write((sbyte)evo.Form);
                    bw.Write((byte)evo.Level);
                }
                return ms.ToArray();
            }
        }
    }
}