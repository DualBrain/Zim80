﻿using Jacobi.Zim80.Components.CpuZ80.Opcodes;
using System;

namespace Jacobi.Zim80.Components.CpuZ80.States.Instructions
{
    internal class LoadImmediate16Instruction : ReadParametersInstruction
    {
        public LoadImmediate16Instruction(Die die)
            : base(die)
        { }

        protected override void OnLastCycleLastM()
        {
            var lsb = ExecutionEngine.MultiCycleOpcode.GetParameter(0);
            var msb = ExecutionEngine.MultiCycleOpcode.GetParameter(1);
            var value = OpcodeByte.MakeUInt16(lsb, msb);

            switch (ExecutionEngine.Opcode.Definition.Register16FromP)
            {
                case Opcodes.Register16Table.BC:
                    Registers.PrimarySet.BC = value;
                    break;
                case Opcodes.Register16Table.DE:
                    Registers.PrimarySet.DE = value;
                    break;
                case Opcodes.Register16Table.HL:
                    Registers.PrimarySet.HL = value;
                    break;
                case Opcodes.Register16Table.SP:
                    Registers.SP = value;
                    break;
            }
        }
    }
}
