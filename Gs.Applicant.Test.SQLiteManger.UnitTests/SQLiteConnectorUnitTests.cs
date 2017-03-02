//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Data.SQLite;
//using System.Linq;

//namespace Gs.Applicant.Test.SQLite.UnitTests
//{
//    /// <summary>
//    /// Summary description for SQLiteConnectorUnitTests
//    /// </summary>
//    [TestClass]
//    public class SQLiteConnectorUnitTests
//    {
//        private string testDbName;
//        private string connString;

//        public SQLiteConnectorUnitTests()
//        {
//            testDbName = string.Format(@"{0}\test.sqlite", AppDomain.CurrentDomain.BaseDirectory);
//            connString = string.Format(@"Data Source={0};Pooling=False;Version=3;", testDbName);
//        }

//        #region Additional test attributes

//        //
//        // You can use the following additional attributes as you write your tests:
//        //
//        // Use ClassInitialize to run code before running the first test in the class
//        // [ClassInitialize()]
//        // public static void MyClassInitialize(TestContext testContext) { }
//        //
//        // Use ClassCleanup to run code after all tests in a class have run
//        // [ClassCleanup()]
//        // public static void MyClassCleanup() { }
//        //
//        // Use TestInitialize to run code before running each test
//        [TestInitialize()]
//        public void MyTestInitialize()
//        {
//            if (System.IO.File.Exists(testDbName))
//            {
//                System.IO.File.Delete(testDbName);
//            }
//        }

//        //
//        // Use TestCleanup to run code after each test has run
//        //[TestCleanup()]
//        //public void MyTestCleanup()
//        //{
//        //}
//        //

//        #endregion Additional test attributes

//        [TestMethod]
//        public void TestCreateDatabaseSuccess()
//        {
//            //Arrange

//            //Act
//            using (SQLiteConnector connector = new SQLiteConnector())
//            {
//                connector.CreateDatabase(testDbName);
//            }
//            //Assert
//            Assert.IsTrue(System.IO.File.Exists(testDbName));
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ArgumentException), "Database already exists.")]
//        public void TestCreateAlreadyExistingDatabase()
//        {
//            //Arrange

//            //Act
//            using (SQLiteConnector connector = new SQLiteConnector())
//            {
//                connector.CreateDatabase(testDbName);
//                connector.CreateDatabase(testDbName);
//            }
//            //Assert
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ArgumentException), "Name must be of type sqlite.")]
//        public void TestCreateDatabaseWithWrongExtension()
//        {
//            //Arrange

//            //Act
//            using (SQLiteConnector connector = new SQLiteConnector())
//            {
//                connector.CreateDatabase(testDbName + ".doc");
//            }
//            //Assert
//        }

//        [TestMethod]
//        public void TestCreateTableSuccess()
//        {
//            //Arrange
//            int Actualcount = 0;
//            int ExpectedCount = 2;
//            Column[] columns = new Column[2] {
//                    new Column()
//                    {
//                        Name="Column 1",
//                        ColumnType = ColumnDataType.TEXT
//                    },
//                    new Column()
//                    {
//                        Name="Column 2",
//                        ColumnType = ColumnDataType.INTEGER
//                    }
//                };
//            string tablename = "test";
//            //Act
//            using (SQLiteConnector connector = new SQLiteConnector(connString))
//            {
//                connector.CreateTable(tablename, columns);
//            }
//            //Assert
//            SQLiteConnection conn = new SQLiteConnection(connString);
//            conn.Open();
//            try
//            {
//                SQLiteCommand cmd = conn.CreateCommand();

//                cmd.CommandText = string.Format("PRAGMA table_info({0});", tablename);
//                var r = cmd.ExecuteReader();

//                while (r.Read())
//                {
//                    ++Actualcount;
//                }
//            }
//            finally
//            {
//                conn.Close();
//                conn.Dispose();
//            }
//            Assert.AreEqual(ExpectedCount, Actualcount);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ArgumentException), "Column Names cannot be empty.")]
//        public void TestCreateTableWithEmptyColumnName()
//        {
//            //Arrange
//            Column[] columns = new Column[2] {
//                    new Column()
//                    {
//                        Name="Column 1",
//                        ColumnType = ColumnDataType.TEXT
//                    },
//                    new Column()
//                    {
//                        Name="",
//                        ColumnType = ColumnDataType.INTEGER
//                    }
//                };
//            string tablename = "test";

//            //Act
//            using (SQLiteConnector connector = new SQLiteConnector(connString))
//            {
//                connector.CreateTable(tablename, columns);
//            }
//            //Assert
//        }

//        [TestMethod]
//        public void TestGetTableListing()
//        {
//            //Arrange
//            int Actualcount = 0;
//            int ExpectedCount = 2;
//            string[] expectedTableNames = new string[2] { "test", "test2" };
//            string[] actualTableNames;

//            SQLiteConnection conn = new SQLiteConnection(connString);
//            conn.Open();
//            try
//            {
//                var cmd = conn.CreateCommand();
//                cmd.CommandText = "CREATE TABLE test( col1 TEXT)";
//                cmd.ExecuteNonQuery();
//                cmd.CommandText = "CREATE TABLE test2( col1 TEXT)";
//                cmd.ExecuteNonQuery();
//                //Act
//                using (SQLiteConnector connector = new SQLiteConnector(connString))
//                {
//                    actualTableNames = connector.GetDatabaseTableListing();
//                }

//                //Assert

//                cmd.CommandText = "SELECT NAME from sqlite_master;";
//                var r = cmd.ExecuteReader();

//                while (r.Read())
//                {
//                    ++Actualcount;
//                }
//            }
//            finally
//            {
//                conn.Close();
//                conn.Dispose();
//            }
//            Assert.AreEqual(ExpectedCount, Actualcount);
//            Assert.IsTrue(actualTableNames.Contains(expectedTableNames[0]));
//            Assert.IsTrue(actualTableNames.Contains(expectedTableNames[1]));
//        }

//        [TestMethod]
//        public void TestGetAllRows()
//        {
//            //Arrange
//            int ExpectedCount = 2;
//            string[] expectedTableNames = new string[2] { "test", "test2" };
//            Row[] actualRows = new Row[0];

//            SQLiteConnection conn = new SQLiteConnection(connString);
//            conn.Open();
//            try
//            {
//                var cmd = conn.CreateCommand();
//                cmd.CommandText = "CREATE TABLE test( col1 TEXT)";
//                cmd.ExecuteNonQuery();
//                cmd.CommandText = "INSERT INTO test( col1 ) VALUES('One')";
//                cmd.ExecuteNonQuery();
//                cmd.CommandText = "INSERT INTO test( col1 ) VALUES('Two')";
//                cmd.ExecuteNonQuery();
//                //Act
//                using (SQLiteConnector connector = new SQLiteConnector(connString))
//                {
//                    connector.CurrentTableName = "test";
//                    actualRows = connector.GetAllRows();
//                }
//            }
//            finally
//            {
//                conn.Close();
//                conn.Dispose();
//            }
//            //Assert
//            Assert.AreEqual(ExpectedCount, actualRows.Length);
//            Assert.IsTrue(actualRows.Where(x => x.Id == -1).Count() == 0);
//        }
//    }
//}