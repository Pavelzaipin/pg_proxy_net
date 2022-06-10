
namespace Netproxy
{


    // How it is written into the buffer: 
    // src\Npgsql\Internal\NpgsqlConnector.FrontendMessages.cs
    // src\Npgsql\PregeneratedMessages.cs

    // 'Q': 
    //   WriteBuffer.WriteByte(FrontendMessageCode.Query);

    // WriteBuffer.WriteByte(FrontendMessageCode.Query);
    // WriteBuffer.WriteInt32(
    // sizeof(int)  +        // Message length (including self excluding code)
    // queryByteLen +        // Query byte length
    // sizeof(byte));        // Null terminator
    // )

    // await WriteBuffer.WriteString(sql, queryByteLen, async, cancellationToken);
    // WriteBuffer.WriteByte(0);  // Null terminator


    public class NpgsqlReadBuffer
    {
        //  internal readonly byte[] Buffer;
        public readonly byte[]? Buffer;
        public readonly int FilledBytes;

        public int ReadPosition;

        internal int ReadBytesLeft
        {
            get
            {
                return FilledBytes - ReadPosition;
            }
        }


        internal System.Text.Encoding TextEncoding { get; }

        /// <summary>
        /// Same as <see cref="TextEncoding"/>, except that it does not throw an exception if an invalid char is
        /// encountered (exception fallback), but rather replaces it with a question mark character (replacement
        /// fallback).
        /// </summary>
        internal System.Text.Encoding RelaxedTextEncoding { get; }


        public NpgsqlReadBuffer(byte[] buffer, int len)
        {
            this.Buffer = buffer;
            this.FilledBytes = len;
            this.ReadPosition = 0;
            this.TextEncoding = System.Text.Encoding.UTF8;
            this.RelaxedTextEncoding = System.Text.Encoding.UTF8;
        }


        public NpgsqlReadBuffer(byte[] buffer)
            : this(buffer, buffer.Length)
        { }


        public void Skip(long len)
        {
            ReadPosition += (int)len;
        }

