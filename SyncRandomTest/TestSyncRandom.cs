using System;
using System.Collections.Generic;
using System.Text;
using KHUtils;
namespace SyncRandomTest
{
	class TestSyncRandom
	{
		private SyncRandom m_random = new SyncRandom();
		private SyncRandom m_random2 = new SyncRandom();

		public void PlayToTest()
		{
			m_random.Init();
			string randomStringData = m_random.GetRandomSyncData();

			List<string> stringRandom = new List<string>() { "Test1", "Test2", "Test3", "Test4", "Test5", "Test6", "Test7", "Test8", "Test9", "Test10", "Test11", "Test12" };
			List<string> afterRandomString = m_random.GetRandomNotOverlab<string>(stringRandom);
			Console.WriteLine("random 1 SyncData = " + randomStringData);
			PrintListData<string>(afterRandomString);

			//Console.WriteLine(" 균등 분포 ");

			//for(int i = 0; i < 1000; i++)
			//{
			//	Console.Write(m_random.NormalDistributionRange(0, 1000) + ",");
			//}

			//Console.WriteLine();

			Console.WriteLine("------------------ Random2 Setting ------------------");
			m_random2.Init(randomStringData);
			afterRandomString.Clear();
			afterRandomString = m_random2.GetRandomNotOverlab<string>(stringRandom);
			Console.WriteLine("random 2 SyncData = " + m_random2.GetRandomSyncData());
			PrintListData<string>(afterRandomString);
		}

		private void PrintListData<T>(List<T> datas) where T  : IComparable<T>
		{
			Console.WriteLine();
			foreach(var data in datas)
			{
				Console.Write(data + ",");
			}
			Console.WriteLine();
		}
	}
}
