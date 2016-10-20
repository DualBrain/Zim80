﻿using System;
using Jacobi.Zim80.Components.CpuZ80.Opcodes;

namespace Jacobi.Zim80.Components.CpuZ80
{
    internal class Alu
    {
        private readonly Registers.RegisterSet _primarySet;

        public Alu(Registers.RegisterSet primarySet)
        {
            _primarySet = primarySet;
        }

        private Registers.Flags Flags
        {
            get { return _primarySet.Flags; }
        }

        public ushort Inc16(ushort value)
        {
            var newValue = (ushort)(value + 1);

            // no flags affected

            return newValue;
        }

        // Y/X flags not supported for (HL) and IX/IY +d
        // see undocumented (p15)
        public void TestBit(byte bit, byte value)
        {
            var isSet = (value & (1 << bit)) != 0;

            Flags.S = bit == 7 && isSet;
            Flags.Z = !isSet;
            Flags.Y = bit == 5 && isSet;
            Flags.H = true;
            Flags.X = bit == 3 && isSet;
            Flags.PV = !isSet;
            Flags.N = true;
        }

        public byte ResetBit(byte bit, byte value)
        {
            return (byte)(value & ~(1 << bit));
        }

        public byte SetBit(byte bit, byte value)
        {
            return (byte)(value | (1 << bit));
        }

        

        public ushort Dec16(ushort value)
        {
            var newValue = (ushort)(value - 1);

            // no flags affected

            return newValue;
        }

        public byte Inc8(byte value)
        {
            var newValue = (byte)(value + 1);

            Flags.S = IsNegative(newValue);
            Flags.PV = (value == 0x7E);
            Flags.Z = (newValue == 0);
            Flags.H = HalfCarryFromLo(value, newValue);
            Flags.N = false;

            return newValue;
        }

        public byte Dec8(byte value)
        {
            var newValue = (byte)(value - 1);

            Flags.S = IsNegative(newValue);
            Flags.PV = (value == 0x80);
            Flags.Z = (newValue == 0);
            Flags.H = HalfCarryFromHi(value, newValue);
            Flags.N = true;

            return newValue;
        }

        public ushort Add16(ushort acc, ushort value, bool addCarry = false)
        {
            int newValue = (int)acc + (int)value;

            if (addCarry && Flags.C) newValue++;

            Flags.C = newValue > 0xFFFF;
            Flags.H = HalfCarryFromLo((byte)(acc >> 8), (byte)(newValue >> 8));
            Flags.N = false;

            return (ushort)newValue;
        }

        public byte Add8(byte acc, byte value, bool addCarry = false)
        {
            var newValue = acc + value;
            if (addCarry && Flags.C) newValue++;

            Flags.S = IsNegative((byte)newValue);
            Flags.Z = newValue == 0;
            Flags.H = HalfCarryFromLo(acc, (byte)newValue);
            Flags.PV = IsOverflow(acc, value, newValue);
            Flags.N = false;
            Flags.C = (uint)newValue > 0xFF;

            return (byte)newValue;
        }

        public byte Sub8(byte acc, byte value, bool subCarry = false)
        {
            var newValue = acc - value;
            if (subCarry && Flags.C) newValue--;

            Flags.S = IsNegative((byte)newValue);
            Flags.Z = newValue == 0;
            Flags.H = HalfCarryFromHi(acc, (byte)newValue);
            Flags.PV = IsOverflow(acc, value, newValue);
            Flags.N = true;
            Flags.C = (uint)newValue > 0xFF;

            return (byte)newValue;
        }

        public byte And8(byte acc, byte value)
        {
            var newValue = acc & value;

            Flags.S = IsNegative((byte)newValue);
            Flags.Z = newValue == 0;
            Flags.H = true;
            Flags.PV = IsOverflow(acc, value, newValue);
            Flags.N = false;
            Flags.C = false;

            return (byte)newValue;
        }

        public byte Xor8(byte acc, byte value)
        {
            var newValue = acc ^ value;

            Flags.S = IsNegative((byte)newValue);
            Flags.Z = newValue == 0;
            Flags.H = false;
            Flags.PV = IsParityEven((byte)newValue);
            Flags.N = false;
            Flags.C = false;

            return (byte)newValue;
        }

        public byte Or8(byte acc, byte value)
        {
            var newValue = acc | value;

            Flags.S = IsNegative((byte)newValue);
            Flags.Z = newValue == 0;
            Flags.H = false;
            Flags.PV = IsOverflow(acc, value, newValue);
            Flags.N = false;
            Flags.C = false;

            return (byte)newValue;
        }

        // rlca
        public byte RotateLeftCarryA(byte acc)
        {
            var newValue = RotateValueLeftCarry(acc);

            Flags.H = false;
            Flags.N = false;

            return newValue;
        }

