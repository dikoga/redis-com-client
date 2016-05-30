using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using redis_com_client;

namespace redis_com_client_test
{
    [TestClass]
    public class BasicTest
    {

        private CacheManager _manager;

        [TestInitialize]
        public void Initialize()
        {
            _manager = new CacheManager();
            _manager.Init("test1");
        }


        [TestMethod]
        public void Add()
        {
            _manager.Add("key1", "abcde");

            Assert.AreEqual("abcde", _manager.Get("key1"));
        }

        //[TestMethod]
        //public void SetExpiration()
        //{
        //    var wait = 5000;
        //    var expiredValue = DateTime.Now;
        //    _manager.SetExpiration("expired", expiredValue, wait);
        //
        //    Assert.AreEqual(expiredValue.ToString(), _manager.Get("expired"));
        //    Thread.Sleep(wait + 100);
        //    Assert.IsNull(_manager.Get("expired"));
        //}

        [TestMethod]
        public void GetByObject()
        {
            _manager.Add("dk", "123");
            Assert.AreEqual("123", _manager["dk"]);
        }

        [TestMethod]
        public void SetByObject()
        {
            _manager["dk"] = "123";
            Assert.AreEqual("123", _manager["dk"]);
        }

        [TestMethod]
        public void Array()
        {
            var array = new object[,] {{1, 2, 3}, { "a", "b", "c" }, { "aa", "bb", "cc" } };

            _manager["arraytest"] = array;

            var x = (object[,])_manager["arraytest"];
            Assert.AreEqual(int.Parse("1"), x[0, 0]); //It guarantees int32 to VBScropt compability.
        }

        [TestMethod]
        public void ArraySpecialChar()
        {
            var specialChar = "#$%ˆ@";
            var array = new object[,] { { 1, 2, 3 }, { specialChar, "b", "c" } };

            _manager["arraytest"] = array;

            var x = (object[,])_manager["arraytest"];
            Assert.AreEqual(specialChar, x[1, 0]);
        }

        [TestMethod]
        public void ArrayHtml()
        {
            var html = "<script>alert();</script>";
            var array = new object[,] { { 1, 2, 3 }, { html, "b", "c" } };

            _manager["arraytest"] = array;

            var x = (object[,])_manager["arraytest"];
            Assert.AreEqual(html, x[1, 0]);
        }

        [TestMethod]
        public void RemoveAllFromThisKey()
        {
            var manager2 = new CacheManager();
            manager2.Init("test2");
            manager2.Add("firstname", "22222");
            manager2.Add("lastname", "33333");

            _manager.Add("firstname", "firstname123");
            _manager.Add("lastname", "lastname123");
            _manager.RemoveAll();

            Assert.IsNull(_manager["firstname"]);
            Assert.IsNull(_manager["lastname"]);
            Assert.IsNotNull(manager2["firstname"]);
            Assert.IsNotNull(manager2["lastname"]);
        }

        [TestMethod]
        public void Exists()
        {
            _manager.Add("exists", "12344");
            Assert.IsTrue(_manager.Exists("exists"));
        }

        [TestMethod]
        public void NotExists()
        {
            Assert.IsFalse(_manager.Exists("notexists"));
        }

        [TestMethod]
        public void Remove()
        {
            _manager.Add("onekey", "12344");
            _manager.Remove("onekey");
            Assert.IsFalse(_manager.Exists("onekey"));
        }

        [TestMethod]
        public void SetExpirationExistingKey()
        {
            _manager.Add("key4", "12344");
            Assert.IsTrue(_manager.Exists("key4"));
            _manager.SetExpiration("key4",1000);
            Thread.Sleep(2000);
            Assert.IsNull(_manager["key4"]);
        }

        [TestMethod]
        public void AddNullValue()
        {
            _manager.Add("null", null);
            Assert.IsTrue(_manager.Exists("null"));
        }

        [TestMethod]
        public void ReplaceDbNull()
        {
            var array = new object[,] {{10, 20}, {"asdf", DBNull.Value}};
            _manager["DBNull"] = array;

            var result = (object[,])_manager["DBNull"];
            Assert.IsNotNull(result);
            Assert.IsNull(result[1,1]);
        }

        [TestMethod]
        public void AddSameKeyTwice()
        {
            _manager["twice"] = 1;
            Thread.Sleep(500);
            _manager["twice"] = "asdf";
            Assert.AreEqual("asdf", _manager["twice"]);
        }

        [TestMethod]
        public void Concurrent()
        {
            ArrayHtml();
            var sb = new StringBuilder();
            Parallel.For((long) 0, 1000, i =>
            {
                sb.AppendFormat("i: {0}{1}", i, Environment.NewLine);
                var sw = new Stopwatch();
                sw.Start();
                object x = _manager["arraytest"];
                sw.Stop();
                sb.AppendFormat("i: {0} - time: {1}ms", i, sw.ElapsedMilliseconds);
            });
            Console.WriteLine(sb);
        }

        [TestMethod]
        public void MyTableTwoDim()
        {
            var array = new object[,] { { 1, 2, 3 }, { "a", "b", "c" } };
            var table = new MyTable(array);

            var newArray  = (object[,])table.GetArray();
            Assert.AreEqual(array[0, 0], newArray[0, 0]);
            Assert.AreEqual(array[1, 1], newArray[1, 1]);
        }

        [TestMethod]
        public void MyTableOneDim()
        {
            var array = new object[] { 1, 2, 3 } ;
            var table = new MyTable(array);

            var newArray = (object[])table.GetArray();
            Assert.AreEqual(array[0], newArray[0]);
            Assert.AreEqual(array[1], newArray[1]);
            Assert.AreEqual(array[2], newArray[2]);
        }
    }
}
