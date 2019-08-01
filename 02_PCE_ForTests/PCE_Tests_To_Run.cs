using System;
using NUnit.Framework;

/*
 * This file contains all the tests that will be run.
 * 
 * If you want to find out what a test does (or why it's failing), look in here
 * 
 */

namespace PCE_StarterProject
{
    [TestFixture]
    [Timeout(2000)] // 2 seconds default timeout
    [Description(TestHelpers.TEST_SUITE_DESC)] // tags this as an exercise to be graded...
    public class BST_FindAndRemoveNextSmaller
    {
        BST tree;

        [SetUp] // gets called before each test
        public void Init()
        {
            tree = new BST();
            // pre-load the tree with the following values:
            tree.AddValue(20);
            tree.AddValue(5);
            tree.AddValue(1);
            tree.AddValue(9);
            tree.AddValue(6);
            tree.AddValue(7);
            tree.AddValue(13);
            tree.AddValue(12);
            tree.AddValue(11);
            tree.AddValue(19);
            tree.AddValue(14);
            tree.AddValue(18);
            tree.AddValue(17);
        }

        [Test]
        [Category("BST Find And Remove Next Smallest: Normal Case")]
        public void FARNS_Normal()
        {
            Console.WriteLine("Test #1: Find And Remove Next Smallest Value 'Normal' case: start at 20, expect to remove 11");
            tree.Print();
            int removed = tree.TestFindAndRemoveNextSmallest(20);
            Assert.That(removed == 19, "TEST FAILED: expected to remove 19, but actually removed " + removed);
        }

        [Test]
        [Category("BST Find And Remove Next Smallest: One Step Left")]
        public void FARNS_OneStepToTheLeft()
        {
            Console.WriteLine("Test #1: Find And Remove Next Smallest Value 'One Step To The Left' case: start at 5, expect to remove 1");
            tree.Print();
            int removed = tree.TestFindAndRemoveNextSmallest(5);
            Assert.That(removed == 1, "TEST FAILED: expected to remove 1, but actually removed " + removed);
        }


        [Test]
        [Category("BST Find And Remove Next Smallest: One Step Left With Subtree")]
        public void FARNS_OneStepToTheLeftWithLeftSubtree()
        {
            Console.WriteLine("Test #1: Find And Remove Next Smallest Value 'One Step To The Left, With A Left Subtree' case: start at 13, expect to remove 12");
            tree.Print();
            int removed = tree.TestFindAndRemoveNextSmallest(13);
            Assert.That(removed == 12, "TEST FAILED: expected to remove 12, but actually removed " + removed);
        }

        [Test]
        [Category("BST Find And Remove Next Smallest: One Step Right")]
        public void FARNS_OneStepToTheLeftOneStepRight()
        {
            Console.WriteLine("Test #1: Find And Remove Next Smallest Value 'One Step To The Left, One Step Right' case: start at 9, expect to remove 7");
            tree.Print();
            int removed = tree.TestFindAndRemoveNextSmallest(9);
            Assert.That(removed == 7, "TEST FAILED: expected to remove 7, but actually removed " + removed);
        }


        [Test]
        [Category("BST Find And Remove Next Smallest: One Step Right With Subtree")]
        public void FARNS_OneStepToTheLeftOneStepRightLeftSubtree()
        {
            Console.WriteLine("Test #1: Find And Remove Next Smallest Value 'One Step To The Left, One Step Right (With Left Subtree)' case: start at 19, expect to remove 18");
            tree.Print();
            int removed = tree.TestFindAndRemoveNextSmallest(19);
            Assert.That(removed == 18, "TEST FAILED: expected to remove 7, but actually removed " + removed);
        }
    }

    [TestFixture]
    [Timeout(2000)] // 2 seconds default timeout
    [Description(TestHelpers.TEST_SUITE_DESC)] // tags this as an exercise to be graded...
    public class BST_Remove
    {
        BST tree;

        // add a couple of 'remove non-existent node'


        [SetUp] // gets called before each test
        public void Init()
        {
            tree = new BST();
        }

        [Test]
        [Category("BST.Remove from Empty Tree")]
        public void RemoveFromEmptyTree()
        {
            Console.WriteLine("Test #1: Remove from an empty tree");
            tree.Print();
            tree.Remove(10); // should not crash, nor throw any exceptions
            Console.WriteLine("Passed: 10 not found in an empty tree!");
        }

