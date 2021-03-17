using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web_Extractor.Controllers;
using System.Web.Mvc;

namespace TestProject
{
    [TestFixture]
    class ControllerTestClass
    {
        [Test]
        public void TestHomeIndex()
        {
            var obj = new HomeController();

            var viewResult = obj.Index() as ViewResult;

            Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
        }

        [Test]
        public void TestHomeTask1()
        {
            var obj = new HomeController();

            var viewResult = obj.Task1() as ViewResult;

            Assert.That(viewResult.ViewName, Is.EqualTo("Task1"));
        }

        [Test]
        public void TestHomeTask2()
        {
            var obj = new HomeController();

            var viewResult = obj.Task2() as ViewResult;

            Assert.That(viewResult.ViewName, Is.EqualTo("Task2"));
        }
    }
}
