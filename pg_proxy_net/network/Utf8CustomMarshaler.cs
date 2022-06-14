namespace TestLucene.CrapLord
{
    /// <summary>
    /// Marshaller for UTF8 strings.
    /// </summary>
    public class Utf8CustomMarshaler
        : System.Runtime.InteropServices.ICustomMarshaler
    {
        private static readonly Utf8CustomMarshaler s_staticInstance;
        private static readonly System.Action<int> s_setLastWin32Error;


        static Utf8CustomMarshaler()
        {
            System.Type t = typeof(System.Runtime.InteropServices.Marshal);
            System.Reflection.MethodInfo mi = t.GetMethod("SetLastWin32Error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            // mi.Invoke(null, new object[] { (object)lastError });
            s_setLastWin32Error = (System.Action<int>)mi.CreateDelegate(typeof(System.Action<int>));
            s_staticInstance = new Utf8CustomMarshaler();
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
        } // End Function MarshalManagedToNative 


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
            //Marshal.Copy(pNativeData, strbuf, 0, length - 1);
            System.Runtime.InteropServices.Marshal.Copy(pNativeData, strbuf, 0, length);
            
            string data = System.Text.Encoding.UTF8.GetString(strbuf);
            return data;
        } // End Function MarshalNativeToManaged 
        */


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


        void System.Runtime.InteropServices.ICustomMarshaler.CleanUpNativeData(System.IntPtr pNativeData)
        {
            int lastError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
            System.Runtime.InteropServices.Marshal.FreeHGlobal(pNativeData);
            s_setLastWin32Error(lastError);
        } // End Function CleanUpNativeData 


        void System.Runtime.InteropServices.ICustomMarshaler.CleanUpManagedData(object pNativeData)
        {
        }


        int System.Runtime.InteropServices.ICustomMarshaler.GetNativeDataSize()
        {
            return System.IntPtr.Size;
        } // End Function GetNativeDataSize 


        // This is required for CustomMarshal apart from implementing the interface ! 
        public static System.Runtime.InteropServices.ICustomMarshaler GetInstance(string cookie)
        {
            return s_staticInstance;
        } // End Function GetInstance 
    } // End Class Utf8CustomMarshaler 
} // End Namespace 
