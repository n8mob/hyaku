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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Silverlight.Testing;

namespace Hyaku.Tests
{
    [TestClass]
    public class SumsTest : SilverlightTest
    {
        [TestMethod]
        public void AlwaysPass()
        {
            Assert.IsTrue(true, "this method always passes");
        }

        [TestMethod]
        public void AlwaysFail()
        {
            Assert.IsFalse(true, "this method always fails");
        }
    }
}
