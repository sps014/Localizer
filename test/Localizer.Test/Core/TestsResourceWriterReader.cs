using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.Core.Helpers;
using Localizer.Core.Resx;

namespace Localizer.Test.Core
{
    public class TestsResourceWriterReader
    {
        public TestsResourceWriterReader()
        {
            try
            {
                var path = Consts.OUT_PATH;
                Directory.Delete(path, true);
            }
            catch
            {

            }
        }

        [Theory]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("cn-ZH")]
        [InlineData("")]

        internal void CreateResxFile(string culture)
        {
            var path = Consts.OUT_PATH.JoinPath($"Resource1.{culture}.resx");

            if(culture==string.Empty)
            {
                path = Consts.OUT_PATH.JoinPath($"Resource1.resx");
            }

            ResxResourceWriter writer = new ResxResourceWriter(path);

            if(!File.Exists(path))
            {
                Assert.Fail("Can't create file");
            }
        }
        [Theory]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("cn-ZH")]
        [InlineData("")]

        internal void AddResxString(string culture)
        {

            var path = Consts.OUT_PATH.JoinPath($"Resource1.{culture}.resx");

            if (culture == string.Empty)
            {
                path = Consts.OUT_PATH.JoinPath($"Resource1.resx");
            }
            ResxResourceWriter writer = new ResxResourceWriter(path);

            writer.AddResource("k1", "v1","cmt");

            var resxRecords = new ResxResourceReader(path).ToList();

            if(resxRecords.Count!=1)
                Assert.Fail("Already contains strings or zero");

            var kvc = resxRecords.First();
            Assert.Equal("k1", kvc.Key);
            Assert.Equal("v1", kvc.Value);
            Assert.Equal("cmt", kvc.Comment);

        }

        [Theory]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("cn-ZH")]
        [InlineData("")]

        internal void AddResxStrings(string culture,bool deleteFast= true)
        {
            if (deleteFast)
            {
                try
                {
                    Directory.Delete(Consts.OUT_PATH, true);
                }
                catch { }
            }

            var path = Consts.OUT_PATH.JoinPath($"Resource1.{culture}.resx");

            if (culture == string.Empty)
            {
                path = Consts.OUT_PATH.JoinPath($"Resource1.resx");
            }
            ResxResourceWriter writer = new ResxResourceWriter(path);

            writer.AddResource("k2", null, "cmt2");

            var resxRecords = new ResxResourceReader(path).ToList();

            var kvc = resxRecords.FirstOrDefault(x=>x.Key=="k2");

            Assert.Equal("k2", kvc.Key);
            Assert.Equal(string.Empty, kvc.Value);
            Assert.Equal("cmt2", kvc.Comment);

            writer.AddResource("k3", "v3", "cmt3");

            resxRecords = new ResxResourceReader(path).ToList();

            var kvc2 = resxRecords.FirstOrDefault(x => x.Key == "k3");

            Assert.Equal("k3", kvc2.Key);
            Assert.Equal("v3", kvc2.Value);
            Assert.Equal("cmt3", kvc2.Comment);

            Assert.Equal(2,resxRecords.Count);


            writer.AddResource("k4", "v4", null);

            resxRecords = new ResxResourceReader(path).ToList();

            var kvc3 = resxRecords.FirstOrDefault(x => x.Key == "k4");

            Assert.Equal("k4", kvc3.Key);
            Assert.Equal("v4", kvc3.Value);
            Assert.Null(kvc3.Comment);

            Assert.Equal(3, resxRecords.Count);
        }

        [Theory]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("cn-ZH")]
        [InlineData("")]
        internal void ResxStringUpdateInplaceWithAddResource(string culture)
        {

            var path = Consts.OUT_PATH.JoinPath($"Resource1.{culture}.resx");

            if (culture == string.Empty)
            {
                path = Consts.OUT_PATH.JoinPath($"Resource1.resx");
            }
            ResxResourceWriter writer = new ResxResourceWriter(path);


            //this is inplace update
            writer.AddResource("k1", "vz", "cmtz");


            var resxRecords = new ResxResourceReader(path).ToList();

            var kvc = resxRecords.FirstOrDefault(x => x.Key == "k1");

            Assert.Equal("k1", kvc.Key);
            Assert.Equal("vz", kvc.Value);
            Assert.Equal("cmtz", kvc.Comment);

        }

        [Theory]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("cn-ZH")]
        [InlineData("")]
        internal void ResxStringUpdate(string culture)
        {
            AddResxStrings(culture);

            var path = Consts.OUT_PATH.JoinPath($"Resource1.{culture}.resx");

            if (culture == string.Empty)
            {
                path = Consts.OUT_PATH.JoinPath($"Resource1.resx");
            }
            ResxResourceWriter writer = new ResxResourceWriter(path);


            writer.UpdateResource("k2", "kz", "kzz");

            var resxRecords = new ResxResourceReader(path).ToList();

            var kvc = resxRecords.FirstOrDefault(x => x.Key == "k2");

            Assert.Equal("k2", kvc.Key);
            Assert.Equal("kz", kvc.Value);
            Assert.Equal("kzz", kvc.Comment);

            writer.UpdateResource("k2", null, string.Empty);

            resxRecords = new ResxResourceReader(path).ToList();

            kvc = resxRecords.FirstOrDefault(x => x.Key == "k2");

            Assert.Equal("k2", kvc.Key);
            Assert.Equal(string.Empty, kvc.Value);
            Assert.Equal(string.Empty, kvc.Comment);

        }

        [Theory]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("cn-ZH")]
        [InlineData("")]
        internal void ResxStringDelete(string culture)
        {
            AddResxStrings(culture);

            var path = Consts.OUT_PATH.JoinPath($"Resource1.{culture}.resx");

            if (culture == string.Empty)
            {
                path = Consts.OUT_PATH.JoinPath($"Resource1.resx");
            }
            ResxResourceWriter writer = new ResxResourceWriter(path);


            writer.DeleteResource("k2");

            var resxRecords = new ResxResourceReader(path).ToList();

            var kvc = resxRecords.FirstOrDefault(x => x.Key == "k2");

            Assert.Null(kvc);

            writer.DeleteResource("k3");

            resxRecords = new ResxResourceReader(path).ToList();

            kvc = resxRecords.FirstOrDefault(x => x.Key == "k3");

            Assert.Null(kvc);
            Assert.Single(resxRecords);

        }

    }
}