        [Test]
        [Category("BST.Remove from tree with single node")]
        public void RemoveFromSingleEltTree()
        {
            Console.WriteLine("Test #2: Remove 10 from a tree that has only a single element, 10");

            tree.AddValue(10);
            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            Console.WriteLine("Passed: 10 removed from tree with just 10");
        }

        [Test]
        [Category("BST.Remove from tree with two nodes")]
        public void RemoveFromTree10_20()
        {
            Console.WriteLine("Test #3: Remove 10 from a tree that has only 10, 20");
            tree.AddValue(10);
            tree.AddValue(20);
            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 10, 20");
        }

        [Test]
        [Category("BST.Remove from tree with two nodes")]
        public void RemoveFromTree10_5()
        {
            Console.WriteLine("Test #4: Remove 10 from a tree that has only 10, 5");
            tree.AddValue(10);
            tree.AddValue(5);
            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            tree.Remove(10);
            tree.Find(10);
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 10, 5!");
        }

        [Test]
        [Category("BST.Remove from three element tree (case 2)")]
        public void RemoveFromTree10_20_5()
        {
            Console.WriteLine("Test #5: Remove 10 from a tree that has only 10, 20, 5");
            tree.AddValue(10);
            tree.AddValue(20);
            tree.AddValue(5);
            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 10, 20, 5!");
        }

        [Test]
        [Category("BST.Remove (general)")]
        public void RemoveFromTree10_20_5_6()
        {
            Console.WriteLine("Test #6: Remove 10 from a tree that has only 10, 20, 5, 6");

            tree.AddValue(10);
            tree.AddValue(20);
            tree.AddValue(5);
            tree.AddValue(6);
            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 10, 20, 5, 6!");
        }

        [Test]
        [Category("BST.Remove (general)")]
        public void RemoveFromTree10_20_5_6_7_8()
        {
            Console.WriteLine("Test #7: Remove 10 from a tree that has only 10, 20, 5, 6, 7, 8");

            tree.AddValue(10);
            tree.AddValue(20);
            tree.AddValue(5);
            tree.AddValue(6);
            tree.AddValue(7);
            tree.AddValue(8);
            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 10, 20, 5, 6, 7, 8!");
        }

        [Test]
        [Category("BST.Remove (general)")]
        public void RemoveFromTree100_10_20()
        {
            Console.WriteLine("Test #8: Remove 10 from a tree that has only 100, 10, 20");

            tree.AddValue(100);
            tree.AddValue(10);
            tree.AddValue(20);
            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 100, 10, 20!");
        }

        [Test]
        [Category("BST.Remove (general)")]
        public void RemoveFromTree100_10_5()
        {
            Console.WriteLine("Test #9: Remove 10 from a tree that has only 100, 10, 5");

            tree.AddValue(100);
            tree.AddValue(10);
            tree.AddValue(5);
            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 100, 10, 5!");
        }

        [Test]
        [Category("BST.Remove (general)")]
        public void RemoveFromTree100_10_20_5()
        {
            Console.WriteLine("Test #10: Remove 10 from a tree that has only 100, 10, 20, 5");
            tree.AddValue(100);
            tree.AddValue(10);
            tree.AddValue(20);
            tree.AddValue(5);
            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 100, 10, 5!");
        }

        [Test]
        [Category("BST.Remove (general)")]
        public void RemoveFromTree100_10_20_5_6()
        {
            Console.WriteLine("Test #11: Remove 10 from a tree that has only 100, 10, 20, 5, 6");

            tree.AddValue(100);
            tree.AddValue(10);
            tree.AddValue(20);
            tree.AddValue(5);
            tree.AddValue(6);

            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 100, 10, 5, 6!");
        }

        [Test]
        [Category("BST.Remove (general)")]
        public void RemoveFromTree100_10_20_5_6_7_8()
        {
            Console.WriteLine("Test #12: Remove 10 from a tree that has only 100, 10, 20, 5, 6, 7, 8");
            tree.AddValue(100);
            tree.AddValue(10);
            tree.AddValue(20);
            tree.AddValue(5);
            tree.AddValue(6);
            tree.AddValue(7);
            tree.AddValue(8);

            Assert.That(tree.Find(10) == true, "TEST FAILED: Couldn't find 10 in the tree after adding it");
            tree.Print();
            
            tree.Remove(10);
            Assert.That(tree.Find(10) == false, "TEST FAILED: Found 10 in the tree, despite removing it");
            tree.Print();
            Console.WriteLine("Passed: 10 removed from 100, 10, 5, 6, 7, 8!");
        }
    }

