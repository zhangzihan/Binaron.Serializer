using System;
using System.Threading.Tasks;
using Binaron.Serializer.Tests.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class ValueSerializationTests
    {
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public async ValueTask RootLevelValueTests<TDestination>(object source, TDestination expectation)
        {
            var (dest, dest2) = await Tester.TestRoundTrip2<TDestination>(source);

            Assert.AreEqual(expectation, dest);
            Assert.AreEqual(GetExpectation(source), dest2);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public async ValueTask NonRootLevelValueInClassTests<TSource, TDestination>(TSource source, TDestination expectation)
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<TSource> {RootValue = now, Value = source};
            (var destClass, dynamic dest) = await Tester.TestRoundTrip2<TestClass<TDestination>>(sourceClass);

            Assert.AreEqual(now, destClass.RootValue);
            Assert.AreEqual(expectation, destClass.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public async ValueTask NonRootLevelValueInStructTests<TSource, TDestination>(TSource source, TDestination expectation)
        {
            var now = DateTime.UtcNow;
            var sourceStruct = new TestStruct<TSource> {RootValue = now, Value = source};
            (var destStruct, dynamic dest) = await Tester.TestRoundTrip2<TestStruct<TDestination>>(sourceStruct);

            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(expectation, destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public async ValueTask RootLevelValueTests(object source, object _)
        {
            var (dest, dest2) = await Tester.TestRoundTrip2<object>(source);

            Assert.AreEqual(GetExpectation(source), dest);
            Assert.AreEqual(GetExpectation(source), dest2);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public async ValueTask NonRootLevelValueInClassTests<TSource>(TSource source, object _)
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<TSource> {RootValue = now, Value = source};
            (var destClass, dynamic dest) = await Tester.TestRoundTrip2<TestClass<object>>(sourceClass);

            Assert.AreEqual(now, destClass.RootValue);
            Assert.AreEqual(GetExpectation(source), destClass.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public async ValueTask NonRootLevelValueInStructTests<TSource>(TSource source, object _)
        {
            var now = DateTime.UtcNow;
            var sourceStruct = new TestStruct<TSource> {RootValue = now, Value = source};
            (var destStruct, dynamic dest) = await Tester.TestRoundTrip2<TestStruct<object>>(sourceStruct);

            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(GetExpectation(source), destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public async ValueTask RootLevelNullToValueTests<TDestination>(object _, TDestination __)
        {
            var (dest, dest2) = await Tester.TestRoundTrip2<TDestination>(null);

            Assert.AreEqual(default(TDestination), dest);
            Assert.AreEqual(null, dest2);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public async ValueTask NonRootLevelNullToValueInClassTests<TDestination>(object _, TDestination __)
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<object> {RootValue = now, Value = null};
            (var destClass, dynamic dest) = await Tester.TestRoundTrip2<TestClass<TDestination>>(sourceClass);

            Assert.AreEqual(now, destClass.RootValue);
            Assert.AreEqual(default(TDestination), destClass.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public async ValueTask NonRootLevelNullToValueInStructTests<TDestination>(object _, TDestination __)
        {
            var now = DateTime.UtcNow;
            var sourceStruct = new TestStruct<object> {RootValue = now, Value = null};
            (var destStruct, dynamic dest) = await Tester.TestRoundTrip2<TestStruct<TDestination>>(sourceStruct);

            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(default(TDestination), destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }

        [Test]
        public async ValueTask NestedStructInClassAsObjectTest()
        {
            var sourceClass = new TestClass<object> {Value = new TestStruct<object> {Value = 1}};
            var dest = await Tester.TestRoundTrip<TestClass<TestStruct<object>>>(sourceClass);

            Assert.AreEqual(1, dest.Value.Value);
        }

        [Test]
        public async ValueTask NestedStructInClassAsTypedObjectTest()
        {
            var sourceClass = new TestClass<TestStruct<object>> {Value = new TestStruct<object> {Value = 1}};
            var dest = await Tester.TestRoundTrip(sourceClass);

            Assert.AreEqual(1, dest.Value.Value);
        }

        [Test]
        public async ValueTask RootLevelNullToObjectTest()
        {
            var (dest, dest2) = await Tester.TestRoundTrip2<object>(null);

            Assert.AreEqual(null, dest);
            Assert.AreEqual(null, dest2);
        }

        [Test]
        public async ValueTask NonRootLevelNullToObjectInClassTest()
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<object> {RootValue = now, Value = null};
            (var destClass, dynamic dest) = await Tester.TestRoundTrip2(sourceClass);

            Assert.AreEqual(now, destClass.RootValue);
            Assert.AreEqual(null, destClass.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }

        [Test]
        public async ValueTask NonRootLevelNullToObjectInStructTest()
        {
            var now = DateTime.UtcNow;
            var sourceStruct = new TestStruct<object> {RootValue = now, Value = null};
            (var destStruct, dynamic dest) = await Tester.TestRoundTrip2(sourceStruct);

            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(null, destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }

        [Test]
        public async ValueTask ClassToStructTest()
        {
            var val = new TestClass<int> {Value = 1};
            var dest = await Tester.TestRoundTrip<TestStruct<int>>(val);
            Assert.AreEqual(1, dest.Value);
        }

        [Test]
        public async ValueTask StructToClassTest()
        {
            var val = new TestStruct<int> {Value = 1};
            var dest = await Tester.TestRoundTrip<TestClass<int>>(val);
            Assert.AreEqual(1, dest.Value);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCasesOfValueTypes))]
        public async ValueTask ToNullableTest<TDestination>(object source, TDestination expectation) where TDestination : struct
        {
            var dest = await Tester.TestRoundTrip<TDestination?>(source);
            TDestination? expected;
            if (source.GetType() != typeof(TDestination))
            {
                if (source is char || source.DynamicCast(typeof(TDestination)) == null)
                    expected = null;
                else
                    expected = expectation;
            }
            else
                expected = expectation;
            Assert.AreEqual(expected, dest);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfEnums))]
        public async ValueTask EnumToNullableEnumTest<T>(T val) where T : struct
        {
            var dest = await Tester.TestRoundTrip<T?>(val);
            Assert.AreEqual(val, dest);
        }
        
        [Test]
        public async ValueTask NullableStructTest()
        {
            var val = new TestStruct<int?> {Value = 1};
            var dest = await Tester.TestRoundTrip<TestStruct<int?>?>(val);
            Assert.AreEqual(val.Value, dest?.Value);
        }
        
        [Test]
        public async ValueTask NullableStructNullValueTest()
        {
            var val = new TestStruct<int?>();
            var dest = await Tester.TestRoundTrip<TestStruct<int?>?>(val);
            Assert.AreEqual(null, dest?.Value);
        }

        private static object GetExpectation<TSource>(TSource source) => typeof(TSource).IsEnum ? GetEnumNumeric(source) : source;
        private static object GetExpectation(object source) => source.GetType().IsEnum ? GetEnumNumeric(source) : source;

        private static object GetEnumNumeric(object source)
        {
            switch (Type.GetTypeCode(source.GetType()))
            {
                case TypeCode.SByte:
                    return (sbyte) source;
                case TypeCode.Byte:
                    return (byte) source;
                case TypeCode.Int16:
                    return (short) source;
                case TypeCode.UInt16:
                    return (ushort) source;
                case TypeCode.Int32:
                    return (int) source;
                case TypeCode.UInt32:
                    return (uint) source;
                case TypeCode.Int64:
                    return (long) source;
                case TypeCode.UInt64:
                    return (ulong) source;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source));
            }
        }

        private sealed class TestClass<T>
        {
            public DateTime RootValue { get; set; }
            public T Value { get; set; }
        }

        private struct TestStruct<T>
        {
            public DateTime RootValue { get; set; }
            public T Value { get; set; }
        }
    }
}