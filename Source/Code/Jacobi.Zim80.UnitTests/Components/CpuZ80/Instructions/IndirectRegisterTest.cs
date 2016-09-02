﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jacobi.Zim80.Model;
using Jacobi.Zim80.Components.CpuZ80.Opcodes;
using Jacobi.Zim80.Components.Memory;
using Jacobi.Zim80.Components.CpuZ80.UnitTests;
using Jacobi.Zim80.Components.Memory.UnitTests;

namespace Jacobi.Zim80.Components.CpuZ80.Instructions.UnitTests
{
    [TestClass]
    public class IndirectRegisterTest
    {
        [TestMethod]
        public void IncHL()
        {
            var ob = OpcodeByte.New(z: 4, y: 6);

            var model = ExecuteTest(ob, (m) => {
                m.Cpu.FillRegisters(hl: 0x10);
                var writer = new MemoryWriter<BusData16, BusData8>(m.Memory);
                writer[new BusData16(0x10)] = new BusData8(0xAA);
            });

            model.Memory.Assert(0x10, 0xAB);
        }

        [TestMethod]
        public void DecHL()
        {
            var ob = OpcodeByte.New(z: 5, y: 6);

            var model = ExecuteTest(ob, (m) => {
                m.Cpu.FillRegisters(hl: 0x10);
                var writer = new MemoryWriter<BusData16, BusData8>(m.Memory);
                writer[new BusData16(0x10)] = new BusData8(0xAA);
            });

            model.Memory.Assert(0x10, 0xA9);
        }

        private static SimulationModel ExecuteTest(OpcodeByte ob,
                Action<SimulationModel> preTest, byte extension = 0)
        {
            var cpuZ80 = new CpuZ80();
            var model = cpuZ80.Initialize(null);

            var writer = new MemoryWriter<BusData16, BusData8>(model.Memory);
            writer.Fill(0x48, new BusData8(0));

            if (extension == 0)
                writer[new BusData16(0)] = new BusData8(ob.Value);
            else
            {
                writer[new BusData16(0)] = new BusData8(extension);
                writer[new BusData16(1)] = new BusData8(ob.Value);
            }

            preTest(model);

            model.ClockGen.BlockWave(extension == 0 ? 11 : 15);

            return model;
        }
    }
}