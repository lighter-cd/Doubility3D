/*
 * Copyright 2014 Google Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using System;
using System.Text;

/// @file
/// @addtogroup flatbuffers_csharp_api
/// @{

namespace FlatBuffers
{
    /// <summary>
    /// Responsible for building up and accessing a FlatBuffer formatted byte
    /// array (via ByteBuffer).
    /// </summary>
    public class FlatBufferBuilder
    {
        private int _space;
        private ByteBuffer _bb;
        private int _minAlign = 1;

        // The vtable for the current table (if _vtableSize >= 0)
        private int[] _vtable = new int[16];
        // The size of the vtable. -1 indicates no vtable
        private int _vtableSize = -1;
        // Starting offset of the current struct/table.
        private int _objectStart;
        // List of offsets of all vtables.
        private int[] _vtables = new int[16];
        // Number of entries in `vtables` in use.
        private int _numVtables = 0;
        // For the current vector being built.
        private int _vectorNumElems = 0;

        /// <summary>
        /// Create a FlatBufferBuilder with a given initial size.
        /// </summary>
        /// <param name="initialSize">
        /// The initial size to use for the internal buffer.
        /// </param>
        public FlatBufferBuilder(int initialSize)
        {
            if (initialSize <= 0)
                throw new ArgumentOutOfRangeException("initialSize",
                    initialSize, "Must be greater than zero");
            _space = initialSize;
            _bb = new ByteBuffer(new byte[initialSize]);
        }

        /// <summary>
        /// Reset the FlatBufferBuilder by purging all data that it holds.
        /// </summary>
        public void Clear()
        {
            _space = _bb.Length;
            _bb.Reset();
            _minAlign = 1;
            while (_vtableSize > 0) _vtable[--_vtableSize] = 0;
            _vtableSize = -1;
            _objectStart = 0;
            _numVtables = 0;
            _vectorNumElems = 0;
        }

        /// <summary>
        /// Gets and sets a Boolean to disable the optimization when serializing
        /// default values to a Table.
        /// 
        /// In order to save space, fields that are set to their default value
        /// don't get serialized into the buffer. 
        /// </summary>
        public bool ForceDefaults { get; set; }

        /// @cond FLATBUFFERS_INTERNAL

        public int Offset { get { return _bb.Length - _space; } }

        public void Pad(int size)
        {
             _bb.PutByte(_space -= size, 0, size);
        }

        // Doubles the size of the ByteBuffer, and copies the old data towards
        // the end of the new buffer (since we build the buffer backwards).
        void GrowBuffer()
        {
            var oldBuf = _bb.Data;
            var oldBufSize = oldBuf.Length;
            if ((oldBufSize & 0xC0000000) != 0)
                throw new Exception(
                    "FlatBuffers: cannot grow buffer beyond 2 gigabytes.");

            var newBufSize = oldBufSize << 1;
            var newBuf = new byte[newBufSize];

            Buffer.BlockCopy(oldBuf, 0, newBuf, newBufSize - oldBufSize,
                             oldBufSize);
            _bb = new ByteBuffer(newBuf, newBufSize);
        }

        // Prepare to write an element of `size` after `additional_bytes`
        // have been written, e.g. if you write a string, you need to align
        // such the int length field is aligned to SIZEOF_INT, and the string
        // data follows it directly.
        // If all you need to do is align, `additional_bytes` will be 0.
        public void Prep(int size, int additionalBytes)
        {
            // Track the biggest thing we've ever aligned to.
            if (size > _minAlign)
                _minAlign = size;
            // Find the amount of alignment needed such that `size` is properly
            // aligned after `additional_bytes`
            var alignSize =
                ((~((int)_bb.Length - _space + additionalBytes)) + 1) &
                (size - 1);
            // Reallocate the buffer if needed.
            while (_space < alignSize + size + additionalBytes)
            {
                var oldBufSize = (int)_bb.Length;
                GrowBuffer();
                _space += (int)_bb.Length - oldBufSize;

            }
            if (alignSize > 0)
                Pad(alignSize);
        }

        public void PutBool(bool x)
        {
          _bb.PutByte(_space -= sizeof(byte), (byte)(x ? 1 : 0));
        }

        public void PutSbyte(sbyte x)
        {
          _bb.PutSbyte(_space -= sizeof(sbyte), x);
        }

        public void PutByte(byte x)
        {
            _bb.PutByte(_space -= sizeof(byte), x);
        }

        public void PutShort(short x)
        {
            _bb.PutShort(_space -= sizeof(short), x);
        }

        public void PutUshort(ushort x)
        {
          _bb.PutUshort(_space -= sizeof(ushort), x);
        }

        public void PutInt(int x)
        {
            _bb.PutInt(_space -= sizeof(int), x);
        }

        public void PutUint(uint x)
        {
          _bb.PutUint(_space -= sizeof(uint), x);
        }

        public void PutLong(long x)
        {
            _bb.PutLong(_space -= sizeof(long), x);
        }

        public void PutUlong(ulong x)
        {
          _bb.PutUlong(_space -= sizeof(ulong), x);
        }

        public void PutFloat(float x)
        {
            _bb.PutFloat(_space -= sizeof(float), x);
        }

        public void PutDouble(double x)
        {
            _bb.PutDouble(_space -= sizeof(double), x);
        }
        /// @endcond

        /// <summary>
        /// Add a `bool` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `bool` to add to the buffer.</param>
        public void AddBool(bool x) { Prep(sizeof(byte), 0); PutBool(x); }

        /// <summary>
        /// Add a `sbyte` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `sbyte` to add to the buffer.</param>
        public void AddSbyte(sbyte x) { Prep(sizeof(sbyte), 0); PutSbyte(x); }

        /// <summary>
        /// Add a `byte` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `byte` to add to the buffer.</param>
        public void AddByte(byte x) { Prep(sizeof(byte), 0); PutByte(x); }

        /// <summary>
        /// Add a `short` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `short` to add to the buffer.</param>
        public void AddShort(short x) { Prep(sizeof(short), 0); PutShort(x); }

        /// <summary>
        /// Add an `ushort` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `ushort` to add to the buffer.</param>
        public void AddUshort(ushort x) { Prep(sizeof(ushort), 0); PutUshort(x); }

        /// <summary>
        /// Add an `int` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `int` to add to the buffer.</param>
        public void AddInt(int x) { Prep(sizeof(int), 0); PutInt(x); }

        /// <summary>
        /// Add an `uint` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `uint` to add to the buffer.</param>
        public void AddUint(uint x) { Prep(sizeof(uint), 0); PutUint(x); }

        /// <summary>
        /// Add a `long` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `long` to add to the buffer.</param>
        public void AddLong(long x) { Prep(sizeof(long), 0); PutLong(x); }

        /// <summary>
        /// Add an `ulong` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `ulong` to add to the buffer.</param>
        public void AddUlong(ulong x) { Prep(sizeof(ulong), 0); PutUlong(x); }

        /// <summary>
        /// Add a `float` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `float` to add to the buffer.</param>
        public void AddFloat(float x) { Prep(sizeof(float), 0); PutFloat(x); }

        /// <summary>
        /// Add a `double` to the buffer (aligns the data and grows if necessary).
        /// </summary>
        /// <param name="x">The `double` to add to the buffer.</param>
        public void AddDouble(double x) { Prep(sizeof(double), 0);
                                          PutDouble(x); }

        /// <summary>
        /// Adds an offset, relative to where it will be written.
        /// </summary>
        /// <param name="off">The offset to add to the buffer.</param>
        public void AddOffset(int off)
        {
            Prep(sizeof(int), 0);  // Ensure alignment is already done.
            if (off > Offset)
                throw new ArgumentException();

            off = Offset - off + sizeof(int);
            PutInt(off);
        }

        /// @cond FLATBUFFERS_INTERNAL
        public void StartVector(int elemSize, int count, int alignment)
        {
            NotNested();
            _vectorNumElems = count;
            Prep(sizeof(int), elemSize * count);
            Prep(alignment, elemSize * count); // Just in case alignment > int.
        }
        /// @endcond

        /// <summary>
        /// Writes data necessary to finish a vector construction.
        /// </summary>
        public VectorOffset EndVector()
        {
            Prep(sizeof(int), 1);
            PutInt(_vectorNumElems);
            return new VectorOffset(Offset);
        }

        /// @cond FLATBUFFERS_INTENRAL
        public void Nested(int obj)
        {
            // Structs are always stored inline, so need to be created right
            // where they are used. You'll get this assert if you created it
            // elsewhere.
            if (obj != Offset)
                throw new Exception(
                    "FlatBuffers: struct must be serialized inline.");
        }

        public void NotNested()
        {
            // You should not be creating any other objects or strings/vectors
            // while an object is being constructed
            if (_vtableSize >= 0)
                throw new Exception(
                    "FlatBuffers: object serialization must not be nested.");
        }

        public void StartObject(int numfields)
        {
            if (numfields < 0)
                throw new ArgumentOutOfRangeException("Flatbuffers: invalid numfields");

            NotNested();

            if (_vtable.Length < numfields)
                _vtable = new int[numfields];

            _vtableSize = numfields;
            _objectStart = Offset;
        }


        // Set the current vtable at `voffset` to the current location in the
        // buffer.
        public void Slot(int voffset)
        {
            if (voffset >= _vtableSize)
                throw new IndexOutOfRangeException("Flatbuffers: invalid voffset");

            _vtable[voffset] = Offset;
        }

        /// <summary>
        /// Adds a Boolean to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddBool(int o, bool x, bool d) { if (ForceDefaults || x != d) { AddBool(x); Slot(o); } }
        
        /// <summary>
        /// Adds a SByte to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddSbyte(int o, sbyte x, sbyte d) { if (ForceDefaults || x != d) { AddSbyte(x); Slot(o); } }
        
        /// <summary>
        /// Adds a Byte to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddByte(int o, byte x, byte d) { if (ForceDefaults || x != d) { AddByte(x); Slot(o); } }

        /// <summary>
        /// Adds a Int16 to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddShort(int o, short x, int d) { if (ForceDefaults || x != d) { AddShort(x); Slot(o); } }

        /// <summary>
        /// Adds a UInt16 to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddUshort(int o, ushort x, ushort d) { if (ForceDefaults || x != d) { AddUshort(x); Slot(o); } }

        /// <summary>
        /// Adds an Int32 to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddInt(int o, int x, int d) { if (ForceDefaults || x != d) { AddInt(x); Slot(o); } }

        /// <summary>
        /// Adds a UInt32 to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddUint(int o, uint x, uint d) { if (ForceDefaults || x != d) { AddUint(x); Slot(o); } }

        /// <summary>
        /// Adds an Int64 to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddLong(int o, long x, long d) { if (ForceDefaults || x != d) { AddLong(x); Slot(o); } }

        /// <summary>
        /// Adds a UInt64 to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddUlong(int o, ulong x, ulong d) { if (ForceDefaults || x != d) { AddUlong(x); Slot(o); } }

        /// <summary>
        /// Adds a Single to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddFloat(int o, float x, double d) { if (ForceDefaults || x != d) { AddFloat(x); Slot(o); } }

        /// <summary>
        /// Adds a Double to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddDouble(int o, double x, double d) { if (ForceDefaults || x != d) { AddDouble(x); Slot(o); } }

        /// <summary>
        /// Adds a buffer offset to the Table at index `o` in its vtable using the value `x` and default `d`
        /// </summary>
        /// <param name="o">The index into the vtable</param>
        /// <param name="x">The value to put into the buffer. If the value is equal to the default
        /// and <see cref="ForceDefaults"/> is false, the value will be skipped.</param>
        /// <param name="d">The default value to compare the value against</param>
        public void AddOffset(int o, int x, int d) { if (ForceDefaults || x != d) { AddOffset(x); Slot(o); } }
        /// @endcond

        /// <summary>
        /// Encode the string `s` in the buffer using UTF-8.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>
        /// The offset in the buffer where the encoded string starts.
        /// </returns>
        public StringOffset CreateString(string s)
        {
            NotNested();
            AddByte(0);
            var utf8StringLen = Encoding.UTF8.GetByteCount(s);
            StartVector(1, utf8StringLen, 1);
            Encoding.UTF8.GetBytes(s, 0, s.Length, _bb.Data, _space -= utf8StringLen);
            return new StringOffset(EndVector().Value);
        }

        /// @cond FLATBUFFERS_INTERNAL
        // Structs are stored inline, so nothing additional is being added.
        // `d` is always 0.
        public void AddStruct(int voffset, int x, int d)
        {
            if (x != d)
            {
                Nested(x);
                Slot(voffset);
            }
        }

        public int EndObject()
        {
            if (_vtableSize < 0)
                throw new InvalidOperationException(
                  "Flatbuffers: calling endObject without a startObject");

            AddInt((int)0);
            var vtableloc = Offset;
            // Write out the current vtable.
            for (int i = _vtableSize - 1; i >= 0 ; i--) {
                // Offset relative to the start of the table.
                short off = (short)(_vtable[i] != 0
                                        ? vtableloc - _vtable[i]
                                        : 0);
                AddShort(off);

                // clear out written entry
                _vtable[i] = 0;
            }

            const int standardFields = 2; // The fields below:
            AddShort((short)(vtableloc - _objectStart));
            AddShort((short)((_vtableSize + standardFields) *
                             sizeof(short)));

            // Search for an existing vtable that matches the current one.
            int existingVtable = 0;
            for (int i = 0; i < _numVtables; i++) {
                int vt1 = _bb.Length - _vtables[i];
                int vt2 = _space;
                short len = _bb.GetShort(vt1);
                if (len == _bb.GetShort(vt2)) {
                    for (int j = sizeof(short); j < len; j += sizeof(short)) {
                        if (_bb.GetShort(vt1 + j) != _bb.GetShort(vt2 + j)) {
                            goto endLoop;
                        }
                    }
                    existingVtable = _vtables[i];
                    break;
                }

            endLoop: { }
            }

            if (existingVtable != 0) {
                // Found a match:
                // Remove the current vtable.
                _space = _bb.Length - vtableloc;
                // Point table to existing vtable.
                _bb.PutInt(_space, existingVtable - vtableloc);
            } else {
                // No match:
                // Add the location of the current vtable to the list of
                // vtables.
                if (_numVtables == _vtables.Length)
                {
                    // Arrays.CopyOf(vtables num_vtables * 2);
                    var newvtables = new int[ _numVtables * 2];
                    Array.Copy(_vtables, newvtables, _vtables.Length);

                    _vtables = newvtables;
                };
                _vtables[_numVtables++] = Offset;
                // Point table to current vtable.
                _bb.PutInt(_bb.Length - vtableloc, Offset - vtableloc);
            }

            _vtableSize = -1;
            return vtableloc;
        }

        // This checks a required field has been set in a given table that has
        // just been constructed.
        public void Required(int table, int field)
        {
          int table_start = _bb.Length - table;
          int vtable_start = table_start - _bb.GetInt(table_start);
          bool ok = _bb.GetShort(vtable_start + field) != 0;
          // If this fails, the caller will show what field needs to be set.
          if (!ok)
            throw new InvalidOperationException("FlatBuffers: field " + field +
                                                " must be set");
        }
        /// @endcond

        /// <summary>
        /// Finalize a buffer, pointing to the given `root_table`.
        /// </summary>
        /// <param name="rootTable">
        /// An offset to be added to the buffer.
        /// </param>
        public void Finish(int rootTable)
        {
            Prep(_minAlign, sizeof(int));
            AddOffset(rootTable);
            _bb.Position = _space;
        }

        /// <summary>
        /// Get the ByteBuffer representing the FlatBuffer.
        /// </summary>
        /// <remarks>
        /// This is typically only called after you call `Finish()`.
        /// </remarks>
        /// <returns>
        /// Returns the ByteBuffer for this FlatBuffer.
        /// </returns>
        public ByteBuffer DataBuffer { get { return _bb; } }

        /// <summary>
        /// A utility function to copy and return the ByteBuffer data as a
        /// `byte[]`.
        /// </summary>
        /// <returns>
        /// A full copy of the FlatBuffer data.
        /// </returns>
        public byte[] SizedByteArray()
        {
            var newArray = new byte[_bb.Data.Length - _bb.Position];
            Buffer.BlockCopy(_bb.Data, _bb.Position, newArray, 0,
                             _bb.Data.Length - _bb.Position);
            return newArray;
        }

         /// <summary>
         /// Finalize a buffer, pointing to the given `rootTable`.
         /// </summary>
         /// <param name="rootTable">
         /// An offset to be added to the buffer.
         /// </param>
         /// <param name="fileIdentifier">
         /// A FlatBuffer file identifier to be added to the buffer before
         /// `root_table`.
         /// </param>
         public void Finish(int rootTable, string fileIdentifier)
         {
             Prep(_minAlign, sizeof(int) +
                             FlatBufferConstants.FileIdentifierLength);
             if (fileIdentifier.Length !=
                 FlatBufferConstants.FileIdentifierLength)
                 throw new ArgumentException(
                     "FlatBuffers: file identifier must be length " +
                     FlatBufferConstants.FileIdentifierLength,
                     "fileIdentifier");
             for (int i = FlatBufferConstants.FileIdentifierLength - 1; i >= 0;
                  i--)
             {
                AddByte((byte)fileIdentifier[i]);
             }
             Finish(rootTable);
        }


    }
}

/// @}
