using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using FirstSample.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirstSample.Test
{
    [TestFixture]
    public class GenerateSchema_Fixture
    {
        [Test]
        public void CanGenerateSchema()
        {
            var cfg = new Configuration();
            cfg.Configure();
            cfg.AddAssembly(typeof(Product).Assembly);
            new SchemaExport(cfg).Execute(false, true, false);
        }
    }
}
