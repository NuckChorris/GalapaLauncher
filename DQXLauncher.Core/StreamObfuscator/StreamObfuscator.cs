namespace DQXLauncher.Core.StreamObfuscator;

public abstract class StreamObfuscator(Stream baseStream) : Stream
{
    protected readonly Stream BaseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
    protected long ProcessedPosition;

    public override bool CanRead => this.BaseStream.CanRead;
    public override bool CanSeek => this.BaseStream.CanSeek;
    public override bool CanWrite => this.BaseStream.CanWrite;
    public override long Length => this.BaseStream.Length;

    public override long Position
    {
        get => this.BaseStream.Position;
        set
        {
            this.BaseStream.Position = value;
            this.ProcessedPosition = value;
        }
    }

    public override void Flush()
    {
        this.BaseStream.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        var newPos = this.BaseStream.Seek(offset, origin);
        this.ProcessedPosition = newPos;
        return newPos;
    }

    public override void SetLength(long value)
    {
        this.BaseStream.SetLength(value);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) this.BaseStream.Dispose();
        base.Dispose(disposing);
    }
}