// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 08-19-2022
//
// Last Modified By : Administrator
// Last Modified On : 08-20-2022
// link             :https://github.com/dotnet/roslyn/blob/main/src/Workspaces/Core/Portable/Shared/Utilities/BloomFilter.cs
// ***********************************************************************
// <copyright file="BloomFilter.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.Algorithms
{
    /// <summary>
    /// Class BloomFilter.
    /// </summary>
    public class BloomFilter
    {
        // From MurmurHash:
        // 'm' and 'r' are mixing constants generated off-line.
        // The values for m and r are chosen through experimentation and 
        // supported by evidence that they work well.
        /// <summary>
        /// The compute hash m
        /// </summary>
        private const uint Compute_Hash_m = 0x5bd1e995;
        /// <summary>
        /// The compute hash r
        /// </summary>
        private const int Compute_Hash_r = 24;
        /// <summary>
        /// The bit array
        /// </summary>
        private readonly BitArray _bitArray;
        /// <summary>
        /// The hash function count
        /// </summary>
        private readonly int _hashFunctionCount;
        /// <summary>
        /// The is case sensitive
        /// </summary>
        private readonly bool _isCaseSensitive;

        /// <summary>
        /// <![CDATA[
        /// 1) n  = Number of items in the filter
        /// 2) p = Probability of false positives, (a double between 0 and 1).
        /// 3) m = Number of bits in the filter
        /// 4) k = Number of hash functions
        /// m = ceil((n * log(p)) / log(1.0 / (pow(2.0, log(2.0)))))
        /// k = round(log(2.0) * m / n)
        /// ]]>
        /// </summary>
        /// <param name="expectedCount">The expected count.</param>
        /// <param name="falsePositiveProbability">The false positive probability.</param>
        /// <param name="isCaseSensitive">if set to <c>true</c> [is case sensitive].</param>
        public BloomFilter(int expectedCount, double falsePositiveProbability, bool isCaseSensitive)
        {
            var m = Math.Max(1, ComputeM(expectedCount, falsePositiveProbability));
            var k = Math.Max(1, ComputeK(expectedCount, falsePositiveProbability));

            // We must have size in even bytes, so that when we deserialize from bytes we get a bit array with the same count.
            // The count is used by the hash functions.
            var sizeInEvenBytes = (m + 7) & ~7;

            _bitArray = new BitArray(length: sizeInEvenBytes);
            _hashFunctionCount = k;
            _isCaseSensitive = isCaseSensitive;
        }


        /// <summary>
        /// Computes the m.
        /// m = ceil((n * log(p)) / log(1.0 / (pow(2.0, log(2.0)))))
        /// </summary>
        /// <param name="expectedCount">The expected count.</param>
        /// <param name="falsePositiveProbability">The false positive probability.</param>
        /// <returns>System.Int32.</returns>
        private static int ComputeM(int expectedCount, double falsePositiveProbability)
        {
            var p = falsePositiveProbability;
            double n = expectedCount;

            var numerator = n * Math.Log(p);
            var denominator = Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0)));
            return unchecked((int)Math.Ceiling(numerator / denominator));
        }

        /// <summary>
        /// Computes the k.
        /// k = round(log(2.0) * m / n)
        /// </summary>
        /// <param name="expectedCount">The expected count.</param>
        /// <param name="falsePositiveProbability">The false positive probability.</param>
        /// <returns>System.Int32.</returns>
        private static int ComputeK(int expectedCount, double falsePositiveProbability)
        {
            double n = expectedCount;
            double m = ComputeM(expectedCount, falsePositiveProbability);

            var temp = Math.Log(2.0) * m / n;
            return unchecked((int)Math.Round(temp));
        }
        /// <summary>
        /// Modification of the murmurhash2 algorithm.  Code is simpler because it operates over
        /// strings instead of byte arrays.  Because each string character is two bytes, it is known
        /// that the input will be an even number of bytes (though not necessarily a multiple of 4).
        /// This is needed over the normal 'string.GetHashCode()' because we need to be able to generate
        /// 'k' different well distributed hashes for any given string s.  Also, we want to be able to
        /// generate these hashes without allocating any memory.  My ideal solution would be to use an
        /// MD5 hash.  However, there appears to be no way to do MD5 in .NET where you can:
        /// a) feed it individual values instead of a byte[]
        /// b) have the hash computed into a byte[] you provide instead of a newly allocated one
        /// Generating 'k' pieces of garbage on each insert and lookup seems very wasteful.  So,
        /// instead, we use murmur hash since it provides well distributed values, allows for a
        /// seed, and allocates no memory.
        /// Murmur hash is public domain.  Actual code is included below as reference.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>System.Int32.</returns>
        private int ComputeHash(string key, int seed)
        {
            unchecked
            {
                // Initialize the hash to a 'random' value

                var numberOfCharsLeft = key.Length;
                var h = (uint)(seed ^ numberOfCharsLeft);

                // Mix 4 bytes at a time into the hash.  NOTE: 4 bytes is two chars, so we iterate
                // through the string two chars at a time.
                var index = 0;
                while (numberOfCharsLeft >= 2)
                {
                    var c1 = GetCharacter(key, index);
                    var c2 = GetCharacter(key, index + 1);

                    h = CombineTwoCharacters(h, c1, c2);

                    index += 2;
                    numberOfCharsLeft -= 2;
                }

                // Handle the last char (or 2 bytes) if they exist.  This happens if the original string had
                // odd length.
                if (numberOfCharsLeft == 1)
                {
                    var c = GetCharacter(key, index);
                    h = CombineLastCharacter(h, c);
                }

                // Do a few final mixes of the hash to ensure the last few bytes are well-incorporated.

                h = FinalMix(h);

                return (int)h;
            }
        }
        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>System.Int32.</returns>
        private static int ComputeHash(long key, int seed)
        {
            // This is a duplicate of ComputeHash(string key, int seed).  However, because
            // we only have 64bits to encode we just unroll that function here.  See
            // Other function for documentation on what's going on here.
            unchecked
            {
                // Initialize the hash to a 'random' value

                var numberOfCharsLeft = 4;
                var h = (uint)(seed ^ numberOfCharsLeft);

                // Mix 4 bytes at a time into the hash.  NOTE: 4 bytes is two chars, so we iterate
                // through the long two chars at a time.
                var index = 0;
                while (numberOfCharsLeft >= 2)
                {
                    var c1 = GetCharacter(key, index);
                    var c2 = GetCharacter(key, index + 1);

                    h = CombineTwoCharacters(h, c1, c2);

                    index += 2;
                    numberOfCharsLeft -= 2;
                }

                Debug.Assert(numberOfCharsLeft == 0);

                // Do a few final mixes of the hash to ensure the last few bytes are well-incorporated.
                h = FinalMix(h);

                return (int)h;
            }
        }
        /// <summary>
        /// Combines the last character.
        /// </summary>
        /// <param name="h">The h.</param>
        /// <param name="c">The c.</param>
        /// <returns>System.UInt32.</returns>
        private static uint CombineLastCharacter(uint h, uint c)
        {
            unchecked
            {
                h ^= c;
                h *= Compute_Hash_m;
                return h;
            }
        }

        /// <summary>
        /// Finals the mix.
        /// </summary>
        /// <param name="h">The h.</param>
        /// <returns>System.UInt32.</returns>
        private static uint FinalMix(uint h)
        {
            unchecked
            {
                h ^= h >> 13;
                h *= Compute_Hash_m;
                h ^= h >> 15;
                return h;
            }
        }

        /// <summary>
        /// Combines the two characters.
        /// </summary>
        /// <param name="h">The h.</param>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns>System.UInt32.</returns>
        private static uint CombineTwoCharacters(uint h, uint c1, uint c2)
        {
            unchecked
            {
                var k = c1 | (c2 << 16);

                k *= Compute_Hash_m;
                k ^= k >> Compute_Hash_r;
                k *= Compute_Hash_m;

                h *= Compute_Hash_m;
                h ^= k;

                return h;
            }
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Char.</returns>
        private char GetCharacter(string key, int index)
        {
            var c = key[index];
            return _isCaseSensitive ? c : char.ToLowerInvariant(c);
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Char.</returns>
        private static char GetCharacter(long key, int index)
        {
            Debug.Assert(index <= 3);
            return (char)(key >> (16 * index));
        }


#if false
        //-----------------------------------------------------------------------------
        // MurmurHash2, by Austin Appleby
        //
        // Note - This code makes a few assumptions about how your machine behaves -
        // 1. We can read a 4-byte value from any address without crashing
        // 2. sizeof(int) == 4
        //
        // And it has a few limitations -
        // 1. It will not work incrementally.
        // 2. It will not produce the same results on little-endian and big-endian
        //    machines.
        unsigned int MurmurHash2(const void* key, int len, unsigned int seed)
        {
            // 'm' and 'r' are mixing constants generated offline.
            // The values for m and r are chosen through experimentation and 
            // supported by evidence that they work well.
            
            const unsigned int m = 0x5bd1e995;
            const int r = 24;

            // Initialize the hash to a 'random' value
            unsigned int h = seed ^ len;

            // Mix 4 bytes at a time into the hash
            const unsigned char* data = (const unsigned char*)key;

            while(len >= 4)
            {
                unsigned int k = *(unsigned int*)data;

                k *= m; 
                k ^= k >> r; 
                k *= m; 

                h *= m; 
                h ^= k;

                data += 4;
                len -= 4;
            }
    
            // Handle the last few bytes of the input array
            switch(len)
            {
                case 3: h ^= data[2] << 16;
                case 2: h ^= data[1] << 8;
                case 1: h ^= data[0];
                        h *= m;
            };

            // Do a few final mixes of the hash to ensure the last few
            // bytes are well-incorporated.

            h ^= h >> 13;
            h *= m;
            h ^= h >> 15;

            return h;
        } 
#endif

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="values">The values.</param>
        public void AddRange(IEnumerable<string> values)
        {
            foreach (var v in values)
            {
                Add(v);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="values">The values.</param>
        public void AddRange(IEnumerable<long> values)
        {
            foreach (var v in values)
            {
                Add(v);
            }
        }
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(string value)
        {
            for (var i = 0; i < _hashFunctionCount; i++)
            {
                _bitArray[GetBitArrayIndex(value, i)] = true;
            }
        }

        /// <summary>
        /// Gets the index of the bit array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Int32.</returns>
        private int GetBitArrayIndex(string value, int i)
        {
            var hash = ComputeHash(value, i);
            hash %= _bitArray.Length;
            return Math.Abs(hash);
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(long value)
        {
            for (var i = 0; i < _hashFunctionCount; i++)
            {
                _bitArray[GetBitArrayIndex(value, i)] = true;
            }
        }

        /// <summary>
        /// Gets the index of the bit array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Int32.</returns>
        private int GetBitArrayIndex(long value, int i)
        {
            var hash = ComputeHash(value, i);
            hash %= _bitArray.Length;
            return Math.Abs(hash);
        }

        /// <summary>
        /// Probablies the contains.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ProbablyContains(string value)
        {
            for (var i = 0; i < _hashFunctionCount; i++)
            {
                if (!_bitArray[GetBitArrayIndex(value, i)])
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Probablies the contains.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ProbablyContains(long value)
        {
            for (var i = 0; i < _hashFunctionCount; i++)
            {
                if (!_bitArray[GetBitArrayIndex(value, i)])
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Determines whether the specified filter is equivalent.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns><c>true</c> if the specified filter is equivalent; otherwise, <c>false</c>.</returns>
        public bool IsEquivalent(BloomFilter filter)
        {
            return IsEquivalent(_bitArray, filter._bitArray)
                && _hashFunctionCount == filter._hashFunctionCount
                && _isCaseSensitive == filter._isCaseSensitive;
        }

        /// <summary>
        /// Determines whether the specified array1 is equivalent.
        /// </summary>
        /// <param name="array1">The array1.</param>
        /// <param name="array2">The array2.</param>
        /// <returns><c>true</c> if the specified array1 is equivalent; otherwise, <c>false</c>.</returns>
        private static bool IsEquivalent(BitArray array1, BitArray array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (var i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