    [TestFixture]
    [Timeout(2000)] // 2 seconds default timeout
    [Description(TestHelpers.TEST_SUITE_DESC)] // tags this as an exercise to be graded...
    public class Test_Partition : TestHelpers
    {
        SearchingAndSorting qsorter;

        [SetUp] // gets called before each test
        public void Init()
        {
            qsorter = new SearchingAndSorting();
        }

        [Test]
        [Category("QS_Partition")]
        public void PartitionTwoEltNoSwap()
        {
            Console.WriteLine("Test #1: Partition an array of two elements, no swapping needed");
            int[] numsStarting = { 1, 2 };
            int[] numsExpected = { 1, 2 };
            Assert.That(qsorter.Partition(numsStarting, 0, 1) == 0, "TEST FAILED: Partition did not put the pivot into the correct location");
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: Partition did not set up arrays as expected");
        }

        [Test]
        [Category("QS_Partition")]
        public void PartitionTwoEltSwap()
        {
            Console.WriteLine("Test #2: Partition an array of two elements, 1 swap needed");
            int[] numsStarting = { 2, 1 };
            int[] numsExpected = { 1, 2 };
            Assert.That(qsorter.Partition(numsStarting, 0, 1) == 1, "TEST FAILED: Partition did not put the pivot into the correct location");
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: Partition did not set up arrays as expected");
        }

        [Test]
        [Category("QS_Partition")]
        public void PartitionThreeElt()
        {
            Console.WriteLine("Test #3: Partition an array of three elements");
            int[] numsStarting = { 2, 1, 3 };
            int[] numsExpected = { 1, 2, 3 };
            Assert.That(qsorter.Partition(numsStarting, 0, 2) == 1, "TEST FAILED: Partition did not put the pivot into the correct location");
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: Partition did not set up arrays as expected");
        }

        [Test]
        [Category("QS_Partition")]
        public void PartitionThreeEltNoSwap()
        {
            Console.WriteLine("Test #4: Partition an array of three elements, no swapping needed");
            int[] numsStarting = { 1, 2, 3 };
            int[] numsExpected = { 1, 2, 3 };
            Assert.That(qsorter.Partition(numsStarting, 0, 2) == 0, "TEST FAILED: Partition did not put the pivot into the correct location");
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: Partition did not set up arrays as expected");
        }

        [Test]
        [Category("QS_Partition")]
        public void PartitionThreeEltSwap()
        {
            Console.WriteLine("Test #5: Partition an array of three elements, 1 swap needed");
            int[] numsStarting = { 3, 2, 1 };
            int[] numsExpected = { 1, 2, 3 };
            Assert.That(qsorter.Partition(numsStarting, 0, 2) == 2, "TEST FAILED: Partition did not put the pivot into the correct location");
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: Partition did not set up arrays as expected");
        }

        [Test]
        [Category("QS_Partition")]
        public void PartitionFiveElt()
        {
            Console.WriteLine("Test #6: Partition an array of five elements, single swap needed");
            int[] numsStarting = { 3, 2, 1, 40, 50 };
            int[] numsExpected = { 1, 2, 3, 40, 50 };
            Assert.That(qsorter.Partition(numsStarting, 0, 2) == 2, "TEST FAILED: Partition did not put the pivot into the correct location");
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: Partition did not set up arrays as expected");
        }

        [Test]
        [Category("QS_Partition")]
        public void PartitionFiveEltLopsided()
        {
            Console.WriteLine("Test #7: Partition an array of five elements, lopsided");
            int[] numsStarting = { 1, 3, 2, 40, 50 };
            int[] numsExpected = { 1, 3, 2, 40, 50 };
            Assert.That(qsorter.Partition(numsStarting, 0, 2) == 0, "TEST FAILED: Partition did not put the pivot into the correct location");
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: Partition did not set up arrays as expected");
        }

        [Test]
        [Category("QS_Partition")]
        public void PartitionFiveEltAgain()
        {
            Console.WriteLine("Test #8: Partition an array of three elements, no swapping needed");
            int[] numsStarting = { 10, 30, 2, 40, 50 };
            int[] numsExpected = { 2, 10, 30, 40, 50 };
            Assert.That(qsorter.Partition(numsStarting, 0, 2) == 1, "TEST FAILED: Partition did not put the pivot into the correct location");
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: Partition did not set up arrays as expected");
        }
    }

