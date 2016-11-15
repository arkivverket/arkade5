using System.Collections.Generic;
using System.IO;

namespace Arkivverket.Arkade.Util
{
    // Based on http://stackoverflow.com/questions/6655246/how-to-read-text-file-by-particular-line-separator-character/31349928#31349928
    internal static class StreamReaderExtensions
    {
        public static IEnumerable<string> ReadUntil(this StreamReader reader, string delimiter)
        {
            List<char> buffer = new List<char>();
            CircularBuffer<char> delimBuffer = new CircularBuffer<char>(delimiter.Length);
            while (reader.Peek() >= 0)
            {
                char c = (char) reader.Read();
                delimBuffer.Enqueue(c);
                buffer.Add(c);
                if (delimBuffer.ToString() == delimiter)
                {
                    if (buffer.Count > 0)
                    {
                        // TODO jostein: This may be optimized
                        yield return new string(buffer.GetRange(0, buffer.Count - delimiter.Length).ToArray());
                        buffer.Clear();
                    }
                }
            }
        }

        private class CircularBuffer<T> : Queue<T>
        {
            private readonly int _capacity;

            public CircularBuffer(int capacity) : base(capacity)
            {
                _capacity = capacity;
            }

            public new void Enqueue(T item)
            {
                if (Count == _capacity)
                {
                    Dequeue();
                }
                base.Enqueue(item);
            }

            public override string ToString()
            {
                List<string> items = new List<string>();
                foreach (T t in this)
                {
                    items.Add(t.ToString());
                }
                ;
                return string.Join("", items);
            }
        }
    }
}