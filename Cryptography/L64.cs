﻿using System;
using System.Linq;

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cryptography
{
    /// <summary>
    /// Base64 extension of the LC4 encryption algorithm (https://eprint.iacr.org/2017/339)
    /// </summary>
    public static class L64
    {
        /// <summary>
        /// Alphabet used for encryption and decryption (Base64 character set w/o padding character)
        /// </summary>
        private const string _alphabet = "+/0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Width / length of the matrix [8 (Sqrt of 64)]
        /// </summary>
        /// <returns>Matrix length</returns>
        private static readonly int _matrixLength = (int)Math.Sqrt(_alphabet.Length);

        /// <summary>
        /// Returns the integer value of a specified alphabet character
        /// </summary>
        /// <param name="c">Character to get integer value for</param>
        /// <returns>Integer value of specified character (index within alphabet).</returns>
        private static int ConvertCharToInt(char c)
        {
            return _alphabet.IndexOf(c);
        }

        /// <summary>
        /// Generates a random key by cryptographically shuffling the alphabet
        /// </summary>
        /// <returns>Key for encryption.</returns>
        public static string GenerateKey() => _alphabet.Shuffle();


        /// <summary>
        /// Initializes the state matrix for encryption or decryption
        /// </summary>
        /// <param name="key">key used to initialize state.</param>
        /// <returns>Initialized state.</returns>
        private static (char[,] matrix, int i, int j) InitializeState(string key)
        {

            var matrix = new char[_matrixLength, _matrixLength];
            var i = 0;
            var j = 0;

            for (var r = 0; r < _matrixLength; r++)
            {
                for (var c = 0; c < _matrixLength; c++)
                {
                    matrix[r, c] = key[(r * _matrixLength) + c];
                }
            }

            return (matrix, i, j);
        }

        /// <summary>
        /// Verifies that a key is valid
        /// </summary>
        /// <param name="key">Key to validate</param>
        private static void CheckKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (string.Concat(key.OrderBy(c => c)) != _alphabet)
            {
                throw new ArgumentException($"Invalid key: '{key}'. Expected a shuffled version of '{_alphabet}'");
            }
        }

        /// <summary>
        /// Rotates a row in the state matrix right by one
        /// </summary>
        /// <param name="matrix">State matrix</param>
        /// <param name="row">Row to rotate</param>
        private static void RotateRight(ref char[,] matrix, int row)
        {
            var lastIndex = _matrixLength - 1;

            var temp = matrix[row, lastIndex];

            for (var i = lastIndex; i > 0; i--)
            {
                matrix[row, i] = matrix[row, i - 1];
            }

            matrix[row, 0] = temp;
        }

        /// <summary>
        /// Rotates a column in the state matrix down by one
        /// </summary>
        /// <param name="matrix">State matrix</param>
        /// <param name="col">Column to rotate</param>
        private static void RotateDown(ref char[,] matrix, int col)
        {
            var lastIndex = _matrixLength - 1;

            var temp = matrix[lastIndex, col];

            for (var i = lastIndex; i > 0; i--)
            {
                matrix[i, col] = matrix[i - 1, col];
            }

            matrix[0, col] = temp;
        }

        /// <summary>
        /// Finds the specified character within the matrix.
        /// </summary>
        /// <param name="matrix">matrix to find character in.</param>
        /// <param name="c">character to find</param>
        /// <returns>Row and column indices of the desired character.</returns>
        private static (int row, int column) FindChar(char[,] matrix, char c)
        {
            for (var row = 0; row < _matrixLength; row++)
            {
                for (var col = 0; col < _matrixLength; col++)
                {
                    if (matrix[row, col] == c)
                    {
                        return (row, col);
                    }
                }
            }

            // Should never get here. Internal method operating on characters guaranteed to be in matrix.
            throw new Exception($"Invalid character {c}, expected character in alphabet: ${_alphabet}");
        }

        /// <summary>
        /// Mathmatical modulus operation that always returns a positive result, even if arguments are negative.
        /// Example: -5 % 3 => 1, not -1 like built in C# % operator.
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divisor</param>
        /// <returns>a % b</returns>
        private static int MathMod(int a, int b)
        {
            return (Math.Abs(a * b) + a) % b;
        }

        /// <summary>
        /// Encrypts a single plaintext character
        /// </summary>
        /// <param name="matrix">State matrix</param>
        /// <param name="i">State row index</param>
        /// <param name="j">State column index</param>
        /// <param name="plaintextChar">Character to encrypt</param>
        /// <returns>Encrypted ciphertext character</returns>
        private static char EncryptCharacter(ref char[,] matrix, ref int i, ref int j, char plaintextChar)
        {
            (var pRow, var pCol) = FindChar(matrix, plaintextChar);

            var stateInt = ConvertCharToInt(matrix[i, j]);
            var cRow = MathMod(pRow + (stateInt / _matrixLength), _matrixLength);
            var cCol = MathMod(pCol + MathMod(stateInt, _matrixLength), _matrixLength);

            var cipherChar = matrix[cRow, cCol];

            RotateRight(ref matrix, pRow);
            RotateDown(ref matrix, cCol);

            var cipherInt = ConvertCharToInt(cipherChar);
            i = (i + (cipherInt / _matrixLength)) % _matrixLength;
            j = (j + (cipherInt % _matrixLength)) % _matrixLength;

            return cipherChar;
        }

        /// <summary>
        /// Encrypts the plaintext with the specified key.
        /// </summary>
        /// <param name="plaintext">Plaintext to encrypt</param>
        /// <param name="key">Key to encrypt plaintext with</param>
        /// <returns>Encrypted ciphertext</returns>
        public static string Encrypt(string plaintext, string key)
        {
            if (plaintext == null)
            {
                throw new ArgumentNullException(nameof(plaintext));
            }

            CheckKey(key);

            (var matrix, var i, var j) = InitializeState(key);

            // pad end with trailing spaces to avoid base64 padding character (=)
            while (plaintext.Length % 3 != 0)
            {
                plaintext += " ";
            }

            plaintext = Base64.Encode(plaintext);

            var cipherText = new char[plaintext.Length];

            for (var index = 0; index < plaintext.Length; index++)
            {
                cipherText[index] = EncryptCharacter(ref matrix, ref i, ref j, plaintext[index]);
            }

            return new string(cipherText);
        }

        /// <summary>
        /// Decrypts a single ciphertext character
        /// </summary>
        /// <param name="matrix">State matrix</param>
        /// <param name="i">State row index</param>
        /// <param name="j">State column index</param>
        /// <param name="cipherChar">Character to decrypt</param>
        /// <returns>Decrypted plaintext character</returns>
        private static char DecryptCharacter(ref char[,] matrix, ref int i, ref int j, char cipherChar)
        {
            (var cRow, var cCol) = FindChar(matrix, cipherChar);

            var stateInt = ConvertCharToInt(matrix[i, j]);
            var pRow = MathMod(cRow - (stateInt / _matrixLength), _matrixLength);
            var pCol = MathMod(cCol - MathMod(stateInt, _matrixLength), _matrixLength);

            var plaintextChar = matrix[pRow, pCol];

            RotateRight(ref matrix, pRow);
            RotateDown(ref matrix, cCol);

            var cipherInt = ConvertCharToInt(cipherChar);
            i = (i + (cipherInt / _matrixLength)) % _matrixLength;
            j = (j + (cipherInt % _matrixLength)) % _matrixLength;

            return plaintextChar;
        }

        /// <summary>
        /// Decrypts ciphertext using the specified key.
        /// </summary>
        /// <param name="ciphertext">Text to decrypt</param>
        /// <param name="key">Key to decrypt text with</param>
        /// <returns>Decrypted plaintext</returns>
        public static string Decrypt(string ciphertext, string key)
        {
            if (ciphertext == null)
            {
                throw new ArgumentNullException(nameof(ciphertext));
            }

            CheckKey(key);

            (var matrix, var i, var j) = InitializeState(key);

            var plaintext = new char[ciphertext.Length];

            for (var index = 0; index < ciphertext.Length; index++)
            {
                plaintext[index] = DecryptCharacter(ref matrix, ref i, ref j, ciphertext[index]);
            }

            return Base64.Decode(new string(plaintext));
        }
    }
}

#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional