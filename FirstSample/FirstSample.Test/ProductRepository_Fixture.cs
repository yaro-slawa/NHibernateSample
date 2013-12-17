using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using FirstSample.Domain;
using NHibernate.Tool.hbm2ddl;
using FirstSample.Repositories;

namespace FirstSample.Test
{
    [TestFixture]
    public class ProductRepository_Fixture
    {
        private ISessionFactory _sessionFactory;
        private Configuration _config;

        private readonly Product[] _products = new[]
            {
                new Product {Name = "Melon", Category = "Fruits"},
                new Product {Name = "Pear", Category = "Fruits"},
                new Product {Name = "Milk", Category = "Beverages"},
                new Product {Name = "Coca Cola", Category = "Beverages"},
                new Product {Name = "Pepsi Cola", Category = "Beverages"}
            };

        private void CreateInitialData()
        {
            using (ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    foreach (var product in _products)
                        session.Save(product);
                    transaction.Commit();
                }
            }
        }

        private bool IsInCollection(Product product, ICollection<Product> fromDb)
        {
            foreach (var item in fromDb)
                if (product.Id == item.Id)
                    return true;
            return false;
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _config = new Configuration();
            _config.Configure();
            _config.AddAssembly(typeof(Product).Assembly);
            _sessionFactory = _config.BuildSessionFactory();
        }

        [SetUp]
        public void SetupContext()
        {
            new SchemaExport(_config).Execute(false, true, false);
            CreateInitialData();
        }

        [Test]
        public void CanAddNewProduct()
        {
            var product = new Product { Name="Apple", Category="Fruits"};
            IProductRepository repository = new ProductRepository();
            repository.Add(product);

            using (ISession session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.IsNotNull(fromDb);
                Assert.AreNotSame(product, fromDb);
                Assert.AreEqual(product.Name, fromDb.Name);
                Assert.AreEqual(product.Category, fromDb.Category);
            }
        }

        [Test]
        public void CanUpdateExsitingProduct()
        {
            var product = _products[0];
            product.Name = "Yellow Pear";
            IProductRepository repository = new ProductRepository();
            repository.Update(product);

            using (ISession session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.AreEqual(product.Name, fromDb.Name);
            }
        }

        [Test]
        public void CanRemoveExsitingProduct()
        {
            var product = _products[0];
            IProductRepository repository = new ProductRepository();
            repository.Remove(product);

            using (ISession session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.IsNull(fromDb);
            }
        }

        [Test]
        public void CanGetExsitingProductById()
        {
            IProductRepository repository = new ProductRepository();
            var fromDb = repository.GetById(_products[1].Id);
            Assert.IsNotNull(fromDb);
            Assert.AreNotSame(_products[1], fromDb);
            Assert.AreEqual(_products[1].Name, fromDb.Name);
        }

        [Test]
        public void CanGetExsitingProductByName()
        {
            IProductRepository repository = new ProductRepository();
            var fromDb = repository.GetByName(_products[1].Name);
            Assert.IsNotNull(fromDb);
            Assert.AreNotSame(_products[1], fromDb);
            Assert.AreEqual(_products[1].Id, fromDb.Id);
        }

        [Test]
        public void CanGetExsitingProductByCategory()
        {
            IProductRepository repository = new ProductRepository();
            var fromDb = repository.GetByCategory("Fruits");
            Assert.AreEqual(2, fromDb.Count);
            Assert.IsTrue(IsInCollection(_products[0], fromDb));
            Assert.IsTrue(IsInCollection(_products[1], fromDb));
        }
    }
}
