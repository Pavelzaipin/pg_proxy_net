namespace TestLucene.CrapLord
{
    // Just here so we can switch implementation without changing NativeMethods

    // https://github.com/mono/mono/blob/master/mcs/class/Mono.Posix/Mono.Unix.Native/FileNameMarshaler.cs
    // https://github.com/mono/mono/blob/master/mcs/class/Mono.Posix/Mono.Unix/UnixMarshal.cs


    // Just here so we can switch implementation without changing NativeMethods
    internal class FileNameMarshaler
        : Utf8CustomMarshaler
    // : libACL.Unix.FileNameMarshaler
    {
    }


}