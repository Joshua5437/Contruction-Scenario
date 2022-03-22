using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovingAverage<T>
{
	/// <summary>
	/// The amount of previous values used (max 40)
	/// </summary>
	int iterations { get; set; }

	/// <summary>
	/// The preference of the newer values (0 - 1) where 0 is only the oldest value and 1 is only the newest value
	/// </summary>
	float preference { get; set; }

	/// <summary>
	/// Add data to the moving average
	/// </summary>
	/// <param name="p_Data">New data</param>
	/// <param name="p_HardSet">Should the value snap?</param>
	void AddData(T p_Data, bool p_HardSet = false);

	/// <summary>
	/// Get the current predected value
	/// </summary>
	/// <returns>Moving average</returns>
	T GetValue();
}

public class MovingAverageVector : IMovingAverage<Vector3>
{
	private int m_CurrentIndex;
	private Vector3[] m_Data = new Vector3[40];

	public int iterations { get; set; } = 20;
	public float preference { get; set; } = .5f;

	public MovingAverageVector(Vector3 p_StartVector)
	{
		for (int i = 0; i < m_Data.Length; i++)
		{
			m_Data[i] = p_StartVector;
		}
	}

	
	public void AddData(Vector3 p_Data, bool p_HardSet)
	{
		if (p_HardSet)
		{
			for (int i = 0; i < m_Data.Length; i++)
			{
				m_Data[i] = p_Data;
			}
		}

		m_CurrentIndex++;
		if (m_CurrentIndex == m_Data.Length)
			m_CurrentIndex = 0;

		m_Data[m_CurrentIndex] = p_Data;
	}

	public Vector3 GetValue()
	{
		Vector3 t_Value = m_Data[GetIndex(m_CurrentIndex - iterations)];

		for (int i = iterations - 1; i >= 0; i--)
		{
			int t_Index = GetIndex(m_CurrentIndex - i);
			t_Value = Vector3.Lerp(t_Value, m_Data[t_Index], preference);
		}

		return t_Value;
	}

	private int GetIndex(int p_Index)
	{
		int t_Index = p_Index;
		if (t_Index < 0)
			t_Index += m_Data.Length;

		return t_Index;
	}
}

public class MovingAverageQuaternion : IMovingAverage<Quaternion>
{
	private int m_CurrentIndex;
	private Quaternion[] m_Data = new Quaternion[40];

	public int iterations { get; set; } = 20;
	public float preference { get; set; } = .5f;

	public MovingAverageQuaternion(Quaternion p_StartQuaternion)
	{
		for (int i = 0; i < m_Data.Length; i++)
		{
			m_Data[i] = p_StartQuaternion;
		}
	}

	public void AddData(Quaternion p_Data, bool p_HardSet)
	{
		if (p_HardSet)
		{
			for (int i = 0; i < m_Data.Length; i++)
			{
				m_Data[i] = p_Data;
			}
		}

		m_CurrentIndex++;
		if (m_CurrentIndex == m_Data.Length)
			m_CurrentIndex = 0;

		m_Data[m_CurrentIndex] = p_Data;
	}

	public Quaternion GetValue()
	{
		Quaternion t_Value = m_Data[GetIndex(m_CurrentIndex - iterations)];
		
		for (int i = iterations - 1; i >= 0; i--)
		{
			int t_Index = GetIndex(m_CurrentIndex - i);
			t_Value = Quaternion.Lerp(t_Value, m_Data[t_Index], preference);
		}
		
		return t_Value;
	}

	private int GetIndex(int p_Index)
	{
		int t_Index = p_Index;
		if (t_Index < 0)
			t_Index += m_Data.Length;

		return t_Index;
	}
}

public class MovingAverageFloat : IMovingAverage<float>
{
	private int m_CurrentIndex;
	private float[] m_Data = new float[40];

	public int iterations { get; set; } = 20;
	public float preference { get; set; } = .5f;

	public void AddData(float p_Data, bool p_HardSet)
	{
		if (p_HardSet)
		{
			for (int i = 0; i < m_Data.Length; i++)
			{
				m_Data[i] = p_Data;
			}
		}

		m_CurrentIndex++;
		if (m_CurrentIndex == m_Data.Length)
			m_CurrentIndex = 0;

		m_Data[m_CurrentIndex] = p_Data;
	}

	public float GetValue()
	{
		float t_Value = m_Data[GetIndex(m_CurrentIndex - iterations)];

		for (int i = iterations - 1; i >= 0; i--)
		{
			int t_Index = GetIndex(m_CurrentIndex - i);
			t_Value = Mathf.Lerp(t_Value, m_Data[t_Index], preference);
		}

		return t_Value;
	}

	private int GetIndex(int p_Index)
	{
		int t_Index = p_Index;
		if (t_Index < 0)
			t_Index += m_Data.Length;

		return t_Index;
	}
}