        public string? ReadNullTerminatedStringRelaxed()
        {
            return this.TextEncoding.GetString(this.Buffer!, 0, this.Buffer!.Length);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        static void ThrowNotSpaceLeft()
        {
            throw new System.InvalidOperationException("There is not enough space left in the buffer.");
        }



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        T Read<T>()
        {
            if (System.Runtime.CompilerServices.Unsafe.SizeOf<T>() > ReadBytesLeft)
                ThrowNotSpaceLeft();

            T? result = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<T>(ref Buffer![ReadPosition]);
            ReadPosition += System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
            return result;
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte() => Read<sbyte>();

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public byte ReadByte() => Read<byte>();


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public char ReadChar() => Read<char>();


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
       => ReadInt16(false);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public short ReadInt16(bool littleEndian)
        {
            short result = Read<short>();
            return littleEndian == System.BitConverter.IsLittleEndian
                ? result : System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(result);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
            => ReadUInt16(false);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16(bool littleEndian)
        {
            ushort result = Read<ushort>();
            return littleEndian == System.BitConverter.IsLittleEndian
                ? result : System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(result);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        { 
            return ReadInt32(false);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int ReadInt32(bool littleEndian)
        {
            int result = Read<int>();
            return littleEndian == System.BitConverter.IsLittleEndian
                ? result : System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(result);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
            => ReadUInt32(false);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32(bool littleEndian)
        {
            uint result = Read<uint>();
            return littleEndian == System.BitConverter.IsLittleEndian
                ? result : System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(result);
        }




        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
            => ReadInt64(false);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public long ReadInt64(bool littleEndian)
        {
            long result = Read<long>();
            return littleEndian == System.BitConverter.IsLittleEndian
                ? result : System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(result);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
            => ReadUInt64(false);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64(bool littleEndian)
        {
            ulong result = Read<ulong>();
            return littleEndian == System.BitConverter.IsLittleEndian
                ? result : System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(result);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public float ReadSingle()
            => ReadSingle(false);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public float ReadSingle(bool littleEndian)
        {
            int result = ReadInt32(littleEndian);
            return System.Runtime.CompilerServices.Unsafe.As<int, float>(ref result);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public double ReadDouble()
            => ReadDouble(false);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public double ReadDouble(bool littleEndian)
        {
            long result = ReadInt64(littleEndian);
            return System.Runtime.CompilerServices.Unsafe.As<long, double>(ref result);
        }


        public string ReadString(int byteLen)
        {
            System.Diagnostics.Debug.Assert(byteLen <= ReadBytesLeft);
            string? result = this.TextEncoding.GetString(Buffer!, ReadPosition, byteLen);
            ReadPosition += byteLen;
            return result;
        }

        public char[] ReadChars(int byteLen)
        {
            System.Diagnostics.Debug.Assert(byteLen <= ReadBytesLeft);
            char[]? result = TextEncoding.GetChars(Buffer!, ReadPosition, byteLen);
            ReadPosition += byteLen;
            return result;
        }

        public void ReadBytes(System.Span<byte> output)
        {
            System.Diagnostics.Debug.Assert(output.Length <= ReadBytesLeft);
            new System.Span<byte>(Buffer, ReadPosition, output.Length).CopyTo(output);
            ReadPosition += output.Length;
        }



        public void ReadBytes(byte[] output, int outputOffset, int len)
        => ReadBytes(new System.Span<byte>(output, outputOffset, len));

        public System.ReadOnlySpan<byte> ReadSpan(int len)
        {
            System.Diagnostics.Debug.Assert(len <= ReadBytesLeft);
            var span = new System.ReadOnlySpan<byte>(Buffer, ReadPosition, len);
            ReadPosition += len;
            return span;
        }

        public System.ReadOnlyMemory<byte> ReadMemory(int len)
        {
            System.Diagnostics.Debug.Assert(len <= ReadBytesLeft);
            var memory = new System.ReadOnlyMemory<byte>(Buffer, ReadPosition, len);
            ReadPosition += len;
            return memory;
        }

        /// <summary>
        /// Seeks the first null terminator (\0) and returns the string up to it. Reads additional data from the network if a null
        /// terminator isn't found in the buffered data.
        /// </summary>
        System.Threading.Tasks.ValueTask<string> ReadNullTerminatedString(System.Text.Encoding encoding, bool async, System.Threading.CancellationToken cancellationToken = default)
        {
            for (var i = ReadPosition; i < FilledBytes; i++)
            {
                if (Buffer![i] == 0)
                {
                    var byteLen = i - ReadPosition;
                    var result = new System.Threading.Tasks.ValueTask<string>(encoding.GetString(Buffer, ReadPosition, byteLen));
                    ReadPosition += byteLen + 1;
                    return result;
                }
            }

            return ReadLong(encoding, async);

            async System.Threading.Tasks.ValueTask<string> ReadLong(System.Text.Encoding encoding, bool async)
            {
                var chunkSize = FilledBytes - ReadPosition;
                var tempBuf = System.Buffers.ArrayPool<byte>.Shared.Rent(chunkSize + 1024);

                try
                {
                    bool foundTerminator;
                    var byteLen = chunkSize;
                    System.Array.Copy(Buffer, ReadPosition, tempBuf, 0, chunkSize);
                    ReadPosition += chunkSize;

                    do
                    {
                        // await ReadMore(async);
                        System.Diagnostics.Debug.Assert(ReadPosition == 0);

                        foundTerminator = false;
                        int i;
                        for (i = 0; i < FilledBytes; i++)
                        {
                            if (Buffer[i] == 0)
                            {
                                foundTerminator = true;
                                break;
                            }
                        }

                        if (byteLen + i > tempBuf.Length)
                        {
                            var newTempBuf = System.Buffers.ArrayPool<byte>.Shared.Rent(
                                foundTerminator ? byteLen + i : byteLen + i + 1024);

                            System.Array.Copy(tempBuf, 0, newTempBuf, 0, byteLen);
                            System.Buffers.ArrayPool<byte>.Shared.Return(tempBuf);
                            tempBuf = newTempBuf;
                        }

                        System.Array.Copy(Buffer, 0, tempBuf, byteLen, i);
                        byteLen += i;
                        ReadPosition = i;
                    } while (!foundTerminator);

                    ReadPosition++;
                    return encoding.GetString(tempBuf, 0, byteLen);
                }
                finally
                {
                    System.Buffers.ArrayPool<byte>.Shared.Return(tempBuf);
                }
            }
        }


    }

}
