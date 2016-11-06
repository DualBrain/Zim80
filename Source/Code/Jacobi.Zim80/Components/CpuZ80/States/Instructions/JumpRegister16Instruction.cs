﻿namespace Jacobi.Zim80.Components.CpuZ80.States.Instructions
{
    internal class JumpRegister16Instruction : SingleCycleInstruction
    {
        public JumpRegister16Instruction(Die die) 
            : base(die)
        { }

        protected override void OnLastCycleFirstM()
        {
            if (ExecutionEngine.Opcode.Definition.IsIX)
                Registers.PC = Registers.IX;
            else if (ExecutionEngine.Opcode.Definition.IsIY)
                Registers.PC = Registers.IY;
            else
                Registers.PC = Registers.PrimarySet.HL;
        }
    }
}
