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
        }
        [Fact]
        public void BuildResxManager()
        {
            Init();
            ResxResourceWriter writer = new ResxResourceWriter(Consts.OUT_PATH.JoinPath("Dir").JoinPath("Properties.resx"));
            File.WriteAllText(Consts.OUT_PATH.JoinPath("Dir").JoinPath("PR0.csproj"), string.Empty);

            manager = new ResxManager(Consts.OUT_PATH);
            manager.BuildCollectionAsync(new CancellationTokenSource()).GetAwaiter().GetResult();
            TestTree(manager.Tree);
        }

        private void TestTree(ResxLoadDataTree tree)
        {
            Assert.NotNull(tree);
            Assert.Equal("TestOut",tree.Root!.NodeName);
            Assert.False(tree.Root!.IsCSharpProjectDirectory);

            var csprojNode = tree.Root!.ChildrenCollection.FirstOrDefault();
            Assert.True(csprojNode!.IsCSharpProjectDirectory);

            var resource = csprojNode.ChildrenCollection.FirstOrDefault();
            Assert.True(resource!.IsLeafResXFileNode);
            Assert.Equal("Properties", resource!.NodeName);

            var leaf = tree.Root.ChildrenCollection.Last() as ResxFileSystemLeafNode;
            TestLeafCulturesEntity(leaf!);
        }

        private void TestLeafCulturesEntity(ResxFileSystemLeafNode  leaf)
        {
            var cultures = leaf.ResxEntry.Cultures.OrderBy(x=>x).ToList();
            Assert.Equal(3, cultures.Count);
            Assert.Equal("", cultures[0]);
            Assert.Equal("de", cultures[1]);
            Assert.Equal("fr", cultures[2]);

        }
    }
}
