using System.Runtime.InteropServices;
using System.Text;

namespace fiskalizimi;

public class DllImporter
{
    private readonly string _key;

    public DllImporter(string privateKey)
    {
        _key = privateKey;
    }

    [DllImport("signer", EntryPoint = "DigitallySign")]
    static extern IntPtr DigitallySign(byte[] msg, byte[] key);

    public string SignMessage(string msg)
    {
        var msgBytes = Encoding.ASCII.GetBytes(msg);
        var result = DigitallySign(msgBytes, Encoding.ASCII.GetBytes(_key));
        var signature = Marshal.PtrToStringAnsi(result);
        return signature;
    }
}