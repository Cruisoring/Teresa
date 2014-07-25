using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Teresa;

namespace TeresaExample
{
    [SetUpFixture]
    public class BaseTest
    {
        [SetUp]
        public void RunFirst()
        {
            DriverManager.SetDriverType(WebDriverTypes.ChromeDriver);
        }

        [TearDown]
        public void RunLast()
        {
            DriverManager.Driver.Quit();
        }
    }
}