        // rrca
        public byte RotateRightCarryA(byte acc)
        {
            var newValue = RotateValueRightCarry(acc);

            Flags.H = false;
            Flags.N = false;

            return newValue;
        }

        // rla
        public byte RotateLeftA(byte acc)
        {
            var newValue = RotateValueLeft(acc);

            Flags.H = false;
            Flags.N = false;

            return newValue;
        }

        // rra
        public byte RotateRightA(byte acc)
        {
            var newValue = RotateValueRight(acc);

            Flags.H = false;
            Flags.N = false;

            return newValue;
        }

        // rlc
        public byte RotateLeftCarry(byte value)
        {
            var newValue = RotateValueLeftCarry(value);

            SetShiftRotateFlags(newValue);

            return newValue;
        }

        // rrc
        public byte RotateRightCarry(byte value)
        {
            var newValue = RotateValueRightCarry(value);

            SetShiftRotateFlags(newValue);

            return newValue;
        }

        // rl
        public byte RotateLeft(byte value)
        {
            var newValue = RotateValueLeft(value);

            SetShiftRotateFlags(newValue);

            return newValue;
        }

        // rr
        public byte RotateRight(byte value)
        {
            var newValue = RotateValueRight(value);

            SetShiftRotateFlags(newValue);

            return newValue;
        }

        

        // sla
        public byte ShiftLeftArithmetic(byte value)
        {
            var newValue = ShiftValueLeftCarry(value);

            SetShiftRotateFlags(newValue);

            return newValue;
        }

        // sra
        public byte ShiftRightArithmetic(byte value)
        {
            var s = (byte)(IsNegative(value) ? 0x80 : 0x00);
            var newValue = ShiftValueRightCarry(value);
            newValue |= s;
            
            SetShiftRotateFlags(newValue);

            return newValue;
        }

        // sll
        public byte ShiftLeftLogical(byte value)
        {
            var newValue = ShiftValueLeftCarry(value);

            SetShiftRotateFlags(newValue);

            return newValue;
        }

        // srl
        public byte ShiftRightLogical(byte value)
        {
            var newValue = ShiftValueRightCarry(value);

            SetShiftRotateFlags(newValue);

            return newValue;
        }

        private void SetShiftRotateFlags(byte newValue)
        {
            Flags.S = IsNegative(newValue);
            Flags.Z = newValue == 0;
            Flags.PV = IsParityEven(newValue);
            Flags.H = false;
            Flags.N = false;
        }

        private byte ShiftValueLeftCarry(byte value)
        {
            var newValue = value << 1;
            Flags.C = (newValue & 0x100) > 0;

            return (byte)newValue;
        }

        private byte ShiftValueRightCarry(byte value)
        {
            Flags.C = (value & 0x01) > 0;
            return (byte)(value >> 1);
        }

        private byte RotateValueLeftCarry(byte value)
        {
            var newValue = ShiftValueLeftCarry(value);

            if (Flags.C)
                newValue |= 1;

            return newValue;
        }

        private byte RotateValueRightCarry(byte value)
        {
            var newValue = ShiftValueRightCarry(value);

            if (Flags.C)
                newValue |= 0x80;

            return newValue;
        }

        private byte RotateValueLeft(byte value)
        {
            var c = Flags.C ? 0x01 : 0x00;
            var newValue = (value << 1);
            newValue |= c;
            Flags.C = (newValue & 0x100) > 0;

            return (byte)newValue;
        }

        private byte RotateValueRight(byte value)
        {
            var c = Flags.C ? 0x80 : 0x00;
            Flags.C = (value & 0x01) > 0;
            var newValue = (value >> 1);
            newValue |= c;

            return (byte)newValue;
        }

        private static bool IsParityEven(byte value)
        {
            bool oddParity = false;

            while (value != 0)
            {
                oddParity = !oddParity;
                value = (byte)(value & (value - 1));
            }

            return !oddParity;
        }

        private static bool IsOverflow(byte value1, byte value2, int newValue)
        {
            if (IsNegative(value1) && IsNegative(value2))
            {
                return newValue > 0;
            }

            if (!IsNegative(value1) && !IsNegative(value2))
            {
                return newValue < 0;
            }

            return false;
        }

        private static bool IsNegative(byte value)
        {
            return (value & 0x80) > 0;
        }

        private static bool HalfCarryFromHi(byte beforeValue, byte afterValue)
        {
            return (beforeValue & 0xF0) > (afterValue & 0xF0);
        }

        private static bool HalfCarryFromLo(byte beforeValue, byte afterValue)
        {
            return (beforeValue & 0xF0) < (afterValue & 0xF0);
        }

        public static UInt16 Add(UInt16 nn, sbyte d)
        {
            var value = (int)nn;
            value += d;
            return (UInt16)value;
        }
    }
}
