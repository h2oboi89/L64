using System;

namespace Cryptography
{
    public class L64
    {
        private static readonly string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        private static readonly int _matrixLength = (int)Math.Sqrt(_alphabet.Length); // 8 (Sqrt of 64)

        private static int _ConvertCharToInt(char c)
        {
            return _alphabet.IndexOf(c);
        }

        public static string GenerateKey() => _alphabet.Shuffle();

        private static (char[,] matrix, int i, int j) _InitializeState(string key)
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

        private static void _CheckKey(string key)
        {
            if (key.Length != _alphabet.Length)
            {
                throw new ArgumentException($"Invalid key length for key '{key}', required length: {_alphabet.Length}");
            }
        }

        private static void _RotateRight(ref char[,] matrix, int pRow)
        {
            var lastIndex = _matrixLength - 1;

            var temp = matrix[pRow, lastIndex];

            for (var i = lastIndex; i > 0; i--)
            {
                matrix[pRow, i] = matrix[pRow, i - 1];
            }

            matrix[pRow, 0] = temp;
        }

        private static void _RotateDown(ref char[,] matrix, int cCol)
        {
            var lastIndex = _matrixLength - 1;

            var temp = matrix[lastIndex, cCol];

            for (var i = lastIndex; i > 0; i--)
            {
                matrix[i, cCol] = matrix[i - 1, cCol];
            }

            matrix[0, cCol] = temp;
        }

        private static (int row, int column) _FindChar(char[,] matrix, char c)
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

            // should never get here
            throw new Exception($"Invalid character {c}, expected character in alphabet: ${_alphabet}");
        }

        private static int MathMod(int a, int b)
        {
            return (Math.Abs(a * b) + a) % b;
        }

        private static char _EncryptCharacter(ref char[,] matrix, ref int i, ref int j, char p)
        {
            (var pRow, var pCol) = _FindChar(matrix, p);

            var cRow = MathMod(pRow + (_ConvertCharToInt(matrix[i, j]) / _matrixLength), _matrixLength);
            var cCol = MathMod(pCol + MathMod(_ConvertCharToInt(matrix[i, j]), _matrixLength), _matrixLength);

            var c = matrix[cRow, cCol];

            _RotateRight(ref matrix, pRow);
            _RotateDown(ref matrix, cCol);

            i = (i + (_ConvertCharToInt(c) / _matrixLength)) % _matrixLength;
            j = (j + (_ConvertCharToInt(c) % _matrixLength)) % _matrixLength;

            return c;
        }

        public static string Encrypt(string plainText, string key)
        {
            _CheckKey(key);

            (var matrix, var i, var j) = _InitializeState(key);

            while (plainText.Length % 3 != 0)
            {
                plainText += " ";
            }

            plainText = Base64.Encode(plainText);

            var cipherText = new char[plainText.Length];

            for (var p = 0; p < plainText.Length; p++)
            {
                cipherText[p] = _EncryptCharacter(ref matrix, ref i, ref j, plainText[p]);
            }

            return new string(cipherText);
        }

        private static char _DecryptCharacter(ref char[,] matrix, ref int i, ref int j, char c)
        {
            (var cRow, var cCol) = _FindChar(matrix, c);

            var pRow = MathMod(cRow - (_ConvertCharToInt(matrix[i, j]) / _matrixLength), _matrixLength);
            var pCol = MathMod(cCol - MathMod(_ConvertCharToInt(matrix[i, j]), _matrixLength), _matrixLength);

            var p = matrix[pRow, pCol];

            _RotateRight(ref matrix, pRow);
            _RotateDown(ref matrix, cCol);

            i = (i + (_ConvertCharToInt(c) / _matrixLength)) % _matrixLength;
            j = (j + (_ConvertCharToInt(c) % _matrixLength)) % _matrixLength;

            return p;
        }

        public static string Decrypt(string cipherText, string key)
        {
            _CheckKey(key);

            (var matrix, var i, var j) = _InitializeState(key);

            var plainText = new char[cipherText.Length];

            for (var c = 0; c < cipherText.Length; c++)
            {
                plainText[c] = _DecryptCharacter(ref matrix, ref i, ref j, cipherText[c]);
            }

            return Base64.Decode(new string(plainText)).TrimEnd();
        }
    }
}
