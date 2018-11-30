using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;

namespace WebApp.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        [TestInitialize]
        public void TestInit()
        {
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));
            
        }

        [TestMethod()]
        public void IndexTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SetRoleTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void AboutTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void HomeTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SelectTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void RefreshSessionTest()
        {
            Assert.IsTrue(true);
        }
    }
}