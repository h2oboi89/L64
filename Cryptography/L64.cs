using System;
using System.Linq;

namespace Cryptography
{
    public class L64
    {
        private static readonly string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        private static readonly int _matrixLength = (int)Math.Sqrt(_alphabet.Length); // 8 (Sqrt of 64)

        private char[,] _matrix = new char[_matrixLength, _matrixLength];

        private int _i, _j;

        private static int _ConvertCharToInt(char c)
        {
            return _alphabet.IndexOf(c);
        }

        public static string GenerateKey() => _alphabet.Shuffle();

        private void _InitializeState(string key)
        {
            for (var r = 0; r < _matrixLength; r++)
            {
                for (var c = 0; c < _matrixLength; c++)
                {
                    this._matrix[r, c] = key[(r * _matrixLength) + c];
                }
            }

            this._i = 0;
            this._j = 0;
        }

        private static void _CheckKey(string key)
        {
            if (key.Length != _alphabet.Length)
            {
                throw new ArgumentException($"Invalid key length for key '{key}', required length: {_alphabet.Length}");
            }
        }

        private void _RotateRight(int pRow)
        {
            var lastIndex = _matrixLength - 1;

            var temp = this._matrix[pRow, lastIndex];

            for (var i = lastIndex; i > 0; i--)
            {
                this._matrix[pRow, i] = this._matrix[pRow, i - 1];
            }

            this._matrix[pRow, 0] = temp;
        }

        private void _RotateDown(int cCol)
        {
            var lastIndex = _matrixLength - 1;

            var temp = this._matrix[lastIndex, cCol];

            for (var i = lastIndex; i > 0; i--)
            {
                this._matrix[i, cCol] = this._matrix[i - 1, cCol];
            }

            this._matrix[0, cCol] = temp;
        }

        private (int row, int column) _FindChar(char c)
        {
            for (var row = 0; row < _matrixLength; row++)
            {
                for (var col = 0; col < _matrixLength; col++)
                {
                    if (this._matrix[row, col] == c)
                    {
                        return (row, col);
                    }
                }
            }

            // should never get here
            throw new Exception($"Invalid character {c}, expected character in alphabet: ${_alphabet}");
        }

        private char _Encrypt(char p)
        {
            (var pRow, var pCol) = _FindChar(p);

            var cRow = pRow + ((_ConvertCharToInt(this._matrix[this._i, this._j]) / _matrixLength) % _matrixLength);
            var cCol = pCol + ((_ConvertCharToInt(this._matrix[this._i, this._j]) % _matrixLength) % _matrixLength);

            var c = this._matrix[cRow, cCol];

            this._RotateRight(pRow);
            this._RotateDown(cCol);

            this._i = (this._i + (_ConvertCharToInt(c) / _matrixLength)) % _matrixLength;
            this._j = (this._j + (_ConvertCharToInt(c) % _matrixLength)) % _matrixLength;

            return c;
        }

        public string Encrypt(string plainText, string key)
        {
            _CheckKey(key);

            _InitializeState(key);

            plainText = Base64.Encode(plainText);

            return plainText.Select(p => this._Encrypt(p)).ToString();
        }

        private char _Decrypt(char c)
        {
            (var cRow, var cCol) = _FindChar(c);

            var pRow = cRow - ((_ConvertCharToInt(this._matrix[this._i, this._j]) / _matrixLength) % _matrixLength);
            var pCol = cCol - ((_ConvertCharToInt(this._matrix[this._i, this._j]) % _matrixLength) % _matrixLength);

            var p = this._matrix[pRow, pCol];

            this._RotateRight(pRow);
            this._RotateDown(cCol);

            this._i = (this._i + (_ConvertCharToInt(c) / _matrixLength)) % _matrixLength;
            this._j = (this._j + (_ConvertCharToInt(c) % _matrixLength)) % _matrixLength;

            return p;
        }

        public string Decrypt(string cipherText, string key)
        {
            _CheckKey(key);

            _InitializeState(key);

            var plainText = cipherText.Select(c => this._Decrypt(c)).ToString();

            return Base64.Decode(plainText);
        }
    }
}
