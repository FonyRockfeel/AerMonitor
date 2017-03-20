namespace AermecNamespace
{
    using System;
    using System.Runtime.CompilerServices;

    public class ModbusMaster
    {
        private ushort addressRequest;
        private byte[] auchCRCHi = new byte[] { 
            0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41, 1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40,
            1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40, 0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41,
            1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40, 0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41,
            0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41, 1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40,
            1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40, 0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41,
            0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41, 1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40,
            0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41, 1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40,
            1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40, 0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41,
            1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40, 0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41,
            0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41, 1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40,
            0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41, 1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40,
            1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40, 0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41,
            0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41, 1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40,
            1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40, 0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41,
            1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40, 0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41,
            0, 0xc1, 0x81, 0x40, 1, 0xc0, 0x80, 0x41, 1, 0xc0, 0x80, 0x41, 0, 0xc1, 0x81, 0x40
        };
        private byte[] auchCRCLo = new byte[] { 
            0, 0xc0, 0xc1, 1, 0xc3, 3, 2, 0xc2, 0xc6, 6, 7, 0xc7, 5, 0xc5, 0xc4, 4,
            0xcc, 12, 13, 0xcd, 15, 0xcf, 0xce, 14, 10, 0xca, 0xcb, 11, 0xc9, 9, 8, 200,
            0xd8, 0x18, 0x19, 0xd9, 0x1b, 0xdb, 0xda, 0x1a, 30, 0xde, 0xdf, 0x1f, 0xdd, 0x1d, 0x1c, 220,
            20, 0xd4, 0xd5, 0x15, 0xd7, 0x17, 0x16, 0xd6, 210, 0x12, 0x13, 0xd3, 0x11, 0xd1, 0xd0, 0x10,
            240, 0x30, 0x31, 0xf1, 0x33, 0xf3, 0xf2, 50, 0x36, 0xf6, 0xf7, 0x37, 0xf5, 0x35, 0x34, 0xf4,
            60, 0xfc, 0xfd, 0x3d, 0xff, 0x3f, 0x3e, 0xfe, 250, 0x3a, 0x3b, 0xfb, 0x39, 0xf9, 0xf8, 0x38,
            40, 0xe8, 0xe9, 0x29, 0xeb, 0x2b, 0x2a, 0xea, 0xee, 0x2e, 0x2f, 0xef, 0x2d, 0xed, 0xec, 0x2c,
            0xe4, 0x24, 0x25, 0xe5, 0x27, 0xe7, 230, 0x26, 0x22, 0xe2, 0xe3, 0x23, 0xe1, 0x21, 0x20, 0xe0,
            160, 0x60, 0x61, 0xa1, 0x63, 0xa3, 0xa2, 0x62, 0x66, 0xa6, 0xa7, 0x67, 0xa5, 0x65, 100, 0xa4,
            0x6c, 0xac, 0xad, 0x6d, 0xaf, 0x6f, 110, 0xae, 170, 0x6a, 0x6b, 0xab, 0x69, 0xa9, 0xa8, 0x68,
            120, 0xb8, 0xb9, 0x79, 0xbb, 0x7b, 0x7a, 0xba, 190, 0x7e, 0x7f, 0xbf, 0x7d, 0xbd, 0xbc, 0x7c,
            180, 0x74, 0x75, 0xb5, 0x77, 0xb7, 0xb6, 0x76, 0x72, 0xb2, 0xb3, 0x73, 0xb1, 0x71, 0x70, 0xb0,
            80, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 150, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54,
            0x9c, 0x5c, 0x5d, 0x9d, 0x5f, 0x9f, 0x9e, 0x5e, 90, 0x9a, 0x9b, 0x5b, 0x99, 0x59, 0x58, 0x98,
            0x88, 0x48, 0x49, 0x89, 0x4b, 0x8b, 0x8a, 0x4a, 0x4e, 0x8e, 0x8f, 0x4f, 0x8d, 0x4d, 0x4c, 140,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 70, 0x86, 130, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80, 0x40
        };
        public short[] Coils = new short[1];
        public byte DeviceID;
        public short[] Registers = new short[1];
        private ushort sizeRequest;

        public AnswerCode Answer(byte[] buffer, ModBusCommand send, ushort sizeRequest)
        {
            int num3;
            int addressRequest;
            ushort num = 0xffff;
            if (buffer == null)
            {
                return AnswerCode.Timeout;
            }
            if (buffer.Length < 3)
            {
                return AnswerCode.AnswerTruncated;
            }
            switch (((ModBusCommand) buffer[1]))
            {
                case ModBusCommand.EXCEPTION_READ_COILS:
                case ModBusCommand.EXCEPTION_READ_REGISTER:
                    if (buffer[2] != 2)
                    {
                        if (buffer[2] == 1)
                        {
                            return AnswerCode.IllegalDataFunction;
                        }
                        if (buffer[2] == 3)
                        {
                            return AnswerCode.IllegalDataValue;
                        }
                        break;
                    }
                    return AnswerCode.IllegalDataAddress;
            }
            int num5 = buffer[2];
            if (buffer.Length < (num5 + 5))
            {
                return AnswerCode.AnswerTruncated;
            }
            for (num3 = 0; num3 < (num5 + 3); num3++)
            {
                this.crc_calc_and_ret(buffer[num3], ref num);
            }
            if ((((byte) num) != buffer[num5 + 4]) || (((byte) (num >> 8)) != buffer[num5 + 3]))
            {
                return AnswerCode.CRCError;
            }
            if (buffer[0] != this.DeviceID)
            {
                return AnswerCode.DeviceIDerror;
            }
            if (((ModBusCommand) buffer[1]) != send)
            {
                return AnswerCode.AnswerCodeNotEqualQueryCode;
            }
            switch (((ModBusCommand) buffer[1]))
            {
                case ModBusCommand.READ_COILS:
                {
                    ushort num6 = (ushort) (sizeRequest / 8);
                    if ((sizeRequest % 8) > 0)
                    {
                        num6 = (ushort) (num6 + 1);
                    }
                    if (num6 != num5)
                    {
                        return AnswerCode.PacketSizeError;
                    }
                    addressRequest = this.addressRequest;
                    num3 = 3;
                    while (num3 < (num5 + 3))
                    {
                        for (ushort i = 0; i < 8; i = (ushort) (i + 1))
                        {
                            this.Coils[addressRequest++] = (short) ((buffer[num3] >> i) & 1);
                            if (addressRequest >= (sizeRequest + this.addressRequest))
                            {
                                return AnswerCode.OK;
                            }
                        }
                        num3++;
                    }
                    return AnswerCode.OK;
                }
                case ModBusCommand.READ_HOLDING_REGISTERS:
                    if ((sizeRequest * 2) == num5)
                    {
                        addressRequest = this.addressRequest;
                        for (num3 = 3; num3 < (buffer[2] + 3); num3 += 2)
                        {
                            this.Registers[addressRequest++] = (short) ((buffer[num3] << 8) | buffer[num3 + 1]);
                        }
                        return AnswerCode.OK;
                    }
                    return AnswerCode.PacketSizeError;

                case ModBusCommand.READ_INPUT_REGISTERS:
                    if ((sizeRequest * 2) == num5)
                    {
                        addressRequest = this.addressRequest;
                        for (num3 = 3; num3 < (buffer[2] + 3); num3 += 2)
                        {
                            this.Registers[addressRequest++] = (short) ((buffer[num3] << 8) | buffer[num3 + 1]);
                        }
                        return AnswerCode.OK;
                    }
                    return AnswerCode.PacketSizeError;

                case ModBusCommand.WRITE_COILS:
                    return AnswerCode.OK;

                case ModBusCommand.WRITE_REGISTERS:
                    return AnswerCode.OK;
            }
            throw new Exception("Answer not decoded");
        }

        private void AutoResizeCoils(int newSize)
        {
            if (this.Coils.Length < newSize)
            {
                short[] array = new short[newSize];
                this.Coils.CopyTo(array, 0);
                this.Coils = array;
            }
        }

        private void AutoResizeRegisters(int newSize)
        {
            if (this.Registers.Length < newSize)
            {
                short[] array = new short[newSize];
                this.Registers.CopyTo(array, 0);
                this.Registers = array;
            }
        }

        public ModbusMaster Clone()
        {
            ModbusMaster master = new ModbusMaster {
                DeviceID = this.DeviceID,
                Registers = new short[this.Registers.Length]
            };
            this.Registers.CopyTo(master.Registers, 0);
            master.Coils = new short[this.Coils.Length];
            this.Coils.CopyTo(master.Coils, 0);
            return master;
        }

        public ushort crc_calc_and_ret(byte ch, ref ushort crc_reg)
        {
            byte num = (byte) (0xff & (crc_reg >> 8));
            byte num2 = (byte) (crc_reg & 0xff);
            ushort index = (ushort) (num ^ ch);
            num = (byte) (num2 ^ this.auchCRCHi[index]);
            num2 = this.auchCRCLo[index];
            crc_reg = (ushort) ((((ushort) (num << 8)) & 0xff00) | ((ushort) (0xff & num2)));
            return crc_reg;
        }

        public byte[] ReadCoils(ushort address, ushort size)
        {
            ushort num = 0xffff;
            byte[] buffer = new byte[8];
            buffer[0] = this.DeviceID;
            buffer[1] = 1;
            buffer[2] = (byte) (address >> 8);
            buffer[3] = (byte) address;
            buffer[4] = (byte) (size >> 8);
            buffer[5] = (byte) size;
            for (int i = 0; i < 6; i++)
            {
                this.crc_calc_and_ret(buffer[i], ref num);
            }
            buffer[6] = (byte) (num >> 8);
            buffer[7] = (byte) num;
            this.AutoResizeCoils(size + address);
            this.addressRequest = address;
            this.sizeRequest = size;
            return buffer;
        }

        public byte[] ReadHoldingRegisters(ushort address, ushort size)
        {
            byte[] buffer = new byte[8];
            ushort num = 0xffff;
            buffer[0] = this.DeviceID;
            buffer[1] = 3;
            buffer[2] = (byte) (address >> 8);
            buffer[3] = (byte) address;
            buffer[4] = (byte) (size >> 8);
            buffer[5] = (byte) size;
            for (int i = 0; i < 6; i++)
            {
                this.crc_calc_and_ret(buffer[i], ref num);
            }
            buffer[6] = (byte) (num >> 8);
            buffer[7] = (byte) num;
            this.addressRequest = address;
            this.sizeRequest = size;
            this.AutoResizeRegisters(size + address);
            return buffer;
        }

        public byte[] ReadInputRegisters(ushort address, ushort size)
        {
            byte[] buffer = new byte[8];
            ushort num = 0xffff;
            buffer[0] = this.DeviceID;
            buffer[1] = 4;
            buffer[2] = (byte) (address >> 8);
            buffer[3] = (byte) address;
            buffer[4] = (byte) (size >> 8);
            buffer[5] = (byte) size;
            for (int i = 0; i < 6; i++)
            {
                this.crc_calc_and_ret(buffer[i], ref num);
            }
            buffer[6] = (byte) (num >> 8);
            buffer[7] = (byte) num;
            this.addressRequest = address;
            this.sizeRequest = size;
            this.AutoResizeRegisters(size + address);
            return buffer;
        }

        public byte[] WriteCoils(ushort address, ushort size, ushort[] value)
        {
            return new byte[7];
        }

        public byte[] WriteRegisters(ushort address, ushort size, ushort[] value)
        {
            return new byte[7];
        }

        public byte[] WriteSingleCoil(ushort address, ushort value)
        {
            return new byte[7];
        }

        public byte[] WriteSingleRegister(ushort address, ushort value)
        {
            return new byte[7];
        }

        public enum AnswerCode
        {
            OK,
            CRCError,
            DeviceIDerror,
            IllegalDataAddress,
            IllegalDataFunction,
            IllegalDataValue,
            Timeout,
            AnswerTruncated,
            AnswerCodeNotEqualQueryCode,
            PacketSizeError
        }

        public delegate void AnswerDecoded(object sender, ModbusMaster.AnswerCode code);

        public delegate void AnswerReadCoils(object sender, ModbusMaster.AnswerCode code);

        public delegate void AnswerReadRegister(object sender, ModbusMaster.AnswerCode code);

        public delegate void AnswerWriteCoils(object sender, ModbusMaster.AnswerCode code);

        public delegate void AnswerWriteRegister(object sender, ModbusMaster.AnswerCode code);

        public enum ModBusCommand
        {
            EXCEPTION_READ_COILS = 0x81,
            EXCEPTION_READ_REGISTER = 0x83,
            READ_COILS = 1,
            READ_HOLDING_REGISTERS = 3,
            READ_INPUT_REGISTERS = 4,
            WRITE_COILS = 15,
            WRITE_REGISTERS = 0x10
        }

        private enum ModBusException
        {
            ILLEGAL_DATA_ADDRESS = 2,
            ILLEGAL_DATA_FUNCTION = 1,
            ILLEGAL_DATA_VALUE = 3
        }
    }
}

