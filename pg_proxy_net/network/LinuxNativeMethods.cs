
namespace TestLucene.CrapLord
{


    class LinuxNativeMethods
    {

        private const string LIBC = "libc";


        // [System.CLSCompliant(false)]
        [System.Flags]
        public enum AccessModes
            : int
        {
            F_OK = 0,
            X_OK = 1,
            W_OK = 2,
            R_OK = 4
        }

        public enum Errno
            : int
        {
            EPERM = 1,
            ENOENT = 2,
            ESRCH = 3,
            EINTR = 4,
            EIO = 5,
            ENXIO = 6,
            E2BIG = 7,
            ENOEXEC = 8,
            EBADF = 9,
            ECHILD = 10,
            EAGAIN = 11,
            EWOULDBLOCK = 11,
            ENOMEM = 12,
            EACCES = 13,
            EFAULT = 14,
            ENOTBLK = 15,
            EBUSY = 16,
            EEXIST = 17,
            EXDEV = 18,
            ENODEV = 19,
            ENOTDIR = 20,
            EISDIR = 21,
            EINVAL = 22,
            ENFILE = 23,
            EMFILE = 24,
            ENOTTY = 25,
            ETXTBSY = 26,
            EFBIG = 27,
            ENOSPC = 28,
            ESPIPE = 29,
            EROFS = 30,
            EMLINK = 31,
            EPIPE = 32,
            EDOM = 33,
            ERANGE = 34,
            EDEADLK = 35,
            EDEADLOCK = 35,
            ENAMETOOLONG = 36,
            ENOLCK = 37,
            ENOSYS = 38,
            ENOTEMPTY = 39,
            ELOOP = 40,
            ENOMSG = 42,
            EIDRM = 43,
            ECHRNG = 44,
            EL2NSYNC = 45,
            EL3HLT = 46,
            EL3RST = 47,
            ELNRNG = 48,
            EUNATCH = 49,
            ENOCSI = 50,
            EL2HLT = 51,
            EBADE = 52,
            EBADR = 53,
            EXFULL = 54,
            ENOANO = 55,
            EBADRQC = 56,
            EBADSLT = 57,
            EBFONT = 59,
            ENOSTR = 60,
            ENODATA = 61,
            ETIME = 62,
            ENOSR = 63,
            ENONET = 64,
            ENOPKG = 65,
            EREMOTE = 66,
            ENOLINK = 67,
            EADV = 68,
            ESRMNT = 69,
            ECOMM = 70,
            EPROTO = 71,
            EMULTIHOP = 72,
            EDOTDOT = 73,
            EBADMSG = 74,
            EOVERFLOW = 75,
            ENOTUNIQ = 76,
            EBADFD = 77,
            EREMCHG = 78,
            ELIBACC = 79,
            ELIBBAD = 80,
            ELIBSCN = 81,
            ELIBMAX = 82,
            ELIBEXEC = 83,
            EILSEQ = 84,
            ERESTART = 85,
            ESTRPIPE = 86,
            EUSERS = 87,
            ENOTSOCK = 88,
            EDESTADDRREQ = 89,
            EMSGSIZE = 90,
            EPROTOTYPE = 91,
            ENOPROTOOPT = 92,
            EPROTONOSUPPORT = 93,
            ESOCKTNOSUPPORT = 94,
            EOPNOTSUPP = 95,
            EPFNOSUPPORT = 96,
            EAFNOSUPPORT = 97,
            EADDRINUSE = 98,
            EADDRNOTAVAIL = 99,
            ENETDOWN = 100,
            ENETUNREACH = 101,
            ENETRESET = 102,
            ECONNABORTED = 103,
            ECONNRESET = 104,
            ENOBUFS = 105,
            EISCONN = 106,
            ENOTCONN = 107,
            ESHUTDOWN = 108,
            ETOOMANYREFS = 109,
            ETIMEDOUT = 110,
            ECONNREFUSED = 111,
            EHOSTDOWN = 112,
            EHOSTUNREACH = 113,
            EALREADY = 114,
            EINPROGRESS = 115,
            ESTALE = 116,
            EUCLEAN = 117,
            ENOTNAM = 118,
            ENAVAIL = 119,
            EISNAM = 120,
            EREMOTEIO = 121,
            EDQUOT = 122,
            ENOMEDIUM = 123,
            EMEDIUMTYPE = 124,
            ECANCELED = 125,
            ENOKEY = 126,
            EKEYEXPIRED = 127,
            EKEYREVOKED = 128,
            EKEYREJECTED = 129,
            EOWNERDEAD = 130,
            ENOTRECOVERABLE = 131,
            EPROCLIM = 1067,
            EBADRPC = 1072,
            ERPCMISMATCH = 1073,
            EPROGUNAVAIL = 1074,
            EPROGMISMATCH = 1075,
            EPROCUNAVAIL = 1076,
            EFTYPE = 1079,
            EAUTH = 1080,
            ENEEDAUTH = 1081,
            EPWROFF = 1082,
            EDEVERR = 1083,
            EBADEXEC = 1085,
            EBADARCH = 1086,
            ESHLIBVERS = 1087,
            EBADMACHO = 1088,
            ENOATTR = 1093,
            ENOPOLICY = 1103
        }




