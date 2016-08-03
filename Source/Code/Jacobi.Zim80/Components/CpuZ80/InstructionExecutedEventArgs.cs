﻿using Jacobi.Zim80.Components.CpuZ80.Opcodes;
using System;

namespace Jacobi.Zim80.Components.CpuZ80
{
    public class InstructionExecutedEventArgs : EventArgs
    {
        public InstructionExecutedEventArgs(Opcode opcode)
        {
            Opcode = opcode;
        }

        public Opcode Opcode { get; private set; }
    }
}