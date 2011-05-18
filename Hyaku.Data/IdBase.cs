using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace Hyaku.Data
{
    [DataContract]
    public class IdBase
    {
        private static ulong nextId = 0;
        private static object lockObject = new object();

        private ulong _id;

        protected static ulong GetNextId()
        {
            ulong temp;
            lock (lockObject) {
                temp = nextId;
                nextId += 1;
            }
            return temp;
        }

        public IdBase()
        {
            _id = GetNextId();
        }

        [DataMember]
        public virtual ulong Id
        {
            get
            {
                return _id;
            }
        }
    }
}