    [TestFixture]
    [Timeout(2000)] // 2 seconds default timeout
    [Description(TestHelpers.TEST_SUITE_DESC)] // tags this as an exercise to be graded...
    public class Test_QuickSort : TestHelpers
    {
        SearchingAndSorting qsorter;

        [SetUp] // gets called before each test
        public void Init()
        {
            qsorter = new SearchingAndSorting();
        }

        [Test]
        [Category("QS_QuickSort")]
        public void SortEmptyArray()
        {
            Console.WriteLine("Test: Sort an array of no elements");
            Console.WriteLine("It's hard for this test to fail\nMostly it's here to check that your code won't crash when given an arry of Length == 0.");
            Console.WriteLine("Watch out for Index Out Of Bounds exceptions\n\t(which indicate that you're trying to access an array slot that's too large/small for the array),\nand Null Reference exceptions\n\twhich indicate that you're\trying to use a null reference variable somewhere");
            int[] numsStarting = new int[0];
            int[] numsExpected = new int[0];
            qsorter.QuickSort(numsStarting);
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: QuickSort did not sort the array correctly!");
        }

        [Test]
        [Category("QS_QuickSort")]
        public void Sort_1_Elt_Array()
        {
            Console.WriteLine("Test: Sort an array of no elements");
            int[] numsStarting = { 10 };
            int[] numsExpected = { 10 };
            qsorter.QuickSort(numsStarting);
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: QuickSort did not sort the array correctly!");
        }

        [Test]
        [Category("QS_QuickSort")]
        public void Sort_2_Elt_Array_Sorted()
        {
            Console.WriteLine("Test: Sort an array of no elements");
            int[] numsStarting = { 10, 20 };
            int[] numsExpected = { 10, 20 };
            qsorter.QuickSort(numsStarting);
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: QuickSort did not sort the array correctly!");
        }

        [Test]
        [Category("QS_QuickSort")]
        public void Sort_2_Elt_Array_Unsorted()
        {
            Console.WriteLine("Test: Sort an array of no elements");
            int[] numsStarting = { 20, 10 };
            int[] numsExpected = { 10, 20 };
            qsorter.QuickSort(numsStarting);
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: QuickSort did not sort the array correctly!");
        }

        [Test]
        [Category("QS_QuickSort")]
        public void Sort_4_Elt_Array_Sorted()
        {
            Console.WriteLine("Test: Sort an array of no elements");
            int[] numsStarting = { -3, 0, 7, 777 };
            int[] numsExpected = { -3, 0, 7, 777 };
            qsorter.QuickSort(numsStarting);
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: QuickSort did not sort the array correctly!");
        }

        [Test]
        [Category("QS_QuickSort")]
        public void Sort_4_Elt_Array_Unsorted()
        {
            Console.WriteLine("Test: Sort an array of no elements");
            int[] numsStarting = { 777, 0, -3, 7 };
            int[] numsExpected = { -3, 0, 7, 777 };
            qsorter.QuickSort(numsStarting);
            Assert.That(ArraysTheSame(numsExpected, numsStarting) == true, "TEST FAILED: QuickSort did not sort the array correctly!");
        }

        [TestCase(20)]
        [TestCase(10)]
        [TestCase(7)]
        [Category("QS_QuickSort_BiggerArrays")]
        public void Test_Sorting(int arraySize)
        {
            Console.WriteLine("\n\n\t\tQuickSort Testing\n\n");

            int[] nums = new int[arraySize];
            int[] numsExpected = new int[nums.Length];
            Random r = new Random();

            for (int k = 0; k < arraySize; k++)
            {
                // give each element a random starting value.
                numsExpected[k] = nums[k] = r.Next(arraySize);
            }

            // ensure that the array is not sorted
            // by forcing the first two elements out of order
            if (arraySize >= 2 && numsExpected[0] < numsExpected[1])
            {
                int temp = numsExpected[1];
                nums[1] = numsExpected[1] = numsExpected[0];
                nums[0] = numsExpected[0] = temp;
            }

            Console.WriteLine("BEFORE sorting:");
            PrintArray(nums);

            qsorter.QuickSort(nums);
            Array.Sort(numsExpected);

            Console.WriteLine("AFTER sorting:");
            PrintArray(nums);
            Assert.That(ArraysTheSame(numsExpected, nums) == true, "TEST FAILED: QuickSort did not sort the array correctly!");
        }
    }

}