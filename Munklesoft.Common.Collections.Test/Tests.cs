namespace Munklesoft.Common.Collections.Test
{
    #region Using statements

    using System;
    using System.Linq;
    using System.Threading;

    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class GroupedObservableCollectionTests
    {
        private static readonly string[] smallListOfItems = { "Apple", "Banana", "Pear", "Pineapple" };

        [Test]
        public void Construction_WhenCalledWithNoInitialItems_ShouldAllowForSubsequentItemsToBeAdded()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0]);

            Assert.That(sut.Count, Is.EqualTo(0));

            sut.Add("Hello");

            CollectionAssert.AreEqual(new[] { "Hello" }, sut.EnumerateItems());
        }

        [Test]
        public void Construction_WhenCalledWithInitialItems_ShouldPrePopulateCollection()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);

            CollectionAssert.AreEqual(new[] { 'A', 'B', 'P' }, sut.Keys);
            CollectionAssert.AreEqual(new[] { "Apple", "Banana", "Pear", "Pineapple" }, sut.EnumerateItems());
            CollectionAssert.AreEqual(new[] { "Apple" }, sut[0]);
            CollectionAssert.AreEqual(new[] { "Banana" }, sut[1]);
            CollectionAssert.AreEqual(new[] { "Pear", "Pineapple" }, sut[2]);
        }

        [Test]
        public void ReplaceWith_WhenReplacementIsIdentical_ShouldHaveNoEffect()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);
            var replacementSet = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);

            var result = sut.ReplaceWith(replacementSet, StringComparer.OrdinalIgnoreCase);

            Assert.AreSame(sut, result);
            CollectionAssert.AreEqual(smallListOfItems, sut.EnumerateItems());
        }

        [Test]
        public void ReplaceWith_WhenSoleItemInGroupMissingInNewSet_ShouldRemoveGroup()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);
            var replacementSet = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems.Except(new[] { "Banana" }));

            var result = sut.ReplaceWith(replacementSet, StringComparer.OrdinalIgnoreCase);

            Assert.AreSame(sut, result);
            CollectionAssert.AreEqual(new[] { 'A', 'P' }, sut.Keys);
            CollectionAssert.AreEqual(replacementSet.EnumerateItems(), sut.EnumerateItems());
        }

        [Test]
        public void ReplaceWith_WhenOrderOfItemsInGroupChanged_ShouldReflectChangeInOrder()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);
            var newList = smallListOfItems.ToList();
            newList[2] = smallListOfItems[3];
            newList[3] = smallListOfItems[2];
            var replacementSet = new GroupedObservableCollection<char, string>(s => s[0], newList);

            var result = sut.ReplaceWith(replacementSet, StringComparer.OrdinalIgnoreCase);

            Assert.AreSame(sut, result);
            CollectionAssert.AreEqual(new[] { 'A', 'B', 'P' }, sut.Keys);
            CollectionAssert.AreEqual(replacementSet.EnumerateItems(), sut.EnumerateItems());
        }

        [Test]
        public void ReplaceWith_NewItemAddedToStartOfExistingSet_ShouldBeAddedInCorrectPlace()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);
            var newList = smallListOfItems.ToList();
            newList.Insert(2, "Peach");
            var replacementSet = new GroupedObservableCollection<char, string>(s => s[0], newList);

            var result = sut.ReplaceWith(replacementSet, StringComparer.OrdinalIgnoreCase);

            Assert.AreSame(sut, result);
            CollectionAssert.AreEqual(new[] { 'A', 'B', 'P' }, sut.Keys);
            CollectionAssert.AreEqual(newList, sut.EnumerateItems());
        }

        [Test]
        public void ReplaceWith_NewItemAddedToEndOfExistingSet_ShouldBeAddedInCorrectPlace()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);
            var newList = new[] { "Potato", "Pineapple", "Plant", "Pear" };
            var replacementSet = new GroupedObservableCollection<char, string>(s => s[0], newList);

            var result = sut.ReplaceWith(replacementSet, StringComparer.OrdinalIgnoreCase);

            Assert.AreSame(sut, result);
            CollectionAssert.AreEqual(new[] { 'P' }, sut.Keys);
            CollectionAssert.AreEqual(newList, sut.EnumerateItems());
        }

        [Test]
        public void ReplaceWith_NewItemAddedToMiddleOfExistingSet_ShouldBeAddedInCorrectPlace()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);
            var newList = smallListOfItems.ToList();
            newList.Insert(3, "Pzzz");
            var replacementSet = new GroupedObservableCollection<char, string>(s => s[0], newList);

            var result = sut.ReplaceWith(replacementSet, StringComparer.OrdinalIgnoreCase);

            Assert.AreSame(sut, result);
            CollectionAssert.AreEqual(new[] { 'A', 'B', 'P' }, sut.Keys);
            CollectionAssert.AreEqual(newList, sut.EnumerateItems());
        }

        [Test]
        public void ReplaceWith_WhenSetCompletelyRestructured_ShouldResultInCorrectOrder()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);
            var newList = smallListOfItems.ToList();
            newList.Insert(3, "Pzzz");
            var replacementSet = new GroupedObservableCollection<char, string>(s => s[0], newList);

            var result = sut.ReplaceWith(replacementSet, StringComparer.OrdinalIgnoreCase);

            Assert.AreSame(sut, result);
            CollectionAssert.AreEqual(new[] { 'A', 'B', 'P' }, sut.Keys);
            CollectionAssert.AreEqual(newList, sut.EnumerateItems());
        }

        [Test]
        public void ReplaceWith_WhenNewGroupRequiredAtEndOfCurrentGroups_ShouldResultInNewGroup()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);
            var newList = smallListOfItems.ToList();
            newList.Add("Radish");
            newList.Add("Raspberry");
            var replacementSet = new GroupedObservableCollection<char, string>(s => s[0], newList);

            var result = sut.ReplaceWith(replacementSet, StringComparer.OrdinalIgnoreCase);

            Assert.AreSame(sut, result);
            CollectionAssert.AreEqual(new[] { 'A', 'B', 'P', 'R' }, sut.Keys);
            CollectionAssert.AreEqual(newList, sut.EnumerateItems());
        }

        [Test]
        public void ReplaceWith_WhenNewGroupRequiredAtStartOfCurrentGroups_ShouldResultInNewGroup()
        {
            var sut = new GroupedObservableCollection<char, string>(s => s[0], smallListOfItems);
            var newList = smallListOfItems.ToList();
            newList.Insert(0, "0number");
            newList.Insert(0, "0number2");
            var replacementSet = new GroupedObservableCollection<char, string>(s => s[0], newList);

            var result = sut.ReplaceWith(replacementSet, StringComparer.OrdinalIgnoreCase);

            Assert.AreSame(sut, result);
            CollectionAssert.AreEqual(new[] { '0', 'A', 'B', 'P' }, sut.Keys);
            CollectionAssert.AreEqual(newList, sut.EnumerateItems());
        }
    }
}
