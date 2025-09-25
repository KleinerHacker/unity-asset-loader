using NUnit.Framework;
using UnityAssetLoader.Runtime.Projects.unity_asset_loader.Scripts.Runtime;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UnityAssetLoader.Test.Projects.unity_asset_loader.Scripts.Test
{
    public class TestAssetResourceManager
    {
        [SetUp]
        public void Setup()
        {
            AssetResources.Reset();
        }
        
        [Test]
        public void TestWithoutKeys()
        {
            var count = TestLoader.FromMemory()
                .Load((key, o) => Assert.IsNull(key, "Key is not null for " + o.Identifier));

            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(3, AssetResources.GetAssets<TestObject>().Length);
        }

        [Test]
        public void TestWithSingleKey()
        {
            var count = TestLoader.FromMemory()
                .WithKey("test")
                .Load((key, o) => Assert.AreEqual("test", key, "Key is not 'test' for " + o.Identifier));

            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>("test").Length);
            Assert.AreEqual(3, AssetResources.GetAssets<TestObject>("test").Length);
        }

        [Test]
        public void TestWithMultipleKeys()
        {
            var count = TestLoader.FromMemory()
                .WithKeySelector(o =>
                {
                    if (o == TestLoader.TestObject1) return "test1";
                    if (o == TestLoader.TestObject2) return "test2";
                    if (o == TestLoader.TestObject3) return "test3";

                    Assert.Fail();
                    return null;
                })
                .Load((key, o) =>
                {
                    if (o == TestLoader.TestObject1) Assert.AreEqual("test1", key, "Key is not 'test1' for " + o.Identifier);
                    else if (o == TestLoader.TestObject2) Assert.AreEqual("test2", key, "Key is not 'test2' for " + o.Identifier);
                    else if (o == TestLoader.TestObject3) Assert.AreEqual("test3", key, "Key is not 'test3' for " + o.Identifier);
                    else Assert.Fail();
                });
            
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>("test1").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>("test2").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>("test3").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<TestObject>("test1").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<TestObject>("test2").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<TestObject>("test3").Length);
        }
        
        [Test]
        public void TestWithMultipleKeysOverwrite()
        {
            var count = TestLoader.FromMemory()
                .WithKey("test")
                .WithKeySelector(o =>
                {
                    if (o == TestLoader.TestObject1) return "test1";
                    if (o == TestLoader.TestObject2) return "test2";
                    if (o == TestLoader.TestObject3) return "test3";

                    Assert.Fail();
                    return null;
                })
                .Load((key, o) =>
                {
                    if (o == TestLoader.TestObject1) Assert.AreEqual("test1", key, "Key is not 'test1' for " + o.Identifier);
                    else if (o == TestLoader.TestObject2) Assert.AreEqual("test2", key, "Key is not 'test2' for " + o.Identifier);
                    else if (o == TestLoader.TestObject3) Assert.AreEqual("test3", key, "Key is not 'test3' for " + o.Identifier);
                    else Assert.Fail();
                });
            
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>("test1").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>("test2").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>("test3").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<TestObject>("test1").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<TestObject>("test2").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<TestObject>("test3").Length);
        }

        [Test]
        public void TestWithNoKeysAndConverter()
        {
            var count = TestLoader.FromMemory()
                .WithConverter(x => x.InnerObject)
                .Load((key, o) =>
                {
                    Assert.IsNull(key, "Key is not null for " + o.Identifier);
                    Assert.IsTrue(o.Identifier.StartsWith("InnerObject"), "Identifier is not starting with InnerObject for " + o.Identifier);
                });
            
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(3, AssetResources.GetAssets<InnerObject>().Length);
        }
        
        [Test]
        public void TestWithSingleKeyAndConverter()
        {
            var count = TestLoader.FromMemory()
                .WithConverter(x => x.InnerObject)
                .WithKey("test")
                .Load((key, o) =>
                {
                    Assert.AreEqual(key, "test", "Key is not 'test' for " + o.Identifier);
                    Assert.IsTrue(o.Identifier.StartsWith("InnerObject"), "Identifier is not starting with InnerObject for " + o.Identifier);
                });
            
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test").Length);
            Assert.AreEqual(3, AssetResources.GetAssets<InnerObject>("test").Length);
        }
        
        [Test]
        public void TestWithMultipleKeysAndConverter()
        {
            var count = TestLoader.FromMemory()
                .WithConverter(x => x.InnerObject)
                .WithKeySelector(o =>
                {
                    if (o == TestLoader.InnerObject1) return "test1";
                    if (o == TestLoader.InnerObject2) return "test2";
                    if (o == TestLoader.InnerObject3) return "test3";
                    
                    Assert.Fail();
                    return null;
                })
                .Load((key, o) =>
                {
                    Assert.IsTrue(o.Identifier.StartsWith("InnerObject"), "Identifier is not starting with InnerObject for " + o.Identifier);
                    
                    if (o == TestLoader.InnerObject1) Assert.AreEqual("test1", key, "Key is not 'test1' for " + o.Identifier);
                    else if (o == TestLoader.InnerObject2) Assert.AreEqual("test2", key, "Key is not 'test2' for " + o.Identifier);
                    else if (o == TestLoader.InnerObject3) Assert.AreEqual("test3", key, "Key is not 'test3' for " + o.Identifier);
                    else Assert.Fail();
                });
            
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test1").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test2").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test3").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test1").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test2").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test3").Length);
        }
        
        [Test]
        public void TestWithMultipleKeysOverwriteAndConverter()
        {
            var count = TestLoader.FromMemory()
                .WithConverter(x => x.InnerObject)
                .WithKey("test")
                .WithKeySelector(o =>
                {
                    if (o == TestLoader.InnerObject1) return "test1";
                    if (o == TestLoader.InnerObject2) return "test2";
                    if (o == TestLoader.InnerObject3) return "test3";
                    
                    Assert.Fail();
                    return null;
                })
                .Load((key, o) =>
                {
                    Assert.IsTrue(o.Identifier.StartsWith("InnerObject"), "Identifier is not starting with InnerObject for " + o.Identifier);
                    
                    if (o == TestLoader.InnerObject1) Assert.AreEqual("test1", key, "Key is not 'test1' for " + o.Identifier);
                    else if (o == TestLoader.InnerObject2) Assert.AreEqual("test2", key, "Key is not 'test2' for " + o.Identifier);
                    else if (o == TestLoader.InnerObject3) Assert.AreEqual("test3", key, "Key is not 'test3' for " + o.Identifier);
                    else Assert.Fail();
                });
            
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test1").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test2").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test3").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test1").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test2").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test3").Length);
        }
        
        [Test]
        public void TestWithSingleKeyBeforeConverter()
        {
            var count = TestLoader.FromMemory()
                .WithKey("test")
                .WithConverter(x => x.InnerObject)
                .Load((key, o) =>
                {
                    Assert.AreEqual(key, "test", "Key is not 'test' for " + o.Identifier);
                    Assert.IsTrue(o.Identifier.StartsWith("InnerObject"), "Identifier is not starting with InnerObject for " + o.Identifier);
                });
            
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test").Length);
            Assert.AreEqual(3, AssetResources.GetAssets<InnerObject>("test").Length);
        }
        
        [Test]
        public void TestWithMultipleKeysBeforeConverter()
        {
            var count = TestLoader.FromMemory()
                .WithKeySelector(o =>
                {
                    if (o == TestLoader.TestObject1) return "test1";
                    if (o == TestLoader.TestObject2) return "test2";
                    if (o == TestLoader.TestObject3) return "test3";
                    
                    Assert.Fail();
                    return null;
                })
                .WithConverter(x => x.InnerObject)
                .Load((key, o) =>
                {
                    Assert.IsTrue(o.Identifier.StartsWith("InnerObject"), "Identifier is not starting with InnerObject for " + o.Identifier);
                    
                    if (o == TestLoader.InnerObject1) Assert.AreEqual("test1", key, "Key is not 'test1' for " + o.Identifier);
                    else if (o == TestLoader.InnerObject2) Assert.AreEqual("test2", key, "Key is not 'test2' for " + o.Identifier);
                    else if (o == TestLoader.InnerObject3) Assert.AreEqual("test3", key, "Key is not 'test3' for " + o.Identifier);
                    else Assert.Fail();
                });
            
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test1").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test2").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test3").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test1").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test2").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test3").Length);
        }
        
        [Test]
        public void TestWithMultipleKeysOverwriteBeforeAndAfterConverter()
        {
            var count = TestLoader.FromMemory()
                .WithKeySelector(o =>
                {
                    if (o == TestLoader.TestObject1) return "test1x";
                    if (o == TestLoader.TestObject2) return "test2x";
                    if (o == TestLoader.TestObject3) return "test3x";
                    
                    Assert.Fail();
                    return null;
                })
                .WithConverter(x => x.InnerObject)
                .WithKeySelector(o =>
                {
                    if (o == TestLoader.InnerObject1) return "test1";
                    if (o == TestLoader.InnerObject2) return "test2";
                    if (o == TestLoader.InnerObject3) return "test3";
                    
                    Assert.Fail();
                    return null;
                })
                .Load((key, o) =>
                {
                    Assert.IsTrue(o.Identifier.StartsWith("InnerObject"), "Identifier is not starting with InnerObject for " + o.Identifier);
                    
                    if (o == TestLoader.InnerObject1) Assert.AreEqual("test1", key, "Key is not 'test1' for " + o.Identifier);
                    else if (o == TestLoader.InnerObject2) Assert.AreEqual("test2", key, "Key is not 'test2' for " + o.Identifier);
                    else if (o == TestLoader.InnerObject3) Assert.AreEqual("test3", key, "Key is not 'test3' for " + o.Identifier);
                    else Assert.Fail();
                });
            
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, AssetResources.GetAssets<InnerObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>().Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test1").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test2").Length);
            Assert.AreEqual(0, AssetResources.GetAssets<TestObject>("test3").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test1").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test2").Length);
            Assert.AreEqual(1, AssetResources.GetAssets<InnerObject>("test3").Length);
        }
    }

    public static class TestLoader
    {
        public static readonly InnerObject InnerObject1 = new("InnerObject1");
        public static readonly InnerObject InnerObject2 = new("InnerObject2");
        public static readonly InnerObject InnerObject3 = new("InnerObject3");
        
        public static readonly TestObject TestObject1 = new("TestObject1", InnerObject1);
        public static readonly TestObject TestObject2 = new("TestObject2", InnerObject2);
        public static readonly TestObject TestObject3 = new("TestObject3", InnerObject3);

        public static AssetResourcesLoaderBuilder<TestObject> FromMemory() =>
            new(() => new[] { TestObject1, TestObject2, TestObject3 });
    }

    public sealed class TestObject : Object
    {
        public string Identifier { get; }
        public InnerObject InnerObject { get; }

        public TestObject(string identifier, InnerObject innerObject)
        {
            Identifier = identifier;
            InnerObject = innerObject;
        }
    }

    public sealed class InnerObject : Object
    {
        public string Identifier { get; }

        public InnerObject(string identifier)
        {
            Identifier = identifier;
        }
    }
}