using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.Core.Helpers;
using Localizer.Core.Model;
using Localizer.Core.Resx;

namespace Localizer.Test.Core
{
    public class TestsResouceManager
    {
        ResxManager manager;
        TestsResourceWriterReader readerWriter;
        ResxFileSystemLeafNode leaf;

        public TestsResouceManager()
        {
            Init();
        }
  

        void Init()
        {
            readerWriter = new TestsResourceWriterReader();
            readerWriter.AddResxStrings("");
            readerWriter.AddResxStrings("de",false);
            readerWriter.AddResxStrings("fr", false);

            ResxResourceWriter writer = new ResxResourceWriter(Consts.OUT_PATH.JoinPath("Dir").JoinPath("Properties.resx"));
            File.WriteAllText(Consts.OUT_PATH.JoinPath("Dir").JoinPath("PR0.csproj"), string.Empty);

            manager = new ResxManager(Consts.OUT_PATH);
            manager.BuildCollectionAsync(new CancellationTokenSource()).GetAwaiter().GetResult();
        }

        [Fact]
        public void TestTree()
        {
            Init();
            var tree= manager.Tree;
            Assert.NotNull(tree);
            Assert.Equal("TestOut",tree.Root!.NodeName);
            Assert.False(tree.Root!.IsCSharpProjectDirectory);

            var csprojNode = tree.Root!.ChildrenCollection.FirstOrDefault();
            Assert.True(csprojNode!.IsCSharpProjectDirectory);

            var resource = csprojNode.ChildrenCollection.FirstOrDefault();
            Assert.True(resource!.IsLeafResXFileNode);
            Assert.Equal("Properties", resource!.NodeName);

            leaf = tree.Root.ChildrenCollection.Last() as ResxFileSystemLeafNode;
        }

        [Fact]
        public void TestLeafCulturesEntity()
        {
            if (leaf == null)
                TestTree();

            var cultures = leaf.ResxEntry.Cultures.OrderBy(x=>x).ToList();
            Assert.Equal(3, cultures.Count);
            Assert.Equal("", cultures[0]);
            Assert.Equal("de", cultures[1]);
            Assert.Equal("fr", cultures[2]);

        }

        [Fact]
        public void TestLeafKeyOfEntity()
        {
            if (leaf == null)
                TestTree();
            var keys = leaf.Keys.ToList();
            Assert.Equal(3, keys.Count);
            Assert.Equal("k2", keys[0]);
            Assert.Equal("k3", keys[1]);
            Assert.Equal("k4", keys[2]);
        }

        [Theory]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("")]

        public void TestLeafValueOfEntity(string culture)
        {
            if (leaf == null)
                TestTree();
            var keys = leaf.Keys.ToList();
            Assert.Equal(3, keys.Count);
            Assert.Equal(string.Empty, leaf.ResxEntry.GetValue(keys[0],culture));
            Assert.Equal("v3", leaf.ResxEntry.GetValue(keys[1], culture));
            Assert.Equal("v4", leaf.ResxEntry.GetValue(keys[2], culture));
        }
        [Theory]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("")]

        public void TestLeafSetValueOfEntity(string culture)
        {
            if (leaf == null)
                TestTree();
            var keys = leaf.Keys.ToList();
            Assert.Equal(3, keys.Count);

            leaf.ResxEntry.SetValue(keys[0],"v2", culture);
            leaf.ResxEntry.SetValue(keys[1], null, culture);

            Assert.Equal("v2", leaf.ResxEntry.GetValue(keys[0], culture));
            Assert.Null(leaf.ResxEntry.GetValue(keys[1], culture));
            Assert.Equal("v4", leaf.ResxEntry.GetValue(keys[2], culture));

            leaf.ResxEntry.TryGetFilePath(culture,out string path);

            var recs = new ResxResourceReader(path).ToList();

            var o = recs.First(x => x.Key == "k2");
            Assert.Equal("v2", o.Value);

            o = recs.First(x => x.Key == "k3");
            Assert.Equal(string.Empty, o.Value);

            Assert.Equal(3, recs.Count);
        }
    }
}
