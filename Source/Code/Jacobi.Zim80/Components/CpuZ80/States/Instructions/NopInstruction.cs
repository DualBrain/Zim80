﻿using System;

namespace Jacobi.Zim80.Components.CpuZ80.States.Instructions
{
    internal class NopInstruction : SingleCycleInstruction
    {
        public NopInstruction(Die die)
            : base(die)
        { }

        protected override void OnLastCycleFirstM()
        {
            // no operation
        }
    }
}