        // char *strerror(int errnum);
        // https://man7.org/linux/man-pages/man3/strerror.3.html
        [System.Security.SuppressUnmanagedCodeSecurity]
        [System.Runtime.InteropServices.DllImport(LIBC, EntryPoint = "strerror", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        // [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8ConstCustomMarshaler))]
        private static extern string strerror(Errno errnum);




        // int access(const char *pathname, int mode);
        // https://linux.die.net/man/2/access
        [System.Security.SuppressUnmanagedCodeSecurity]
        [System.Runtime.InteropServices.DllImport(LIBC, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "access", SetLastError = true)]
#if USE_LPUTF8Str
        private static extern int access([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPUTF8Str)] string path, AccessModes mode);
#else
        private static extern int access([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(FileNameMarshaler))] string path, AccessModes mode);
#endif


        // ssize_t readlink(const char *path, char *buf, size_t bufsiz);
        // https://linux.die.net/man/2/readlink
        [System.Security.SuppressUnmanagedCodeSecurity]
        [System.Runtime.InteropServices.DllImport(LIBC, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "readlink", SetLastError = true)]
#if USE_LPUTF8Str
        private static extern long readlink([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPUTF8Str)] string path, byte[] buf, ulong bufsiz);
#else
        private static extern long readlink([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(FileNameMarshaler))] string path, byte[] buf, ulong bufsiz);
#endif


        public static Errno errno
        {
            get
            {
                // How would you guarantee that the runtime has not called some CRT function 
                // during its internal processing that has affected the errno ?
                // For the same reason, you should not call GetLastError directly either. 
                // The DllImportAttribute provides a SetLastError property so the runtime knows 
                // to immediately capture the last error and store it in a place that the managed code 
                // can read using Marshal.GetLastWin32Error.

                // this work on Linux !
                // Marshal.GetLastWin32Error can be used to retrieve errno.
                int num = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                return (Errno)num;
            }
        }


        private static string GetErrorMessage(Errno err)
        {
            string msg = strerror(err);
            return msg;
        }


        private static string ErrorMessage
        {
            get
            {
                Errno err = errno;
                string msg = GetErrorMessage(err);
                return msg;
            }
        }




        /// <summary>
        /// access() checks whether the calling process can access the file pathname. If pathname is a symbolic link, it is dereferenced.
        /// </summary>
        /// <param name="pathmame"></param>
        /// <param name="mode"></param>
        /// <returns>On success (all requested permissions granted), zero is returned. On error (at least one bit in mode asked for a permission that is denied, or some other error occurred), -1 is returned, and errno is set appropriately.</returns>
        public static bool Access(string pathmame, AccessModes mode)
        {
            int ret = access(pathmame, mode);
            if (ret == -1)
            {
                Errno err = errno;
                // return null;
                System.Console.Error.WriteLine("Error on syscall \"access\"");

                // https://github.com/mono/mono/blob/master/mcs/class/Mono.Posix/Mono.Unix.Native/Stdlib.cs
                // https://stackoverflow.com/questions/2485648/access-c-global-variable-errno-from-c-sharp
                // throw ACLManagerException(Glib::locale_to_utf8(strerror(errno)));
                throw new System.InvalidOperationException(GetErrorMessage(err));
            } // End if (ret == -1) 

            return ret == 0;
        } // End Function Access 


        public static string ReadLink(string path)
        {
            // 4096 characters path Limux for ext4
            byte[] buffer = new byte[4096];

            long ret = readlink(path, buffer, (ulong)buffer.Length);
            if (ret == -1)
            {
                Errno curError = errno;
                if (curError == Errno.ENOENT)
                    return null;

                string message = GetErrorMessage(curError);

                // return null;
                System.Console.Error.WriteLine("Error on syscall \"readlink\"");
                System.Console.Error.WriteLine(message);

                // https://github.com/mono/mono/blob/master/mcs/class/Mono.Posix/Mono.Unix.Native/Stdlib.cs
                // https://stackoverflow.com/questions/2485648/access-c-global-variable-errno-from-c-sharp
                // throw ACLManagerException(Glib::locale_to_utf8(strerror(errno)));
                throw new System.InvalidOperationException(message);
            } // End if (ret == -1) 

            System.Array.Resize(ref buffer, (int)ret);
            string target = System.Text.Encoding.UTF8.GetString(buffer);

            return target;
        } // End Function ReadLink 


        private static string ReadLink_crap(string path)
        {
            string target = null;

            // Need to set capacity to maximum, otherwise, default is 16...
            // (otherwise truncation after 16 characters)
            System.Text.StringBuilder sb = new System.Text.StringBuilder(4096);
            // Mono.Unix.Native.Syscall.readlink(path, sb);
            target = sb.ToString();
            sb.Clear();
            sb = null;
            if (target != null)
                target = target.TrimEnd('\0');

            return target;
        } // End Function ReadLink_crap 


    }


}
