namespace Galapa.Core.StreamObfuscator;

public class XorObfuscator : StreamObfuscator
{
    private readonly byte[] _key;

    public XorObfuscator(Stream baseStream, byte[] key) : base(baseStream)
    {
        if (key == null || key.Length == 0)
            throw new ArgumentException("Key must be a non-empty byte array.", nameof(key));

        this._key = key;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var read = this.BaseStream.Read(buffer, offset, count);
        for (var i = 0; i < read; i++)
        {
            var keyByte = this._key[(this.ProcessedPosition + i) % this._key.Length];
            var current = buffer[offset + i];

            if (current != 0x00 && current != keyByte) buffer[offset + i] = (byte)(current ^ keyByte);
        }

        this.ProcessedPosition += read;
        return read;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        var tempBuffer = new byte[count];
        for (var i = 0; i < count; i++)
        {
            var keyByte = this._key[(this.ProcessedPosition + i) % this._key.Length];
            var current = buffer[offset + i];

            tempBuffer[i] = current != 0x00 && current != keyByte
                ? (byte)(current ^ keyByte)
                : current;
        }

        this.BaseStream.Write(tempBuffer, 0, count);
        this.ProcessedPosition += count;
    }
}