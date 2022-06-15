namespace TestLucene.CrapLord
{
    public class Utf8ConstCustomMarshaler
        : System.Runtime.InteropServices.ICustomMarshaler
    {
        private static readonly Utf8ConstCustomMarshaler s_staticInstance;


        static Utf8ConstCustomMarshaler()
        {
            s_staticInstance = new Utf8ConstCustomMarshaler();
        }


        System.IntPtr System.Runtime.InteropServices.ICustomMarshaler.MarshalManagedToNative(object objManagedObj)
        {
            string managedObj = objManagedObj as string;

            if (managedObj == null)
                return System.IntPtr.Zero;

            // not null terminated
            byte[] strbuf = System.Text.Encoding.UTF8.GetBytes(managedObj);
            System.IntPtr buffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(strbuf.Length + 1);
            System.Runtime.InteropServices.Marshal.Copy(strbuf, 0, buffer, strbuf.Length);

            // write the terminating null
            //Marshal.WriteByte(buffer + strbuf.Length, 0);

            long lngPosEnd = buffer.ToInt64() + strbuf.Length;
            System.IntPtr ptrPosEnd = new System.IntPtr(lngPosEnd);
            System.Runtime.InteropServices.Marshal.WriteByte(ptrPosEnd, 0);

            return buffer;
        }


        // object System.Runtime.InteropServices.ICustomMarshaler.MarshalNativeToManaged(System.IntPtr pNativeData)
        object System.Runtime.InteropServices.ICustomMarshaler.MarshalNativeToManaged(System.IntPtr pNativeData)
        {
            int i = 0;
            while (System.Runtime.InteropServices.Marshal.ReadByte(pNativeData, i) != 0)
            {
                ++i;
            } // Whend 

            byte[] ba = new byte[i];
            System.Runtime.InteropServices.Marshal.Copy(pNativeData, ba, 0, i);
            string str = System.Text.Encoding.UTF8.GetString(ba);

            // System.Console.WriteLine(str);
            return str;
            // return Instance.MarshalNativeToManaged(pNativeData);
        }

        /*
        unsafe object System.Runtime.InteropServices.ICustomMarshaler.MarshalNativeToManaged(System.IntPtr pNativeData)
        {
            byte* walk = (byte*)pNativeData;

            // find the end of the string
            while (*walk != 0)
            {
                walk++;
            }

            int length = (int)(walk - (byte*)pNativeData);

            // should not be null terminated
            //byte[] strbuf = new byte[length - 1];
            byte[] strbuf = new byte[length];

            // skip the trailing null
            // System.Runtime.InteropServices.Marshal.Copy(pNativeData, strbuf, 0, length - 1);
            System.Runtime.InteropServices.Marshal.Copy(pNativeData, strbuf, 0, length);

            string data = System.Text.Encoding.UTF8.GetString(strbuf);
            return data;
        }
        */


        void System.Runtime.InteropServices.ICustomMarshaler.CleanUpNativeData(System.IntPtr pNativeData)
        {
            // SigSegV Segmentation Fault - You cannot free a const-string
            // Marshal.FreeHGlobal(pNativeData);
            // Mono.Unix.Native.Stdlib.free(pNativeData);
        }


        void System.Runtime.InteropServices.ICustomMarshaler.CleanUpManagedData(object managedObj)
        {
        }


        int System.Runtime.InteropServices.ICustomMarshaler.GetNativeDataSize()
        {
            return System.IntPtr.Size;
        }


        public static System.Runtime.InteropServices.ICustomMarshaler GetInstance(string cookie)
        {
            return s_staticInstance;
        }
    } // End Class ConstUtf8Marshaler 
} // End Namespace 
