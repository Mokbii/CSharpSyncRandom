using System;
using System.Collections.Generic;

namespace KHUtils
{
	public class SyncRandom
	{
		public const int StateCount = 16;

		private UInt32[] m_originState;
		private UInt32[] m_state;

		private int m_originIndex;
		private int m_index;

		public SyncRandom()
		{
			m_state = new UInt32[StateCount];
			m_originState = new UInt32[StateCount];
		}
		/// <summary>
		/// 기본 초기화
		/// </summary>
		public void Init()
		{
			System.Random random = new System.Random();

			m_originIndex = m_index = random.Next(0, StateCount - 1);

			for (int i = 0; i < StateCount; i++)
				m_originState[i] = m_state[i] = (UInt32)random.Next(0, 10000);
		}
		/// <summary>
		/// 동기화 정보 초기화(String)
		/// </summary>
		/// <param name="randomSeedString"></param>
		public void Init(string randomSeedString)
		{
			string[] randomStateSeed = randomSeedString.Split(',');
			for (int i = 0; i < StateCount; i++)
			{
				m_originState[i] = m_state[i] = UInt32.Parse(randomStateSeed[i]);
			}
			m_originIndex = m_index = int.Parse(randomStateSeed[randomStateSeed.Length - 1]);
		}
		/// <summary>
		/// 동기화 정보 초기화(실제 값 적용)
		/// </summary>
		/// <param name="states"></param>
		/// <param name="randomSeed"></param>
		public void Init(UInt32[] states, int randomSeed)
		{
			for (int i = 0; i < states.Length; i++)
				m_originState[i] = m_state[i] = states[i];

			m_originIndex = m_index = randomSeed % StateCount;
		}
		/// <summary>
		/// 동기화 정보 관련 string 정보
		/// </summary>
		/// <returns></returns>
		public string GetRandomSyncData()
		{
			string result = string.Empty;
			for (int i = 0; i < StateCount; ++i)
			{
				result += m_originState[i].ToString() + ",";
			}
			result += m_originIndex.ToString();

			return result;
		}
		/// <summary>
		/// 랜덤 함수 
		/// </summary>
		/// <returns></returns>
		public uint Range()
		{
			uint a, b, c, d;

			a = m_state[m_index];
			c = m_state[(m_index + 13) & 15];
			b = a ^ c ^ (a << 16) ^ (c << 15);
			c = m_state[(m_index + 9) & 15];
			c ^= (c >> 11);
			a = m_state[m_index] = b ^ c;
			d = a ^ ((a << 5) & 0xda442d24U);
			m_index = (m_index + 15) & 15;
			a = m_state[m_index];
			m_state[m_index] = a ^ b ^ d ^ (a << 2) ^ (b << 18) ^ (c << 28);

			return m_state[m_index];
		}
		/// <summary>
		/// 랜덤 범위 함수(min 이상 max '미만') 
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public int Range(int min, int max)
		{
			if (max <= min)
				return min;

			return (int)((Range() % (max - min)) + min);
		}
		/// <summary>
		/// 정규 분포 랜덤 범위 함수(min 이상 max '미만')
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public int NormalDistributionRange(int min, int max)
		{
			// 균등분포
			if (max <= min)
				return min;

			return (int)((Range() % (max - min)) + min);
		}
		/// <summary>
		/// 해당 확률(%)이 성공했는지 여부를 랜덤 계산합니다. 
		/// </summary>
		/// <param name="prob"></param>
		/// <returns></returns>
		public bool CheckProb(int prob)
		{
			if (prob > 0)
				return (((Range() % 100) + 1) <= prob);
			else
				return false;
		}
		/// <summary>
		/// 랜덤 인덱스 
		/// </summary>
		/// <param name="prob"></param>
		/// <returns></returns>
		public int RandomIndex(int[] prob)
		{
			int randomSum = 0;
			for (int i = 0; i < prob.Length; ++i)
				randomSum += prob[i];
			int randomValue = (int)Range(0, randomSum);

			for (int i = 0; i < prob.Length; ++i)
			{
				if (randomValue < prob[i])
					return i;

				randomValue -= prob[i];
			}

			return -1;
		}
		/// <summary>
		/// 겹치지 않는 랜덤 인덱스 함수 
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="outRandomIndex"></param>
		public void RandomNotOverlap(int min, int max, out int[] outRandomIndex)
		{
			int size = Math.Abs(max - min);
			outRandomIndex = new int[size];

			for (int i = 0; i < size; i++)
			{
				outRandomIndex[i] = i + min;
			}

			for (int i = 0; i < size; i++)
			{
				int randomIndex = (int)Range(min, max);
				int tempValue = outRandomIndex[i];
				if (randomIndex == i)
					continue;

				outRandomIndex[i] = outRandomIndex[randomIndex];
				outRandomIndex[randomIndex] = tempValue;
			}
		}

		/// <summary>
		/// 겹치지 않는 랜덤 함수 템플릿
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="targetList"></param>
		/// <returns></returns>
		public List<T> GetRandomNotOverlab<T>(List<T> targetList) where T : IComparable<T>
		{
			List<T> result = new List<T>();
			int size = targetList.Count;
			int[] listIndexs = new int[size];

			RandomNotOverlap(0, size, out listIndexs);

			for (int i = 0; i < size; ++i)
			{
				result.Add(targetList[listIndexs[i]]);
			}
			return result;
		}

	}

}
